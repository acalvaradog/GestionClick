using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("evaluacion conductuales")]
    public class evaluacion_conductuales
    {
        [Key]
        public int Id { get; set; }
        [Column("codigo evaluacion")]
        public int codigoevaluacion { get; set; }
        [Column("codigo competencia")]
        public int codigocompetencia { get; set; }
        [Column("codigo nivel")]
        public int codigonivel { get; set; }
        [Column("codigo calificacion")]
        public int codigocalificacion { get; set; }
        [Required]
        public string observacion { get; set; }
    }
}