using Microsoft.EntityFrameworkCore;
using ClinicaMVC.Models;

namespace ClinicaMVC.Data
{
    public class ClinicaContext : DbContext
    {
        public ClinicaContext(DbContextOptions<ClinicaContext> options) : base(options) { }

        public DbSet<Rol> Roles { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Paciente> Pacientes { get; set; } = null!;
        public DbSet<Medico> Medicos { get; set; } = null!;
        public DbSet<Cita> Citas { get; set; } = null!;
        public DbSet<HistorialMedico> HistorialesMedicos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Paciente>()
                .HasIndex(p => p.Cedula)
                .IsUnique();

            modelBuilder.Entity<Medico>()
                .HasIndex(m => m.Cedula)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.NombreUsuario)
                .IsUnique();

            modelBuilder.Entity<Rol>()
                .HasIndex(r => r.NombreRol)
                .IsUnique();

            // Acelera la verificación de disponibilidad del médico (id_medico + fecha + hora)
            modelBuilder.Entity<Cita>()
                .HasIndex(c => new { c.IdMedico, c.Fecha, c.Hora });

            // Restrict evita borrados en cascada accidentales de pacientes/médicos con citas o historial
            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Paciente)
                .WithMany()
                .HasForeignKey(c => c.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Medico)
                .WithMany()
                .HasForeignKey(c => c.IdMedico)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HistorialMedico>()
                .HasOne(h => h.Paciente)
                .WithMany()
                .HasForeignKey(h => h.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HistorialMedico>()
                .HasOne(h => h.Medico)
                .WithMany()
                .HasForeignKey(h => h.IdMedico)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HistorialMedico>()
                .HasOne(h => h.Cita)
                .WithMany(c => c.HistorialesMedicos)
                .HasForeignKey(h => h.IdCita)
                .OnDelete(DeleteBehavior.Restrict);

            // Tipos de columna explícitos: sin esto, Npgsql asume "timestamp with time zone"
            // por defecto para cualquier DateTime, y exige valores en UTC. Las columnas reales
            // en ClinicaDB.sql son DATE y TIMESTAMP (sin zona horaria), así que las declaramos
            // explícitamente para que acepten los valores tal como los genera el formulario/código.
            modelBuilder.Entity<Paciente>()
                .Property(p => p.FechaNacimiento)
                .HasColumnType("date");

            modelBuilder.Entity<Paciente>()
                .Property(p => p.FechaRegistro)
                .HasColumnType("timestamp without time zone");

            modelBuilder.Entity<Medico>()
                .Property(m => m.FechaRegistro)
                .HasColumnType("timestamp without time zone");

            modelBuilder.Entity<Cita>()
                .Property(c => c.Fecha)
                .HasColumnType("date");

            modelBuilder.Entity<HistorialMedico>()
                .Property(h => h.FechaConsulta)
                .HasColumnType("timestamp without time zone");

            // Datos semilla de roles (coincide con Módulos: Administrador, Recepcionista, Médico)
            modelBuilder.Entity<Rol>().HasData(
                new Rol { IdRol = 1, NombreRol = "Administrador" },
                new Rol { IdRol = 2, NombreRol = "Recepcionista" },
                new Rol { IdRol = 3, NombreRol = "Medico" }
            );
        }
    }
}