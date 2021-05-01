using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Core.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Voxed.WebApp.Hubs;
using Newtonsoft.Json;

namespace Voxed.WebApp.Controllers
{
    public class CommentController : BaseController
    {
        private FormateadorService formateadorService;        
        private FileStoreService fileStoreService;
        private readonly IVoxedRepository voxedRepository;
        private readonly UserManager<User> _userManager;
        private User anonUser;
        public IHubContext<VoxedHub, INotificationHub> _notificationHub { get; }       

        public CommentController(
            FormateadorService formateadorService,
            FileStoreService fileStoreService,
            IVoxedRepository voxedRepository,
            UserManager<User> userManager,
            IHubContext<VoxedHub, INotificationHub> notificationHub)
        {
            this.formateadorService = formateadorService;
            this.fileStoreService = fileStoreService;
            this.voxedRepository = voxedRepository;
            _userManager = userManager;
            _notificationHub = notificationHub;
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
                    User = user ?? GetAnonUser(),
                    Content = request.Content == null ? null : formateadorService.Parsear(request.Content),
                    Style = StyleService.GetRandomCommentStyle(),
                    IpAddress = UserIpAddress.ToString(),
                    UserAgent = UserAgent
                };

                await ProcessMedia(request, comment);

                //Detectar Tag >HIDE
                //if (!comentario.Contenido.ToLower().Contains("gt;hide"))
                //{
                //    await db.Query("Hilos")
                //        .Where("Id", comentario.HiloId)
                //        .UpdateAsync(new { Bump = DateTimeOffset.Now });
                //}

                await voxedRepository.Comments.Add(comment);
                var vox = await voxedRepository.Voxs.GetById(comment.VoxID);
                vox.Bump = DateTimeOffset.Now;
                await voxedRepository.CompleteAsync();

                var commentNotification = new CommentNotification()
                {
                    UniqueId = null, //si es unique id puede tener colores unicos
                    UniqueColor = null,
                    UniqueColorContrast = null,

                    Id = comment.ID.ToString(),
                    Hash = comment.Hash,
                    VoxHash = vox.Hash,
                    AvatarColor = comment.Style.ToString().ToLower(),
                    IsOp = vox.UserID == comment.UserID && vox.User.UserType != UserType.Anon, //probar cambiarlo cuando solo pruedan craer los usuarios.
                    Tag = GetUserTypeTag(comment.User.UserType), //admin o dev               
                    Content = comment.Content ?? "",
                    Name = comment.User.UserName,
                    CreatedAt = TimeAgo.ConvertToTimeAgo(comment.CreatedOn.DateTime),
                    Poll = null, //aca va una opcion respondida

                    //Media
                    MediaUrl = comment.Media?.Url,
                    MediaThumbnailUrl = comment.Media?.ThumbnailUrl,
                    Extension = request.GetUploadData()?.Extension,
                    ExtensionData = request.GetUploadData()?.ExtensionData,
                    Via = request.GetUploadData()?.Extension == Models.UploadDataExtension.Youtube ? comment.Media?.Url : null,
                };

                await _notificationHub.Clients.All.Comment(commentNotification);

                if (comment.UserID != vox.User.Id)
                {
                    var notification = new Notification()
                    {
                        Type = "new",
                        Content = new Content()
                        {
                            VoxHash = vox.Hash,
                            NotificationBold = "Nuevo Comentario",
                            NotificationText = vox.Title,
                            Count = "14",
                            ContentHash = comment.Hash,
                            Id = GuidConverter.ToShortString(vox.ID),
                            ThumbnailUrl = vox.Media?.ThumbnailUrl
                        }
                    };

                    await _notificationHub.Clients.User(vox.User.Id.ToString()).Notification(notification);
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
            if (anonUser == null)
            {
                anonUser = _userManager.Users.Where(x => x.UserType == UserType.Anon).FirstOrDefault();
            }

            return anonUser;
        }

        private async Task ProcessMedia(Models.CommentRequest request, Comment comment)
        {
            var data = request.GetUploadData();

            if (data != null && request.File == null)
            {                
                if (data.Extension == Models.UploadDataExtension.Youtube)
                {
                    comment.Media = await fileStoreService.SaveExternal(data, comment.Hash);
                }
                else if (data.Extension == Models.UploadDataExtension.Base64)
                {
                    throw new NotImplementedException("Opcion no implementada");
                }
            }
            else if (request.File != null)
            {
                var isValidFile = await fileStoreService.IsValidFile(request.File);

                if (!isValidFile)
                {
                    throw new NotImplementedException("Archivo invalido");
                }

                comment.Media = await fileStoreService.Save(request.File, comment.Hash);
            }
        }

        private string GetUserTypeTag(UserType userType)
        {
            switch (userType)
            {
                case UserType.Anon:
                    return "anon";
                case UserType.Admin:
                    return "admin";
                case UserType.Mod:
                    return "mod";
                case UserType.Account:
                    return "anon";
                default:
                    return "anon";
            }
        }
    }
}
