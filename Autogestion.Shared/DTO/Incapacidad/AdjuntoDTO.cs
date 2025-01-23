using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Incapacidad
{
    public class AdjuntoDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public int IdTiposIncapacidad { get; set; }

        public string adjunto { get; set; }
    }
}
