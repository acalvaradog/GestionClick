using Autogestion.Shared.DTO.Empleado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Chat
{
    public class MessageDTO
    {
        public long Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public int FromEmpleadoId { get; set; } // Propiedad para la clave foránea de FromUserId
        public int ToEmpleadoId { get; set; }   // Propiedad para la clave foránea de ToUserId

        public EmpleadoDTO? FromEmpleado { get; set; } // Propiedad de navegación
        public EmpleadoDTO? ToEmpleado { get; set; }   // Propiedad de navegación
    }
}
