using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.HorasExtra
{
    public class HorasExtraDTO
    {
        public int Id { get; set; }
        public int EmpleadoId { get; set; }
        public int Estado { get; set; }
        public string? EstadoNombre { get; set; }
        public DateTime FechaDeRegistro { get; set; }
        public float? TotalLiquidacionDiurna { get; set; }



        public float? TotalLiquidacionNocturna { get; set; }



        public float? TotalLiquidacionDiurnaFestivo { get; set; }



        public float? TotalLiquidacionNocturnaFestivo { get; set; }
        public float? TotalHoras { get; set; }
        public bool? Terminos { get; set; }
        public string Firma { get; set;  }
    }
    
}
