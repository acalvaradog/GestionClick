using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class SoporteDestino
    {
        public int Id { get; set; }
        public string Nombre { get; set; } // Ejemplo: "Folio Actual", "Promesa de Compraventa"
        public int DestinoId { get; set; }
        public DestinoCesantia Destino { get; set; }
    }
}