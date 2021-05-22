using Core.Data.Repositories;
using Core.Entities;
using Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class NotificationService
    {
        //private readonly FormateadorService _formateadorService;
        //private readonly IVoxedRepository _voxedRepository;
        //private readonly IHubContext<VoxedHub, INotificationHub> _notificationHub;

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
        //    await _voxedRepository.CompleteAsync();

        //    foreach (var notification in repliesNotifications)
        //    {
        //        await SendReplyLiveNotification(vox, comment, notification);
        //    }
        //}

        //private async Task SendReplyLiveNotification(Vox vox, Comment comment, Notification notification)
        //{
        //    var userNotification = new UserNotification()
        //    {
        //        Type = "new",
        //        Content = new Content()
        //        {
        //            VoxHash = vox.Hash,
        //            NotificationBold = "Nueva respuesta",
        //            NotificationText = vox.Title,
        //            Count = "1",
        //            ContentHash = comment.Hash,
        //            Id = notification.Id.ToString(), //id notification
        //            ThumbnailUrl = vox.Media?.ThumbnailUrl
        //        }
        //    };

        //    await _notificationHub.Clients.Users(notification.UserId.ToString()).Notification(userNotification);
        //}
    }
}
