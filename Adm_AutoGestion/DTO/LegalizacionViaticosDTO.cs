using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.DTO
{
    public class LegalizacionViaticosDTO
    {
        public int Id { get; set; }

        public int ViaticosId { get; set; }  // Relación con Viaticos
        public DateTime FechaRegistro { get; set; } //fecha registro de la legalizacion

        public DateTime FechaSalida { get; set; }  // Fecha de salida del viaje

        public DateTime FechaLlegada { get; set; }  // Fecha de llegada del viaje

        public int TotalViajeGenerales { get; set; }  // Gastos de hotel, transporte, varios|

        public int EstadoId { get; set; }  // Pendiente, aprobado

        public string EstadoNombre { get; set; }  // Nombre del estado legalizacion

        public int? GastosRepresentacion { get; set; }  // Gastos de representación, si aplica

        public int? GastosInesperados { get; set; }  // Gastos inesperados, si aplica

        public int? TotalViaje { get; set; }  // Incluye (A-B-C)
        public int? ValorViaje { get; set; } // valor del viaje cuando se solicito el viatico
        public int? AnticipoRecibido { get; set; }  // Anticipo recibido por el empleado

        public int? CuentaCorrienteHotel { get; set; }  // Cuenta corriente del hotel, si aplica

        public int? TotalDevolverEmpleado { get; set; }  // Total a devolver por el empleado (D-E-F)

        public int? EmpleadoAprobadorId { get; set; }  // Relación con el empleado que aprueba

        public string EmpleadoAprobadorNombre { get; set; }  // Nombre del empleado que aprueba

        public DateTime? FechaAprobacion { get; set; }  // Fecha en la que fue aprobada la legalización

        public DateTime? FechaAnticipo { get; set; } // Fecha en la que se hizo el anticipo
        public List<DetalleLegalizacionViaticosDTO> DetalleLegalizacionViaticos { get; set; } = new List<DetalleLegalizacionViaticosDTO>();
    }
}