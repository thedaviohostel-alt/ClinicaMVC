using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicaMVC.Models
{
    [Table("citas")]
    public class Cita
    {
        [Key]
        [Column("id_cita")]
        public int IdCita { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un paciente")]
        [Column("id_paciente")]
        [Display(Name = "Paciente")]
        public int IdPaciente { get; set; }

        [ForeignKey(nameof(IdPaciente))]
        public Paciente? Paciente { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un médico")]
        [Column("id_medico")]
        [Display(Name = "Médico")]
        public int IdMedico { get; set; }

        [ForeignKey(nameof(IdMedico))]
        public Medico? Medico { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [Column("fecha")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }

        // TimeOnly se usa (en vez de TimeSpan) porque Npgsql lo mapea de forma nativa
        // a la columna PostgreSQL "TIME" (TimeSpan se mapearía como "interval").
        [Required(ErrorMessage = "La hora es obligatoria")]
        [Column("hora")]
        [DataType(DataType.Time)]
        [Display(Name = "Hora")]
        public TimeOnly Hora { get; set; }

        [Column("estado")]
        [StringLength(20)]
        public string Estado { get; set; } = EstadosCita.Pendiente;

        [Column("observaciones")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        public ICollection<HistorialMedico>? HistorialesMedicos { get; set; }
    }

    // Constantes con los estados definidos en el proyecto (Pendiente, Atendida, Cancelada)
    public static class EstadosCita
    {
        public const string Pendiente = "Pendiente";
        public const string Atendida = "Atendida";
        public const string Cancelada = "Cancelada";
    }
}
