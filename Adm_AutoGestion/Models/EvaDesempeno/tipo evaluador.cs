using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("tipo evaluador")]
    public class tipo_evaluador
    {
        [Key]
        public int Id { get; set; }
        public int codigo { get; set; }
        [Required]
        [StringLength(50)]
        public string tipoevaluador { get; set; }
    }
}