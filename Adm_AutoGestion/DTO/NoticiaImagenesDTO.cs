using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Noticia
{
    public class NoticiaImagenesDTO
    {
      
            public int Id { get; set; }
            public string ImagenUrl { get; set; } // Ruta o URL de la imagen
            public int NoticiaId { get; set; }
            public virtual NoticiaDTO Noticia { get; set; }
       
    }
}
