using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Views.Shared.Components.CategoriesDropdown
{
    public class CategoriesDropdownViewComponent : ViewComponent
    {
        private readonly IBlogRepository _blogRepository;
        private static IEnumerable<Category> _categories;

        public CategoriesDropdownViewComponent(
            IBlogRepository voxedRepository)
        {
            _blogRepository = voxedRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            _categories = _categories ?? await _blogRepository.Categories.GetAll();
            return View(_categories);
        }
    }
}
