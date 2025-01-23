using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("Temp_Cod")]
    public class Temp_Cod
    {
        [Key]
        public int Codigo { get; set; }
    }
}