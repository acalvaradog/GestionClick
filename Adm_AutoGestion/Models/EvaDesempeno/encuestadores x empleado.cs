using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("encuestadores x empleado")]
    public class encuestadores_x_empleado
    {
        [Key]
        public int codigo { get; set; }
        [Column("codigo empleado")]
        public int codigoempleado { get; set; }
        [Column("tipo evaluador")]
        public int tipoevaluador { get; set; }
        [Column("codigo evaluador")]
        public int codigoevaluador { get; set; }
        [Column("fecha registro")]
        public DateTime fecharegistro { get; set; }
        [Column("hora registro")]
        public TimeSpan horaregistro { get; set; }

        [Column("usuario registro")]
        public int usuarioregistro { get; set; }
        public int periodo { get; set; }

        [Column("tipo evaluacion")]
        public int tipoevaluacion { get; set; }

        [Column("RetroalimentacionEmp")]
        public bool RetroalimentacionEmp { get; set; }

        [Column("RetroalimentacionJefe")]
        public bool RetroalimentacionJefe { get; set; }

        [Column("Estado")]
        public int Estado { get; set; }
        [Column("FechaRTA_Jefe")]
        public DateTime? FechaRTA_Jefe { get; set; }
        [Column("FechaRTA_Emp")]
        public DateTime? FechaRTA_Emp { get; set; }
    }
}