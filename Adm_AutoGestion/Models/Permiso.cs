using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adm_AutoGestion.Models
{
    public class Permiso
    {
        public int Id { get; set; }
        [Required]
        public DateTime Fecha { get; set; }
        [Required]
        public DateTime FechaPermiso { get; set; }
        public DateTime FechaFinPermiso { get; set; }
        public string HoraInicioPermiso { get; set; }
        public string HoraFinPermiso { get; set; }
        public string Remunerado { get; set; }
        public string ObservacionJefe { get; set; }
        public string Observacion { get; set; }
        public string ObservacionGH { get; set; }
        public int EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public Empleado Empleado { get; set; }
        public int MotivoId { get; set; }
        [ForeignKey("MotivoId")]
        public MotivoPermiso MotivoPermiso { get; set; }
          public int EstadoId { get; set; }
        [ForeignKey("EstadoId")]
        public EstadoPermiso EstadoPermiso { get; set; }
        public string Empresa { get; set; }
        public string Adjunto { get; set; }
        public string Jornada { get; set; }
        public string RevisadoNomina { get; set; }
        [NotMapped]
        public virtual List<Empleado> ListadoEmpleadosJefe { get; set; }
        [NotMapped]
        public PersonalActivo PersonalActivo { get; set; }
        [NotMapped]
        public string cantdias { get; set; }
        [NotMapped]
        public virtual List<PermisosAdjuntos> ListadoAdjuntos { get; set; }
      
    }
}