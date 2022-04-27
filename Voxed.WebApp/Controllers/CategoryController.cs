using Core.Data.Filters;
using Core.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voxed.WebApp.Mappers;

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
                return BadRequest();
            }

            var category = await _voxedRepository.Categories.GetByShortName(shortName);

            if (category == null)
            {
                return NotFound();
            }

            var filter = new VoxFilter() { Categories = new List<int>() { category.ID } };

            var voxs = await _voxedRepository.Voxs.GetByFilterAsync(filter);
            return View(VoxedMapper.Map(voxs));
        }
    }
}
