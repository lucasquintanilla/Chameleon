﻿using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voxed.WebApp.Models;

namespace Voxed.WebApp.Controllers
{
    public class AccountController : BaseController
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(SignInManager<User> signInManager,
            UserManager<User> userManager,
            IHttpContextAccessor accessor) : base(accessor)
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

            return new LoginResponse()
            {
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
                    UserType = UserType.Account,
                    IpAddress = UserIpAddress,
                    UserAgent = UserAgent
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
