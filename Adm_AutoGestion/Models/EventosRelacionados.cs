using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class EventosRelacionados
    {
        public int Id { get; set; }
        public int EventosId { get; set; }
        [ForeignKey("EventosId")]
        public Eventos Eventos { get; set; }
        public int EventosId2 { get; set; }
    }
}