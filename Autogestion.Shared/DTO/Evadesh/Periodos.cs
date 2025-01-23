using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Evadesh
{
    public class Periodos
    {
        public int codigo { get; set; }
        public DateTime fechaincio { get; set; }
        public DateTime fechafinal { get; set; }
        public int sociedad { get; set; }
        public int TipoPeriodo { get; set; }

        public bool? Retroalimentacion { get; set; } = false;
    }
}
