using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Dispositivos
    {
        public int Id { get; set; }
        [Required]
        public string token { get; set; }
        [StringLength(8)]
        public string NroEmpleado { get; set; }
        [StringLength(20)]
        public string Documento { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }
}