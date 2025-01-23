using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adm_AutoGestion.Models
{
    public class TipoElementos
    {

        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [StringLength(20)]
        public string Estado { get; set; }
        
    }
}