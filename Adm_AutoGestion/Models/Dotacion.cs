using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Dotacion
    {
        public int Id { get; set; }
        public string Nro { get; set; }
        public int EmpleadoId { get; set; }
        public Empleado Empleado { get; set; }
        public DateTime? Fecha { get; set; }
        [NotMapped]
        public string NombreEmpleado { get; internal set; }
        [NotMapped]
        public string Categoria { get; set; }
        [NotMapped]
        public string TipoArea { get; set; }
        [NotMapped]
        public int Año { get; set; }
        [NotMapped]
        public bool? Recibido { get; set; }
        [NotMapped]
        public string Empresa { get; set; }
        public string Tallas { get; set; }
        [NotMapped]
        public int? CantidadEntregas { get; set; }

    }
}