using Core.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Voxed.WebApp.Data;

namespace Voxed.WebApp.Views.Shared.Components.CategoriesMenuList
{
    public class CategoriesMenuListViewComponent : ViewComponent
    {
        private readonly IVoxedRepository voxedRepository;

        public CategoriesMenuListViewComponent(
            IVoxedRepository voxedRepository)
        {
            this.voxedRepository = voxedRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //var categories = await voxedRepository.Categories.GetAll();
            //return View(categories);
            return View();
        }
    }
}
