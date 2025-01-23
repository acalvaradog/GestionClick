using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Eventos
{
    public class EventosDTO
    {
        public int Id { get; set; }
        public string NombreEvento { get; set; }
        public string TipoEvento { get; set; }
        public string DirigidoA { get; set; }
        public int? Cupo { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public DateTime? HoraInicio { get; set; }
        public DateTime? HoraFin { get; set; }
        public DateTime? FechaCierre { get; set; }
        public DateTime? HoraCierre { get; set; }
        public bool? RegistroRequerido { get; set; }
        public string LinkEncuestaAsistidos { get; set; }
        public string LinkEncuestaNoAsistidos { get; set; }
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        public string Estado { get; set; }
        public string ParentescoPermitido { get; set; }
        public int? EdadLimite { get; set; }
        public bool? Inscrito {  get; set; }
        public bool? EmpInscrito { get; set; }
        public bool? EsEventoPrincipal { get; set; }
        public int? EventoRelacionado { get; set; }
        public bool? EstaInscritoOtraFecha { get; set; }

    }
}
