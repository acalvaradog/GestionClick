using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Perfil
    {
        public int Id { get; set; }
        public int GrupoId { get; set; }
        [ForeignKey("GrupoId")]
        public GrupoEmpleados GrupoEmpleados { get; set; }
        public string Funcion { get; set; }
        [NotMapped]
        public virtual List<GrupoEmpleados> ListadoGrupos { get; set; }
    }
}