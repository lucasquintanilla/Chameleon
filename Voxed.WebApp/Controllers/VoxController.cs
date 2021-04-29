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
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR;
using Voxed.WebApp.Hubs;

namespace Voxed.WebApp.Controllers
{
    public class VoxController : BaseController
    {
        //private readonly VoxedContext _context;
        private IWebHostEnvironment _env;
        private FileStoreService fileStoreService;
        private string _dir;
        private IVoxedRepository voxedRepository;
        private readonly UserManager<User> _userManager;
        private FormateadorService formateadorService;
        private User anonUser;

        private IHubContext<VoxedHub, INotificationHub> _notificationHub;

        public VoxController(
            //VoxedContext context, 
            IWebHostEnvironment env,
            FileStoreService fileStoreService,
            IVoxedRepository voxedRepository,
            UserManager<User> userManager,
            FormateadorService formateadorService, 
            IHubContext<VoxedHub, INotificationHub> notificationHub)
        {
            //_context = context;
            _env = env;
            _dir = env.WebRootPath;
            this.fileStoreService = fileStoreService;
            this.voxedRepository = voxedRepository;
            _userManager = userManager;
            this.formateadorService = formateadorService;
            _notificationHub = notificationHub;
        }

        // GET: Vox
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Voxes.ToListAsync());
        //}

        // GET: Vox/Details/5

        //public async Task<IActionResult> Details(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var vox = await voxedRepository.Voxs.GetById(id.Value);

        //    if (vox == null)
        //    {
        //        return NotFound();
        //    }

        //    if (vox.State == VoxState.Deleted)
        //    {
        //        return NotFound();
        //    }

        //    return View(vox);
        //}

        //[HttpGet("vox/{hash}")]
        //public async Task<IActionResult> Details(string hash)
        //{
        //    if (hash == null)
        //    {
        //        return NotFound();
        //    }

        //    var vox = await voxedRepository.Voxs.GetByHash(hash);

        //    if (vox == null)
        //    {
        //        return NotFound();
        //    }

        //    if (vox.State == VoxState.Deleted)
        //    {
        //        return NotFound();
        //    }

        //    return View(vox);
        //}

        [HttpGet("vox/{hash}")]
        public async Task<IActionResult> Details(string hash)
        {
            if (hash == null)
            {
                return NotFound();
            }

            var voxId = GuidConverter.FromShortString(hash);

            var vox = await voxedRepository.Voxs.GetById(voxId);

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

        [HttpGet("search/{value}")]
        public async Task<IActionResult> Index(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                BadRequest();
            }

            //var words = search.Split(" ");

            var voxs = await voxedRepository.Voxs.SearchAsync(value);

            return View(voxs);
        }

        //public async Task<IActionResult> Index(string search)
        //{
        //    if (string.IsNullOrWhiteSpace(search))
        //    {
        //        BadRequest();
        //    }

        //    //var words = search.Split(" ");

        //    var voxs = await voxedRepository.Voxs.SearchAsync(search);

        //    return View(voxs);
        //}

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
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                var vox = new Vox() 
                {
                    ID = Guid.NewGuid(),
                    State = VoxState.Normal,
                    User = user ?? GetAnonUser(),
                    Hash = new Hash().NewHash(),
                    Title = voxRequest.Title,
                    Content = formateadorService.Parsear(voxRequest.Content),
                    CategoryID = voxRequest.CategoryID,
                };  

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

                return RedirectToAction(nameof(Details), new { Hash = GuidConverter.ToShortString(vox.ID) });
            }

            return RedirectToAction("Index", "Home");
        }

        private User GetAnonUser()
        {
            if (anonUser == null)
            {
                anonUser = _userManager.Users.Where(x => x.UserType == UserType.Anon).FirstOrDefault();                
            }

            return anonUser;
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Route("anon/vox")]
        public async Task<CreateVoxResponse> Create(CreateVoxRequest request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(HttpContext.User);

                    var vox = new Vox()
                    {
                        ID = Guid.NewGuid(),
                        State = VoxState.Normal,
                        User = user ?? GetAnonUser(),
                        Hash = new Hash().NewHash(),
                        Title = request.Title,
                        Content = formateadorService.Parsear(request.Content),
                        CategoryID = request.Niche,
                        IpAddress = UserIpAddress.ToString(),
                        UserAgent = UserAgent
                    };

                    if (request.PollOne != null && request.PollTwo != null)
                    {
                        vox.Poll = new Poll()
                        {
                            OptionADescription = request.PollOne,
                            OptionBDescription = request.PollTwo,
                        };
                    }

                    //if (request.File != null)
                    //{
                    //    var isValidFile = await fileStoreService.IsValidFile(request.File);

                    //    if (!isValidFile)
                    //    {
                    //        ModelState.AddModelError("File", "El archivo no es válido.");
                    //    }

                    //    vox.Media = await fileStoreService.Save(request.File, vox.Hash);
                    //}

                    await ProcessMedia(request, vox);

                    await voxedRepository.Voxs.Add(vox);
                    await voxedRepository.CompleteAsync();

                    //disparo notificacion del vox
                    vox = await voxedRepository.Voxs.GetById(vox.ID);
                    var voxToHub = ConvertoToVoxResponse(vox);

                    await _notificationHub.Clients.All.Vox(voxToHub);

                    return new CreateVoxResponse()
                    {
                        VoxHash = GuidConverter.ToShortString(vox.ID),
                        Status = true,
                        Error = "",
                        Swal = "",
                    };
                }
                catch (NotImplementedException e)
                {
                    return new CreateVoxResponse()
                    {
                        VoxHash = "",
                        Status = false,
                        Error = "",
                        Swal = e.Message,
                    };
                }
                catch (Exception e)
                {
                    return new CreateVoxResponse()
                    {
                        VoxHash = "",
                        Status = false,
                        Error = "",
                        Swal = "Error",
                    };
                }
                
            }

            return new CreateVoxResponse()
            {
                VoxHash = "",
                Status = false,
                Error = "",
                Swal = "Error",
            };
        }

        private async Task ProcessMedia(CreateVoxRequest request, Vox vox)
        {
            var data = request.GetUploadData();

            if (data != null && request.File == null)
            {
                if (data.Extension == Models.UploadDataExtension.Youtube)
                {
                    vox.Media = await fileStoreService.SaveExternal(data, vox.Hash);
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

                vox.Media = await fileStoreService.Save(request.File, vox.Hash);
            }
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
        public async Task<ListResponse> List([FromForm]ListRequest request)
        {
            var skipList = JsonConvert.DeserializeObject<List<string>>(request?.Ignore);

            var lastVox = await voxedRepository.Voxs.GetLastVoxBump(skipList);

            var voxs = await voxedRepository.Voxs.GetLastestAsync(skipList, lastVox.Bump);

            var voxsList = voxs.Select(x => new VoxResponse() {
                Hash = GuidConverter.ToShortString(x.ID),
                Status = "1",
                Niche = "20",
                Title = x.Title,                
                Comments = x.Comments.Count().ToString(),
                Extension = "",
                Sticky = x.Type == VoxType.Sticky ? "1" : "0",
                CreatedAt = x.CreatedOn.ToString(),
                PollOne = "",
                PollTwo = "",
                Id = "20",
                Slug = x.Category.ShortName.ToUpper(),
                VoxId = x.ID.ToString(),
                New = false,
                ThumbnailUrl = x.Media?.ThumbnailUrl
            });

            //Devuelve 36 voxs

            if (voxsList.Count() > 0)
            {
                return new ListResponse()
                {
                    Status = true,
                    List = new List()
                    {
                        //Page = "",
                        Page = "category-sld",
                        Voxs = voxsList
                    },
                };
            }
           
            return new ListResponse()
            {
                Status = false,
                List = new List()
                {
                    Page = "category-sld",
                    Voxs = voxsList
                },
            };
        }

        private VoxResponse ConvertoToVoxResponse(Vox vox)
        {
            return new VoxResponse()
            {
                Hash = GuidConverter.ToShortString(vox.ID),
                Status = "1",
                Niche = "20",
                Title = vox.Title,
                Comments = vox.Comments.Count().ToString(),
                Extension = "",
                Sticky = vox.Type == VoxType.Sticky ? "1" : "0",
                CreatedAt = vox.CreatedOn.ToString(),
                PollOne = "",
                PollTwo = "",
                Id = "20",
                Slug = vox.Category.ShortName.ToUpper(),
                VoxId = vox.ID.ToString(),
                New = false,
                ThumbnailUrl = vox.Media?.ThumbnailUrl
            };
        }
    }

        //{
        //        "hash": "LVsFqy15CYaRdNXsv5jR",
        //        "status": "1",
        //        "niche": "20",
        //        "title": "Es verdad que las concha de tanto cojer se oscurecen? ",
        //        "comments": "101",
        //        "extension": "jpg",
        //        "sticky": "0",
        //        "createdAt": "2020-10-30 10:20:34",
        //        "pollOne": " Es por tanto cojer, mir\u00e1 te cuento ",
        //        "pollTwo": "Es por esto",
        //        "id": "20",
        //        "slug": "sld",
        //        "voxId": "405371",
        //        "new": false
        //},

    public class CreateVoxRequest
    {
        [Required(ErrorMessage = "Debe ingresar un titulo")]
        [StringLength(120, ErrorMessage = "El titulo no puede superar los {1} caracteres.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Debe ingresar un contenido")]
        [StringLength(5000, ErrorMessage = "El contenido no puede superar los {1} caracteres.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoria.")]
        public int Niche { get; set; }
        public IFormFile File { get; set; }
        public string PollOne { get; set; }
        public string PollTwo { get; set; }
        public string UploadData { get; set; }

        //g-recaptcha-response
        [JsonPropertyName("g-recaptcha-response")]
        public string GReCaptcha { get; set; }
        //h-captcha-response
        [JsonPropertyName("h-captcha-response")]
        public string HCaptcha { get; set; }

        public Models.UploadData GetUploadData()
        {
            return JsonConvert.DeserializeObject<Models.UploadData>(UploadData);
        }

    }

    public class CreateVoxResponse
    {
        public bool Status { get; set; }
        public string VoxHash { get; set; }
        public string Swal { get; set; }
        public string Error { get; set; }
    }

    public class ListRequest
    {
        public string Page { get; set; } //category-anm o home
        public int LoadMore { get; set; }
        public string Suscriptions { get; set; }
        public string Ignore { get; set; }

    }
    public class VoxResponse
    {
        public string Hash { get; set; }
        public string Status { get; set; }
        public string Niche { get; set; }
        public string Title { get; set; }
        public string Comments { get; set; }
        public string Extension { get; set; }
        public string Sticky { get; set; }
        public string CreatedAt { get; set; }
        public string PollOne { get; set; }
        public string PollTwo { get; set; }
        public string Id { get; set; }
        public string Slug { get; set; }
        public string VoxId { get; set; }
        public bool New { get; set; }


        //Agregado extras
        public string ThumbnailUrl { get; set; }
    }

    public class List
    {
        public IEnumerable<VoxResponse> Voxs { get; set; } = new List<VoxResponse>();
        public string Page { get; set; }
    }

    public class ListResponse
    {
        public bool Status { get; set; }
        public List List { get; set; }
    }


}
