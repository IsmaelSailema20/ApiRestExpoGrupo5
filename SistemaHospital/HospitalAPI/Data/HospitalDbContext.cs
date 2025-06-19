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

            public DbSet<CentroMedico> CentrosMedicos { get; set; }
            public DbSet<Medico> Medicos { get; set; }
            public DbSet<Ciudad> Ciudades { get; set; }
            public DbSet<Especialidad> Especialidades { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // Configurar la tabla ciudades
                modelBuilder.Entity<Ciudad>()
                    .ToTable("ciudad")
                    .HasKey(c => c.id);

                // Configurar la tabla centro_medico
                modelBuilder.Entity<CentroMedico>()
                    .ToTable("centro_medico")
                    .HasKey(c => c.id);

                modelBuilder.Entity<CentroMedico>()
                    .HasOne(c => c.Ciudad)
                    .WithMany()
                    .HasForeignKey(c => c.ciudad_id)
                    .HasConstraintName("fk_ciudad");

                // Configurar la tabla medico
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

                // Configurar la tabla especialidad
                modelBuilder.Entity<Especialidad>()
                    .ToTable("especialidad")
                    .HasKey(e => e.id);
            }
        }
}