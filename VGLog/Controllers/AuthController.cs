using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VGLog.Models.ViewModels;
using VGLog.Services.Interfaces;

namespace VGLog.Controllers
{
    /// Controller MVC che gestisce i redirect di autenticazione. Eredita da Controller invece che ControllerBase perché ha bisogno di gestire redirect e viste.
    /// questo controller è separato da AccountController perché gestisce il flusso di login con redirect HTTP, mentre AccountController gestisce le chiamate API pure.
    public class AuthController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ILogger _iLogger;
        public AuthController(IAccountService accountService, ILogger<AuthController> logger)
        {
            _accountService = accountService;
            _iLogger = logger;
        }
        /// [FromForm]: i dati arrivano dal form HTML
        [HttpPost("/auth/login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginRedirect([FromForm] LoginViewModel model, string? returnUrl = null)
        {
            // Controlla specificamente l'errore sull'email PRIMA così la pagina di login può mostrare il messaggio giusto.
            var emailError = ModelState["Email"]?.Errors.FirstOrDefault()?.ErrorMessage;

            if (emailError != null)
                return Redirect($"/account/login?error=invalidemail&returnUrl={returnUrl}");

            if (!ModelState.IsValid)
                return Redirect($"/account/login?returnUrl={returnUrl}");

            var result = await _accountService.LoginAsync(model);

            if (result.Succeeded)
                // LocalRedirect è più sicuro di Redirect, accetta SOLO URL relativi al dominio corrente, prevenendo attacchi di "open redirect"
                return LocalRedirect(returnUrl ?? "/");

            return Redirect($"/account/login?error=invalid&returnUrl={returnUrl}");
        }
    }
}
