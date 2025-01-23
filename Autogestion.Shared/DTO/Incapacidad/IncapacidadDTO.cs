using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autogestion.Shared.DTO.Empleado;

namespace Autogestion.Shared.DTO.Incapacidad
{
    public class IncapacidadDTO
    {
     
            public int Id { get; set; }

            public int EmpleadoId { get; set; }

            public EmpleadoDTO Empleado { get; set; }
 
            public DateTime? Fecha { get; set; }

            public DateTime? FechaInicio { get; set; }

            public DateTime? FechaFin { get; set; }
 
            public string CantidadDias { get; set; }
   
            public string Diagnostico { get; set; }
            public int EstadoId { get; set; }

            public string Estado { get; set; }

            public string Prorroga { get; set; }
  
            public int EPSId { get; set; }
            public string Empresa { get; set; }

            public int TipoIncapacidadId { get; set; }
            public virtual List<TiposIncapacidadDTO> ListadoTiposInc { get; set; }


       
    }
}
