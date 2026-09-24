using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ClinicaVeterinaria.Models;
using ClinicaVeterinaria.Data;

namespace ClinicaVeterinaria.Controllers
{
    public class CitasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CitasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Citas
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            IQueryable<Cita> query = _context.Citas
                .Include(c => c.Mascota)
                .Include(c => c.ServicioVeterinario);

            // Si el usuario está autenticado y NO es Administrador, filtrar solo sus citas
            if (User.Identity != null && User.Identity.IsAuthenticated && !User.IsInRole("Administrador"))
            {
                query = query.Where(c => c.Mascota != null && c.Mascota.UsuarioId == userId);
            }

            return View(await query.OrderByDescending(c => c.Fecha).ToListAsync());
        }

        // GET: Citas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cita = await _context.Citas
                .Include(c => c.Mascota)
                .Include(c => c.ServicioVeterinario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cita == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (User.Identity != null && User.Identity.IsAuthenticated && !User.IsInRole("Administrador") && cita.Mascota?.UsuarioId != userId)
            {
                return Forbid();
            }

            return View(cita);
        }

        // GET: Citas/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);

            var misMascotas = User.IsInRole("Administrador")
                ? await _context.Mascotas.ToListAsync()
                : await _context.Mascotas.Where(m => m.UsuarioId == userId).ToListAsync();

            if (!misMascotas.Any() && !User.IsInRole("Administrador"))
            {
                TempData["Error"] = "Primero debe registrar una mascota antes de agendar una cita.";
                return RedirectToAction("Create", "Mascotas");
            }

            ViewData["MascotaId"] = new SelectList(misMascotas, "Id", "Nombre");
            ViewData["ServicioVeterinarioId"] = new SelectList(_context.ServiciosVeterinarios, "Id", "Nombre");
            return View();
        }

        // POST: Citas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Fecha,MascotaId,ServicioVeterinarioId")] Cita cita)
        {
            var userId = _userManager.GetUserId(User);

            if (cita.Fecha < DateTime.Now)
            {
                ModelState.AddModelError("Fecha", "No se puede agendar una cita en una fecha u hora pasada.");
            }

            bool existeCita = await _context.Citas.AnyAsync(c => c.MascotaId == cita.MascotaId && c.Fecha == cita.Fecha);
            if (existeCita)
            {
                ModelState.AddModelError("Fecha", "La mascota ya tiene una cita agendada para esa misma fecha y hora.");
            }

            if (!User.IsInRole("Administrador"))
            {
                bool esMascotaPropia = await _context.Mascotas.AnyAsync(m => m.Id == cita.MascotaId && m.UsuarioId == userId);
                if (!esMascotaPropia)
                {
                    ModelState.AddModelError("MascotaId", "Debe seleccionar una mascota válida registrada a su nombre.");
                }
            }

            if (ModelState.IsValid)
            {
                cita.Estado = "Pendiente";
                _context.Add(cita);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var misMascotas = User.IsInRole("Administrador")
                ? await _context.Mascotas.ToListAsync()
                : await _context.Mascotas.Where(m => m.UsuarioId == userId).ToListAsync();

            ViewData["MascotaId"] = new SelectList(misMascotas, "Id", "Nombre", cita.MascotaId);
            ViewData["ServicioVeterinarioId"] = new SelectList(_context.ServiciosVeterinarios, "Id", "Nombre", cita.ServicioVeterinarioId);
            return View(cita);
        }

        // GET: Citas/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cita = await _context.Citas.FindAsync(id);
            if (cita == null) return NotFound();

            ViewData["MascotaId"] = new SelectList(_context.Mascotas, "Id", "Nombre", cita.MascotaId);
            ViewData["ServicioVeterinarioId"] = new SelectList(_context.ServiciosVeterinarios, "Id", "Nombre", cita.ServicioVeterinarioId);
            return View(cita);
        }

        // POST: Citas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Fecha,Estado,MascotaId,ServicioVeterinarioId")] Cita cita)
        {
            if (id != cita.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cita);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CitaExists(cita.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MascotaId"] = new SelectList(_context.Mascotas, "Id", "Nombre", cita.MascotaId);
            ViewData["ServicioVeterinarioId"] = new SelectList(_context.ServiciosVeterinarios, "Id", "Nombre", cita.ServicioVeterinarioId);
            return View(cita);
        }

        // GET: Citas/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cita = await _context.Citas
                .Include(c => c.Mascota)
                .Include(c => c.ServicioVeterinario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cita == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (!User.IsInRole("Administrador") && cita.Mascota?.UsuarioId != userId)
            {
                return Forbid();
            }

            return View(cita);
        }

        // POST: Citas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita != null)
            {
                _context.Citas.Remove(cita);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CitaExists(int? id)
        {
            return _context.Citas.Any(e => e.Id == id);
        }
    }
}