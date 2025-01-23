using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class DiasFestivos
    {

        public int Id { get; set; }
        [Required]
        public DateTime festivo { get; set; }

    }
}