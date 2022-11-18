using Core.Data.Repositories;
using Core.Entities;
using Core.Extensions;
using Core.Services.AttachmentServices;
using Core.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Voxed.WebApp.Extensions;
using Voxed.WebApp.Models;
using Voxed.WebApp.Services;
using static Voxed.WebApp.Models.CommentStickyResponse;

namespace Voxed.WebApp.Controllers
{
    public class CommentController : BaseController
    {
        private readonly ILogger<CommentController> _logger;
        private readonly IContentFormatterService _formatter;
        private readonly IAttachmentService _attachmentService;
        private readonly IVoxedRepository _voxedRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly INotificationService _notificationService;
        private readonly IServiceScopeFactory _scopeFactory;

        public CommentController(
            ILogger<CommentController> logger,
            INotificationService notificationService,
            IContentFormatterService formatter,
            IAttachmentService attachmentService,
            IVoxedRepository voxedRepository,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IHttpContextAccessor accessor,
            IServiceScopeFactory scopeFactory) 
            : base(accessor, userManager)
        {
            _formatter = formatter;
            _attachmentService = attachmentService;
            _voxedRepository = voxedRepository;
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
            _notificationService = notificationService;
            _scopeFactory = scopeFactory;
        }


        [HttpPost]
        [Route("comment/nuevo/{id}")]
        public async Task<CreateCommentResponse> Create([FromForm] CreateCommentRequest request, [FromRoute] string id)
        {
            _logger.LogWarning($"{nameof(CreateCommentRequest)} received.");
            _logger.LogWarning(JsonConvert.SerializeObject(request));

            if (ModelState.IsValid is false)
            {
                var errorMessage = ModelState.GetErrorMessage();
                _logger.LogWarning($"Request received is not valid. Message: {errorMessage}");
                return CreateCommentResponse.Failure(errorMessage);
            }

            try
            {
                if (request.HasEmptyContent())
                {
                    _logger.LogWarning($"Comment request received has empty content");
                    return CreateCommentResponse.Failure("Debes ingresar un contenido");
                }

                var comment = await ProcessComment(request, id);

                //cambiar para buscar vox con el id de la request
                await _voxedRepository.Comments.Add(comment);
                var vox = await _voxedRepository.Voxs.GetById(comment.VoxId);

                if (comment.Content != null && !comment.Content.ToLower().Contains("&gt;hide"))
                {
                    vox.Bump = DateTimeOffset.Now;
                }

                await _voxedRepository.SaveChangesAsync();

                _ = Task.Run(() => NotifyCommentCreated(vox, comment, request));

                return CreateCommentResponse.Success(comment.Hash);
            }
            catch (NotImplementedException e)
            {
                _logger.LogWarning(e.Message);
                return CreateCommentResponse.Failure(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogError(e.StackTrace);
                return CreateCommentResponse.Failure("Hubo un error");
            }
        }

        private async Task NotifyCommentCreated(Vox vox, Comment comment, CreateCommentRequest request)
        {
            using var scope = _scopeFactory.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            await notificationService.NotifyCommentCreated(comment, vox, request);
            await notificationService.ManageNotifications(vox, comment);
        }

        [HttpPost]
        public async Task<CommentStickyResponse> Sticky(CommentStickyRequest request)
        {
            var response = new CommentStickyResponse() { Type = CommentStickyType.CommentSticky };

            try
            {
                var comment = await _voxedRepository.Comments.GetById(request.ContentId);

                if (comment == null) NotFound(response);

                comment.IsSticky = true;
                response.Id = request.ContentId;

                await _voxedRepository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogError(e.StackTrace);
            }

            return response;
        }

        private async Task<Comment> ProcessComment(CreateCommentRequest request, string id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                user = await CreateAnonymousUser();

                await _signInManager.SignInAsync(user, true);
                //Crear una notificacion para el nuevo usuario anonimo
            }

            var comment = new Comment()
            {
                Hash = new Hash().NewHash(7),
                VoxId = GuidExtension.FromShortString(id),
                Owner = user,
                Content = _formatter.Format(request.Content),
                Style = StyleService.GetRandomCommentStyle(),
                IpAddress = UserIpAddress,
                UserAgent = UserAgent,
                Attachment = request.HasAttachment() ? await _attachmentService.ProcessAttachment(request.GetVoxedAttachment(), request.File) : null,
            };

            return comment;
        }
    }
}