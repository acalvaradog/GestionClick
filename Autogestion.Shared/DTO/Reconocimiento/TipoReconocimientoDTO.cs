using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Reconocimiento
{
    public class TipoReconocimientoDTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Imagen { get; set; }
        public string? Texto { get; set; }
    }
}
