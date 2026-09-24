using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaVeterinaria.Data;

namespace ClinicaVeterinaria.Controllers
{
    [Authorize(Roles = "Administrador")] // Acceso exclusivo para el Administrador
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Contadores para las tarjetas del Dashboard
            ViewBag.TotalMascotas = await _context.Mascotas.CountAsync();
            ViewBag.TotalUsuarios = await _context.Users.CountAsync();

            // Verificar si la tabla Citas o Servicios existe para evitar errores antes de que tu compañera las cree
            ViewBag.TotalServicios = _context.Model.FindEntityType("ClinicaVeterinaria.Models.ServicioVeterinario") != null
                ? await _context.Set<Models.Mascota>().CountAsync() : 0; // Se actualizará cuando esté la entidad

            ViewBag.TotalCitas = 0;

            // Datos ficticios para el gráfico de Citas por Mes (se conectarán con la BD real cuando tu compañera cree la tabla Citas)
            ViewBag.Meses = new string[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio" };
            ViewBag.CitasPorMes = new int[] { 12, 19, 8, 15, 22, 10 };

            return View();
        }
    }
}