using Adm_AutoGestion.DTO;
using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Net.Http;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;
using Org.BouncyCastle.Crypto;
using Adm_AutoGestion.Models.EvaDesempeno;
using Microsoft.Reporting.Map.WebForms.BingMaps;

namespace Adm_AutoGestion.Controllers
{

    public class CapacitacionController : Controller
    {

        private AutogestionContext db = new AutogestionContext();
        public EduFoscalRepository _EduFoscalRepository = new EduFoscalRepository();
        private CapacitacionRepository _repo;
        private DetalleCapacitacionRepository _repo2;
        private ServiciosRepository _servicios;
        private static readonly HttpClient httpClient = new HttpClient();

        public CapacitacionController()
        {
            _repo = new CapacitacionRepository();
            _repo2 = new DetalleCapacitacionRepository();
            _servicios = new ServiciosRepository();
        }
        //
        // GET: /Capacitacion/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Estudiantes(int id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("CrearCapacitacion"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var cap = db.Capacitacion.Find(id);

            var msg = TempData["Mensaje"] as string;
            ViewBag.Message = msg;
            ViewBag.Id = id;

            return View(cap);
        }

        [HttpPost]
        public ActionResult Estudiantes(int id, HttpPostedFileBase file)
        {

            try
            {
                if (file != null && file.ContentLength > 0)
                {

                    var extension = Path.GetExtension(file.FileName).ToLower();
                    if (extension != ".csv")
                    {
                        TempData["Mensaje"] = "Solo se permite archivo .csv";
                        return RedirectToAction("Estudiantes");
                    }

                    using (var reader = new StreamReader(file.InputStream))
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                    {
                        var registros = csv.GetRecords<EstudiantesCSV>().ToList();
                        var estudiantesOmitidos = 0;
                        var estudiantesYaInscritos = 0;

                        foreach (var registro in registros)
                        {
                            var doc = registro.Documento;

                            if (!db.Tercero.Any(x => x.Documento == doc))
                            {
                                var estudiante = new Tercero
                                {
                                    Documento = registro.Documento,
                                    Nombres = registro.Nombres.ToUpper(),
                                    CorreoPersonal = registro.CorreoPersonal,
                                    Area = registro.Area.ToUpper(),
                                    Cargo = registro.Cargo.ToUpper(),
                                    Universidad = registro.Universidad.ToUpper(),
                                    Activo = "SI",
                                    Estudiante = "SI",
                                    SociedadCOD = "1"
                                };
                                db.Tercero.Add(estudiante);
                            }
                            else
                            {
                                estudiantesOmitidos++;
                            }

                        }
                        db.SaveChanges();

                        foreach (var registro in registros)
                        {
                            var doc = registro.Documento;

                            var estudianteId = db.Tercero.FirstOrDefault(x => x.Documento == doc)?.Id.ToString();

                            if (!db.DetalleCapacitacion.Any(x => x.TerceroId == estudianteId && x.CapacitacionId == id))
                            {
                                var detalle = new DetalleCapacitacion();
                                var tercero = db.Tercero.FirstOrDefault(x => x.Documento == doc);

                                detalle.Estado = "Activo";
                                detalle.EsTercero = "ESTUDIANTE";
                                detalle.TerceroId = estudianteId;
                                detalle.Cargo = tercero.Cargo;
                                detalle.Area = tercero.Area;
                                detalle.CapacitacionId = id;

                                db.DetalleCapacitacion.Add(detalle);

                                //var relacion = db.Capacitacion.FirstOrDefault(x => x.Id == id)?.IdentificadorRelacion;
                                //var capP = db.Capacitacion.FirstOrDefault(x => x.IdentificadorRelacion == relacion);
                                //capP.TotalPersonas++;

                                var cap = db.Capacitacion.Find(id);
                                cap.CtnProgramados++;

                            }
                            else
                            {
                                estudiantesYaInscritos++;
                            }
                        }
                        db.SaveChanges();

                        TempData["Mensaje"] = "Se añadieron los estudiantes correctamente. Se omitió el registro de " + estudiantesOmitidos + " estudiantes. Y se omitió la inscripción de " + estudiantesYaInscritos + " estudiantes";

                    }
                }
                else
                {
                    TempData["Mensaje"] = "Error. Verifique el archivo subido.";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Hubo un error: " + ex;
            }

            return RedirectToAction("Estudiantes");
        }

        public ActionResult CerrarCap(string Id)
        {

            Int32.TryParse(Id, out int id);

            using (var db = new AutogestionContext())
            {
                //ArrayList lst = new ArrayList();
                ////model.ListadoTiposInc = db.TiposIncapacidad.Where(e => e.EstadoId == 13).ToList();
                //var terceros = db.Tercero.Select(e => new { e.Id, e.Nombres, e.Superior, e.Documento, e.Area, e.Cargo, nomcodigo = e.Id + "-" + e.Documento + "-" + e.Area + "-" + e.Cargo + "-" + e.Superior + "-" + "SI" }).ToArray();
                //foreach (var item in terceros)
                //{
                //    lst.Add(item);

                //}
                //var emp3 = (from emp in db.Empleados
                //            join e in db.Empleados on emp.Jefe equals e.NroEmpleado into personal
                //            from ps in personal.DefaultIfEmpty()
                //            where emp.Activo != "NO" || emp.FechaFin >= DateTime.Now
                //            select new { emp.Id, emp.Documento, emp.Cargo, Jefe = emp.Jefe, emp.Nombres, emp.Activo, nomcodigo = emp.Id + "-" + emp.Documento + "-" + emp.Area + "-" + emp.Cargo + "-" + ps.Nombres + "-" + "NO", emp.FechaFin }
                //  ).ToArray();

                //foreach (var item in emp3)
                //{
                //    lst.Add(item);

                //}

                //ViewBag.Empleado = lst;
                //ViewBag.Empleado = db.Empleados.Select(e => new { e.Id, e.Nombres, e.Activo, nomcodigo = e.Id + "-" + e.Nombres, e.FechaFin }).Where(f => f.Activo != "NO" || f.FechaFin >= DateTime.Now).ToList();
                //ViewBag.Numero = id;

                var idcap = Convert.ToInt32(Id);
                ViewBag.Capacitacion = db.Capacitacion.FirstOrDefault(x => x.Id == idcap);

                //var competencia = db.CompetenciaLaboral.Where(z => z.Estado == "Activo").Select(x => new { x.Id, x.Nombre, x.Estado }).ToList();
                //var cultura = db.CulturaOrganizacional.Where(z => z.Estado == "Activo").Select(x => new { x.Id, x.Nombre }).ToList();
                //var metodologia = db.Metodologia.Where(z=>z.Estado=="Activo").Select(x => new { x.Id, x.Nombre }).ToList();
                //ViewBag.Competencia = competencia;
                //ViewBag.Cultura = cultura;
                //ViewBag.Metodologia = metodologia;

                var query = (from c in db.DetalleCapacitacion
                             where c.CapacitacionId == id && c.Estado == "Activo"
                             select c).Count();
                ViewBag.programados = query;

                var query2 = (from c in db.DetalleCapacitacion
                              where c.CapacitacionId == id && c.FechaFirma != null
                              select c).Count();

                ViewBag.Asistentes = query2;

                if (query != 0)
                {
                    var calcular = (query2 * 100) / query;
                    ViewBag.Cobertura = calcular;

                }
                else
                {
                    ViewBag.Cobertura = 0;
                }

            }

            return PartialView();
        }
        public JsonResult CerrarCap2()
        {
            var respuesta = "0";
            try
            {
                var IdCap = HttpContext.Request.Params["IdCapacitacion"];
                //var Conocimientos = HttpContext.Request.Params["Conocimientos"];
                //var MedicionE = HttpContext.Request.Params["Medicion"];
                //var Competencia = HttpContext.Request.Params["Competencia"];
                //var CulturaO = HttpContext.Request.Params["Cultura"];
                //var MetaE = HttpContext.Request.Params["Meta"];
                //var ResultadoM = HttpContext.Request.Params["Resultado"];
                //var Metodologia = HttpContext.Request.Params["Metodología"];
                //var Cobertura = HttpContext.Request.Params["Cobertura"];
                //var CtnAsistentes = HttpContext.Request.Params["CantidadAsistentes"];
                //var CtnProgramados = HttpContext.Request.Params["CantidadProgramados"];

                using (var db = new AutogestionContext())
                {
                    var id = Convert.ToInt32(IdCap);
                    var ModalidadCapacitacion = db.Capacitacion.FirstOrDefault(z => z.Id == id)?.Modalidad;
                    var NombreCap = db.Capacitacion.FirstOrDefault(z => z.Id == id)?.Nombre;
                    
                    if (ModalidadCapacitacion != "4")
                    {
                        var relacion = db.Capacitacion.FirstOrDefault(x => x.Id == id)?.IdentificadorRelacion;
                        var capP = db.Capacitacion.FirstOrDefault(x => x.IdentificadorRelacion == relacion);
                        var cap = db.Capacitacion.FirstOrDefault(x => x.Id == id);

                        var cantidadSesiones = capP.TotalSesiones;
                        var actualCobertura = double.TryParse(capP.Cobertura, out double actualCoberturaDouble);

                        var detalle = db.DetalleCapacitacion.Any(x => x.CapacitacionId == id && x.FechaFirma != null);

                        double Cobertura = 0.0;

                        if (detalle)
                        {
                            if (cantidadSesiones > 0)
                            {
                                double calculoCobertura = 1.0 * 100 / cantidadSesiones;
                                double coberturaRedondeada = Math.Round(calculoCobertura, 2);
                                actualCoberturaDouble += (double)coberturaRedondeada;
                                if (actualCoberturaDouble == 99.99)
                                {
                                    Cobertura = 100.00;
                                }
                                else
                                {
                                    Cobertura = actualCoberturaDouble;
                                }
                            }
                            else
                            {
                                Cobertura = actualCoberturaDouble;
                            }
                        }
                        else
                        {
                            Cobertura = actualCoberturaDouble;
                        }
                        string CoberturaString = Cobertura.ToString("F2");

                        _repo.Cerrar(IdCap, CoberturaString);

                    }
                    else
                    {

                        var relacion = db.Capacitacion.FirstOrDefault(x => x.Id == id)?.IdentificadorRelacion;
                        var capP = db.Capacitacion.FirstOrDefault(x => x.IdentificadorRelacion == relacion);

                        //AL SER CURSO EDUFOSCAL SOLO HAY FIRMAS DE ASISTENCIAS EN LA PRIMERA SESION
                        var detalle = db.DetalleCapacitacion.Any(x => x.CapacitacionId == capP.Id && x.FechaFirma == "CERTIFICADO");

                        //LA COBERTURA SIEMPRE SERÁ 100 SIN IMPORTAR CUAL SEA LA SESIÓN QUE SE CIERRE
                        string CoberturaString = "100,00";

                        _repo.Cerrar(IdCap, CoberturaString);
                    }
                    respuesta = "Se ha cerrado la acción de formación " + NombreCap + " de manera exitosa";

                }
                return Json(new
                {
                    respuesta,
                    isRedirect = true
                });
            }
            catch (SystemException ex)
            {
                respuesta = String.Format("Error al guardar. {0}", ex.Message);
                return Json(new
                {
                    respuesta,
                    isRedirect = false
                });


            }


        }

        //
        // GET: /Capacitacion/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Capacitacion/Create
        public ActionResult Create()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("CrearCapacitacion"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Sede = db.Sede.Select(z => new { z.Id, z.Nombre }).ToList();
            ViewBag.Sociedad = db.Sociedad.Select(y => new { y.Codigo, y.Descripcion }).ToList();
            ViewBag.Metodologia = db.Metodologia.Where(u => u.Estado == "Activo").Select(x => new { x.Id, x.Nombre }).ToList();
            ViewBag.PICO = db.ProgramaInstitucional.Where(x => x.Activo == true).Select(x => new { x.Id, x.Valor }).ToList();
            ViewBag.DirigidoA = db.DirigidoA.Where(x => x.Activo == true).Select(x => new { x.Id, x.Valor }).ToList();
            ViewBag.ReqInst = db.RequerimientoInstitucional.Where(x => x.Activo == true).Select(x => new { x.Id, x.Valor }).ToList();
            ViewBag.Ciudad = db.Municipio.Where(x => x.Nombre == "BUCARAMANGA" || x.Nombre == "FLORIDABLANCA" || x.Nombre == "GIRON" || x.Nombre == "PIEDECUESTA" || x.Nombre == "SAN GIL").Select(x => new { x.Id, x.Nombre }).ToList();
            ViewBag.Lugares = db.LugaresCapacitacion.Select(x => new { x.Id, Nombre = x.Nombre.ToUpper() }).ToList();
            ViewBag.EncuestaSatisfaccion = db.Configuraciones.FirstOrDefault(x => x.Parametro == "EncuestaSatisfaccion")?.Valor;
            ViewBag.Docentes = db.Empleados.Where(x => x.Activo == "SI").Select(x => new { Id = x.Nombres, Nombres = x.Nombres }).ToList();
            ViewBag.CursosNormativa = db.CursosNormativa.Where(x => x.Estado == "Activo").Select(x => new { Id = x.Titulo, Valor = x.Titulo }).ToList();
            
            var userId = Convert.ToInt16(Session["Empleado"]);

            ViewBag.Responsable = db.Empleados.FirstOrDefault(x => x.Id == userId)?.Nombres;
            ViewBag.CargoResponsable = db.Empleados.FirstOrDefault(x => x.Id == userId)?.Cargo;
            ViewBag.AreaResponsable = db.Empleados.FirstOrDefault(x => x.Id == userId)?.AreaDescripcion;

            ViewBag.TipoPEC = "";

            if (funciones.Contains("CrearCapacitacionJefe"))
            {
                ViewBag.TipoPEC = db.TipoPEC.Where(x => x.Activo == true && x.Id == 1).Select(x => new { x.Id, x.Valor }).ToList();
            }

            if (funciones.Contains("CrearCapacitacionPICO"))
            {
                ViewBag.TipoPEC = db.TipoPEC.Where(x => x.Activo == true && x.Id == 2).Select(x => new { x.Id, x.Valor }).ToList();
            }

            if (funciones.Contains("CrearCapacitacionJefe") && funciones.Contains("CrearCapacitacionPICO"))
            {
                ViewBag.TipoPEC = db.TipoPEC.Where(x => x.Activo == true && (x.Id == 1 || x.Id == 2) ).Select(x => new { x.Id, x.Valor }).ToList();
            }

            if (funciones.Contains("CrearCapacitacionAdmin"))
            {
                ViewBag.TipoPEC = db.TipoPEC.Where(x => x.Activo == true).Select(x => new { x.Id, x.Valor }).ToList();

            }

            return View();
        }

        //
        // POST: /Capacitacion/Create

        public JsonResult CrearCap()
        {
            try
            {
                var CursoId = HttpContext.Request.Params["CursoId"];
                var Nombre = HttpContext.Request.Params["Nombre"];
                var HoraInicio = HttpContext.Request.Params["HoraInicio"];
                var Mes = HttpContext.Request.Params["Mes"];
                var HoraFin = HttpContext.Request.Params["HoraFin"];
                var IdTipoPEC = HttpContext.Request.Params["IdTipoPEC"];
                var Empresa = HttpContext.Request.Params["Empresa"];
                var IdSede = HttpContext.Request.Params["IdSede"];
                var Responsable = HttpContext.Request.Params["Responsable"];
                var CargoResponsable = HttpContext.Request.Params["CargoResponsable"];
                var IdProgramaInstitucional = HttpContext.Request.Params["IdProgramaInstitucional"];
                var ResponsablePrograma = HttpContext.Request.Params["ResponsablePrograma"];
                var CargoResponsablePrograma = HttpContext.Request.Params["CargoResponsablePrograma"];
                var AreaObjetivo = HttpContext.Request.Params["AreaObjetivo"];
                var TotalHoras = HttpContext.Request.Params["TotalHoras"];
                var TotalSesiones = HttpContext.Request.Params["TotalSesiones"];
                var DirigidoASelect = HttpContext.Request.Params["DirigidoASelect"];
                var Proveedor = HttpContext.Request.Params["Proveedor"];
                var Docente = HttpContext.Request.Params["Docente"];
                var MetaEficacia = HttpContext.Request.Params["MetaEficacia"];
                var IdRequerimientoInstitucional = HttpContext.Request.Params["IdRequerimientoInstitucional"];
                var EspecificacionReq = HttpContext.Request.Params["EspecificacionReq"];
                var Metodologia = HttpContext.Request.Params["Metodologia"];
                var CtnProgramados = HttpContext.Request.Params["CtnProgramados"];
                var Modalidad = HttpContext.Request.Params["Modalidad"];
                var Ciudad = HttpContext.Request.Params["Ciudad"];
                var OtraCiudad = HttpContext.Request.Params["OtraCiudad"];
                var Lugar = HttpContext.Request.Params["Lugar"];
                var OtroLugar = HttpContext.Request.Params["OtroLugar"];
                var PresupuestoRequerido = HttpContext.Request.Params["PresupuestoRequerido"];
                var Presupuesto = HttpContext.Request.Params["Presupuesto"];
                var EvaluacionConocimiento = HttpContext.Request.Params["EvaluacionConocimiento"];
                var EncuestaSatisfaccion = HttpContext.Request.Params["EncuestaSatisfaccion"];
                var Objetivo = HttpContext.Request.Params["Objetivo"];
                var temas = HttpContext.Request.Params["temas"];
                var CantidadSesiones = HttpContext.Request.Params["CantidadSesiones"];
                string[] FechaCapacitaciones = new string[Convert.ToInt32(CantidadSesiones)];

                for (var i = 0; i < Convert.ToInt32(CantidadSesiones); i++)
                {
                    FechaCapacitaciones[i] = HttpContext.Request.Params["FechaCapacitacion_" + (i + 1)];
                }

                Capacitacion model = new Capacitacion();

                model.CursoId = CursoId;
                //model.Nombre = Nombre;
                model.HoraInicio = HoraInicio;
                model.Mes = DateTime.Parse(Mes);
                model.HoraFin = HoraFin;

                if (IdTipoPEC != "")
                {
                    model.IdTipoPEC = int.Parse(IdTipoPEC);
                }

                model.Empresa = Empresa;
                if (IdSede != "")
                {
                    model.IdSede = int.Parse(IdSede);
                
                }

                if (IdTipoPEC == "2")
                {
                    model.ResponsablePrograma = ResponsablePrograma;
                    model.CargoResponsablePrograma = CargoResponsablePrograma;

                    if (IdProgramaInstitucional != "")
                    {
                        model.IdProgramaInstitucional = int.Parse(IdProgramaInstitucional);
                    }
                }
                else
                {
                    model.Responsable = Responsable;
                    model.CargoResponsable = CargoResponsable;
                }

                model.AreaObjetivo = AreaObjetivo;
                model.TotalHoras = TotalHoras;

                if (TotalSesiones != "")
                {
                    model.TotalSesiones = int.Parse(TotalSesiones);
                }

                model.DirigidoASelect = DirigidoASelect;
                model.Proveedor = Proveedor;
                model.Docente = Docente;
                model.MetaEficacia = MetaEficacia;

                if (IdRequerimientoInstitucional != "")
                {
                    model.IdRequerimientoInstitucional = int.Parse(IdRequerimientoInstitucional);
                }
                model.EspecificacionReq = EspecificacionReq;

                if (Metodologia != "")
                {
                    model.Metodologia = int.Parse(Metodologia);
                }

                if (int.TryParse(CtnProgramados, out int TotalPersonas))
                {
                    model.CtnProgramados = TotalPersonas;
                }

                model.Modalidad = Modalidad;
                model.Ciudad = Ciudad;

                if (Ciudad == "OTRO")
                {
                    model.Ciudad = OtraCiudad;
                }

                model.Lugar = Lugar;

                if (Lugar == "OTRO")
                {
                    model.Lugar = OtroLugar;
                }

                model.PresupuestoRequerido = bool.Parse(PresupuestoRequerido);
                model.Presupuesto = Presupuesto;
                model.EvaluacionConocimiento = EvaluacionConocimiento;
                model.EncuestaSatisfaccion = EncuestaSatisfaccion;
                model.Objetivo = Objetivo;
                model.temas = temas;

                var userlog = Convert.ToInt16(Session["Empleado"]);

                return Json(_repo.Crear(model, userlog, FechaCapacitaciones, Nombre));

            }
            catch (Exception ex)
            {
                return Json("Error:" + ex);
            }
        }

        public ActionResult RegistroTercero(string id, string perfil)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            var Id = Convert.ToInt32(id);

            Capacitacion capacitacion = db.Capacitacion.Find(Id);

            ViewBag.Empresas = db.Empresas.Where(x => x.Activo == true).Select(x => new { x.Id, x.Nombre }).ToList();

            return View(capacitacion);
        }

        [HttpPost]
        public JsonResult CrearTercero()
        {
            try
            {
                var Id = HttpContext.Request.Params["Id"];
                var Perfil = HttpContext.Request.Params["Perfil"];
                var IdCap = int.Parse(Id);
                var CrearNombre = HttpContext.Request.Params["CrearNombre"];
                var CrearDocumento = HttpContext.Request.Params["CrearDocumento"];
                var CrearCorreo = HttpContext.Request.Params["CrearCorreo"];
                var CrearArea = HttpContext.Request.Params["CrearArea"];
                var CrearCargo = HttpContext.Request.Params["CrearCargo"];
                var CrearSociedad = HttpContext.Request.Params["CrearSociedad"];

                Tercero model = new Tercero();

                if (Perfil == "tercero")
                {
                    model.Nombres = CrearNombre.ToUpper();
                    model.Documento = CrearDocumento;
                    model.CorreoPersonal = CrearCorreo;
                    model.Area = CrearArea.ToUpper();
                    model.Cargo = CrearCargo.ToUpper();
                    model.SociedadCOD = CrearSociedad;
                }

                if (Perfil == "estudiante")
                {
                    model.Nombres = CrearNombre.ToUpper();
                    model.Documento = CrearDocumento;
                    model.CorreoPersonal = CrearCorreo;
                    model.Area = CrearArea.ToUpper();
                    model.Cargo = CrearCargo.ToUpper();
                    model.Universidad = CrearSociedad;
                    model.SociedadCOD = "1";
                    model.Estudiante = "SI";
                }
                
                if (Perfil == null)
                {
                    model.Nombres = CrearNombre.ToUpper();
                    model.Documento = CrearDocumento;
                    model.CorreoPersonal = CrearCorreo;
                    model.Area = CrearArea.ToUpper();
                    model.Cargo = CrearCargo.ToUpper();
                    model.SociedadCOD = CrearSociedad;
                }

                return Json(_repo.CrearTercero(model, IdCap));
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex);
            }
        }

        [HttpPost]
        public JsonResult ValidarRegistroTercero()
        {
            var Id = HttpContext.Request.Params["Id"];
            var Cedula = HttpContext.Request.Params["Cedula"];

            try
            {
                if (string.IsNullOrEmpty(Cedula))
                {
                    return Json("Verifique que el número de documento esté escrito correctamente");
                }

                var IdCap = int.Parse(Id);

                return Json(_repo.ValidarRegistroTercero(Cedula, IdCap));
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex);
            }
        }

        public ActionResult FirmaAsistencia(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            var Id = Convert.ToInt32(id);

            Capacitacion capacitacion = db.Capacitacion.Find(Id);

            return View(capacitacion);
        }

        [HttpPost]
        public JsonResult FirmaEmpleado()
        {
            var Id = HttpContext.Request.Params["Id"];
            var Cedula = HttpContext.Request.Params["Cedula"];
            var TipoPersona = HttpContext.Request.Params["TipoPersona"];
            var EsFirmaAbierta = HttpContext.Request.Params["EsFirmaAbierta"];

            try
            {

                if (string.IsNullOrEmpty(Cedula))
                {
                    return Json("Verifique que el número de documento esté escrito correctamente");
                }

                if (string.IsNullOrEmpty(TipoPersona))
                {
                    return Json("Debe seleccionar el Tipo de Usuario primero");
                }

                var IdCap = int.Parse(Id);

                if (EsFirmaAbierta == "true")
                {

                    if (TipoPersona == "Tercero")
                    {
                        return Json("Esta sesión no permite firmas de terceros");
                    }

                    var empleado = db.Empleados.FirstOrDefault(x => x.Documento == Cedula && x.Activo == "SI");

                    if (empleado != null)
                    {
                        List<DetalleCapacitacion> model = new List<DetalleCapacitacion>();
                        var detalle = new DetalleCapacitacion();

                        if (!db.DetalleCapacitacion.Any(x => x.CapacitacionId == IdCap && x.EmpleadoId == empleado.Id))
                        {
                            detalle.Estado = "Activo";
                            detalle.EsTercero = "NO";
                            detalle.EmpleadoId = empleado.Id;
                            detalle.Cargo = empleado.Cargo;
                            detalle.Area = empleado.AreaDescripcion;
                            detalle.CapacitacionId = IdCap;
                            model.Add(detalle);

                            return Json(_repo2.Crear(model, IdCap, 1, true));

                        }
                        else
                        {
                            return Json(_repo.FirmaEmpleado(Cedula, IdCap, TipoPersona));
                        }

                    }
                    else
                    {
                        return Json("No existe empleado con el número de documento proporcionado.");
                    }

                }
                else
                {
                    return Json(_repo.FirmaEmpleado(Cedula, IdCap, TipoPersona));
                }


            }
            catch (Exception ex)
            {
                return Json("Error: " + ex);
            }

        }

        [HttpPost]
        public JsonResult FirmaPruebas()
        {
            var id = HttpContext.Request.Params["Id"];
            var Id = Convert.ToInt32(id);

            var registro = db.DetalleCapacitacion.FirstOrDefault(x => x.Id == Id);
            registro.FechaFirma = DateTime.Now.ToString();

            var cap = db.Capacitacion.FirstOrDefault(x => x.Id == registro.CapacitacionId);
            cap.CtnAsistentes++;

            db.SaveChanges();

            return Json("true");
        }

        [HttpPost]
        public String ObtenerQR(string id, string url)
        {
            try
            {

                string textoqr = url + "FirmaAsistencia" + "/" + id;
                byte[] ImagenQR = _servicios.GenerarQR(textoqr);
                return Convert.ToBase64String(ImagenQR);

            }
            catch (Exception e)
            {

                return "error" + e.Message.ToString();
            }

        }

        [HttpPost]
        public String ObtenerQRE(string url1)
        {
            try
            {
                if (!string.IsNullOrEmpty(url1))
                {
                    byte[] ImagenQR = _servicios.GenerarQR(url1);
                    return Convert.ToBase64String(ImagenQR);
                }
                else
                {
                    return null;
                }


            }
            catch (Exception e)
            {

                return "error" + e.Message.ToString();
            }

        }

        [HttpPost]
        public String ObtenerQRS(string url2)
        {
            try
            {
                if (!string.IsNullOrEmpty(url2))
                {
                    byte[] ImagenQR = _servicios.GenerarQR(url2);
                    return Convert.ToBase64String(ImagenQR);
                }
                else
                {
                    return null;
                }


            }
            catch (Exception e)
            {

                return "error" + e.Message.ToString();
            }

        }

        //public JsonResult BuscarDatosResponsable()
        //{
        //    try
        //    {
        //        var Cedula = HttpContext.Request.Params["cedula"];

        //        return Json(_repo.BuscarDatosResponsable(Cedula));
        //    }
        //    catch
        //    {
        //        return Json("Error");
        //    }

        //}

        //
        // GET: /Capacitacion/Edit/5
        public ActionResult Edit(int id)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EditarCapacitacion"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var capacitacion = db.Capacitacion.FirstOrDefault(x => x.Id == id);

                ViewBag.Capacitacion = capacitacion;
                Capacitacion model = db.Capacitacion.Find(id);

                if (model == null)
                {
                    return HttpNotFound();
                }

                model.Empleado = db.Empleados.FirstOrDefault(x => x.Id == model.EmpleadoRegistraId);

                ViewBag.TipoPEC = db.TipoPEC.FirstOrDefault(x => x.Id == model.IdTipoPEC)?.Valor;
                //model.ListEmpresa = db.Sociedad.ToList();
                ViewBag.Empresa = db.Sociedad.FirstOrDefault(x => x.Codigo == model.Empresa)?.Descripcion;
                //model.ListSede = db.Sede.ToList();
                ViewBag.Sede = db.Sede.FirstOrDefault(x => x.Id == model.IdSede)?.Nombre;
                //ViewBag.PICO = db.ProgramaInstitucional.Where(x => x.Activo == true).Select(x => new { x.Id, x.Valor }).ToList();
                ViewBag.PICO = db.ProgramaInstitucional.FirstOrDefault(x => x.Id == model.IdProgramaInstitucional)?.Valor;
                //ViewBag.ReqInst = db.RequerimientoInstitucional.Where(x => x.Activo == true).Select(x => new { x.Id, x.Valor }).ToList();
                ViewBag.ReqInst = db.RequerimientoInstitucional?.FirstOrDefault(x => x.Id == model.IdRequerimientoInstitucional)?.Valor;
                //model.ListMetodología = db.Metodologia.ToList();
                ViewBag.Metodologia = db.Metodologia.FirstOrDefault(x => x.Id == model.Metodologia)?.Nombre;

                ViewBag.dirigidoArray = db.DirigidoA.Where(x => x.Activo == true).Select(x => new { x.Id, x.Valor }).ToArray();
                ViewBag.numeros = model.DirigidoASelect;
                //ViewBag.DirigidoA = db.DirigidoA.Where(x => x.Activo == true).Select(x=> new { x.Id, x.Valor}).ToList();


                ViewBag.Ciudad = db.Municipio.Where(x => x.Nombre == "BUCARAMANGA" || x.Nombre == "FLORIDABLANCA" || x.Nombre == "GIRON" || x.Nombre == "PIEDECUESTA" || x.Nombre == "SAN GIL").Select(x => new { Id = x.Nombre, x.Nombre }).ToList();
                ViewBag.Lugares = db.LugaresCapacitacion.Select(x => new { Id = x.Nombre.ToUpper(), Nombre = x.Nombre.ToUpper() }).ToList();

                ViewBag.selectedCiudad = db.Capacitacion.FirstOrDefault(x => x.Id == id)?.Ciudad;

                int añoActual = DateTime.Now.Year;

                var fechaInicio = new DateTime(añoActual, 1, 1);
                var fechaFin = new DateTime(añoActual, 12, 31);

                var festivos = db.DiasFestivos.Where(x => DbFunctions.TruncateTime(x.festivo) >= fechaInicio && DbFunctions.TruncateTime(x.festivo) <= fechaFin)
                    .ToList();
                
                List<string> festivosColombia = new List<string>();

                foreach (var festivo in festivos)
                {
                    festivosColombia.Add($"new Date('{festivo.festivo:yyyy-MM-dd}'),");
                }

                ViewBag.FestivosColombia = string.Join(Environment.NewLine, festivosColombia);


                var errMsg = TempData["ErrorMessage"] as string;
                ViewBag.Message = errMsg;

                return View(model);

            }
            catch (SystemException x)
            {

            }
            return View();
        }

        //
        // POST: /Capacitacion/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Capacitacion model)
        {
            string message;
            try
            {
                var detalle = db.Capacitacion.Find(id);

                if (model.Estado == "Cerrado")
                {
                    message = "No es posible modificar  el registro debido a que se encuentra en estado " + model.Estado + ".";
                    TempData["ErrorMessage"] = message;

                    return RedirectToAction("/Edit/" + id);

                }

                else
                {

                    message = _repo.Editar(id, model);
                    TempData["ErrorMessage"] = message;

                    return RedirectToAction("/Edit/" + id);
                }

            }
            catch (SystemException EX)
            {
                message = "ERROR: " + EX;
                TempData["ErrorMessage"] = message;

                return View(message);
            }
        }

        //
        // GET: /Capacitacion/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Capacitacion/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult ListaPorCompletar()
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ListadoCompletarCap"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            string usuario = String.Format("{0}", Session["Empleado"]);
            //ArrayList lst = new ArrayList();
            //var terceros = db.Tercero.Select(e => new { e.Id, e.Nombres, e.Superior, e.Documento, e.Area, e.Cargo, nomcodigo = e.Id + "-" + e.Documento + "-" + e.Area + "-" + e.Cargo + "-" + e.Superior + "-" + "SI" }).ToArray();
            //foreach (var item in terceros)
            //{
            //    lst.Add(item);

            //}
            //var emp3 = (from emp in db.Empleados
            //            join e in db.Empleados on emp.Jefe equals e.NroEmpleado into personal
            //            from ps in personal.DefaultIfEmpty()
            //            where emp.Activo != "NO" || emp.FechaFin >= DateTime.Now
            //            select new { emp.Id, emp.Documento, emp.Cargo, Jefe = emp.Jefe, emp.Nombres, emp.Activo, nomcodigo = emp.Id + "-" + emp.Documento + "-" + emp.Area + "-" + emp.Cargo + "-" + ps.Nombres + "-" + "NO", emp.FechaFin }
            //  ).ToArray();

            //foreach (var item in emp3)
            //{
            //    lst.Add(item);

            //}


            //ViewBag.Empleado = lst;
            var Cap = _repo.ObtenerTodos(usuario);


            return View(Cap);
        }

        public ActionResult PendientePorFirmar(int id)
        {
            using (var db = new AutogestionContext())
            {

                List<DetalleCapacitacion> detalle = new List<DetalleCapacitacion>();
                detalle = db.DetalleCapacitacion.Where(x => x.CapacitacionId == id && x.FechaFirma == null && x.Estado == "Activo").ToList();

                foreach (DetalleCapacitacion Item in detalle)
                {

                    var Tercero = Item.EsTercero;
                    if (Tercero == "NO")
                    {
                        Item.Empleado2 = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    }
                    else
                    {
                        var TerceroId = Convert.ToInt32(Item.TerceroId);
                        Item.Tercero2 = db.Tercero.FirstOrDefault(e => e.Id == TerceroId);
                    }
                    //Item.Sede2 = db.Sede.FirstOrDefault(s => s.Id == Item.SedeId);
                }

                if (detalle == null)
                {
                    return HttpNotFound();
                }
                return PartialView(detalle);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ListarCursos()
        {
            var r = await _EduFoscalRepository.ListarCursosAF();

            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public String EnviarEmailFirma(int id)
        {

            List<DetalleCapacitacion> Detalle = new List<DetalleCapacitacion>();

            using (var db = new AutogestionContext())
            {

                try
                {

                    Detalle = db.DetalleCapacitacion.Where(x => x.CapacitacionId == id).ToList();

                    foreach (var Item in Detalle)
                    {
                        Empleado empleado = new Empleado();
                        Tercero empleado2 = new Tercero();

                        var Tercero = Item.EsTercero;
                        if (Tercero == "NO")
                        {
                            empleado = db.Empleados.FirstOrDefault(x => x.Id == Item.EmpleadoId);
                            if (Item.FechaFirma == null)
                            {
                                _servicios.EnviarEmailCAP(empleado, Item);
                            }
                        }
                        else
                        {
                            var TerceroId = Convert.ToInt32(Item.TerceroId);
                            empleado2 = db.Tercero.FirstOrDefault(e => e.Id == TerceroId);

                            if (Item.FechaFirma == null)
                            {
                                _servicios.EnviarEmailCAP2(empleado2, Item);
                            }
                        }

                    }

                    return "OK";
                }
                catch (Exception ex)
                {

                    return ex.Message.ToString();
                }


            }

        }

        public ActionResult DetalleCapacitacion(string Empresa, string TipoPEC, string Area, int? PICO, int? Year)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("InformeGeneralCap"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            using (var db = new AutogestionContext())
            {

                ViewBag.Empresa = db.Sociedad.Select(y => new { y.Codigo, y.Descripcion }).ToList();
                ViewBag.TipoPEC = db.TipoPEC.Select(x => new { x.Id, x.Valor }).ToList();
                var areasF = db.Empleados
                .Where(x => !string.IsNullOrEmpty(x.Area) && !string.IsNullOrEmpty(x.AreaDescripcion) && x.Activo == "SI" && x.Empresa == "1000")
                .Select(x => new { x.AreaDescripcion })
                .GroupBy(x => x.AreaDescripcion)
                .Select(g => g.FirstOrDefault())
                .ToList();

                var selectListF = areasF.Select(a => new
                {
                    Value = a.AreaDescripcion,
                    Text = a.AreaDescripcion
                }).ToList();

                var areasFU = db.Empleados
                .Where(x => !string.IsNullOrEmpty(x.Area) && !string.IsNullOrEmpty(x.AreaDescripcion) && x.Activo == "SI" && x.Empresa == "2000")
                .Select(x => new { x.AreaDescripcion })
                .GroupBy(x => x.AreaDescripcion)
                .Select(g => g.FirstOrDefault())
                .ToList();

                var selectListFU = areasFU.Select(a => new
                {
                    Value = a.AreaDescripcion,
                    Text = a.AreaDescripcion
                }).ToList();

                ViewBag.AreasF = selectListF;
                ViewBag.AreasFU = selectListFU;

                ViewBag.AreasFJson = JsonConvert.SerializeObject(selectListF);
                ViewBag.AreasFUJson = JsonConvert.SerializeObject(selectListFU);

                ViewBag.PICO = db.ProgramaInstitucional.Where(x => x.Activo == true).Select(x => new { x.Id, x.Valor }).ToList();

                var YearInt = 2024;
                if (Year != null)
                {
                    YearInt = Convert.ToInt32(Year);
                }

                if (!string.IsNullOrEmpty(TipoPEC))
                {
                    DateTime Fecha1 = new DateTime(YearInt, 1, 1);
                    DateTime Fecha2 = new DateTime(YearInt, 12, 31);
                    var empid = Session["Empleado"].ToString();
                    var idint = int.Parse(empid);
                    var tipopec = Convert.ToInt32(TipoPEC);

                    var query = db.Capacitacion.AsQueryable();

                    query = query.Where(e => DbFunctions.TruncateTime(e.Mes) >= Fecha1 &&
                                             DbFunctions.TruncateTime(e.Mes) <= Fecha2 &&
                                             e.Empresa == Empresa);

                    if (tipopec == 1)
                    {
                        query = query.Where(e => e.AreaObjetivo == Area && e.IdTipoPEC == 1);
                    }
                    else if (tipopec == 2)
                    {
                        query = query.Where(e => e.IdProgramaInstitucional == PICO && e.IdTipoPEC == 2);
                    }
                    else if (tipopec == 3)
                    {
                        query = query.Where(e => e.IdTipoPEC == 3);
                    }

                    var capacitacionRaw = query.ToList();

                    var capacitacion = capacitacionRaw
                        .GroupBy(e => e.IdentificadorRelacion)
                        .Select(g => new
                        {
                            IdentificadorRelacion = g.Key,
                            TotalHoras = g.Sum(e =>
                            {
                                decimal horas;
                                return decimal.TryParse(e.TotalHoras, NumberStyles.Any, CultureInfo.InvariantCulture, out horas) ? horas : 0;
                            }),
                            TotalAsistentes = g.Sum(x => x.CtnAsistentes),
                            TotalPersonas = g.Sum(x => x.CtnProgramados),
                            PrimerRegistro = g.FirstOrDefault()
                        })
                        .ToList();

                    Dictionary<string, string> Modalidades = new Dictionary<string, string>
                    {
                        { "1", "Presencial" },
                        { "2", "Virtual con conectividad" },
                        { "3", "Híbrida" },
                        { "4", "Plataforma Virtual Formativa EduFoscal" }
                    };

                    var empleados = db.Empleados.ToDictionary(e => e.Id);
                    var tiposPEC = db.TipoPEC.ToDictionary(t => t.Id);
                    var sociedades = db.Sociedad.ToDictionary(x => x.Codigo, x => x.Descripcion);
                    var programas = db.ProgramaInstitucional.ToDictionary(x => x.Id, x => x.Valor);
                    var requerimientos = db.RequerimientoInstitucional.ToDictionary(x => x.Id, x => x.Valor);

                    var capacitacionConHoras = capacitacion.Select(g =>
                    {
                        var item = g.PrimerRegistro;
                        if (item != null)
                        {
                            item.TotalHoras = g.TotalHoras.ToString();

                            var idsRelacionados = db.Capacitacion
                                .Where(e => e.IdentificadorRelacion == g.IdentificadorRelacion)
                                .Select(e => e.Id)
                                .ToList();

                            var listaAreas = db.DetalleCapacitacion
                                .Where(x => idsRelacionados.Contains(x.CapacitacionId) && x.Estado == "Activo")
                                .Select(x => x.Area)
                                .Distinct()
                                .ToList();

                            item.Dependencias = string.Join("% ", listaAreas);

                            item.Empleado = empleados.TryGetValue(item.EmpleadoRegistraId, out var empleado) ? empleado : null;
                            item.TipoPEC = tiposPEC.TryGetValue(item.IdTipoPEC, out var tipo) ? tipo?.Valor : null;
                            item.Empresa = sociedades.TryGetValue(item.Empresa, out var empresaDescripcion) ? empresaDescripcion : null;
                            item.ProgramaInstitucional = programas.TryGetValue(item.IdProgramaInstitucional, out var programa) ? programa : null;
                            item.RequerimientoInstitucional = requerimientos.TryGetValue(item.IdRequerimientoInstitucional, out var requerimiento) ? requerimiento : null;
                            item.Modalidad2 = Modalidades.TryGetValue(item.Modalidad, out var modalidad) ? modalidad : null;
                            item.CtnAsistentes = g.TotalAsistentes;
                            item.CtnProgramados = g.TotalPersonas;
                            var totalAsistentes = g.TotalAsistentes;
                            var totalProgramados = g.TotalPersonas;

                            var ids = g.PrimerRegistro.DirigidoASelect.Split(',')
                            .Select(int.Parse)
                            .ToList();

                            item.DirigidoA = string.Join(",", db.DirigidoA
                                .Where(d => ids.Contains(d.Id))
                                .Select(d => d.Valor)
                                .ToList());

                            item.PorcentajeCobertura = totalProgramados != 0
                                ? Convert.ToString(Math.Min((totalAsistentes * 100 / totalProgramados), 100))
                                : "0";

                        }

                        return item;
                    }).ToList();

                    return View(capacitacionConHoras);
                }
                else
                {
                    List<Capacitacion> model = new List<Capacitacion>();
                    return View(model);
                }

            }
            
        }

        public ActionResult Dashboard()
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("InformeGeneralCap"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        public async Task<JsonResult> DatosGraficas()
        {
            var respuesta = await _repo.ObtenerDatosGraficas();
            return Json(respuesta);
        }

        public JsonResult ActualizarDatosCursosEduFoscal()
        {
            var respuesta = _repo.ActualizarDatosCursosEduFoscal();
            return Json(respuesta);
        }

        public class ModeloReportePersonal
        {
            public string Empleado { get; set; }
            public bool Capacitado { get; set; }
            public string Area { get; set; }
            public string Cargo { get; set; }
            public List<string> NombresAF { get; set; }
            public decimal Horas { get; set; }
            public string Documento { get; set; }

        }

        public ActionResult ReportePersonal(int? Periodo, int? Personal, string Empresa, int? Year, string Perfil)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            if (!funciones.Contains("InformeGeneralCap"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            if (Periodo == null || Empresa == null)
            {
                return View(new List<ModeloReportePersonal>());
            }

            //if (Personal == 2)
            //{
            //    if (Empresa == "1000")
            //    {
            //        Empresa = "1";
            //    }else if (Empresa == "2000")
            //    {
            //        Empresa = "2";
            //    }
            //}

            var empleadosQuery = db.Empleados
                .Where(x => x.Activo == "SI" && x.Empresa == Empresa);

            if (Perfil == "Administrativos CO" || Perfil == "Asistenciales CO")
            {
                empleadosQuery = empleadosQuery.Where(x => x.TipoArea == Perfil);
            }

            var totalEmpleados = empleadosQuery.Count();

            var listaEmpleados = empleadosQuery
                .Select(x => new
                {
                    x.Id,
                    x.Nombres,
                    x.AreaDescripcion,
                    x.Cargo,
                    x.Documento
                })
                .ToList();

            var listaTerceros = db.Tercero
                .Where(x => x.Activo == "SI" && x.Estudiante == "NO")
                .Select(x => new { x.Id, x.Nombres, x.Area, x.Cargo, x.Documento })
                .ToList();

            var listaEstudiantes = db.Tercero
                .Where(x => x.Activo == "SI" && x.Estudiante == "SI")
                .Select(x => new { x.Id, x.Nombres, x.Area, x.Cargo, x.Documento })
                .ToList();

            DateTime Fecha1 = new DateTime((int)Year, 1, 1);
            DateTime Fecha2 = new DateTime((int)Year, 12, 31);

            var capacitacionesPorTrimestre = db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Modalidad != "4" && x.Empresa == Empresa)
                .GroupBy(x => new
                {
                    Trimestre = (x.FechaCapacitacion.Month - 1) / 3 + 1,
                    Año = x.Mes.Year
                })
                .Select(g => new
                {
                    Trimestre = g.Key.Trimestre,
                    Año = g.Key.Año,
                    Id = g.Select(x=> x.Id).ToList()
                })
                .ToList();

            List<int> capacitacionesDelPeriodo;

            if (Periodo == 5)
            {
                capacitacionesDelPeriodo = capacitacionesPorTrimestre
                    .SelectMany(x => x.Id)
                    .ToList();
            }
            else
            {
                capacitacionesDelPeriodo = capacitacionesPorTrimestre
                    .Where(x => x.Trimestre == Periodo)
                    .SelectMany(x => x.Id)
                    .ToList();
            }

            var detallesCapacitacion = db.DetalleCapacitacion
                .Where(x => capacitacionesDelPeriodo.Contains(x.CapacitacionId)
                            && x.FechaFirma != null
                            && x.Estado == "Activo")
                .Select(x => new { x.CapacitacionId, x.EmpleadoId, x.TerceroId })
                .ToList();

            var capacitaciones = db.Capacitacion
                .Where(x => capacitacionesDelPeriodo.Contains(x.Id))
                .Select(x => new { x.Id, x.Nombre, x.TotalHoras })
                .ToDictionary(x => x.Id, x => new { x.Nombre, x.TotalHoras});

            List<ModeloReportePersonal> modelo = new List<ModeloReportePersonal>();

            if (Personal == 1)
            {
                var detallesEmpleados = detallesCapacitacion
                    .Where(x => x.EmpleadoId != 0)
                    .GroupBy(x => x.EmpleadoId)
                    .Select(g => new
                    {
                        EmpleadoId = g.Key,
                        CapacitacionNames = g.Select(d => capacitaciones[d.CapacitacionId].Nombre).ToList(),
                        TotalHoras = g.Sum(d =>
                        {
                            decimal horas;
                            return decimal.TryParse(capacitaciones[d.CapacitacionId].TotalHoras, NumberStyles.Any, CultureInfo.InvariantCulture, out horas) ? horas : 0;
                        })
                    })
                    .ToDictionary(g => g.EmpleadoId, g => g);

                modelo = listaEmpleados.Select(i => new ModeloReportePersonal
                {
                    Empleado = i.Nombres,
                    Area = i.AreaDescripcion,
                    Cargo = i.Cargo,
                    Capacitado = detallesEmpleados.ContainsKey(i.Id),
                    NombresAF = detallesEmpleados.ContainsKey(i.Id) ? detallesEmpleados[i.Id].CapacitacionNames : new List<string>(),
                    Horas = detallesEmpleados.ContainsKey(i.Id) ? detallesEmpleados[i.Id].TotalHoras : 0,
                    Documento = i.Documento
                    
                })
                .ToList();
            }

            if (Personal == 2)
            {
                var detallesTerceros = detallesCapacitacion
                    .Where(x => x.TerceroId != null)
                    .GroupBy(x => x.TerceroId)
                    .Select(g => new
                    {
                        TerceroId = g.Key,
                        CapacitacionNames = g.Select(d => capacitaciones[d.CapacitacionId].Nombre).ToList(),
                        TotalHoras = g.Sum(d =>
                        {
                            decimal horas;
                            return decimal.TryParse(capacitaciones[d.CapacitacionId].TotalHoras, NumberStyles.Any, CultureInfo.InvariantCulture, out horas) ? horas : 0;
                        })
                    })
                    .ToDictionary(g => Convert.ToInt32(g.TerceroId), g => new { g.CapacitacionNames, g.TotalHoras });

                modelo = listaTerceros.Select(i => new ModeloReportePersonal
                {
                    Empleado = i.Nombres,
                    Area = i.Area,
                    Cargo = i.Cargo,
                    Capacitado = detallesTerceros.ContainsKey(i.Id),
                    NombresAF = detallesTerceros.ContainsKey(i.Id) ? detallesTerceros[i.Id].CapacitacionNames : new List<string>(),
                    Horas = detallesTerceros.ContainsKey(i.Id) ? detallesTerceros[i.Id].TotalHoras : 0,
                    Documento = i.Documento
                })
                .ToList();
            }

            if (Personal == 3)
            {
                var detallesEstudiantes = detallesCapacitacion
                    .Where(x => x.TerceroId != null)
                    .GroupBy(x => x.TerceroId)
                    .Select(g => new
                    {
                        TerceroId = g.Key,
                        CapacitacionNames = g.Select(d => capacitaciones[d.CapacitacionId].Nombre).ToList(),
                        TotalHoras = g.Sum(d =>
                        {
                            decimal horas;
                            return decimal.TryParse(capacitaciones[d.CapacitacionId].TotalHoras, NumberStyles.Any, CultureInfo.InvariantCulture, out horas) ? horas : 0;
                        })
                    })
                    .ToDictionary(g => Convert.ToInt32(g.TerceroId), g => new { g.CapacitacionNames, g.TotalHoras });

                modelo = listaEstudiantes.Select(i => new ModeloReportePersonal
                {
                    Empleado = i.Nombres,
                    Area = i.Area,
                    Cargo = i.Cargo,
                    Capacitado = detallesEstudiantes.ContainsKey(i.Id),
                    NombresAF = detallesEstudiantes.ContainsKey(i.Id) ? detallesEstudiantes[i.Id].CapacitacionNames : new List<string>(),
                    Horas = detallesEstudiantes.ContainsKey(i.Id) ? detallesEstudiantes[i.Id].TotalHoras : 0,
                    Documento = i.Documento

                })
                .ToList();
            }

            ViewBag.CantidadSI = modelo
                .Where(x => x.Capacitado == true)
                .Select(x => new { x.Capacitado })
                .Count();

            ViewBag.CantidadNO = modelo
                .Where(x => x.Capacitado == false)
                .Select(x => new { x.Capacitado})
                .Count();

            if (Personal == 1)
            {
                double cantidadSI = Convert.ToDouble(ViewBag.CantidadSI);
                double calculo = cantidadSI * 100 / totalEmpleados;
                ViewBag.Porcentaje = calculo.ToString("F2");
                ViewBag.selectedPerfil = Perfil;
            }

            return View(modelo);
        }

        public class ModeloReporteAccion
        {
            public string Accion { get; set; }
            public string Area { get; set; }
            public bool Ejecutada { get; set; }
            public bool EduFoscal { get; set; }
            public decimal TotalHoras { get; set; }
            public int Id { get; set; }
            public string TipoPEC { get; set; }

        }

        public ActionResult ReporteAcciones(int? Periodo, int? Personal, string Empresa, int? Year) { 

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            if (!funciones.Contains("InformeGeneralCap"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            if (Periodo == null || Empresa == null)
            {
                return View(new List<ModeloReporteAccion>());
            }

            DateTime Fecha1 = new DateTime((int)Year, 1, 1);
            DateTime Fecha2 = new DateTime((int)Year, 12, 31);
            var tiposPEC = db.TipoPEC.ToDictionary(t => t.Id);

            var capacitacionesRaw = db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Empresa == Empresa)
                .ToList();

            var capacitacionesPrincipal = capacitacionesRaw
                .Select(x => new
                {
                    x.IdentificadorRelacion,
                    TotalHoras = decimal.Parse(x.TotalHoras, CultureInfo.InvariantCulture),
                    x.FechaCapacitacion,
                    x.Id,
                    x.Nombre,
                    x.AreaObjetivo,
                    x.Cobertura,
                    x.Modalidad,
                    x.DirigidoASelect,
                    x.IdTipoPEC
                })
                .GroupBy(x => new
                {
                    x.IdentificadorRelacion
                })
                .Select(g => new
                {
                    TotalHoras = g.Sum(x => x.TotalHoras),
                    PrimerRegistro = g.FirstOrDefault()
                })
                .ToList();

            var capacitacionesPorTrimestre = capacitacionesPrincipal
                .GroupBy(x => new
                {
                    Trimestre = (x.PrimerRegistro.FechaCapacitacion.Month - 1) / 3 + 1,
                    Año = x.PrimerRegistro.FechaCapacitacion.Year
                })
                .Select(g => new
                {
                    Trimestre = g.Key.Trimestre,
                    Año = g.Key.Año,
                    Id = g.Select(x => x.PrimerRegistro.Id).ToList(),
                    TotalHoras = g.Sum(x => x.TotalHoras)
                })
                .ToList();

            List<int> capacitacionesDelPeriodo;

            if (Periodo == 5)
            {
                capacitacionesDelPeriodo = capacitacionesPorTrimestre
                    .SelectMany(x => x.Id)
                    .ToList();
            }
            else
            {
                capacitacionesDelPeriodo = capacitacionesPorTrimestre
                    .Where(x => x.Trimestre == Periodo)
                    .SelectMany(x => x.Id)
                    .ToList();
            }

            List<ModeloReporteAccion> modelo = new List<ModeloReporteAccion>();

            if (Personal == 1)
            {
                var detalles = capacitacionesPrincipal
                    .Where(g => capacitacionesDelPeriodo.Contains(g.PrimerRegistro.Id) &&
                                (g.PrimerRegistro.DirigidoASelect.Contains("1")
                                || g.PrimerRegistro.DirigidoASelect.Contains("2")
                                || g.PrimerRegistro.DirigidoASelect.Contains("3")
                                || g.PrimerRegistro.DirigidoASelect.Contains("4")))
                    .Select(g => new { g.PrimerRegistro.Nombre, g.PrimerRegistro.AreaObjetivo, g.PrimerRegistro.Cobertura, g.PrimerRegistro.Modalidad, g.TotalHoras, g.PrimerRegistro.Id, g.PrimerRegistro.IdTipoPEC });
                    
                modelo = detalles.Select(i => new ModeloReporteAccion
                {
                    Accion = i.Nombre,
                    Area = i.AreaObjetivo,
                    Ejecutada = i.Cobertura != null && i.Cobertura != "0,00",
                    EduFoscal = i.Modalidad == "4",
                    TotalHoras = i.TotalHoras,
                    Id = i.Id,
                    TipoPEC = tiposPEC.TryGetValue(i.IdTipoPEC, out var tipo) ? tipo?.Valor : null
            })
                .ToList();
            }

            if (Personal == 5)
            {
                var detalles = capacitacionesPrincipal
                    .Where(g => capacitacionesDelPeriodo.Contains(g.PrimerRegistro.Id)
                                && g.PrimerRegistro.DirigidoASelect.Contains(Personal.ToString()))
                    .Select(g => new { g.PrimerRegistro.Nombre, g.PrimerRegistro.AreaObjetivo, g.PrimerRegistro.Cobertura, g.PrimerRegistro.Modalidad, g.TotalHoras, g.PrimerRegistro.Id, g.PrimerRegistro.IdTipoPEC });

                modelo = detalles.Select(i => new ModeloReporteAccion
                {
                    Accion = i.Nombre,
                    Area = i.AreaObjetivo,
                    Ejecutada = i.Cobertura != null && i.Cobertura != "0,00",
                    EduFoscal = i.Modalidad == "4",
                    TotalHoras = i.TotalHoras,
                    Id = i.Id,
                    TipoPEC = tiposPEC.TryGetValue(i.IdTipoPEC, out var tipo) ? tipo?.Valor : null
                })
                .ToList();
            }

            if (Personal == 6)
            {
                var detalles = capacitacionesPrincipal
                    .Where(g => capacitacionesDelPeriodo.Contains(g.PrimerRegistro.Id)
                                && g.PrimerRegistro.DirigidoASelect.Contains(Personal.ToString()))
                    .Select(g => new { g.PrimerRegistro.Nombre, g.PrimerRegistro.AreaObjetivo, g.PrimerRegistro.Cobertura, g.PrimerRegistro.Modalidad, g.TotalHoras, g.PrimerRegistro.Id, g.PrimerRegistro.IdTipoPEC });

                modelo = detalles.Select(i => new ModeloReporteAccion
                {
                    Accion = i.Nombre,
                    Area = i.AreaObjetivo,
                    Ejecutada = i.Cobertura != null && i.Cobertura != "0,00",
                    EduFoscal = i.Modalidad == "4",
                    TotalHoras = i.TotalHoras,
                    Id = i.Id,
                    TipoPEC = tiposPEC.TryGetValue(i.IdTipoPEC, out var tipo) ? tipo?.Valor : null
                })
                .ToList();
            }

            ViewBag.CantidadSI = modelo
                .Where(x => x.Ejecutada == true && x.EduFoscal == false)
                .Select(x => new { x.Ejecutada })
                .Count();

            ViewBag.CantidadNO = modelo
                .Where(x => x.Ejecutada == false && x.EduFoscal == false)
                .Select(x => new { x.Ejecutada })
                .Count();

            ViewBag.CantidadSIEduF = modelo
                .Where(x => x.Ejecutada == true && x.EduFoscal == true)
                .Select(x => new { x.Ejecutada })
                .Count();

            ViewBag.CantidadNOEduF = modelo
                .Where(x => x.Ejecutada == false && x.EduFoscal == true)
                .Select(x => new { x.Ejecutada })
                .Count();

            ViewBag.CantidadTotalHorasFormacion = modelo
                .Where(x => x.Ejecutada == true)
                .Sum(x => x.TotalHoras);

            return View(modelo);
        }

        public class ModeloReporteDocentes
        {
            public string Nombres { get; set; }
            //public string Area { get; set; }
            public int TotalAcciones { get; set; }
            public string Tipo { get; set; }
        }

        public ActionResult ReporteDocentes(int? Periodo, string Empresa)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            if (!funciones.Contains("InformeGeneralCap"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            if (Periodo == null || Empresa == null)
            {
                return View(new List<ModeloReporteDocentes>());
            }

            DateTime Fecha1 = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime Fecha2 = new DateTime(DateTime.Now.Year, 12, 31);

            var capacitacionesRaw = db.Capacitacion
                .Where(x => DbFunctions.TruncateTime(x.Mes) >= Fecha1 && DbFunctions.TruncateTime(x.Mes) <= Fecha2 && x.Empresa == Empresa)
                .ToList();

            var capacitacionesPrincipal = capacitacionesRaw
                .GroupBy(x => x.IdentificadorRelacion)
                .Select(g => g.FirstOrDefault())
                .ToList();

            List<int> capacitacionesDelPeriodo;

            if (Periodo == 5)
            {
                capacitacionesDelPeriodo = capacitacionesPrincipal
                    .Select(x => x.Id)
                    .Distinct()
                    .ToList();
            }
            else
            {
                capacitacionesDelPeriodo = capacitacionesPrincipal
                    .Where(x => (x.FechaCapacitacion.Month - 1) / 3 + 1 == Periodo)
                    .Select(x => x.Id)
                    .Distinct()
                    .ToList();
            }

            var capacitacionesPorEmpleado = capacitacionesPrincipal
                .Where(x => capacitacionesDelPeriodo.Contains(x.Id) && !string.IsNullOrEmpty(x.Docente))
                .SelectMany(x => x.Docente.Split(',')
                    .Select(docente => new { Docente = docente.Trim(), x.Id })
                )
                .GroupBy(x => x.Docente)
                .Select(g => new
                {
                    Docente = g.Key,
                    TotalAcciones = g.Count()
                })
                .ToList();

            var modelo = capacitacionesPorEmpleado
                .Select(x => new ModeloReporteDocentes
                {
                    Nombres = x.Docente,
                    Tipo = db.Empleados.Any(e => e.Nombres == x.Docente) ? "INTERNO" : "EXTERNO",
                    TotalAcciones = x.TotalAcciones
                })
                .ToList();

            return View(modelo);
        }

        public ActionResult ReporteAsistentes(int id)
        {
            if (id == 0)
            {
                return HttpNotFound();
            }

            var relacion = db.Capacitacion
                .FirstOrDefault(x => x.Id == id)?.IdentificadorRelacion;

            var sesiones = db.Capacitacion
                .Where(x => x.IdentificadorRelacion == relacion)
                .Select(x => x.Id)
                .ToList();

            var asistentesSesiones = db.DetalleCapacitacion
                .Include(x => x.Capacitacion)
                .Where(x => sesiones.Contains(x.CapacitacionId))
                .ToList();

            foreach (DetalleCapacitacion Item in asistentesSesiones)
            {
                if (Item.EsTercero == "NO")
                {
                    var Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    var EmpEmpresa = Empleado.Empresa;
                    Item.Sociedad2 = db.Sociedad.FirstOrDefault(x => x.Codigo == EmpEmpresa)?.Descripcion;
                    Item.Empleado2 = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                }
                else
                {
                    var TerceroId = Convert.ToInt32(Item.TerceroId);
                    var Tercero = db.Tercero.FirstOrDefault(x => x.Id == TerceroId);
                    Item.Tercero2 = db.Tercero.FirstOrDefault(e => e.Id == TerceroId);

                    if (int.TryParse(Tercero.SociedadCOD, out int TerEmpresa))
                    {
                        Item.NombreEmpresaTercera = db.Empresas.FirstOrDefault(x => x.Id == TerEmpresa)?.Nombre;
                    }
                    else
                    {
                        Item.NombreEmpresaTercera = Tercero.SociedadCOD;
                    }

                    Item.Universidad = db.Tercero.FirstOrDefault(x => x.Id == TerceroId)?.Universidad;
                }

            }

            ViewBag.CantidadSI = asistentesSesiones
                .Where(x => x.FechaFirma != null && x.Estado == "Activo")
                .Count();

            ViewBag.CantidadNO = asistentesSesiones
                .Where(x => x.FechaFirma == null && x.Estado == "Activo")
                .Count();


            return PartialView(asistentesSesiones);
        }

        public ActionResult Expositores(string Id)
        {
            using (var db = new AutogestionContext())
            {
                ArrayList lst = new ArrayList();
                var terceros = db.Tercero.Select(e => new { e.Id, e.Nombres, e.Superior, e.Documento, e.Area, e.Cargo, nomcodigo = e.Id + "-" + e.Documento + "-" + e.Area + "-" + e.Cargo + "-" + e.Superior + "-" + "SI" }).ToArray();
                foreach (var item in terceros)
                {
                    lst.Add(item);

                }
                var emp3 = (from emp in db.Empleados
                            join e in db.Empleados on emp.Jefe equals e.NroEmpleado into personal
                            from ps in personal.DefaultIfEmpty()
                            where emp.Activo != "NO" || emp.FechaFin >= DateTime.Now
                            select new { emp.Id, emp.Documento, emp.Cargo, Jefe = emp.Jefe, emp.Nombres, emp.Activo, nomcodigo = emp.Id + "-" + emp.Documento + "-" + emp.Area + "-" + emp.Cargo + "-" + ps.Nombres + "-" + "NO", emp.FechaFin }
                  ).ToArray();

                foreach (var item in emp3)
                {
                    lst.Add(item);

                }
                var idcap = Convert.ToInt32(Id);
                ViewBag.Capacitacion = db.Capacitacion.FirstOrDefault(x => x.Id == idcap);

                ViewBag.Empleado = lst;
                return PartialView();
            }
        }

        public JsonResult Expositores2() 
        {
            var respuesta = "";
            try
            {
                var CantidadExpo = HttpContext.Request.Params["CantidadDetalles"];
                var Empleados = HttpContext.Request.Params["Empleados"];
                var NmrCapacitacion = HttpContext.Request.Params["NmrCapacitacion"];
                int cantidadExpo = Convert.ToInt16(CantidadExpo);
                var idCap = Convert.ToInt32(NmrCapacitacion);
                string[] Emps = Empleados.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                var capacitacion = db.Capacitacion.FirstOrDefault(x => x.Id == idCap);
                // TODO: Add insert logic here
                var model = new List<Expositor>();
                for (var i = 0; i < cantidadExpo; i++)
                {
                    var Expo = new Expositor();
                    string[] Emps2 = Emps[i].Split(new[] { '/' }).ToArray();
                                  
                    if (Emps2[5] == "NO")
                    {
                        Expo.EmpleadoId = Emps2[1];
                        Expo.TipoExpositor = "Empleado";
                    }
                    else
                    {
                        Expo.TerceroId = Emps2[1];
                        Expo.TipoExpositor = "Tercero";

                    }

                    
                    Expo.CapacitacionId = idCap;
                  
                    model.Add(Expo);
                }
                _repo.CrearExpositor(model);
                respuesta = "Los datos fueron guardados correctamente.";




            }
            catch (SystemException ex)
            {
                respuesta = String.Format("Error al guardar. {0}", ex.Message);
                return Json(new
                {
                    respuesta,
                    isRedirect = false
                });
            }

            return Json(new { respuesta, isRedirect = true });
        }

        public ActionResult Capacitacioninfo(string Id) 
        {
            try 
            {
                using (var db = new AutogestionContext())
                { 
                    var id =Convert.ToInt32(Id);
                    var capacitacion = db.Capacitacion.FirstOrDefault(z => z.Id == id);
                    var entrega = _repo2.ObtenerTodos(id);
                    capacitacion.Participantes = entrega;
                    var Expositores = db.Expositor.Where(x => x.CapacitacionId == capacitacion.Id).ToList();
                    foreach(Expositor Item in Expositores)
                    {
                        if (Item.TipoExpositor == "Empleado")
                        {
                            var idEmp= Convert.ToInt32(Item.EmpleadoId);
                            Item.Empleado = db.Empleados.FirstOrDefault(x=>x.Id==idEmp);
                        }
                        if (Item.TipoExpositor == "Tercero")
                        {
                            var idTer = Convert.ToInt32(Item.TerceroId);
                            Item.Tercero = db.Tercero.FirstOrDefault(x => x.Id == idTer);
                        }
                    }
                    capacitacion.Expositores = Expositores;


                    var sede=Convert.ToInt32(capacitacion.IdSede);
                    var metodologia= capacitacion.Metodologia;
                    var metodologia2 = capacitacion.Metodologia2;
                    capacitacion.Sede = db.Sede.FirstOrDefault(b => b.Id == sede);
                    capacitacion.Empleado = db.Empleados.FirstOrDefault(x => x.Id == capacitacion.EmpleadoRegistraId);
                    capacitacion.Empresa2 = db.Sociedad.FirstOrDefault(x => x.Codigo == capacitacion.Empresa);
                    capacitacion.MetodologiaC = db.Metodologia.FirstOrDefault(z => z.Id == metodologia);
                    capacitacion.MetodologiaCierre = db.Metodologia.FirstOrDefault(z => z.Id == metodologia2);
                    capacitacion.CulturaO = db.CulturaOrganizacional.FirstOrDefault(z => z.Id == capacitacion.CulturaOrganizacional);
                    capacitacion.CompetenciaL = db.CompetenciaLaboral.FirstOrDefault(z => z.Id == capacitacion.CompetenciaLaboral);
                    capacitacion.DirigidoA = db.DirigidoA.FirstOrDefault(x => x.Id == capacitacion.IdDirigidoA)?.Valor;
                    capacitacion.TipoPEC = db.TipoPEC.FirstOrDefault(x => x.Id == capacitacion.IdTipoPEC)?.Valor;
                    capacitacion.ProgramaInstitucional = db.ProgramaInstitucional.FirstOrDefault(x => x.Id == capacitacion.IdProgramaInstitucional)?.Valor;
                    capacitacion.RequerimientoInstitucional = db.RequerimientoInstitucional.FirstOrDefault(x => x.Id == capacitacion.IdRequerimientoInstitucional)?.Valor;

                    ViewBag.Capacitacion=capacitacion;
                }
                return PartialView();
            }
            catch (SystemException ex)
            {
                return PartialView();
            }

            
        }

        public ActionResult CopiarCap(int Id)
        {
            using (var db = new AutogestionContext())
            {
                Capacitacion model = new Capacitacion();
                model = _repo.CrearCopia(Id);
            }
            return RedirectToAction("ListaPorCompletar");
        }

        public ActionResult MostrarListaEncuestas(int id, string page)
        {
            using (var db = new AutogestionContext())
            {

                
                List<Configuraciones> config = new List<Configuraciones>();
                config = db.Configuraciones.Where(x => x.Parametro == "LINKCAPACITA").ToList();
                ViewBag.lista = config;
                ViewBag.Id = id;
                ViewBag.Page = page;


                if (config == null)
                {
                    return HttpNotFound();
                }



                return PartialView();
            }
        }


        [HttpPost]
        public String EnvioEncuestaCap(int id, string Encuesta, int Page)
        {
            Empleado empleado = new Empleado();
            DetalleCapacitacion det = new DetalleCapacitacion();

            try
            {
                using (var db = new AutogestionContext())
                {
                    Capacitacion capacitacion = db.Capacitacion.Find(id);
                    List<DetalleCapacitacion> detalle = db.DetalleCapacitacion.Where(e => e.CapacitacionId == capacitacion.Id).ToList();
                    
                    foreach (DetalleCapacitacion Item in detalle)
                    {
                        empleado = db.Empleados.Find(Item.EmpleadoId);
                        det = Item;

                        if (empleado.CorreoPersonal == null || empleado.CorreoPersonal == "")
                            return "SINCORREO";
                        else
                        {
                            if (enviar_correo_encuesta(empleado.Documento, empleado.Nombres, id, empleado.CorreoPersonal, Encuesta, capacitacion.Nombre) == false)
                            {
                                return "ERROR";
                            }
                            else
                            {
                                det.EnvioEncuesta = "SI";
                                _repo.ActualizarEnvioEncuesta(det);
                            }
                        }

                    }

                    
                }
                return "OK";
            }
            catch
            {
                return "ERROR";
            }

            


        }

        public bool enviar_correo_encuesta(string cedula, string nombres, int idcapacita, string email, string Encuesta, string capacitacion)
        {
            string textocorreo = "";
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
            using (var db = new AutogestionContext())
            {
                var configuracion = db.Configuraciones.First(x => x.Parametro == "TXTEMAILENVIOCAPACITA");



                var link = Encuesta;

                link = link.Replace("nombres", nombres);
                link = link.Replace("cedula", cedula);
                link = link.Replace("nrocapacitacion", idcapacita.ToString());

                textocorreo = configuracion.Valor;
                textocorreo = textocorreo.Replace("$CAPACITACION", capacitacion);
                textocorreo = textocorreo.Replace("$URL", link);
            }
            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);


                correo.To.Add(email);




                correo.Subject = "Encuesta de Capacitacion";
                correo.Body = "Hola " + nombres + "</BR>" + textocorreo;
                correo.Priority = System.Net.Mail.MailPriority.Normal;
                correo.IsBodyHtml = true;



                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                smtp.EnableSsl = true;
                smtp.Send(correo);
                confirmacion = true;
                return confirmacion;
            }
            catch (Exception ex)
            {

                confirmacion = false;
                return confirmacion;
            }
        }

        public ActionResult LoginExpositor() {

            return View();
        }

        public ActionResult VistaExpositor()
        {
            List<Capacitacion> model = new List<Capacitacion>();
            List<Expositor> cap = new List<Expositor>();

            AutogestionContext db = new AutogestionContext();
            {

                string emp = String.Format("{0}", Session["EmpleadoT"]);
                cap = db.Expositor.Where(e => e.TerceroId == emp).ToList();

                model = _repo.ObtenerTodos2(cap);

                //                    63529367
                //RINCON SOLANO ALVARO ANDRES 
                //alvaro_rincon@hotmail.com

            }

            return View(model);
        }

        public ActionResult LoginVistaExpositor(string alias, string correo, string Tercero)
        {

            List<Capacitacion> model = new List<Capacitacion>();
            List<Expositor> cap = new List<Expositor>();
            

            if (!string.IsNullOrEmpty(alias))
            {
                AutogestionContext db = new AutogestionContext();

                if (Tercero == "SI")
                {
                     var user = db.Tercero.FirstOrDefault(e => e.Documento == alias.Trim() && e.CorreoPersonal == correo.Trim());
                     if (user != null)
                     {
                         Session.Add("EmpleadoT", user.Id);
                         return RedirectToAction("VistaExpositor", "Capacitacion");


                     }
                     else
                     {
                         ViewBag.message = "Nro de Documento o Email incorrectos.";
                         Session.Add("message", "Nro de Documento o Email incorrectos.");
                         return RedirectToAction("LoginExpositor");

                     }
                }
                else { 
                    var user = db.Empleados.FirstOrDefault(e => e.Documento == alias.Trim() && e.CorreoPersonal == correo.Trim() && e.Activo == "SI");
                    if (user != null)
                    {
                        Session.Add("EmpleadoT", user.Id);
                        return RedirectToAction("VistaExpositor", "Capacitacion");


                    }
                    else
                    {
                        ViewBag.message = "Nro de Documento o Email incorrectos.";
                        Session.Add("message", "Nro de Documento o Email incorrectos.");
                        return RedirectToAction("LoginExpositor");

                    }
                }
                
            }
            else
            {
                return RedirectToAction("LoginExpositor", new { message = "Sin datos" });
            }
        }

        public ActionResult Preinscripcion(int? id)
        {
            ViewBag.Sociedad = db.Sociedad.Select(y => new { y.Codigo, y.Descripcion }).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult Preinscripcion(int id, string Nombre, string Empresa, string Cargo, string Correo, string Celular, string Tipo, string Documento)
        {

            return View();
        }

    }
}
