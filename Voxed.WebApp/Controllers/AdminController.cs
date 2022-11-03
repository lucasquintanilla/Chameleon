using Core.Data.EF;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Voxed.WebApp.Constants;
using Voxed.WebApp.Models;

namespace Voxed.WebApp.Controllers
{
    [Authorize(Roles = nameof(RoleType.Administrator))]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IVoxedRepository _voxedRepository;
        private readonly IWebHostEnvironment _env;

        public AdminController(
            IVoxedRepository voxedRepository, 
            IWebHostEnvironment env, 
            ILogger<AdminController> logger)
        {
            _logger = logger;
            _voxedRepository = voxedRepository;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<DeleteResponse> Delete([FromForm] DeleteRequest request)
        {
            try
            {
                switch (request.ContentType)
                {
                    case ContentType.Comment:

                        var comment = await _voxedRepository.Comments.GetById(new Guid(request.ContentId));
                        if (comment == null) NotFound();                        

                        comment.State = CommentState.Deleted;

                        if (comment.MediaId.HasValue)
                        {
                            await BanMedia(comment.MediaId.Value);
                        }

                        await UpdateVoxLastBump(comment);

                        await _voxedRepository.SaveChangesAsync();

                        break;
                    case ContentType.Vox:

                        var vox = await _voxedRepository.Voxs.GetById(new Guid(request.ContentId));
                        if (vox == null)
                        {
                            NotFound();
                        }

                        await BanMedia(vox.MediaId);

                        vox.State = VoxState.Deleted;
                        await _voxedRepository.SaveChangesAsync();

                        break;
                    default:
                        break;
                }

                return new DeleteResponse() { Value = "OK" };
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogError(e.StackTrace);
                return new DeleteResponse() { Value = "Error" };
            }
        }

        private async Task UpdateVoxLastBump(Comment comment)
        {
            var vox = await _voxedRepository.Voxs.GetById(comment.VoxId);

            if (vox == null) return;            

            var lastBump = vox.Comments
                .Where(x => x.State == CommentState.Active)
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => x.CreatedOn)
                .FirstOrDefault();

            vox.Bump = lastBump;
        }

        private async Task BanMedia(Guid mediaId)
        {

            var media = await _voxedRepository.Media.GetById(mediaId);

            if (media == null) return;

            if (media.Type == AttachmentType.Image)
            {
                string destination = Path.Combine(_env.WebRootPath, "media", "banned");
                var filename = Core.Utilities.UrlUtility.GetFileNameFromUrl(media.Url);

                try
                {
                    System.IO.File.Copy(Path.Combine(_env.WebRootPath, "media", filename), Path.Combine(destination, filename));
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
