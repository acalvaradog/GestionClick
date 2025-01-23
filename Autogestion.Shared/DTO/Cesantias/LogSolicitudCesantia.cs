using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Autogestion.Shared.DTO.Cesantias
{
    public class LogSolicitudCesantia
    {
        public int Id { get; set; }
        public int SolicitudCesantiaId { get; set; }
        public SolicitudCesantiaDTO SolicitudCesantia { get; set; }

        public string Usuario { get; set; } // Usuario que realiza la acción
        public string Accion { get; set; } // Ejemplo: "Aprobado", "Rechazado", "Gestionado"
        public DateTime FechaHora { get; set; } // Fecha y hora de la acción
    }
}