using Core.Data.Repositories;
using Core.Entities;
using Core.Shared;
using Core.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;
using Voxed.WebApp.Constants;
using Voxed.WebApp.Extensions;
using Voxed.WebApp.Hubs;
using Voxed.WebApp.Mappers;
using Voxed.WebApp.Models;

namespace Voxed.WebApp.Services
{
    public interface INotificationService
    {
        Task NotifyClients(Guid voxId);
        Task ManageNotifications(Vox vox, Comment comment);
        Task SendBoardUpdate(Comment comment, Vox vox, CreateCommentRequest request);
    }

    public class NotificationService : INotificationService
    {
        private readonly IVoxedRepository _voxedRepository;
        private readonly IHubContext<VoxedHub, INotificationHub> _notificationHub;
        private readonly FormateadorService _formateadorService;

        public NotificationService(
            IVoxedRepository voxedRepository,
            IHubContext<VoxedHub, INotificationHub> notificationHub,
            FormateadorService formateadorService
            )
        {
            _voxedRepository = voxedRepository;
            _notificationHub = notificationHub;
            _formateadorService = formateadorService;
        }

        public async Task NotifyClients(Guid voxId)
        {
            //disparo notificacion del vox
            var vox = await _voxedRepository.Voxs.GetById(voxId); // Ver si se puede remover

            if (!Categories.HiddenCategories.Contains(vox.CategoryId))
            {
                //var voxToHub = ConvertoToVoxResponse(vox);
                var voxToHub = VoxedMapper.Map(vox);
                await _notificationHub.Clients.All.Vox(voxToHub);
            }
        }

        public async Task ManageNotifications(Vox vox, Comment comment)
        {
            //new Thread(async () =>
            //{
            //    Thread.CurrentThread.IsBackground = true;

            //    await Task.Delay(8000);

            //    Debug.WriteLine("Hello, world");                

            //}).Start();

            var notifications = new NotificationBuilder()
                    .WithVox(vox)
                    .WithComment(comment)
                    .UseRepository(_voxedRepository)
                    .UseFormatter(_formateadorService)
                    .AddReplies()
                    .AddOPNotification()
                    .AddVoxSusbcriberNotifications()
                    .Save();

            var sender = new NotificationSender()
                .WithVox(vox)
                .WithComment(comment)
                .WithNotifications(notifications)
                .UseHub(_notificationHub);

            sender.Notify();
        }

        public async Task SendBoardUpdate(Comment comment, Vox vox, CreateCommentRequest request)
        {
            if (Categories.HiddenCategories.Contains(vox.CategoryId)) return;            

            var commentUpdate = new CommentLiveUpdate()
            {
                UniqueId = null, //si es unique id puede tener colores unicos
                UniqueColor = null,
                UniqueColorContrast = null,

                Id = comment.Id.ToString(),
                Hash = comment.Hash,
                VoxHash = GuidConverter.ToShortString(vox.Id),
                AvatarColor = comment.Style.ToString().ToLower(),
                IsOp = vox.UserId == comment.UserId && vox.User.UserType != UserType.Anonymous, //probar cambiarlo cuando solo pruedan craer los usuarios.
                Tag = UserViewHelper.GetUserTypeTag(comment.User.UserType), //admin o dev               
                Content = comment.Content ?? String.Empty,
                Name = UserViewHelper.GetUserName(comment.User),
                CreatedAt = comment.CreatedOn.DateTime.ToTimeAgo(),
                Poll = null, //aca va una opcion respondida

                //Media
                MediaUrl = comment.Media?.Url,
                MediaThumbnailUrl = comment.Media?.ThumbnailUrl,
                Extension = request.GetUploadData()?.Extension == UploadDataExtension.Base64 ? Core.Utilities.Utilities.GetFileExtensionFromUrl(comment.Media?.Url) : request.GetUploadData()?.Extension,
                ExtensionData = request.GetUploadData()?.ExtensionData,
                Via = request.GetUploadData()?.Extension == UploadDataExtension.Youtube ? comment.Media?.Url : null,
            };

            await _notificationHub.Clients.All.Comment(commentUpdate);
        }
    }
}
