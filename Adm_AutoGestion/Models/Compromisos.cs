using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Compromisos
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaLimiteCompromiso { get; set; }
        public string EmpleadoId { get; set; }
        public int ReunionId { get; set; }
        [ForeignKey("ReunionId")]
        public Reunion Reunion { get; set; }
        public string Estado { get; set; }
        
    }
}