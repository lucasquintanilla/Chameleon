using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
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
            var opNotificationList = new List<OpNotification>();

            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user != null)
            {
                var notifications = await _voxedRepository.Notifications.GetByUserId(user.Id);

                if (notifications.Count() > 0)
                {
                    ViewData["NotificationsCount"] = $"({notifications.Count()})";
                    //ViewBag["NotificationsCount"] = $"({notifications.Count()})";
                }

                

                foreach (var notification in notifications)
                {
                    var opNotification = new OpNotification()
                    {
                        Type = "new",
                        Content = new Content()
                        {
                            VoxHash = notification.Vox.Hash,
                            NotificationBold = "Nuevo Comentario",
                            NotificationText = notification.Vox.Title,
                            Count = "1",
                            ContentHash = notification.Comment.Hash,
                            Id = Core.Shared.GuidConverter.ToShortString(notification.Vox.ID),
                            ThumbnailUrl = notification.Vox.Media?.ThumbnailUrl
                        }
                    };

                    opNotificationList.Add(opNotification);
                }
            }

            return View(opNotificationList);
        }
    }
}
