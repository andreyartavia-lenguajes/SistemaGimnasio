
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SistemaGimnasio.Data;
using SistemaGimnasio.Models;
using System.Security.Claims;

namespace SistemaGimnasio.Controllers
{
    public class LoginController : Controller
    {
        private readonly GimnasioContext _context;

        public LoginController(GimnasioContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string usuario, string contrasena)
        {
            var usuarioDB = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.Usuario1 == usuario);

            if (usuarioDB != null)
            {
                var passwordHasher = new PasswordHasher<Usuario>();

                var resultado = passwordHasher.VerifyHashedPassword(
                    usuarioDB,
                    usuarioDB.ContrasenaHash,
                    contrasena
                );

                if (resultado != PasswordVerificationResult.Failed)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuarioDB.Usuario1),
                        new Claim(ClaimTypes.Role, usuarioDB.IdRolNavigation.NombreRol)
                    };

                    var identity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme
                    );

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity)
                    );

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Usuario o contraseña incorrectos";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction("Index", "Login");
        }
    }
}
