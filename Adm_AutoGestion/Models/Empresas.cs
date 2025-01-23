using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Empresas
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
    
    }
}