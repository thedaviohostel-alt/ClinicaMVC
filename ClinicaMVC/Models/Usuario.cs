using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicaMVC.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [Column("nombre_usuario")]
        [StringLength(50)]
        public string NombreUsuario { get; set; } = string.Empty;

        // Guarda el hash de la contraseña (BCrypt), nunca texto plano
        [Required]
        [Column("clave")]
        public string Clave { get; set; } = string.Empty;

        [Required]
        [Column("id_rol")]
        public int IdRol { get; set; }

        [ForeignKey(nameof(IdRol))]
        public Rol? Rol { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;
    }
}
