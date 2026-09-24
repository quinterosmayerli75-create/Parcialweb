using System.ComponentModel.DataAnnotations;

namespace ClinicaVeterinaria.Models
{
    public class Mascota
    {
        public int Id { get; set; }

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
    }
}