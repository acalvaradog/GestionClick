using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Autogestion.Shared.DTO.Cesantias
{
    public class SoporteDestinoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } // Ejemplo: "Folio Actual", "Promesa de Compraventa"
        public int DestinoId { get; set; }
        public DestinoCesantiaDTO Destino { get; set; }
        public string? adjunto { get; set; }
    }
}