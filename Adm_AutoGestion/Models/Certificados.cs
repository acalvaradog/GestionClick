using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adm_AutoGestion.Models
{
    public class Certificados
    {
        public int Id { get; set; }
        public int EmpleadoId { get; set; }
        public string Titulo { get; set; }
        public int IdCursoNormativo { get; set; }

        public string Estado { get; set; }
        public string Archivo { get; set; }
        public string Observacion { get; set; }
        [NotMapped]
        public string NombreEmpleado { get; set; }
        public DateTime FechaCaducidad { get; set; }
    }
}
