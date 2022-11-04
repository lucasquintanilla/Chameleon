using Core.Data.Repositories;
using Core.Entities;
using Core.Services.AttachmentServices;
using Core.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public CommentController(
            ILogger<CommentController> logger,
            INotificationService notificationService,
            IContentFormatterService formatter,
            IAttachmentService attachmentService,
            IVoxedRepository voxedRepository,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IHttpContextAccessor accessor
            ) : base(accessor, userManager)
        {
            _formatter = formatter;
            _attachmentService = attachmentService;
            _voxedRepository = voxedRepository;
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
            _notificationService = notificationService;
        }


        [HttpPost]
        [Route("comment/nuevo/{id}")]
        public async Task<CreateCommentResponse> Create([FromForm] CreateCommentRequest request, [FromRoute] string id)
        {
            _logger.LogWarning($"{nameof(CreateCommentRequest)} received.");
            _logger.LogWarning(JsonConvert.SerializeObject(request));

            var response = new CreateCommentResponse(id);

            if (ModelState.IsValid is false)
            {
                var errorMessage = ModelState.GetErrorMessage();

                _logger.LogWarning($"Request received is not valid. Message: {errorMessage}");
                response.Swal = errorMessage;
                return response;
            }

            try
            {
                if (request.HasEmptyContent())
                {
                    _logger.LogWarning($"Request received has empty content");
                    response.Swal = "Debes ingresar un contenido";
                    return response;
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

                await _notificationService.ManageNotifications(vox, comment);
                await _notificationService.SendBoardUpdate(comment, vox, request);

                response.Hash = comment.Hash;
                response.Status = true;
            }
            catch (NotImplementedException e)
            {
                _logger.LogWarning(e.Message);
                response.Swal = e.Message;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogError(e.StackTrace);
                response.Swal = "Hubo un error";
            }

            _logger.LogWarning($"{nameof(CreateCommentResponse)}: {JsonConvert.SerializeObject(response)}");
            return response;
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
                VoxId = GuidConverter.FromShortString(id),
                Owner = user,
                Content = _formatter.Format(request.Content),
                Style = StyleService.GetRandomCommentStyle(),
                IpAddress = UserIpAddress,
                UserAgent = UserAgent,
                Attachment = await _attachmentService.ProcessAttachment(request.GetVoxedAttachment(), request.File)
            };

            return comment;
        }
    }
}