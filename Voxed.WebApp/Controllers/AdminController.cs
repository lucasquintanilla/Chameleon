using Core.Data.EF;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Controllers
{
    [Authorize(Roles = nameof(RoleType.Administrator))]
    public class AdminController : Controller
    {
        private readonly IVoxedRepository _voxedRepository;
        private readonly IWebHostEnvironment _env;

        public AdminController(IVoxedRepository voxedRepository, IWebHostEnvironment env)
        {
            _voxedRepository = voxedRepository;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<Response> Delete([FromForm] Request request)
        {
            try
            {
                //comment
                if (request.ContentType == "0")
                {
                    var comment = await _voxedRepository.Comments.GetById(new Guid(request.ContentId));
                    if (comment == null)
                    {
                        NotFound();
                    }

                    comment.State = CommentState.Deleted;

                    if (comment.MediaID.HasValue)
                    {
                        await BanMedia(comment.MediaID.Value);
                    }

                    await UpdateVoxLastBump(comment);

                    await _voxedRepository.SaveChangesAsync();
                }

                if (request.ContentType == "1")
                {
                    var vox = await _voxedRepository.Voxs.GetById(new Guid(request.ContentId));
                    if (vox == null)
                    {
                        NotFound();
                    }

                    await BanMedia(vox.MediaID);

                    vox.State = Core.Entities.VoxState.Deleted;
                    await _voxedRepository.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           

            return new Response() { Value = "OK" };
        }

        private async Task UpdateVoxLastBump(Comment comment)
        {
            var vox = await _voxedRepository.Voxs.GetById(comment.VoxID);

            if (vox == null)
            {
                return;
            }

            var lastBump = vox.Comments
                .Where(x => x.State == CommentState.Normal)
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => x.CreatedOn)
                .First();

            vox.Bump = lastBump;
        }

        private async Task BanMedia(Guid mediaId)
        {
            var media = await _voxedRepository.Media.GetById(mediaId);

            if (media.MediaType == MediaType.Image)
            {
                string destination = Path.Combine(_env.WebRootPath, "media", "banned");
                var filename = Core.Utilities.Utilities.GetFileNameFromUrl(media.Url);

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

    public class Request
    {
        public string ContentType { get; set; } //puede ser 0:comment o 1
        public string ContentId { get; set; }
    }

    public class Response
    {
        public string Value { get; set; }
    }
}
