using HospitalAPI.Data;
using HospitalAPI.Models;
using HospitalAPI.Models.Dto;
using HospitalAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PacientesController : ControllerBase
    {
        private readonly PacienteDbContextFactory _contextFactory;
        private readonly ReplicaSelectorService _replicaSelector;

        public PacientesController(PacienteDbContextFactory contextFactory, ReplicaSelectorService replicaSelector)
        {
            _contextFactory = contextFactory;
            _replicaSelector = replicaSelector;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PacienteDto>>> GetPacientes([FromQuery] int? idMedico)
        {
            if (!idMedico.HasValue)
            {
                var pacientesDtoget = new List<PacienteDto>();
                foreach (var dbreplica in new[] { "guayaquil", "cuenca" })
                {
                    using var context = _contextFactory.CreateDbContext(dbreplica);
                    var pacientesr = await context.Pacientes
                        .AsNoTracking()
                        .ToListAsync();
                    pacientesDtoget.AddRange(pacientesr.Select(p => new PacienteDto
                    {
                        id = p.id,
                        nombre = p.nombre,
                        apellido = p.apellido,
                        cedula = p.cedula,
                        fecha_nacimiento = p.fecha_nacimiento
                    }));
                }
                return pacientesDtoget;
            }

            var replica = await _replicaSelector.GetReplicaForMedico(idMedico.Value);
            if (replica != "guayaquil" && replica != "cuenca")
            {
                return BadRequest("El médico no está asociado a una réplica válida para pacientes.");
            }

            using var dbContext = _contextFactory.CreateDbContext(replica);
            var pacientes = await dbContext.Pacientes
                .AsNoTracking()
                .ToListAsync();

            var pacientesDto = pacientes.Select(p => new PacienteDto
            {
                id = p.id,
                nombre = p.nombre,
                apellido = p.apellido,
                cedula = p.cedula,
                fecha_nacimiento = p.fecha_nacimiento
            }).ToList();

            return pacientesDto;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PacienteDto>> GetPaciente(int id, [FromQuery] int idMedico)
        {
            var replica = await _replicaSelector.GetReplicaForMedico(idMedico);
            if (replica != "guayaquil" && replica != "cuenca")
            {
                return BadRequest("El médico no está asociado a una réplica válida para pacientes.");
            }

            using var context = _contextFactory.CreateDbContext(replica);
            var paciente = await context.Pacientes
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.id == id);

            if (paciente == null)
            {
                return NotFound();
            }

            var pacienteDto = new PacienteDto
            {
                id = paciente.id,
                nombre = paciente.nombre,
                apellido = paciente.apellido,
                cedula = paciente.cedula,
                fecha_nacimiento = paciente.fecha_nacimiento
            };

            return pacienteDto;
        }

        [HttpPost]
        public async Task<ActionResult<PacienteDto>> PostPaciente([FromBody] PacienteCreateDto request, [FromQuery] int idMedico)
        {
            if (idMedico == 0)
            {
                return BadRequest("El parámetro idMedico es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(request.cedula))
            {
                return BadRequest("La cédula es obligatoria.");
            }

            foreach (var dbreplica in new[] { "guayaquil", "cuenca" })
            {
                using var dbContext = _contextFactory.CreateDbContext(dbreplica);
                if (await dbContext.Pacientes.AnyAsync(p => p.cedula == request.cedula))
                {
                    return BadRequest("La cédula ya existe en una de las réplicas.");
                }
            }

            var paciente = new Paciente
            {
                nombre = request.nombre,
                apellido = request.apellido,
                cedula = request.cedula,
                fecha_nacimiento = request.fecha_nacimiento
            };

            var replica = await _replicaSelector.GetReplicaForMedico(idMedico);
            if (replica != "guayaquil" && replica != "cuenca")
            {
                return BadRequest("El médico no está asociado a una réplica válida para pacientes.");
            }

            using var context = _contextFactory.CreateDbContext(replica);
            context.Pacientes.Add(paciente);
            await context.SaveChangesAsync();

            var pacienteDto = new PacienteDto
            {
                id = paciente.id,
                nombre = paciente.nombre,
                apellido = paciente.apellido,
                cedula = paciente.cedula,
                fecha_nacimiento = paciente.fecha_nacimiento
            };

            return CreatedAtAction(nameof(GetPaciente), new { id = paciente.id, idMedico }, pacienteDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaciente(int id, [FromBody] PacienteUpdateDto paciente, [FromQuery] int idMedico)
        {
            var replica = await _replicaSelector.GetReplicaForMedico(idMedico);
            if (replica != "guayaquil" && replica != "cuenca")
            {
                return BadRequest("El médico no está asociado a una réplica válida para pacientes.");
            }

            using var context = _contextFactory.CreateDbContext(replica);
            var pacienteDb = await context.Pacientes
                .FirstOrDefaultAsync(p => p.id == id);

            if (pacienteDb == null)
            {
                return NotFound();
            }

            if (paciente.cedula != null && await context.Pacientes.AnyAsync(p => p.cedula == paciente.cedula && p.id != id))
            {
                return BadRequest("La cédula ya existe en otro paciente.");
            }

            pacienteDb.nombre = paciente.nombre ?? pacienteDb.nombre;
            pacienteDb.apellido = paciente.apellido ?? pacienteDb.apellido;
            pacienteDb.cedula = paciente.cedula ?? pacienteDb.cedula;
            pacienteDb.fecha_nacimiento = paciente.fecha_nacimiento ?? pacienteDb.fecha_nacimiento;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaciente(int id, [FromQuery] int idMedico)
        {
            var replica = await _replicaSelector.GetReplicaForMedico(idMedico);
            if (replica != "guayaquil" && replica != "cuenca")
            {
                return BadRequest("El médico no está asociado a una réplica válida para pacientes.");
            }

            using var context = _contextFactory.CreateDbContext(replica);
            var paciente = await context.Pacientes
                .FirstOrDefaultAsync(p => p.id == id);

            if (paciente == null)
            {
                return NotFound();
            }

            context.Pacientes.Remove(paciente);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}