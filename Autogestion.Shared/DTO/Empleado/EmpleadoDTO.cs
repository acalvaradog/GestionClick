using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Empleado
{
    public class EmpleadoDTO
    {

        public int? Id { get; set; }


        public string? Documento { get; set; }

        public string? NroEmpleado { get; set; }

        public string? Nombres { get; set; }

        public string? Correo { get; set; }

        public string? Empresa { get; set; }
        public string? Area { get; set; }
        public string? Telefono { get; set; }

        public string? Lider { get; set; }

        public string? Superior { get; set; }

        public string? Jefe { get; set; }
        public string? Contraseña { get; set; }
        public string? Director { get; set; }
        public string? Activo { get; set; }
        public string? UnidadOrganizativa { get; set; }
        public string? RH { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? CorreoPersonal { get; set; }
        public string? Genero { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? AreaDescripcion { get; set; }
        public string? Cargo { get; set; }
        public string? CeCo { get; set; }
        public string? TipoArea { get; set; }
        public string? ModoTrabajo { get; set; }
        public string? ObservacionModoTrabajo { get; set; }
        public string? VacunaDosis1 { get; set; }
        public string? OtraD1 { get; set; }
        public DateTime? FechaDosis1 { get; set; }
        public string? VacunaDosis2 { get; set; }
        public string? OtraD2 { get; set; }
        public DateTime? FechaDosis2 { get; set; }
        public string? DosisRefuerzo { get; set; }
        public string? OtraR { get; set; }
        public DateTime? FechaRefuerzo { get; set; }
        public string? DosisRefuerzo2 { get; set; }
        public string? OtraR2 { get; set; }
        public DateTime? FechaRefuerzo2 { get; set; }
        public string DesplazamientosLaborales { get; set; }
        public string Direccion { get; set; }
        public string Barrio { get; set; }
        public int? MunicipioId { get; set; }
        public string Estrato { get; set; }
        public int? TipoViviendaId { get; set; }

    }
}
