using Core.Data.Repositories;
using Core.Entities;
using Core.Shared;
using Core.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Voxed.WebApp.Hubs;

namespace Voxed.WebApp.Controllers
{
    public class VoxController : BaseController
    {
        private readonly FileUploadService _fileUploadService;
        private readonly IVoxedRepository _voxedRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly FormateadorService _formateadorService;
        private readonly IHubContext<VoxedHub, INotificationHub> _notificationHub;
        private User _anonUser;

        public VoxController(
            FileUploadService fileUploadService,
            IVoxedRepository voxedRepository,
            UserManager<User> userManager,
            FormateadorService formateadorService,
            IHubContext<VoxedHub, INotificationHub> notificationHub, 
            SignInManager<User> signInManager,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _fileUploadService = fileUploadService;
            _voxedRepository = voxedRepository;
            _userManager = userManager;
            _formateadorService = formateadorService;
            _notificationHub = notificationHub;
            _signInManager = signInManager;
        }

        [HttpGet("vox/{hash}")]
        public async Task<IActionResult> Details(string hash)
        {
            if (hash == null)
            {
                return NotFound();
            }

            var voxId = GuidConverter.FromShortString(hash);

            var vox = await _voxedRepository.Voxs.GetById(voxId);

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

            var voxs = await _voxedRepository.Voxs.SearchAsync(value);

            var voxsList = ConvertToViewModel(voxs);

            return View(voxsList);
        }
       
        private User GetAnonymousUser()
        {
            if (_anonUser == null)
            {
                _anonUser = _userManager.Users.Where(x => x.UserType == UserType.Anonymous).FirstOrDefault();                
            }

            return _anonUser;
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

                    if (user == null)
                    {
                        user = await CreateAnonymousUser();

                        await _signInManager.SignInAsync(user, true);

                        //Crear una notificacion para el nuevo usuario anonimo
                    }

                    var vox = new Vox()
                    {
                        ID = Guid.NewGuid(),
                        State = VoxState.Normal,
                        User = user ?? GetAnonymousUser(),
                        Hash = new Hash().NewHash(),
                        Title = request.Title,
                        Content = _formateadorService.Parsear(request.Content),
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

                    await _fileUploadService.ProcessMedia(request.GetUploadData(), request.File, vox);

                    await _voxedRepository.Voxs.Add(vox);
                    await _voxedRepository.SaveChangesAsync();

                    //disparo notificacion del vox
                    vox = await _voxedRepository.Voxs.GetById(vox.ID);
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

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ListResponse> List([FromForm]ListRequest request)
        {
            var response = new ListResponse();

            var skipList = JsonConvert.DeserializeObject<IEnumerable<string>>(request?.Ignore);

            var guidList = skipList.Select(x => GuidConverter.FromShortString(x)).ToList();

            var lastVox = await _voxedRepository.Voxs.GetLastVoxBump(guidList);

            var voxs = await _voxedRepository.Voxs.GetLastestAsync(guidList, lastVox.Bump);

            var voxsList = ConvertToViewModel(voxs);

            //Devuelve 36 voxs

            if (voxsList.Count() > 0)
            {
                response.Status = true;
                response.List = new List()
                {
                    Page = "category-sld",
                    Voxs = voxsList
                };
                
            }
            else
            {
                response.Status = false;
                response.List = new List()
                {
                    Page = "category-sld",
                    Voxs = voxsList
                };
            }

            return response;
        }

        private async Task<User> CreateAnonymousUser()
        {
            var user = new User
            {
                UserName = UserNameGenerator.NewAnonymousUserName(),
                EmailConfirmed = true,
                UserType = UserType.AnonymousAccount,
                IpAddress = UserIpAddress.ToString(),
                UserAgent = UserAgent,
                Token = TokenGenerator.NewToken()
            };

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                return user;
            }

            throw new Exception();
        }

        private Models.VoxResponse ConvertoToVoxResponse(Vox vox)
        {
            return new Models.VoxResponse()
            {
                Hash = GuidConverter.ToShortString(vox.ID),
                //Hash = vox.Hash,
                Status = "1",
                Niche = "20",
                Title = vox.Title,
                Comments = vox.Comments.Count().ToString(),
                Extension = "",
                Sticky = vox.IsSticky ? "1" : "0",
                CreatedAt = vox.CreatedOn.ToString(),
                PollOne = "",
                PollTwo = "",
                Id = "20",
                Slug = vox.Category.ShortName.ToUpper(),
                VoxId = vox.ID.ToString(),
                New = vox.CreatedOn.Date == DateTime.Now.Date,
                ThumbnailUrl = vox.Media?.ThumbnailUrl
            };
        }

        public IEnumerable<Models.VoxResponse> ConvertToViewModel(IEnumerable<Vox> voxs)
        {
            return voxs.Select(vox => ConvertoToVoxResponse(vox));
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

        public UploadData GetUploadData()
        {
            return JsonConvert.DeserializeObject<UploadData>(UploadData);
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

    public class List
    {
        public IEnumerable<Models.VoxResponse> Voxs { get; set; } = new List<Models.VoxResponse>();
        public string Page { get; set; }
    }

    public class ListResponse
    {
        public bool Status { get; set; }
        public List List { get; set; }
    }
}
