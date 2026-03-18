using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VGLog.Models.ViewModels;
using VGLog.Services.Interfaces;

namespace VGLog.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAccountService _accountService;

        public AuthController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("/auth/login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginRedirect([FromForm] LoginViewModel model, string? returnUrl = null)
        {
            var emailError = ModelState["Email"]?.Errors.FirstOrDefault()?.ErrorMessage;

            if (emailError != null)
                return Redirect($"/account/login?error=invalidemail&returnUrl={returnUrl}");

            if (!ModelState.IsValid)
                return Redirect("/account/login");

            var result = await _accountService.LoginAsync(model);

            if (result.Succeeded)
                return LocalRedirect(returnUrl ?? "/");

            return Redirect($"/account/login?error=invalid");
        }
    }
}
