using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Controllers
{
    public class AccountController : Controller
    {

        [HttpPost]
        [AllowAnonymous]
        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var response = new LoginResponse() { Status = false };
            return response;
        }
    }

    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public bool Status { get; set; }
        public string Token { get; set; }
        public string MaxAge { get; set; }
        public SweetAlert SweetAlert { get; set; }
    }

    public class SweetAlert
    {

    }
}
