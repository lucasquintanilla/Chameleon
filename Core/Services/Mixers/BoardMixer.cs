using Core.DataSources.Devox;
using Core.DataSources.Devox.Helpers;
using Core.DataSources.Devox.Models;
using Core.DataSources.Ufftopia;
using Core.Entities;
using Core.Services.Mixers.Models;
using Core.Services.Posts;
using Core.Services.Posts.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Services.Mixers
{
    public interface IMixer
    {
        Task<Mix> GetMix();
    }

    public class BoardMixer : IMixer
    {
        private readonly IDevoxDataSource _devoxDataSource;
        private readonly IPostService _postService;
        private readonly IUfftopiaDataSource _ufftopiaDataSource;
        private readonly ILogger<BoardMixer> _logger;
        private List<DevoxPost> _devoxPosts = new();
        private IEnumerable<KeyValuePair<string, DevoxPost>> _devoxPostsDictionary;

        public BoardMixer(
            IUfftopiaDataSource ufftopiaDataSource,
            IDevoxDataSource devoxDataSource,
            ILogger<BoardMixer> logger,
            IPostService postService)
        {
            _ufftopiaDataSource = ufftopiaDataSource;
            _devoxDataSource = devoxDataSource;
            _logger = logger;
            _postService = postService;
        }

        public async Task<Mix> GetMix()
        {
            var mix = new Mix();
            var tasks = new List<Task<IEnumerable<MixItem>>>
            {
                GetDevoxs(),
                //GetUpptopia()
            };

            try
            {
                foreach (var items in await Task.WhenAll(tasks))
                    mix.Items.AddRange(items);
            }
            catch (Exception e)
            {
                //_logger.LogWarning(e.ToString());
            }

            return mix;
        }

        private async Task<IEnumerable<MixItem>> GetUpptopia()
        {
            try
            {
                var posts = await _ufftopiaDataSource.Fetch();

                return posts.Select(post => new MixItem()
                {
                    Hash = post.Id,
                    //Status = true,
                    Niche = post.CategoriaId.ToString(),
                    Title = post.Titulo,
                    Comments = post.CantidadComentarios.ToString(),
                    Extension = string.Empty,
                    //Sticky = vox.IsSticky ? "1" : "0",
                    //CreatedAt = vox.CreatedOn.ToString(),
                    PollOne = string.Empty,
                    PollTwo = string.Empty,
                    Id = post.Id,
                    Slug = "ufftopia",
                    VoxId = post.Id.ToString(),
                    //New = vox.CreatedOn.IsNew(),
                    ThumbnailUrl = "https://ufftopia.net" + post.Media.VistaPreviaCuadrado,
                    Category = post.CategoriaId.ToString(),
                    Href = "https://ufftopia.net/Hilo/" + post.Id,
                    LastActivityOn = post.Bump

                });
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.ToString());
                return Enumerable.Empty<MixItem>();
            }
        }

        private async Task<IEnumerable<MixItem>> GetDevoxs()
        {
            try
            {
                //if (!_devoxPosts.Any() || _devoxPosts.Any(post => (DateTime.Now - post.LastUpdate) > TimeSpan.FromMinutes(5)))
                //{
                //    var posts = await _devoxDataSource.GetPosts();
                //    //_devoxPostsDictionary = _devoxPostsDictionary.Union(posts.ToDictionary(x => x.Id));
                //    //_devoxPostsDictionary.Where(x=>x.Value.LastUpdate.Day == 4).Take(5).ToList();
                //    _devoxPosts.AddRange(posts);
                //}
                var posts = await _devoxDataSource.GetPosts();
                return posts.Select(post => new MixItem()
                {
                    Hash = post.Id ?? post.Filename,
                    //Status = true,
                    Niche = post.Category.ToString(),
                    Title = post.Title,
                    Comments = post.CommentsCount.ToString(),
                    Extension = string.Empty,
                    //Sticky = vox.IsSticky ? "1" : "0",
                    //CreatedAt = vox.CreatedOn.ToString(),
                    PollOne = string.Empty,
                    PollTwo = string.Empty,
                    Id = post.Id ?? post.Filename,
                    Slug = DataSources.Devox.Constants.Domain,
                    VoxId = post.Id?.ToString() ?? post.Filename,
                    //New = vox.Date,
                    ThumbnailUrl = DevoxHelpers.GetThumbnailUrl(post),
                    Category = post.Category.ToString(),
                    Href = $"{DataSources.Devox.Constants.VoxEnpoint}{post.Filename}",
                    LastActivityOn = post.LastUpdate
                }).ToList();

                //var post = posts.FirstOrDefault();

                //var newpost = new CreatePostRequest()
                //{
                //    UserId = Guid.Parse("8fa680f2-d861-47e3-98a8-146d3cb4fa40"),
                //    Title = post.Title,
                //    File = null,
                //    CategoryId = 39,
                //};
                //await _postService.CreatePost(newpost);
                //return Enumerable.Empty<MixItem>();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Enumerable.Empty<MixItem>();
            }
        }
    }
}
