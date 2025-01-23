using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("empleados")]
    public class empleados
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int codigo { get; set; }
        [Required]
        [Column("tipo identificacion")]
        public int tipoidentificacion {get; set; }
        [Required]
        [StringLength(20)]
        public string identificacion { get; set; }
        [Required]
        [StringLength(255)]
        public string nombres { get; set; }
        [Required]
        [StringLength(255)]
        public string apellidos { get; set; }
        [StringLength(250)]
        public string telefono { get; set;}
        [StringLength(30)]
        public string movil { get; set; }
        [StringLength(255)]
        public string email { get; set; }
        [Column("codigo cargo")]
        public int codigocargo { get; set; }
        [Column("codigo area")]
        public int codigoarea { get; set; }
        public int estado { get; set; }
        [StringLength(100)]
        public string clave { get; set; }


    }
}