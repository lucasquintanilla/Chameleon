using Core.Data.Repositories;
using Core.Entities;
using Core.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IVoxedRepository _voxedRepository;

        public HomeController(ILogger<HomeController> logger, 
            IVoxedRepository voxedRepository)
        {
            _logger = logger;
            _voxedRepository = voxedRepository;
        }

        [HttpGet("v1/voxs")]
        public async Task<IActionResult> Voxs()
        {
            var voxs = await _voxedRepository.Voxs.GetLastestAsync();

            var voxsList = voxs.Select(vox => new Models.VoxResponse()
            {
                Hash = GuidConverter.ToShortString(vox.ID),
                //Hash = x.Hash,
                Status = "1",
                Niche = "20",
                Title = vox.Title,
                Comments = vox.Comments.Count().ToString(),
                Extension = "",
                Sticky = vox.IsSticky ? "1" : "0",
                CreatedAt = vox.CreatedOn.ToString(),
                PollOne = "",
                PollTwo = "",
                Id = "20",
                Slug = vox.Category.ShortName.ToUpper(),
                VoxId = GuidConverter.ToShortString(vox.ID),
                New = vox.CreatedOn.Date == DateTime.Now.Date,
                ThumbnailUrl = vox.Media?.ThumbnailUrl
            }).ToList();

            return Ok(voxsList);
        }

        [HttpGet("v1/vox/{hash}")]
        public async Task<IActionResult> Vox(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voxId = GuidConverter.FromShortString(id);

            var vox = await _voxedRepository.Voxs.GetById(voxId);

            if (vox == null)
            {
                return NotFound();
            }

            if (vox.State == VoxState.Deleted)
            {
                return NotFound();
            }

            return Ok(vox);
        }

        public async Task<IActionResult> Index()
        {
            var voxs = await _voxedRepository.Voxs.GetLastestAsync();

            var voxsList = voxs.Select(vox => new Models.VoxResponse()
            {
                Hash = GuidConverter.ToShortString(vox.ID),
                //Hash = x.Hash,
                Status = "1",
                Niche = "20",
                Title = vox.Title,
                Comments = vox.Comments.Count().ToString(),
                Extension = "",
                Sticky = vox.IsSticky ? "1" : "0",
                CreatedAt = vox.CreatedOn.ToString(),
                PollOne = "",
                PollTwo = "",
                Id = "20",
                Slug = vox.Category.ShortName.ToUpper(),
                VoxId = GuidConverter.ToShortString(vox.ID),
                New = vox.CreatedOn.Date == DateTime.Now.Date,
                ThumbnailUrl = vox.Media?.ThumbnailUrl
            }).ToList();

            return View(voxsList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Info()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
