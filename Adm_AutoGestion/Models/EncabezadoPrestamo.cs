using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adm_AutoGestion.Models
{
    public class EncabezadoPrestamo
    {

        public int Id { get; set; }
        [Required]
        public string Documento { get; set; }
        public DateTime FechaRegistro { get; set; }
        [Required]
        public String AreaDirige { get; set; }
        [Required]
        public String TipoArea { get; set; }
        public String LugarEntrega { get; set; }
        [StringLength(20)]
        public String Estado { get; set; }
        [Required]
        public String Sociedad { get; set; }
        public int UsuarioRegistraId { get; set; }
        public String QRPrestamos { get; set;  }

        [NotMapped]
        public virtual List<DetalleEncabezadoPrestamo> ListadoElementos { get; set; }
       
    }
}