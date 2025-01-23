using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("seguimientos")]
    public class seguimientos
    {
        [Key]
        public int codigo { get; set; }
        public DateTime fecha { get; set; }
        [Required]
        public string descripcion { get; set; }
        public string comentario { get; set; }
        [Column("proximo seguimiento")]
        public DateTime proximoseguimiento { get; set; }
        public bool? cumplimiento { get; set; }
        [Column("Aspectos generales")]
        public int? Aspectosgenerales { get; set; }
        [Column("Competencias institucionales")]
        public int? Competenciasinstitucionales { get; set; }
        [Column("Competencias conceptuales")]
        public int ?Competenciasconceptuales { get; set; }
        [Column("tipo evaluacion")]
        public int tipoevaluacion { get; set; }
        public int periodo { get; set; }
        [Column("codigo empleado")]
        public int codigoempleado { get; set; }
        [NotMapped]
        public string NombreEmp { get; set; }
        
    }
}