using HospitalAPI.Data;
using Microsoft.EntityFrameworkCore;
namespace HospitalAPI.Services
{
    public class ReplicaSelectorService
    {
        private readonly HospitalDbContext _context;

        public ReplicaSelectorService(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetReplicaForMedico(int idMedico)
        {
            var medico = await _context.Medicos
                .Include(m => m.CentroMedico)
                .ThenInclude(cm => cm.Ciudad)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.id == idMedico);

            if (medico == null || medico.CentroMedico?.Ciudad == null)
            {
                throw new ArgumentException("Médico o ciudad no encontrada.");
            }

            return medico.CentroMedico.Ciudad.nombre.ToLower() switch
            {
                "guayaquil" => "guayaquil",
                "cuenca" => "cuenca",
                _ => throw new ArgumentException("Ciudad no válida para réplicas (solo Guayaquil o Cuenca).")
            };
        }
    }
}
