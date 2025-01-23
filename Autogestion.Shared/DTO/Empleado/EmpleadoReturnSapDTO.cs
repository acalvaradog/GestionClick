using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Empleado
{
    public class EmpleadoReturnSapDTO
    {
        public string PERNR { get; set; }
        public string PERSG { get; set; }
        public string ENDDA { get; set; }
        public string BUKRS { get; set; }
        public string ENAME { get; set; }
        public string CARGO { get; set; }
        public string PLANS { get; set; }
        public string ORGEH { get; set; }
        public string AREA { get; set; }
        public string EPS { get; set; }
        public string RH { get; set; }
        public string GESCH { get; set; }
        public string GBDAT { get; set; }
        public string DOCUMENTO { get; set; }
        public string Foto { get; set; }
        public int? Id { get; set; }
        public string Correo { get; set; }
        public string CorreoPersonal { get; set; }
        public string Telefono { get; set; }
        public string DesplazamientosLaborales { get; set; }
        public string Direccion { get; set; }
        public string Barrio { get; set; }
        public int? MunicipioId { get; set; }
        public string Estrato { get; set; }
        public int? TipoViviendaId { get; set; }


    }
}
