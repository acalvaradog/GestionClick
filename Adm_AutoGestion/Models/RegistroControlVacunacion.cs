using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adm_AutoGestion.Models
{
    public class RegistroControlVacunacion
    {
        public int Id { get; set; }
        [Required]
        public int TipoRegistro { get; set; }
        [Required]
        public string TipoIdentificacionEntidad { get; set; }
        [Required]
        public string NumeroIdentificacionEntidad { get; set; }
        [Required]
        public DateTime FechaInicialReportada { get; set; }
        [Required]
        public DateTime FechaFinalReportada { get; set; }
        [Required]
        public int TotalRegistros { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaModifica { get; set; }
        public int UsuarioModificaId { get; set; }
        public string TipoModificacion { get; set; }

        
    }
}