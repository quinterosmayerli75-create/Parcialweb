using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ClinicaVeterinaria.Models;
using ClinicaVeterinaria.Data;

namespace ClinicaVeterinaria.Controllers
{
    public class ServicioVeterinariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicioVeterinariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SERVICIOVETERINARIOS (Accesible por Clientes y Administradores)
        public async Task<IActionResult> Index()
        {
            return View(await _context.ServiciosVeterinarios.ToListAsync());
        }

        // GET: SERVICIOVETERINARIOS/Details/5 (Accesible por Clientes y Administradores)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicioveterinario = await _context.ServiciosVeterinarios
                .FirstOrDefaultAsync(m => m.Id == id);

            if (servicioveterinario == null)
            {
                return NotFound();
            }

            return View(servicioveterinario);
        }

        // GET: SERVICIOVETERINARIOS/Create (Restringido solo a Administrador)
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: SERVICIOVETERINARIOS/Create (Restringido solo a Administrador)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,Precio")] ServicioVeterinario servicioveterinario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(servicioveterinario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(servicioveterinario);
        }

        // GET: SERVICIOVETERINARIOS/Edit/5 (Restringido solo a Administrador)
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicioveterinario = await _context.ServiciosVeterinarios.FindAsync(id);
            if (servicioveterinario == null)
            {
                return NotFound();
            }
            return View(servicioveterinario);
        }

        // POST: SERVICIOVETERINARIOS/Edit/5 (Restringido solo a Administrador)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Nombre,Descripcion,Precio")] ServicioVeterinario servicioveterinario)
        {
            if (id != servicioveterinario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servicioveterinario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicioVeterinarioExists(servicioveterinario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(servicioveterinario);
        }

        // GET: SERVICIOVETERINARIOS/Delete/5 (Restringido solo a Administrador)
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicioveterinario = await _context.ServiciosVeterinarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (servicioveterinario == null)
            {
                return NotFound();
            }

            return View(servicioveterinario);
        }

        // POST: SERVICIOVETERINARIOS/Delete/5 (Restringido solo a Administrador)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var servicioveterinario = await _context.ServiciosVeterinarios.FindAsync(id);
            if (servicioveterinario != null)
            {
                _context.ServiciosVeterinarios.Remove(servicioveterinario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServicioVeterinarioExists(int? id)
        {
            return _context.ServiciosVeterinarios.Any(e => e.Id == id);
        }
    }
}