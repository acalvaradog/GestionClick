using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class PersonalActivo
    {
        public int Id { get; set; }
        [Required]
        [StringLength(8)]
        public string CodigoEmpleado { get; set; }
        [Required]
        [StringLength(200)]
        public string Nombres { get; set; }
        [Required]
        [StringLength(20)]
        public string Documento { get; set; }
        [StringLength(200)]
        public string Cargo { get; set; }
        [StringLength(200)]
        public string Area { get; set; }
        [Required]
        [StringLength(4)]
        public string Empresa { get; set; }
        [StringLength(8)]
        public string Superior { get; set; }
        [StringLength(8)]
        public string Jefe { get; set; }
        [StringLength(8)]
        public string Lider { get; set; }
        [StringLength(8)]
        public string Director { get; set; }
        [Required]
        public string UnidadOrganizativa { get; set; }
        public string CodigoCeco { get; set; }
        public string DescripcionCeco { get; set; }
    }
}