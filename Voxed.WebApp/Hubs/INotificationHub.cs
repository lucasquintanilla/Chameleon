using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voxed.WebApp.Controllers;
using Voxed.WebApp.Models;

namespace Voxed.WebApp.Hubs
{
    public interface INotificationHub
    {
        Task Comment(CommentNotification comment);
        Task Notification(OpNotification notification);
        Task Vox(VoxResponse notification);
    }
}

