using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using VGLog.Models;
using VGLog.Models.ViewModels;

namespace VGLog.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty]
        public LoginViewModel Input { get; set; } = new();

        public string? ErrorMessage { get; set; }
        public void OnGet() { }

        public class LoginViewModel
        {
            [Required(ErrorMessage = "The E-mail is required.")]
            [EmailAddress(ErrorMessage = "Email format not valid.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "The Password is required.")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= "/";

            if (!ModelState.IsValid)
                return Page();

            var result = await _signInManager.PasswordSignInAsync(
                Input.Email,
                Input.Password,
                Input.RememberMe,
                lockoutOnFailure: true
            );

            if (result.Succeeded)
                return LocalRedirect(returnUrl);

            if (result.IsLockedOut)
                ErrorMessage = "Account bloccato. Riprova tra 15 minuti.";
            else
                ErrorMessage = "Email o password non corretti.";

            return Page();
        }
    }
}
    