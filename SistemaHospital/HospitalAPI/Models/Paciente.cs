namespace HospitalAPI.Models
{
    public class Paciente
    {
        public int id { get; set; }
        public string? nombre { get; set; }
        public string? apellido { get; set; }
        public string? cedula { get; set; }
        public DateTime? fecha_nacimiento { get; set; }
        public List<ConsultaMedica>? ConsultasMedicas { get; set; }
    }
}
