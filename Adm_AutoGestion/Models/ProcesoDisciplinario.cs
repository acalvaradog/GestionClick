using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class ProcesoDisciplinario
    {
        public int Id { get; set; }
        [Required]
        public DateTime FechaRegistro { get; set; }
        [Required]
        public int EmpleadoRegistraId { get; set; }
        [ForeignKey("EmpleadoRegistraId")]
        public Empleado Empleado { get; set; }
        [Required]
        public string Fundamentos { get; set; }
        public string Lugar { get; set; }
        public DateTime FechaHechos { get; set; }
        public string NivelPrioridad { get; set; }
        public string TipoFalta { get; set; }
        public string Sanciones { get; set; }
        public string MtvSancion { get; set; }
        public string Suspencion { get; set; }

        public DateTime? FechaRespuestaJ { get; set; }
        public DateTime? FechaSuspencion { get; set; }
        public string RespuestaJuridica { get; set; }
        [Required]
        public string Estado { get; set; }

        public DateTime? FechaDescargo { get; set; }
        public DateTime? FechaCitacionDes { get; set; }
        public string Justificación { get; set; }
        public string AdjJuridico { get; set; }
        public string Empresa { get; set; }
        [NotMapped]
        public virtual List<PDAnexos> PDAnexos { get; set; }
        [NotMapped]
        public virtual List<PDPruebas> PDPruebas { get; set; }
        [NotMapped]
        public virtual List<PDTrabajador> PDTrabajador { get; set; }
        [NotMapped]
        public Empleado Jefe { get; set; }
        [NotMapped]
        public List<Empleado> Implicados { get; set; }
        [NotMapped]
        public virtual List<NotasProcesos> NotasProcesos { get; set; }
        [NotMapped]
        public string EmpleadoNombres { get; set; }
        [NotMapped]
        public string EmpleadoArea { get; set; }
        [NotMapped]
        public string SuperiorEmpleado { get; set; }
        [NotMapped]
        public string EmpleadosImplicados { get; set; }
      
    }
}