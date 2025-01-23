using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class HistorialDotacion
    {
        public int Id { get; set; }
        public string Nro { get; set; }
        public int EmpleadoId { get; set; }
        [NotMapped]
        public Empleado Empleado { get; set; }
        public DateTime? Fecha { get; set; }
        public bool Recibido { get; set; }
        public DateTime? FechaRecibido { get; set; }
        public int CantidadEntregada { get; set; }
        public int CantidadPendiente { get; set; }
        [NotMapped]
        public string NombreEmpleado { get; internal set; }
    }
}