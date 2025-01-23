using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Sociedad
    {
        public int Id { get; set; }
        [Required]
        public String Codigo { get; set; }
        [Required]
        public String Descripcion { get; set; }

        [NotMapped]
        public List<Sede> Sedes { get; set; }
    }
}