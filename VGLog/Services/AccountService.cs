using Microsoft.AspNetCore.Identity;
using VGLog.Models;
using VGLog.Services.Interfaces;
using System.Security.Claims;
using VGLog.Models.ViewModels;

namespace VGLog.Services
{
    /// Service che gestisce le operazioni di autenticazione e registrazione.
    /// Implementa IAccountService seguendo il pattern "Interface + Implementation":
    /// il controller dipende dall'interfaccia, non dalla classe concreta.
    /// Questo rende il codice più testabile (è possibile fare il mock dell'interfaccia)
    /// e più flessibile (si può cambiare implementazione senza toccare il controller).
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            //creazione dell'utente dal viewmodel
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                DisplayName = model.DisplayName
            };

            // CreateAsync salva l'utente nel DB con la password hashata.
            var result = await _userManager.CreateAsync(user, model.Password);

            //inserimento del claim DisplayName nei cookie/token, in modo da poter leggerlo da li invece di fare chiamate al db
            if (result.Succeeded && !string.IsNullOrWhiteSpace(user.DisplayName))
            {
                var claimResult = await _userManager.AddClaimAsync(
                    user,
                    new Claim("DisplayName", user.DisplayName)
                    );

                if (!claimResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    return claimResult;
                }
            }

            return result;
        }

        /// Effettua il login di un utente. ritorna SignInResult che indica se il login è riuscito o ha avuto problemi
        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return SignInResult.Failed;
            }

            //verifica la password e crea il cookie di autenticazione in caso il login abbia successo, il terzo parametro serve a dire al cookie di non eliminarsi alla chiusura del browser,
            //il quarto non blocca l'account in caso di troppi accessi falliti
            return await _signInManager.PasswordSignInAsync(
                user.UserName,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);
        }

        //SignOutAsync invalida il cookie di autenticazione in modo tale da rendere non più autenticate le successive richieste
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            
        }
    }
}
