using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Autogestion.Shared.DTO.Noticia
{
    public class NoticiaDTO
    {
        public int Id { get; set; }

        public string Titulo { get; set; }
  
        public string Contenido { get; set; }
  
        public string Autor { get; set; }
        public DateTime Publicacion { get; set; }
        public bool Activo { get; set; }
        public int EmpleadoId { get; set; }
  
       // public EmpleadoDTO Empleado { get; set; }

        public List<NoticiaImagenesDTO> Imagenes { get; set; }
    }
}
