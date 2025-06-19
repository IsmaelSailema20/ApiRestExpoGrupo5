namespace HospitalAPI.Models.Dto
{
    public class MedicoUpdateDto
    {
        
        public string? nombre { get; set; }
        public string? apellido { get; set; }
        public string? correo { get; set; }
        public int? id_especialidad { get; set; }
        public int? id_centro_medico { get; set; }
        public string? rol { get; set; }
    
}
}
