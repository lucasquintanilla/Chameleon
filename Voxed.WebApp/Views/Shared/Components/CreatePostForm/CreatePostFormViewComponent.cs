﻿using Core.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
//using Voxed.WebApp.Data;
using Voxed.WebApp.Models;

namespace Voxed.WebApp.Views.Shared.Components.VoxForm
{
    public class CreatePostFormViewComponent : ViewComponent
    {
        private readonly IBlogRepository blogRepository;
        public static string Name = "CreatePostForm";
        public CreatePostFormViewComponent(
            IBlogRepository blogRepository)
        {
            this.blogRepository = blogRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            await Task.Run(() => Console.WriteLine());
            //var form = new VoxFormViewModel();
            var form = new CreateVoxRequest();

            //var categories = await blogRepository.Categories.GetAll();

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
