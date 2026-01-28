using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VGLog.Views;
using VGLog.Data;
using VGLog.Models;
using VGLog.Services;
using VGLog.Services.Interfaces;
using VGLog.Views.Home;
using Microsoft.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);

//Registrazione dei controller
builder.Services.AddControllers();

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddAntiforgery();
builder.Services.AddHttpClient();


//Registrazione dei services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<VideogameService>();
builder.Services.AddScoped<SoftwareHouseService>();
builder.Services.AddScoped<PlatformService>();
builder.Services.AddScoped<GenreService>();


// 2) Identity configuration
// AddIdentity registra UserManager, SignInManager, PasswordHasher, stores, token providers, options, ecc.
// Internamente:
// - registra implementazioni di IUserStore/IUserPasswordStore (EntityFramework stores)
// - registra PasswordHasher (hash sicuro, salt, PBKDF2 per default in Microsoft impl.)
// - registra UserValidators, PasswordValidators, SecurityStampValidator...
// - registra SignInManager (responsabile di creare ClaimsPrincipal e invocare IAuthenticationService per la cookie)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
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

    // sicurezza cookie
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;

    options.Events = new CookieAuthenticationEvents
    {

    };
});

// 4) MVC / Razor
builder.Services.AddControllersWithViews();


//Registrazione del DbContext
builder.Services.AddDbContext <AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

// routing deve essere registrato prima della auth endpoint mapping
app.UseRouting();

// authentication deve venire prima di authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapStaticAssets();
app.UseAntiforgery();

app.Run();
