using Core.Data.Repositories;
using Core.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Voxed.WebApp.Extensions;

namespace Voxed.WebApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IVoxedRepository _voxedRepository;

        public CategoryController(
            IVoxedRepository voxedRepository
            )
        {
            _voxedRepository = voxedRepository;
        }

        [Route("/{shortName}")]
        public async Task<IActionResult> Details(string shortName)
        {
            if (shortName == null)
            {
                return NotFound();
            }

            var exists = await _voxedRepository.Categories.Exists(shortName);

            if (!exists)
            {
                return NotFound();
            }

            var voxs = await _voxedRepository.Voxs.GetByCategoryShortNameAsync(shortName);

            var voxsList = voxs.Select(x => new Models.VoxResponse()
            {
                Hash = GuidConverter.ToShortString(x.ID),
                //Status = "1",
                Status = true,
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
                New = x.CreatedOn.IsNew(),
                ThumbnailUrl = x.Media?.ThumbnailUrl,
                Category = x.Category.Name
            }).ToList();

            return View(voxsList);
        }
    }
}
