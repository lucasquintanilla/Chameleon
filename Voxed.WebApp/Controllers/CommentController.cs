using Core.Data.Repositories;
using Core.Entities;
using Core.Shared;
using Core.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Voxed.WebApp.Hubs;

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
        private readonly IHubContext<VoxedHub, INotificationHub> _notificationHub;
        private static User _anonUser;

        public CommentController(
            FormateadorService formateadorService,
            FileUploadService fileUploadService,
            IVoxedRepository voxedRepository,
            UserManager<User> userManager,
            IHubContext<VoxedHub, INotificationHub> notificationHub,
            ILogger<HomeController> logger,
            ILoggerFactory loggerFactory,
            SignInManager<User> signInManager, 
            IHttpContextAccessor accessor) : base(accessor)
        {
            _formateadorService = formateadorService;
            _fileUploadService = fileUploadService;
            _voxedRepository = voxedRepository;
            _userManager = userManager;
            _notificationHub = notificationHub;
            _logger = logger;
            //_logger = loggerFactory.CreateLogger(nameof(CommentController));
            _signInManager = signInManager;
        }


        [HttpPost]
        [Route("comment/nuevo/{id}")]
        public async Task<Models.CommentResponse> Create([FromForm] Models.CommentRequest request, [FromRoute] string id)
        {
            if (!ModelState.IsValid)
                return new Models.CommentResponse()
                {
                    Hash = id,
                    Status = false,
                    Error = "",
                    Swal = "Formato de comentario invalido",
                };

            try
            {
                if (request.Content == null && request.File == null && request.GetUploadData().Extension == null)
                {
                    return new Models.CommentResponse()
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
                //await _voxedRepository.CompleteAsync();

                //var repliesNotificationTask = Task.Run(() => SaveRepliesNotifications(vox, comment, request));
                //var opNotificationTask = Task.Run(() => SaveOpNotification(vox, comment));

                //new Task(() => SaveRepliesNotifications(vox, comment, request)).Start();
                //new Task(() => SaveOpNotification(vox, comment)).Start();

                //Thread backgroundThread = new Thread(new ThreadStart(SaveRepliesNotifications));
                //backgroundThread.IsBackground = true;
                //backgroundThread.Start(vox, comment, request);

                //new Thread(() => SaveRepliesNotifications(vox, comment, request)).Start();
                //new Thread(() => SaveOpNotification(vox, comment)).Start();




                // FUNCIONA EN BACKGORUND PERO TIRA ERRROR POR LOS OBJETOS DE DISPONEN
                //var voxForNotification = vox;
                //var commentForNotification = comment;
                //var requestForNotification = request;

                //var th = new Thread(() => SaveRepliesNotifications(voxForNotification, commentForNotification, requestForNotification));
                //th.IsBackground = true;
                //th.Start();
                //var th2 = new Thread(() => SaveOpNotification(voxForNotification, commentForNotification));
                //th2.IsBackground = true;
                //th2.Start();



                // NO FUNCIONA
                //var task1 = SaveRepliesNotifications(vox, comment, request);
                //var task2 = SaveOpNotification(vox, comment);

                // Manejar guardado de y envio de notificaciones en threads separados en background
                await SaveRepliesNotifications(vox, comment, request);
                await SaveOpNotification(vox, comment);

                await _voxedRepository.SaveChangesAsync();

                await SendCommentLiveUpdate(comment, vox, request);

                var response = new Models.CommentResponse()
                {
                    Hash = comment.Hash,
                    Status = true,
                };

                return response;
            }
            catch (NotImplementedException e)
            {
                return new Models.CommentResponse()
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

                return new Models.CommentResponse()
                {
                    Hash = "",
                    Status = false,
                    Error = "",
                    Swal = "Hubo un error",
                };
            }
        }

        private User GetAnonUser()
        {
            if (_anonUser == null)
            {
                _anonUser = _userManager.Users.Where(x => x.UserType == UserType.Anonymous).FirstOrDefault();
            }

            return _anonUser;
        }

        private string GetFileExtensionFromUrl(string url)
        {
            var array = url.Split(".");
            return array[array.Length - 1];
        }

        private async Task SaveOpNotification(Vox vox, Comment comment)
        {
            //await Task.Delay(10000);

            if (vox.User.UserType != UserType.Anonymous && vox.UserID != comment.UserID)
            {
                var notification = new Notification()
                {
                    CommentId = comment.ID,
                    VoxId = vox.ID,
                    UserId = vox.UserID,
                    Type = NotificationType.NewComment,
                };

                await _voxedRepository.Notifications.Add(notification);
                //await _voxedRepository.CompleteAsync();

                await SendOpLiveNotification(comment, vox, notification);
            }
        }

        private async Task SaveRepliesNotifications(Vox vox, Comment comment, Models.CommentRequest request)
        {
            if (string.IsNullOrWhiteSpace(comment.Content)) return;

            var hashList = _formateadorService.GetRepliedHash(request.Content);

            if (!hashList.Any()) return;

            var usersId = await _voxedRepository.Comments.GetUsersByCommentHash(hashList, new Guid[] { comment.UserID });

            if (!usersId.Any()) return;

            var repliesNotifications = usersId
                .Where(x => x != GetAnonUser().Id)
                .Select(userId => new Notification()
                {
                    CommentId = comment.ID,
                    VoxId = vox.ID,
                    UserId = userId,
                    Type = NotificationType.Reply,
                })
                .ToList();

            await _voxedRepository.Notifications.AddRange(repliesNotifications);
            //await _voxedRepository.CompleteAsync();

            foreach (var notification in repliesNotifications)
            {
                await SendReplyLiveNotification(vox, comment, notification);
            }
        }

        private async Task SendCommentLiveUpdate(Comment comment, Vox vox, Models.CommentRequest request)
        {
            var commentNotification = new CommentLiveUpdate()
            {
                UniqueId = null, //si es unique id puede tener colores unicos
                UniqueColor = null,
                UniqueColorContrast = null,

                Id = comment.ID.ToString(),
                Hash = comment.Hash,
                VoxHash = GuidConverter.ToShortString(vox.ID),
                AvatarColor = comment.Style.ToString().ToLower(),
                IsOp = vox.UserID == comment.UserID && vox.User.UserType != UserType.Anonymous, //probar cambiarlo cuando solo pruedan craer los usuarios.
                Tag = UserViewHelper.GetUserTypeTag(comment.User.UserType), //admin o dev               
                Content = comment.Content ?? "",
                Name = UserViewHelper.GetUserName(comment.User),
                CreatedAt = TimeAgo.ConvertToTimeAgo(comment.CreatedOn.DateTime),
                Poll = null, //aca va una opcion respondida

                //Media
                MediaUrl = comment.Media?.Url,
                MediaThumbnailUrl = comment.Media?.ThumbnailUrl,
                Extension = request.GetUploadData()?.Extension == UploadDataExtension.Base64 ? GetFileExtensionFromUrl(comment.Media?.Url) : request.GetUploadData()?.Extension,
                ExtensionData = request.GetUploadData()?.ExtensionData,
                Via = request.GetUploadData()?.Extension == UploadDataExtension.Youtube ? comment.Media?.Url : null,
            };

            await _notificationHub.Clients.All.Comment(commentNotification);
        }

        private async Task SendOpLiveNotification(Comment comment, Vox vox, Notification notification)
        {
            if (comment.UserID != vox.User.Id)
            {
                var userNotification = new UserNotification()
                {
                    Type = "new",
                    Content = new Content()
                    {
                        VoxHash = vox.Hash,
                        NotificationBold = "Nuevo Comentario",
                        NotificationText = vox.Title,
                        Count = "1",
                        ContentHash = comment.Hash,
                        Id = notification.Id.ToString(),
                        ThumbnailUrl = vox.Media?.ThumbnailUrl
                    }
                };

                await _notificationHub.Clients.User(vox.User.Id.ToString()).Notification(userNotification);
            }
        }

        private async Task SendReplyLiveNotification(Vox vox, Comment comment, Notification notification)
        {
            var userNotification = new UserNotification()
            {
                Type = "new",
                Content = new Content()
                {
                    VoxHash = vox.Hash,
                    NotificationBold = "Nueva respuesta",
                    NotificationText = vox.Title,
                    Count = "1",
                    ContentHash = comment.Hash,
                    Id = notification.Id.ToString(), //id notification
                    ThumbnailUrl = vox.Media?.ThumbnailUrl
                }
            };

            await _notificationHub.Clients.Users(notification.UserId.ToString()).Notification(userNotification);
        }

        private async Task<Comment> ProcessComment(Models.CommentRequest request, string id)
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
                UserID = user == null ? GetAnonUser().Id : user.Id,
                Content = request.Content == null ? null : _formateadorService.Parse(request.Content),
                Style = StyleService.GetRandomCommentStyle(),
                IpAddress = UserIpAddress.ToString(),
                UserAgent = UserAgent
            };

            await _fileUploadService.ProcessMedia(request.GetUploadData(), request.File, comment);

            return comment;
        }

        private async Task<User> CreateAnonymousUser()
        {
            var user = new User
            {
                UserName = UserNameGenerator.NewAnonymousUserName(),
                EmailConfirmed = true,
                UserType = UserType.AnonymousAccount,
                IpAddress = UserIpAddress.ToString(),
                UserAgent = UserAgent,
                Token = TokenGenerator.NewToken()
            };

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                return user;
            }

            throw new Exception();
        }
    }
}