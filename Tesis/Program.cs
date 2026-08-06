using Microsoft.AspNetCore.Authentication.Cookies;
using Tesis.Dominio;
using Tesis.Persistencia;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    // Todo el sitio queda detras del login. Las unicas excepciones son la propia
    // pagina de inicio de sesion y la de error.
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/PagesSeguridad/Login");
    options.Conventions.AllowAnonymousToPage("/Error");
});

// Autenticacion por cookie: si no hay sesion iniciada el sitio manda al login
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opciones =>
    {
        opciones.LoginPath = "/PagesSeguridad/Login";
        opciones.AccessDeniedPath = "/PagesSeguridad/Login";
    });

// Los datos de conexion a MySQL y las credenciales de acceso se leen de la
// configuracion, no estan escritos en el codigo fuente.
pConexion.Configurar(builder.Configuration.GetConnectionString("Tambo") ?? "");
Controladora.ConfigurarCredenciales(
    builder.Configuration["Seguridad:Usuario"] ?? "",
    builder.Configuration["Seguridad:Contrasena"] ?? "");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

// UseAuthentication tiene que ir antes de UseAuthorization: primero se resuelve
// quien es el usuario y recien despues si tiene permiso.
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
