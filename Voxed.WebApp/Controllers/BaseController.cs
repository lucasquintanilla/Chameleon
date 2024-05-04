using AngleSharp.Common;
using Core.Entities;
using Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voxed.WebApp.Extensions;

namespace Voxed.WebApp.Controllers;

public class BaseController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _accessor;

    public BaseController(IHttpContextAccessor accessor, UserManager<User> userManager)
    {
        _accessor = accessor;
        _userManager = userManager;
    }

    protected string UserAgent => Request.Headers.ContainsKey("User-Agent") ? Request.Headers["User-Agent"].ToString() : string.Empty;
    protected string UserIpAddress => HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
    protected Dictionary<string, string> Headers => Request.Headers.ToDictionary();

    internal async Task<User> CreateAnonymousUser()
    {
        var token = Guid.NewGuid().ToShortString();
        var user = new User
        {
            UserName = Guid.NewGuid().ToShortString(),
            EmailConfirmed = true,
            UserType = UserType.AnonymousAccount,
            IpAddress = UserIpAddress,
            UserAgent = UserAgent,
            Token = token
        };

        var result = await _userManager.CreateAsync(user, token);
        if (result.Succeeded) return user;

        throw new Exception("Error al crear usuario anonimo");
    }

    //public bool IsMobileBrowser()
    //{
    //    //GETS THE CURRENT USER CONTEXT
    //    var context = HttpContext.Request.IsBrowser().Headers;
    //    //FIRST TRY BUILT IN ASP.NT CHECK
    //    if (context.Request.Browser.IsMobileDevice)
    //    {
    //        return true;
    //    }
    //    //THEN TRY CHECKING FOR THE HTTP_X_WAP_PROFILE HEADER
    //    if (context.Request.ServerVariables["HTTP_X_WAP_PROFILE"] != null)
    //    {
    //        return true;
    //    }
    //    //THEN TRY CHECKING THAT HTTP_ACCEPT EXISTS AND CONTAINS WAP
    //    if (context.Request.ServerVariables["HTTP_ACCEPT"] != null &&
    //        context.Request.ServerVariables["HTTP_ACCEPT"].ToLower().Contains("wap"))
    //    {
    //        return true;
    //    }
    //    //AND FINALLY CHECK THE HTTP_USER_AGENT 
    //    //HEADER VARIABLE FOR ANY ONE OF THE FOLLOWING
    //    if (context.Request.ServerVariables["HTTP_USER_AGENT"] != null)
    //    {
    //        //Create a list of all mobile types
    //        string[] mobiles = new[]
    //        {
    //          "midp", "j2me", "avant", "docomo",
    //          "novarra", "palmos", "palmsource",
    //          "240x320", "opwv", "chtml",
    //          "pda", "windows ce", "mmp/",
    //          "blackberry", "mib/", "symbian",
    //          "wireless", "nokia", "hand", "mobi",
    //          "phone", "cdm", "up.b", "audio",
    //          "SIE-", "SEC-", "samsung", "HTC",
    //          "mot-", "mitsu", "sagem", "sony"
    //          , "alcatel", "lg", "eric", "vx",
    //         "NEC", "philips", "mmm", "xx",
    //         "panasonic", "sharp", "wap", "sch",
    //         "rover", "pocket", "benq", "java",
    //         "pt", "pg", "vox", "amoi",
    //         "bird", "compal", "kg", "voda",
    //         "sany", "kdd", "dbt", "sendo",
    //         "sgh", "gradi", "jb", "dddi",
    //         "moto", "iphone"
    //    };

    //        //Loop through each item in the list created above 
    //        //and check if the header contains that text
    //        foreach (string s in mobiles)
    //        {
    //            if (context.Request.ServerVariables["HTTP_USER_AGENT"].ToLower().Contains(s.ToLower()))
    //            {
    //                return true;
    //            }
    //        }
    //    }

    //    return false;
    //}
}
