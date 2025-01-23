using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("Temp")]
    public class Temp
    {
        [Key]
        public int Codigo { get; set; }
        [Required]
        [Column("Tipo identificacion")]
        public int Tipoidentificacion { get; set; }
        [Required]
        [StringLength(20)]
        public string Identificacion { get; set; }
        [Required]
        [StringLength(255)]
        public string Nombres { get; set; }
        [Required]
        [StringLength(255)]
        public string Apellidos { get; set; }
        [StringLength(30)]
        public string Telefono { get; set; }
        [StringLength(30)]
        public string Movil { get; set; }
        [StringLength(255)]
        public string Correo { get; set; }
        public int Cargo { get; set; }
        public int Area { get; set; }
        public int Sociedad { get; set; }
        public int Jefe { get; set; }
    }
}