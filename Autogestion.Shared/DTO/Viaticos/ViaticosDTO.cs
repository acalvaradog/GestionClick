using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autogestion.Shared.DTO.Empleado;

namespace Autogestion.Shared.DTO.Viaticos
{
    public class ViaticosDTO
    {


        public int Id { get; set; }
        public int EmpleadoId { get; set; }
        public object Empleado { get; set; }
        public bool Prorroga { get; set; }
        public int IdSolicitud { get; set; }
        public int? DestinoViaticoID { get; set; }   
        
        public string MtvViaje { get; set; }
        public bool Hospedaje { get; set; }
        public string Vehiculo { get; set; }
        public DateTime? FechaInicio { get; set; } = Convert.ToDateTime(DateTime.Today);
        public DateTime? FechaFin { get; set; } = Convert.ToDateTime(DateTime.Today);
        public int GastosTransporte { get; set; }
        public int GastoAlimentacion { get; set; }
        public bool CheckNomina { get; set; }
        public bool CheckTesoreria { get; set; }
        public int? Estado { get; set; }
        public string? EstadoNombre { get; set; }        
        public string? Observacion { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string? Destino { get; set; }
        public string? Placa { get; set; }
        public bool ViajeRealizado { get; set; }
        public int Total { get; set; }
    }

    
}
