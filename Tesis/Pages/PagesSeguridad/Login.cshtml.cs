using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;
using System.ComponentModel.DataAnnotations;

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

        public IActionResult OnPostIngresar()
        {
            usuario = Request.Form["usuario"];
            contrasena = Request.Form["contrasena"];

            Controladora unaControladora = new Controladora();
            if (unaControladora.ValidarCredenciales(usuario, contrasena))
            {
                return Redirect("/Index");
            }

            // Credenciales incorrectas: se deniega el acceso
            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos!");
            return Page();
        }
    }
}
