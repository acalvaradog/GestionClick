using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Adm_AutoGestion.Models
{
    public class EstadoPermiso
    {

        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
    }
}