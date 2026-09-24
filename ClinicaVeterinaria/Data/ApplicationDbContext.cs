<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ClinicaVeterinaria.Models;

namespace ClinicaVeterinaria.Data
{
    public class ApplicationDbContext : IdentityDbContext
=======
﻿using ClinicaVeterinaria.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ClinicaVeterinaria.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
>>>>>>> origin/main
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Mascota> Mascotas { get; set; }
<<<<<<< HEAD
        public DbSet<ServicioVeterinario> ServiciosVeterinarios { get; set; }
        public DbSet<Cita> Citas { get; set; }
=======

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Relación Usuario - Mascota (1 a Muchos)
            builder.Entity<Mascota>()
                .HasOne(m => m.Usuario)
                .WithMany(u => u.Mascotas)
                .HasForeignKey(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
>>>>>>> origin/main
    }
}