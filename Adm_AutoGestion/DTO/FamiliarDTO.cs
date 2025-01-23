using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Eventos
{
    public class FamiliarDTO
    {
        public int Id { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string TipoDocumento { get; set; }
        public string Documento { get; set; }
        public string Parentesco { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int Edad { get; set; }
        public string NroEmpleado { get; set; }
        public int? EmpleadoId { get; set; }
        public bool? Inscrito { get; set; }
        public int? EventoRelacionado { get; set; }
        public bool? EstaInscritoOtraFecha { get; set; }
    }
}
