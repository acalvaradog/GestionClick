using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Preguntas
    {
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Pregunta { get; set; }
        public Boolean Activo { get; set; }
        public string Grupo { get; set; }
    }
}