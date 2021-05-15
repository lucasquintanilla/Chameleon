using System.Threading.Tasks;

namespace Voxed.WebApp.Hubs
{
    public interface INotificationHub
    {
        Task Comment(CommentNotification comment);
        Task Notification(UserNotification notification);
        Task Vox(Models.VoxResponse notification);
    }
}

