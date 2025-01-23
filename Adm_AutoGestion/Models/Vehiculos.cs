using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Vehiculos
    {
        [Key]
        public int ID { get; set; }
        public string Nombre { get; set; }
        public int Valor { get; set; } 
        public string Estado { get; set; }  
    }
}