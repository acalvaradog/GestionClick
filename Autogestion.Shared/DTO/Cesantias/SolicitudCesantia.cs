using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Autogestion.Shared.DTO.Cesantias
{
    public class SolicitudCesantiaDTO
    {
        public int Id { get; set; }
        public int EmpleadoId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public decimal ValorRetiro { get; set; }

        // Relación con Destino
        public int DestinoId { get; set; }
        public DestinoCesantiaDTO? Destino { get; set; }

        // Relación con Estado
        public int FondoCesantiasId { get; set; }
        public FondoCesantiasDTO FondoCesantias { get; set; }


        public int EstadoId { get; set; }
        public EstadoCesantiaDTO? Estado { get; set; }
        public List<SoporteCesantiaDTO>? Soportes { get; set; }

        public string Observacion { get; set; }

    }
}