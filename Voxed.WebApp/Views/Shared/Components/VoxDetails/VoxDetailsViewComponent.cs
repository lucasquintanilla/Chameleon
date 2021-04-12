using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Views.Shared.Components.VoxDetails
{
    public class VoxDetailsViewComponent : ViewComponent
    {
        private readonly IVoxedRepository voxedRepository;

        public VoxDetailsViewComponent(
                IVoxedRepository voxedRepository
            )
        {
            this.voxedRepository = voxedRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<Vox> voxs)
        {
            return View(voxs);
        }
    }
}
