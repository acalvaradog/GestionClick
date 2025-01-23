using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class PDPruebas
    {
        public int Id { get; set; }
        [Required]
        public int IdProcesoDisciplinario { get; set; }
        [ForeignKey("IdProcesoDisciplinario")]
        public ProcesoDisciplinario ProcesoDisciplinario { get; set; }
        public string Descripcion { get; set; }
        [StringLength(20)]
        [Required]
        public string TipoPrueba { get; set; }
        [Required]
        public string Adjunto { get; set; }

    }
}