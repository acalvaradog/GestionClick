using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Capacitacion
    {
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Nombre { get; set; }
        [Required]
        public DateTime FechaCapacitacion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public int EmpleadoRegistraId { get; set; }
        [ForeignKey("EmpleadoRegistraId")]
        public Empleado Empleado { get; set; }
        public string temas { get; set; }
        public string Empresa { get; set; }
        public int IdSede { get; set; }
        [ForeignKey("IdSede")]
        public Sede Sede { get; set; }
        public string Objetivo { get; set; }
        public int IdDirigidoA { get; set; }
        //[ForeignKey("IdDirigidoA")]
        //public DirigidoA DirigidoA { get; set; }
        public string Ciudad { get; set; }
        public string Lugar { get; set; }
        public int Metodologia { get; set; }
        public int CompetenciaLaboral { get; set; }
        public string Conocimientos { get; set; }
        public int CulturaOrganizacional { get; set; }
        public string MedicionEficacion { get; set; }
        public string MetaEficacia { get; set; }
        public string ResultadoMedicion { get; set; }
        public int Metodologia2 { get; set; }
        public string Responsable { get; set; }
        public string CargoResponsable { get; set; }
        public string ResponsablePrograma { get;set; }
        public string CargoResponsablePrograma { get; set; }
        public string AreaObjetivo { get;set; }
        public int TotalSesiones { get; set; }
        public int TotalPersonas { get; set; }
        public string TotalHoras { get; set; }
        public string Proveedor { get; set; }
        public string EspecificacionReq { get; set; }
        public bool PresupuestoRequerido { get; set; }
        public string Presupuesto { get; set; }
        public string Estado { get; set; }
        public string Cobertura { get; set; }
        [NotMapped]
        public string PorcentajeCobertura { get; set; }
        [NotMapped]
        public string Dependencias { get; set; }
        public int CtnAsistentes { get; set; }
        public int CtnProgramados { get; set; }
        public int IdTipoPEC { get; set; }
        //[ForeignKey("IdTipoPEC")]
        //public TipoPEC TipoPEC { get; set; }
        public int IdProgramaInstitucional { get; set; }
        //[ForeignKey("IdProgramaInstitucional")]
        //public ProgramaInstitucional ProgramaInstitucional { get; set; }
        public int IdRequerimientoInstitucional { get; set; }
        //[ForeignKey("IdRequerimientoInstitucional")]
        //public RequerimientoInstitucional RequerimientoInstitucional { get; set; }
        [NotMapped]
        public virtual List<Expositor> Expositores { get; set; }
        [NotMapped]
        public virtual List<DetalleCapacitacion> Participantes { get; set; }
        [NotMapped]
        public virtual Metodologia MetodologiaC { get; set; }
        [NotMapped]
        public virtual Sociedad Empresa2 { get; set; }
        [NotMapped]
        public virtual Metodologia MetodologiaCierre { get; set; }
        [NotMapped]
        public virtual CulturaOrganizacional CulturaO { get; set; }
        [NotMapped]
        public virtual CompetenciaLaboral CompetenciaL { get; set; }
        [NotMapped]
        public virtual List<Sede> ListSede { get; set; }
        [NotMapped]
        public virtual List<Sociedad> ListEmpresa { get; set; }
        [NotMapped]
        public virtual List<Metodologia> ListMetodología { get; set; }
        [NotMapped]
        public string DirigidoA { get; set; }
        [NotMapped]
        public string TipoPEC { get; set; }
        [NotMapped]
        public string ProgramaInstitucional { get; set; }
        [NotMapped]
        public string RequerimientoInstitucional { get; set; }
        public string EvaluacionConocimiento {  get; set; }
        public DateTime Mes {  get; set; }
        public string EncuestaSatisfaccion {  get; set; }
        public string DirigidoASelect { get; set; }
        public string Modalidad { get; set; }
        [NotMapped]
        public string Modalidad2 { get;set; }
        public string CursoId { get; set; }
        public Guid IdentificadorRelacion { get; set; }
        public string Docente { get; set; }
        public int IdCursoNormativo { get; set; }
    }
}