using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Municipio
    {
       public int Id { get; set; }
       public string Nombre { get; set; }
       public string CodigoMunicipio { get; set; }
       public int DepartamentoId { get; set; }
       [ForeignKey("DepartamentoId")]
       public Departamento Departamento { get; set; }
       public string Estado { get; set; }
        
    }
}