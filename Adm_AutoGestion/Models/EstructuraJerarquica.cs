using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class EstructuraJerarquica
    {
        public int Id { get; set; }
        [Required]
        public string Area { get; set; }
        [StringLength(8)]
        public string Lider { get; set; }
        [StringLength(8)]
        public string Superior { get; set; }
        [Required]
        [StringLength(8)]
        public string Jefe { get; set; }
        [Required]
        [StringLength(8)]
        public string Director { get; set; }
        public string Sociedad { get; set; }
        public string UnidadOrg { get; set; }
        [NotMapped]
        public Empleado NSuperior { get; set; }
        [NotMapped]
        public Empleado NJefe { get; set; }
        [NotMapped]
        public Empleado NDirector { get; set; }
        [NotMapped]
        public Empleado NLider { get; set; }
        //CAMPOS DE AUDITORIA
        public DateTime? A_Modificacion { get; set; }
        public string A_UsuarioModifica { get; set; }
        public DateTime? A_Creacion { get; set; }
        public string A_UsuarioCreador { get; set; }


    }
}