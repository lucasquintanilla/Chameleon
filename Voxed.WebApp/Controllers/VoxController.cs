using Core.Data.Filters;
using Core.Data.Repositories;
using Core.Entities;
using Core.Services.Telegram;
using Core.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voxed.WebApp.Constants;
using Voxed.WebApp.Extensions;
using Voxed.WebApp.Models;
using Voxed.WebApp.Services;
using Voxed.WebApp.ViewModels;

namespace Voxed.WebApp.Controllers
{
    public class VoxController : BaseController
    {
        private readonly ILogger<VoxController> _logger;
        private readonly IVoxedRepository _voxedRepository;
        private readonly SignInManager<User> _signInManager;
        private readonly IVoxService _voxService;
        private readonly TelegramService _telegramService;
        private readonly IUserVoxActionService _userVoxActionService;
        private readonly IContentReportService _contentReportService;

        public VoxController(
            TelegramService telegramService,
            ILogger<VoxController> logger,
            IVoxedRepository voxedRepository,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IHttpContextAccessor accessor,
            IVoxService voxService,
            IUserVoxActionService userVoxActionService,
            IContentReportService contentReportService) 
            : base(accessor, userManager)
        {
            _voxedRepository = voxedRepository;
            _signInManager = signInManager;
            _voxService = voxService;
            _logger = logger;
            _telegramService = telegramService;
            _userVoxActionService = userVoxActionService;
            _contentReportService = contentReportService;
        }

        [HttpPost("account/message")]
        public async Task<GlobalMessageResponse> SendGlobalMessage(GlobalMessageFormViewModel form)
        {
            var response = new GlobalMessageResponse();

            try
            {
                var message = new GlobalMessage() { Content = form.Content, UserIpAddress = UserIpAddress, UserAgent = UserAgent };

                switch (form.Type)
                {
                    case GlobalMessageFormViewModel.GlobalMessageType.TenMinutes:
                        message.DueDate = DateTime.UtcNow.AddMinutes(10);
                        message.Tokens = 50;
                        break;
                    case GlobalMessageFormViewModel.GlobalMessageType.ThirtyMinutes:
                        message.DueDate = DateTime.UtcNow.AddMinutes(30);
                        message.Tokens = 30;
                        break;
                    case GlobalMessageFormViewModel.GlobalMessageType.OneHour:
                        message.DueDate = DateTime.UtcNow.AddMinutes(60);
                        message.Tokens = 300;
                        break;
                    case GlobalMessageFormViewModel.GlobalMessageType.TwoHours:
                        message.DueDate = DateTime.UtcNow.AddHours(2);
                        message.Tokens = 500;
                        break;
                    case GlobalMessageFormViewModel.GlobalMessageType.FourHours:
                        message.DueDate = DateTime.UtcNow.AddHours(4);
                        message.Tokens = 1000;
                        break;
                    case GlobalMessageFormViewModel.GlobalMessageType.TwentyFourHours:
                        message.DueDate = DateTime.UtcNow.AddHours(24);
                        message.Tokens = 4000;
                        break;
                    default:
                        throw new Exception("Tiempo de mensaje global invalido");
                }

                GlobalMessageService.AddMessage(message);

                response.Status = true;
                response.Swal = $"Mensaje global agregado!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response.Swal = $"Hubo un error al agregar tu mensaje global";
            }

            return await Task.FromResult(response);
        }

        [HttpPost("meta/{id}/toggle")]
        public async Task<FavoriteResponse> Toggle(FavoriteRequest request, string id)
        {
            var response = new FavoriteResponse();

            try
            {
                var userId = User.GetLoggedInUserId<Guid?>();
                if (userId == null)
                {
                    response.Swal = $"Debe iniciar sesion o crear un post o un comentario";
                    return response;
                }

                response.Action = await _userVoxActionService.ManageUserVoxAction(userId.Value, request.ContentId, id);
                response.Swal = $"Accion {id} aplicada con exito";
                response.Status = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.StackTrace);
                response.Status = false;
                response.Swal = $"Hubo un error al aplicar la accion {id}";
            }

            return response;
        }        

        [HttpPost("report")]
        public async Task<ReportResponse> Report(ReportRequest request)
        {
            return await _contentReportService.Report(request);
        }

        [HttpGet("vox/{hash}")]
        public async Task<IActionResult> Details(string hash)
        {
            if (hash == null) return BadRequest();

            var voxId = GuidConverter.FromShortString(hash);

            var vox = await _voxedRepository.Voxs.GetById(voxId);

            if (vox == null || vox.State == VoxState.Deleted) return NotFound();

            var userId = User.GetLoggedInUserId<Guid?>();

            var actions = await _userVoxActionService.GetUserVoxActions(voxId, userId);

            var voxViewModel = new VoxDetailViewModel()
            {
                Id = vox.ID,
                Title = vox.Title,
                Content = vox.Content,
                Hash = vox.Hash,
                UserId = vox.UserID,

                CommentTag = UserTypeDictionary.GetDescription(vox.User.UserType).ToLower(),
                CategoryName = vox.Category.Name,
                CategoryShortName = vox.Category.ShortName,
                CategoryThumbnailUrl = vox.Category.Media.ThumbnailUrl,
                CommentsAttachmentCount = vox.Comments.Where(x => x.Media != null).Count(),
                CommentsCount = vox.Comments.Count(),
                UserName = vox.User.UserName,
                UserType = (ViewModels.UserType)(int)vox.User.UserType,
                CreatedOn = vox.CreatedOn.DateTime.ToTimeAgo(),

                Media = new MediaViewModel()
                {
                    ThumbnailUrl = vox.Media.ThumbnailUrl,
                    Url = vox.Media.Url,
                    MediaType = (ViewModels.MediaType)(int)vox.Media.MediaType,
                    ExtensionData = vox.Media?.Url.Split('=')[(vox.Media?.Url.Split('=').Length - 1).Value]
                },

                IsFavorite = actions.IsFavorite,
                IsFollowed = actions.IsFollowed,
                IsHidden = actions.IsHidden,

                Comments = vox.Comments.OrderByDescending(x => x.IsSticky).ThenByDescending(x => x.CreatedOn).Select(x => new CommentViewModel()
                {
                    ID = x.ID,
                    Content = x.Content,
                    Hash = x.Hash,
                    AvatarColor = x.Style.ToString().ToLower(),
                    AvatarText = UserTypeDictionary.GetDescription(x.User.UserType).ToUpper(),
                    IsOp = x.UserID == vox.UserID,
                    Media = x.Media == null ? null : new MediaViewModel()
                    {
                        Url = x.Media?.Url,
                        MediaType = (ViewModels.MediaType)(int)x.Media?.MediaType,
                        ExtensionData = x.Media?.Url.Split('=')[(vox.Media?.Url.Split('=').Length - 1).Value],
                        ThumbnailUrl = x.Media?.ThumbnailUrl,
                    },
                    IsSticky = x.IsSticky,
                    CreatedOn = x.CreatedOn,
                    Style = x.Style.ToString().ToLower(),
                    User = x.User,

                }).ToList(),
            };

            return View(voxViewModel);
        }

        [HttpPost]
        [Route("anon/vox")]
        public async Task<CreateVoxResponse> Create(CreateVoxRequest request)
        {
            var response = new CreateVoxResponse();

            if (!ModelState.IsValid)
            {
                response.Swal = "error";
                return response;
            }

            try
            {
                //await _telegramService.UploadFile(request.File.OpenReadStream());

                var userId = User.GetLoggedInUserId<Guid?>();
                //var user = await _userManager.GetUserAsync(HttpContext.User);
                if (userId == null)
                {
                    var user = await CreateAnonymousUser();
                    await _signInManager.SignInAsync(user, true);
                    userId = user.Id;

                    //TODO: Crear una notificacion para el nuevo usuario anonimo
                }

                return await _voxService.CreateVox(request, userId.Value);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                response.Swal = "error";
            }

            return response;
        }

        [HttpPost]
        public async Task<ListResponse> List([FromForm] ListRequest request)
        {
            // Page: home, category-anm, vox, favorites, hidden, search
            //HttpContext.Request.Cookies.TryGetValue("categoriasFavoritas", out string categoriasActivas);
            var skipList = JsonConvert.DeserializeObject<IEnumerable<string>>(request?.Ignore);
            var skipIdList = skipList.Select(x => GuidConverter.FromShortString(x)).ToList();

            var filter = new VoxFilter()
            {
                UserId = User.GetLoggedInUserId<Guid?>(),
                IgnoreVoxIds = skipIdList,
                Categories = GetSubscriptionCategories(request)
            };

            return await _voxService.GetByFilter(filter);
        }

        private List<int> GetSubscriptionCategories(ListRequest request)
        {
            try
            {
                var subscriptions = JsonConvert.DeserializeObject<List<string>>(request?.Suscriptions);

                if (subscriptions == null)
                {
                    return Categories.DefaultCategories.ToList();
                }

                return subscriptions.Select(x => int.Parse(x)).ToList();

            }
            catch (Exception)
            {
                return Categories.DefaultCategories.ToList();
            }
        }
    }
}
