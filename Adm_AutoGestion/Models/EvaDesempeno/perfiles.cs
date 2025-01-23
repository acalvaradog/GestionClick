using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("perfiles")]
    public class perfiles
    {

        [Key]
        public int codigo { get; set; }
        [Required]
        public string descripcion { get; set; }
    }
}