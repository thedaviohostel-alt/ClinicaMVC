using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicaMVC.Models
{
    [Table("medicos")]
    public class Medico
    {
        [Key]
        [Column("id_medico")]
        public int IdMedico { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Column("nombre")]
        [StringLength(100)]
        [Display(Name = "Nombre completo")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cédula es obligatoria")]
        [Column("cedula")]
        [StringLength(20)]
        public string Cedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "La especialidad es obligatoria")]
        [Column("especialidad")]
        [StringLength(100)]
        public string Especialidad { get; set; } = string.Empty;

        [Column("telefono")]
        [StringLength(20)]
        [Phone(ErrorMessage = "Número de teléfono inválido")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "El turno es obligatorio")]
        [Column("turno")]
        [StringLength(20)]
        public string Turno { get; set; } = string.Empty; // "Mañana" o "Tarde"

        [Column("fecha_registro")]
        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
