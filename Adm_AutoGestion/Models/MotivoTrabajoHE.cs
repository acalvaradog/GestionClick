using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adm_AutoGestion.Models
{
    public class MotivoTrabajoHE
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(100)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(100)]
        public string Estado { get; set; }
    }
}
