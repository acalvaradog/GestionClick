using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.DTO
{
    public class CrearCursoDTO
    {
        public string FullName { get; set; }
        public string ShortName { get; set; }

        public int CategoryId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}