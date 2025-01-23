using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.HorasExtra
{
    public class InformeTotalHEDTO
    {
        public int Id { get; set; }
        public int EmpleadoId { get; set; }
        //public Empleado Empleado { get; set; }
        public HorasExtraDTO HorasExtra { get; set; }
        public DateTime FechaHora { get; set; }
        public string HoraDesde { get; set; }
        public string HoraHasta { get; set; }
        public string ObservacionesMotivo { get; set; }
        public float? LiquidacionDiurna { get; set; }
        public float? LiquidacionNocturna { get; set; }
        public float? LiquidacionDiurnaFestivo { get; set; }
        public float? LiquidacionNocturnaFestivo { get; set; }
        public MotivoTrabajoHEDTO MotivoTrabajoHE { get; set; }
        public float? TotalHoras { get; set; }
        public string MotivoNombre { get; set; }
        public DateTime FechadeRegistro { get; set; }
        public int Estado { get; set; }
        public DateTime? FechaPago { get; set; }
        public EstadosHorasExtrasDTO EstadosHorasExtra { get; set; }
        public string EstadoNombre { get; set; }
        public string Observacion { get; set; }

    }
}
