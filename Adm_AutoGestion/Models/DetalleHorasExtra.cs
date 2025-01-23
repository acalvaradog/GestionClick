using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adm_AutoGestion.Models
{
    public class DetalleHorasExtra
    {
        public int Id { get; set; }

        public int HorasExtraId { get; set; }
        [ForeignKey("HorasExtraId")]
        public HorasExtra HorasExtra { get; set; }


        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string HoraDesde { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string HoraHasta { get; set; }

        public string ObservacionesMotivo { get; set; }
        
        public int? LiquidacionDiurna { get; set; }

        
        
        public int? LiquidacionNocturna { get; set; }

        
        
        public int? LiquidacionDiurnaFestivo { get; set; }

        
        

        public int? LiquidacionNocturnaFestivo { get; set; }




        public int MotivoTrabajoHEId { get; set; }
        [ForeignKey("MotivoTrabajoHEId")]
        public MotivoTrabajoHE MotivoTrabajoHE { get; set; }

        
        public int? TotalHoras { get; set; }
        [NotMapped]
        public string MotivoNombre { get; set; }

    }
}
