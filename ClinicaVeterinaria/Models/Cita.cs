using System.ComponentModel.DataAnnotations;

namespace ClinicaVeterinaria.Models
{
    public class Cita
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha y hora de la cita es obligatoria.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha y Hora")]
        public DateTime Fecha { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente";

        [Required(ErrorMessage = "Debe seleccionar una mascota.")]
        [Display(Name = "Mascota")]
        public int MascotaId { get; set; }
        public Mascota? Mascota { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un servicio.")]
        [Display(Name = "Servicio Veterinario")]
        public int ServicioVeterinarioId { get; set; }
        public ServicioVeterinario? ServicioVeterinario { get; set; }
    }
}