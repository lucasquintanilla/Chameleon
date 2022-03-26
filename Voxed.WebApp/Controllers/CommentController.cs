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

namespace Voxed.WebApp.Controllers
{
    public class CommentController : BaseController
    {
        private readonly FormateadorService _formateadorService;
        private readonly FileUploadService _fileUploadService;
        private readonly IVoxedRepository _voxedRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<HomeController> _logger;
        //private readonly ILogger _logger;
        private readonly NotificationService _notificationService;

        public CommentController(
            NotificationService notificationService,
            FormateadorService formateadorService,
            FileUploadService fileUploadService,
            IVoxedRepository voxedRepository,
            UserManager<User> userManager,
            ILogger<HomeController> logger,
            //ILoggerFactory loggerFactory,
            SignInManager<User> signInManager,
            IHttpContextAccessor accessor
            ) : base(accessor)
        {
            _formateadorService = formateadorService;
            _fileUploadService = fileUploadService;
            _voxedRepository = voxedRepository;
            _userManager = userManager;
            _logger = logger;
            //_logger = loggerFactory.CreateLogger(nameof(CommentController));
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
                if (request.HasInvalidContent())
                {
                    response.Swal = "Debes ingresar un contenido";
                    return response;
                }

                var comment = await ProcessComment(request, id);

                //cambiar para buscar vox con el id de la request
                await _voxedRepository.Comments.Add(comment);
                var vox = await _voxedRepository.Voxs.GetById(comment.VoxID);

                if (!comment.Content.ToLower().Contains("&gt;hide"))
                {
                    vox.Bump = DateTimeOffset.Now;
                }

                await _voxedRepository.SaveChangesAsync();

                // Manejar guardado de y envio de notificaciones en threads separados en background
                await _notificationService.ManageReplyNotifications(vox, comment, request);

                if (vox.User.UserType != UserType.Anonymous && vox.UserID != comment.UserID)
                {
                    await _notificationService.ManageOpNotification(vox, comment);
                }

                await _notificationService.SendCommentLiveUpdate(comment, vox, request);

                response.Hash = comment.Hash;
                response.Status = true;

                return response;
            }
            catch (NotImplementedException e)
            {
                response.Swal = e.Message;
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                response.Swal = "Hubo un error";
                return response;
            }
        }

        [HttpPost]
        [Route("comment/sticky")]
        public async Task Sticky(string request)
        {
            //a.append("contentType", t), a.append("contentId", n), a.append("vox", e),/ comment/sticky
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
                UserID = user.Id,
                Content = _formateadorService.Parse(request.Content),
                Style = StyleService.GetRandomCommentStyle(),
                IpAddress = UserIpAddress,
                UserAgent = UserAgent
            };

            await _fileUploadService.ProcessAttachment(request.GetUploadData(), request.File, comment);

            return comment;
        }

        private async Task<User> CreateAnonymousUser()
        {
            var user = new User
            {
                UserName = UserNameGenerator.NewAnonymousUserName(),
                EmailConfirmed = true,
                UserType = UserType.AnonymousAccount,
                IpAddress = UserIpAddress,
                UserAgent = UserAgent,
                Token = TokenGenerator.NewToken()
            };

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                return user;
            }

            throw new Exception("Error al crear usuario anonimo");
        }
    }
}