using Core.Data.Filters;
using Core.Data.Repositories;
using Core.Entities;
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
        Task<CreateVoxResponse> CreateVox(CreateVoxRequest request, Guid userId);
        Task<ListResponse> GetByFilter(VoxFilter filter);
    }

    public class VoxService : IVoxService
    {
        private readonly ILogger<VoxService> _logger;
        private readonly AttachmentService _attachmentService;
        private readonly IVoxedRepository _voxedRepository;
        private readonly FormateadorService _formatterService;
        private readonly INotificationService _notificationService;

        public VoxService(
            ILogger<VoxService> logger,
            AttachmentService attachmentService,
            IVoxedRepository voxedRepository,
            FormateadorService formatterService,
            INotificationService notificationService
            )
        {
            _logger = logger;
            _attachmentService = attachmentService;
            _voxedRepository = voxedRepository;
            _formatterService = formatterService;
            _notificationService = notificationService;
        }

        public async Task<CreateVoxResponse> CreateVox(CreateVoxRequest request, Guid userId)
        {
            var response = new CreateVoxResponse();

            try
            {
                var vox = new Vox()
                {
                    State = VoxState.Active,
                    UserId = userId,
                    Hash = new Hash().NewHash(),
                    Title = request.Title,
                    Content = _formatterService.Parse(request.Content),
                    CategoryId = request.Niche,
                    //IpAddress = UserIpAddress,
                    //UserAgent = UserAgent
                };

                if (request.PollOne != null && request.PollTwo != null)
                {
                    vox.Poll = new Poll()
                    {
                        OptionADescription = request.PollOne,
                        OptionBDescription = request.PollTwo,
                    };
                }

                vox.Attachment = await _attachmentService.ProcessAttachment(request.GetUploadData(), request.File);

                await _voxedRepository.Voxs.Add(vox);
                await _voxedRepository.SaveChangesAsync();
                await _notificationService.NotifyClients(vox.Id);

                response.VoxHash = GuidConverter.ToShortString(vox.Id);
                response.Status = true;
            }
            catch (NotImplementedException e)
            {
                response.Swal = e.Message;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                response.Swal = "error";
            }

            return response;
        }

        public async Task<ListResponse> GetByFilter(VoxFilter filter)
        {
            var voxs = await _voxedRepository.Voxs.GetByFilterAsync(filter);
            return new ListResponse(VoxedMapper.Map(voxs));
        }
    }
}
