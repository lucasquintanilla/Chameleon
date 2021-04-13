using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Data.Repositories;
using Core.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
//using Voxed.WebApp.Data;
//using Voxed.WebApp.Models;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Voxed.WebApp.Controllers
{
    public class VoxController : Controller
    {
        //private readonly VoxedContext _context;
        private IWebHostEnvironment _env;
        private FileStoreService fileStoreService;
        private string _dir;
        private IVoxedRepository voxedRepository;
        private readonly UserManager<User> _userManager;
        private FormateadorService formateadorService;

        public VoxController(
            //VoxedContext context, 
            IWebHostEnvironment env,
            FileStoreService fileStoreService,
            IVoxedRepository voxedRepository,
            UserManager<User> userManager, 
            FormateadorService formateadorService)
        {
            //_context = context;
            _env = env;
            _dir = env.WebRootPath;
            this.fileStoreService = fileStoreService;
            this.voxedRepository = voxedRepository;
            _userManager = userManager;
            this.formateadorService = formateadorService;
        }

        // GET: Vox
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Voxes.ToListAsync());
        //}

        // GET: Vox/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vox = await voxedRepository.Voxs.GetById(id.Value);

            if (vox == null)
            {
                return NotFound();
            }

            if (vox.State == VoxState.Deleted)
            {
                return NotFound();
            }

            return View(vox);
        }

        // GET: Vox/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Vox/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ID,Title,Content,CategoryID")] Vox vox)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        vox.ID = Guid.NewGuid();
        //        vox.State = VoxState.Normal;
        //        vox.Media = new Media
        //        {
        //            Url = "http://web.archive.org/web/20200816001928im_/https://upload.voxed.net/thumb_8V4XHRrdXGrWaOtVhWnY.jpg",
        //            MediaType = MediaType.Image
        //        };
        //        vox.User = new User()
        //        {
        //            Username = "Anonimo"
        //        };


        //        _context.Add(vox);              


        //        await _context.SaveChangesAsync();
        //        //return RedirectToAction(nameof(Index));
        //        return RedirectToAction(nameof(Details), new { ID = vox.ID});
        //    }
        //    return View(vox);
        //}

        // POST: Vox/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,Content,CategoryID,File,Poll")] Models.VoxFormViewModel voxRequest)
        {
            var vox = new Vox();

            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (ModelState.IsValid)
            {                
                vox.ID = Guid.NewGuid();
                vox.State = VoxState.Normal;
                vox.User = user ?? new User(){ UserName = "Anonimo" };

                vox.Title = voxRequest.Title;
                vox.Content = formateadorService.Parsear(voxRequest.Content);
                //vox.Content = voxRequest.Content;
                vox.CategoryID = voxRequest.CategoryID;

                if (voxRequest.Poll != null)
                {
                    vox.Poll = new Poll()
                    {
                        OptionADescription = voxRequest.Poll.OptionADescription,
                        OptionBDescription = voxRequest.Poll.OptionBDescription,
                    };
                }

                if (voxRequest.File != null)
                {
                    var isValidFile = await fileStoreService.IsValidFile(voxRequest.File);

                    if (!isValidFile)
                    {
                        
                        ModelState.AddModelError("File", "El archivo no es válido.");
                        return RedirectToAction("Index", "Home");
                    }

                    vox.Media = await fileStoreService.Save(voxRequest.File, vox.Hash);
                }

                await voxedRepository.Voxs.Add(vox);
                await voxedRepository.CompleteAsync();

                return RedirectToAction(nameof(Details), new { ID = vox.ID });
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Vox/Edit/5
        //public async Task<IActionResult> Edit(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var vox = await _context.Voxes.FindAsync(id);
        //    if (vox == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(vox);
        //}

        // POST: Vox/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Guid id, [Bind("ID,Title,Content,CategoryID,UserID,State,Bump")] Vox vox)
        //{
        //    if (id != vox.ID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(vox);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!VoxExists(vox.ID))
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
        //    return View(vox);
        //}

        // GET: Vox/Delete/5
        //public async Task<IActionResult> Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var vox = await _context.Voxes
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (vox == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(vox);
        //}

        // POST: Vox/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    var vox = await _context.Voxes.FindAsync(id);
        //    _context.Voxes.Remove(vox);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool VoxExists(Guid id)
        //{
        //    return _context.Voxes.Any(e => e.ID == id);
        //}

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> List(ListRequest request)
        {
            Console.WriteLine(request);


            return Ok();
        }
    }

    public class ListRequest
    {
        public string Page { get; set; }
        public int LoadMore { get; set; }
        public string Suscriptions { get; set; }
        public string[] Ignore { get; set; }

    }
}
