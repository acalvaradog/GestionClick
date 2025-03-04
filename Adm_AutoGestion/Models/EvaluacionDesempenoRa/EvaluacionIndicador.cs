using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaluacionDesempenoRa
{
    public class EvaluacionIndicador
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Evidencia { get; set; }
        public string UnidadOrganizativa { get; set; }
        public int CriterioId { get; set; }
        [ForeignKey("CriterioId")]
        public EvaluacionCriterio EvaluacionCriterio { get; set; }
    }
}