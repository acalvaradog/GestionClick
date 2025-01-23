using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.HorasExtra
{
    public class DiasFestivosDTO
    {
        public int Id { get; set; }
        public DateTime festivo { get; set; }
    }
}
