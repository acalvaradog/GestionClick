using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class SeguimientoSintomas
    {

        public int Id { get; set; }
        public string Plan { get; set; }
        public string Observacion { get; set; }
        public DateTime Fecha { get; set; }
        public int EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public Empleado Empleado { get; set; }
        public int EncabezadoEncuestaId { get; set; }

    }
}