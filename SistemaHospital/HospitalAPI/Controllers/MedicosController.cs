using HospitalAPI.Data;
using HospitalAPI.Models;
using HospitalAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MedicosController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public MedicosController(HospitalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Medico>>> GetMedicos()
        {
            return await _context.Medicos
                .Include(m => m.CentroMedico)
                    .ThenInclude(cm => cm.Ciudad)
                .Include(m => m.Especialidad)
                .AsNoTracking()
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Medico>> GetMedico(int id)
        {
            var medico = await _context.Medicos
                .Include(m => m.CentroMedico)
                    .ThenInclude(cm => cm.Ciudad)
                .Include(m => m.Especialidad)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.id == id);

            return medico == null ? NotFound() : medico;
        }

        [HttpPost]
        public async Task<ActionResult<Medico>> PostMedico(MedicoCreateDto createDto)
        {
            if (await _context.Medicos.AnyAsync(m => m.correo == createDto.correo))
            {
                return BadRequest("El correo ya existe.");
            }

            var validationError = await ValidateForeignKeys(createDto.id_especialidad, createDto.id_centro_medico);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            var medico = new Medico
            {
                nombre = createDto.nombre,
                apellido = createDto.apellido,
                correo = createDto.correo,
                id_especialidad = createDto.id_especialidad,
                id_centro_medico = createDto.id_centro_medico,
                rol = createDto.rol
            };

            _context.Medicos.Add(medico);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMedico), new { id = medico.id }, medico);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedico(int id, MedicoUpdateDto updateDto)
        {
            var medico = await _context.Medicos.FindAsync(id);
            if (medico == null)
            {
                return NotFound();
            }

            if (updateDto.correo != null && await _context.Medicos.AnyAsync(m => m.correo == updateDto.correo && m.id != id))
            {
                return BadRequest("El correo ya existe en otro médico.");
            }

            var validationError = await ValidateForeignKeys(updateDto.id_especialidad, updateDto.id_centro_medico);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            medico.nombre = updateDto.nombre ?? medico.nombre;
            medico.apellido = updateDto.apellido ?? medico.apellido;
            medico.correo = updateDto.correo ?? medico.correo;
            medico.id_especialidad = updateDto.id_especialidad ?? medico.id_especialidad;
            medico.id_centro_medico = updateDto.id_centro_medico ?? medico.id_centro_medico;
            medico.rol = updateDto.rol ?? medico.rol;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedico(int id)
        {
            var medico = await _context.Medicos.FindAsync(id);
            if (medico == null)
            {
                return NotFound();
            }

            _context.Medicos.Remove(medico);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<string?> ValidateForeignKeys(int? idEspecialidad, int? idCentroMedico)
        {
            if (idEspecialidad.HasValue && !await _context.Especialidades.AnyAsync(e => e.id == idEspecialidad))
            {
                return "El ID de la especialidad no existe.";
            }
            if (idCentroMedico.HasValue && !await _context.CentrosMedicos.AnyAsync(c => c.id == idCentroMedico))
            {
                return "El ID del centro médico no existe.";
            }
            return null;
        }
    }
}