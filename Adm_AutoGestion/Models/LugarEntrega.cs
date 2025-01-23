using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace Adm_AutoGestion.Models
{
    public class LugarEntrega
    {
        public int Id { get; set; }
        [Required]
        public String Descripcion { get; set; }


    }
}