using HospitalAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Services
{
    public class PacienteDbContextFactory
    {
        private readonly IConfiguration _configuration;

        public PacienteDbContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public PacienteDbContext CreateDbContext(string replica)
        {
            var connectionString = replica.ToLower() switch
            {
                "guayaquil" => _configuration.GetConnectionString("GuayaquilDb"),
                "cuenca" => _configuration.GetConnectionString("CuencaDb"),
                _ => throw new ArgumentException("Réplica no válida. Use 'guayaquil' o 'cuenca'.")
            };

            var optionsBuilder = new DbContextOptionsBuilder<PacienteDbContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new PacienteDbContext(optionsBuilder.Options);
        }
    }
}
