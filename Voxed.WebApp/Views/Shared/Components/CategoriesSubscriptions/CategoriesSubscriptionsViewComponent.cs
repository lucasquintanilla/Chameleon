using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Voxed.WebApp.Views.Shared.Components.CategoriesSubscriptions
{
    public class CategoriesSubscriptionsViewComponent : ViewComponent
    {
        private readonly IBlogRepository _blogRepository;
        private static IEnumerable<Category> _categories;

        public CategoriesSubscriptionsViewComponent(
            IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            _categories ??= await _blogRepository.Categories.GetAll();

            return View(_categories);
        }
    }
}
