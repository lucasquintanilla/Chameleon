using Core.Data.Repositories;
using Core.Entities;
using Core.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Voxed.WebApp.Extensions;
using Voxed.WebApp.Models;

namespace Voxed.WebApp.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IVoxedRepository _voxedRepository;
        private static IEnumerable<Vox> _lastestVoxs = new List<Vox>();
        private readonly int[] _defaultCategories = { 1, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 30, 16, 14, 13, 12, 11, 10, 9, 8, 15, 7, 31, 6, 5, 4 };


        public HomeController(ILogger<HomeController> logger,
            IVoxedRepository voxedRepository)
        {
            _logger = logger;
            _voxedRepository = voxedRepository;
        }

        [HttpGet("v1/voxs")]
        public async Task<IActionResult> Voxs()
        {
            if (!_lastestVoxs.Any())
            {
                _lastestVoxs = await _voxedRepository.Voxs.GetLastestAsync(_defaultCategories);
            }


            var voxsList = _lastestVoxs.Select(vox => new VoxResponse()
            {
                Hash = GuidConverter.ToShortString(vox.ID),
                //Hash = x.Hash,
                //Status = "1",
                Status = true,
                Niche = "20",
                Title = vox.Title,
                Comments = vox.Comments.Count().ToString(),
                Extension = "",
                Sticky = vox.IsSticky ? "1" : "0",
                CreatedAt = vox.CreatedOn.ToString(),
                PollOne = "",
                PollTwo = "",
                Id = "20",
                Slug = vox.Category.ShortName.ToUpper(),
                VoxId = GuidConverter.ToShortString(vox.ID),
                New = vox.CreatedOn.IsNew(),
                ThumbnailUrl = vox.Media?.ThumbnailUrl
            }).ToList();

            return Ok(voxsList);
        }

        [HttpGet("v1/vox/{hash}")]
        public async Task<IActionResult> Vox(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voxId = GuidConverter.FromShortString(id);

            var vox = await _voxedRepository.Voxs.GetById(voxId);

            if (vox == null)
            {
                return NotFound();
            }

            if (vox.State == VoxState.Deleted)
            {
                return NotFound();
            }

            return Ok(vox);
        }

        public async Task<IActionResult> Index()
        {
            //if (HttpContext.Request.Cookies.TryGetValue("config", out string configCookie))
            //{
            //}
            //else
            //{
            //    HttpContext.Response.Cookies.Append("config", "{\"darkmode\":false}", new Microsoft.AspNetCore.Http.CookieOptions()
            //    {
            //        Expires = DateTimeOffset.MaxValue
            //    });

            //var x = Request.Headers.TryGetValue("CF-IPCountry", out var resulto);            
            
            var voxs = await _voxedRepository.Voxs.GetLastestAsync(GetCategorySubscriptions());

            var voxsList = voxs.Select(vox => new VoxResponse()
            {
                Hash = GuidConverter.ToShortString(vox.ID),
                //Status = "1",
                Status = true,
                Niche = "20",
                Title = vox.Title,
                Comments = vox.Comments.Count().ToString(),
                Extension = "",
                Sticky = vox.IsSticky ? "1" : "0",
                CreatedAt = vox.CreatedOn.ToString(),
                PollOne = "",
                PollTwo = "",
                Id = "20",
                Slug = vox.Category.ShortName.ToUpper(),
                VoxId = GuidConverter.ToShortString(vox.ID),
                New = vox.CreatedOn.IsNew(),
                ThumbnailUrl = vox.Media?.ThumbnailUrl
            }).ToList();

            return View(voxsList);
        }

        private ICollection<int> GetCategorySubscriptions()
        {
            var cookieName = "suscriptions";
            HttpContext.Request.Cookies.TryGetValue(cookieName, out string subscriptionsCookie);

            if (subscriptionsCookie == null)
            {
                var result = JsonConvert.SerializeObject(_defaultCategories.Select(category => category.ToString()), new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                HttpContext.Response.Cookies.Append(cookieName, result, new Microsoft.AspNetCore.Http.CookieOptions()
                {
                    Expires = DateTimeOffset.MaxValue
                });

                return _defaultCategories;
            }

            var subscriptions = JsonConvert.DeserializeObject<List<int>>(subscriptionsCookie);
            return subscriptions;

        }

        private IEnumerable<string> GetHiddenWords()
        {
            if (HttpContext.Request.Cookies.TryGetValue("hiddenWords", out var hiddenWordsCookie))
            {
                var words = hiddenWordsCookie.Split(',');
                return words.Select(word => word.Trim());
            }

            return new string[0];
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
