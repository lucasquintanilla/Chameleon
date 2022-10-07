using Core.Data.Filters;
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
using Voxed.WebApp.Constants;
using Voxed.WebApp.Extensions;
using Voxed.WebApp.Mappers;
using Voxed.WebApp.Models;
using Voxed.WebApp.ViewModels;

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
            if (id == null) return BadRequest();

            var voxId = GuidConverter.FromShortString(id);

            var vox = await _voxedRepository.Voxs.GetById(voxId);

            if (vox == null) return NotFound();

            if (vox.State == VoxState.Deleted) return NotFound();

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

            var filter = new VoxFilter()
            {
                UserId = User.GetLoggedInUserId<Guid?>(),
                Categories = GetUserCategorySubscriptions(),
                IncludeHidden = false,
                HiddenWords = GetUserHiddenWords()
            };

            var voxs = await _voxedRepository.Voxs.GetByFilterAsync(filter);

            var board = new BoardViewModel()
            {
                Voxs = VoxedMapper.Map(voxs),
                Title = "Home",
                Page = "home"
            };

            return View("board", board);
        }

        [HttpGet]
        [Route("favoritos")]
        public async Task<IActionResult> Favorites()
        {
            if (!User.Identity.IsAuthenticated) return BadRequest();

            var filter = new VoxFilter()
            {
                UserId = User.GetLoggedInUserId<Guid?>(),
                IncludeFavorites = true
            };

            var voxs = await _voxedRepository.Voxs.GetByFilterAsync(filter);

            var board = new BoardViewModel()
            {
                Voxs = VoxedMapper.Map(voxs),
                Title = "Favoritos",
                Page = "favorites"
            };

            return View("board", board);
        }

        [HttpGet]
        [Route("ocultos")]
        public async Task<IActionResult> Hidden()
        {
            if (!User.Identity.IsAuthenticated) return BadRequest();

            var filter = new VoxFilter()
            {
                UserId = User.GetLoggedInUserId<Guid?>(),
                IncludeHidden = true
            };

            var voxs = await _voxedRepository.Voxs.GetByFilterAsync(filter);

            var board = new BoardViewModel()
            {
                Voxs = VoxedMapper.Map(voxs),
                Title = "Ocultos",
                Page = "hidden"
            };

            return View("board", board);
        }

        [Route("/{shortName}")]
        public async Task<IActionResult> Category(string shortName)
        {
            if (shortName == null) return BadRequest();

            var category = await _voxedRepository.Categories.GetByShortName(shortName);

            if (category == null) return NotFound();

            var filter = new VoxFilter() { Categories = new List<int>() { category.ID } };

            var voxs = await _voxedRepository.Voxs.GetByFilterAsync(filter);
            var board = new BoardViewModel()
            {
                Voxs = VoxedMapper.Map(voxs),
                Title = category.Name,
                Page = "category-" + category.ShortName
            };

            return View("board", board);
        }

        [HttpGet("search/{value}")]
        public async Task<IActionResult> Search(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return BadRequest();

            var filter = new VoxFilter() { Search = value };

            var voxs = await _voxedRepository.Voxs.GetByFilterAsync(filter);

            var board = new BoardViewModel()
            {
                Voxs = VoxedMapper.Map(voxs),
                Title = "Resultado",
                Page = "search"
            };

            return View("board", board);
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

        private List<string> GetUserHiddenWords()
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
