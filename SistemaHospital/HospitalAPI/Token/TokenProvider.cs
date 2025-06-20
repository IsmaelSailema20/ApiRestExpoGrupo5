using HospitalAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Security.Claims;
using System.Text;

namespace HospitalAPI.Token
{
    public class TokenProvider
    {
        private readonly IConfiguration _settings;

        public TokenProvider(IConfiguration settings)
        {
            _settings = settings;
        }

        public string GenerateToken(Usuario usuario)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim("Rol", usuario.Rol),
            new Claim("ciudad",usuario.Medico!=null?usuario.Medico.CentroMedico.Ciudad.nombre:usuario.Empleado.CentroMedico.Ciudad.nombre),
            
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
            claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.GetValue<int>("JwtSettings:ExpirationMinutes")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
