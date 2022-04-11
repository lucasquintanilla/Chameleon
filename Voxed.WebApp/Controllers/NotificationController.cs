using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Voxed.WebApp.Extensions;
using Voxed.WebApp.Hubs;

namespace Voxed.WebApp.Controllers
{
    [Route("notification")]
    public class NotificationController : BaseController
    {
        private IVoxedRepository _voxedRepository;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<VoxedHub, INotificationHub> _notificationHub;

        public NotificationController(IVoxedRepository voxedRepository,
            UserManager<User> userManager,
            IHubContext<VoxedHub,
            INotificationHub> notificationHub,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _voxedRepository = voxedRepository;
            _userManager = userManager;
            _notificationHub = notificationHub;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var notification = await _voxedRepository.Notifications.GetById(id);
            if (notification == null) { return Redirect($"/"); }

            //var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = User.GetLoggedInUserId<Guid?>();
            if (userId != notification.UserId) return Redirect("/");

            var voxHash = Core.Shared.GuidConverter.ToShortString(notification.VoxId);
            var commentHash = notification.Comment.Hash;

            new RemoveNotificationModel() { Id = notification.Id.ToString() };

            await _notificationHub.Clients
                .User(notification.UserId.ToString())
                .RemoveNotification(new RemoveNotificationModel() { Id = notification.Id.ToString() });


            await _voxedRepository.Notifications.Remove(notification);
            await _voxedRepository.SaveChangesAsync();


            return Redirect($"~/vox/{voxHash}#{commentHash}");
        }

        [AllowAnonymous]
        [Route("delete")]
        public async Task<IActionResult> DeleteAll()
        {
            //var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = User.GetLoggedInUserId<Guid?>();

            if (userId == null)
            {
                return BadRequest();
            }

            var notifications = await _voxedRepository.Notifications.GetByUserId(userId.Value);

            await _voxedRepository.Notifications.RemoveRange(notifications);
            await _voxedRepository.SaveChangesAsync();

            var returnUrl = Request.Headers["Referer"].ToString();

            return Redirect(returnUrl);
        }
    }
}