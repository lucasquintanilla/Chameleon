﻿using Core.Data.Repositories;
using Core.Entities;
using Core.Services.Telegram;
using Core.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voxed.WebApp.Extensions;
using Voxed.WebApp.Hubs;
using Voxed.WebApp.Mappers;
using Voxed.WebApp.Models;
using Voxed.WebApp.Services;
using Voxed.WebApp.ViewModels;

namespace Voxed.WebApp.Controllers
{
    public class VoxController : BaseController
    {
        private readonly FileUploadService _fileUploadService;
        private readonly IVoxedRepository _voxedRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly FormateadorService _formatterService;
        private readonly IHubContext<VoxedHub, INotificationHub> _notificationHub;
        private readonly ILogger<VoxController> _logger;
        private readonly TelegramService _telegramService;
        private readonly int[] _excludedCategories = { 2, 3 };
        private readonly int[] _defaultCategories = { 1, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 30, 16, 14, 13, 12, 11, 10, 9, 8, 15, 7, 31, 6, 5, 4 };

        public VoxController(
            TelegramService telegramService,
            ILogger<VoxController> logger,
            FileUploadService fileUploadService,
            IVoxedRepository voxedRepository,
            UserManager<User> userManager,
            FormateadorService formatterService,
            IHubContext<VoxedHub, INotificationHub> notificationHub,
            SignInManager<User> signInManager,
            IHttpContextAccessor accessor
            ) : base(accessor)
        {
            _fileUploadService = fileUploadService;
            _voxedRepository = voxedRepository;
            _userManager = userManager;
            _formatterService = formatterService;
            _notificationHub = notificationHub;
            _signInManager = signInManager;
            _logger = logger;
            _telegramService = telegramService;
        }

        [HttpPost("account/message")]
        public async Task<GlobalMessageResponse> SendGlobalMessage(GlobalMessageFormViewModel form)
        {
            var response = new GlobalMessageResponse();

            try
            {
                var message = new GlobalMessage() { Content = form.Content, };
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
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    response.Swal = $"Debe iniciar sesion o crear una comentando o creando un post";
                    return response;
                }

                var action = await _voxedRepository.UserVoxActions.GetByUserIdVoxId(user.Id, request.ContentId);
                if (action == null)
                {
                    action = new UserVoxAction()
                    {
                        UserId = user.Id,
                        VoxId = request.ContentId
                    };

                    SetAction(action, id);
                    await _voxedRepository.UserVoxActions.Add(action);
                }
                else
                {
                    SetAction(action, id);
                }

                await _voxedRepository.SaveChangesAsync();

                response.Status = true;
                response.Swal = $"Accion {id} aplicada con exito";
                response.Action = "create";
            }
            catch (Exception e)
            {
                response.Status = false;
                response.Swal = $"Hubo un error al aplicar la accion {id}";
            }

            return response;
        }

        private void SetAction(UserVoxAction userVoxAction, string action)
        {
            switch (action)
            {
                case Actions.Hide:
                    userVoxAction.IsHidden = true;
                    break;
                case Actions.Favorite:
                    userVoxAction.IsFavorite = true;
                    break;
                case Actions.Follow:
                    userVoxAction.IsFollowed = true;
                    break;
                default:
                    break;
            }
        }

        [HttpPost("report")]
        public async Task<ReportResponse> Report(ReportRequest request)
        {
            var response = new ReportResponse();

            try
            {
                string message = null;

                switch (request.ContentType)
                {
                    case 0:
                        //var comment = await _voxedRepository.Comments.GetById(new Guid(request.ContentId));
                        var comment = await _voxedRepository.Comments.GetByHash(request.ContentId);
                        message = $"NUEVA DENUNCIA \n Reason: {request.Reason}. \n https://voxed.club/vox/{GuidConverter.ToShortString(comment.VoxID)}#{comment.Hash}";
                        break;

                    case 1:
                        message = $"NUEVA DENUNCIA \n Reason: {request.Reason}. \n https://voxed.club/vox/{GuidConverter.ToShortString(new Guid(request.ContentId))}";
                        break;
                }

                await _telegramService.SendMessage(message);

                response.Status = true;
                response.Swal = "Gracias por enviarnos tu denuncia";

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                response.Status = false;
                response.Swal = "Hubo un error al enviar tu denuncia";
            }

            return response;
        }

        [HttpGet("vox/{hash}")]
        public async Task<IActionResult> Details(string hash)
        {
            if (hash == null) return NotFound();

            var voxId = GuidConverter.FromShortString(hash);

            var vox = await _voxedRepository.Voxs.GetById(voxId);

            if (vox == null) return NotFound();

            if (vox.State == VoxState.Deleted) return NotFound();

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
                Comments = vox.Comments,
                Media = new MediaViewModel()
                {
                    ThumbnailUrl = vox.Media.ThumbnailUrl,
                    Url = vox.Media.Url,
                    MediaType = (ViewModels.MediaType)(int)vox.Media.MediaType,
                    ExtensionData = vox.Media?.Url.Split('=')[(vox.Media?.Url.Split('=').Length - 1).Value]
                }

                //Comments = vox.Comments.Select(x => new CommentViewModel()
                //{
                //    Id = x.ID,
                //    Content = x.Content,
                //    Hash = x.Hash,
                //    AvatarColor = x.Style.ToString().ToLower(),
                //    AvatarText = Core.Shared.UserTypeDictionary.GetDescription(x.User.UserType).ToUpper(),
                //    IsOp = x.
                //}).ToList(),
            };

            return View(voxViewModel);
        }

        [HttpGet("search/{value}")]
        public async Task<IActionResult> Search(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return BadRequest();

            var voxs = await _voxedRepository.Voxs.SearchAsync(value);
            return View(VoxedMapper.Map(voxs));
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
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    user = await CreateAnonymousUser();
                    await _signInManager.SignInAsync(user, true);

                    //TODO: Crear una notificacion para el nuevo usuario anonimo
                }

                var vox = new Vox()
                {
                    ID = Guid.NewGuid(),
                    State = VoxState.Normal,
                    User = user,
                    Hash = new Hash().NewHash(),
                    Title = request.Title,
                    Content = _formatterService.Parse(request.Content),
                    CategoryID = request.Niche,
                    IpAddress = UserIpAddress,
                    UserAgent = UserAgent
                };

                if (request.PollOne != null && request.PollTwo != null)
                {
                    vox.Poll = new Poll()
                    {
                        OptionADescription = request.PollOne,
                        OptionBDescription = request.PollTwo,
                    };
                }

                await _fileUploadService.ProcessAttachment(request.GetUploadData(), request.File, vox);

                await _voxedRepository.Voxs.Add(vox);
                await _voxedRepository.SaveChangesAsync();

                //disparo notificacion del vox
                vox = await _voxedRepository.Voxs.GetById(vox.ID); // Ver si se puede remover

                if (!_excludedCategories.Contains(vox.CategoryID))
                {
                    //var voxToHub = ConvertoToVoxResponse(vox);
                    var voxToHub = VoxedMapper.Map(vox);
                    await _notificationHub.Clients.All.Vox(voxToHub);
                }

                response.VoxHash = GuidConverter.ToShortString(vox.ID);
                response.Status = true;
            }
            catch (NotImplementedException e)
            {
                response.Swal = e.Message;
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
            //HttpContext.Request.Cookies.TryGetValue("categoriasFavoritas", out string categoriasActivas);
            var skipList = JsonConvert.DeserializeObject<IEnumerable<string>>(request?.Ignore);
            var skipIdList = skipList.Select(x => GuidConverter.FromShortString(x)).ToList();
            //var skipIdList = JsonConvert.DeserializeObject<List<Guid>>(request?.Ignore);

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                var userVoxActions = await _voxedRepository.UserVoxActions.Find(x => x.UserId == user.Id && x.IsHidden);
                var hiddenVoxIds = userVoxActions.Select(x => x.VoxId).ToList();   
                skipIdList.AddRange(hiddenVoxIds);
            }

            
            var lastVox = await _voxedRepository.Voxs.GetLastVoxBump(skipIdList);

            var voxs = await _voxedRepository.Voxs.GetLastestAsync(skipIdList, lastVox.Bump, GetSubscriptionCategories(request));

            var response = new ListResponse(VoxedMapper.Map(voxs));
            return response;
        }

        private ICollection<int> GetSubscriptionCategories(ListRequest request)
        {
            try
            {
                var subscriptions = JsonConvert.DeserializeObject<IEnumerable<string>>(request?.Suscriptions);

                if (subscriptions == null)
                {
                    return _defaultCategories;
                }

                return subscriptions.Select(x => int.Parse(x)).ToList();

            }
            catch (Exception e)
            {
                return _defaultCategories;
            }
        }

        private async Task<User> CreateAnonymousUser()
        {
            var user = new User
            {
                UserName = UserNameGenerator.NewAnonymousUserName(),
                EmailConfirmed = true,
                UserType = Core.Entities.UserType.AnonymousAccount,
                IpAddress = UserIpAddress.ToString(),
                UserAgent = UserAgent,
                Token = TokenGenerator.NewToken()
            };

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                return user;
            }

            throw new Exception("Usuario no pudo ser creado");
        }
    }

    public class Actions
    {
        public const string Hide = "hide";
        public const string Favorite = "follow";
        public const string Follow = "favorite";
    }
}
