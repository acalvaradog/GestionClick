using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autogestion.Shared.DTO.Empleado;

namespace Autogestion.Shared.DTO.Permisos
{
    public class PermisoDTO
    {

        public int Id { get; set; }

        public DateTime Fecha { get; set; }
   
        public DateTime FechaPermiso { get; set; }
        public DateTime FechaFinPermiso { get; set; }
        public string HoraInicioPermiso { get; set; }
        public string HoraFinPermiso { get; set; }
        public string Remunerado { get; set; }
        public string ObservacionJefe { get; set; }
        public string Observacion { get; set; }
        public string ObservacionGH { get; set; }
        public int EmpleadoId { get; set; }
     
        public EmpleadoDTO Empleado { get; set; }
        public int MotivoId { get; set; }

        public MotivoPermisoDTO MotivoPermiso { get; set; }
        public int EstadoId { get; set; }
  
        public EstadoPermisoDTO EstadoPermiso { get; set; }
        public string Empresa { get; set; }
        public string Adjunto { get; set; }
        public string Jornada { get; set; }
        public string RevisadoNomina { get; set; }

        public virtual List<EmpleadoDTO> ListadoEmpleadosJefe { get; set; }

  
        public string cantdias { get; set; }

    }
}
