using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicaMVC.Models
{
    [Table("pacientes")]
    public class Paciente
    {
        [Key]
        [Column("id_paciente")]
        public int IdPaciente { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Column("nombre")]
        [StringLength(100)]
        [Display(Name = "Nombre completo")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cédula es obligatoria")]
        [Column("cedula")]
        [StringLength(20)]
        [RegularExpression(@"^\d{3}-?\d{7}-?\d{1}$", ErrorMessage = "Formato de cédula inválido (ej: 001-1234567-8)")]
        public string Cedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [Column("fecha_nacimiento")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [Column("telefono")]
        [StringLength(20)]
        [Phone(ErrorMessage = "Número de teléfono inválido")]
        public string? Telefono { get; set; }

        [Column("direccion")]
        [StringLength(200)]
        public string? Direccion { get; set; }

        [Column("tipo_sangre")]
        [StringLength(5)]
        [Display(Name = "Tipo de sangre")]
        public string? TipoSangre { get; set; }

        [Column("fecha_registro")]
        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
