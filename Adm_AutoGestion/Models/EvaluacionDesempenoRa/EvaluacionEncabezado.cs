using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaluacionDesempenoRa
{
    public class EvaluacionEncabezado
    {
        public int Id { get; set; }
        [ForeignKey("Empleado")]
        public int EmpleadoId { get; set; }
        public Empleado Empleado { get; set; }
        public int EvaluadorId { get; set; }
        public string PeriodoEvaluacion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Retroalimentacion { get; set; }
        public string PlandeMejora { get; set; }
        public float PuntajeFinal { get; set; }

    }
}