using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class TopeDescuento
    {
        public int Id { get; set; }
        [Required]
        public string CodigoEmpleado { get; set; }
        [Required]
        public string NroDocumento { get; set; }
        [Required]
        public int Valor { get; set; }
    }
}