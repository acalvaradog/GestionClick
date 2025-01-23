using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adm_AutoGestion.Models
{
    public class HorasExtra
    {
        public int Id { get; set; }

        [Required]
        public int EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public Empleado Empleado { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int Estado { get; set; }
        [ForeignKey("Estado")]
        public EstadosHorasExtra EstadosHorasExtra { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public DateTime FechaDeRegistro { get; set; }
        [NotMapped]
        public string EstadoNombre { get; set; }
        public int? TotalLiquidacionDiurna { get; set; }
        public int? TotalLiquidacionNocturna { get; set; }
        public int? TotalLiquidacionDiurnaFestivo { get; set; }     
        public int? TotalLiquidacionNocturnaFestivo { get; set; }
        public int? TotalHoras { get; set; }
        public string Firma { get; set;  }

        public DateTime? FechaPago { get; set; }

        internal IEnumerable<HorasExtra> TolistAsync()
        {
            throw new NotImplementedException();
        }
    }
}
