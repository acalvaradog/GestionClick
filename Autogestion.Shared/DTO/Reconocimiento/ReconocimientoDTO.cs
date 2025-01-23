using Autogestion.Shared.DTO.Empleado;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Reconocimiento
{
    public class ReconocimientoDTO
    {
        public int Id { get; set; }

        public int EmpleadoId { get; set; }
        
        public int TipoReconocimientoId { get; set; }
        
        public string? Fuente { get; set; }
        public DateTime? Fecha { get; set; } = Convert.ToDateTime(DateTime.Today);
        public string? Observaciones { get; set; }
        public bool Visto { get; set; }
        public bool Activo { get; set; }

        public int? EmpleadoReconocidoId { get; set; }
        
        public int? EmpleadoModificaId { get; set; }
        public string? EmpleadoSeleccionado { get; set; }
        
        public string? TipoNombre { get; set; }
        
        public string? TipoTexto { get; set; }
        
        public string? TipoImagen { get; set; }
        public string? EmpleadoNombre { get; set; }
        public string? EmpReco { get; set; }

    }
}
