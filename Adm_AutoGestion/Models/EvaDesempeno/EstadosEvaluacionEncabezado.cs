using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("EstadosEvaluacionEncabezado")]
    public class EstadosEvaluacionEncabezado
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int Codigo { get; set; }
    }
}