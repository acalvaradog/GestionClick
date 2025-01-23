using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class NotasProcesos
    {
        public int Id { get; set; }
        public string Anotacion { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaHora { get; set; } 
        public string Anexo { get; set; }

        public int ProcesoDisciplinarioId { get; set; }
        [ForeignKey("ProcesoDisciplinarioId")]
        public ProcesoDisciplinario ProcesoDisciplinario { get; set; }
        [NotMapped]
        public Empleado Empleado2 { get; set; }
    }
}