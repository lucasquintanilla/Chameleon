﻿using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voxed.WebApp.Hubs;
using Voxed.WebApp.Extensions;
using Core.Shared;

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
            //var user = await _userManager.GetUserAsync(HttpContext.User.I);
            var userId = HttpContext.User.GetLoggedInUserId<Guid?>();
            if (userId == null)
            {
                return View(new List<UserNotification>());
            }

            var notifications = await _voxedRepository.Notifications.GetByUserId(userId.Value);

            if (notifications.Count() > 0)
            {
                ViewData["NotificationsCount"] = $"({notifications.Count()})";
            }

            var userNotifications = notifications
                .Select(notification => new UserNotification
                {
                    Type = "new",
                    Content = new Content()
                    {
                        VoxHash = GuidConverter.ToShortString(notification.Vox.Id),
                        NotificationBold = GetTitleNotification(notification.Type),
                        NotificationText = notification.Vox.Title,
                        Count = "1",
                        ContentHash = notification.Comment.Hash,
                        Id = notification.Id.ToString(),
                        ThumbnailUrl = notification.Vox.Attachment?.ThumbnailUrl
                    }
                });

            return View(userNotifications);
        }

        private string GetTitleNotification(NotificationType notificationType)
        {
            switch (notificationType)
            {
                case NotificationType.NewComment:
                    return "Nuevo comentario";
                case NotificationType.Reply:
                    return "Nueva respuesta";
                default:
                    return "Nuevo notificacion";
            }
        }
    }
}
