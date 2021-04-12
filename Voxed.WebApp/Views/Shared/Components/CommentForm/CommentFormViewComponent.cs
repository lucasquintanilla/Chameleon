using Core.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Voxed.WebApp.Data;
using Voxed.WebApp.Models;

namespace Voxed.WebApp.Views.Shared.Components.CommentForm
{
    public class CommentFormViewComponent : ViewComponent
    {
        //private readonly VoxedContext _context;
        private readonly IVoxedRepository voxedRepository;

        public CommentFormViewComponent(IVoxedRepository voxedRepository)
        {
            this.voxedRepository = voxedRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid voxID, Guid userID)
        {
            var commentForm = new CommentFormViewModel() 
            { 
                VoxID = voxID, 
                UserID = userID 
            };
                
            return View(commentForm);
        }
    }
}
