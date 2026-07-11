using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ClinicaMVC.Data;
using ClinicaMVC.Services;
using QuestPDF.Infrastructure;

// QuestPDF requiere declarar el tipo de licencia; Community es gratuita para
// proyectos académicos/pequeños. Ver: https://www.questpdf.com/license/
QuestPDF.Settings.License = LicenseType.Community;


var builder = WebApplication.CreateBuilder(args);

// Conexión a PostgreSQL usando el connection string de appsettings.json
builder.Services.AddDbContext<ClinicaContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ClinicaDB")));

// Autenticación basada en cookies + control de acceso por roles
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ReporteService>();

var app = builder.Build();

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
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
