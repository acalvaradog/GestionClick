using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace Adm_AutoGestion.Models
{
    public class PreguntaFrecuente
    {
        public int Id { get; set; }
        [Required]
        public string Pregunta { get; set; }
        [Required]
        public string Respuesta { get; set; }
        public Boolean Activo { get; set; }
        public int TemaId { get; set; }
        [ForeignKey("TemaId")]
        public TemaPregunta TemaPregunta { get; set; }
        public int EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public Empleado Empleado { get; set; }
        //Listas desplegable
        [NotMapped]
        public virtual List<TemaPregunta> ListadoTemas { get; set; }
        [NotMapped]
        public virtual List<Empleado> ListadoEmpleados { get; set; }
    }
  

}