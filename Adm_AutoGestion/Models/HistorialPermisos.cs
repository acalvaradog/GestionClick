using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Adm_AutoGestion.Models
{
    public class HistorialPermisos
    {
        public int Id { get; set; }
        [Required]
        public int PermisoId { get; set; }
        [ForeignKey("PermisoId")]
        public Permiso Permiso { get; set; }
        [Required]
        public DateTime Fecha_Permiso { get; set; }
        
        public string Estado { get; set; }
        [Required]         
        public int EmpleadoId { get; set; }
        [Required]
        public int Usuario_Modifica { get; set; }
        public string Observaciones_Permiso { get; set; }


        [NotMapped]
        public Empleado Empleado { get; set; }
        [NotMapped]
        public EstadoPermiso EstadoPermiso { get; set; }

    }
}