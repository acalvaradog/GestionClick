using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Permisos
{
    public class MotivoPermisoDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; }
        public Boolean Activo { get; set; }
    }
}
