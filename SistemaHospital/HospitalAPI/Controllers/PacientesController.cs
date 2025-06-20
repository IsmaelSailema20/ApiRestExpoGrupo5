using HospitalAPI.Data;
using HospitalAPI.Models;
using HospitalAPI.Models.Dto;
using HospitalAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HospitalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "MedicoPolitica")]
    public class PacientesController : ControllerBase
    {
        private readonly PacienteDbContextFactory _contextFactory;

        public PacientesController(PacienteDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        private string? ObtenerCiudadDesdeToken()
        {
            var claimCiudad = User.Claims.FirstOrDefault(c => c.Type == "ciudad")?.Value?.ToLower();
            if (claimCiudad == "guayaquil" || claimCiudad == "cuenca")
                return claimCiudad;
            return null;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PacienteDto>>> GetPacientes()
        {
            var ciudad = ObtenerCiudadDesdeToken();
            if (ciudad == null)
                return Unauthorized("Ciudad no válida o no presente en el token.");

            using var context = _contextFactory.CreateDbContext(ciudad);
            var pacientes = await context.Pacientes.AsNoTracking().ToListAsync();

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
        public async Task<ActionResult<PacienteDto>> GetPaciente(int id)
        {
            var ciudad = ObtenerCiudadDesdeToken();
            if (ciudad == null)
                return Unauthorized("Ciudad no válida o no presente en el token.");

            using var context = _contextFactory.CreateDbContext(ciudad);
            var paciente = await context.Pacientes.AsNoTracking().FirstOrDefaultAsync(p => p.id == id);

            if (paciente == null)
                return NotFound();

            return new PacienteDto
            {
                id = paciente.id,
                nombre = paciente.nombre,
                apellido = paciente.apellido,
                cedula = paciente.cedula,
                fecha_nacimiento = paciente.fecha_nacimiento
            };
        }

        [HttpPost]
        public async Task<ActionResult<PacienteDto>> PostPaciente([FromBody] PacienteCreateDto request)
        {
            var ciudad = ObtenerCiudadDesdeToken();
            if (ciudad == null)
                return Unauthorized("Ciudad no válida o no presente en el token.");

            if (string.IsNullOrWhiteSpace(request.cedula))
                return BadRequest("La cédula es obligatoria.");

            foreach (var dbreplica in new[] { "guayaquil", "cuenca" })
            {
                using var dbContext = _contextFactory.CreateDbContext(dbreplica);
                if (await dbContext.Pacientes.AnyAsync(p => p.cedula == request.cedula))
                    return BadRequest("La cédula ya existe en una de las réplicas.");
            }

            var paciente = new Paciente
            {
                nombre = request.nombre,
                apellido = request.apellido,
                cedula = request.cedula,
                fecha_nacimiento = request.fecha_nacimiento
            };

            using var context = _contextFactory.CreateDbContext(ciudad);
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

            return CreatedAtAction(nameof(GetPaciente), new { id = paciente.id }, pacienteDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaciente(int id, [FromBody] PacienteUpdateDto paciente)
        {
            var ciudad = ObtenerCiudadDesdeToken();
            if (ciudad == null)
                return Unauthorized("Ciudad no válida o no presente en el token.");

            using var context = _contextFactory.CreateDbContext(ciudad);
            var pacienteDb = await context.Pacientes.FirstOrDefaultAsync(p => p.id == id);

            if (pacienteDb == null)
                return NotFound();

            if (paciente.cedula != null && await context.Pacientes.AnyAsync(p => p.cedula == paciente.cedula && p.id != id))
                return BadRequest("La cédula ya existe en otro paciente.");

            pacienteDb.nombre = paciente.nombre ?? pacienteDb.nombre;
            pacienteDb.apellido = paciente.apellido ?? pacienteDb.apellido;
            pacienteDb.cedula = paciente.cedula ?? pacienteDb.cedula;
            pacienteDb.fecha_nacimiento = paciente.fecha_nacimiento ?? pacienteDb.fecha_nacimiento;

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaciente(int id)
        {
            var ciudad = ObtenerCiudadDesdeToken();
            if (ciudad == null)
                return Unauthorized("Ciudad no válida o no presente en el token.");

            using var context = _contextFactory.CreateDbContext(ciudad);
            var paciente = await context.Pacientes.FirstOrDefaultAsync(p => p.id == id);

            if (paciente == null)
                return NotFound();

            context.Pacientes.Remove(paciente);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
