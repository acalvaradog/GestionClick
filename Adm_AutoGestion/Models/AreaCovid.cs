using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adm_AutoGestion.Models
{
    public class AreaCovid
    {
        public string Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Nombre { get; set; }

    }
}