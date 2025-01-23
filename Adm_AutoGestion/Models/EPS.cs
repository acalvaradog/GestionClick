using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class EPS
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public int EstadoId { get; set; }
        [ForeignKey("EstadoId")]
        public EstadosIncapacidades EstadosIncapacidades { get; set; }
    }
}