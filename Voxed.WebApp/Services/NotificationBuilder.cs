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
        private IContentFormatterService _formateadorService;

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
            if (_comment.UserId != _vox.UserId)
            {
                var notification = new Notification()
                {
                    CommentId = _comment.Id,
                    VoxId = _comment.VoxId,
                    UserId = _vox.UserId,
                    Type = NotificationType.NewComment,
                };

                _notifications.Add(notification);
            }

            return this;
        }

        public NotificationBuilder UseFormatter(IContentFormatterService formatter)
        {
            _formateadorService = formatter;
            return this;

        }
        public NotificationBuilder AddReplies()
        {
            var hashList = _formateadorService.GetRepliedHash(_comment.Content);

            if (!hashList.Any()) return this;

            var usersId = _voxedRepository.Comments.GetUsersByCommentHash(hashList, new Guid[] { _comment.UserId }).GetAwaiter().GetResult();

            if (!usersId.Any()) return this;

            var replyNotifications = usersId
                .Select(userId => new Notification()
                {
                    CommentId = _comment.Id,
                    VoxId = _comment.VoxId,
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
                .GetVoxSubscriberUserIds(_vox.Id, ignoreUserIds: new List<Guid>() { _comment.UserId, _vox.UserId })
                .GetAwaiter()
                .GetResult();

            var subscriberNotifications = voxSubscriberUserIds
                  .Select(userId => new Notification()
                  {
                      CommentId = _comment.Id,
                      VoxId = _vox.Id,
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
