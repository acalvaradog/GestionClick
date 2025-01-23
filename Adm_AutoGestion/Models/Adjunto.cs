using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Adjunto
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public int IdTiposIncapacidad { get; set; }
        [ForeignKey("IdTiposIncapacidad")]
        public TiposIncapacidad TiposIncapacidad { get; set; }

    }
}