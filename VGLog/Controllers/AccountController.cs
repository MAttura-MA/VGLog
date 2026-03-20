using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VGLog.Models.ViewModels;
using VGLog.Services.Interfaces;

namespace VGLog.Controllers
{
    // Controller che gestisce le operazioni di autenticazione, [ApiController] abilita comportamenti automatici come validazione, binding automatico, risposte di errore standardizzate.
    //Route definisce il prefisso URL, [controller] viene sostituito con il nome dell classe senza "Controller)
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
                    // Aggiunge gli errori di Identity al ModelState
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);

                    //ritrna 400 con solo la descrizione dei messaggi di errore
                    return BadRequest(result.Errors.Select( e => e.Description));
                }

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la registrazione");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                return StatusCode(500);
            }
        }

        [Authorize]
        [HttpPost("logout")]
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
                _logger.LogError(ex, "An error occurred during Logout for User {UserId}.", User?.Identity?.Name ?? "Unknown");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                return BadRequest();
            }
        }
    }
}
