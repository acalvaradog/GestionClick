using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaluacionDesempenoRa
{
    public class EvaluacionEncabezado
    {
        public int Id { get; set; }
        public int EmpleadoId { get; set; }
        public int EvaluadorId { get; set; }
        public string PeriodoEvaluacion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Retroalimentacion { get; set; }
        public string PlandeMejora { get; set; }
        public float PuntajeFinal { get; set; }

        public List<EvaluacionDetalle> EvaluacionDetalle { get; set; }
    }
}