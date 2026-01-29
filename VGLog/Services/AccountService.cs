using Microsoft.AspNetCore.Identity;
using VGLog.Models;
using VGLog.Services.Interfaces;
using System.Security.Claims;

namespace VGLog.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _SignInManager;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _SignInManager = signInManager;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                DisplayName = model.DisplayName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded && !string.IsNullOrWhiteSpace(user.DisplayName))
            {
                await _userManager.AddClaimAsync(
                    user,
                    new Claim("DisplayName", user.DisplayName)
                    );
            }

            return result;
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {

            return await _SignInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);
        }

        public async Task LogoutAsync()
        {
            await _SignInManager.SignOutAsync();
            
        }
    }
}
