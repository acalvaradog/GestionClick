using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.AutoEvaluacion
{
    public class EncuestaDTO
    {
        public int Id { get; set; }

        public int NumeroPregunta { get; set; }

        public PreguntaEncuestaDTO Preguntas { get; set; }

        public string Respuesta { get; set; }
        public int EncabezadoEncuesta_Id { get; set; }

        public EncabezadoEncuestaDTO EncabezadoEncuesta { get; set; }
        //public virtual EncabezadoEncuesta EncabezadoEncuestas { get; set; }
        [NotMapped]
        public virtual List<PreguntaEncuestaDTO> ListadoPreguntas { get; set; }

    }
}
