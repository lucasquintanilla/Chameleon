using System;
using System.Threading.Tasks;

namespace Voxed.WebApp.Hubs
{
    public interface INotificationHub
    {
        Task Comment(CommentLiveUpdate comment);
        Task Notification(UserNotification notification);
        Task RemoveNotification(RemoveNotificationModel removeNotification);
        Task Vox(Models.VoxResponse notification);
    }
}

