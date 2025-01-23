using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autogestion.Shared.DTO.Empleado;
namespace Autogestion.Shared.DTO.SoporteHojaVida
{
    public class SoporteHojaVidaDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }

        // Otros campos relevantes para el soporte
        public string NombreArchivo { get; set; }
        public int EmpleadoId { get; set; }
        public EmpleadoDTO Empleado { get; set; }

        public int TipoSoporteId { get; set; }
        public TipoSoporteDTO TipoSoporte { get; set; }

  
    }
}
