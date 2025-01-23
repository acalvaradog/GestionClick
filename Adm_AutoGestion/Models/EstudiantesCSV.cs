using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adm_AutoGestion.Models
{
    public class EstudiantesCSV
    {
        public string Documento { get; set; }
        public string Nombres { get; set; }
        public string CorreoPersonal { get; set; }
        public string Area { get; set; }
        public string Cargo { get; set; }
        public string Universidad { get; set; }
    }
}
