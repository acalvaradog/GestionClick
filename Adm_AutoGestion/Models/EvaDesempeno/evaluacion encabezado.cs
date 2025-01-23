using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("evaluacion encabezado")]
    public class evaluacion_encabezado
    {
        
        public int codigo { get; set; }
        [Column("codigo empleado")]
        public int codigoempleado { get; set; }
        [Column("codigo evaluador")]
        public int codigoevaluador { get; set; }
        [Column("fecha registro")]
        public DateTime fecharegistro { get; set; }
        [Column("hora registro")]
        public TimeSpan horaregistro { get; set; }
        [Column("fecha evaluacion")]
        public DateTime fechaevaluacion { get; set; }
        [Column("usuario registra")]
        public int usuarioregistra { get; set; }
        [Key]
        public int Id { get; set; }
    }
}