﻿using Core.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voxed.WebApp.Models;

namespace Voxed.WebApp.Views.Shared.Components.VoxDetails
{
    public class VoxDetailsViewComponent : ViewComponent
    {
        private readonly IVoxedRepository voxedRepository;

        public VoxDetailsViewComponent(IVoxedRepository voxedRepository)
        {
            this.voxedRepository = voxedRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<VoxResponse> voxs)
        {
            return View(voxs);
        }
    }
}
