using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("periodos")]
    public class periodos
    {
        [Key]
        public int codigo { get; set; }
        [Column("fecha inicio")]
        public DateTime fechaincio { get; set; }
        [Column("fecha final")]
        public DateTime fechafinal { get; set; }
        public int sociedad { get; set; }
        public int TipoPeriodo { get; set; }
        public int? Parametro_Id { get; set; }
    }   
}