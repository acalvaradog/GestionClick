using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adm_AutoGestion.Models
{
    public class Tercero
    {
        public int Id { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "Documento:")]
        public string Documento { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Nombres Completos:")]
        public string Nombres { get; set; }
        [Display(Name = "Fecha de Nacimiento:")]
        public DateTime? FechaNacimiento { get; set; }
        [StringLength(200)]
        [Display(Name = "Dirección:")]
        public string Direccion { get; set; }
        [Display(Name = "Telefono:")]
        public string Telefono { get; set; }
        [Display(Name = "Correo:")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
        ErrorMessage = "Dirección de Correo electrónico incorrecta.")]
        public string CorreoPersonal { get; set; }
        [Display(Name = "Area:")]
        public string Area { get; set; }
        [Display(Name = "Jefe  Inmediato:")]
        public string Superior { get; set; }
        [Display(Name = "Cargo:")]
        public string Cargo { get; set; }
        public string Activo { get; set; }
        public string Estudiante { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaFin { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime? FechaModifica { get; set; }
        public int? UsuarioModificaId { get; set; }

        public string QRPrestamo { get; set; }  
        
        public string SociedadCOD { get; set; }
        public string Universidad { get; set; }

    }
}