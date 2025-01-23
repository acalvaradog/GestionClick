using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Dotacion
{
    public class DotacionDTO
    {
        public int Id { get; set; }
        public string? Camisa { get; set; }
        public string? Pantalon { get; set; }
        public string? Zapatos { get; set; }
    }
}
