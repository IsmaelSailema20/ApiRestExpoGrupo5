using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.Models.Dto;
using System.ComponentModel.DataAnnotations;

public class PacienteCreateDto
{

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
    public string nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres.")]
    public string apellido { get; set; } = null!;

    [Required(ErrorMessage = "La cédula es obligatoria.")]
    [StringLength(10, ErrorMessage = "La cédula debe tener 10 caracteres.", MinimumLength = 10)]
    public string cedula { get; set; } = null!;

    public DateTime? fecha_nacimiento { get; set; }
}
