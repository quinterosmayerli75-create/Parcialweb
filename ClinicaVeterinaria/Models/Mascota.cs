using System.ComponentModel.DataAnnotations;

namespace ClinicaVeterinaria.Models
{
    public class Mascota
    {
        public int Id { get; set; }

<<<<<<< HEAD
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Especie { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Raza { get; set; } = string.Empty;

        public string UsuarioId { get; set; } = string.Empty;

        public virtual ICollection<Cita>? Citas { get; set; }
=======
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La especie es obligatoria")]
        [StringLength(30)]
        public string Especie { get; set; } = string.Empty;

        [StringLength(30)]
        public string Raza { get; set; } = string.Empty;

        [Range(0, 50, ErrorMessage = "La edad debe estar entre 0 y 50 años")]
        public int Edad { get; set; }

        [Range(0.1, 200.0, ErrorMessage = "El peso debe ser mayor a 0")]
        public decimal Peso { get; set; }

        // Relación con el usuario (cliente)
        public string? UsuarioId { get; set; }
        public ApplicationUser? Usuario { get; set; }
>>>>>>> origin/main
    }
}