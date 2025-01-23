using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO
{
    public class DispositivoDTO
    {
        public int Id { get; set; }

        public string token { get; set; }

        public string NroEmpleado { get; set; }
   
        public string Documento { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }
}
