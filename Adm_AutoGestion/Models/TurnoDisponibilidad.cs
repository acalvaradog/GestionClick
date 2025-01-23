using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class TurnoDisponibilidad
    {
        public int Id { get; set; }
        [Required]
        public int TrabajadorId { get; set; }
        [ForeignKey("TrabajadorId")]
        public Empleado Empleado { get; set; }
        [Required]
        public DateTime FechaRegistro { get; set; }
        [Required]
        public DateTime FechaInicio { get; set; }
        [Required]
        public DateTime FechaFin { get; set; }
        [Required]
        public string HoraInicio { get; set; }
        [Required]
        public string HoraFin { get; set; }
        [Required]
        public string Extras { get; set; }
        [Required]
        public string CantExtras { get; set; }
        public string Liquidado { get; set; }
        [Required]
        public string Estado { get; set; }
        public string Empresa { get; set; }
        //[NotMapped]
        //public Empleado Empleado { get; set; }
    }
}