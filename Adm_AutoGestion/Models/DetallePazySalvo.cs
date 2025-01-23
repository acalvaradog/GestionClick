using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class DetallePazySalvo
    {
        public int Id { get; set; }
        [Required]
        public int IdPazySalvo { get; set; }
        [ForeignKey("IdPazySalvo")]
        public PazySalvo PazySalvo { get; set; }
        [Required]
        public string Area { get; set; }
        public int Responsable { get; set; }
        public string Firma { get; set; }
        public DateTime? FechaFirma { get; set; }
        public string Observacion { get; set; }
        [NotMapped]
        public Empleado Empleado { get; set; }
    }
}