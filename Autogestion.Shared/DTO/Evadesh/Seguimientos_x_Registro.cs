using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Evadesh
{
    public class Seguimientos_x_Registro
    {
        public int? Codigo { get; set; }
        public int? Cod_Seguimiento { get; set; }
        public DateTime? Fecha { get; set; }
        public string Descripcion { get; set; }
        public DateTime? Proximo_Seguimiento { get; set; }
        public bool? Cumplimiento { get; set; }
        public string Cumplimiento_Observacion { get; set; }
        public bool? Socializado_Emp { get; set; }
        public DateTime? Fecha_SocEmp { get; set; }
        public string Observacion_Emp { get; set; }

        public string TEXTO {  get; set; }
    }

}
