using Application.ServiceManager;
using Application.Services;
using Application.Services.IServices;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repository;
using Infrastructure.Repository.IRepository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentityCore<User>()
    .AddRoles<IdentityRole>() // <-- Add this line
    .AddSignInManager()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Options for Gmail
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));

builder.Services.AddScoped<GmailEmailProvider>();
builder.Services.AddScoped<RnwoodEmailProvider>();

// ??? ??? Resolver
builder.Services.AddScoped<IEmailProviderResolver, EmailProviderResolver>();

// Template renderer
builder.Services.AddScoped<IEmailTemplateRenderer, FileEmailTemplateRenderer>();

// Infra & Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddHttpClient("Jira", client =>
{
    var jiraSection = builder.Configuration.GetSection("Jira");
    var email = jiraSection["Email"];
    var apiToken = jiraSection["ApiToken"];
    var baseUrl = jiraSection["BaseUrl"];

    string auth = Convert.ToBase64String(
        System.Text.Encoding.ASCII.GetBytes($"{email}:{apiToken}")
    );

    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

//builder.Services.AddScoped<JiraService>();

// ServiceManager
builder.Services.AddScoped<IServiceManager, ServiceManager>();
//// Configure application cookie for Identity:

builder.Services.AddAuthentication
    (options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "oidc";
    })
              .AddCookie(options =>
              {
                  options.Cookie.HttpOnly = true;
                  options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                  options.LoginPath = "/User/Login";
                  options.AccessDeniedPath = "/User/AccessDenied";
                  options.SlidingExpiration = true;
              }).AddOpenIdConnect("oidc", options =>
              {
                  //The Authority indicates where the trusted token service is located
                  options.Authority = builder.Configuration["ServiceUrls:IdentityAPI"];
                  options.GetClaimsFromUserInfoEndpoint = true;
                  options.ClientId = "magic";
                  options.ClientSecret = "secret";
                  options.ResponseType = "code";
                  options.TokenValidationParameters.NameClaimType = "name";
                  options.TokenValidationParameters.RoleClaimType = "role";
                  options.Scope.Add("magic");
                  options.SaveTokens = true;
                  options.ClaimActions.MapJsonKey("role", "role");
                  options.Events = new OpenIdConnectEvents
                  {
                      OnRemoteFailure = context =>
                      {
                          context.Response.Redirect("/");
                          context.HandleResponse();
                          return Task.FromResult(0);
                      }
                  };
              });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var resolver = scope.ServiceProvider.GetRequiredService<IEmailProviderResolver>();
    Console.WriteLine("[DEBUG] Resolver Gmail => " + resolver.Get("Gmail").Name);
    Console.WriteLine("[DEBUG] Resolver Rnwood => " + resolver.Get("Rnwood").Name);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
