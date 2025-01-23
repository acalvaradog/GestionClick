using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class unionpersonalapoyo
    {

         public List<DetallePersonalSalud> ListadoPersonal { get; set; }
         public List<DetalleApoyoLogisticoSalud> ListadoApoyo { get; set; }
    }
}