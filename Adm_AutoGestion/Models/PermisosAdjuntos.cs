using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class PermisosAdjuntos
    {
        public int Id { get; set; }
        [Required]
        public int PermisosId { get; set; }
        [ForeignKey("PermisosId")]
        public Permiso Permisos { get; set; }
        [Required]
        public string MotivoPermiso { get; set; }
        [Required]
        public string Adjunto { get; set; }

    }
}