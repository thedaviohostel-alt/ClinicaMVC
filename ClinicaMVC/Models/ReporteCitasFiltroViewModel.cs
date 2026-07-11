using System.ComponentModel.DataAnnotations;

namespace ClinicaMVC.Models
{
    // Filtro usado en ReportesController para el reporte de citas por rango de fechas
    public class ReporteCitasFiltroViewModel
    {
        [Required(ErrorMessage = "La fecha inicial es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Desde")]
        public DateTime FechaInicio { get; set; } = DateTime.Today.AddDays(-7);

        [Required(ErrorMessage = "La fecha final es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Hasta")]
        public DateTime FechaFin { get; set; } = DateTime.Today;

        [Display(Name = "Estado")]
        public string? Estado { get; set; }
    }
}
