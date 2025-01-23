using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class SoporteCesantia
    {
        public int Id { get; set; }
        public int SolicitudCesantiaId { get; set; }
        [JsonIgnore]
        public SolicitudCesantia SolicitudCesantia { get; set; }

        public string NombreSoporte { get; set; } // Ejemplo: "Folio matrícula inmobiliaria"
        public string UrlSoporte { get; set; }   // Ruta al archivo del soporte almacenado
  
    }
}