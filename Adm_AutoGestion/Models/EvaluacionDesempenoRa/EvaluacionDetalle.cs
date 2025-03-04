using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaluacionDesempenoRa
{
    public class EvaluacionDetalle
    {
        public int Id { get; set; }
        public int EvaluacionId { get; set; }
        [ForeignKey("EvaluacionId")]
        public EvaluacionEncabezado EvaluacionEncabezado { get; set; }

        public int IndicadorId { get; set; }
        [ForeignKey("IndicadorId")]
        public EvaluacionIndicador EvaluacionIndicador { get; set; }

        public int? BaseNumerador { get; set; }

        public int? BaseDenominador { get; set; }

        public int? IndicadorNumerador { get; set; }

        public int? IndicadorDenominador { get; set; }

        public float Porcentaje { get; set; }
    }
}