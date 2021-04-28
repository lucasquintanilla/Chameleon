using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
//using Voxed.WebApp.Data;
//using Voxed.WebApp.Models;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Voxed.WebApp.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IVoxedRepository voxedRepository;

        public HomeController(ILogger<HomeController> logger, 
            IVoxedRepository voxedRepository)
        {
            _logger = logger;
            this.voxedRepository = voxedRepository;
        }

        public async Task<IActionResult> Index()
        {
            var voxs = await voxedRepository.Voxs.GetLastestAsync();

            return View(voxs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Info()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
