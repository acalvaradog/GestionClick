using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.HorasExtra
{
    public class MotivoTrabajoHEDTO
    {
        public int Id { get; set; }

        
        public string Descripcion { get; set; }

        
        public string Estado { get; set; }
    }
}
