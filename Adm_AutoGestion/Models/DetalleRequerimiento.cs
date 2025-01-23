using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class DetalleRequerimiento
    {

        public int Id { get; set; }
        [Required]
        public int RequerimientoId { get; set; }
        [ForeignKey("RequerimientoId")]
        public RequerimientosDelPersonal RequerimientosDelPersonal { get; set; }
        [StringLength(8)]
        public string EmpSaliente { get; set; }
        public int MotivoEgresoId { get; set; }//Maestro
        [ForeignKey("MotivoEgresoId")]
        public MotivoEgreso MotivoEgreso { get; set; }
        [StringLength(100)]
        public string Contratado { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public string TipoEmpleado { get; set; }
        [NotMapped]
        public Empleado Empleado { get; set; }

        
        

    }
}