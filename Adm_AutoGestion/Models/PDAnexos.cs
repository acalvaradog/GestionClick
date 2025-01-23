using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class PDAnexos
    {
        public int Id {get; set;}
        [Required]
        public int IdProcesoDisciplinario { get; set; }
        [ForeignKey("IdProcesoDisciplinario")]
        public ProcesoDisciplinario ProcesoDisciplinario { get; set; }
        [Required]
        public string Archivo { get; set; }
  
    }
}