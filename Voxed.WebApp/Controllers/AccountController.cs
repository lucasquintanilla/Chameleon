using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
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
                Swal = "Hubo un error"
            };
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<RegisterResponse> Register(RegisterRequest request)
        {
            if (ModelState.IsValid)
            {
                var user = new User 
                { 
                    UserName = request.UserName,  
                    EmailConfirmed = true, 
                    UserType = UserType.Anon 
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    ////_logger.LogInformation("User created a new account with password.");

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    //{
                    //    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    //}
                    //else
                    //{
                    //    await _signInManager.SignInAsync(user, isPersistent: false);
                    //    return LocalRedirect(returnUrl);
                    //}

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    //return LocalRedirect(returnUrl);

                    return new RegisterResponse()
                    {
                        Status = true,
                    };
                }
                foreach (var error in result.Errors)
                {
                    //ModelState.AddModelError(string.Empty, error.Description);
                    return new RegisterResponse()
                    {
                        Status = false,
                        Swal = error.Description
                    };
                }
            }

            return new RegisterResponse()
            {
                Status = false,
                Swal = "Ups"
            };
        }
    }

    public class RegisterRequest
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class RegisterResponse
    {
        public bool Status { get; set; }
        public string Token { get; set; }
        public string MaxAge { get; set; }
        public string Swal { get; set; }
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
