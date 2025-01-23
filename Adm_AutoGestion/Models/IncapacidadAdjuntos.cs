using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class IncapacidadAdjuntos
    {
        public int Id { get; set; }
        [Required]
        public int IncapacidadId { get; set; }
        [ForeignKey("IncapacidadId")]
        public Incapacidades Incapacidades { get; set; }
        [Required]
        public string TipoIncapacidad { get; set; }
        [Required]
        public string TipoAdjunto { get; set; }
        [Required]
        public string Adjunto { get; set; }
        [NotMapped]
        public virtual TiposIncapacidad ListadoTiposInc { get; set; }
        [NotMapped]
        public string NombreAdjunto { get; set; }
        [NotMapped]
        public virtual Empleado Empleado { get; set; }
    }
}