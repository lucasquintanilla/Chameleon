using Core.Data.Filters;
using Core.Data.Repositories;
using Core.Entities;
using Core.Services.AttachmentServices;
using Core.Services.AttachmentServices.Models;
using Core.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voxed.WebApp.Models;

namespace Voxed.WebApp.Services
{
    public interface IPostService
    {
        Task<Vox> CreatePost(CreateVoxRequest request, Guid userId, string userIpAddress, string userAgent);
        Task<IEnumerable<Vox>> GetByFilter(PostFilter filter);
    }

    public class PostService : IPostService
    {
        private readonly ILogger<PostService> _logger;
        private readonly IAttachmentService _attachmentService;
        private readonly IVoxedRepository _voxedRepository;
        private readonly IContentFormatterService _formatterService;
        private readonly IServiceScopeFactory _scopeFactory;

        public PostService(
            ILogger<PostService> logger,
            IAttachmentService attachmentService,
            IVoxedRepository voxedRepository,
            IContentFormatterService formatterService,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _attachmentService = attachmentService;
            _voxedRepository = voxedRepository;
            _formatterService = formatterService;
            _scopeFactory = scopeFactory;
        }

        public async Task<Vox> CreatePost(CreateVoxRequest request, Guid userId, string userIpAddress, string userAgent)
        {
            var voxedAttachment = request.GetVoxedAttachment();

            var postAttachment = new CreateAttachmentRequest()
            {
                Extension = voxedAttachment.Extension,
                ExtensionData = voxedAttachment.ExtensionData,
                File = request.File
            };

            var attachment = await _attachmentService.CreateAttachment(postAttachment);
            await _voxedRepository.Media.Add(attachment);

            var vox = new Vox()
            {
                State = VoxState.Active,
                UserId = userId,
                Title = request.Title,
                Content = _formatterService.Format(request.Content),
                CategoryId = request.Niche,
                AttachmentId = attachment.Id,
                IpAddress = userIpAddress,
                UserAgent = userAgent
            };

            await _voxedRepository.Voxs.Add(vox);
            await _voxedRepository.SaveChangesAsync();
            _ = Task.Run(() => NotifyPostCreated(vox.Id));

            return vox;
        }

        private async Task NotifyPostCreated(Guid voxId)
        {
            using var scope = _scopeFactory.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            await notificationService.NotifyPostCreated(voxId);
        }

        public async Task<IEnumerable<Vox>> GetByFilter(PostFilter filter)
        {
            return await _voxedRepository.Voxs.GetByFilterAsync(filter);
        }
    }

    public class CreatePostRequest
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int CategoryId { get; set; }
        public string UserAgent { get; set; }
        public string IpAddress { get; set; }
    }


}
