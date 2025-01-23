using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Autogestion.Shared.DTO.Viaticos
{
    public class VehiculoDTO
    {

        public int id { get; set; }
        public string? Nombre { get; set; }
        public int Valor { get; set; }

    }
    
}
