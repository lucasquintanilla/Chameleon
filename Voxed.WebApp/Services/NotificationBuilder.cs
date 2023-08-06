using Core.Data.Repositories;
using Core.Entities;
using Core.Services.TextFormatter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Voxed.WebApp.Services
{
    public interface INotificationBuilder
    {
        INotificationBuilder WithPost(Post post);
        INotificationBuilder WithComment(Comment comment);
        INotificationBuilder AddOPNotification();
        INotificationBuilder AddReplies();
        INotificationBuilder AddVoxSusbcriberNotifications();
        List<Notification> Build();
    }

    public class NotificationBuilder : INotificationBuilder
    {
        private Post _post;
        private Comment _comment;
        private List<Notification> _notifications = new List<Notification>();
        private IVoxedRepository _voxedRepository;
        private ITextFormatterService _formateadorService;

        public NotificationBuilder(
            IVoxedRepository voxedRepository,
            ITextFormatterService formatter)
        {
            _voxedRepository = voxedRepository;
            _formateadorService = formatter;
        }

        public INotificationBuilder WithPost(Post vox)
        {
            _post = vox;
            return this;
        }

        public INotificationBuilder WithComment(Comment comment)
        {
            _comment = comment;
            return this;
        }

        public INotificationBuilder AddOPNotification()
        {
            if (_comment.UserId != _post.UserId)
            {
                var notification = new Notification()
                {
                    CommentId = _comment.Id,
                    PostId = _comment.PostId,
                    UserId = _post.UserId,
                    Type = NotificationType.New,
                };

                _notifications.Add(notification);
            }

            return this;
        }

        public INotificationBuilder AddReplies()
        {
            var hashList = _formateadorService.GetRepliedHash(_comment.Content);

            if (!hashList.Any()) return this;

            var usersId = _voxedRepository.Comments.GetUsersByCommentHash(hashList, new Guid[] { _comment.UserId }).GetAwaiter().GetResult();

            if (!usersId.Any()) return this;

            var replyNotifications = usersId
                .Select(userId => new Notification()
                {
                    CommentId = _comment.Id,
                    PostId = _comment.PostId,
                    UserId = userId,
                    Type = NotificationType.Reply,
                })
                .ToList();

            _notifications.AddRange(replyNotifications);

            return this;
        }

        public INotificationBuilder AddVoxSusbcriberNotifications()
        {
            var voxSubscriberUserIds = _voxedRepository.UserPostActions
                .GetPostSubscriberUserIds(_post.Id, ignoreUserIds: new List<Guid>() { _comment.UserId, _post.UserId })
                .GetAwaiter()
                .GetResult();

            var subscriberNotifications = voxSubscriberUserIds
                  .Select(userId => new Notification()
                  {
                      CommentId = _comment.Id,
                      PostId = _post.Id,
                      UserId = userId,
                      Type = NotificationType.New,
                  })
                  .ToList();

            _notifications.AddRange(subscriberNotifications);
            return this;
        }

        public List<Notification> Build()
        {
            _voxedRepository.Notifications.AddRange(_notifications).GetAwaiter().GetResult();
            _voxedRepository.SaveChangesAsync().GetAwaiter().GetResult();
            return _notifications;
        }
    }
}
