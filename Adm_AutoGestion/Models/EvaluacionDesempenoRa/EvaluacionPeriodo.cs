using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaluacionDesempenoRa
{
    public class EvaluacionPeriodo
    {
        public int Id { get; set; }

        public string Periodo { get; set; }

        public bool Estado { get; set; }
    }
}