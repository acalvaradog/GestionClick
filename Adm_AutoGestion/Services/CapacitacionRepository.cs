using Adm_AutoGestion.Models;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using Org.BouncyCastle.Cms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;

namespace Adm_AutoGestion.Services
{
    public class CapacitacionRepository
    {

        AutogestionContext db = new AutogestionContext();
        CertificadosContext db2 = new CertificadosContext();

        //internal void Crear(string Nombre, DateTime FechaCapacitacion, string HoraInicio, string HoraFin,string temas,string Empresa, int IdSede, int userlog)
        internal string Crear(Capacitacion model, int userlog, string[] Fechas, string Nombre)
        {

            using (var db = new AutogestionContext())
            {
                string message = null;
                try
                {
                    int count = 0;
                    var identificador = Guid.NewGuid();

                    foreach (var f in Fechas)
                    {
                        count++;
                        model.Nombre = Nombre + " - Sesion " + count;
                        model.EmpleadoRegistraId = userlog;
                        model.FechaRegistro = DateTime.Now;
                        model.Estado = "Activo";
                        model.FechaCapacitacion = DateTime.Parse(f);
                        model.IdentificadorRelacion = identificador;
                        db.Capacitacion.Add(model);
                        db.SaveChanges();
                    }

                    message = "true";

                    return message;
                }
                catch (Exception ex)
                {
                    message = String.Format("Se genero un error. {0}", ex.Message);

                    return message;
                }

            }
        }

        public string SubirArchivo(int EmpleadoId, string Titulo, string NombreArchivo, DateTime fecha)
        {
            Certificados certificados = new Certificados();

            try
            {
                certificados.Archivo = NombreArchivo;
                certificados.Titulo = Titulo;
                certificados.EmpleadoId = EmpleadoId;
                certificados.Estado = "Pendiente";
                certificados.FechaCaducidad = fecha;
                db.Certificados.Add(certificados);
                db.SaveChanges();
                return "true";

            }
            catch
            {
                return "false";
            }

        }

        public List<Capacitacion> ObtenerTodos(string usuario)
        {
            int cont = 0;
            using (var db = new AutogestionContext())
            {

                int Idemp = 0;
                //int Soc = 0;
                Int32.TryParse(usuario, out Idemp);
                List<Capacitacion> Items = db.Capacitacion.Where(e => e.EmpleadoRegistraId == Idemp && e.Estado != "Cerrado").ToList();
                //foreach (Capacitacion Item in Items)
                //{
                //    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);

                //}

                return Items;
            }
        }

        internal void Cerrar(string IdCap, string Cobertura)/*string MedicionE, string Conocimientos, string Competencia, string CulturaO, string MetaE, string ResultadoM, string Metodologia, string Cobertura, int CtnA, int CtnP*/
        {

            using (var db = new AutogestionContext())
            {
                try
                {

                    Capacitacion Capacitacion = new Capacitacion();

                    int.TryParse(IdCap, out int id);

                    Capacitacion = db.Capacitacion.FirstOrDefault(e => e.Id == id);

                    db.Capacitacion.Attach(Capacitacion);
                    Capacitacion.Estado = "Cerrado";
                    //Capacitacion.MedicionEficacion = MedicionE;
                    //Capacitacion.Conocimientos = Conocimientos;
                    //Capacitacion.CulturaOrganizacional = Convert.ToInt32(CulturaO);
                    //Capacitacion.MetaEficacia = MetaE;
                    //Capacitacion.ResultadoMedicion = ResultadoM;
                    //Capacitacion.Cobertura = Cobertura;
                    //Capacitacion.CtnAsistentes = CtnA;
                    //Capacitacion.CtnProgramados = CtnP;
                    //--------------------------//
                    //if (Metodologia != "")
                    //{ Capacitacion.Metodologia2 = Convert.ToInt32(Metodologia); };
                    ////--------------------------//
                    //Capacitacion.CompetenciaLaboral = Convert.ToInt32(Competencia);

                    var relacion = db.Capacitacion.FirstOrDefault(x => x.Id == id)?.IdentificadorRelacion;
                    var capP = db.Capacitacion.FirstOrDefault(x => x.IdentificadorRelacion == relacion);

                    capP.Cobertura = Cobertura;

                    db.SaveChanges();

                }
                catch
                {
                }
            }

        }

        internal void CrearExpositor(List<Expositor> model)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    Expositor detalle = new Expositor();

                    foreach (Expositor Item in model)
                    {
                        db.Expositor.Add(Item);
                        db.SaveChanges();
                    }


                }
                catch
                {
                }
            }

        }
        //----------//

        public string ValidarRegistroTercero(string cedula, int IdCap)
        {
            try
            {
                var Ter = db.Tercero.Any(x => x.Documento == cedula);

                if (Ter)
                {
                    var terceroId = db.Tercero.FirstOrDefault(x => x.Documento == cedula)?.Id.ToString();

                    if (db.DetalleCapacitacion.Any(x => x.CapacitacionId == IdCap && x.TerceroId == terceroId))
                    {
                        return "Ya estás inscrito a esta sesión";
                    }

                    var detalle = new DetalleCapacitacion();
                    var tercero = db.Tercero.FirstOrDefault(x => x.Documento == cedula);

                    detalle.Estado = "Activo";
                    if (tercero.Estudiante == "SI")
                    {
                        detalle.EsTercero = "ESTUDIANTE";
                    }
                    else
                    {
                        detalle.EsTercero = "SI";
                    }
                    detalle.TerceroId = terceroId;
                    detalle.Cargo = tercero.Cargo;
                    detalle.Area = tercero.Area;
                    detalle.CapacitacionId = IdCap;
                    //detalle.SedeId = int.Parse(tercero.SociedadCOD);

                    db.DetalleCapacitacion.Add(detalle);

                    //var relacion = db.Capacitacion.FirstOrDefault(x => x.Id == IdCap)?.IdentificadorRelacion;
                    //var capP = db.Capacitacion.FirstOrDefault(x => x.IdentificadorRelacion == relacion);
                    //capP.TotalPersonas++;

                    var cap = db.Capacitacion.Find(IdCap);
                    cap.CtnProgramados++;

                    db.SaveChanges();

                    if (cap.Metodologia == 2 || cap.Metodologia == 3 || cap.Metodologia == 4 || cap.Metodologia == 9)
                    {
                        return FirmaEmpleado(cedula, IdCap, "Tercero");
                    }

                    return "true";

                }
                else
                {
                    return "false";
                }

            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public string CrearTercero(Tercero model, int IdCap)
        {
            string message;
            try
            {
                if (!db.Tercero.Any(x => x.Documento == model.Documento))
                {
                    model.Activo = "SI";
                    db.Tercero.Add(model);
                    db.SaveChanges();

                    message = ValidarRegistroTercero(model.Documento, IdCap);
                }
                else
                {
                    message = "El Tercero ya existe.";
                }

                return message;
            }
            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);

                return message;
            }
        }

        public string FirmaEmpleado(string cedula, int IdCap, string TipoPersona)
        {
            try
            {
                if (TipoPersona == "Empleado")
                {
                    var Emp = db.Empleados.Any(x => x.Documento == cedula);

                    if (Emp)
                    {

                        var empleado = db.Empleados.FirstOrDefault(x => x.Documento == cedula);

                        var registro = db.DetalleCapacitacion.FirstOrDefault(x => x.CapacitacionId == IdCap && x.EmpleadoId == empleado.Id);

                        if (registro == null)
                        {
                            return "El empleado no está inscrito a esta sesión";
                        }
                        else if (!string.IsNullOrEmpty(registro.FechaFirma))
                        {
                            return "El empleado ya firmó asistencia anteriormente";
                        }
                        else
                        {
                            registro.FechaFirma = DateTime.Now.ToString();

                            //var relacion = db.Capacitacion.FirstOrDefault(x => x.Id == IdCap)?.IdentificadorRelacion;
                            //var capP = db.Capacitacion.FirstOrDefault(x => x.IdentificadorRelacion == relacion);
                            //capP.CtnAsistentes++;

                            var cap = db.Capacitacion.FirstOrDefault(x => x.Id == IdCap);
                            cap.CtnAsistentes++;
                            db.SaveChanges();

                            return "true";
                        }

                    }
                    else
                    {
                        return "El numero de documento ingresado no corresponde a ningún empleado";
                    }
                } else if (TipoPersona == "Tercero")
                {
                    var Ter = db.Tercero.Any(x => x.Documento == cedula);

                    if (Ter)
                    {

                        var tercero = db.Tercero.FirstOrDefault(x => x.Documento == cedula);

                        var registro = db.DetalleCapacitacion.FirstOrDefault(x => x.CapacitacionId == IdCap && x.TerceroId == tercero.Id.ToString());

                        if (registro == null)
                        {
                            return "El tercero no está inscrito a esta sesión";
                        }
                        else if (!string.IsNullOrEmpty(registro.FechaFirma))
                        {
                            return "El tercero ya firmó asistencia anteriormente";
                        }
                        else
                        {
                            registro.FechaFirma = DateTime.Now.ToString();
                            //var relacion = db.Capacitacion.FirstOrDefault(x => x.Id == IdCap)?.IdentificadorRelacion;
                            //var capP = db.Capacitacion.FirstOrDefault(x => x.IdentificadorRelacion == relacion);
                            //capP.CtnAsistentes++;

                            var cap = db.Capacitacion.FirstOrDefault(x => x.Id == IdCap);
                            cap.CtnAsistentes++;
                            db.SaveChanges();

                            return "true";
                        }

                    }
                    else
                    {
                        return "El numero de documento ingresado no corresponde a ningún tercero";
                    }
                }
                else
                {
                    return "Debe seleccionar el Tipo de Usuario primero";
                }


            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }

        }

        public string Modificar(int capacita_id, int empleado_id, string medio)
        {
            string message = "";
            DetalleCapacitacion detalle = new DetalleCapacitacion();


            using (var db = new AutogestionContext())
            {
                try
                {
                    Empleado emp = db.Empleados.Find(empleado_id);
                    Capacitacion capacita = db.Capacitacion.Find(capacita_id);
                    detalle.EmpleadoId = emp.Id;
                    detalle.FechaFirma = DateTime.Now.ToString() + "|" + medio;
                    detalle.Estado = "Activo";
                    detalle.Cargo = emp.Cargo;
                    detalle.Area = emp.Area;
                    detalle.EsTercero = "NO";
                    detalle.TerceroId = null;
                    detalle.CapacitacionId = capacita_id;
                    detalle.SedeId = capacita.IdSede;
                    db.DetalleCapacitacion.Add(detalle);
                    db.SaveChanges();
                    message = "ok";
                }
                catch (Exception ex)
                {
                    message = "Error, " + ex.Message.ToString();
                    return message;
                }
            }


            return message;
        }

        //public string BuscarDatosResponsable (string Cedula)
        //{
        //    var emp = db.Empleados.FirstOrDefault(x => x.Documento == Cedula && x.Activo == "SI");

        //    if (emp != null)
        //    {
        //        var Nombres = emp.Nombres;
        //        var Cargo = emp.Cargo;

        //        return Nombres + "-" + Cargo;
        //    }
        //    else
        //    {
        //        return "false";
        //    }

        //}

        public string Editar(int id, Capacitacion model)
        {

            string message = "";

            using (var db = new AutogestionContext())
            {

                try
                {

                    Capacitacion detalle = new Capacitacion();

                    detalle = db.Capacitacion.Find(id);
                    detalle.HoraFin = model.HoraFin;
                    detalle.HoraInicio = model.HoraInicio;
                    detalle.FechaCapacitacion = model.FechaCapacitacion;
                    detalle.EvaluacionConocimiento = model.EvaluacionConocimiento;
                    detalle.EncuestaSatisfaccion = model.EncuestaSatisfaccion;
                    detalle.Modalidad = model.Modalidad;
                    detalle.Ciudad = model.Ciudad;
                    detalle.Lugar = model.Lugar;
                    if (detalle.Metodologia == 2 || detalle.Metodologia == 3 || detalle.Metodologia == 4 || detalle.Metodologia == 9)
                    {
                        detalle.CtnProgramados = model.CtnProgramados;
                    }

                    db.SaveChanges();

                    message = "Sesión editada con exito.";


                }
                catch (SystemException EX)
                {

                    message = "ERROR: " + EX;
                }


                return message;

            }

        }

        public Capacitacion CrearCopia(int id)
        {
            var Copiacapacitacion = new Capacitacion();
            using (var db = new AutogestionContext())
            {

                Capacitacion OrgCap = db.Capacitacion.FirstOrDefault(x => x.Id == id);
                //var fechacap =Convert.ToDateTime( OrgCap.FechaCapacitacion.GetDateTimeFormats());

                Copiacapacitacion.Nombre = OrgCap.Nombre;
                //Copiacapacitacion.Objetivo = OrgCap.Objetivo;
                Copiacapacitacion.IdSede = OrgCap.IdSede;
                Copiacapacitacion.EmpleadoRegistraId = OrgCap.EmpleadoRegistraId;
                Copiacapacitacion.Metodologia = OrgCap.Metodologia;
                Copiacapacitacion.HoraFin = OrgCap.HoraFin;
                Copiacapacitacion.HoraInicio = OrgCap.HoraInicio;
                Copiacapacitacion.FechaCapacitacion = OrgCap.FechaCapacitacion;
                Copiacapacitacion.Empresa = OrgCap.Empresa;
                //Copiacapacitacion.temas = OrgCap.temas;
                Copiacapacitacion.IdDirigidoA = OrgCap.IdDirigidoA;
                Copiacapacitacion.Ciudad = OrgCap.Ciudad;
                Copiacapacitacion.Estado = "Activo";
                Copiacapacitacion.FechaRegistro = DateTime.Now;

                Copiacapacitacion.IdTipoPEC = OrgCap.IdTipoPEC;
                Copiacapacitacion.Responsable = OrgCap.Responsable;
                Copiacapacitacion.CargoResponsable = OrgCap.CargoResponsable;
                Copiacapacitacion.IdProgramaInstitucional = OrgCap.IdProgramaInstitucional;
                Copiacapacitacion.ResponsablePrograma = OrgCap.ResponsablePrograma;
                Copiacapacitacion.CargoResponsablePrograma = OrgCap.CargoResponsablePrograma;
                Copiacapacitacion.AreaObjetivo = OrgCap.AreaObjetivo;
                Copiacapacitacion.TotalSesiones = OrgCap.TotalSesiones;
                Copiacapacitacion.TotalPersonas = OrgCap.TotalPersonas;
                Copiacapacitacion.IdDirigidoA = OrgCap.IdDirigidoA;
                Copiacapacitacion.Proveedor = OrgCap.Proveedor;
                Copiacapacitacion.MetaEficacia = OrgCap.MetaEficacia;
                Copiacapacitacion.IdRequerimientoInstitucional = OrgCap.IdRequerimientoInstitucional;
                Copiacapacitacion.EspecificacionReq = OrgCap.EspecificacionReq;
                Copiacapacitacion.Metodologia = OrgCap.Metodologia;
                Copiacapacitacion.PresupuestoRequerido = OrgCap.PresupuestoRequerido;
                Copiacapacitacion.Presupuesto = OrgCap.Presupuesto;

                db.Capacitacion.Add(Copiacapacitacion);
                db.SaveChanges();
            }
            return Copiacapacitacion;
        }


        internal void ActualizarEnvioEncuesta(DetalleCapacitacion model)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    DetalleCapacitacion detalle = new DetalleCapacitacion();

                    detalle = model;

                    db.Entry(detalle).State = EntityState.Modified;
                    db.SaveChanges();



                }
                catch
                { }
            }
        }


        public List<Capacitacion> ObtenerTodos2(List<Expositor> expo)
        {

            List<Capacitacion> lst = new List<Capacitacion>();


            using (var db = new AutogestionContext())
            {


                foreach (Expositor item in expo) {
                    Capacitacion Items = db.Capacitacion.Find(item.CapacitacionId);

                    lst.Add(new Capacitacion() { Id = Items.Id, EmpleadoRegistraId = Items.EmpleadoRegistraId, Nombre = Items.Nombre, FechaCapacitacion = Items.FechaCapacitacion, FechaRegistro = Items.FechaRegistro, HoraInicio = Items.HoraInicio, HoraFin = Items.HoraFin, temas = Items.temas });

                }

            }

            return lst;
        }
        public void CrearPreinscripcion()
        {

        }

        public string ActualizarDatosCursosEduFoscal()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 &&
                x.Modalidad == "4" && x.IdRequerimientoInstitucional == 1 && x.CursoId != "0" && x.Estado == "Activo");

            var listadoCapacitacionesId = capacitacionesRaw
                //.GroupBy(x => x.IdentificadorRelacion)
                .Select(g => new { g.Id, g.CursoId })
                .ToDictionary(x => x.Id, x => Convert.ToInt32(x.CursoId));

            var listadoPersonalInscritoId = db.DetalleCapacitacion
                .Where(x => listadoCapacitacionesId.Keys.Contains(x.CapacitacionId) && x.FechaFirma == null)
                .Select(x => x.EmpleadoId)
                .Distinct()
                .ToList();
            
            if (!listadoPersonalInscritoId.Any())
            {
                return "No hay datos por actualizar.";
            }

            var listadoDocumentosPersonal = db.Empleados
                .Where(x => listadoPersonalInscritoId.Contains(x.Id))
                .Select(x => x.Documento)
                .ToList();

            //BUSCAR SI EXISTE UN REGISTRO CON EL USERNAME Y COURSE ID
            var registroCertificado = db2.CertificadoEduFoscal
                .Where(x => listadoDocumentosPersonal.Contains(x.username) && listadoCapacitacionesId.Values.Contains(x.course_id))
                .ToList();

            if (!registroCertificado.Any())
            {
                return "No hay datos por actualizar.";
            }

            foreach (var i in registroCertificado)
            {
                //OBTENER EL ID DEL EMPLEADO CON EL DOCUMENTO
                var idEmpleado = db.Empleados.FirstOrDefault(x => x.Documento == i.username)?.Id;

                if (idEmpleado > 0)
                {

                    //BUSCAR EL REGISTRO DE INSCRIPCION DEL EMPLEADO
                    var registroDeInscripcion = db.DetalleCapacitacion
                        .Where(x => x.EmpleadoId == idEmpleado && x.Estado == "Activo" && listadoCapacitacionesId.Keys.Contains(x.CapacitacionId)).ToList();

                    foreach (var registro in registroDeInscripcion)
                    {
                        //ACTUALIZAR SU ASISTENCIA
                        registro.FechaFirma = "CERTIFICADO";

                        var capacitacion = db.Capacitacion.FirstOrDefault(x => x.Id == registro.CapacitacionId);

                        if (capacitacion != null)
                        {
                            capacitacion.CtnAsistentes += 1;
                        }
                    }
                }
            }
            db.SaveChanges();

            return "Se actualizaron los datos correctamente.";
        }


        public async Task<ListaGraficasEducacion> ObtenerDatosGraficas()
        {
            ListaGraficasEducacion sr = new ListaGraficasEducacion();

            sr.IndicadorAccionesEjecutadasGeneral = await ObtenerIndicadorAccionesEjecutadasGeneral();
            sr.IndicadorAccionesEjecutadasAreaFoscal = await ObtenerIndicadorAccionesEjecutadasAreaFoscal();
            sr.IndicadorAccionesEjecutadasAreasFoscal = await ObtenerIndicadorAccionesEjecutadasAreasFoscal();
            sr.IndicadorAccionesEjecutadasAreaFosunab = await ObtenerIndicadorAccionesEjecutadasAreaFosunab();
            sr.IndicadorAccionesEjecutadasAreasFosunab = await ObtenerIndicadorAccionesEjecutadasAreasFosunab();
            sr.IndicadorAccionesEjecutadasPICO = await ObtenerIndicadorAccionesEjecutadasPICO();
            sr.IndicadorAccionesEjecutadasPICOs = await ObtenerIndicadorAccionesEjecutadasPICOs();
            sr.IndicadorAccionesEjecutadasDI = await ObtenerIndicadorAccionesEjecutadasDI();
            sr.IndicadorAccionesEjecutadasSUH = await ObtenerIndicadorAccionesEjecutadasSUH();
            //
            sr.IndicadorAccionesEjecutadasGeneralAnual = await ObtenerIndicadorAccionesEjecutadasGeneralAnual();
            sr.IndicadorAccionesEjecutadasAreaFoscalAnual = await ObtenerIndicadorAccionesEjecutadasAreaFoscalAnual();
            sr.IndicadorAccionesEjecutadasAreasFoscalAnual = await ObtenerIndicadorAccionesEjecutadasAreasFoscalAnual();
            sr.IndicadorAccionesEjecutadasAreaFosunabAnual = await ObtenerIndicadorAccionesEjecutadasAreaFosunabAnual();
            sr.IndicadorAccionesEjecutadasAreasFosunabAnual = await ObtenerIndicadorAccionesEjecutadasAreasFosunabAnual();
            sr.IndicadorAccionesEjecutadasPICOAnual = await ObtenerIndicadorAccionesEjecutadasPICOAnual();
            sr.IndicadorAccionesEjecutadasPICOsAnual = await ObtenerIndicadorAccionesEjecutadasPICOsAnual();
            sr.IndicadorAccionesEjecutadasDIAnual = await ObtenerIndicadorAccionesEjecutadasDIAnual();
            sr.IndicadorAccionesEjecutadasSUHAnual = await ObtenerIndicadorAccionesEjecutadasSUHAnual();

            sr.IndicadorAsistentesGeneral = await ObtenerIndicadorAsistentesGeneral();
            sr.IndicadorAsistentesAreaFoscal = await ObtenerIndicadorAsistentesAreaFoscal();
            sr.IndicadorAsistentesAreasFoscal = await ObtenerIndicadorAsistentesAreasFoscal();
            sr.IndicadorAsistentesAreaFosunab = await ObtenerIndicadorAsistentesAreaFosunab();
            sr.IndicadorAsistentesAreasFosunab = await ObtenerIndicadorAsistentesAreasFosunab();
            sr.IndicadorAsistentesPICO = await ObtenerIndicadorAsistentesPICO();
            sr.IndicadorAsistentesPICOs = await ObtenerIndicadorAsistentesPICOs();
            sr.IndicadorAsistentesDI = await ObtenerIndicadorAsistentesDI();
            sr.IndicadorAsistentesSUH = await ObtenerIndicadorAsistentesSUH();
            //
            sr.IndicadorAsistentesGeneralAnual = await ObtenerIndicadorAsistentesGeneralAnual();
            sr.IndicadorAsistentesAreaFoscalAnual = await ObtenerIndicadorAsistentesAreaFoscalAnual();
            sr.IndicadorAsistentesAreasFoscalAnual = await ObtenerIndicadorAsistentesAreasFoscalAnual();
            sr.IndicadorAsistentesAreaFosunabAnual = await ObtenerIndicadorAsistentesAreaFosunabAnual();
            sr.IndicadorAsistentesAreasFosunabAnual = await ObtenerIndicadorAsistentesAreasFosunabAnual();
            sr.IndicadorAsistentesPICOAnual = await ObtenerIndicadorAsistentesPICOAnual();
            sr.IndicadorAsistentesPICOsAnual = await ObtenerIndicadorAsistentesPICOsAnual();
            sr.IndicadorAsistentesDIAnual = await ObtenerIndicadorAsistentesDIAnual();
            sr.IndicadorAsistentesSUHAnual = await ObtenerIndicadorAsistentesSUHAnual();

            //GRAFICAS EDUFOSCAL
            sr.IndicadorAccionesEjecutadasGeneralEduFoscal = await ObtenerIndicadorAccionesEjecutadasGeneralEduFoscal();
            sr.IndicadorAccionesEjecutadasGeneralEduFoscalAnual = await ObtenerIndicadorAccionesEjecutadasGeneralEduFoscalAnual();

            sr.IndicadorAsistentesGeneralEduFoscal = await ObtenerIndicadorAsistentesGeneralEduFoscal();
            sr.IndicadorAsistentesGeneralEduFoscalAnual = await ObtenerIndicadorAsistentesGeneralEduFoscalAnual();

            return sr;
        }

        public async Task<List<IndicadorAccionesEjecutadas>> ObtenerIndicadorAccionesEjecutadasGeneral()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion,
                })
                .Select(g => new
                {
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorTrimestre = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    Trimestre = (x.PrimerRegistro.FechaCapacitacion.Month - 1) / 3 + 1
                })
                .Select(g => new IndicadorAccionesEjecutadas
                {
                    Trimestre = GetTrimestreName(g.Key.Trimestre),
                    Foscal = g.Key.Empresa == "1000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00
                })
                .ToList();

            return capacitacionesPorTrimestre;
        }
        public async Task<List<IndicadorAccionesEjecutadasAreaFoscal>> ObtenerIndicadorAccionesEjecutadasAreaFoscal()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Empresa == "1000" && x.IdTipoPEC == 1
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionesPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorAreaYTrimestre = capacitacionesPrincipal
                .GroupBy(x => new
                {
                    x.PrimerRegistro.AreaObjetivo,
                    Trimestre = (x.PrimerRegistro.FechaCapacitacion.Month - 1) / 3 + 1
                })
                .Select(g => new IndicadorAccionesEjecutadasAreaFoscal
                {
                    Area = g.Key.AreaObjetivo,
                    Trimestre = GetTrimestreName(g.Key.Trimestre),
                    Indicador = g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura))
                })
                .ToList();

            return capacitacionesPorAreaYTrimestre;
        }
        public async Task<List<IndicadorAccionesEjecutadasAreasFoscal>> ObtenerIndicadorAccionesEjecutadasAreasFoscal()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Empresa == "1000" && x.IdTipoPEC == 1
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionesPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorAreaYTrimestre = capacitacionesPrincipal
                .GroupBy(x => new
                {
                    //x.PrimerRegistro.AreaObjetivo,
                    Trimestre = (x.PrimerRegistro.FechaCapacitacion.Month - 1) / 3 + 1
                })
                .Select(g => new IndicadorAccionesEjecutadasAreasFoscal
                {
                    Trimestre = GetTrimestreName(g.Key.Trimestre),
                    Indicador = g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura))
                })
                .ToList();

            return capacitacionesPorAreaYTrimestre;
        }
        public async Task<List<IndicadorAccionesEjecutadasAreaFosunab>> ObtenerIndicadorAccionesEjecutadasAreaFosunab()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Empresa == "2000" && x.IdTipoPEC == 1 
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionesPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorAreaYTrimestre = capacitacionesPrincipal
                .GroupBy(x => new
                {
                    x.PrimerRegistro.AreaObjetivo,
                    Trimestre = (x.PrimerRegistro.FechaCapacitacion.Month - 1) / 3 + 1
                })
                .Select(g => new IndicadorAccionesEjecutadasAreaFosunab
                {
                    Area = g.Key.AreaObjetivo,
                    Trimestre = GetTrimestreName(g.Key.Trimestre),
                    Indicador = g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura))
                })
                .ToList();

            return capacitacionesPorAreaYTrimestre;
        }
        public async Task<List<IndicadorAccionesEjecutadasAreasFosunab>> ObtenerIndicadorAccionesEjecutadasAreasFosunab()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Empresa == "2000" && x.IdTipoPEC == 1
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionesPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorAreaYTrimestre = capacitacionesPrincipal
                .GroupBy(x => new
                {
                    //x.PrimerRegistro.AreaObjetivo,
                    Trimestre = (x.PrimerRegistro.FechaCapacitacion.Month - 1) / 3 + 1
                })
                .Select(g => new IndicadorAccionesEjecutadasAreasFosunab
                {
                    Trimestre = GetTrimestreName(g.Key.Trimestre),
                    Indicador = g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura))
                })
                .ToList();

            return capacitacionesPorAreaYTrimestre;
        }
        public async Task<List<IndicadorAccionesEjecutadasPICO>> ObtenerIndicadorAccionesEjecutadasPICO()
        {
            List<IndicadorAccionesEjecutadasPICO> tabla = new List<IndicadorAccionesEjecutadasPICO>();

            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.IdProgramaInstitucional != 0 && x.IdTipoPEC == 2
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorTrimestre = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    PICO = x.PrimerRegistro.IdProgramaInstitucional,
                    Trimestre = (x.PrimerRegistro.FechaCapacitacion.Month - 1) / 3 + 1
                })
                .Select(g => new IndicadorAccionesEjecutadasPICO
                {
                    Trimestre = GetTrimestreName(g.Key.Trimestre),
                    PICO = GetPICOName(g.Key.PICO),
                    Foscal = g.Key.Empresa == "1000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00
                })
                .ToList();

            return capacitacionesPorTrimestre;
        }
        public async Task<List<IndicadorAccionesEjecutadasPICOs>> ObtenerIndicadorAccionesEjecutadasPICOs()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.IdProgramaInstitucional != 0 && x.IdTipoPEC == 2
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorTrimestre = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    Trimestre = (x.PrimerRegistro.FechaCapacitacion.Month - 1) / 3 + 1
                })
                .Select(g => new IndicadorAccionesEjecutadasPICOs
                {
                    Trimestre = GetTrimestreName(g.Key.Trimestre),
                    Foscal = g.Key.Empresa == "1000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00
                })
                .ToList();

            return capacitacionesPorTrimestre;
        }
        public async Task<List<IndicadorAccionesEjecutadasDI>> ObtenerIndicadorAccionesEjecutadasDI()
        {
            List<IndicadorAccionesEjecutadasDI> tabla = new List<IndicadorAccionesEjecutadasDI>();

            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.IdTipoPEC == 3
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorTrimestre = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    Trimestre = (x.PrimerRegistro.FechaCapacitacion.Month - 1) / 3 + 1
                })
                .Select(g => new IndicadorAccionesEjecutadasDI
                {
                    Trimestre = GetTrimestreName(g.Key.Trimestre),
                    Foscal = g.Key.Empresa == "1000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00
                })
                .ToList();

            return capacitacionesPorTrimestre;
        }
        public async Task<List<IndicadorAccionesEjecutadasSUH>> ObtenerIndicadorAccionesEjecutadasSUH()
        {

            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.IdRequerimientoInstitucional == 1
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorTrimestre = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    Trimestre = (x.PrimerRegistro.FechaCapacitacion.Month - 1) / 3 + 1
                })
                .Select(g => new IndicadorAccionesEjecutadasSUH
                {
                    Trimestre = GetTrimestreName(g.Key.Trimestre),
                    Foscal = g.Key.Empresa == "1000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00
                })
                .ToList();

            return capacitacionesPorTrimestre;
        }


        public async Task<List<IndicadorAccionesEjecutadasAnual>> ObtenerIndicadorAccionesEjecutadasGeneralAnual()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion,
                })
                .Select(g => new
                {
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionAnual = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    Year = Fecha1.Year.ToString()
                })
                .Select(g => new IndicadorAccionesEjecutadasAnual
                {
                    Year = g.Key.Year,
                    Foscal = g.Key.Empresa == "1000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00
                })
                .ToList();
            
            return capacitacionAnual;
        }
        public async Task<List<IndicadorAccionesEjecutadasAreaFoscalAnual>> ObtenerIndicadorAccionesEjecutadasAreaFoscalAnual()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Empresa == "1000" && x.IdTipoPEC == 1
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionesPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorAreaYTrimestre = capacitacionesPrincipal
                .GroupBy(x => new
                {
                    x.PrimerRegistro.AreaObjetivo,
                    Year = Fecha1.Year.ToString()
                })
                .Select(g => new IndicadorAccionesEjecutadasAreaFoscalAnual
                {
                    Area = g.Key.AreaObjetivo,
                    Year = g.Key.Year,
                    Indicador = g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura))
                })
                .ToList();

            return capacitacionesPorAreaYTrimestre;
        }
        public async Task<List<IndicadorAccionesEjecutadasAreasFoscalAnual>> ObtenerIndicadorAccionesEjecutadasAreasFoscalAnual()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Empresa == "1000" && x.IdTipoPEC == 1
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionesPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorAreaYTrimestre = capacitacionesPrincipal
                .GroupBy(x => new
                {
                    //x.PrimerRegistro.AreaObjetivo,
                    Year = Fecha1.Year.ToString()
                })
                .Select(g => new IndicadorAccionesEjecutadasAreasFoscalAnual
                {
                    Year = g.Key.Year,
                    Indicador = g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura))
                })
                .ToList();

            return capacitacionesPorAreaYTrimestre;
        }
        public async Task<List<IndicadorAccionesEjecutadasAreaFosunabAnual>> ObtenerIndicadorAccionesEjecutadasAreaFosunabAnual()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Empresa == "2000" && x.IdTipoPEC == 1
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionesPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorAreaYTrimestre = capacitacionesPrincipal
                .GroupBy(x => new
                {
                    x.PrimerRegistro.AreaObjetivo,
                    Year = Fecha1.Year.ToString()
                })
                .Select(g => new IndicadorAccionesEjecutadasAreaFosunabAnual
                {
                    Area = g.Key.AreaObjetivo,
                    Year = g.Key.Year,
                    Indicador = g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura))
                })
                .ToList();

            return capacitacionesPorAreaYTrimestre;
        }
        public async Task<List<IndicadorAccionesEjecutadasAreasFosunabAnual>> ObtenerIndicadorAccionesEjecutadasAreasFosunabAnual()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Empresa == "2000" && x.IdTipoPEC == 1
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionesPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorAreaYTrimestre = capacitacionesPrincipal
                .GroupBy(x => new
                {
                    //x.PrimerRegistro.AreaObjetivo,
                    Year = Fecha1.Year.ToString()
                })
                .Select(g => new IndicadorAccionesEjecutadasAreasFosunabAnual
                {
                    Year = g.Key.Year,
                    Indicador = g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura))
                })
                .ToList();

            return capacitacionesPorAreaYTrimestre;
        }
        public async Task<List<IndicadorAccionesEjecutadasPICOAnual>> ObtenerIndicadorAccionesEjecutadasPICOAnual()
        {

            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.IdProgramaInstitucional != 0 && x.IdTipoPEC == 2
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorTrimestre = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    PICO = x.PrimerRegistro.IdProgramaInstitucional,
                    Year = Fecha1.Year.ToString()
                })
                .Select(g => new IndicadorAccionesEjecutadasPICOAnual
                {
                    Year = g.Key.Year,
                    PICO = GetPICOName(g.Key.PICO),
                    Foscal = g.Key.Empresa == "1000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00
                })
                .ToList();

            return capacitacionesPorTrimestre;
        }
        public async Task<List<IndicadorAccionesEjecutadasPICOsAnual>> ObtenerIndicadorAccionesEjecutadasPICOsAnual()
        {

            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.IdProgramaInstitucional != 0 && x.IdTipoPEC == 2
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorTrimestre = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    Year = Fecha1.Year.ToString()
                })
                .Select(g => new IndicadorAccionesEjecutadasPICOsAnual
                {
                    Year = g.Key.Year,
                    Foscal = g.Key.Empresa == "1000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00
                })
                .ToList();

            return capacitacionesPorTrimestre;
        }
        public async Task<List<IndicadorAccionesEjecutadasDIAnual>> ObtenerIndicadorAccionesEjecutadasDIAnual()
        {

            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.IdTipoPEC == 3
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorTrimestre = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    Year = Fecha1.Year.ToString()
                })
                .Select(g => new IndicadorAccionesEjecutadasDIAnual
                {
                    Year = g.Key.Year,
                    Foscal = g.Key.Empresa == "1000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00
                })
                .ToList();

            return capacitacionesPorTrimestre;
        }
        public async Task<List<IndicadorAccionesEjecutadasSUHAnual>> ObtenerIndicadorAccionesEjecutadasSUHAnual()
        {

            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.IdRequerimientoInstitucional == 1
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorTrimestre = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    Year = Fecha1.Year.ToString()
                })
                .Select(g => new IndicadorAccionesEjecutadasSUHAnual
                {
                    Year = g.Key.Year,
                    Foscal = g.Key.Empresa == "1000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00
                })
                .ToList();

            return capacitacionesPorTrimestre;
        }

        public async Task<List<IndicadorAccionesEjecutadasGeneralEduFoscal>> ObtenerIndicadorAccionesEjecutadasGeneralEduFoscal()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Modalidad == "4"
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697" && x.CursoId != "0"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion,
                })
                .Select(g => new
                {
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorTrimestre = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    Trimestre = (x.PrimerRegistro.FechaCapacitacion.Month - 1) / 3 + 1
                })
                .Select(g => new IndicadorAccionesEjecutadasGeneralEduFoscal
                {
                    Trimestre = GetTrimestreName(g.Key.Trimestre),
                    Foscal = g.Key.Empresa == "1000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00
                })
                .ToList();

            return capacitacionesPorTrimestre;
        }
        public async Task<List<IndicadorAccionesEjecutadasGeneralEduFoscalAnual>> ObtenerIndicadorAccionesEjecutadasGeneralEduFoscalAnual()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Modalidad == "4"
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697" && x.CursoId != "0"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion,
                })
                .Select(g => new
                {
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionAnual = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    Year = Fecha1.Year.ToString()
                })
                .Select(g => new IndicadorAccionesEjecutadasGeneralEduFoscalAnual
                {
                    Year = g.Key.Year,
                    Foscal = g.Key.Empresa == "1000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? g.Average(x => Convert.ToDouble(x.PrimerRegistro.Cobertura)) : 0.00
                })
                .ToList();

            return capacitacionAnual;
        }


        private string GetTrimestreName(int trimestre)
        {
            if (trimestre == 1)
            {
                return "Primer Trimestre";
            }
            else if (trimestre == 2)
            {
                return "Segundo Trimestre";
            }
            else if (trimestre == 3)
            {
                return "Tercer Trimestre";
            }
            else if (trimestre == 4)
            {
                return "Cuarto Trimestre";
            }
            else
            {
                return "Desconocido";
            }
        }
        private string GetPICOName(int IdPICO)
        {
            var PICO = db.ProgramaInstitucional.FirstOrDefault(x => x.Id == IdPICO)?.Valor;

            return PICO;
        }



        public async Task<List<IndicadorAsistentesGeneral>> ObtenerIndicadorAsistentesGeneral()
        {

            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    TotalAsistentes = g.Sum(x => x.CtnAsistentes),
                    TotalPersonas = g.Sum(x => x.CtnProgramados),
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorTrimestre = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    Trimestre = (x.PrimerRegistro.FechaCapacitacion.Month - 1) / 3 + 1
                })
                .Select(g => new IndicadorAsistentesGeneral
                {
                    Trimestre = GetTrimestreName(g.Key.Trimestre),
                    Foscal = g.Key.Empresa == "1000" ?  CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00
                })
                .ToList();

            return capacitacionesPorTrimestre;
        }
        public async Task<List<IndicadorAsistentesAreaFoscal>> ObtenerIndicadorAsistentesAreaFoscal()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Empresa == "1000" && x.IdTipoPEC == 1
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionesPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    TotalAsistentes = g.Sum(x => x.CtnAsistentes),
                    TotalPersonas = g.Sum(x => x.CtnProgramados),
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorAreaYTrimestre = capacitacionesPrincipal
                .GroupBy(x => new
                {
                    x.PrimerRegistro.AreaObjetivo,
                    Trimestre = (x.PrimerRegistro.FechaCapacitacion.Month - 1) / 3 + 1
                })
                .Select(g => new IndicadorAsistentesAreaFoscal
                {
                    Area = g.Key.AreaObjetivo,
                    Trimestre = GetTrimestreName(g.Key.Trimestre),
                    Indicador = CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas))
                })
                .ToList();

            return capacitacionesPorAreaYTrimestre;
        }
        public async Task<List<IndicadorAsistentesAreasFoscal>> ObtenerIndicadorAsistentesAreasFoscal()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Empresa == "1000" && x.IdTipoPEC == 1
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionesPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    TotalAsistentes = g.Sum(x => x.CtnAsistentes),
                    TotalPersonas = g.Sum(x => x.CtnProgramados),
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorAreaYTrimestre = capacitacionesPrincipal
                .GroupBy(x => new
                {
                    //x.PrimerRegistro.AreaObjetivo,
                    Trimestre = (x.PrimerRegistro.FechaCapacitacion.Month - 1) / 3 + 1
                })
                .Select(g => new IndicadorAsistentesAreasFoscal
                {
                    Trimestre = GetTrimestreName(g.Key.Trimestre),
                    Indicador = CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas))
                })
                .ToList();

            return capacitacionesPorAreaYTrimestre;
        }
        public async Task<List<IndicadorAsistentesAreaFosunab>> ObtenerIndicadorAsistentesAreaFosunab()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Empresa == "2000" && x.IdTipoPEC == 1
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionesPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    TotalAsistentes = g.Sum(x => x.CtnAsistentes),
                    TotalPersonas = g.Sum(x => x.CtnProgramados),
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorAreaYTrimestre = capacitacionesPrincipal
                .GroupBy(x => new
                {
                    x.PrimerRegistro.AreaObjetivo,
                    Trimestre = (x.PrimerRegistro.FechaCapacitacion.Month - 1) / 3 + 1
                })
                .Select(g => new IndicadorAsistentesAreaFosunab
                {
                    Area = g.Key.AreaObjetivo,
                    Trimestre = GetTrimestreName(g.Key.Trimestre),
                    Indicador = CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas))
                })
                .ToList();

            return capacitacionesPorAreaYTrimestre;
        }
        public async Task<List<IndicadorAsistentesAreasFosunab>> ObtenerIndicadorAsistentesAreasFosunab()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Empresa == "2000" && x.IdTipoPEC == 1
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionesPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    TotalAsistentes = g.Sum(x => x.CtnAsistentes),
                    TotalPersonas = g.Sum(x => x.CtnProgramados),
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorAreaYTrimestre = capacitacionesPrincipal
                .GroupBy(x => new
                {
                    //x.PrimerRegistro.AreaObjetivo,
                    Trimestre = (x.PrimerRegistro.FechaCapacitacion.Month - 1) / 3 + 1
                })
                .Select(g => new IndicadorAsistentesAreasFosunab
                {
                    Trimestre = GetTrimestreName(g.Key.Trimestre),
                    Indicador = CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas))
                })
                .ToList();

            return capacitacionesPorAreaYTrimestre;
        }
        public async Task<List<IndicadorAsistentesPICO>> ObtenerIndicadorAsistentesPICO()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.IdProgramaInstitucional != 0 && x.IdTipoPEC == 2
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    TotalAsistentes = g.Sum(x => x.CtnAsistentes),
                    TotalPersonas = g.Sum(x => x.CtnProgramados),
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorTrimestre = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    PICO = x.PrimerRegistro.IdProgramaInstitucional,
                    Trimestre = (x.PrimerRegistro.FechaCapacitacion.Month - 1) / 3 + 1
                })
                .Select(g => new IndicadorAsistentesPICO
                {
                    Trimestre = GetTrimestreName(g.Key.Trimestre),
                    PICO = GetPICOName(g.Key.PICO),
                    Foscal = g.Key.Empresa == "1000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00
                })
                .ToList();

            return capacitacionesPorTrimestre;
        }
        public async Task<List<IndicadorAsistentesPICOs>> ObtenerIndicadorAsistentesPICOs()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.IdProgramaInstitucional != 0 && x.IdTipoPEC == 2
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    TotalAsistentes = g.Sum(x => x.CtnAsistentes),
                    TotalPersonas = g.Sum(x => x.CtnProgramados),
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorTrimestre = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    Trimestre = (x.PrimerRegistro.FechaCapacitacion.Month - 1) / 3 + 1
                })
                .Select(g => new IndicadorAsistentesPICOs
                {
                    Trimestre = GetTrimestreName(g.Key.Trimestre),
                    Foscal = g.Key.Empresa == "1000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00
                })
                .ToList();

            return capacitacionesPorTrimestre;
        }
        public async Task<List<IndicadorAsistentesDI>> ObtenerIndicadorAsistentesDI()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.IdTipoPEC == 3
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    TotalAsistentes = g.Sum(x => x.CtnAsistentes),
                    TotalPersonas = g.Sum(x => x.CtnProgramados),
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorTrimestre = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    Trimestre = (x.PrimerRegistro.FechaCapacitacion.Month - 1) / 3 + 1
                })
                .Select(g => new IndicadorAsistentesDI
                {
                    Trimestre = GetTrimestreName(g.Key.Trimestre),
                    Foscal = g.Key.Empresa == "1000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00
                })
                .ToList();

            return capacitacionesPorTrimestre;
        }
        public async Task<List<IndicadorAsistentesSUH>> ObtenerIndicadorAsistentesSUH()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.IdRequerimientoInstitucional == 1
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    TotalAsistentes = g.Sum(x => x.CtnAsistentes),
                    TotalPersonas = g.Sum(x => x.CtnProgramados),
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorTrimestre = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    Trimestre = (x.PrimerRegistro.FechaCapacitacion.Month - 1) / 3 + 1
                })
                .Select(g => new IndicadorAsistentesSUH
                {
                    Trimestre = GetTrimestreName(g.Key.Trimestre),
                    Foscal = g.Key.Empresa == "1000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00
                })
                .ToList();

            return capacitacionesPorTrimestre;
        }


        public async Task<List<IndicadorAsistentesAnual>> ObtenerIndicadorAsistentesGeneralAnual()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion,
                })
                .Select(g => new
                {
                    TotalAsistentes = g.Sum(x => x.CtnAsistentes),
                    TotalPersonas = g.Sum(x => x.CtnProgramados),
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionAnual = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    Year = Fecha1.Year.ToString()
                })
                .Select(g => new IndicadorAsistentesAnual
                {
                    Year = g.Key.Year,
                    Foscal = g.Key.Empresa == "1000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00
                })
                .ToList();

            return capacitacionAnual;
        }
        public async Task<List<IndicadorAsistentesAreaFoscalAnual>> ObtenerIndicadorAsistentesAreaFoscalAnual()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Empresa == "1000" && x.IdTipoPEC == 1
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionesPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    TotalAsistentes = g.Sum(x => x.CtnAsistentes),
                    TotalPersonas = g.Sum(x => x.CtnProgramados),
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorAreaYTrimestre = capacitacionesPrincipal
                .GroupBy(x => new
                {
                    x.PrimerRegistro.AreaObjetivo,
                    Year = Fecha1.Year.ToString()
                })
                .Select(g => new IndicadorAsistentesAreaFoscalAnual
                {
                    Area = g.Key.AreaObjetivo,
                    Year = g.Key.Year,
                    Indicador = CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas))
                })
                .ToList();

            return capacitacionesPorAreaYTrimestre;
        }
        public async Task<List<IndicadorAsistentesAreasFoscalAnual>> ObtenerIndicadorAsistentesAreasFoscalAnual()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Empresa == "1000" && x.IdTipoPEC == 1
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionesPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    TotalAsistentes = g.Sum(x => x.CtnAsistentes),
                    TotalPersonas = g.Sum(x => x.CtnProgramados),
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorAreaYTrimestre = capacitacionesPrincipal
                .GroupBy(x => new
                {
                    //x.PrimerRegistro.AreaObjetivo,
                    Year = Fecha1.Year.ToString()
                })
                .Select(g => new IndicadorAsistentesAreasFoscalAnual
                {
                    Year = g.Key.Year,
                    Indicador = CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas))
                })
                .ToList();

            return capacitacionesPorAreaYTrimestre;
        }
        public async Task<List<IndicadorAsistentesAreaFosunabAnual>> ObtenerIndicadorAsistentesAreaFosunabAnual()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Empresa == "2000" && x.IdTipoPEC == 1
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionesPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    TotalAsistentes = g.Sum(x => x.CtnAsistentes),
                    TotalPersonas = g.Sum(x => x.CtnProgramados),
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorAreaYTrimestre = capacitacionesPrincipal
                .GroupBy(x => new
                {
                    x.PrimerRegistro.AreaObjetivo,
                    Year = Fecha1.Year.ToString()
                })
                .Select(g => new IndicadorAsistentesAreaFosunabAnual
                {
                    Area = g.Key.AreaObjetivo,
                    Year = g.Key.Year,
                    Indicador = CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas))
                })
                .ToList();

            return capacitacionesPorAreaYTrimestre;
        }
        public async Task<List<IndicadorAsistentesAreasFosunabAnual>> ObtenerIndicadorAsistentesAreasFosunabAnual()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Empresa == "2000" && x.IdTipoPEC == 1
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionesPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    TotalAsistentes = g.Sum(x => x.CtnAsistentes),
                    TotalPersonas = g.Sum(x => x.CtnProgramados),
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorAreaYTrimestre = capacitacionesPrincipal
                .GroupBy(x => new
                {
                    //x.PrimerRegistro.AreaObjetivo,
                    Year = Fecha1.Year.ToString()
                })
                .Select(g => new IndicadorAsistentesAreasFosunabAnual
                {
                    Year = g.Key.Year,
                    Indicador = CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas))
                })
                .ToList();

            return capacitacionesPorAreaYTrimestre;
        }
        public async Task<List<IndicadorAsistentesPICOAnual>> ObtenerIndicadorAsistentesPICOAnual()
        {

            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.IdProgramaInstitucional != 0 && x.IdTipoPEC == 2
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    TotalAsistentes = g.Sum(x => x.CtnAsistentes),
                    TotalPersonas = g.Sum(x => x.CtnProgramados),
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorTrimestre = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    PICO = x.PrimerRegistro.IdProgramaInstitucional,
                    Year = Fecha1.Year.ToString()
                })
                .Select(g => new IndicadorAsistentesPICOAnual
                {
                    Year = g.Key.Year,
                    PICO = GetPICOName(g.Key.PICO),
                    Foscal = g.Key.Empresa == "1000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00
                })
                .ToList();

            return capacitacionesPorTrimestre;
        }
        public async Task<List<IndicadorAsistentesPICOsAnual>> ObtenerIndicadorAsistentesPICOsAnual()
        {

            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.IdProgramaInstitucional != 0 && x.IdTipoPEC == 2
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    TotalAsistentes = g.Sum(x => x.CtnAsistentes),
                    TotalPersonas = g.Sum(x => x.CtnProgramados),
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorTrimestre = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    Year = Fecha1.Year.ToString()
                })
                .Select(g => new IndicadorAsistentesPICOsAnual
                {
                    Year = g.Key.Year,
                    Foscal = g.Key.Empresa == "1000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00
                })
                .ToList();

            return capacitacionesPorTrimestre;
        }
        public async Task<List<IndicadorAsistentesDIAnual>> ObtenerIndicadorAsistentesDIAnual()
        {

            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.IdTipoPEC == 3
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    TotalAsistentes = g.Sum(x => x.CtnAsistentes),
                    TotalPersonas = g.Sum(x => x.CtnProgramados),
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorTrimestre = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    Year = Fecha1.Year.ToString()
                })
                .Select(g => new IndicadorAsistentesDIAnual
                {
                    Year = g.Key.Year,
                    Foscal = g.Key.Empresa == "1000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00
                })
                .ToList();

            return capacitacionesPorTrimestre;
        }
        public async Task<List<IndicadorAsistentesSUHAnual>> ObtenerIndicadorAsistentesSUHAnual()
        {

            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.IdRequerimientoInstitucional == 1
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    TotalAsistentes = g.Sum(x => x.CtnAsistentes),
                    TotalPersonas = g.Sum(x => x.CtnProgramados),
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorTrimestre = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    Year = Fecha1.Year.ToString()
                })
                .Select(g => new IndicadorAsistentesSUHAnual
                {
                    Year = g.Key.Year,
                    Foscal = g.Key.Empresa == "1000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00
                })
                .ToList();

            return capacitacionesPorTrimestre;
        }


        public async Task<List<IndicadorAsistentesGeneralEduFoscal>> ObtenerIndicadorAsistentesGeneralEduFoscal()
        {

            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Modalidad == "4"
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697" && x.CursoId != "0"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    TotalAsistentes = g.Sum(x => x.CtnAsistentes),
                    TotalPersonas = g.Sum(x => x.CtnProgramados),
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorTrimestre = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    Trimestre = (x.PrimerRegistro.FechaCapacitacion.Month - 1) / 3 + 1
                })
                .Select(g => new IndicadorAsistentesGeneralEduFoscal
                {
                    Trimestre = GetTrimestreName(g.Key.Trimestre),
                    Foscal = g.Key.Empresa == "1000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00
                })
                .ToList();

            return capacitacionesPorTrimestre;
        }
        public async Task<List<IndicadorAsistentesGeneralEduFoscalAnual>> ObtenerIndicadorAsistentesGeneralEduFoscalAnual()
        {
            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = await db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Modalidad == "4"
                && (x.CursoId != "1687" && x.CursoId != "1336" && x.CursoId != "1697" && x.CursoId != "0"))
                .ToListAsync();

            var capacitacionPrincipal = capacitacionesRaw
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion,
                })
                .Select(g => new
                {
                    TotalAsistentes = g.Sum(x => x.CtnAsistentes),
                    TotalPersonas = g.Sum(x => x.CtnProgramados),
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionAnual = capacitacionPrincipal
                .GroupBy(x => new
                {
                    Empresa = x.PrimerRegistro.Empresa,
                    Year = Fecha1.Year.ToString()
                })
                .Select(g => new IndicadorAsistentesGeneralEduFoscalAnual
                {
                    Year = g.Key.Year,
                    Foscal = g.Key.Empresa == "1000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00,
                    Fosunab = g.Key.Empresa == "2000" ? CalcularCobertura(g.Sum(x => x.TotalAsistentes), g.Sum(x => x.TotalPersonas)) : 0.00
                })
                .ToList();

            return capacitacionAnual;
        }

        private double CalcularCobertura (int TotalAsistentes, int TotalProgramados)
        {
            if (TotalProgramados == 0)
            {
                return 0.00;
            }

            return TotalAsistentes * 100 / TotalProgramados;
        }
    }
}