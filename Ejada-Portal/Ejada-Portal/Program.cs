using Application.ServiceManager;
using Application.Services;
using Application.Services.IServices;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repository;
using Infrastructure.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentity<User, IdentityRole>()
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
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IServiceManager, ServiceManager>();

builder.Services.ConfigureApplicationCookie(o => o.LoginPath = "/User/Login");
builder.Services.AddAuthorization();

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
