using Core.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voxed.WebApp.Models;

namespace Voxed.WebApp.Views.Shared.Components.VoxDetails
{
    public class VoxDetailsViewComponent : ViewComponent
    {
        private readonly IBlogRepository blogRepository;
        public static string Name = "VoxDetails";

        public VoxDetailsViewComponent(IBlogRepository blogRepository)
        {
            this.blogRepository = blogRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<IBoardPostViewModel> voxs)
        {
            await Task.Run(() => Console.WriteLine());
            return View(voxs);
        }
    }
}
