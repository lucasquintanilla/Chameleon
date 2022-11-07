using Core.Data.Filters;
using Core.Data.Repositories;
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

        public HomeController(
            ILogger<HomeController> logger,
            IVoxedRepository voxedRepository)
        {
            _logger = logger;
            _voxedRepository = voxedRepository;
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
                Categories = await GetUserCategorySubscriptions(),
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

            var filter = new VoxFilter() { Categories = new List<int>() { category.Id } };

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

        private async Task<IEnumerable<int>> GetUserCategorySubscriptions()
        {

            HttpContext.Request.Cookies.TryGetValue(CookieName.Subscriptions, out string subscriptionsCookie);

            if (subscriptionsCookie is not null)
            {
                return JsonConvert.DeserializeObject<List<int>>(subscriptionsCookie);
            }

            var userCategories = Categories.DefaultCategories;
            //var userCategories = (await _voxedRepository.Categories.Find(x => !x.Nsfw)).Select(x=>x.Id);
            var subscriptionsCookieValue = JsonConvert.SerializeObject(userCategories.Select(categoryId => categoryId.ToString()), new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
            HttpContext.Response.Cookies.Append(CookieName.Subscriptions, subscriptionsCookieValue, new Microsoft.AspNetCore.Http.CookieOptions()
            {
                Expires = DateTimeOffset.MaxValue
            });

            return await Task.FromResult(userCategories);
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
