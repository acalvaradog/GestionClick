using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("cargos x competencias")]
    public class cargos_x_competencias
    {
        [Key]
        public int codigo { get; set; }

        [Column("codigo cargo")]
         public int codigocargo { get; set; }
        [Column("codigo competencia")]
        public int codigocompetencia { get; set; }
        [Column("codigo nivel")]
        public int codigonivel { get; set; }
    }
}