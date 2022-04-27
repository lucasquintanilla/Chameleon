using Core.Data.Filters;
using Core.Data.Repositories;
using Core.Entities;
using Core.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Voxed.WebApp.Constants;
using Voxed.WebApp.Extensions;
using Voxed.WebApp.Mappers;
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

        public HomeController(
            ILogger<HomeController> logger,
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
                var filter = new VoxFilter() { Categories = _defaultCategories.ToList() };
                _lastestVoxs = await _voxedRepository.Voxs.GetByFilterAsync(filter);
            }

            return Ok(VoxedMapper.Map(_lastestVoxs));
        }

        [HttpGet("v1/vox/{hash}")]
        public async Task<IActionResult> Vox(string id)
        {
            if (id == null)
            {
                return BadRequest();
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

            //var hiddenVoxIds = await GetUserHiddenVoxIds();

            //var voxs = await _voxedRepository.Voxs.GetLastestAsync(GetUserCategorySubscriptions(), hiddenVoxIds);

            var filter = new VoxFilter()
            {
                UserId = User.GetLoggedInUserId<Guid?>(),
                Categories = GetUserCategorySubscriptions(),
                IncludeHidden = false,
                HiddenWords = GetHiddenWords()
            };

            var voxs = await _voxedRepository.Voxs.GetByFilterAsync(filter);
            return View(VoxedMapper.Map(voxs));
        }

        private List<int> GetUserCategorySubscriptions()
        {

            HttpContext.Request.Cookies.TryGetValue(CookieName.Subscriptions, out string subscriptionsCookie);

            if (subscriptionsCookie == null)
            {
                var result = JsonConvert.SerializeObject(_defaultCategories.Select(category => category.ToString()), new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                HttpContext.Response.Cookies.Append(CookieName.Subscriptions, result, new Microsoft.AspNetCore.Http.CookieOptions()
                {
                    Expires = DateTimeOffset.MaxValue
                });

                return _defaultCategories.ToList();
            }

            var subscriptions = JsonConvert.DeserializeObject<List<int>>(subscriptionsCookie);
            return subscriptions;

        }

        private List<string> GetHiddenWords()
        {
            if (HttpContext.Request.Cookies.TryGetValue(CookieName.HiddenWords, out var hiddenWordsCookie))
            {
                var words = hiddenWordsCookie.Split(',');
                return words.Select(word => word.Trim()).ToList();
            }

            return new List<string>();
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
