using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class CentrodeCostos
    {
        public int Id { get; set; }
        [Required]
        public String Codigo { get; set; }
        [Required]
        public String Descripcion { get; set; }
    }
}