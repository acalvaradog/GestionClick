using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("aspectos generales")]
    public class aspectos_generales
    {
        [Key]
        public int codigo { get; set; }
        [Required]
        public string descripcion { get; set; }
        public int Estado { get; set; }
    }
}