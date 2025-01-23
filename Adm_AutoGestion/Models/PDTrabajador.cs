using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class PDTrabajador
    {
        public int Id { get; set; }
        [Required]
        public int ProcesoDisciplinarioId { get; set; }
        [ForeignKey("ProcesoDisciplinarioId")]
        public ProcesoDisciplinario ProcesoDisciplinario { get; set; }
        [Required]
        public int EmpleadoId { get; set; }
        [NotMapped]
        public Empleado Empleado2 { get; set; }
        [NotMapped]
        public Empleado Jefe { get; set; }
 
    }
}