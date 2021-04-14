using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
//using Voxed.WebApp.Data;
//using Voxed.WebApp.Models;
using Core.Entities;
using Core.Data.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Voxed.WebApp.Controllers
{
    public class CommentController : Controller
    {
        //private readonly VoxedContext _context;
        private FormateadorService formateadorService;        
        private FileStoreService fileStoreService;
        private readonly IVoxedRepository voxedRepository;
        private readonly UserManager<User> _userManager;

        public CommentController(
            //VoxedContext context, 
            FormateadorService formateadorService,
            FileStoreService fileStoreService,
            IVoxedRepository voxedRepository, 
            UserManager<User> userManager)
        {
            //_context = context;
            this.formateadorService = formateadorService;
            this.fileStoreService = fileStoreService;
            this.voxedRepository = voxedRepository;
            _userManager = userManager;
        }

        //// GET: Comment
        //public async Task<IActionResult> Index()
        //{
        //    var voxedContext = _context.Comments.Include(c => c.Media).Include(c => c.User);
        //    return View(await voxedContext.ToListAsync());
        //}

        //// GET: Comment/Details/5
        //public async Task<IActionResult> Details(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var comment = await _context.Comments
        //        .Include(c => c.Media)
        //        .Include(c => c.User)
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (comment == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(comment);
        //}

        // GET: Comment/Create
        //public IActionResult Create()
        //{
        //    ViewData["MediaID"] = new SelectList(_context.Media, "ID", "ID");
        //    ViewData["UserID"] = new SelectList(_context.Set<User>(), "ID", "ID");
        //    return View();
        //}

        //// POST: Comment/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ID,Hash,VoxID,UserID,MediaID,Content,CreatedOn,Bump,State")] Comment comment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var contentFormarted = formateadorService.Parsear(comment.Content);
        //        comment.Content = contentFormarted;
        //        comment.ID = Guid.NewGuid();
        //        _context.Add(comment);

        //        var vox = _context.Voxes.Where(x => x.ID == comment.VoxID).FirstOrDefault();
        //        vox.Bump = DateTimeOffset.Now;

        //        await _context.SaveChangesAsync();
        //        return RedirectToAction("Details", "Vox", new { ID = comment.VoxID});
        //    }

        //    ViewData["MediaID"] = new SelectList(_context.Media, "ID", "ID", comment.MediaID);
        //    ViewData["UserID"] = new SelectList(_context.Set<User>(), "ID", "ID", comment.UserID);
        //    return View(comment);
        //}

        // POST: Comment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Hash,VoxID,UserID,MediaID,Content,CreatedOn,Bump,State,File")] Models.CommentFormViewModel commentForm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                var comment = new Comment() {
                    ID = Guid.NewGuid(),
                    Hash = new Hash().NewHash(7),
                    VoxID = commentForm.VoxID,
                    User = user ?? new User() { UserName = "Anonimo" },
                    Content = formateadorService.Parsear(commentForm.Content),
                    Style = ColorService.GetRandomCommentStyle()
                };
                          

                if (commentForm.File != null)
                {
                    var isValidFile = await fileStoreService.IsValidFile(commentForm.File);

                    if (isValidFile)
                    {
                        comment.Media = await fileStoreService.Save(commentForm.File, comment.Hash);
                    }

                    ModelState.AddModelError("File", "El archivo no es válido.");
                }

                //Detectar Tag >HIDE
                //if (!comentario.Contenido.ToLower().Contains("gt;hide"))
                //{
                //    await db.Query("Hilos")
                //        .Where("Id", comentario.HiloId)
                //        .UpdateAsync(new { Bump = DateTimeOffset.Now });
                //}

                await voxedRepository.Comments.Add(comment);
                var vox = await voxedRepository.Voxs.GetById(comment.VoxID);
                vox.Bump = DateTimeOffset.Now;
                await voxedRepository.CompleteAsync();

                return RedirectToAction("Details", "Vox", new { ID = comment.VoxID });
            }

            //ViewData["MediaID"] = new SelectList(_context.Media, "ID", "ID", comment.MediaID);
            //ViewData["UserID"] = new SelectList(_context.Set<User>(), "ID", "ID", comment.UserID);
            //return View();
            return RedirectToAction("Details", "Vox", new { ID = commentForm.VoxID });

        }

        // GET: Comment/Edit/5
        //public async Task<IActionResult> Edit(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var comment = await voxedRepository.Comments.GetById(id.Value);
        //    if (comment == null)
        //    {
        //        return NotFound();
        //    }
        //    //ViewData["MediaID"] = new SelectList(_context.Media, "ID", "ID", comment.MediaID);
        //    //ViewData["UserID"] = new SelectList(_context.Set<User>(), "ID", "ID", comment.UserID);
        //    return View(comment);
        //}

        // POST: Comment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Guid id, [Bind("ID,Hash,VoxID,UserID,MediaID,Content,CreatedOn,Bump,State")] Comment comment)
        //{
        //    if (id != comment.ID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(comment);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CommentExists(comment.ID))
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
        //    ViewData["MediaID"] = new SelectList(_context.Media, "ID", "ID", comment.MediaID);
        //    ViewData["UserID"] = new SelectList(_context.Set<User>(), "ID", "ID", comment.UserID);
        //    return View(comment);
        //}

        // GET: Comment/Delete/5
        //public async Task<IActionResult> Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var comment = await _context.Comments
        //        .Include(c => c.Media)
        //        .Include(c => c.User)
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (comment == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(comment);
        //}

        // POST: Comment/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    var comment = await _context.Comments.FindAsync(id);
        //    _context.Comments.Remove(comment);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool CommentExists(Guid id)
        //{
        //    return _context.Comments.Any(e => e.ID == id);
        //}
    }
}
