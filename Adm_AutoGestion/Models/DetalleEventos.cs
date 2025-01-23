using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adm_AutoGestion.Models
{
    public class DetalleEventos
    {
        public int Id { get; set; }
        public int EventosId { get; set; }
        [ForeignKey("EventosId")]
        public Eventos Eventos { get; set; }
        public int EmpleadoId { get; set; }
        public string FamiliarId { get; set; }
        public string FechaFirma { get; set; }
        [NotMapped]
        public Familiar Familiar { get; set; }
        [NotMapped]
        public Empleado Empleado { get; set; }
        public string Observaciones { get; set; }

    }
}
