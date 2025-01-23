using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class AreasPazySalvo
    {
        public int Id { get; set; }
        [Required]
        public string Area { get; set; }
        [Required]
        public string Responsable { get; set; }
        [NotMapped]
        public Empleado empleado { get; set; }
    }
}