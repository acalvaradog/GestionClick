using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class HistorialIncapacidades
    {
        public int Id { get; set; }
        [Required]
        public int IncapacidadId { get; set; }
        [ForeignKey("IncapacidadId")]
        public Incapacidades Incapacidades { get; set; }
        [Required]
        public DateTime Fecha { get; set; }
        [Required]
        public string Accion { get; set; }
        [Required]
        public int EmpleadoId { get; set; }
        public string Observaciones { get; set; }
    }
}