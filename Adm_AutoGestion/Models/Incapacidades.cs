using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Incapacidades
    {
        public int Id { get; set; }
        [Required]
        public int EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public Empleado Empleado { get; set; }
        [Required]
        public DateTime Fecha { get; set; }
        [Required]
        public DateTime FechaInicio { get; set; }
        [Required]
        public DateTime FechaFin { get; set; }
        [Required]
        public string CantidadDias { get; set; }
        [Required]
        public string Diagnostico { get; set; }
        public int EstadoId { get; set; }
        [ForeignKey("EstadoId")]
        public EstadosIncapacidades EstadosIncapacidades{ get; set; }
        public string Prorroga { get; set; }
        [Required]
        public string EPS { get; set; }
        public string Empresa { get; set; }
        [NotMapped]
        public string Estado { get; set; }
        [NotMapped]
        public virtual List<TiposIncapacidad> ListadoTiposInc { get; set; }
        [NotMapped]
        public virtual List<IncapacidadAdjuntos> ListadoAdjuntos { get; set; }
        [NotMapped]
        public PersonalActivo PersonalActivo { get; set; }
        [NotMapped]
        public EPS ListadoEps { get; set; }
        
    }
}