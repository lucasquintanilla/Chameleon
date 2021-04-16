using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voxed.WebApp.Models;

namespace Voxed.WebApp.Hubs
{
    public class VoxedHub : Hub<INotificationHub>
    {
        //public async Task SendMessage(Notification notification)
        //{
        //    await Clients.All.ReceiveNotification(notification);
        //}

        public async Task SendMessage(CommentNotification comment)
        {
            await Clients.All.Comment(comment);
        }
    }

    public class CommentNotification {

        public string Hash { get; set; }
    }
}
