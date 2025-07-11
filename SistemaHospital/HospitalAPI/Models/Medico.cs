﻿using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalAPI.Models
{
    public class Medico
    {
        public int id { get; set; }
        public string? nombre { get; set; }
        public string? apellido { get; set; }
        public string? correo { get; set; }
        public int? id_especialidad { get; set; }
        public int? id_centro_medico { get; set; }
        public string? rol { get; set; }

        [ForeignKey("id_centro_medico")]
        public CentroMedico? CentroMedico { get; set; }

        [ForeignKey("id_especialidad")]
        public Especialidad? Especialidad { get; set; }
    }
}
