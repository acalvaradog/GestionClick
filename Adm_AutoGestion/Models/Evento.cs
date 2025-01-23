using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adm_AutoGestion.Models
{
    public class Evento
    {
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Titulo { get; set; }
        [Required]
        [StringLength(200)]
        public string Descripcion { get; set; }
        public Boolean Activo { get; set; }
        public int EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public Empleado Empleado { get; set; }
    }
}