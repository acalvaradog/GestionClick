using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Evadesh
{
    public class evaluacion_otrosDTO
    {

        public int Id { get; set; }
        public int? codigoevaluacion { get; set; }
        public string? fortalezas { get; set; }
        public string? aspectos { get; set; }
        public string? recomendacion { get; set; }
        public string? porque { get; set; }
        public string? Autoevaluacion { get; set; }
        
        public int CodEmp { get; set; }
    }
}
