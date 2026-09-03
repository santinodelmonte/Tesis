using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.WebEncoders;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Tesis.Dominio;
using Tesis.Notificaciones;
using Tesis.Persistencia;

var builder = WebApplication.CreateBuilder(args);

// La cultura con la que el sistema muestra fechas y numeros, fijada a proposito.
//
// Sin esta linea, .NET toma la del sistema operativo de la maquina donde corre: el
// mismo sitio muestra 19/08/2026 en una computadora y 08/19/2026 en otra, y los
// decimales cambian de coma a punto. Lo que se ve en pantalla no puede depender de
// como quedo configurada la maquina.
//
// Esto es solo la salida. Los campos <input type="number"> del formulario viajan
// siempre con punto decimal, y se leen con InvariantCulture desde
// Pages/Shared/CampoNumerico.
CultureInfo vCultura = new CultureInfo("es-AR");

// Con cero adelante. El patron corto de es-AR es d/M/yyyy y deja las fechas
// desparejas en las tablas: 5/8/2026 al lado de 19/08/2026.
vCultura.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";

CultureInfo.DefaultThreadCurrentCulture = vCultura;
CultureInfo.DefaultThreadCurrentUICulture = vCultura;

// El codificador de Razor. El de fabrica solo deja pasar ASCII basico y convierte
// todo lo demas en entidades HTML: la enie y las tildes salian como &#xF3;. En el
// texto de la pagina no se nota -el navegador las decodifica- pero adentro de un
// bloque <script> queda la entidad literal en el string de JavaScript, y una
// comparacion contra una constante con tilde no coincide nunca.
//
// Paso de verdad: el selector de pajuela de Registrar servicio no aparecia jamas,
// porque comparaba contra "Inseminación artificial" y recibia
// "Inseminaci&#xF3;n artificial".
builder.Services.Configure<WebEncoderOptions>(opciones =>
{
    opciones.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
});

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

// El token del bot de Telegram, por el mismo motivo y por el mismo camino: es una
// credencial -quien lo tiene maneja el bot- y no puede estar escrita en el codigo.
//
// En desarrollo se lee de appsettings.Development.json, que no se versiona. En el
// hosting, de la variable de entorno Telegram__Token, porque ese archivo no se
// publica. Sin token el sistema funciona igual: la pantalla de notificaciones avisa
// que falta y el proceso del resumen no arranca.
BotTelegram.Configurar(builder.Configuration["Telegram:Token"] ?? "");

// El proceso programado de CU49. Arranca con el sitio, escucha los mensajes que le
// llegan al bot y manda el resumen diario a la hora configurada.
builder.Services.AddHostedService<ServicioNotificaciones>();

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
