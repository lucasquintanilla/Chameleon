using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Data.Repositories;
using Core.Entities;
using Core.Shared;
using Core.Utilities;
using Core.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using Voxed.WebApp.Hubs;

namespace Voxed.WebApp.Services
{
    public class NotificationService
    {
        private readonly IVoxedRepository _voxedRepository; 
        private readonly IHubContext<VoxedHub, INotificationHub> _notificationHub;
        private readonly FormateadorService _formateadorService;

        public NotificationService(
            IVoxedRepository voxedRepository,
            IHubContext<VoxedHub, INotificationHub> notificationHub,
            FormateadorService formateadorService
            )
        {
            _voxedRepository = voxedRepository;
            _notificationHub = notificationHub;
            _formateadorService = formateadorService;
        }

        private async Task SaveOpNotification(Vox vox, Comment comment)
        {
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

        //private async Task SaveRepliesNotifications(Vox vox, Comment comment, Models.CommentRequest request)
        //{
        //    if (string.IsNullOrWhiteSpace(comment.Content)) return;

        //    var hashList = _formateadorService.GetRepliedHash(request.Content);

        //    if (!hashList.Any()) return;

        //    var usersId = await _voxedRepository.Comments.GetUsersByCommentHash(hashList, new Guid[] { comment.UserID });

        //    if (!usersId.Any()) return;

        //    var repliesNotifications = usersId
        //        .Where(x => x != GetAnonUser().Id)
        //        .Select(userId => new Notification()
        //        {
        //            CommentId = comment.ID,
        //            VoxId = vox.ID,
        //            UserId = userId,
        //            Type = NotificationType.Reply,
        //        })
        //        .ToList();

        //    await _voxedRepository.Notifications.AddRange(repliesNotifications);
        //    //await _voxedRepository.CompleteAsync();

        //    foreach (var notification in repliesNotifications)
        //    {
        //        await SendReplyLiveNotification(vox, comment, notification);
        //    }
        //}

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
                Extension = request.GetUploadData()?.Extension == UploadDataExtension.Base64 ? Core.Utilities.Utilities.GetFileExtensionFromUrl(comment.Media?.Url) : request.GetUploadData()?.Extension,
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
    }


}
