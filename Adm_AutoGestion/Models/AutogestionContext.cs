using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Adm_AutoGestion.Models
{
    public class AutogestionContext: DbContext
    {
        public AutogestionContext()
            :base("Conexion")
        {
 
        }

        //para poder hacerle querys al modelo, se indica al frente el nombre de la tabla que a crear

        public DbSet<Noticia> Noticias { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<EstadoPermiso> EstadosPermiso { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<MotivoPermiso> MotivosPermiso { get; set; }
        public DbSet<Eventos> Eventos { get; set; }
        public DbSet<PreguntaFrecuente> PreguntasFrecuentes { get; set; }
        public DbSet<Reconocimiento> Reconocimientos { get; set; }
        public DbSet<TemaPregunta> TemasPreguntas { get; set; }
        public DbSet<Preguntas> Preguntas { get; set; }
        public DbSet<Encuesta> Encuesta { get; set; }
        public DbSet<EncabezadoEncuesta> EncabezadoEncuesta { get; set; }
        public DbSet<GrupoEmpleados> GrupoEmpleados { get; set; }
        public DbSet<Rol> Rol { get; set; }
        public DbSet<Perfil> Perfil { get; set; }
        public DbSet<EstadoVacaciones> EstadoVacaciones { get; set; }
        public DbSet<Vacaciones> Vacaciones { get; set; }
        public DbSet<HistorialVacaciones> HistorialVacaciones { get; set; }
        public DbSet<SeguimientoSintomas> SeguimientoSintomas { get; set; }
        public DbSet<PersonalActivo> PersonalActivo { get; set; }
        public DbSet<Configuraciones> Configuraciones { get; set; }
        public DbSet<Incapacidades> Incapacidades { get; set; }
        public DbSet<IncapacidadAdjuntos> IncapacidadAdjuntos { get; set; }
        public DbSet<TiposIncapacidad> TiposIncapacidad { get; set; }
        public DbSet<HistorialIncapacidades> HistorialIncapacidades { get; set; }
        public DbSet<HabilitarVacaciones> HabilitarVacaciones { get; set; }
        public DbSet<Motivos> Motivos { get; set; }
        public DbSet<Retiros> Retiros { get; set; }
        public DbSet<EstructuraJerarquica> EstructuraJerarquica { get; set; }
        public DbSet<DiasFestivos> DiasFestivos { get; set; }
        public DbSet<Diagnostico> Diagnostico { get; set; }
        public DbSet<Adjunto> Adjunto { get; set; }
        public DbSet<Dispositivos> Dispositivos { get; set; }
        public DbSet<EntregaEPP> EntregaEPP { get; set; }
        public DbSet<DetalleEntregaEPP> DetalleEntregaEPP { get; set; }
        public DbSet<ElementosProtecionPersonal> ElementosProtecionPersonal { get; set; }
        public DbSet<Sociedad> Sociedad { get; set; }
        public DbSet<CentrodeCostos> CentrodeCostos { get; set; }
        public DbSet<UnidadOrganizativa> UnidadOrganizativa { get; set; }
        public DbSet<AdjuntoPermisos> AdjuntoPermisos { get; set; }
        public DbSet<PermisosAdjuntos> PermisosAdjuntos { get; set; }
        public DbSet<TopeDescuento> TopeDescuento { get; set; }
        public DbSet<Descuentos> Descuentos { get; set; }
        public DbSet<Servicios> Servicios { get; set; }
        public DbSet<DescuentosReportados> DescuentosReportados { get; set; }
        public DbSet<EPS> EPS { get; set; }
        public DbSet<TipoElementos> TipoElementos { get; set; }
        public DbSet<EncabezadoPrestamo> EncabezadoPrestamo { get; set; }
        public DbSet<DetalleEncabezadoPrestamo> DetalleEncabezadoPrestamo { get; set; }
        public DbSet<THSPerfil> THSPerfil { get; set; }
        public DbSet<TipoDocumento> TipoDocumento { get; set; }
        public DbSet<AreaCovid> AreaCovid { get; set; }
        public DbSet<ServiciosTHSCovid> ServiciosTHSCovid { get; set; }
        public DbSet<CargoAsistencial> CargoAsistencial { get; set; }
        public DbSet<CargoApoyo> CargoApoyo { get; set; }
        public DbSet<RegistroControlVacunacion> RegistroControlVacunacion { get; set; }
        public DbSet<TiempoDedicacion> TiempoDedicacion { get; set; }
        public DbSet<IndicadorActualizacionRegistro> IndicadorActualizacionRegistro { get; set; }
        public DbSet<DetallePersonalSalud> DetallePersonalSalud { get; set; }
        public DbSet<DetalleApoyoLogisticoSalud> DetalleApoyoLogisticoSalud { get; set; }
        public DbSet<ProcesoDisciplinario> ProcesoDisciplinario { get; set; }
        public DbSet<PDTrabajador> PDTrabajador { get; set; }
        public DbSet<PDPruebas> PDPruebas { get; set; }
        public DbSet<PDAnexos> PDAnexos { get; set; }
        public DbSet<PazySalvo> PazySalvo { get; set; }
        public DbSet<AreasPazySalvo> AreasPazySalvo { get; set; }
        public DbSet<DetallePazySalvo> DetallePazySalvo { get; set; }
        public DbSet<Tercero> Tercero { get; set; }
        public DbSet<LugarEntrega> LugarEntrega { get; set; }
        public DbSet<TurnoDisponibilidad> TurnoDisponibilidad { get; set; }
        public DbSet<NotasProcesos> NotasProcesos { get; set; }
        public DbSet<Capacitacion> Capacitacion { get; set; }
        public DbSet<DetalleCapacitacion> DetalleCapacitacion { get; set; }
        public DbSet<Sede> Sede { get; set; }
        public DbSet<CompetenciaLaboral> CompetenciaLaboral { get; set; }
        public DbSet<CulturaOrganizacional> CulturaOrganizacional { get; set; }
        public DbSet<Metodologia> Metodologia { get; set; }
        public DbSet<Expositor> Expositor { get; set; }
        public DbSet<MotivoEgreso> MotivoEgreso { get; set; }
        public DbSet<Jornada> Jornada { get; set; }
        public DbSet<EstadoRdP> EstadoRdP { get; set; }
        public DbSet<EstadoSeleccionRdP> EstadoSeleccionRdP { get; set; }
        public DbSet<Horario> Horario { get; set; }
        public DbSet<MtvSolicitud> MtvSolicitud { get; set; }
        public DbSet<RequerimientosDelPersonal> RequerimientosDelPersonal { get; set; }
        public DbSet<Aprobacioneslog> Aprobacioneslog { get; set; }
        public DbSet<Direcciones> Direcciones { get; set; }
        public DbSet<DetalleRequerimiento> DetalleRequerimiento { get; set; }
        public DbSet<TipoReunion> TipoReunion { get; set; }
        public DbSet<Reunion> Reunion { get; set; }
        public DbSet<EstadoReunion> EstadosReuniones { get; set; }
        public DbSet<DetalleReunion> DetalleReunion { get; set; }
        public DbSet<Compromisos> Compromisos { get; set; }
        public DbSet<HistorialPermisos> HistorialPermisos { get; set; }
        public DbSet<Familiar> Familiar { get; set; }
        public DbSet<DetalleEventos> DetalleEventos { get; set; }
        public DbSet<EstadosViaticos> EstadosViaticos { get; set; }
        public DbSet<DestinoViatico> DestinoViaticos { get; set; }
        public DbSet<Viaticos> Viaticos { get; set; }
        public DbSet<ViaticosLog> ViaticosLogs { get; set; }
        public DbSet<Vehiculos> Vehiculos { get; set; }
        public DbSet<EventosRelacionados> EventosRelacionados { get; set;}
        public DbSet<Dotacion> Dotacion { get; set;}
        public DbSet<HistorialDotacion> HistorialDotacion { get; set;}
        public DbSet<SoportesHojaDeVida> SoportesHojaDeVida { get; set; }
        public DbSet<TipoSoporte> TipoSoporte { get; set; }
        public DbSet<Notificacion> Notificacion { get; set; }
        public DbSet<Curso> Curso { get; set; }
        public DbSet<CursoxEmpleado> CursoxEmpleado { get; set; }
        public DbSet<RequerimientoInstitucional> RequerimientoInstitucional { get;set; }
        public DbSet<ProgramaInstitucional> ProgramaInstitucional { get; set; }
        public DbSet<DirigidoA> DirigidoA { get; set; }
        public DbSet<TipoPEC> TipoPEC { get; set; }
        public DbSet<HorasExtra> HorasExtra { get; set; }
        public DbSet<MotivoTrabajoHE> MotivoTrabajoHE { get; set; }
        public DbSet<DetalleHorasExtra> DetalleHorasExtras { get; set; }
        public DbSet<EstadosHorasExtra> EstadosHorasExtra { get; set; }
        public DbSet<TipoVivienda> TipoVivienda { get; set; }
        public DbSet<Municipio> Municipio { get; set; }
        public DbSet<Departamento> Departamento { get; set; }
        public DbSet<HistoricoHorasExtra> HistoricoHorasExtra { get;set; }
        public DbSet<TipoReconocimiento> TipoReconocimiento { get; set; }
        public DbSet<EstadosIncapacidades> EstadosIncapacidades { get; set; }
        public DbSet<Certificados> Certificados { get; set; }
        public DbSet<CursosNormativa> CursosNormativa { get; set; }
        public DbSet<Empresas> Empresas { get; set; }
        public DbSet<LugaresCapacitacion> LugaresCapacitacion { get; set; }


        public DbSet <ChatMessage> ChatMessage { get; set; }
        public DbSet<Dosimetria> Dosimetria { get; set; }

        public DbSet<SoporteCesantia> SoporteCesantia { get; set; }
        public DbSet<SoporteDestino> SoporteDestino { get; set; }
        public DbSet<DestinoCesantia> DestinoCesantia { get; set; }
        public DbSet<SolicitudCesantia> SolicitudCesantia { get; set; }
        public DbSet<LogSolicitudCesantia> LogSolicitudCesantia { get; set; }
        public DbSet<EstadoCesantia> EstadoCesantia { get; set; }
        public DbSet<NoticiaImagen> NoticiaImagen { get; set; }
        public DbSet<FondoCesantias> FondoCesantias { get; set; }
    }
}