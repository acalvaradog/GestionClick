using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Empleado
{
    public class DepartamentoDTO
    {
        public int? Id { get; set; }
        public string Nombre { get; set; }
        public string CodDepartamento { get; set; }
        public string Estado { get; set; }
    }
}
