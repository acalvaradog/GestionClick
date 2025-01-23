using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Viaticos
{
    public class DestinoViaticoDTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Estado { get; set; }
    }
}
