using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voxed.WebApp.Models;

namespace Voxed.WebApp.Hubs
{
    public interface INotificationHub
    {
        //Task ReceiveNotification(Notification notification);
        Task Comment(CommentNotification comment);
    }
}
