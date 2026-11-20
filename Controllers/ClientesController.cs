
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SistemaGimnasio.Data;
using SistemaGimnasio.Models;

namespace SistemaGimnasio.Controllers
{
    [Authorize(Roles = "Administrador,Empleado")]
    public class ClientesController : Controller
    {
        private readonly GimnasioContext _context;

        public ClientesController(GimnasioContext context)
        {
            _context = context;
        }

        // Listar clientes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clientes.ToListAsync());
        }

        // Crear cliente
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Nombre,Apellido,Telefono,Correo")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(cliente);
        }

        // Editar cliente
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("IdCliente,Nombre,Apellido,Telefono,Correo")] Cliente cliente)
        {
            if (id != cliente.IdCliente)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var clienteDB = await _context.Clientes.FindAsync(id);

                if (clienteDB == null)
                {
                    return NotFound();
                }

                clienteDB.Nombre = cliente.Nombre;
                clienteDB.Apellido = cliente.Apellido;
                clienteDB.Telefono = cliente.Telefono;
                clienteDB.Correo = cliente.Correo;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(cliente);
        }

        // Eliminar cliente
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Asistencia)
                .Include(c => c.Membresia)
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
            {
                return NotFound();
            }

            if (cliente.Asistencia.Any() || cliente.Membresia.Any())
            {
                TempData["Error"] = "No se puede eliminar un cliente con asistencias o membresías asociadas.";

                return RedirectToAction(nameof(Index));
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
