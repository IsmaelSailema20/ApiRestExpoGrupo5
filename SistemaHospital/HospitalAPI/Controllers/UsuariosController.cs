using HospitalAPI.Data;
using HospitalAPI.Models;
using HospitalAPI.Models.Dto;
using HospitalAPI.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HospitalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {

        private readonly HospitalDbContext _context;
        private readonly IConfiguration _configuration;

        public UsuariosController(HospitalDbContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }

        // GET: api/<UsuariosController>
        [HttpGet]
        [Authorize]
        public async Task<List<Usuario>> Get()
        {
            return await _context.Usuarios
                .Include(u => u.Medico)
                    .ThenInclude(m => m.CentroMedico)
                        .ThenInclude(c => c.Ciudad)
                .Include(u => u.Empleado)
                    .ThenInclude(e => e.CentroMedico)
                        .ThenInclude(c => c.Ciudad)
                .ToListAsync();
        }

        // GET api/<UsuariosController>/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Usuario>> Get(int id)
        {
            Usuario u= await _context.Usuarios
                .Include(u => u.Medico)
                    .ThenInclude(m => m.CentroMedico)
                        .ThenInclude(c => c.Ciudad)
                .Include(u => u.Empleado)
                    .ThenInclude(e => e.CentroMedico)
                        .ThenInclude(c => c.Ciudad)
                .FirstOrDefaultAsync(u=>u.Id==id);

            return u == null ? NotFound() : u;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            Usuario u =await _context.Usuarios
            .Include(u => u.Medico)
                .ThenInclude(m => m.CentroMedico)
                    .ThenInclude(c => c.Ciudad)
            .Include(u => u.Empleado)
                .ThenInclude(e => e.CentroMedico)
                    .ThenInclude(c => c.Ciudad)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (u.Password==request.Password)
            {
                TokenProvider token = new TokenProvider(_configuration);
                return Ok(new { token = token.GenerateToken(u)});
            }
            return Unauthorized(new { message = "Credenciales incorrectas" });

        }

    }
}
