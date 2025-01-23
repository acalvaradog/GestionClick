using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.PreguntasFrecuentes
{
    public class PreguntaFrecuentes
    {
       public int? Id { get; set; }
      public string Pregunta { get; set; }
      public string Respuesta { get; set; }
      public bool Activo { get; set; }
      public int? TemaId { get; set; }
      public int? EmpleadoId { get; set; }
    }
}
