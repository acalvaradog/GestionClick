using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class DescuentosReportados
    {
        public int Id { get; set; }
        [Required]
        public string NroDocumento { get; set; }
        [Required]
        public string Mes { get; set; }
        [Required]
        public string anio { get; set; }
        [Required]
        public string ValorDescuento { get; set; }
        public int ServicioId { get; set; }
        [ForeignKey("ServicioId")]
        public Servicios Servicios { get; set; }
        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId ")]
        public Empleado Empleado { get; set; }
        public DateTime Fecha { get; set; }
        [NotMapped]
        public virtual List<Servicios> ListadoServicios { get; set; }
    }
}