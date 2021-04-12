using Core.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Voxed.WebApp.Data;
using Voxed.WebApp.Models;

namespace Voxed.WebApp.Views.Shared.Components.VoxForm
{
    public class VoxFormViewComponent : ViewComponent
    {
        private readonly IVoxedRepository voxedRepository;

        public VoxFormViewComponent(
                IVoxedRepository voxedRepository
            )
        {
            this.voxedRepository = voxedRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var form = new VoxFormViewModel();

            //var categories = await voxedRepository.Categories.GetAll();

            //form.Categories = categories
            //    .Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Name }).ToList();

            ////Agrega una opcion como placeholder deshabilitado en el dropdownlist
            //form.Categories
            //    .Insert(0, new SelectListItem { 
            //        Value = "0", 
            //        Text = "Elige una categoría", 
            //        Selected = true, 
            //        Disabled=true 
            //    });
            //form.CategoryID = 0; //selecciona el item 0 como default selected

            return View(form);
        }
    }
}
