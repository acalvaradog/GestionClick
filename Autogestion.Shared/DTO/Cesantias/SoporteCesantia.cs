using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Autogestion.Shared.DTO.Cesantias
{
    public class SoporteCesantiaDTO
    {
        public int Id { get; set; }
        public int SolicitudCesantiaId { get; set; }
        public SolicitudCesantiaDTO SolicitudCesantia { get; set; }

        public int DestinoId { get; set; }
        public DestinoCesantiaDTO Destino { get; set; }

        public string NombreSoporte { get; set; } // Ejemplo: "Folio matrícula inmobiliaria"
        public string UrlSoporte { get; set; }   // Ruta al archivo del soporte almacenado
        public DateTime FechaCarga { get; set; }
    }
}