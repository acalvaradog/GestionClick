using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Models
{
    public class Encuesta
    {
        public int Id { get; set; }
        [Required]
        public int NumeroPregunta { get; set; }
        [ForeignKey("NumeroPregunta")]
        public Preguntas Preguntas { get; set; }
        [Required]
        public string Respuesta { get; set; }
        public int EncabezadoEncuesta_Id { get; set; }
        [ForeignKey("EncabezadoEncuesta_Id")]
        public EncabezadoEncuesta EncabezadoEncuesta { get; set; }
        //public virtual EncabezadoEncuesta EncabezadoEncuestas { get; set; }
        [NotMapped]
        public virtual List<Preguntas> ListadoPreguntas { get; set; }
        [NotMapped]
        public virtual List<SelectListItem> Opciones { get; set; }
    }
}