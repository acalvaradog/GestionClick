using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Reunion
    {
       
            public int Id { get; set; }
            [Required]
            public DateTime FechaReunion { get; set; }
            public string HoraInicio { get; set; }
            public string HoraFin { get; set; }
            [Required]
            public int AreaOrganizadora { get; set; }
            public int TipoReunionId { get; set; }
            [ForeignKey("TipoReunionId")]
            public TipoReunion TipoReunion { get; set; }
            [Required]
            public string temas { get; set; }
            [Required]
            public DateTime FechaRegistro { get; set; }
            public int EmpleadoRegistraId { get; set; }
            [ForeignKey("EmpleadoRegistraId")]
            public Empleado Empleado { get; set; }
            public string EstadoReunion { get; set; }
            [Required]
            public string Empresa { get; set; }
            
            
            
            
            


        
    }
}