using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Data.EF;
using Core.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Voxed.WebApp.Controllers
{
    [Authorize(Roles = nameof(RoleType.Administrator))]
    public class AdminController : Controller
    {
        private readonly IVoxedRepository voxedRepository;

        public AdminController(IVoxedRepository voxedRepository)
        {
            this.voxedRepository = voxedRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<Response> Delete([FromForm]Request request)
        {
            //comment
            if (request.ContentType == "0")
            {
                var comment = await voxedRepository.Comments.GetById(new Guid(request.ContentId));
                if (comment == null)
                {
                    NotFound();
                }

                comment.State = Core.Entities.CommentState.Deleted;
                await voxedRepository.CompleteAsync();
            }

            if (request.ContentType == "1")
            {
                var vox = await voxedRepository.Voxs.GetById(new Guid(request.ContentId));
                if (vox == null)
                {
                    NotFound();
                }

                vox.State = Core.Entities.VoxState.Deleted;
                await voxedRepository.CompleteAsync();
            }

            return new Response() { Value = "OK" };
        }

        public class Request
        {
            public string ContentType { get; set; } //puede ser 0:comment o 1
            public string ContentId { get; set; }
        }

        public class Response
        {
            public string Value { get; set; }
        }
    }
}
