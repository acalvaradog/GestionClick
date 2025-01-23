using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adm_AutoGestion.Models
{
    public class Eventos
    {
        public int Id { get; set; }
        [Required]
        public string NombreEvento { get; set; }
        [Required]
        public string TipoEvento { get; set; }
        [Required]
        public string DirigidoA { get; set; }
        public int Cupo { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime? HoraFin { get; set; }
        public DateTime? FechaCierre { get; set; }
        public DateTime? HoraCierre { get; set; }
        public bool RegistroRequerido { get; set; }
        public string LinkEncuestaAsistidos { get; set; }
        public string LinkEncuestaNoAsistidos { get; set; }
        [Required]
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        public string Estado { get; set; }
        public string Presupuesto { get; set; }
        public string Mes { get; set; }
        [NotMapped]
        public int Inscritos { get; set; }
        [NotMapped]
        public int Asistentes { get; set; }
        [NotMapped]
        public float Indicador { get; set; }
        public string ParentescoPermitido { get;set;}
        public int EdadLimite { get; set; }
        public bool EsEventoPrincipal { get; set; }
        public bool EncuestaEnviada { get; set; }
    }
}