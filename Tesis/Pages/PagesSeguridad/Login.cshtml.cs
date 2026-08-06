using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Tesis.Pages.PagesSeguridad
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "El usuario es requerido")]
        public string usuario { get; set; } = "";
        [BindProperty]
        [Required(ErrorMessage = "La contraseña es requerida")]
        public string contrasena { get; set; } = "";

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostIngresarAsync()
        {
            usuario = Request.Form["usuario"];
            contrasena = Request.Form["contrasena"];

            Controladora unaControladora = new Controladora();
            if (unaControladora.ValidarCredenciales(usuario, contrasena))
            {
                // Se abre la sesion: la identidad queda guardada en una cookie y a
                // partir de aca el resto del sitio sabe que hay un usuario validado.
                List<Claim> datosDeLaSesion = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario)
                };

                ClaimsIdentity unaIdentidad = new ClaimsIdentity(datosDeLaSesion,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(unaIdentidad));

                return Redirect("/Index");
            }

            // Credenciales incorrectas: se deniega el acceso
            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos!");
            return Page();
        }

        public async Task<IActionResult> OnPostSalirAsync()
        {
            // Cierra la sesion y borra la cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/PagesSeguridad/Login");
        }
    }
}
