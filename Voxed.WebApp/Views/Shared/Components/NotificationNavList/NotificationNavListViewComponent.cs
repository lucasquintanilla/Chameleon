using Core.Data.Repositories;
using Core.Entities;
using Core.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voxed.WebApp.Extensions;
using Voxed.WebApp.Hubs;

namespace Voxed.WebApp.Views.Shared.Components.NotificationNavList
{
    public class NotificationNavListViewComponent : ViewComponent
    {
        private readonly IVoxedRepository _voxedRepository;
        private readonly UserManager<User> _userManager;

        public NotificationNavListViewComponent(IVoxedRepository voxedRepository,
            UserManager<User> userManager)
        {
            _voxedRepository = voxedRepository;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.User.GetUserId();
            if (userId == null)
            {
                return View(new List<UserNotification>());
            }

            var notifications = await _voxedRepository.Notifications.GetByUserId(userId.Value);

            if (notifications.Any())
            {
                ViewData["NotificationsCount"] = $"({notifications.Count()})";
            }

            var userNotifications = notifications
                .Select(notification => new UserNotification
                {
                    Type = notification.Type.ToString().ToLowerInvariant(),
                    Content = new NotificationContent()
                    {
                        VoxHash = notification.Vox.Id.ToShortString(),
                        NotificationBold = GetTitleNotification(notification.Type),
                        NotificationText = notification.Vox.Title,
                        Count = "1",
                        ContentHash = notification.Comment.Hash,
                        Id = notification.Id.ToString(),
                        ThumbnailUrl = notification.Vox.Media?.ThumbnailUrl
                    }
                });

            return View(userNotifications);
        }

        private string GetTitleNotification(NotificationType notificationType)
        {
            switch (notificationType)
            {
                case NotificationType.New:
                    return "Nuevo comentario";
                case NotificationType.Reply:
                    return "Nueva respuesta";
                default:
                    return "Nueva notificacion";
            }
        }
    }
}
