using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Adm_AutoGestion.Models
{
    public class TemaPregunta
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Tema { get; set; }
    }
}