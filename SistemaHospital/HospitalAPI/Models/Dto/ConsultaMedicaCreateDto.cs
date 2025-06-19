using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.Models.Dto
{
    public class ConsultaMedicaCreateDto
    {
        [Required(ErrorMessage = "La fecha es obligatoria.")]
        public DateTime fecha { get; set; }

        [StringLength(500, ErrorMessage = "El motivo no puede exceder los 500 caracteres.")]
        public string? motivo { get; set; }

        [StringLength(500, ErrorMessage = "El diagnóstico no puede exceder los 500 caracteres.")]
        public string? diagnostico { get; set; }

        [StringLength(500, ErrorMessage = "El tratamiento no puede exceder los 500 caracteres.")]
        public string? tratamiento { get; set; }
    }
}
