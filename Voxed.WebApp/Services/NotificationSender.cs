using Core.Constants;
using Core.Entities;
using Core.Extensions;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using Voxed.WebApp.Hubs;
using Voxed.WebApp.Mappers;

namespace Voxed.WebApp.Services
{
    public interface INotificationSender
    {
        INotificationSender WithVox(Post vox);
        INotificationSender WithComment(Comment comment);
        INotificationSender WithNotifications(List<Notification> notifications);
        void Notify();
    }
    public class NotificationSender : INotificationSender
    {
        private IHubContext<NotificationHub, INotificationHub> _notificationHub;
        private List<Notification> _notifications = new List<Notification>();
        private Post _vox;
        private Comment _comment;
        private IMapper _mapper;

        public NotificationSender(
            IMapper mapper, 
            IHubContext<NotificationHub, INotificationHub> notificationHub)
        {
            _mapper = mapper;
            _notificationHub = notificationHub;
        }

        public INotificationSender WithVox(Post vox)
        {
            _vox = vox;
            return this;
        }

        public INotificationSender WithComment(Comment comment)
        {
            _comment = comment;
            return this;
        }

        public INotificationSender WithNotifications(List<Notification> notifications)
        {
            _notifications = notifications;
            return this;
        }

        public void Notify()
        {
            foreach (var notification in _notifications)
            {
                //var userNotification = new UserNotification()
                //{
                //    Type = "new",
                //    Content = new NotificationContent()
                //    {
                //        VoxHash = _vox.Id.ToShortString(),
                //        NotificationBold = GetNotificationBoldString(notification.Type),
                //        NotificationText = _vox.Title,
                //        Count = "1",
                //        ContentHash = _comment.Hash,
                //        Id = notification.Id.ToString(),
                //        //ThumbnailUrl = _vox.Media?.Url + Core.Constants.ImageParameter.FormatWebP
                //        ThumbnailUrl = $"/media/{_vox.Media?.Key}" + ImageParameter.FormatWebP,
                //    }
                //};

                var userNotification = _mapper.Map(notification);

                _notificationHub.Clients.Users(notification.UserId.ToString()).Notification(userNotification);
            }
        }
    }
}
