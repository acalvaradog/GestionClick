using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class HistorialVacaciones
    {
        public int Id { get; set; }
        [Required]
        public int VacacionesId { get; set; }
        [ForeignKey("VacacionesId")]
        public Vacaciones Vacaciones { get; set; }
        [Required]
        public DateTime Fecha { get; set; }
        [Required]
        public string Accion { get; set; }
        [Required]
        public int EmpleadoId { get; set; }
        [Required]
        public int UsuarioModifica { get; set; }
        public string Observaciones { get; set; }
        [NotMapped]
        public Empleado Empleado { get; set; }
        [NotMapped]
        public EstadoVacaciones EstadoVacaciones { get; set; }
       
    }
}