using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("niveles")]
    public class niveles
    {
        [Key]
        public int codigo { get; set; }
        [Required]
        public int nivel { get; set; }
        [Required]
        public string descripcion { get; set; }
        [Column("codigo competencia")]    
        public int codigocompetencia { get; set; } 
        public int estado { get; set; }
    }
}