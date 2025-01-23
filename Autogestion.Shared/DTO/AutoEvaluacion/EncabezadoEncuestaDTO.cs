using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autogestion.Shared.DTO.Empleado;

namespace Autogestion.Shared.DTO.AutoEvaluacion
{
    public class EncabezadoEncuestaDTO
    {
        public int Id { get; set; }
   
        public int EmpleadoId { get; set; }
    
        public EmpleadoDTO Empleado { get; set; }

        public string Cargo { get; set; }

        public string UnidadOrganizativa { get; set; }

        public string Eps { get; set; }

        public string Transporte { get; set; }

        public string ModoTrabajo { get; set; }
        public string Sospechoso { get; set; }
        public string Empresa { get; set; }
        public DateTime Fecha { get; set; }
        public string Temperatura { get; set; }
        public string Cerco { get; set; }
        [NotMapped]
        public virtual List<EncuestaDTO> Encuesta { get; set; }
        [NotMapped]
        public virtual List<EmpleadoDTO> ListadoEmpleado { get; set; }
        [NotMapped]
        public string sintomas { get; set; }
    }
}
