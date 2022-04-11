using Core.Data.Repositories;
using Core.Entities;
using Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Voxed.WebApp.Services
{
    public class NotificationBuilder
    {
        private Vox _vox;
        private Comment _comment;
        private List<Notification> _notifications = new List<Notification>();
        private IVoxedRepository _voxedRepository;
        private FormateadorService _formateadorService;

        public NotificationBuilder UseRepository(IVoxedRepository voxedRepository)
        {
            _voxedRepository = voxedRepository;
            return this;
        }

        public NotificationBuilder WithVox(Vox vox)
        {
            _vox = vox;
            return this;
        }

        public NotificationBuilder WithComment(Comment comment)
        {
            _comment = comment;
            return this;
        }

        public NotificationBuilder AddOPNotification()
        {
            if (_vox.User.UserType != UserType.Anonymous && _vox.UserID != _comment.UserID)
            {
                var notification = new Notification()
                {
                    CommentId = _comment.ID,
                    VoxId = _vox.ID,
                    UserId = _vox.UserID,
                    Type = NotificationType.NewComment,
                };

                _notifications.Add(notification);
            }

            return this;
        }

        public NotificationBuilder UseFormatter(FormateadorService formatter)
        {
            _formateadorService = formatter;
            return this;

        }
        public NotificationBuilder AddReplies()
        {
            var hashList = _formateadorService.GetRepliedHash(_comment.Content);

            if (!hashList.Any()) return this;

            var usersId = _voxedRepository.Comments.GetUsersByCommentHash(hashList, new Guid[] { _comment.UserID }).GetAwaiter().GetResult();

            if (!usersId.Any()) return this;

            var replyNotifications = usersId
                .Select(userId => new Notification()
                {
                    CommentId = _comment.ID,
                    VoxId = _vox.ID,
                    UserId = userId,
                    Type = NotificationType.Reply,
                })
                .ToList();

            _notifications.AddRange(replyNotifications);

            return this;
        }

        public NotificationBuilder AddVoxSusbcriberNotifications()
        {
            var voxSubscriberUserIds = _voxedRepository.UserVoxActions
                .GetVoxSubscriberUserIds(_vox.ID, ignoreUserIds: new List<Guid>() { _comment.UserID, _vox.UserID })
                .GetAwaiter()
                .GetResult();

            var subscriberNotifications = voxSubscriberUserIds
                  .Select(userId => new Notification()
                  {
                      CommentId = _comment.ID,
                      VoxId = _vox.ID,
                      UserId = userId,
                      Type = NotificationType.NewComment,
                  })
                  .ToList();

            _notifications.AddRange(subscriberNotifications);
            return this;
        }

        public List<Notification> Save()
        {
            _voxedRepository.Notifications.AddRange(_notifications).GetAwaiter().GetResult();
            _voxedRepository.SaveChangesAsync().GetAwaiter().GetResult();
            return _notifications;
        }
    }
}
