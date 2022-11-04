using Core.Data.Filters;
using Core.Data.Repositories;
using Core.Entities;
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
        private readonly IUserVoxActionService _userVoxActionService;
        private readonly IContentReportService _contentReportService;

        public VoxController(
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
            _userVoxActionService = userVoxActionService;
            _contentReportService = contentReportService;
        }

        [HttpPost("account/message")]
        public async Task<GlobalMessageResponse> SendGlobalMessage(GlobalMessageFormViewModel form)
        {
            var response = new GlobalMessageResponse();

            if (ModelState.IsValid is false)
            {
                return new GlobalMessageResponse()
                {
                    Status = false,
                    Swal = ModelState.GetErrorMessage()
                };
            }

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
                Id = vox.Id,
                Title = vox.Title,
                Content = vox.Content,
                Hash = GuidConverter.ToShortString(vox.Id),
                UserId = vox.UserId,

                CommentTag = UserTypeDictionary.GetDescription(vox.Owner.UserType).ToLower(),
                CategoryName = vox.Category.Name,
                CategoryShortName = vox.Category.ShortName,
                CategoryThumbnailUrl = vox.Category.Attachment.ThumbnailUrl,
                CommentsAttachmentCount = vox.Comments.Where(x => x.Attachment != null).Count(),
                CommentsCount = vox.Comments.Count(),
                UserName = vox.Owner.UserName,
                UserType = (ViewModels.UserType)(int)vox.Owner.UserType,
                CreatedOn = vox.CreatedOn.DateTime.ToTimeAgo(),

                Media = new MediaViewModel()
                {
                    ThumbnailUrl = vox.Attachment.ThumbnailUrl,
                    Url = vox.Attachment.Url,
                    MediaType = (ViewModels.MediaType)(int)vox.Attachment.Type,
                    ExtensionData = vox.Attachment?.Url.Split('=')[(vox.Attachment?.Url.Split('=').Length - 1).Value]
                },

                IsFavorite = actions.IsFavorite,
                IsFollowed = actions.IsFollowed,
                IsHidden = actions.IsHidden,

                Comments = vox.Comments.OrderByDescending(c => c.IsSticky).ThenByDescending(c => c.CreatedOn).Select(x => new CommentViewModel()
                {
                    ID = x.Id,
                    Content = x.Content,
                    Hash = x.Hash,
                    AvatarColor = x.Style.ToString().ToLower(),
                    AvatarText = UserTypeDictionary.GetDescription(x.Owner.UserType).ToUpper(),
                    IsOp = x.UserId == vox.UserId,
                    Media = x.Attachment == null ? null : new MediaViewModel()
                    {
                        Url = x.Attachment?.Url,
                        MediaType = (ViewModels.MediaType)(int)x.Attachment?.Type,
                        ExtensionData = x.Attachment?.Url.Split('=')[(vox.Attachment?.Url.Split('=').Length - 1).Value],
                        ThumbnailUrl = x.Attachment?.ThumbnailUrl,
                    },
                    IsSticky = x.IsSticky,
                    CreatedOn = x.CreatedOn,
                    Style = x.Style.ToString().ToLower(),
                    User = x.Owner,

                }).ToList(),
            };

            return View(voxViewModel);
        }

        [HttpPost]
        [Route("anon/vox")]
        public async Task<CreateVoxResponse> Create(CreateVoxRequest request)
        {
            if (ModelState.IsValid is false)            
                return CreateVoxResponse.Failure(ModelState.GetErrorMessage());

            try
            {
                var userId = User.GetLoggedInUserId<Guid?>();
                //var user = await _userManager.GetUserAsync(HttpContext.User);
                if (userId == null)
                {
                    var user = await CreateAnonymousUser();
                    await _signInManager.SignInAsync(user, true);
                    userId = user.Id;

                    //TODO: Crear una notificacion para el nuevo usuario anonimo
                }

                var voxId = await _voxService.CreateVox(request, userId.Value);
                return CreateVoxResponse.Succesful(voxId);
            }
            catch (NotImplementedException e)
            {
                return CreateVoxResponse.Failure(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogError(e.StackTrace);
                return CreateVoxResponse.Failure("Error inesperado");
            }
        }

        [HttpPost("vox/list")]
        public async Task<LoadMoreResponse> LoadMore([FromForm] LoadMoreRequest request)
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

        private List<int> GetSubscriptionCategories(LoadMoreRequest request)
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
