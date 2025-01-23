using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Configuraciones
    {
        public int Id { get; set; }
        public string Parametro { get; set; }
        public string Descripcion { get; set; }
        public string Valor { get; set; }
    }
}