using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.HorasExtra
{
    public class ContructorHE_DTO
    {
        public bool HayRegistroValido { get; set; }
        public float HorasdiaunoDB { get; set; }
        public float HorasdiaPosterior { get; set; }
        public string Error { get; set; }

    }
}
