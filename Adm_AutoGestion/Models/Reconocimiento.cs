using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adm_AutoGestion.Models
{
    public class Reconocimiento
    {

        public int Id { get; set; }

        public int EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public Empleado Empleado { get; set; }
        public int TipoReconocimientoId { get; set; }
        [ForeignKey("TipoReconocimientoId")]
        public TipoReconocimiento TipoReconocimiento { get; set; }
        public string Fuente { get; set; }
        public DateTime Fecha { get; set; }
        public string Observaciones { get; set; }
        public bool Visto { get; set; }
        public bool Activo { get; set; }
        
        public int? EmpleadoReconocidoId { get; set; }
        [ForeignKey("EmpleadoReconocidoId")]
        public Empleado EmpleadoReconocido { get; set; }
        public int? EmpleadoModificaId { get; set; }
        [ForeignKey("EmpleadoModificaId")]
        public Empleado EmpleadoModifica { get; set; }
        [NotMapped]
        public string EmpleadoSeleccionado { get; set; }
        [NotMapped]
        public string TipoNombre { get; set; }
        [NotMapped]
        public string TipoTexto { get; set; }
        [NotMapped]
        public string TipoImagen { get; set; }
        [NotMapped]
        public string EmpleadoNombre { get; set; }
        [NotMapped]
        public string EmpReco { get; set; }
    }
}