namespace HospitalAPI.Models
{
    public class Empleado
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Cargo { get; set; }

        public int id_centro_medico { get; set; }
        public CentroMedico CentroMedico { get; set; }

        public string Rol { get; set; }
    }
}
