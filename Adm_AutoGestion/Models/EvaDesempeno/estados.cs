using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("estados")]
    public class estados
    {

        [Key]
        public int codigo { get; set; }
        [Required]
        [StringLength(255)]
        public string descripcion { get; set; }
    }
}