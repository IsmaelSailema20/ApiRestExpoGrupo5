namespace HospitalAPI.Models
{
    public class CentroMedico
    {
        public int id { get; set; }
        public string? nombre { get; set; }
        public string? direccion { get; set; }
        public int? ciudad_id { get; set; }
        public Ciudad? Ciudad { get; set; }
    }
}
