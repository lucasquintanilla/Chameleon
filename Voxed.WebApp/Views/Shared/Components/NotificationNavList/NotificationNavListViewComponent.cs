using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voxed.WebApp.Extensions;
using Voxed.WebApp.Hubs;
using Voxed.WebApp.Mappers;

namespace Voxed.WebApp.Views.Shared.Components.NotificationNavList
{
    public class NotificationNavListViewComponent : ViewComponent
    {
        private readonly IVoxedRepository _voxedRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public NotificationNavListViewComponent(IVoxedRepository voxedRepository,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _voxedRepository = voxedRepository;
            _userManager = userManager;
            _mapper = mapper;
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
                .Select(_mapper.Map);

            return View(userNotifications);
        }
    }
}
