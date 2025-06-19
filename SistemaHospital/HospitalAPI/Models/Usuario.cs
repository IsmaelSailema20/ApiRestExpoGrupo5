namespace HospitalAPI.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }

        public int? id_medico { get; set; }
        public Medico? Medico { get; set; }

        public int? id_empleado { get; set; }
        public Empleado? Empleado { get; set; }

    }
}
