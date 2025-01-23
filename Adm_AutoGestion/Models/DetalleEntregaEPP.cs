using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Adm_AutoGestion.Models
{
    public class DetalleEntregaEPP
    {
        public int Id { get; set; }
        [Required]
        public int NumeroEntrega { get; set; }
        [ForeignKey("NumeroEntrega")]
        public EntregaEPP EntregaEPP { get; set; }
        public int EPP { get; set; }
        [ForeignKey("EPP")]
        public ElementosProtecionPersonal ElementosProtecionPersonal { get; set; }
        [Required]
        public int EmpleadoId { get; set; }
        [Required]
        public int Cantidad { get; set; }
        [Required]
        [StringLength(40)]
        public string MotivoEntrega { get; set; }
        [Required]
        public DateTime Fecha { get; set; }
        [StringLength(50)]
        public string FechaFirma { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Observacion { get; set; }
        [Required]
        [StringLength(20)]
        public string Estado { get; set; }
        public string Cargo { get; set; }
        public string Area { get; set; }
        [NotMapped]
        public virtual List<ElementosProtecionPersonal> ListadoEPP{ get; set; }
        [NotMapped]
        public virtual List<SelectList> ListadoElementos { get; set; }
        [NotMapped]
        public Empleado Empleado { get; set; }
        [NotMapped]
        public Empleado EmpleadoCA { get; set; }
        [NotMapped]
        public PersonalActivo Empleado1 { get; set; }
        [NotMapped]
        public Sociedad Sociedades { get; set; }
        [NotMapped]
        public Sociedad EmpleadoSoc { get; set; }
        [NotMapped]
        public Empleado Empleado2 { get; set; }
       
    }
}