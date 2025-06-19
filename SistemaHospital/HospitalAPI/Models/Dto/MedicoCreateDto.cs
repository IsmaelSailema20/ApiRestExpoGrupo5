using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.Models.Dto
{
    public class MedicoCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        public string nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres.")]
        public string apellido { get; set; } = null!;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no es válido.")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")]
        public string correo { get; set; } = null!;
        [Required(ErrorMessage = "El ID de la especialidad es obligatorio.")]
        public int id_especialidad { get; set; }

        [Required(ErrorMessage = "El ID del centro médico es obligatorio.")]
        public int id_centro_medico { get; set; }

        [StringLength(50, ErrorMessage = "El rol no puede exceder los 50 caracteres.")]
        public string? rol { get; set; }
    }
}
