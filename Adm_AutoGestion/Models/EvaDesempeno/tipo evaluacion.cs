using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("tipo evaluacion")]
    public class tipo_evaluacion
    {
        [Key]
        public int codigo { get; set; }

        [StringLength(100)]
        public string tipo { get; set; }
    }
}