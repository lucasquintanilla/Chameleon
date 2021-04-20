using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Data.Repositories;
using Core.Shared;
using GroupDocs.Metadata;
using ImageProcessor;
using ImageProcessor.Plugins.WebP.Imaging.Formats;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Voxed.WebApp.Hubs;
//using Voxed.WebApp.Data;
using Voxed.WebApp.Models;
using Xabe.FFmpeg;

namespace Voxed.WebApp.Controllers
{
    public class CategoryController : Controller
    {
        //private readonly VoxedContext _context;
        private IVoxedRepository voxedRepository;
        private IWebHostEnvironment _env;
        private readonly IHubContext<VoxedHub> notifyHub;

        private readonly UserManager<Core.Entities.User> _userManager;

        public CategoryController(
            //VoxedContext context, 
            IVoxedRepository voxedRepository, 
            IWebHostEnvironment env, 
            IHubContext<VoxedHub> notifyHub, 
            UserManager<Core.Entities.User> userManager)
        {
            //_context = context;
            this.voxedRepository = voxedRepository;
            _env = env;
            this.notifyHub = notifyHub;
            _userManager = userManager;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            FFmpeg.SetExecutablesPath(@"C:\FFmpeg");
            //var voxs = await voxedRepository.Voxs.GetAll();

            //foreach (var vox in voxs.Where(x => x.Media.Url.EndsWith("jpg") || x.Media.Url.EndsWith("png")))
            //{

            //    try
            //    {
            //        var array = vox.Media.Url.Split("/");
            //        var filename = array[array.Length - 1];
            //        var rootPath = _env.WebRootPath;
            //        var folderName = "media";
            //        var filePath = Path.Combine(rootPath, "media", filename);


            //        // Open the stream and read it back.
            //        using (FileStream fs = System.IO.File.Open(filePath, FileMode.Open))
            //        {
            //            filename = DateTime.Now.ToString("yyyyMMdd") + "-" + vox.Hash + ".webp";
            //            filePath = Path.Combine(rootPath, "media", filename);

            //            // Then save in WebP format
            //            using (var webPFileStream = new FileStream(filePath, FileMode.Create))
            //            {
            //                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: false))
            //                {
            //                    imageFactory.Load(fs)
            //                                .Format(new WebPFormat())
            //                                .Quality(70)
            //                                .Save(webPFileStream);

            //                    vox.Media.Url = $"/{folderName}/" + filename;
            //                    vox.Media.ThumbnailUrl = $"/{folderName}/" + filename;
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception)
            //    {


            //    }

            //}


            //var voxs = await voxedRepository.Comments.GetAll();

            //foreach (var vox in voxs.Where(x => x.Media != null && (x.Media.Url.EndsWith("jpg") || x.Media.Url.EndsWith("png"))))
            //{

            //    try
            //    {
            //        var array = vox.Media.Url.Split("/");
            //        var filename = array[array.Length - 1];
            //        var rootPath = _env.WebRootPath;
            //        var folderName = "media";
            //        var filePath = Path.Combine(rootPath, "media", filename);


            //        // Open the stream and read it back.
            //        using (FileStream fs = System.IO.File.Open(filePath, FileMode.Open))
            //        {
            //            filename = DateTime.Now.ToString("yyyyMMdd") + "-" + vox.Hash + ".webp";
            //            filePath = Path.Combine(rootPath, "media", filename);

            //            // Then save in WebP format
            //            using (var webPFileStream = new FileStream(filePath, FileMode.Create))
            //            {
            //                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: false))
            //                {
            //                    imageFactory.Load(fs)
            //                                .Format(new WebPFormat())
            //                                .Quality(70)
            //                                .Save(webPFileStream);

            //                    vox.Media.Url = $"/{folderName}/" + filename;
            //                    vox.Media.ThumbnailUrl = $"/{folderName}/" + filename;
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception)
            //    {


            //    }

            //}


            //voxedRepository.Complete();
            //




            ///ACA ARRANCA OTRA COSA

            //var rootPath = _env.WebRootPath;
            //var folderName = "media";

            //var files = Directory.GetFiles(Path.Combine(rootPath, folderName));

            //foreach (var filePath in files)
            //{
            //    // Create a temporary file, and put some data into it.
            //    string tempPath = Path.GetTempFileName();

            //    using (Metadata metadata = new Metadata(filePath))
            //    {
            //        var affected = metadata.Sanitize();                 

            //        metadata.Save(tempPath);
            //    }

            //    System.IO.File.Delete(filePath);

            //    // Use the Path.Combine method to safely append the file name to the path.
            //    // Will overwrite if the destination file already exists.
            //    System.IO.File.Copy(tempPath, filePath, true);

            //}

            //await MassiveConvertGifToWebP();

            //await ConvertGifToThumbnail();

            //await AddStylesToComments();

            //var notification = new Notification();
            //notification.Message = "Buenass";

            //await notifyHub.Clients.All.SendAsync("ReceiveNotification", notification);

            //await UnificarAnons();

            return RedirectToAction("Index", "Home");
        }

        //// GET: Category/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var category = await _context.Categories
        //        .Include(c => c.Media)
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(category);
        //}

        // GET: Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exists = await voxedRepository.Categories.Exists(id.Value);

            if (!exists)
            {
                return NotFound();
            }

            var voxs = await voxedRepository.Voxs.GetByCategoryIdAsync(id.Value);

            return View(voxs);
        }

        // GET: Category/Create
        //public IActionResult Create()
        //{
        //    ViewData["MediaID"] = new SelectList(_context.Media, "ID", "ID");
        //    return View();
        //}

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ID,Name,ShortName,MediaID")] Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(category);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["MediaID"] = new SelectList(_context.Media, "ID", "ID", category.MediaID);
        //    return View(category);
        //}

        // GET: Category/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var category = await _context.Categories.FindAsync(id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["MediaID"] = new SelectList(_context.Media, "ID", "ID", category.MediaID);
        //    return View(category);
        //}

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("ID,Name,ShortName,MediaID")] Category category)
        //{
        //    if (id != category.ID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(category);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CategoryExists(category.ID))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["MediaID"] = new SelectList(_context.Media, "ID", "ID", category.MediaID);
        //    return View(category);
        //}

        // GET: Category/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var category = await _context.Categories
        //        .Include(c => c.Media)
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(category);
        //}

        // POST: Category/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var category = await _context.Categories.FindAsync(id);
        //    _context.Categories.Remove(category);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool CategoryExists(int id)
        //{
        //    return _context.Categories.Any(e => e.ID == id);
        //}

        private async Task AddStylesToComments()
        {
            var comments = await voxedRepository.Comments.GetAll();

            foreach (var comment in comments)
            {
                try
                {
                    comment.Style = StyleService.GetRandomCommentStyle();                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            await voxedRepository.CompleteAsync();
        }

        private async Task ConvertGifToThumbnail()
        {
            var voxs = await voxedRepository.Voxs.GetAll();

            foreach (var vox in voxs.Where(x => x.Media != null && x.Media.Url.ToLower().EndsWith("gif")))
            {

                try
                {
                    var array = vox.Media.Url.Split("/");
                    var filename = array[array.Length - 1];
                    var rootPath = _env.WebRootPath;
                    var folderName = "media";
                    var filePath = Path.Combine(rootPath, folderName, filename);
                    var outputFilename = $"{DateTime.Now:yyyyMMdd}-{vox.Hash}.webp";
                    var outputFilePath = Path.Combine(rootPath, folderName, outputFilename);

                    //ConvertoAndSaveToWebp(filePath, outputFilePath);

                    await SaveGifThumbnail(filePath, outputFilePath);

                    //vox.Media.Url = $"/{folderName}/" + outputFilename;
                    vox.Media.ThumbnailUrl = $"/{folderName}/{outputFilename}";
                    await voxedRepository.CompleteAsync();                   
                }
                catch (Exception e)                
                {


                }
            }
        }

        private async Task UnificarAnons()
        {
            var comments = await voxedRepository.Comments.GetAll();

            var anonUser = new Core.Entities.User() { UserName = "Anonimo", UserType = Core.Entities.UserType.Anon };

            foreach (var comment in comments.Where(x => x.User.UserType == Core.Entities.UserType.Anon))
            {
                try
                {
                    comment.User = anonUser;

                    //await voxedRepository.CompleteAsync();
                }
                catch (Exception e)
                {


                }
            }

            var voxs = await voxedRepository.Voxs.GetAll();           

            foreach (var vox in voxs.Where(x => x.User.UserType == Core.Entities.UserType.Anon))
            {
                try
                {
                    vox.User = anonUser;

                   
                }
                catch (Exception e)
                {


                }
            }

             await voxedRepository.CompleteAsync();

            var usersToDelete = _userManager.Users.Where(x => x.Id != anonUser.Id && x.UserType == Core.Entities.UserType.Anon);

            foreach (var user in usersToDelete)
            {
                await _userManager.DeleteAsync(user);
            }
            
        }

        private async Task MassiveConvertGifToWebP()
        {
            var voxs = await voxedRepository.Voxs.GetAll();

            foreach (var vox in voxs.Where(x => x.Media != null && x.Media.Url.ToLower().EndsWith("gif")))
            {

                try
                {
                    var array = vox.Media.Url.Split("/");
                    var filename = array[array.Length - 1];
                    var rootPath = _env.WebRootPath;
                    var folderName = "media";
                    var filePath = Path.Combine(rootPath, "media", filename);
                    var outputFilename = $"{DateTime.Now:yyyyMMdd}-{vox.Hash}.webp";
                    var outputFilePath = Path.Combine(rootPath, folderName, outputFilename);

                    ConvertoAndSaveToWebp(filePath, outputFilePath);


                    vox.Media.Url = $"/{folderName}/" + outputFilename;
                    vox.Media.ThumbnailUrl = $"/{folderName}/" + outputFilename;
                    await voxedRepository.CompleteAsync();

                    // Open the stream and read it back.
                    //using (FileStream fs = System.IO.File.Open(filePath, FileMode.Open))
                    //{
                    //    filename = DateTime.Now.ToString("yyyyMMdd") + "-" + vox.Hash + ".webp";
                    //    filePath = Path.Combine(rootPath, "media", filename);

                    //    // Then save in WebP format
                    //    using (var webPFileStream = new FileStream(filePath, FileMode.Create))
                    //    {
                    //        using (ImageFactory imageFactory = new ImageFactory(preserveExifData: false))
                    //        {
                    //            imageFactory.Load(fs)
                    //                        .Format(new WebPFormat())
                    //                        .Quality(70)
                    //                        .Save(webPFileStream);

                    //            vox.Media.Url = $"/{folderName}/" + filename;
                    //            vox.Media.ThumbnailUrl = $"/{folderName}/" + filename;
                    //        }
                    //    }
                    //}
                }
                catch (Exception e)
                {
                    

                }
            }

            
        }

        public bool ConvertoAndSaveToWebp(string inputFilePath, string outputFilePath)
        {
            const string ffmpegPath = @"C:\ffmpeg\bin\ffmpeg.exe";
            //var tempPath = _CreateTempAndClear();
            //var inputFilePath = _SaveFile(gifFile, tempPath);

            //var command = $"-i {inputFilePath} -vcodec libwebp -lossless 0 -qscale 75 -preset default -loop 0 -vf scale=320:-1,fps=15 -an -vsync 0 {outputFilePath}";
            var command = string.Format($"-i {inputFilePath} -b:v 0 -crf 25 -loop 0 {outputFilePath}");
            using (var process = Process.Start(ffmpegPath, command))
            {
                process.WaitForExit();
                if (process.ExitCode == 0)
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> SaveGifThumbnail(string inputFilePath, string outputFilePath)
        {
            
            var result = await FFmpeg.Conversions.FromSnippet.Snapshot(inputFilePath,
                                               outputFilePath,
                                               TimeSpan.FromSeconds(0))
                                                .Result.Start();

            return true;            
        }
    }
}
