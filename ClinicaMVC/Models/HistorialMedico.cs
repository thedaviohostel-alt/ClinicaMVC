using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicaMVC.Models
{
    [Table("historial_medico")]
    public class HistorialMedico
    {
        [Key]
        [Column("id_historial")]
        public int IdHistorial { get; set; }

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

        // Opcional: una consulta puede registrarse sin estar vinculada a una cita agendada
        [Column("id_cita")]
        [Display(Name = "Cita")]
        public int? IdCita { get; set; }

        [ForeignKey(nameof(IdCita))]
        public Cita? Cita { get; set; }

        [Column("fecha_consulta")]
        [Display(Name = "Fecha de consulta")]
        public DateTime FechaConsulta { get; set; } = DateTime.Now;

        [Column("sintomas")]
        [Display(Name = "Síntomas")]
        public string? Sintomas { get; set; }

        [Required(ErrorMessage = "El diagnóstico es obligatorio")]
        [Column("diagnostico")]
        [Display(Name = "Diagnóstico")]
        public string Diagnostico { get; set; } = string.Empty;

        [Column("tratamiento")]
        [Display(Name = "Tratamiento")]
        public string? Tratamiento { get; set; }
    }
}
