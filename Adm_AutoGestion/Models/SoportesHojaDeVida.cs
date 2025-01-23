
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class SoportesHojaDeVida
    {
        public int Id { get; set; }
        public string Titulo { get; set; }

        // Otros campos relevantes para el soporte
        public string NombreArchivo { get; set; }
        public int EmpleadoId { get; set; }
        public Empleado Empleado { get; set; }

        public int TipoSoporteId { get; set; }
        public TipoSoporte TipoSoporte { get; set; }

        [NotMapped]
        public HttpPostedFile Archivo { get; set; }
    }
}