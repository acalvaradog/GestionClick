using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Retiros
    {
        public int Id { get; set; }
        [Required]
        [StringLength(8)]
        public string CodigoEmpleado { get; set; }
        [Required]
        public string AreaPersonal { get; set; }
        [Required]
        public int MotivoCancelacion { get; set; }
        [ForeignKey("MotivoCancelacion")]
        public Motivos Motivos { get; set; }
        [Required]
        public string TipoContrato { get; set; }
        [Required]
        public DateTime InicioContrato { get; set; }
        [Required]
        public DateTime FechaTerminacion { get; set; }
        [StringLength(200)]
        public string Observacion { get; set; }
        [Required]
        public string Liquidacion { get; set; }
        [Required]
        public int UsuarioRegistra { get; set; }
        public int? UsuarioModifica { get; set; }
        public DateTime? FechaModifica { get; set; }
        [Required]
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
        public string EnvioEncuesta { get; set; }
        public string RespuestaEncuesta { get; set; }
        [NotMapped]
        public virtual List<Motivos> ListadoMotivos { get; set; }
        public int IdEmpleado { get; set; }
        [ForeignKey("IdEmpleado")]
        public Empleado Empleado { get; set; }
        public string Empresa { get; set; }
        [NotMapped]
        public Empleado EmpleadoRegistra { get; set; }
        [NotMapped]
        public Empleado EmpleadoModif { get; set; }
    }
}