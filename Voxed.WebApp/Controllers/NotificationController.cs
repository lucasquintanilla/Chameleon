using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Voxed.WebApp.Controllers
{
    public class NotificationController : BaseController
    {
        private IVoxedRepository _voxedRepository;
        private readonly UserManager<User> _userManager;

        public NotificationController(IVoxedRepository voxedRepository, 
            UserManager<User> userManager)
        {
            _voxedRepository = voxedRepository;
            _userManager = userManager;
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
            var voxHash = Core.Shared.GuidConverter.ToShortString(notification.VoxId);
            var commentHash = notification.Comment.Hash;

            if (notification != null)
            {
                await _voxedRepository.Notifications.Remove(notification);
                await _voxedRepository.CompleteAsync();
            }

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