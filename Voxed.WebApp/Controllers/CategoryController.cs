using Core.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
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
                return NotFound();
            }

            // chaear las categorias y luego buscar los vox por categoryId
            var exists = await _voxedRepository.Categories.Exists(shortName);

            if (!exists)
            {
                return NotFound();
            }

            var voxs = await _voxedRepository.Voxs.GetByCategoryShortNameAsync(shortName);
            return View(VoxedMapper.Map(voxs));
        }
    }
}
