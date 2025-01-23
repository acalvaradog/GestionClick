using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class DetalleReunion
    {
        public int Id { get; set; }
        public int EmpleadoId { get; set; }
        public string FechaFirma { get; set; }
        public string Firma { get; set; }
        public int ReunionId { get; set; }
        [ForeignKey("ReunionId")]
        public Reunion Reunion { get; set; }
        public string Desarrollo { get; set; }
       
        
       
       
    }
}