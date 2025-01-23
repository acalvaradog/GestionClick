using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class NoticiaImagen
    {
        public int Id { get; set; }
        public string ImagenUrl { get; set; } // Ruta o URL de la imagen
        public int NoticiaId { get; set; }

        [ForeignKey("NoticiaId")]
        public virtual Noticia Noticia { get; set; }
    }
}