
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SistemaGimnasio.Data;
using SistemaGimnasio.Models;

namespace SistemaGimnasio.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class RolesController : Controller
    {
        private readonly GimnasioContext _context;

        public RolesController(GimnasioContext context)
        {
            _context = context;
        }

        // Listar roles
        public async Task<IActionResult> Index()
        {
            return View(await _context.Roles.ToListAsync());
        }

        // Crear rol
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NombreRol")] Role role)
        {
            if (ModelState.IsValid)
            {
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(role);
        }

        // Editar rol
        public async Task<IActionResult> Edit(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRol,NombreRol")] Role role)
        {
            if (id != role.IdRol)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var roleDB = await _context.Roles.FindAsync(id);

                if (roleDB == null)
                {
                    return NotFound();
                }

                roleDB.NombreRol = role.NombreRol;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(role);
        }

        // Eliminar rol
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = await _context.Roles
                .Include(r => r.Usuarios)
                .FirstOrDefaultAsync(r => r.IdRol == id);

            if (role == null)
            {
                return NotFound();
            }

            if (role.Usuarios.Any())
            {
                TempData["Error"] = "No se puede eliminar un rol con usuarios asociados.";
                return RedirectToAction(nameof(Index));
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
