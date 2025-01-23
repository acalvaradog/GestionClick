using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class HistoricoHorasExtra
    {
        public int Id { get; set; }
        public int HorasExtraId { get; set; }
        [ForeignKey("HorasExtraId")]
        public HorasExtra HorasExtra { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string EstadoNombre { get; set; }
        public string Observaciones { get; set; }
        public string UsuarioModifica { get; set; }
      
        [NotMapped]
        public Empleado Empleado { get; set; }
        [NotMapped]
        public EstadosHorasExtra EstadosHorasExtra { get; set; }

    }
}