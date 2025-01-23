using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autogestion.Shared.DTO.Empleado;

namespace Autogestion.Shared.DTO.Vacaciones
{
    public class VacacionesDTO
    {
        public int Id { get; set; }
 
        public int EmpleadoId { get; set; }

        public EmpleadoDTO Empleado { get; set; }
    
        public DateTime Fecha { get; set; }
   
        public DateTime FechaInicial { get; set; }
   
        public DateTime FechaFin { get; set; }
    
        public string? CantDiasSolicitados { get; set; }
   
        public string? CantDiasPendientes { get; set; }
 
        public string? VacacionesPagadas { get; set; }

        public string? VacacionesAdelantadas { get; set; }

        public string? VacacionesDiasMayor6 { get; set; }
        public int EstadoId { get; set; }
 
        public EstadoVacacionesDTO EstadoVacaciones { get; set; }
        public string? Empresa { get; set; }
        public string? Adjunto { get; set; }
        public string? Periodo { get; set; }

        public string? Observacion { get; set; }
    }
}
