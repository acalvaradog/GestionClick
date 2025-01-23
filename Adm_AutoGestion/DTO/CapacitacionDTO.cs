using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.TalentoHumano
{
    public class CapacitacionDTO
    {
        public int? Id { get; set; }
        public string Nombre { get; set; }
        public DateTime? FechaCapacitacion { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string temas { get; set; }
        public string Objetivo { get; set; }
        public string Modalidad { get; set; }
        public string Ciudad { get; set; }
        public string Lugar { get; set; }
        public string Responsable { get; set; }
        public string ResponsablePrograma { get; set; }
        public string Docente { get; set; }
        public string AreaObjetivo { get; set; }
        public string Proveedor { get; set; }
        public string Estado { get; set; }
        public string RequerimientoInstitucional { get; set; }
        public string EvaluacionConocimiento { get; set; }
        public string EncuestaSatisfaccion { get; set; }
        public string Asistencia { get; set; }

    }
}
