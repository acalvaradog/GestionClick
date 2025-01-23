using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Models
{
    public class EncabezadoEncuesta
    {
        public int Id { get; set; }
        [Required]
        public int EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public Empleado Empleado { get; set; }
        [Required]
        [StringLength(200)]
        public string Cargo { get; set; }
        [Required]
        [StringLength(200)]
        public string UnidadOrganizativa { get; set; }
        [Required]
        [StringLength(50)]
        public string Eps { get; set; }
        [Required]
        [StringLength(50)]
        public string Transporte { get; set; }
        [Required]
        [StringLength(50)]
        public string ModoTrabajo { get; set; }
        public string Sospechoso { get; set; }
        public string Empresa { get; set; }
        public DateTime Fecha { get; set; }
        public string Temperatura { get; set; }
        public string Cerco { get; set; }
        [NotMapped]
        public virtual List<Encuesta> Encuesta { get; set; }
        [NotMapped]
        public virtual List<Empleado> ListadoEmpleado { get; set; }
        [NotMapped]
        public string sintomas { get; set; }
    }
}