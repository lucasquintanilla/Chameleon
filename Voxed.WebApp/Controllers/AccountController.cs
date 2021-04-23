using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;

        public AccountController(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<LoginResponse> Login(LoginRequest request)
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(request.UserName, request.Password, true, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    //_logger.LogInformation("User logged in.");
                    //return LocalRedirect(returnUrl);
                    return new LoginResponse() { Status = true };
                }
                //if (result.RequiresTwoFactor)
                //{
                //    //return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = true });
                //}
                //if (result.IsLockedOut)
                //{
                //    //_logger.LogWarning("User account locked out.");
                //    //return RedirectToPage("./Lockout");
                //}
                //else
                //{
                //    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                //    //return Page();
                //}
            }

            return new LoginResponse() { 
                Status = false,
                Swal = "Usuario o contraseña incorrecta"
            };
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
        public string Swal { get; set; }
    }

    public class SweetAlert
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public bool ShowCancelButton { get; set; }
        public string CancelButtonText { get; set; }
        public string ConfirmButtonText { get; set; }


        //SweetAlert = new SweetAlert()
        //{
        //    Title = "Buenasssss",
        //    Text = "como le baillaaa",
        //    ShowCancelButton = true,
        //    CancelButtonText = "CAncelar",
        //    ConfirmButtonText = "de unaaa",
        //    Type = "Warning"
        //}

    }
}
