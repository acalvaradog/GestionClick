using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Adm_AutoGestion.Models
{
    public class Noticia
    {
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Titulo { get; set; }
        [Required]
        [AllowHtml]
        public string Contenido { get; set; }
        [StringLength(100)]
        public string Autor { get; set; }
        public DateTime Publicacion { get; set; }
        public bool Activo { get; set; }
        public int EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public Empleado Empleado { get; set; }
       // public ICollection<NoticiaImagen> Imagenes { get; set; } // Relación uno a muchos
    }
}