using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.AutoEvaluacion
{
    public class PreguntaEncuestaDTO
    {
        public int Id { get; set; }
        public string Pregunta { get; set; }
        public Boolean Activo { get; set; }
        public string Grupo { get; set; }

        public bool Seleccion { get; set; }
    }
}
