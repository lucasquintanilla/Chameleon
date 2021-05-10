using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Voxed.WebApp.Controllers
{
    public class BaseController : Controller
    {
        //private UserManager<ApplicationUser> UserManager { get; set; }

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

        protected string UserAgent => Request.Headers.ContainsKey("User-Agent") ? Request.Headers["User-Agent"].ToString() : "";

        protected IPAddress UserIpAddress => HttpContext.Connection.RemoteIpAddress;

        //public IActionResult Index()
        //{
        //    return View();
        //}
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
