using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Eventos
{
    public class DetalleEventosDTO
    {
        public int Id { get; set; }
        public int? EventosId { get; set; }
        public int? EmpleadoId { get; set; }
        public string? FamiliarId { get; set; }
        public string? Observaciones { get; set; }
    }
}
