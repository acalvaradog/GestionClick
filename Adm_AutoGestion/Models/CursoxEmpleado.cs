using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class CursoxEmpleado
    {
        public int Id { get; set; }
        public int CursoId { get; set; }
        public int EmpleadoId { get; set; }
        public int Progreso { get; set; }
    }
}