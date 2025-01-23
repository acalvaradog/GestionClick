using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Aprobacioneslog
    {
        public int Id { get; set; }

        public int  IdRequerimiento {get;set;}
        [ForeignKey("IdRequerimiento")]
        public RequerimientosDelPersonal RequerimientosDelPersonal { get; set; }

        public DateTime Fecha { get; set; }
        public int Usuario { get; set; }
        public int EstadoAnterior { get; set; }
        public int EstadoNuevo { get; set; }
        public string Observación { get; set; }
        [NotMapped]
        public Empleado DatosUsuario { get; set; }
    }
}