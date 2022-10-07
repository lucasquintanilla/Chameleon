using Core.Entities;
using Core.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Voxed.WebApp.Controllers
{
    //[ServiceFilter(typeof(TraceIPAttribute))]
    public class BaseController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _accessor;
        private static string[] bannedIpList = {
            "198.41.231.163",
            "198.41.231.229"
        };

        public BaseController(IHttpContextAccessor accessor, UserManager<User> userManager)
        {
            _accessor = accessor;
            _userManager = userManager;
        }

        protected string UserAgent => Request.Headers.ContainsKey("User-Agent") ? Request.Headers["User-Agent"].ToString() : String.Empty;

        protected string UserIpAddress => HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        //protected string UserIpAddress => _accessor.GetIpAddress();

        protected void RetringBannedIps()
        {

        }


        internal async Task<User> CreateAnonymousUser()
        {
            var user = new User
            {
                UserName = UserNameGenerator.NewAnonymousUserName(),
                EmailConfirmed = true,
                UserType = UserType.AnonymousAccount,
                IpAddress = UserIpAddress,
                UserAgent = UserAgent,
                Token = TokenGenerator.NewToken()
            };

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                return user;
            }

            throw new Exception("Error al crear usuario anonimo");
        }
    }

    //public class BaseMvcController : Controller
    //{
    //    private UserManager<ApplicationUser> UserManager { get; set; }

    //    public BaseMvcController(UserManager<ApplicationUser> userManager)
    //    {
    //        UserManager = userManager;
    //    }

    //    public TPrimaryKey GetCurrentUserId()
    //    {
    //        if (User.Identity.IsAuthenticated)
    //        {
    //            return TPrimaryKey.Parse(UserManager.GetUserId(User));
    //        }
    //        else
    //        {
    //            throw new HttpResponseException(HttpStatusCode.Unauthorized, "User is not authenticated");
    //        }
    //    }

    //    protected string UserAgent => Request.Headers.ContainsKey("User-Agent") ? Request.Headers["User-Agent"].ToString() : "";

    //    protected IPAddress UserIpAddress => Request.HttpContext.Connection.RemoteIpAddress;
    //}
}
