using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class DestinoCesantia
    {
        public int Id { get; set; }
        public string Nombre { get; set; } // Ejemplo: "Adquisición de Vivienda"

        // Requerimientos específicos del destino
        public List<SoporteDestino> Requerimientos { get; set; }
    }
}