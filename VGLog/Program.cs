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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Account/Login");
});

builder.Services.AddRadzenComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAntiforgery();
//builder.Services.AddHttpClient();


//Registrazione dei services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<VideogameService>();
builder.Services.AddScoped<SoftwareHouseService>();
builder.Services.AddScoped<PlatformService>();
builder.Services.AddScoped<GenreService>();
builder.Services.AddScoped<UserGamesService>();



//Registrazione di radzen.blazor
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();


// 2) Identity configuration
// AddIdentity registra UserManager, SignInManager, PasswordHasher, stores, token providers, options, ecc.
// Internamente:
// - registra implementazioni di IUserStore/IUserPasswordStore (EntityFramework stores)
// - registra PasswordHasher (hash sicuro, salt, PBKDF2 per default in Microsoft impl.)
// - registra UserValidators, PasswordValidators, SecurityStampValidator...
// - registra SignInManager (responsabile di creare ClaimsPrincipal e invocare IAuthenticationService per la cookie)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // requisiti della password
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    options.User.RequireUniqueEmail = true;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
})
    .AddEntityFrameworkStores<AppDbContext>() // registra i store EF
    .AddDefaultTokenProviders(); // token provider usato per reset password, conferma email

// 3) Configura cookie di Identity (Identity usa uno schema cookie predefinito "Identity.Application")
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";

    // opzioni dei cookie
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.None
        : CookieSecurePolicy.Always; options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
});

string dbPath;

//if (builder.Environment.IsEnvironment("GitHubActions"))
//{
//    // CI runner: path temporaneo
//    dbPath = "/tmp/vglog.db";
//}
//else if (builder.Environment.IsProduction())
//{
//    // Azure App Service: path scrivibile
//    dbPath = Path.Combine(Environment.GetEnvironmentVariable("HOME")!, "vglog.db");
//}
//else
//{
//    // Locale
//    dbPath = "vglog.db";
//}

//Registrazione del DbContext

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

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
        // modo per trovare cosa conta verso il ratelimiter
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

//if (!builder.Environment.IsEnvironment("GitHubActions"))
//{
//    using (var scope = app.Services.CreateScope())
//    {
//        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//        db.Database.Migrate();
//    }
//}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseRouting();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// aggiunto middleware del ratelimited dichiarato sopra
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapStaticAssets();
app.MapRazorPages();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
//app.MapBlazorHub();
//app.MapRazorPages();

app.Run();
