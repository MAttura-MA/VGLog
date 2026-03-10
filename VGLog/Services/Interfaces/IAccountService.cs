using Microsoft.AspNetCore.Identity;
using VGLog.Models.ViewModels;

namespace VGLog.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
    }
}
