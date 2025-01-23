using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Evadesh
{
    [Table("encuestadores x empleado")]
    public class encuestadores_x_empleado
    {
        public int? codigo { get; set; }
        public int? codigoempleado { get; set; }
        public int? tipoevaluador { get; set; }
        public int? codigoevaluador { get; set; }
        public DateTime? fecharegistro { get; set; }
        public TimeSpan? horaregistro { get; set; }
        public int? usuarioregistro { get; set; }
        public int? periodo { get; set; }
        public int? tipoevaluacion { get; set; }
        public bool? RetroalimentacionEmp { get; set; }
        public bool? RetroalimentacionJefe { get; set; }
        public int? Estado { get; set; }
    }
}
