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
using Core.Entities;
using Core.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Voxed.WebApp.Hubs;
using Newtonsoft.Json;

namespace Voxed.WebApp.Controllers
{
    public class CommentController : Controller
    {
        //private readonly VoxedContext _context;
        private FormateadorService formateadorService;        
        private FileStoreService fileStoreService;
        private readonly IVoxedRepository voxedRepository;
        private readonly UserManager<User> _userManager;
        private User anonUser;
        public IHubContext<VoxedHub, INotificationHub> _notificationHub { get; }       

        public CommentController(
            //VoxedContext context, 
            FormateadorService formateadorService,
            FileStoreService fileStoreService,
            IVoxedRepository voxedRepository,
            UserManager<User> userManager,
            IHubContext<VoxedHub, INotificationHub> notificationHub)
        {
            //_context = context;
            this.formateadorService = formateadorService;
            this.fileStoreService = fileStoreService;
            this.voxedRepository = voxedRepository;
            _userManager = userManager;
            _notificationHub = notificationHub;
        }

        //// GET: Comment
        //public async Task<IActionResult> Index()
        //{
        //    var voxedContext = _context.Comments.Include(c => c.Media).Include(c => c.User);
        //    return View(await voxedContext.ToListAsync());
        //}

        
        [HttpPost]
        public async Task<Models.CommentResponse> Nuevo([FromForm] Models.CommentRequest request, [FromRoute] string id)
        {
            if (!ModelState.IsValid)
                return new Models.CommentResponse()
                {
                    Hash = id,
                    Status = false,
                    Error = "",
                    Swal = "Formato de comentario invalido",
                };


            

            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                var comment = new Comment()
                {
                    ID = Guid.NewGuid(),
                    Hash = new Hash().NewHash(7),
                    VoxID = new Guid(id),
                    User = user ?? GetAnonUser(),
                    Content = request.Content == null ? null : formateadorService.Parsear(request.Content),
                    Style = StyleService.GetRandomCommentStyle()
                };

                await ProcessMedia(request, comment);

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

                var commentNotification = new CommentNotification()
                {
                    UniqueId = null, //si es unique id puede tener colores unicos
                    UniqueColor = null,
                    UniqueColorContrast = null,

                    Id = comment.ID.ToString(),
                    Hash = comment.Hash,
                    VoxHash = vox.Hash,
                    AvatarColor = comment.Style.ToString().ToLower(),
                    IsOp = vox.UserID == comment.UserID && vox.User.UserType != UserType.Anon, //probar cambiarlo cuando solo pruedan craer los usuarios.
                    Tag = comment.User.UserType.ToString().ToLower(), //admin o dev               
                    Content = comment.Content ?? "",
                    Name = comment.User.UserName,
                    CreatedAt = TimeAgo.ConvertToTimeAgo(comment.CreatedOn.DateTime),
                    Poll = null, //aca va una opcion respondida

                    //Media
                    MediaUrl = comment.Media?.Url,
                    MediaThumbnailUrl = comment.Media?.ThumbnailUrl,
                    Extension = request.GetUploadData()?.Extension,
                    ExtensionData = request.GetUploadData()?.ExtensionData,
                    Via = request.GetUploadData()?.Extension == Models.UploadDataExtension.Youtube ? comment.Media?.Url : null,
                };

                await _notificationHub.Clients.All.Comment(commentNotification);

                if (comment.UserID != vox.User.Id)
                {
                    var notification = new Notification()
                    {
                        Type = "new",
                        Content = new Content()
                        {
                            VoxHash = vox.Hash,
                            NotificationBold = "Nuevo Comentario",
                            NotificationText = vox.Title,
                            Count = "14",
                            ContentHash = comment.Hash,
                            Id = GuidConverter.ToShortString(vox.ID),
                            ThumbnailUrl = vox.Media?.ThumbnailUrl
                        }
                    };

                    await _notificationHub.Clients.User(vox.User.Id.ToString()).Notification(notification);
                }

                var response = new Models.CommentResponse()
                {
                    Hash = comment.Hash,
                    Status = true,
                };

                return response;
            }
            catch (NotImplementedException e)
            {
                return new Models.CommentResponse()
                {
                    Hash = "",
                    Status = false,
                    Error = "",
                    Swal = e.Message,
                };
            }
            catch (Exception e)
            {
                return new Models.CommentResponse()
                {
                    Hash = "",
                    Status = false,
                    Error = "",
                    Swal = "Hubo un error",
                };
            }
        }

        private User GetAnonUser()
        {
            if (anonUser == null)
            {
                anonUser = _userManager.Users.Where(x => x.UserType == UserType.Anon).FirstOrDefault();
            }

            return anonUser;
        }

        private async Task ProcessMedia(Models.CommentRequest request, Comment comment)
        {
            var data = request.GetUploadData();

            if (data != null && request.File == null)
            {                
                if (data.Extension == Models.UploadDataExtension.Youtube)
                {
                    comment.Media = await fileStoreService.SaveExternal(data, comment.Hash);
                }
                else if (data.Extension == Models.UploadDataExtension.Base64)
                {
                    throw new NotImplementedException("Opcion no implementada");
                }
            }
            else if (request.File != null)
            {
                var isValidFile = await fileStoreService.IsValidFile(request.File);

                if (!isValidFile)
                {
                    throw new NotImplementedException("Archivo invalido");
                }

                comment.Media = await fileStoreService.Save(request.File, comment.Hash);
            }
        }

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
                    Style = StyleService.GetRandomCommentStyle()
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


                //var notification = new CommentNotification();
                //notification.Hash = vox.Hash;
                //await _notificationHub.Clients.All.Comment(notification);

                //await _notificationHub.Clients.User(vox.UserID.ToString()).ReceiveNotification(notification);

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
