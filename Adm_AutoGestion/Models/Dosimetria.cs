using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
 
        public class Dosimetria
        {
            [Key]
            public int Id { get; set; }

            [Required]
            [ForeignKey("Empleado")]
            public int EmpleadoId { get; set; } // Relación con la tabla de empleados.

            [Required]
            public int Anio { get; set; } // Año del registro.

            [Required]
            public int Mes { get; set; } // Mes del registro (1-12).

           
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal? ValorHp10 { get; set; } // Valor de dosimetría (Hp(10)).
     
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal? ValorHp3 { get; set; } // Valor de dosimetría (Hp(10)).

        [Required]
            [ForeignKey("Sede")]
            public int SedeId { get; set; } // Relación con la tabla de sedes.

            // Relaciones de navegación
            public virtual Empleado Empleado { get; set; }
            public virtual Sede Sede { get; set; }
        }
   
}