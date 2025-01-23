using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.HorasExtra
{
    public class DetalleHorasExtraDTO
    {
        public int Id { get; set; }

        public int HorasExtraId { get; set; }
        
        

        public DateTime? Fecha { get; set; } = Convert.ToDateTime(DateTime.Today);

        public string HoraDesde { get; set; }

        public string HoraHasta { get; set; }
        public string ObservacionesMotivo { get; set; }

        public float? LiquidacionDiurna { get; set; }

        
        public float? LiquidacionNocturna { get; set; }

        
        public float? LiquidacionDiurnaFestivo { get; set; }

        
        public float? LiquidacionNocturnaFestivo { get; set; }

        public int MotivoTrabajoHEId { get; set; }
        //? permite null
        public string? MotivoDescripcion { get; set; }

        public float? TotalHoras { get; set; }
        public string? MotivoNombre { get; set; }

    }
}
