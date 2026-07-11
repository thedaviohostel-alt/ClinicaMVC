using System.ComponentModel.DataAnnotations;

namespace ClinicaMVC.Models
{
    // Se usa en CitasController.Atender: muestra los datos de la cita (solo lectura)
    // y captura síntomas/diagnóstico/tratamiento para generar el HistorialMedico.
    public class AtenderCitaViewModel
    {
        public int IdCita { get; set; }

        [Display(Name = "Paciente")]
        public string NombrePaciente { get; set; } = string.Empty;

        [Display(Name = "Médico")]
        public string NombreMedico { get; set; } = string.Empty;

        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }

        [Display(Name = "Hora")]
        public TimeOnly Hora { get; set; }

        [Display(Name = "Síntomas")]
        public string? Sintomas { get; set; }

        [Required(ErrorMessage = "El diagnóstico es obligatorio")]
        [Display(Name = "Diagnóstico")]
        public string Diagnostico { get; set; } = string.Empty;

        [Display(Name = "Tratamiento")]
        public string? Tratamiento { get; set; }
    }
}
