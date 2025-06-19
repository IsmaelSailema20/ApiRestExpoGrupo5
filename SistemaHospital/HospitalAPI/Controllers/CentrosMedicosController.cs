using HospitalAPI.Data;
using HospitalAPI.Models;
using HospitalAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CentrosMedicosController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public CentrosMedicosController(HospitalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CentroMedico>>> GetCentrosMedicos()
        {
            return await _context.CentrosMedicos
                .Include(c => c.Ciudad)
                .AsNoTracking()
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CentroMedico>> GetCentroMedico(int id)
        {
            var centroMedico = await _context.CentrosMedicos
                .Include(c => c.Ciudad)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.id == id);

            return centroMedico == null ? NotFound() : centroMedico;
        }

        [HttpPost]
        public async Task<ActionResult<CentroMedico>> PostCentroMedico(CentroMedicoCreateDto createDto)
        {
            if (!await _context.Ciudades.AnyAsync(c => c.id == createDto.ciudad_id))
            {
                return BadRequest("El ID de la ciudad no existe.");
            }

            var centroMedico = new CentroMedico
            {
                nombre = createDto.nombre,
                direccion = createDto.direccion,
                ciudad_id = createDto.ciudad_id
            };

            _context.CentrosMedicos.Add(centroMedico);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCentroMedico), new { id = centroMedico.id }, centroMedico);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCentroMedico(int id, CentroMedicoUpdateDto updateDto)
        {
            var centroMedico = await _context.CentrosMedicos.FindAsync(id);
            if (centroMedico == null)
            {
                return NotFound();
            }

            // Validar ciudad_id solo si se envía
            if (updateDto.ciudad_id!=null)
            {
                if (!await _context.Ciudades.AnyAsync(c => c.id == updateDto.ciudad_id.Value))
                {
                    return BadRequest("El ID de la ciudad no existe.");
                }
                centroMedico.ciudad_id = updateDto.ciudad_id.Value;
            }

            centroMedico.nombre = updateDto.nombre ?? centroMedico.nombre;
            centroMedico.direccion = updateDto.direccion ?? centroMedico.direccion;

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
        public async Task<IActionResult> DeleteCentroMedico(int id)
        {
            var centroMedico = await _context.CentrosMedicos.FindAsync(id);
            if (centroMedico == null)
            {
                return NotFound();
            }

            _context.CentrosMedicos.Remove(centroMedico);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}