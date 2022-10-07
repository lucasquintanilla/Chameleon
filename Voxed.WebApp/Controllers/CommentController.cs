using Core.Data.Repositories;
using Core.Entities;
using Core.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Voxed.WebApp.Models;
using Voxed.WebApp.Services;
using static Voxed.WebApp.Models.CommentStickyResponse;

namespace Voxed.WebApp.Controllers
{
    public class CommentController : BaseController
    {
        private readonly ILogger<CommentController> _logger;
        private readonly FormateadorService _formateadorService;
        private readonly FileUploadService _fileUploadService;
        private readonly IVoxedRepository _voxedRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly INotificationService _notificationService;

        public CommentController(
            ILogger<CommentController> logger,
            INotificationService notificationService,
            FormateadorService formateadorService,
            FileUploadService fileUploadService,
            IVoxedRepository voxedRepository,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IHttpContextAccessor accessor
            ) : base(accessor, userManager)
        {
            _formateadorService = formateadorService;
            _fileUploadService = fileUploadService;
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
            var response = new CreateCommentResponse(id);

            if (!ModelState.IsValid)
            {
                response.Swal = "Formato de comentario invalido";
                return response;
            }

            try
            {
                if (request.HasEmptyContent())
                {
                    response.Swal = "Debes ingresar un contenido";
                    return response;
                }

                var comment = await ProcessComment(request, id);

                //cambiar para buscar vox con el id de la request
                await _voxedRepository.Comments.Add(comment);
                var vox = await _voxedRepository.Voxs.GetById(comment.VoxID);

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
                response.Swal = e.Message;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                response.Swal = "Hubo un error";
            }

            return response;
        }

        [HttpPost]
        public async Task<CommentStickyResponse> Sticky(CommentStickyRequest request)
        {
            var response = new CommentStickyResponse() { Type = CommentStickyType.CommentSticky };

            try
            {
                var comment = await _voxedRepository.Comments.GetById(request.ContentId);

                if (comment == null)
                {
                    NotFound(response);
                }

                comment.IsSticky = true;
                response.Id = request.ContentId;

                await _voxedRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
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
                ID = Guid.NewGuid(),
                Hash = new Hash().NewHash(7),
                VoxID = GuidConverter.FromShortString(id),
                User = user,
                Content = _formateadorService.Parse(request.Content),
                Style = StyleService.GetRandomCommentStyle(),
                IpAddress = UserIpAddress,
                UserAgent = UserAgent
            };

            await _fileUploadService.ProcessAttachment(request.GetUploadData(), request.File, comment);

            return comment;
        }
    }
}