using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Eventos
{
    public class EventosRelacionadosDTO
    {
        public int Id { get; set; }
        public int EventosId { get; set; }
        public int EventosId2 { get; set; }
    }
}
