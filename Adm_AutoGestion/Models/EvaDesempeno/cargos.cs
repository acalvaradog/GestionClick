using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("cargos")]
    public class cargos
    {
        [Key]
        public int codigo { get; set; }
        [Required]
        [StringLength(130)]
        public string descripcion { get; set; }
        public bool completo { get; set; }
        [StringLength(30)]
        public string CodigoSAP { get; set; }
    }
}