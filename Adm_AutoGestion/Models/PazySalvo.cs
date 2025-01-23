using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class PazySalvo
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        [Required]
        public int EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public Empleado Empleado { get; set; }
        public string Estado { get; set; }
        public int RetiroId { get; set; }
        public string Empresa { get; set; }
        [NotMapped]
        public virtual List<string> ListadoAreasPazySalvo { get; set; }
    }
}