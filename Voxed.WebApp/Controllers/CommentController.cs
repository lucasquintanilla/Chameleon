using Core.Data.Repositories;
using Core.Entities;
using Core.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Voxed.WebApp.Hubs;
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
        public async Task<Models.CommentResponse> Create([FromForm] CommentRequest request, [FromRoute] string id)
        {
            if (!ModelState.IsValid)
                return new CommentResponse()
                {
                    Hash = id,
                    Status = false,
                    Error = "",
                    Swal = "Formato de comentario invalido",
                };

            try
            {
                if (request.Content == null && request.File == null && request.GetUploadData()?.Extension == null)
                {
                    return new CommentResponse()
                    {
                        Hash = id,
                        Status = false,
                        Error = "",
                        Swal = "Debes ingresar un contenido",
                    };
                }

                var comment = await ProcessComment(request, id);

                //Detectar Tag >HIDE
                //if (!comentario.Contenido.ToLower().Contains("gt;hide"))
                //{
                //    await db.Query("Hilos")
                //        .Where("Id", comentario.HiloId)
                //        .UpdateAsync(new { Bump = DateTimeOffset.Now });
                //}

                //cambiar para buscar vox con el id de la request
                await _voxedRepository.Comments.Add(comment);
                var vox = await _voxedRepository.Voxs.GetById(comment.VoxID);
                vox.Bump = DateTimeOffset.Now;
                await _voxedRepository.SaveChangesAsync();

                // Manejar guardado de y envio de notificaciones en threads separados en background
                await _notificationService.ManageReplyNotifications(vox, comment, request);

                if (vox.User.UserType != UserType.Anonymous && vox.UserID != comment.UserID)
                {
                    await _notificationService.ManageOpNotification(vox, comment);
                }

                await _notificationService.SendCommentLiveUpdate(comment, vox, request);

                var response = new CommentResponse()
                {
                    Hash = comment.Hash,
                    Status = true,
                };

                return response;
            }
            catch (NotImplementedException e)
            {
                return new CommentResponse()
                {
                    Hash = "",
                    Status = false,
                    Error = "",
                    Swal = e.Message,
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return new CommentResponse()
                {
                    Hash = "",
                    Status = false,
                    Error = "",
                    Swal = "Hubo un error",
                };
            }
        }

        private async Task<Comment> ProcessComment(CommentRequest request, string id)
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
                Content = request.Content == null ? null : _formateadorService.Parse(request.Content),
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