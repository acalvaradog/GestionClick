using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adm_AutoGestion.Models
{
    public class Familiar
    {
        public int Id { get; set; }
        [Required]
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        [Required]
        public string PrimerApellido { get; set; }

        public string SegundoApellido { get; set; }
        [Required]
        public string TipoDocumento { get; set; }
        [Required]
        public string Documento { get; set; }
        [Required]
        public string Parentesco { get; set; }
        [Required]
        public DateTime FechaNacimiento { get; set; }
        [Required]
        public int Edad { get; set; }
        public string NroEmpleado { get; set; }
        public int EmpleadoId { get; set; }
    }
}
