using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Models
{
    public class Vacaciones
    {
        public int Id { get; set; }
        [Required]
        public int EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public Empleado Empleado { get; set; }
        [Required]
        public DateTime Fecha { get; set; }
        [Required]
        public DateTime FechaInicial { get; set; }
        [Required]
        public DateTime FechaFin { get; set; }
        [Required]
        public string CantDiasSolicitados { get; set; }
        [Required]
        public string CantDiasPendientes { get; set; }
        [Required]
        public string VacacionesPagadas { get; set; }
        [Required]
        public string VacacionesAdelantadas { get; set; }
        [Required]
        public string VacacionesDiasMayor6 { get; set; }
        public int EstadoId { get; set; }
        [ForeignKey("EstadoId")]
        public EstadoVacaciones EstadoVacaciones { get; set; }
        public string Empresa { get; set; }
        public string Adjunto { get; set; }
        [NotMapped]
        public virtual List<SelectListItem> Opciones { get; set; }
        [NotMapped]
        public virtual List<Empleado> ListadoEmpleado { get; set; }
        [NotMapped]
        public virtual List<Empleado> ListadoEmpleadosJefe { get; set; }
        [NotMapped]
        public virtual int IdModifica { get; set; }
        [NotMapped]
        public PersonalActivo personal { get; set; }
        [NotMapped]
        public HistorialVacaciones HistorialVacaciones { get; set; }
        [NotMapped]
        public Empleado Empleado2 { get; set; }
        [NotMapped]
        public string Periodo { get; set; }
        [Required]
        public string Observacion { get; set; }

    }
}