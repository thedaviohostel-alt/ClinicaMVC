using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicaMVC.Models
{
    [Table("roles")]
    public class Rol
    {
        [Key]
        [Column("id_rol")]
        public int IdRol { get; set; }

        [Required]
        [Column("nombre_rol")]
        [StringLength(20)]
        public string NombreRol { get; set; } = string.Empty;

        public ICollection<Usuario>? Usuarios { get; set; }
    }

    // Constantes con los roles definidos en el proyecto (Administrador, Recepcionista, Médico)
    public static class Roles
    {
        public const string Administrador = "Administrador";
        public const string Recepcionista = "Recepcionista";
        public const string Medico = "Medico";
    }
}
