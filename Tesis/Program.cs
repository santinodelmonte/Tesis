using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;
using Tesis.Dominio;
using Tesis.Persistencia;

var builder = WebApplication.CreateBuilder(args);

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

// Las fotos de los animales son archivos dentro de wwwroot. La ruta fisica se
// resuelve aca por el mismo motivo que la cadena de conexion: la persistencia no
// tiene por que saber donde quedo instalado el sitio.
//
// WebRootPath viene vacio si la carpeta wwwroot no existe al arrancar. En ese caso
// se arma la ruta a mano: sin esto la aplicacion no levanta, y quedarse sin sitio
// entero por una carpeta de fotos que falta no es una respuesta razonable.
string vRutaWwwRoot = app.Environment.WebRootPath;
if (string.IsNullOrEmpty(vRutaWwwRoot))
{
    vRutaWwwRoot = Path.Combine(app.Environment.ContentRootPath, "wwwroot");
}

pFotoAnimal.Configurar(vRutaWwwRoot);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Las fotos se suben con el sistema andando, asi que no figuran en el manifiesto de
// recursos estaticos que arma la compilacion y MapStaticAssets no las publica. Por
// eso la carpeta se sirve aparte, como archivos comunes.
string vRutaFotos = Path.Combine(vRutaWwwRoot, pFotoAnimal.CARPETA);
Directory.CreateDirectory(vRutaFotos);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(vRutaFotos),
    RequestPath = "/" + pFotoAnimal.CARPETA
});

app.UseRouting();

// UseAuthentication tiene que ir antes de UseAuthorization: primero se resuelve
// quien es el usuario y recien despues si tiene permiso.
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
