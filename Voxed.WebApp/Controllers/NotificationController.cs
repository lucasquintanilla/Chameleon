using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Voxed.WebApp.Controllers
{
    public class NotificationController : BaseController
    {
        private IVoxedRepository voxedRepository;

        public NotificationController(IVoxedRepository voxedRepository)
        {
            this.voxedRepository = voxedRepository;
        }

        ///notification/${e.id}#${e.contentHash}
        [AllowAnonymous]
        [Route("notification/{parameters}#{commentHash}")]
        public IActionResult Index(string parameters, string commentHash)
        {
            var array = parameters.Split("#");

            string voxId = array[0];
            //string commentHash = array[1];
            //guardar como notificacion ya leida

            // ir al vox 

            ///vox/${e.id}#${e.contentHash}
            ///
            //return RedirectToAction("details", "Vox", new { Hash = voxId });
            return Redirect($"vox/{voxId}#{commentHash}");
            //return LocalRedirect($"vox/{voxId}#{commentHash}");
        }
    }
}