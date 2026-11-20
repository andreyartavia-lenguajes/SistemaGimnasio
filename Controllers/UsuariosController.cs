
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaGimnasio.Data;
using SistemaGimnasio.Models;

namespace SistemaGimnasio.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly GimnasioContext _context;

        public UsuariosController(GimnasioContext context)
        {
            _context = context;
        }

        // Listar usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = _context.Usuarios
                .Include(u => u.IdRolNavigation);

            return View(await usuarios.ToListAsync());
        }

        // Crear usuario
        public IActionResult Create()
        {
            ViewBag.IdRol = new SelectList(
                _context.Roles, "IdRol", "NombreRol");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string nombre,
            string usuario1, string contrasena, int idRol)
        {
            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(usuario1) ||
                string.IsNullOrWhiteSpace(contrasena))
            {
                ViewBag.Error = "Complete todos los campos.";
            }
            else if (await _context.Usuarios.AnyAsync(
                u => u.Usuario1 == usuario1))
            {
                ViewBag.Error = "El usuario ya existe.";
            }
            else if (!await _context.Roles.AnyAsync(
                r => r.IdRol == idRol))
            {
                ViewBag.Error = "El rol no es válido.";
            }
            else
            {
                var usuario = new Usuario
                {
                    Nombre = nombre,
                    Usuario1 = usuario1,
                    IdRol = idRol
                };

                var passwordHasher = new PasswordHasher<Usuario>();

                usuario.ContrasenaHash = passwordHasher.HashPassword(
                    usuario, contrasena);

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.IdRol = new SelectList(
                _context.Roles, "IdRol", "NombreRol", idRol);

            return View();
        }

        // Editar usuario
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            ViewBag.IdRol = new SelectList(
                _context.Roles, "IdRol", "NombreRol", usuario.IdRol);

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string nombre,
            string usuario1, string? contrasena, int idRol)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(usuario1))
            {
                ViewBag.Error = "Complete los campos obligatorios.";
            }
            else if (await _context.Usuarios.AnyAsync(
                u => u.Usuario1 == usuario1 && u.IdUsuario != id))
            {
                ViewBag.Error = "El usuario ya existe.";
            }
            else if (!await _context.Roles.AnyAsync(
                r => r.IdRol == idRol))
            {
                ViewBag.Error = "El rol no es válido.";
            }
            else if (usuario.Usuario1 == User.Identity?.Name &&
                (usuario.IdRol != idRol || usuario.Usuario1 != usuario1))
            {
                ViewBag.Error = "No puede cambiar su propio rol o nombre de usuario.";
            }
            else
            {
                usuario.Nombre = nombre;
                usuario.Usuario1 = usuario1;
                usuario.IdRol = idRol;

                if (!string.IsNullOrWhiteSpace(contrasena))
                {
                    var passwordHasher = new PasswordHasher<Usuario>();

                    usuario.ContrasenaHash = passwordHasher.HashPassword(
                        usuario, contrasena);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.IdRol = new SelectList(
                _context.Roles, "IdRol", "NombreRol", idRol);

            return View(usuario);
        }

        // Eliminar usuario
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            if (usuario.Usuario1 == User.Identity?.Name)
            {
                TempData["Error"] = "No puede eliminar su propio usuario.";
                return RedirectToAction(nameof(Index));
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
