using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Motivos
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string Nombre { get; set; }
        public Boolean Activo { get; set; }
    }
}