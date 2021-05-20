﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Voxed.WebApp.Hubs;

namespace Voxed.WebApp.Controllers
{
    public class NotificationController : BaseController
    {
        private IVoxedRepository _voxedRepository;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<VoxedHub, INotificationHub> _notificationHub;

        public NotificationController(IVoxedRepository voxedRepository,
            UserManager<User> userManager, 
            IHubContext<VoxedHub, INotificationHub> notificationHub)
        {
            _voxedRepository = voxedRepository;
            _userManager = userManager;
            _notificationHub = notificationHub;
        }

        //[AllowAnonymous]
        //[Route("notification/{voxId}/{commentHash}")]
        //public async Task<IActionResult> Index(string voxId, string commentHash)
        //{
        //    var notification = await _voxedRepository.Notifications.GetByVoxId(Core.Shared.GuidConverter.FromShortString(voxId));

        //    if (notification != null)
        //    {
        //        await _voxedRepository.Notifications.Remove(notification);
        //        await _voxedRepository.CompleteAsync();
        //    }

        //    return Redirect($"~/vox/{voxId}#{commentHash}");
        //}

        [AllowAnonymous]
        [Route("notification/{id}")]
        public async Task<IActionResult> Index(Guid id)
        {
            var notification = await _voxedRepository.Notifications.GetById(id);

            if (notification == null)
            {
                return Redirect($"/");
            }

            var voxHash = Core.Shared.GuidConverter.ToShortString(notification.VoxId);
            var commentHash = notification.Comment.Hash;

            new RemoveNotificationModel() { Id = notification.Id.ToString() };

            await _notificationHub.Clients
                .User(notification.UserId.ToString())
                .RemoveNotification(new RemoveNotificationModel() { Id = notification.Id.ToString() });

            
            await _voxedRepository.Notifications.Remove(notification);
            await _voxedRepository.CompleteAsync();
            

            return Redirect($"~/vox/{voxHash}#{commentHash}");
        }

        [AllowAnonymous]
        [Route("notification/delete")]
        public async Task<IActionResult> Delete()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var notifications = await _voxedRepository.Notifications.GetByUserId(user.Id);

            await _voxedRepository.Notifications.RemoveRange(notifications);
            await _voxedRepository.CompleteAsync();

            var returnUrl = Request.Headers["Referer"].ToString();

            return Redirect(returnUrl);
        }
    }
}