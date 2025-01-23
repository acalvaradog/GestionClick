using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Expositor
    {
        public int Id { get; set; }

        public string EmpleadoId { get; set; }
        public string TipoExpositor { get; set; }
        public string TerceroId { get; set; }
        
        
        public int CapacitacionId { get; set; }
        [ForeignKey("CapacitacionId")]
        public Capacitacion Capacitacion { get; set; }
        [NotMapped]
        public virtual Empleado Empleado { get; set; }
        [NotMapped]
        public virtual Tercero Tercero { get; set; }
    }
}