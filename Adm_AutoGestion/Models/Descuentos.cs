using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Descuentos
    {
        public int Id { get; set; }
        [Required]
        public int EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public Empleado Empleado { get; set; }
        [Required]
        public int TopeMaximo { get; set; }
        [Required]
        public int ValorDescuento { get; set; }
        public DateTime Fecha { get; set; }
        [Required]
        public DateTime FechaInicioVigencia { get; set; }
        [Required]
        public DateTime FechaFinVigencia { get; set; }
        public string Activo { get; set; }
        [Required]
        public int ServicioId { get; set; }
        [ForeignKey("ServicioId")]
        public Servicios Servicios { get; set; }
        public string  Empresa { get; set; }
        [NotMapped]
        public virtual List<Servicios> ListadoServicios { get; set; }
        
    }
}