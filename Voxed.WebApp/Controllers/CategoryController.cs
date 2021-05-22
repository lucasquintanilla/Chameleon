using Core.Data.EF.MySql;
using Core.Data.EF.Sqlite;
using Core.Data.Repositories;
using Core.Entities;
using Core.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IVoxedRepository _voxedRepository;
        private readonly SqliteVoxedContext _sqliteVoxedContext;
        private readonly MySqlVoxedContext _mySqlVoxedContext;

        public CategoryController(
            IVoxedRepository voxedRepository, 
            SqliteVoxedContext sqliteVoxedContext, 
            MySqlVoxedContext mySqlVoxedContext)
        {
            _voxedRepository = voxedRepository;
            _sqliteVoxedContext = sqliteVoxedContext;
            _mySqlVoxedContext = mySqlVoxedContext;
        }

        [Route("/{shortName}")]
        public async Task<IActionResult> Details(string shortName)
        {
            if (shortName == null)
            {
                return NotFound();
            }

            var exists = await _voxedRepository.Categories.Exists(shortName);

            if (!exists)
            {
                return NotFound();
            }

            var voxs = await _voxedRepository.Voxs.GetByCategoryShortNameAsync(shortName);

            var voxsList = voxs.Select(x => new Models.VoxResponse()
            {
                Hash = GuidConverter.ToShortString(x.ID),
                Status = "1",
                Niche = "20",
                Title = x.Title,
                Comments = x.Comments.Count().ToString(),
                Extension = "",
                Sticky = x.IsSticky ? "1" : "0",
                CreatedAt = x.CreatedOn.ToString(),
                PollOne = "",
                PollTwo = "",
                Id = "20",
                Slug = x.Category.ShortName.ToUpper(),
                VoxId = GuidConverter.ToShortString(x.ID),
                New = x.CreatedOn.Date == DateTime.Now.Date,
                ThumbnailUrl = x.Media?.ThumbnailUrl,
                Category = x.Category.Name
            }).ToList();

            return View(voxsList);
        }

        [Route("test")]
        public async Task<IActionResult> Test()
        {
            var media = await _sqliteVoxedContext.Media.ToListAsync();
            await _mySqlVoxedContext.Media.AddRangeAsync(media);

            var categories = await _sqliteVoxedContext.Categories.ToListAsync();
            await _mySqlVoxedContext.Categories.AddRangeAsync(categories);

            //var voxs = await _sqliteVoxedContext.Voxs.ToListAsync();
            //await _mySqlVoxedContext.Voxs.AddRangeAsync(voxs);

            //var comments = await _sqliteVoxedContext.Comments.ToListAsync();
            //await _mySqlVoxedContext.Comments.AddRangeAsync(comments);

            //var notifications = await _sqliteVoxedContext.Notifications.ToListAsync();
            //await _mySqlVoxedContext.Notifications.AddRangeAsync(notifications);

            await _mySqlVoxedContext.SaveChangesAsync();

            return Ok();
        }
    }
}
