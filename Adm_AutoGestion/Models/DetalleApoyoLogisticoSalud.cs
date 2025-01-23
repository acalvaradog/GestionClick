using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Adm_AutoGestion.Models
{
    public class DetalleApoyoLogisticoSalud
    {
        public int Id { get; set; }
        [Required]
        public int TipoRegistro { get; set; }
        [Required]
        public string IdTipoDocumento { get; set; }
        [ForeignKey("IdTipoDocumento")]
        public TipoDocumento TipoDocumento { get; set; }
        [Required]
        public string NumeroIdentificacion { get; set; }
        [Required]
        [StringLength(200)]
        public string PrimerApellido { get; set; }
        [StringLength(200)]
        public string SegundoApellido { get; set; }
        [Required]
        [StringLength(200)]
        public string PrimerNombre { get; set; }
        [StringLength(200)]
        public string SegundoNombre { get; set; }
        [Required]
        [StringLength(10)]
        public string CodigoMunicipio { get; set; }
        
        public string IdCargo { get; set; }
        [ForeignKey("IdCargo")]
        public CargoApoyo CargoApoyo { get; set; }
        [Required]
        [StringLength(200)]
        public string CodigoEntidad { get; set; }
        [Required]
        [StringLength(200)]
        public string NombreEntidad { get; set; }
        public string IdServicioCovid { get; set; }
        [ForeignKey("IdServicioCovid")]
        public ServiciosTHSCovid ServiciosTHSCovid { get; set; }
        public string IdAreaCovid { get; set; }
        [ForeignKey("IdAreaCovid")]
        public AreaCovid AreaCovid { get; set; }
        public string IdDedicacion { get; set; }
        [ForeignKey("IdDedicacion")]
        public TiempoDedicacion TiempoDedicacion { get; set; }
        public string IdIndicadorActualizacion { get; set; }
        [ForeignKey("IdIndicadorActualizacion")]
        public IndicadorActualizacionRegistro IndicadorActualizacionRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime? FechaModifica { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaElimina { get; set; }
        public int? UsuarioEliminaId { get; set; }
        public string Empresa { get; set; }
        [NotMapped]
        public virtual List<TipoDocumento> ListadoTipoDocumento { get; set; }
        [NotMapped]
        public virtual List<AreaCovid> ListadoAreaCovid { get; set; }
        [NotMapped]
        public virtual List<TiempoDedicacion> ListadoTiempoDedicacion { get; set; }
        [NotMapped]
        public virtual List<IndicadorActualizacionRegistro> ListadoActualizacionRegistro { get; set; }
        [NotMapped]
        public virtual List<CargoApoyo> ListadocargoApoyo { get; set; }




    }
}