using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class HabilitarVacaciones
    {
        public int Id { get; set; }
        [Required]
        public int EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public Empleado Empleado { get; set; }
        [Required]
        public string DiasMayor6 { get; set; }
        [Required]
        public string Anticipadas { get; set; }
        [Required]
        public string Pagadas { get; set; }
        [Required]
        public string UsuarioRegistra { get; set; }
        [NotMapped]
        public Empleado Empleado2 { get; set; }
    }
}