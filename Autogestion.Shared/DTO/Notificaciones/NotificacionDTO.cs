using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Notificaciones
{
    public class NotificacionDTO
    {
        public int Id { get; set; }
        public int EmpleadoId { get; set; }
        public DateTime Fecha { get; set; }
        public string Mensaje { get; set; }
        public string Titulo { get; set; }
        public string Icono { get; set; }
        public string Accion { get; set; }
        public string Dispositivo { get; set; }
    }
}
