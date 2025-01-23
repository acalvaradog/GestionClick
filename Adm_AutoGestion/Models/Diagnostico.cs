using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Diagnostico
    {

        public int Id { get; set; }
        [Required]
        [StringLength(10)]
        public string Codigo { get; set; }
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(2)]
        public string Genero { get; set; }
        [Required]
        [StringLength(2)]
        public string Limite_Inf { get; set; }
        [Required]
        [StringLength(2)]
        public string Limite_Sup { get; set; }
        [Required]
        [StringLength(20)]
        public string Diagnostico_Afpr { get; set; }
        [Required]
        public string Diagnostico_Obs { get; set; }


    }
}