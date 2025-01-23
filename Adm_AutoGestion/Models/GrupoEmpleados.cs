using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class GrupoEmpleados
    {
        public int Id { get; set; }
        [Required]
        public string Descripcion { get; set; }
        [Required]
        public Boolean Estado { get; set; }
    }
}