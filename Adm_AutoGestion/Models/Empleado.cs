using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace Adm_AutoGestion.Models
{
    public class Empleado
    {
        public int Id { get; set; }
        [Required]
        [StringLength(20)]
        public string Documento { get; set; }
        [Required]
        [StringLength(20)]
        public string NroEmpleado { get; set; }
        [Required]
        [StringLength(200)]
        public string Nombres { get; set; }
        [StringLength(100)]
        public string Correo { get; set; }
        [Required]
        [StringLength(4)]
        public string Empresa { get; set; }
        public string Area { get; set; }
        public int CodigoSAPArea { get; set; }  
        public string Telefono { get; set; }
        [StringLength(8)]
        public string Lider { get; set; }
        [StringLength(8)]
        public string Superior { get; set; }
        [StringLength(8)]
        public string Jefe { get; set; }
        [StringLength(30)]
        public string Contraseña { get; set; }
        [StringLength(8)]
        public string Director { get; set; }
        public string Activo { get; set; }
        public string UnidadOrganizativa { get; set; }
        public string RH { get; set; }
        public DateTime? FechaFin { get; set; }
        public string CorreoPersonal { get; set; }
        public string Genero { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string AreaDescripcion { get; set; }
        public string Cargo { get; set; }
        public int CodigoSAPCargo { get; set; }
        public string CeCo { get; set; }
        public string TipoArea { get; set; }
        public string ModoTrabajo { get; set; }
        public string ObservacionModoTrabajo { get; set; }
        public string VacunaDosis1 { get; set; }
        public string OtraD1 { get; set; }
        public DateTime? FechaDosis1 { get; set; }
        public string VacunaDosis2 { get; set; }
        public string OtraD2 { get; set; }
        public DateTime? FechaDosis2 { get; set; }
        public string DosisRefuerzo { get; set; }
        public string OtraR { get; set; }
        public DateTime? FechaRefuerzo { get; set; }
        public string DosisRefuerzo2 { get; set; }
        public string OtraR2 { get; set; }
        public DateTime? FechaRefuerzo2 { get; set; }
        public string CategoriaDotacion { get; set; }
        public string RequiereDesplazamiento { get; set; }
        public string DesplazamientosLaborales { get; set; }
        public string Direccion { get; set; }
        public string Barrio { get; set; }
        public int? MunicipioId { get; set; }
        [ForeignKey("MunicipioId")]
        public Municipio Municipio { get; set; }
        public string Estrato { get; set; }      
        public int? TipoViviendaId { get; set; }
        [ForeignKey("TipoViviendaId")]
        public TipoVivienda TipoVivienda { get; set; }
        //CAMPOS DE AUDITORIA
        public DateTime? A_Modificacion { get; set; }
        public string A_UsuarioModifica { get; set; }
        public DateTime? A_Creacion { get; set; }
        public string A_UsuarioCreador { get; set; }
        //CAMPOS DATOS SAP
        public string EstadoCivil { get; set; }
        public string EPS { get; set; }
        public bool Externo { get; set; }

    }
}