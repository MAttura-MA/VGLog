using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using VGLog.Models;
using VGLog.Services;
using VGLog.Services.Interfaces;

namespace VGLog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ILogger _logger;
        

        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            //if (ModelState.IsValid)
            //    return View(model);

            try
            {

                Console.WriteLine($"Email: {model.Email}, Password is null? {model.Password == null}");

                var result = await _accountService.RegisterAsync(model);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);

                    return View(model);
                }

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating an offer for User {UserId}.", User?.Identity?.Name ?? "Unknown");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                return View(model);
            }
        }
        

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            //if (!ModelState.IsValid)
            //    return View(model);

            var result = await _accountService.LoginAsync(model);

            return Ok(result.Succeeded);
        }


        [HttpPost("account/logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("index", "home");
        }
    }
}
