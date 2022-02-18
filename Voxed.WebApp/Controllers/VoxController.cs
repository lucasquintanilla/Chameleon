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
        private readonly FormateadorService _formatterService;
        private readonly IHubContext<VoxedHub, INotificationHub> _notificationHub;
        private User _anonUser;

        public VoxController(
            FileUploadService fileUploadService,
            IVoxedRepository voxedRepository,
            UserManager<User> userManager,
            FormateadorService formatterService,
            IHubContext<VoxedHub, INotificationHub> notificationHub,
            SignInManager<User> signInManager,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _fileUploadService = fileUploadService;
            _voxedRepository = voxedRepository;
            _userManager = userManager;
            _formatterService = formatterService;
            _notificationHub = notificationHub;
            _signInManager = signInManager;
        }

        [HttpGet("vox/{hash}")]
        public async Task<IActionResult> Details(string hash)
        {
            if (hash == null) return NotFound();

            var voxId = GuidConverter.FromShortString(hash);

            var vox = await _voxedRepository.Voxs.GetById(voxId);

            if (vox == null) return NotFound();

            if (vox.State == VoxState.Deleted) return NotFound();

            return View(vox);
        }

        [HttpGet("search/{value}")]
        public async Task<IActionResult> Index(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return BadRequest();

            var voxs = await _voxedRepository.Voxs.SearchAsync(value);

            var voxsList = ConvertToViewModel(voxs);

            return View(voxsList);
        }

        private User GetAnonymousUser()
        {
            return _anonUser ??= _userManager.Users.FirstOrDefault(x => x.UserType == UserType.Anonymous);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Route("anon/vox")]
        public async Task<CreateVoxResponse> Create(CreateVoxRequest request)
        {
            var response = new CreateVoxResponse();

            if (!ModelState.IsValid)
            {
                response.Status = false;
                response.Swal = "error";
                return response;
            }

            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    user = await CreateAnonymousUser();
                    await _signInManager.SignInAsync(user, true);

                    //TODO: Crear una notificacion para el nuevo usuario anonimo
                }

                var vox = new Vox()
                {
                    ID = Guid.NewGuid(),
                    State = VoxState.Normal,
                    User = user ?? GetAnonymousUser(),
                    Hash = new Hash().NewHash(),
                    Title = request.Title,
                    Content = _formatterService.Parse(request.Content),
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
                vox = await _voxedRepository.Voxs.GetById(vox.ID); // Ver si se puede remover
                var voxToHub = ConvertoToVoxResponse(vox);
                await _notificationHub.Clients.All.Vox(voxToHub);

                response.VoxHash = GuidConverter.ToShortString(vox.ID);
                response.Status = true;

                return response;
            }
            catch (NotImplementedException e)
            {
                response.Status = false;
                response.Swal = e.Message;
                return response;
            }
            catch (Exception)
            {
                response.Status = false;
                response.Swal = "Error";
                return response;
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ListResponse> List([FromForm] ListRequest request)
        {
            var response = new ListResponse();

            var skipList = JsonConvert.DeserializeObject<IEnumerable<string>>(request?.Ignore);

            var skipIdList = skipList.Select(x => GuidConverter.FromShortString(x)).ToList();

            var lastVox = await _voxedRepository.Voxs.GetLastVoxBump(skipIdList);

            var voxs = await _voxedRepository.Voxs.GetLastestAsync(skipIdList, lastVox.Bump);

            var voxsList = ConvertToViewModel(voxs);

            response.Status = voxsList.Any();
            response.List = new List()
            {
                Page = "category-sld",
                Voxs = voxsList
            };

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

            throw new Exception("Usuario no pudo ser creado");
        }

        private Models.VoxResponse ConvertoToVoxResponse(Vox vox)
        {
            return new Models.VoxResponse()
            {
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

                Hash = GuidConverter.ToShortString(vox.ID),
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
            return voxs.Select(ConvertoToVoxResponse);
        }
    }

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


        [JsonPropertyName("g-recaptcha-response")]
        public string GReCaptcha { get; set; }

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
