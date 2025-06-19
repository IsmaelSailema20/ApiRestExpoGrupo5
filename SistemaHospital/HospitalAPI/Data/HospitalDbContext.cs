// Data/HospitalDbContext.cs
using HospitalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Data
{

        public class HospitalDbContext : DbContext
        {
            public HospitalDbContext(DbContextOptions<HospitalDbContext> options)
                : base(options)
            {
            }
            public DbSet<Usuario> Usuarios { get; set; }
            public DbSet<Empleado> Empleados { get; set; }
            public DbSet<CentroMedico> CentrosMedicos { get; set; }
            public DbSet<Medico> Medicos { get; set; }
            public DbSet<Ciudad> Ciudades { get; set; }
            public DbSet<Especialidad> Especialidades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ciudad
            modelBuilder.Entity<Ciudad>()
                .ToTable("ciudad")
                .HasKey(c => c.id);

            // centro_medico
            modelBuilder.Entity<CentroMedico>()
                .ToTable("centro_medico")
                .HasKey(c => c.id);

            modelBuilder.Entity<CentroMedico>()
                .HasOne(c => c.Ciudad)
                .WithMany()
                .HasForeignKey(c => c.ciudad_id)
                .HasConstraintName("fk_ciudad");

            // especialidad
            modelBuilder.Entity<Especialidad>()
                .ToTable("especialidad")
                .HasKey(e => e.id);

            // medico
            modelBuilder.Entity<Medico>()
                .ToTable("medico")
                .HasKey(m => m.id);

            modelBuilder.Entity<Medico>()
                .HasOne(m => m.CentroMedico)
                .WithMany()
                .HasForeignKey(m => m.id_centro_medico)
                .HasConstraintName("fk_centro_medico");

            modelBuilder.Entity<Medico>()
                .HasOne(m => m.Especialidad)
                .WithMany()
                .HasForeignKey(m => m.id_especialidad)
                .HasConstraintName("fk_especialidad");

            // empleado
            modelBuilder.Entity<Empleado>()
                .ToTable("empleado")
                .HasKey(e => e.Id);

            modelBuilder.Entity<Empleado>()
                .HasOne(e => e.CentroMedico)
                .WithMany()
                .HasForeignKey(e => e.id_centro_medico)
                .HasConstraintName("fk_empleado_centro")
                .OnDelete(DeleteBehavior.Restrict);

            // usuario
            modelBuilder.Entity<Usuario>()
                .ToTable("usuario")
                .HasKey(u => u.Id);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Medico)
                .WithMany()
                .HasForeignKey(u => u.id_medico)
                .HasConstraintName("fk_usuario_medico")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Empleado)
                .WithMany()
                .HasForeignKey(u => u.id_empleado)
                .HasConstraintName("fk_usuario_empleado")
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}