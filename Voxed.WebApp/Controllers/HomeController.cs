using Core.Data.Filters;
using Core.Data.Repositories;
using Core.Entities;
using Core.Services.Mixers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

namespace Voxed.WebApp.Controllers;

//[AllowAnonymous]
[Authorize]
public class HomeController : Controller
{
    private readonly IVoxedRepository _voxedRepository;
    private readonly IMixer _boardMixer;
    private readonly IMapper _mapper;
    private readonly IEnumerable<Category> _categories = Enumerable.Empty<Category>();

    public HomeController(
        IVoxedRepository voxedRepository,
        IMixer boardMixer,
        IMapper mapper)
    {
        _voxedRepository = voxedRepository;
        _boardMixer = boardMixer;
        _mapper = mapper;
        if (!_categories.Any())
        {
            _categories = voxedRepository.Categories.GetAll().GetAwaiter().GetResult();
        }
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
        var filter = new PostFilter()
        {
            UserId = User.GetUserId(),
            //Categories = (await GetUserCategorySubscriptions()).ToList(),
            IncludeHidden = false,
            HiddenWords = GetUserHiddenWords()
        };

        var posts = await _voxedRepository.Posts.GetByFilterAsync(filter);

        var board = new BoardViewModel()
        {
            Voxs = _mapper.Map(posts),
            Title = "Home",
            Page = "home",
            Categories = _categories
        };
        return View("board", board);
    }

    [HttpGet("hub")]
    public async Task<IActionResult> Hub()
    {
        var mix = await _boardMixer.GetMix();

        var board = new BoardViewModel()
        {
            Voxs = mix.Items.OrderByDescending(x => x.LastActivityOn).Select(_mapper.Map).ToArray(),
            Title = "Hub",
            Page = "category-hub",
            Categories = _categories
        };
        return View("board", board);
    }

    [HttpGet("favoritos")]
    public async Task<IActionResult> Favorites()
    {
        if (!User.Identity.IsAuthenticated) return BadRequest();

        var filter = new PostFilter()
        {
            UserId = User.GetLoggedInUserId<Guid?>(),
            IncludeFavorites = true
        };

        var voxs = await _voxedRepository.Posts.GetByFilterAsync(filter);

        var board = new BoardViewModel()
        {
            Voxs = _mapper.Map(voxs),
            Title = "Favoritos",
            Page = "favorites",
            Categories = _categories
        };

        return View("board", board);
    }

    [HttpGet("ocultos")]
    public async Task<IActionResult> Hidden()
    {
        if (!User.Identity.IsAuthenticated) return BadRequest();

        var filter = new PostFilter()
        {
            UserId = User.GetLoggedInUserId<Guid?>(),
            IncludeHidden = true
        };

        var voxs = await _voxedRepository.Posts.GetByFilterAsync(filter);

        var board = new BoardViewModel()
        {
            Voxs = _mapper.Map(voxs),
            Title = "Ocultos",
            Page = "hidden",
            Categories = _categories
        };

        return View("board", board);
    }

    [Route("/{shortName}")]
    public async Task<IActionResult> Category(string shortName)
    {
        if (shortName == null) return BadRequest();

        var category = await _voxedRepository.Categories.GetByShortName(shortName);

        if (category == null) return NotFound();

        var filter = new PostFilter() { Categories = new List<int>() { category.Id } };

        var voxs = await _voxedRepository.Posts.GetByFilterAsync(filter);
        var board = new BoardViewModel()
        {
            Voxs = _mapper.Map(voxs),
            Title = category.Name,
            Page = category.ShortName,
            Categories = _categories
        };

        return View("board", board);
    }

    [HttpGet("search/{searchText}")]
    public async Task<IActionResult> Search(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText)) return BadRequest();

        var filter = new PostFilter() { SearchText = searchText };

        var voxs = await _voxedRepository.Posts.GetByFilterAsync(filter);

        var board = new BoardViewModel()
        {
            Voxs = _mapper.Map(voxs),
            Title = "Resultado",
            Page = "search",
            Categories = _categories
        };

        return View("board", board);
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

    private async Task<IEnumerable<int>> GetUserCategorySubscriptions()
    {
        HttpContext.Request.Cookies.TryGetValue(CookieName.Subscriptions, out string subscriptionsCookie);

        if (subscriptionsCookie is not null)
            return JsonConvert.DeserializeObject<IEnumerable<int>>(subscriptionsCookie);

        var userCategories = Categories.DefaultCategories;
        var subscriptionsCookieValue = JsonConvert.SerializeObject(userCategories.Select(categoryId => categoryId.ToString()), new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
        HttpContext.Response.Cookies.Append(CookieName.Subscriptions, subscriptionsCookieValue, new Microsoft.AspNetCore.Http.CookieOptions()
        {
            Expires = DateTimeOffset.MaxValue
        });

        return await Task.FromResult(userCategories);
    }

    private IEnumerable<string> GetUserHiddenWords()
    {
        if (HttpContext.Request.Cookies.TryGetValue(CookieName.HiddenWords, out var hiddenWordsCookie))
        {
            if (hiddenWordsCookie.Trim().Length > 0)
            {
                var words = hiddenWordsCookie.Trim().Split(',');
                return words.Select(word => word.Trim());
            }
        }

        return Enumerable.Empty<string>();
    }
}
