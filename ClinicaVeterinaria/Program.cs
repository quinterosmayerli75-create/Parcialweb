<<<<<<< HEAD
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ClinicaVeterinaria.Data;
using QuestPDF.Infrastructure;

// Configurar licencia de QuestPDF (Comunitaria / Gratuita)
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar la cadena de conexión a la base de datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");
=======
using ClinicaVeterinaria.Data;
using ClinicaVeterinaria.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar la cadena de conexión a la Base de Datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
>>>>>>> origin/main

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

<<<<<<< HEAD
// 2. Configurar Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Configuración de rutas de autenticación para evitar el Error 404
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// 3. Agregar soporte para Controladores y Vistas
=======
// 2. Configurar ASP.NET Core Identity con ApplicationUser y Roles
builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4; // Para facilitar pruebas
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// 3. Agregar Vistas y Controladores
>>>>>>> origin/main
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Necesario para las páginas por defecto de Identity

var app = builder.Build();

<<<<<<< HEAD
// Configurar el pipeline de solicitudes HTTP
=======
// Configurar el pipeline HTTP
>>>>>>> origin/main
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

<<<<<<< HEAD
// 4. Habilitar Autenticación y Autorización
=======
// Autenticación y Autorización
>>>>>>> origin/main
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

<<<<<<< HEAD
=======
app.MapRazorPages(); // Mapear vistas de Login/Registro de Identity

// Poblar Roles al iniciar
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbInitializer.SeedRolesAndAdminAsync(services);
}

app.Run();

>>>>>>> origin/main
app.Run();