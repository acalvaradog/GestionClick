using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Autogestion.Shared.DTO.Cesantias
{
    public class DestinoCesantiaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } // Ejemplo: "Adquisición de Vivienda"

        // Requerimientos específicos del destino
        public List<SoporteDestinoDTO> Requerimientos { get; set; }
    }
}