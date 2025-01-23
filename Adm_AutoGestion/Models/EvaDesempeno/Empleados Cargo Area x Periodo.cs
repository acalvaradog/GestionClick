using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Windows.Controls;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("Empleados Cargo Area x Periodo")]
    public class Empleados_Cargo_Area_x_Periodo
    {
        //[Key]
        //public int Id { get; set; }
        [Key, Column(Order = 1), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Periodo { get; set; }
        //[ForeignKey("Periodo")]
        //public periodos periodos { get; set; }
        [Key, Column(Order = 2), DatabaseGenerated(DatabaseGeneratedOption.None)]

        public int Empleado { get; set;}
        //[ForeignKey("Empleado")]
        //public empleados empleados { get; set; }
        public int Cargo { get; set;}   
        public int Area { get; set;}

    }
}