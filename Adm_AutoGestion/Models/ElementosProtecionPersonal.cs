using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Adm_AutoGestion.Models
{
    public class ElementosProtecionPersonal
    {

        public int Id { get; set; }
        [Required]
        [StringLength(500)]
        public String Nombre { get; set; }
        [Required]
        [StringLength(2)]
        public String Activo { get; set; }
    }
}