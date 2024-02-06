using Core.Data.Filters;
using Core.Data.Repositories;
using Core.DataSources.Devox;
using Core.DataSources.Devox.Helpers;
using Core.Entities;
using Core.Services.Mixers;
using Core.Services.Posts.Models;
using Core.Services.Posts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voxed.WebApp.Constants;
using Voxed.WebApp.Extensions;
using Voxed.WebApp.Mappers;
using Voxed.WebApp.Models;
using Voxed.WebApp.Services;
using Voxed.WebApp.Services.Moderation;
using Voxed.WebApp.ViewModels;
using Core.Extensions;
using Core.Services;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Voxed.WebApp.Controllers;

//[Authorize(Roles = "Administrator")]
[Authorize]
public class VoxController : BaseController
{
    private readonly ILogger<VoxController> _logger;
    private readonly IBlogRepository _blogRepository;
    private readonly SignInManager<User> _signInManager;
    private readonly IPostService _postService;
    private readonly IUserVoxActionService _userVoxActionService;
    private readonly IModerationService _moderationService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMixer _boardMixer;
    private readonly IDevoxDataSource _devoxDataSource;
    private readonly IMapper _mapper;

    public VoxController(
        ILogger<VoxController> logger,
        IBlogRepository voxedRepository,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IHttpContextAccessor accessor,
        IPostService postService,
        IUserVoxActionService userVoxActionService,
        IModerationService moderationService,
        IServiceScopeFactory scopeFactory,
        IMixer boardMixer,
        IDevoxDataSource devoxDataSource,
        IMapper mapper)
        : base(accessor, userManager)
    {
        _blogRepository = voxedRepository;
        _signInManager = signInManager;
        _postService = postService;
        _logger = logger;
        _userVoxActionService = userVoxActionService;
        _moderationService = moderationService;
        _scopeFactory = scopeFactory;
        _boardMixer = boardMixer;
        _devoxDataSource = devoxDataSource;
        _mapper = mapper;
    }

    [HttpGet("vox/{id}")]
    public async Task<IActionResult> Index(string id)
    {
        if (id == null) return BadRequest();
        var voxId = GuidExtension.FromShortString(id);

        var vox = await _blogRepository.Posts.GetById(voxId);
        if (vox == null || vox.State == PostState.Deleted) return NotFound();

        var userId = User.GetUserId();
        var actions = await _userVoxActionService.GetUserVoxActions(voxId, userId);

        var filter = new PostFilter();
        filter.Categories.Add(vox.CategoryId);
        filter.IgnorePostIds = new List<Guid>() { vox.Id };
        var morePosts = await _blogRepository.Posts.GetByFilterAsync(filter);
        var posts = _mapper.Map(morePosts);

        return View(_mapper.Map(vox, actions, posts));
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
            var userId = User.GetUserId();
            if (userId == null)
            {
                response.Swal = $"Debe iniciar sesion";
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
        var response = new ReportResponse();

        try
        {
            await _moderationService.Report(request);

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

    [HttpGet]
    [Route("posts/create")]
    public async Task<IActionResult> CreatePost()
    {
        return View();
    }


    [HttpPost]
    [Route("anon/vox")]
    public async Task<CreateVoxResponse> Create(CreateVoxRequest request)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            _logger.LogWarning(JsonConvert.SerializeObject(request));

            if (ModelState.IsValid is false)
                return CreateVoxResponse.Failure(ModelState.GetErrorMessage());

            var userId = User.GetUserId();
            if (userId == null)
            {
                var user = await CreateAnonymousUser();
                await _signInManager.SignInAsync(user, true);
                userId = user.Id;
            }

            var voxedAttachment = request.GetVoxedAttachment();

            var createPostRequest = new CreatePostRequest()
            {
                UserId = userId.Value,
                IpAddress = UserIpAddress,
                UserAgent = UserAgent,
                Title = GetTitleFromContent(request.Content),
                Content = request.Content,
                CategoryId = request.Niche,
                Extension = voxedAttachment.Extension,
                ExtensionData = voxedAttachment.ExtensionData,
                File = request.File
            };

            var post = await _postService.CreatePost(createPostRequest);

            _ = Task.Run(() => NotifyPostCreated(post.Id));

            stopwatch.Stop();

            _logger.LogWarning($"Post created: {post.Title} after {stopwatch.Elapsed.TotalMilliseconds} milliseconds");

            return CreateVoxResponse.Success(post.Id);
        }
        catch (NotImplementedException e)
        {
            return CreateVoxResponse.Failure(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);

            stopwatch.Stop();
            _logger.LogWarning($"Post creation failed: {request.Content} after {stopwatch.Elapsed.Seconds} seconds");
            return CreateVoxResponse.Failure("Error inesperado");
        }
    }

    private string GetTitleFromContent(string content)
    {
        using var reader = new StringReader(content);
        string title = reader.ReadLine();
        return title;
    }


    [HttpPost("vox/list")]
    public async Task<LoadMoreResponse> LoadMore([FromForm] LoadMoreRequest request)
    {
        if (request.Page == "category-hub")
        {
            var devox = await _devoxDataSource.GetPosts(request.Ignore.Count());
            var devoxPosts = devox.Select(post => new VoxResponse()
            {
                //Hash = post.Id,
                //Status = true,
                //Niche = post.Category.ToString(),
                //Title = post.Title,
                //Comments = post.CommentsCount.ToString(),
                //Extension = string.Empty,
                ////Sticky = vox.IsSticky ? "1" : "0",
                ////CreatedAt = vox.CreatedOn.ToString(),
                //PollOne = string.Empty,
                //PollTwo = string.Empty,
                //Id = post.Id,
                //Slug = "devox",
                //VoxId = post.Id?.ToString(),
                ////New = vox.CreatedOn.IsNew(),
                //ThumbnailUrl = DevoxHelpers.GetThumbnailUrl(post),
                //Category = post.Category.ToString(),
                //Href = "https://devox.me/v/" + post.Filename

                Hash = post.Id ?? post.Filename,
                //Status = true,
                Niche = post.Category.ToString(),
                Title = post.Title,
                Comments = post.CommentsCount.ToString(),
                Extension = string.Empty,
                //Sticky = vox.IsSticky ? "1" : "0",
                //CreatedAt = vox.CreatedOn.ToString(),
                PollOne = string.Empty,
                PollTwo = string.Empty,
                Id = post.Id ?? post.Filename,
                Slug = Core.DataSources.Devox.Constants.Domain,
                VoxId = post.Id?.ToString() ?? post.Filename,
                //New = vox.Date,
                ThumbnailUrl = DevoxHelpers.GetThumbnailUrl(post),
                Category = post.Category.ToString(),
                Href = $"{Core.DataSources.Devox.Constants.VoxEnpoint}{post.Filename}",
                //LastActivityOn = post.LastUpdate
            });

            return new LoadMoreResponse(devoxPosts.ToList());
        }


        // Page: home, category-anm, vox, favorites, hidden, search
        //HttpContext.Request.Cookies.TryGetValue("categoriasFavoritas", out string categoriasActivas);
        var skipList = JsonConvert.DeserializeObject<IEnumerable<string>>(request?.Ignore);
        var skipIdList = skipList.Select(x => GuidExtension.FromShortString(x)).ToList();

        var filter = new PostFilter()
        {
            UserId = User.GetUserId(),
            IgnorePostIds = skipIdList,
            //Categories = (GetSubscriptionCategories(request)).ToList()
        };

        var posts = await _postService.GetByFilter(filter);

        if (!posts.Any())
        {
            filter = new PostFilter()
            {
                UserId = User.GetUserId(),
                IgnorePostIds = Enumerable.Empty<Guid>(),
                //Categories = (GetSubscriptionCategories(request)).ToList()
            };

            posts = await _postService.GetByFilter(filter);
        }

        return new LoadMoreResponse(_mapper.Map(posts));
       
    }
    
    private IEnumerable<int> GetSubscriptionCategories(LoadMoreRequest request)
    {
        try
        {
            var subscriptions = JsonConvert.DeserializeObject<List<string>>(request?.Suscriptions);

            if (subscriptions == null)
                return Categories.DefaultCategories;

            return subscriptions.Select(x => int.Parse(x));

        }
        catch (Exception)
        {
            return Categories.DefaultCategories;
        }
    }

    private async Task NotifyPostCreated(Guid voxId)
    {
        using var scope = _scopeFactory.CreateScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        await notificationService.NotifyPostCreated(voxId);
    }
}
