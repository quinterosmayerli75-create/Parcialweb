<<<<<<< HEAD
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaVeterinaria.Models;
using ClinicaVeterinaria.Data;

namespace ClinicaVeterinaria.Controllers
{
    public class MascotasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MascotasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MASCOTAS
        public async Task<IActionResult> Index()
        {
            return View(await _context.Mascotas.ToListAsync());
        }

        // GET: MASCOTAS/Details/5
=======
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaVeterinaria.Data;
using ClinicaVeterinaria.Models;

namespace ClinicaVeterinaria.Controllers
{
    [Authorize] // Requiere estar autenticado
    public class MascotasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MascotasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Mascotas
        public async Task<IActionResult> Index()
        {
            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null) return Challenge();

            // Si es Administrador, ve todas las mascotas; si es Cliente, solo las suyas
            if (User.IsInRole("Administrador"))
            {
                var todasMascotas = await _context.Mascotas.Include(m => m.Usuario).ToListAsync();
                return View(todasMascotas);
            }

            var misMascotas = await _context.Mascotas
                .Where(m => m.UsuarioId == usuarioActual.Id)
                .ToListAsync();

            return View(misMascotas);
        }
        // GET: Mascotas/Details/5
>>>>>>> origin/main
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mascota = await _context.Mascotas
<<<<<<< HEAD
                .FirstOrDefaultAsync(m => m.Id == id);
=======
                .Include(m => m.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

>>>>>>> origin/main
            if (mascota == null)
            {
                return NotFound();
            }

            return View(mascota);
        }

<<<<<<< HEAD
        // GET: MASCOTAS/Create
=======
        // GET: Mascotas/Create
>>>>>>> origin/main
        public IActionResult Create()
        {
            return View();
        }

<<<<<<< HEAD
        // POST: MASCOTAS/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Especie,Raza,UsuarioId")] Mascota mascota)
        {
=======
        // POST: Mascotas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Mascota mascota)
        {
            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null) return Challenge();

            mascota.UsuarioId = usuarioActual.Id;

            // Limpiamos la validación del objeto de navegación para evitar errores de ModelState
            ModelState.Remove("Usuario");
            ModelState.Remove("UsuarioId");

>>>>>>> origin/main
            if (ModelState.IsValid)
            {
                _context.Add(mascota);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mascota);
        }

<<<<<<< HEAD
        // GET: MASCOTAS/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mascota = await _context.Mascotas.FindAsync(id);
            if (mascota == null)
            {
                return NotFound();
            }
            return View(mascota);
        }

        // POST: MASCOTAS/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Nombre,Especie,Raza,UsuarioId")] Mascota mascota)
        {
            if (id != mascota.Id)
            {
                return NotFound();
            }
=======
        // GET: Mascotas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var mascota = await _context.Mascotas.FindAsync(id);
            if (mascota == null) return NotFound();

            var usuarioActual = await _userManager.GetUserAsync(User);
            if (mascota.UsuarioId != usuarioActual?.Id && !User.IsInRole("Administrador"))
            {
                return Forbid();
            }

            return View(mascota);
        }

        // POST: Mascotas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Mascota mascota)
        {
            if (id != mascota.Id) return NotFound();

            ModelState.Remove("Usuario");
            ModelState.Remove("UsuarioId");
>>>>>>> origin/main

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mascota);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
<<<<<<< HEAD
                    if (!MascotaExists(mascota.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
=======
                    if (!_context.Mascotas.Any(e => e.Id == mascota.Id)) return NotFound();
                    else throw;
>>>>>>> origin/main
                }
                return RedirectToAction(nameof(Index));
            }
            return View(mascota);
        }

<<<<<<< HEAD
        // GET: MASCOTAS/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mascota = await _context.Mascotas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mascota == null)
            {
                return NotFound();
            }
=======
        // GET: Mascotas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var mascota = await _context.Mascotas
                .Include(m => m.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mascota == null) return NotFound();
>>>>>>> origin/main

            return View(mascota);
        }

<<<<<<< HEAD
        // POST: MASCOTAS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
=======
        // POST: Mascotas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
>>>>>>> origin/main
        {
            var mascota = await _context.Mascotas.FindAsync(id);
            if (mascota != null)
            {
                _context.Mascotas.Remove(mascota);
<<<<<<< HEAD
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MascotaExists(int? id)
        {
            return _context.Mascotas.Any(e => e.Id == id);
        }
=======
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
>>>>>>> origin/main
    }
}