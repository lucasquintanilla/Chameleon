using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Voxed.WebApp.Extensions;
using Voxed.WebApp.Services;

namespace Voxed.WebApp.Controllers
{
    //[ServiceFilter(typeof(TraceIPAttribute))]
    public class BaseController : Controller
    {
        private readonly IHttpContextAccessor _accessor;
        private static string[] bannedIpList = {
            "198.41.231.163",
            "198.41.231.229"
        };

        public BaseController(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        protected string UserAgent => Request.Headers.ContainsKey("User-Agent") ? Request.Headers["User-Agent"].ToString() : String.Empty;

        protected string UserIpAddress => HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        //protected string UserIpAddress => _accessor.GetIpAddress();

        protected void RetringBannedIps()
        {

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
