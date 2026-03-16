using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using VGLog.Models.ViewModels;
using VGLog.Services;
using VGLog.Services.Interfaces;

namespace VGLog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger _logger;
        

        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {

                var result = await _accountService.RegisterAsync(model);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);

                    return BadRequest(result.Errors.Select( e => e.Description));
                }

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating an offer for User {UserId}.", User?.Identity?.Name ?? "Unknown");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                return StatusCode(500);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                var result = await _accountService.LoginAsync(model);

                if (result.Succeeded)
                    return Ok(new { success = true });

                return Unauthorized(new { success = false, message = "Invalid credentials" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed");
                return StatusCode(500, new { success = false, message = "Server error" });
            }
        }

        [Authorize]
        [HttpPost("/auth/logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _accountService.LogoutAsync();
                return Redirect("/");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating an offer for User {UserId}.", User?.Identity?.Name ?? "Unknown");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                return BadRequest();
            }
        }
    }
}
