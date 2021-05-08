using Core.Data.Repositories;
using Core.Entities;
using Core.Shared;
using Core.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Voxed.WebApp.Hubs;

namespace Voxed.WebApp.Controllers
{
    public class CommentController : BaseController
    {
        private readonly FormateadorService _formateadorService;        
        private readonly FileStoreService _fileStoreService;
        private readonly IVoxedRepository _voxedRepository;
        private readonly UserManager<User> _userManager;        
        private readonly ILogger<HomeController> _logger;
        private readonly IHubContext<VoxedHub, INotificationHub> _notificationHub;
        private static User _anonUser;

        public CommentController(
            FormateadorService formateadorService,
            FileStoreService fileStoreService,
            IVoxedRepository voxedRepository,
            UserManager<User> userManager,
            IHubContext<VoxedHub, INotificationHub> notificationHub, 
            ILogger<HomeController> logger)
        {
            _formateadorService = formateadorService;
            _fileStoreService = fileStoreService;
            _voxedRepository = voxedRepository;
            _userManager = userManager;
            _notificationHub = notificationHub;
            _logger = logger;
        }


        [HttpPost]
        public async Task<Models.CommentResponse> Nuevo([FromForm] Models.CommentRequest request, [FromRoute] string id)
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
                var user = await _userManager.GetUserAsync(HttpContext.User);

                var comment = new Comment()
                {
                    ID = Guid.NewGuid(),
                    Hash = new Hash().NewHash(7),
                    VoxID = new Guid(id),
                    UserID = user == null ? GetAnonUser().Id : user.Id,
                    Content = request.Content == null ? null : _formateadorService.Parsear(request.Content),
                    Style = StyleService.GetRandomCommentStyle(),
                    IpAddress = UserIpAddress.ToString(),
                    UserAgent = UserAgent
                };
                
                await _fileStoreService.ProcessMedia(request.GetUploadData(), request.File, comment);

                //Detectar Tag >HIDE
                //if (!comentario.Contenido.ToLower().Contains("gt;hide"))
                //{
                //    await db.Query("Hilos")
                //        .Where("Id", comentario.HiloId)
                //        .UpdateAsync(new { Bump = DateTimeOffset.Now });
                //}

                //var replied = _formateadorService.GetRepliedHash(request.Content);



                await _voxedRepository.Comments.Add(comment);

                var vox = await _voxedRepository.Voxs.GetById(comment.VoxID);
                vox.Bump = DateTimeOffset.Now;
                await _voxedRepository.CompleteAsync();

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
                    await _voxedRepository.CompleteAsync();
                }

                var commentNotification = new CommentNotification()
                {
                    UniqueId = null, //si es unique id puede tener colores unicos
                    UniqueColor = null,
                    UniqueColorContrast = null,

                    Id = comment.ID.ToString(),
                    Hash = comment.Hash,
                    VoxHash = vox.Hash,
                    AvatarColor = comment.Style.ToString().ToLower(),
                    IsOp = vox.UserID == comment.UserID && vox.User.UserType != UserType.Anonymous, //probar cambiarlo cuando solo pruedan craer los usuarios.
                    Tag = GetUserTypeTag(comment.User.UserType), //admin o dev               
                    Content = comment.Content ?? "",
                    Name = comment.User.UserName,
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

                if (comment.UserID != vox.User.Id)
                {
                    var opNotification = new OpNotification()
                    {
                        Type = "new",
                        Content = new Content()
                        {
                            VoxHash = vox.Hash,
                            NotificationBold = "Nuevo Comentario",
                            NotificationText = vox.Title,
                            Count = "1",
                            ContentHash = comment.Hash,
                            Id = GuidConverter.ToShortString(vox.ID),
                            ThumbnailUrl = vox.Media?.ThumbnailUrl                  
                        }
                    };

                    await _notificationHub.Clients.User(vox.User.Id.ToString()).Notification(opNotification);
                }

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
                _logger.LogError(e.ToString());

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

        private string GetUserTypeTag(UserType userType)
        {
            switch (userType)
            {
                case UserType.Anonymous:
                    return "anon";
                case UserType.Administrator:
                    return "admin";
                case UserType.Moderator:
                    return "mod";
                case UserType.Account:
                    return "anon";
                default:
                    return "anon";
            }
        }

        private string GetFileExtensionFromUrl(string url)
        {
            var array = url.Split(".");
            return array[array.Length - 1];
        }
    }
}
