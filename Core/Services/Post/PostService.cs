﻿using Core.Data.Filters;
using Core.Data.Repositories;
using Core.Entities;
using Core.Services.MediaServices.Models;
using Core.Services.MediaServices;
using Core.Services.Post.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Services.TextFormatter;

namespace Core.Services.Post
{
    public interface IPostService
    {
        Task<Entities.Post> CreatePost(CreatePostRequest request);
        Task<IEnumerable<Entities.Post>> GetByFilter(PostFilter filter);
    }

    public class PostService : IPostService
    {
        private readonly ILogger<PostService> _logger;
        private readonly IMediaService _mediaService;
        private readonly IVoxedRepository _voxedRepository;
        private readonly ITextFormatterService _textFormatter;

        public PostService(
            ILogger<PostService> logger,
            IMediaService mediaService,
            IVoxedRepository voxedRepository,
            ITextFormatterService textFormatter)
        {
            _logger = logger;
            _mediaService = mediaService;
            _voxedRepository = voxedRepository;
            _textFormatter = textFormatter;
        }

        public async Task<Entities.Post> CreatePost(CreatePostRequest request)
        {
            var mediaRequest = new CreateMediaRequest()
            {
                Extension = request.Extension,
                ExtensionData = request.ExtensionData,
                File = request.File
            };

            var media = await _mediaService.CreateMedia(mediaRequest);
            await _voxedRepository.Media.Add(media);

            var vox = new Entities.Post()
            {
                State = PostState.Active,
                UserId = request.UserId,
                Title = request.Title,
                Content = _textFormatter.Format(request.Content),
                CategoryId = request.CategoryId,
                MediaId = media.Id,
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent
            };

            await _voxedRepository.Posts.Add(vox);
            await _voxedRepository.SaveChangesAsync();
            return vox;
        }

        public async Task<IEnumerable<Entities.Post>> GetByFilter(PostFilter filter)
        {
            return await _voxedRepository.Posts.GetByFilterAsync(filter);
        }
    }
}
