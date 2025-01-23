using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adm_AutoGestion.Models
{
    public class ViaticosLog
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; }
        [NotMapped]
        public virtual Empleado Empleado { get; set; }

        public int ValorAnterior { get; set; }
        public int ValorNuevo { get; set; }

        public string Observaciones { get; set; }

        public int ViaticoId { get; set; }
        [ForeignKey("ViaticoId")]
        public Viaticos Viaticos { get; set; }

        public string Estado { get; set; }
        


    }
}
