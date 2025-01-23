using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("usuario perfil")]
    public class usuario_perfil
    {
        
        [Key, Column("codigo usuario"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int codigousuario { get; set; }
        [Required]
        [Column("codigo perfil")]
        public int codigoperfil { get; set; }
    }
}