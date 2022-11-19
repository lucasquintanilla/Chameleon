using Core.Data.Filters;
using Core.Data.Repositories;
using Core.Entities;
using Core.Services.AttachmentServices;
using Core.Services.AttachmentServices.Models;
using Core.Services.Post.Models;
using Core.Shared;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services.Post
{
    public interface IPostService
    {
        Task<Vox> CreatePost(CreatePostRequest request);
        Task<IEnumerable<Vox>> GetByFilter(PostFilter filter);
    }

    public class PostService : IPostService
    {
        private readonly ILogger<PostService> _logger;
        private readonly IAttachmentService _attachmentService;
        private readonly IVoxedRepository _voxedRepository;
        private readonly IContentFormatterService _formatterService;

        public PostService(
            ILogger<PostService> logger,
            IAttachmentService attachmentService,
            IVoxedRepository voxedRepository,
            IContentFormatterService formatterService)
        {
            _logger = logger;
            _attachmentService = attachmentService;
            _voxedRepository = voxedRepository;
            _formatterService = formatterService;
        }

        public async Task<Vox> CreatePost(CreatePostRequest request)
        {
            var postAttachment = new CreateAttachmentRequest()
            {
                Extension = request.Extension,
                ExtensionData = request.ExtensionData,
                File = request.File
            };

            var attachment = await _attachmentService.CreateAttachment(postAttachment);
            await _voxedRepository.Media.Add(attachment);

            var vox = new Vox()
            {
                State = VoxState.Active,
                UserId = request.UserId,
                Title = request.Title,
                Content = _formatterService.Format(request.Content),
                CategoryId = request.CategoryId,
                AttachmentId = attachment.Id,
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent
            };

            await _voxedRepository.Voxs.Add(vox);
            await _voxedRepository.SaveChangesAsync();
            return vox;
        }

        public async Task<IEnumerable<Vox>> GetByFilter(PostFilter filter)
        {
            return await _voxedRepository.Voxs.GetByFilterAsync(filter);
        }
    }
}
