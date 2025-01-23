using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class competencias
    {
        [Key]
        public int codigo { get; set; }
        [StringLength(255)]
        public string titulo { get; set; }
        public string descripcion { get; set; }
        [Column("tipo competencia")]
        public int tipocompetencia { get; set; }
        public int estado { get; set; }
        public int dominio { get; set; }

    }
}