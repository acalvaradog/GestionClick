using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("areas")]
    public class areas
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 0), ]
        public int codigo { get; set; }

        [StringLength(130)]
        public string descripcion { get; set; }
        [StringLength(30)]
        public string CodigoSAP { get; set; }
        [StringLength(8)]
        public string Sociedad { get; set; }
    }
}