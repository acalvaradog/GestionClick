using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("Estructura_Jerarquica_EVADES")]
    public class Estructura_Jerarquica_EVADES
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Area { get; set; }
        public string Superior { get; set; }
        public string Jefe { get; set; }
        public string Director { get; set; }
        public string Sociedad { get; set; }
        [NotMapped]
        public Empleado DSuperior { get; set; }
        [NotMapped]
        public Empleado DJefe { get; set; }
        [NotMapped]
        public Empleado DDirector { get; set; }
        public string UnidadOrg { get; set; }
        //CAMPOS DE AUDITORIA
        public DateTime? A_Modificacion { get; set; }
        public string A_UsuarioModifica { get; set; }
        public DateTime? A_Creacion { get; set; }
        public string A_UsuarioCreador { get; set; }
    }
}