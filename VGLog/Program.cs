using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Threading.RateLimiting;
using VGLog.Components;
using VGLog.Data;
using VGLog.Models;
using VGLog.Services;
using VGLog.Services.Interfaces;

// punto d'ingresso dell'app
var builder = WebApplication.CreateBuilder(args);

//usato per pagine Account (es: login)
builder.Services.AddControllersWithViews(options =>
{
    //creazione  di una policy che autorizza solamente gli utenti autenticati ad accedere globalmente ai vari controller, è possibile ignorare questo metodo con [AllowAnonymous]
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

//eccezione della policy globale di restrizioni verso gli utenti non loggati per poter accedere al login anche da anonimi
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/account/Login");
});

// registrazione dei servizi necessari dei componenti blazor (DialogService, NotificationService, tooltipService, ContextMenuService)
builder.Services.AddRadzenComponents();

//permette di accedere all'HttpContext da dentro un servizio che normalmente non potrebbe (cookies, utente loggato, dati della richiesta, ecc...)
builder.Services.AddHttpContextAccessor();

//servizio di creazione token per le richieste, in modo tale da validarla
builder.Services.AddAntiforgery();



//Registrazione dei services tramite dependency injection, che possono essere: Scoped, Transient, Singleton.
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<VideogameService>();
builder.Services.AddScoped<SoftwareHouseService>();
builder.Services.AddScoped<PlatformService>();
builder.Services.AddScoped<GenreService>();
builder.Services.AddScoped<UserGamesService>();

//Identity è il sistema di ASP.NET per la gestione degli utenti, registra una serie di servizi pronti:
//- UserManager (creazione utenti),
//- SignInManager (gestione login/logout e cookie),
//- RoleManager(gstione ruoli),
//- PasswordHasher(trasforma le password in hash sicuro per non salvare in chiaro nel db)


//ApplicationUser è una classe custom che estende IdentityUser, serve ad aggiungere campi falcoltativi qualora servissero
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // requisiti minimi della password
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    options.User.RequireUniqueEmail = true;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
})
    .AddEntityFrameworkStores<AppDbContext>() // Creazioni di tabelle addizionali necessarie a Identity nel db 
    .AddDefaultTokenProviders(); // token provider usato per reset password, conferma email


//Configurazione della creazione del cookie nel browser per riconoscimento dell'utente, importante che stia dopo AddIdentity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login"; //reindirizzazione automatica dell'utente non loggato al tentativo di accedere ad una pagina protetta

    //protezione da attacchi JS
    options.Cookie.HttpOnly = true; 

    //cookie funzionante in http locale qualora l'app sia in development, in produzione switcha a Https
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.None
        : CookieSecurePolicy.Always;

    //il cookie viene inviato nelle navigazioni normali, ma non in richieste automatiche cross-site
    options.Cookie.SameSite = SameSiteMode.Lax;

    //durata del cookie: 14 giorni, dopodiché bisogna fare il login nuovamente
    options.ExpireTimeSpan = TimeSpan.FromDays(14);

    //se l'utente continua ad entrare nel sito, il timer dei 14 giorni si resetta
    options.SlidingExpiration = true;
});

//configurazione di sqlite con stringa di connessione presa dai settings.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

//Registrazione dell'httpclient come servizio che userà l'url base dell'app per le chiamate http relative
builder.Services.AddScoped(sp =>
{
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(navigationManager.BaseUri) };
});


// aggiunto ratelimiter per limitare a 10 le richieste http degli utenti in un determinato intervallo di tempo (in questo caso 10 minuti), preso da documentazione MS
builder.Services.AddRateLimiter(options =>
{
    // GlobalLimiter serve per TUTTE le richieste HTTP dell'app, indipendentemente dall'endpoint.
    // PartitionedRateLimiter inserisce un "contatore" per ogni client. HttpContext: la fonte dei dati (la richiesta HTTP corrente) string: il tipo della chiave di partizione (l'id del client)
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        // visto che anche le richieste per i file statici (come il css) vengono contate verso il rate limiter, vengono ignorate
        var path = httpContext.Request.Path.Value ?? "";

        if (path.StartsWith("/_blazor") ||
            path.StartsWith("/_framework") ||
            path.StartsWith("/css") ||
            path.EndsWith(".css") ||
            path.EndsWith(".js") ||
            path.EndsWith(".woff2") ||
            path.StartsWith("/js") ||
            path.StartsWith("/_vs"))
        {
            return RateLimitPartition.GetNoLimiter<string>("static");
        }

        // modo per trovare le chiamate inviate ed escludere poi successevimante dal ratelimiter
        // Console.WriteLine($"[RateLimit] Path: {path} | Client: {httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown"}");
       
        // GetFixedWindowLimiter usa una "finestra temporale" fissa
        return RateLimitPartition.GetFixedWindowLimiter(
                // variabile per l'id del client che prova ad usarlo; si tenta, in ordine: username utente autenticato, ip diretto della connessione, come fallback "unknown"
                partitionKey: httpContext.User.Identity?.Name
                ?? httpContext.Connection.RemoteIpAddress?.ToString()
                ?? "unknown",
                factory: partition =>
                {
                    return new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        // numero richieste permesse (messo a 100 per evitare di venir bloccato in debug)
                        PermitLimit = 100,
                        // coda delle richieste in caso esse abbiano superato il limite (in questo caso non c'è coda)
                        QueueLimit = 0,
                        //tempo di azzeramento della finestra temprale
                        Window = TimeSpan.FromMinutes(1)
                    };
                });

    });
});

var app = builder.Build();

// da qui in poi l'app segue un pipeline in ordine sequenziale, sbagliare ordine porta ad errori

if (!app.Environment.IsDevelopment())
{
    //in produzione gli errori non gestiti vengono reindirizzati ad una pagina di errore generica
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

    //Invio di un header al browser per dirgli di usare sempre HTTPS anche in caso l'utente provi ad accedere via http manualmente
    app.UseHsts();
}

//reindirizzazione delle richieste http ad https
app.UseHttpsRedirection();

//attivazione del rate limiter
app.UseRateLimiter();

//analisi dell'url e determina a quale endpoint è destinato
app.UseRouting();

//serve i file dalla cartella wwwroot
app.UseStaticFiles();

//lettura dei cookie di sessione per restituire l'identità all'utente, leggibili dal HttpContext.User
app.UseAuthentication();

//Controllo dei permessi per la risorsa richiesta
app.UseAuthorization();

//validazione dei token anti forgery sulle richieste in scrittura
app.UseAntiforgery();

//Mappa gli endpoint
app.MapControllers();

//mappa la convenzione delle route dei controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//ottimizzazione degli asset statici
app.MapStaticAssets();

//mappa le razor pages, ogni .cshtml con @page diventa un endpoint
app.MapRazorPages();

//comunicazione in real-time 
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

//Avvio dell web server
app.Run();
