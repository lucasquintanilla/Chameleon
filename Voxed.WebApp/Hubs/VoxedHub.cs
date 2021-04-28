using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voxed.WebApp.Controllers;
using Voxed.WebApp.Models;

namespace Voxed.WebApp.Hubs
{
    public class VoxedHub : Hub<INotificationHub>
    {
        private static HashSet<string> usersOnline = new HashSet<string>();

        public int TotalUsersOnline => VoxedHub.usersOnline.Count;

        public async Task SendMessage(CommentNotification comment)
        {
            await Clients.All.Comment(comment);
        }

        public async Task SendOPCommentNotification(Core.Entities.User user, Notification notification)
        {
            await Clients.Users(user.Id.ToString()).Notification(notification);
        }

        public async Task HomeNewVoxEvent(VoxResponse notification)
        {
            await Clients.All.Vox(notification);
        }

        public async Task SuscribeToVox(string voxId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, voxId);
        }

        public override async Task OnConnectedAsync()
        {
            usersOnline.Add(Context.GetHttpContext().Connection.RemoteIpAddress.MapToIPv4().ToString());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            usersOnline.Remove(Context.GetHttpContext().Connection.RemoteIpAddress.MapToIPv4().ToString());
            await base.OnDisconnectedAsync(exception);
        }
    }

    public class Notification
    {
        public string Type { get; set; }
        public Content Content { get; set; }
    }

    public class VoxNotification
    {
        public string Niche { get; set; }
       
    }

    public class Content
    {
        public string Id { get; set; }
        public string VoxHash { get; set; }
        public string NotificationBold { get; set; }
        public string NotificationText { get; set; }
        public string ContentHash { get; set; }
        public string Count { get; set; }
        public string ThumbnailUrl { get; set; }
    }
    public class CommentNotification 
    {
        public string Id { get; set; }
        public string Hash { get; set; }
        public string UniqueId { get; set; }
        public string VoxHash { get; set; }
        public string AvatarColor { get; set; }
        public bool IsOp { get; set; }
        public string Tag { get; set; }
        public string UniqueColor { get; set; }
        public string UniqueColorContrast { get; set; }
        public string Name { get; set; }
        public string CreatedAt { get; set; }
        public string Poll { get; set; }
        public string Extension { get; set; }
        public string ExtensionData { get; set; }
        public string Via { get; set; } // es una url ??
        public string Content { get; set; }
        public string MediaUrl { get; set; }
        public string MediaThumbnailUrl { get; set; }
    }
}
