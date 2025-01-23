using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class DetalleCapacitacion
    {
        public int Id { get; set; }
        public int EmpleadoId { get; set; }
        [StringLength(50)]
        public string FechaFirma { get; set; }
        public string Estado { get; set; }
        public string Cargo { get; set; }
        public string Area { get; set; }
        public string EsTercero { get; set; }
        public string TerceroId { get; set; }
        public int CapacitacionId { get; set; }
        [ForeignKey("CapacitacionId")]
        public Capacitacion Capacitacion { get; set; }
        public int SedeId { get; set; }
        public string EnvioEncuesta { get; set; }
        public string RespuestaEncuesta { get; set; }
        [NotMapped]
        public Empleado Empleado2 { get; set; }
        [NotMapped]
        public Sede Sede2 { get; set; }
        [NotMapped]
        public string Sociedad2 { get; set; }
        [NotMapped]
        public Empresas Empresa2 { get; set; }
        [NotMapped]
        public string NombreEmpresaTercera { get; set; }
        [NotMapped]
        public Tercero Tercero2 { get; set; }
        [NotMapped]
        public string Universidad { get; set; }

    }
}