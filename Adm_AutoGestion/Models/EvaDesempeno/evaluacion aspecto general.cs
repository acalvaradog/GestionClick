using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("evaluacion aspecto general")]
    public class evaluacion_aspecto_general
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column("codigo evaluacion")]
        public int codigoevaluacion { get; set; }
        [Required]
        [Column("codigo aspecto general")]
        public int codigoaspectogeneral { get; set; }
        [Required]
        [Column("codigo calificacion")]
        public int codigocalificacion { get; set; }
        [Required]
        public string observacion { get; set; }
    }
}