namespace HospitalAPI.Models.Dto
{
    public class PacienteUpdateDto
    {
        public string? nombre { get; set; }
        public string? apellido { get; set; }
        public string? cedula { get; set; }
        public DateTime? fecha_nacimiento { get; set; }
    }
}
