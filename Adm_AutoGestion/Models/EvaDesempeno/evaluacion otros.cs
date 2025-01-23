using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Razor.Text;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("evaluacion otros")]
    public class evaluacion_otros
    {
        [Key]
        public int Id { get; set; }
        //[Required]
        //[Key, Column("codigo evaluacion", Order = 1), DatabaseGenerated(DatabaseGeneratedOption.None)]
       
        [Column("codigo evaluacion")]
        public int  codigoevaluacion { get; set; }
        [Required]
        public string fortalezas { get; set; }
        [Required]
        public string aspectos { get; set; }
        public string recomendacion { get; set; }
        public string porque { get; set; }
        public string Autoevaluacion { get; set; }
        [NotMapped]
        public int CodEmp { get; set; }
    }
}