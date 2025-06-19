using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.Models.Dto
{
    public class CentroMedicoCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string nombre { get; set; } = null!;

        [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres.")]
        public string? direccion { get; set; }

        [Required(ErrorMessage = "El ID de la ciudad es obligatorio.")]
        public int ciudad_id { get; set; }
    }
}
