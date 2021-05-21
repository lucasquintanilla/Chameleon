using Core.Data.Repositories;
using Core.Entities;
using Core.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Controllers
{
    public class CategoryController : Controller
    {
        private IVoxedRepository voxedRepository;
       
        public CategoryController(
            IVoxedRepository voxedRepository)
        {
            this.voxedRepository = voxedRepository;
        }

        [Route("/{shortName}")]
        public async Task<IActionResult> Details(string shortName)
        {
            if (shortName == null)
            {
                return NotFound();
            }

            var exists = await voxedRepository.Categories.Exists(shortName);

            if (!exists)
            {
                return NotFound();
            }

            var voxs = await voxedRepository.Voxs.GetByCategoryShortNameAsync(shortName);

            var voxsList = voxs.Select(x => new Models.VoxResponse()
            {
                Hash = GuidConverter.ToShortString(x.ID),
                Status = "1",
                Niche = "20",
                Title = x.Title,
                Comments = x.Comments.Count().ToString(),
                Extension = "",
                Sticky = x.IsSticky ? "1" : "0",
                CreatedAt = x.CreatedOn.ToString(),
                PollOne = "",
                PollTwo = "",
                Id = "20",
                Slug = x.Category.ShortName.ToUpper(),
                VoxId = GuidConverter.ToShortString(x.ID),
                New = x.CreatedOn.Date == DateTime.Now.Date,
                ThumbnailUrl = x.Media?.ThumbnailUrl,
                Category = x.Category.Name
            }).ToList();

            return View(voxsList);
        }
    }
}
