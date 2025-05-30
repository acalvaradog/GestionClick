using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class PeriodoVacacionesEmpleado
    {

        public int Id { get; set; }

        public int EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public Empleado Empleado { get; set; }

        public DateTime? FechaIngresoReal { get; set; }
        [Required]
        public DateTime FechaIngreso { get; set; }
        [Required]
        public DateTime PeriodoInicio { get; set; }
        [Required]
        public DateTime PeriodoFin { get; set; }
        [Required]
        public int Dias { get; set; }

        public int DiasporDisfrutar { get; set; }

        // Auditoria
        public DateTime FechaRegistro { get; set; }
        public int EmpleadoIdRegistra  { get; set; }

        public DateTime? FechaModifica { get; set; }
        public int? EmpleadoModifica { get; set; }
    }
}


