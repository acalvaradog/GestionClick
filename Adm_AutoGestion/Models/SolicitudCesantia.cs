using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class SolicitudCesantia
    {
        public int Id { get; set; }
        public int EmpleadoId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public decimal ValorRetiro { get; set; }

        // Relación con Destino
        public int DestinoId { get; set; }
        public DestinoCesantia Destino { get; set; }

        // Relación con Estado
        public int EstadoId { get; set; }
        public EstadoCesantia Estado { get; set; }

        public int FondoCesantiasId { get; set; }
        public FondoCesantias FondoCesantias { get; set; }
        public Empleado Empleado { get; set; }
        public List<SoporteCesantia> Soportes { get; set; }
        public List<LogSolicitudCesantia> Log { get; set; }

    }
}