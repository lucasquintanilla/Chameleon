using Core.Data.Repositories;
using Core.Entities;
using Core.Shared;
using Core.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voxed.WebApp.Extensions;
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

        public async Task ManageReplyNotifications(Vox vox, Comment comment, Models.CommentRequest request)
        {
            var replyNotifications = await CreateReplyNotifications(vox, comment, request);

            foreach (var notification in replyNotifications)
            {
                await SendReplyLiveNotification(vox, comment, notification);
            }
        }

        public async Task ManageOpNotification(Vox vox, Comment comment)
        {
            var notification = await CreateOpNotification(vox, comment);
            await SendOpLiveNotification(comment, vox, notification);
        }

        private async Task<Notification> CreateOpNotification(Vox vox, Comment comment)
        {
            var notification = new Notification()
            {
                CommentId = comment.ID,
                VoxId = vox.ID,
                UserId = vox.UserID,
                Type = NotificationType.NewComment,
            };

            await _voxedRepository.Notifications.Add(notification);

            await _voxedRepository.SaveChangesAsync();

            return notification;
        }

        private async Task<List<Notification>> CreateReplyNotifications(Vox vox, Comment comment, Models.CommentRequest request)
        {
            var notifications = new List<Notification>();

            if (string.IsNullOrWhiteSpace(comment.Content)) return notifications;

            var hashList = _formateadorService.GetRepliedHash(request.Content);

            if (!hashList.Any()) return notifications;

            var usersId = await _voxedRepository.Comments.GetUsersByCommentHash(hashList, new Guid[] { comment.UserID });

            if (!usersId.Any()) return notifications;

            var repliesNotifications = usersId
                .Select(userId => new Notification()
                {
                    CommentId = comment.ID,
                    VoxId = vox.ID,
                    UserId = userId,
                    Type = NotificationType.Reply,
                })
                .ToList();

            await _voxedRepository.Notifications.AddRange(repliesNotifications);

            await _voxedRepository.SaveChangesAsync();

            return repliesNotifications;

        }

        public async Task SendCommentLiveUpdate(Comment comment, Vox vox, Models.CommentRequest request)
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
                CreatedAt = comment.CreatedOn.DateTime.ToTimeAgo(),
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

            await _notificationHub.Clients.User(vox.UserID.ToString()).Notification(userNotification);
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
                    Id = notification.Id.ToString(),
                    ThumbnailUrl = vox.Media?.ThumbnailUrl
                }
            };

            await _notificationHub.Clients.Users(notification.UserId.ToString()).Notification(userNotification);
        }
    }
}
