
using HospitalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Data
{
    public class PacienteDbContext : DbContext
    {
        public PacienteDbContext(DbContextOptions<PacienteDbContext> options)
            : base(options)
        {
        }

        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<ConsultaMedica> ConsultasMedicas { get; set; }
        public DbSet<Medico> Medicos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Paciente>()
                .ToTable("paciente")
                .HasKey(p => p.id);

            modelBuilder.Entity<ConsultaMedica>()
                .ToTable("consulta_medica")
                .HasKey(c => c.id);

            modelBuilder.Entity<ConsultaMedica>()
                .HasOne(c => c.Paciente)
                .WithMany(p => p.ConsultasMedicas)
                .HasForeignKey(c => c.id_paciente)
                .HasConstraintName("fk_paciente");

            modelBuilder.Entity<ConsultaMedica>()
                .HasOne(c => c.Medico)
                .WithMany()
                .HasForeignKey(c => c.id_medico)
                .HasConstraintName("fk_medico");

            modelBuilder.Entity<Medico>()
                .ToTable("medico")
                .HasKey(m => m.id);
        }
    }

}