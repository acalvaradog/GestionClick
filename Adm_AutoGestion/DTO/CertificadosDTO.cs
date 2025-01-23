using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.TalentoHumano
{
    public class CertificadosDTO
    {
        public int? Id { get; set; }
        public int? EmpleadoId { get; set; }
        public string Titulo { get; set; }
        public string Estado { get; set; }
        public string Archivo { get; set; }
        public string Observacion { get; set; }
        public DateTime FechaCaducidad { get; set; }
    }
}
