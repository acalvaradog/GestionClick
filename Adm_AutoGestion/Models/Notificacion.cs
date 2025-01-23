using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Notificacion
    {

        public int Id { get; set; }
        public int EmpleadoId { get; set; }
        public Empleado Empleado { get; set; }
        public DateTime Fecha { get; set; }
        public string Mensaje { get; set; }
    }
}