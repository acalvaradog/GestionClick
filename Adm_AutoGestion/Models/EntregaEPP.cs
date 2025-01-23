using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Adm_AutoGestion.Models
{
    public class EntregaEPP
    {

        public int Id { get; set; }
        [Required]
        public int EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public Empleado Empleado { get; set; }
        public DateTime Fecha { get; set; }
        [Required]
        public String  Area { get; set; }
        [StringLength(20)]
        public String Estado { get; set; }
        [Required]
        public String Sociedad { get; set; }
        [NotMapped]
        public String cantifirmados { get; set; }
        [NotMapped]
        public Sociedad Sociedades { get; set; }

    }
}