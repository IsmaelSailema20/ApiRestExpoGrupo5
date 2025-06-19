namespace HospitalAPI.Models
{
    public class ConsultaMedica
    {
        public int id { get; set; }
        public DateTime? fecha { get; set; }
        public string? motivo { get; set; }
        public string? diagnostico { get; set; }
        public string? tratamiento { get; set; }
        public int? id_medico { get; set; }
        public int? id_paciente { get; set; }
        public Medico? Medico { get; set; }
        public Paciente? Paciente { get; set; }
    }
}
