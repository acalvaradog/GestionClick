using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Adm_AutoGestion.Models
{
    public class DetalleEncabezadoPrestamo
    {

        public int Id { get; set; }
        [Required]
        public int IdEncabezadoPrestamo{ get; set; }
        [ForeignKey("IdEncabezadoPrestamo")]
        public EncabezadoPrestamo EncabezadoPrestamo { get; set; }
        public int IdTipoElementos { get; set; }
        [ForeignKey("IdTipoElementos")]
        public TipoElementos TipoElementos { get; set; }
        [Required]
        public string Documento { get; set; }
        [Required]
        public int Cantidad { get; set; }
        [Required]
        public DateTime FechaEntrega { get; set; }
        [StringLength(50)]
        public string FechaFirmaEntrega { get; set; }
        public DateTime? FechaFirmaRecibido { get; set; }
        public string Observacion { get; set; }
        [Required]
        [StringLength(20)]
        public String Estado { get; set; }
        public int UsuarioModificaId { get; set; }
        public string Talla { get; set; }
        public string QRPrestamos { get; set; }
        [NotMapped]
        public virtual List<TipoElementos> ListadoElementos { get; set; }
        
     
    }
}