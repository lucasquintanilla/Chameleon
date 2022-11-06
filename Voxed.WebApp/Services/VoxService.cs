using Core.Data.Filters;
using Core.Data.Repositories;
using Core.Entities;
using Core.Services.AttachmentServices;
using Core.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Voxed.WebApp.Mappers;
using Voxed.WebApp.Models;

namespace Voxed.WebApp.Services
{
    public interface IVoxService
    {
        Task<Guid> CreateVox(CreateVoxRequest request, Guid userId);
        Task<LoadMoreResponse> GetByFilter(VoxFilter filter);
    }

    public class VoxService : IVoxService
    {
        private readonly ILogger<VoxService> _logger;
        private readonly IAttachmentService _attachmentService;
        private readonly IVoxedRepository _voxedRepository;
        private readonly IContentFormatterService _formatterService;
        private readonly INotificationService _notificationService;

        public VoxService(
            ILogger<VoxService> logger,
            IAttachmentService attachmentService,
            IVoxedRepository voxedRepository,
            IContentFormatterService formatterService,
            INotificationService notificationService
            )
        {
            _logger = logger;
            _attachmentService = attachmentService;
            _voxedRepository = voxedRepository;
            _formatterService = formatterService;
            _notificationService = notificationService;
        }

        public async Task<Guid> CreateVox(CreateVoxRequest request, Guid userId)
        {
            var attachment = await _attachmentService.ProcessAttachment(request.GetVoxedAttachment(), request.File);
            await _voxedRepository.Media.Add(attachment);

            var vox = new Vox()
            {
                State = VoxState.Active,
                UserId = userId,
                Title = request.Title,
                Content = _formatterService.Format(request.Content),
                CategoryId = request.Niche,
                AttachmentId = attachment.Id,
                //IpAddress = UserIpAddress,
                //UserAgent = UserAgent
            };

            await _voxedRepository.Voxs.Add(vox);
            await _voxedRepository.SaveChangesAsync();
            await _notificationService.NotifyClients(vox.Id);

            return vox.Id;
        }

        public async Task<LoadMoreResponse> GetByFilter(VoxFilter filter)
        {
            var voxs = await _voxedRepository.Voxs.GetByFilterAsync(filter);
            return new LoadMoreResponse(VoxedMapper.Map(voxs));
        }
    }
}
