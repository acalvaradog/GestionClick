using Adm_AutoGestion.Models;
using Adm_AutoGestion.Models.EvaDesempeno;
using Adm_AutoGestion.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace Adm_AutoGestion.Controllers
{
    public class Datos
    {
        public int Id { get; set; }
        public string Nombres { get; set; }
    }
    public class RequerimientosPersonalController : Controller
    {
        private AutogestionContext db = new AutogestionContext();
        private RequerimientosPersonalRepository _repo;
        private ServiciosRepository _servicios;


        public RequerimientosPersonalController()
        {
            _repo = new RequerimientosPersonalRepository();
            _servicios = new ServiciosRepository();
        }

        // GET: /RequerimientosPersonal/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /RequerimientosPersonal/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /RequerimientosPersonal/Create
        public ActionResult Create()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("CrearRdP"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var IdE = Session["Empleado"];
            var IdUsu = Convert.ToInt32(IdE);
            var EmpId = db.Empleados.Where(a => a.Id == IdUsu).FirstOrDefault();
            var emp3 = (from emp in db.Empleados
                        join e in db.Empleados on emp.Jefe equals e.NroEmpleado into personal
                        from ps in personal.DefaultIfEmpty()
                        where emp.Activo != "NO" && emp.Jefe == EmpId.NroEmpleado || emp.FechaFin >= DateTime.Now && emp.Jefe == EmpId.NroEmpleado
                        select new { emp.Id, Jefe = emp.Jefe, emp.Nombres, emp.Activo }).ToArray();
           
            ViewBag.MtvEgreso = db.MotivoEgreso.Where(x => x.Estado == "Activo").ToList();
            ViewBag.MtvEgresoNovedad = db.MotivoEgreso.Where(x => x.Estado == "Activo" && x.Nombre == "INCAPACIDAD" || x.Estado == "Activo" && x.Nombre == "VACACIONES" || x.Estado == "Activo" && x.Nombre == "LICENCIA DE MATERNIDAD" || x.Estado == "Activo" && x.Nombre == "OTRO REEMPLAZO" || x.Estado == "Activo" && x.Nombre == "VACANTE").ToList();
            ViewBag.Jornada = db.Jornada.Where(x => x.Estado == "Activo").ToList();
            ViewBag.Horario = db.Horario.Where(x => x.Estado == "Activo").ToList();
            ViewBag.MtvSolicitud = db.MtvSolicitud.Where(x => x.Estado == "Activo").ToList();
            ViewBag.Empleado = emp3;
            ViewBag.Sociedad = db.Sociedad.ToList();
            //ViewBag.Direccion = db.Direcciones.ToList(); 

            List<SelectListItem> lst = new List<SelectListItem>();
            List<SelectListItem> lst2 = new List<SelectListItem>();
            List<SelectListItem> lst3 = new List<SelectListItem>();


            var lista = db.PersonalActivo.Select(x => new { x.Area }).Where(x=> x.Area == EmpId.Area).GroupBy(b => b.Area).ToList();
            foreach (var x in lista)
            {
                lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
            }


            ViewBag.Area = lst;


            var lista2 = db.PersonalActivo.Select(x => new { x.Cargo }).GroupBy(b => b.Cargo).ToList();
            foreach (var x in lista2)
            {
                lst2.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
            }

            ViewBag.Cargo = lst2;

            var lista3 = db.Direcciones.Select(x => new { x.Nombre }).ToList();
            foreach (var x in lista3)
            {
                lst3.Add(new SelectListItem() { Text = x.Nombre.ToString(), Value = x.Nombre.ToString() });
            }


            ViewBag.Direccion = lst3;


            return View();
        }
        public ActionResult DatosUsuEgrePartiV(int? Id)
        {
            string respuesta = "true";
            try
            {
                var Emp = db.Empleados.Where(a => a.Id == Id).FirstOrDefault();
                var FechaIni = Convert.ToDateTime(Emp.FechaIngreso);
                var d = FechaIni.Day;
                var m = FechaIni.Month;
                var t = FechaIni.Year;
                ViewBag.DatosUsuSalFec = d + "/" + m + "/" + t;
                ViewBag.DatosUsuSalCar = Emp.Cargo.ToLower();
                ViewBag.DatosUsuSalNom = Emp.Nombres.ToLower();
                return PartialView(Emp);
            }
            catch (SystemException ex)
            {
                respuesta = String.Format("Error. {0}", ex.Message);
                return PartialView(new{respuesta});
            }          
        }       
        [HttpPost]
        public JsonResult CreateRequeri(List<DetalleRequerimiento> model)
        {

            //List<DetalleRequerimiento> detalle = new List<Models.DetalleRequerimiento>();
            RequerimientosDelPersonal requerimiento = new RequerimientosDelPersonal();
            string respuesta = "true";
            var url = Url.Action("");

            var Empresa = HttpContext.Request.Params["Empresa"];
            var MtvSolicitudID = HttpContext.Request.Params["MotivoSolt"];
            var Cargo = HttpContext.Request.Params["Cargo"];
            var TipoConcurso = HttpContext.Request.Params["TipoConcurso"];
            var Area = HttpContext.Request.Params["Area"];
            var Direccion = HttpContext.Request.Params["Direccion"];
            var NumeroPresonas = HttpContext.Request.Params["NumeroPresonas"];
            //var Sexo = HttpContext.Request.Params["Sexo"];
            var Sexo = "SinPreferencia";
            var JornadaRequeridaId = HttpContext.Request.Params["JornadaRequeridaId"];
            var HorarioTrabajoId = HttpContext.Request.Params["HorarioTrabajoId"];
            var FechaSugeridaIngreso = HttpContext.Request.Params["FechaSugeridaIngreso"];
            var Contratacion = HttpContext.Request.Params["Contratacion"];
            var TiempoContratacion = HttpContext.Request.Params["TiempoContratacion"];
            var JustificacionCargo = HttpContext.Request.Params["JustificacionCargo"];
            var RequisitosAdicionales = HttpContext.Request.Params["RequisitosAdicionales"];
            var Empleadolog = Session["Empleado"];
            var Empleados = HttpContext.Request.Params["Empleados"];
            var Cual = HttpContext.Request.Params["Cual"];
            //Empleados = Empleados.Substring(0, Empleados.Length - 1);
            string[] filas;

            if (Empleados != null)
            {
                filas = Empleados.Split(',');
            }
            else { filas = null; }


            requerimiento.EmpresaId = Convert.ToInt32(Empresa);
            requerimiento.MtvSolicitudID = Convert.ToInt32(MtvSolicitudID);
            requerimiento.Cargo = Cargo;
            requerimiento.TipoConcurso = TipoConcurso;
            requerimiento.Area = Area;
            requerimiento.Direccion = Direccion;
            requerimiento.NumeroPresonas = Convert.ToInt32(NumeroPresonas);
            requerimiento.Sexo = Sexo;
            requerimiento.EstadoSeleccion = 1;
            requerimiento.JornadaRequeridaId = Convert.ToInt32(JornadaRequeridaId);
            requerimiento.HorarioTrabajoId = Convert.ToInt32(HorarioTrabajoId);
            requerimiento.FechaSugeridaIngreso = FechaSugeridaIngreso;
            requerimiento.Contratacion = Contratacion;
            requerimiento.TiempoContratacion = TiempoContratacion;
            requerimiento.JustificacionCargo = JustificacionCargo;
            requerimiento.RequisitosAdicionales = RequisitosAdicionales;
            requerimiento.Cual = Cual;

            try
            {
                if (requerimiento.MtvSolicitudID == 2 || requerimiento.MtvSolicitudID == 3 || requerimiento.MtvSolicitudID == 7 || requerimiento.MtvSolicitudID == 8)
                {
                    if (string.IsNullOrEmpty(JustificacionCargo))
                    {
                        respuesta = "Debe agregar una Justificacion del Cargo.";
                        return Json(new { respuesta });
                    }

                }

                if (MtvSolicitudID == "2")
                {


                    var deserializer = new JavaScriptSerializer();
                    var httpPostedFile = HttpContext.Request.Files["Adjunto"];


                    var extension = httpPostedFile.FileName.Split('.');
                    if (extension[1] != "jpg" && extension[1] != "jpeg" && extension[1] != "png" && extension[1] != "doc" && extension[1] != "docx" && extension[1] != "pdf" && extension[1] != "xlsx")
                    {
                        respuesta = "El tipo de archivo ." + extension[1] + " no es permitido.";
                        return Json(new { respuesta });
                    }
                    var size = httpPostedFile.ContentLength / 1024;
                    if (size > 800)
                    {
                        respuesta = "El archivo supera el tamaño permitido de carga.";
                        return Json(new { respuesta });
                    }
                    DateTime date1 = DateTime.Now;
                    var nombrearchivo = date1.ToString("yyyyMMddHHmmssffff").ToString() + "-" + httpPostedFile.FileName;                    
                    var fileSavePath = Path.Combine(HttpContext.Server.MapPath("~/AnexosRequerimientosPersonal"), nombrearchivo);
                    requerimiento.Archivo = nombrearchivo;
                    httpPostedFile.SaveAs(fileSavePath);
                    var Id = Session["Empleado"];
                    var id = Convert.ToInt32(Id);
                    var Fin = _repo.Crear(requerimiento, id, filas, NumeroPresonas);
                    var nombreemp = db.Empleados.Find(id);
                    var Correo = "";
                    //if (requerimiento.MtvSolicitudID == 2 || requerimiento.MtvSolicitudID == 7 || requerimiento.MtvSolicitudID == 3 || requerimiento.MtvSolicitudID == 8)
                    //{
                    //    Correo = db.Configuraciones.First(s => s.Parametro == "CORREOGERENCIA").Valor.ToString();
                    //    EnviarCorreoNotifiRequerimientoNuevoCargo(requerimiento.Id, nombreemp.Nombres, Correo, requerimiento.Cargo, requerimiento.Area, requerimiento.EmpresaId, requerimiento.NumeroPresonas);
                    //}
                    //else
                    //{

                    //}

                    Correo = db.Empleados.First(e => e.NroEmpleado == nombreemp.Jefe).Correo.ToString();//Correo Director  
                    EnviarCorreoNotifiRequerimiento(nombreemp.Nombres, Correo, requerimiento.Cargo, requerimiento.Area, requerimiento.EmpresaId, requerimiento.NumeroPresonas);
                    var EmailEmp = db.Empleados.Find(id);
                    respuesta = Fin;

                }
                else
                {
                    var Id = Session["Empleado"];
                    var id = Convert.ToInt32(Id);
                    var Fin = _repo.Crear(requerimiento, id, filas, NumeroPresonas);

                    var nombreemp = db.Empleados.Find(id);
                    var Correo = "";
                    //if (requerimiento.MtvSolicitudID == 2 || requerimiento.MtvSolicitudID == 7 || requerimiento.MtvSolicitudID == 3 || requerimiento.MtvSolicitudID == 8)
                    //{
                    //    Correo = db.Configuraciones.First(s => s.Parametro == "CORREOGERENCIA").Valor.ToString();
                    //}
                    //else
                    //{
                    //    Correo = db.Empleados.First(e => e.NroEmpleado == nombreemp.Jefe).Correo.ToString();
                    //}
                    Correo = db.Empleados.First(e => e.NroEmpleado == nombreemp.Jefe).Correo.ToString();
                    EnviarCorreoNotifiRequerimiento(nombreemp.Nombres, Correo, requerimiento.Cargo, requerimiento.Area, requerimiento.EmpresaId, requerimiento.NumeroPresonas);

                    var EmailEmp = db.Empleados.Find(id);
                    respuesta = Fin;
                }
                List<string> funciones = Acceso.Validar(Session["Empleado"]);
                //var url = Url.Action("");
                if (!Acceso.EsAnonimo && funciones.Contains("CrearRdP"))
                {
                    url = Url.Action("Create");
                }
                return Json(new
                {
                    RedirectUrl = url,
                    isRedirect = true,
                    respuesta
                });
            }
            catch (SystemException ex)
            {
                respuesta = String.Format("Error al guardar. {0}", ex.Message);
                return Json(new
                {
                    respuesta
                });
            }
        }

        public FileResult Download1(string archivo)
        {

            var fileBytes = Server.MapPath(@"..\AnexosRequerimientosPersonal\" + archivo);
            string fileName = archivo;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        //
        // GET: /RequerimientosPersonal/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /RequerimientosPersonal/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /RequerimientosPersonal/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /RequerimientosPersonal/Delete/5
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

        public ActionResult AprobarDirectorArea()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            List<RequerimientosDelPersonal> Datos = new List<RequerimientosDelPersonal>();
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("RdPAprobacionDirectorArea"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var id = Convert.ToInt32(Session["Empleado"]);
            var emp = db.Empleados.Where(a => a.Id == id).FirstOrDefault();
            var Areas = db.Empleados.Where(x => x.Area != null && x.Area != "" && x.Jefe == emp.NroEmpleado).Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
            var model = _repo.ObtenerTodos();
            if (funciones.Contains("RdPAprobacionDirectorArea"))
            {
                foreach (var i in Areas)
                {
                    foreach (var item in model)
                    {
                        if (item.Area == i.Key)
                        {
                            Datos.Add(item);
                        }
                    }
                }
            }
            

            return View(Datos);
        }

        public ActionResult AprobadoGerencia()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("RdPGerencia"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var model = _repo.ObtenerTodos3(1);
            return View(model);


        }

        public ActionResult GestionHumana()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("RdPGestionHumana"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var model = _repo.ObtenerTodos3(2);

            return View(model);
        }

        public ActionResult DetalleRequerimiento(int id)
        {
            var model = _repo.ObtenerTodos2(id);
            //Session["IdEmpleado"] = id;

            ViewBag.Requerimiento = model;

            return PartialView(model);
        }

        public ActionResult DetalleRequerimientoinfo(int id)
        {
            var model = _repo.ObtenerTodos2(id);
            ViewBag.Requerimiento = model;
            var Idusu =Convert.ToInt32(Session["Empleado"]);
            if (model.EmpleadoRegistraId == Idusu)
            {
                ViewBag.creador = "Si";
            }
            else
            {
                ViewBag.creador = "No";
            }
            
            ViewBag.estado = db.EstadoRdP.Where(a => a.Id == model.EstadoID).FirstOrDefault().Nombre;            
            
            return PartialView(model);
        }

        public ActionResult DetalleRequerimientoFirma(int id)
        {
            var model = _repo.ObtenerTodos2(id);
            //Session["IdEmpleado"] = id;

            return PartialView(model);
        }
       

        public ActionResult DetalleGerencia(int id)
        {
            var model = _repo.ObtenerTodos2(id);
            ViewBag.Requerimiento = model;
            return PartialView(model);
        }

        public ActionResult DetalleGestionHumana(int id)
        {
            var model = _repo.ObtenerTodos2(id);
            List<Datos> Datos = new List<Datos>();
            var EncarNove = db.GrupoEmpleados.Where(a => a.Descripcion == "Encargado Seleccion Novedad").FirstOrDefault();                   
            if (model.MtvSolicitud.Nombre == "NOVEDAD")
            {
                var buscarSeleccionGH = db.Rol.Where(x => x.GrupoId == EncarNove.Id).Distinct().ToList();
                foreach (var i in buscarSeleccionGH)
                {
                    var Empleados = (from emp in db.Empleados
                                     join e in db.Rol on emp.Id equals e.EmpleadoId
                                     where emp.Activo != "NO" && emp.Id == i.EmpleadoId && e.GrupoId == EncarNove.Id
                                     select new { emp.Id, emp.Nombres }).FirstOrDefault();
                    if (Empleados != null)
                    {
                        Datos.Add(new Datos { Id = i.EmpleadoId, Nombres = Empleados.Nombres });
                    }
                }
            }
            else
            {
                var buscarSeleccionGH = db.Rol.Where(x => x.GrupoId == 23 && x.GrupoId != 29).Distinct().ToList();
                foreach (var i in buscarSeleccionGH)
                {
                    var Empleados = (from emp in db.Empleados
                                     join e in db.Rol on emp.Id equals e.EmpleadoId
                                     where emp.Activo != "NO" && emp.Id == i.EmpleadoId
                                     select new { emp.Id, emp.Nombres }).Distinct().FirstOrDefault();
                    if (Empleados != null)
                    {
                        Datos.Add(new Datos { Id = i.EmpleadoId, Nombres = Empleados.Nombres });
                    }
                }
            }

            ViewBag.MtvSolicitud = Convert.ToString(model.MtvSolicitud.Nombre);
            ViewBag.Empleados = Datos;            
            ViewBag.Requerimiento = model;
            return PartialView(model);
        }

        public ActionResult DetalleContratacion(int id)
        {
            var model = _repo.ObtenerTodos2(id);
            ViewBag.Requerimiento = model;
            ViewBag.EstadoSelec = db.EstadoSeleccionRdP.ToList();
            return PartialView(model);
        }

        public ActionResult Registros(int Id)
        {
            var model = _repo.ObtenerRegistro(Id);
            if (model.Count > 0) { ViewBag.NmrRegistros = true; }
            else { ViewBag.NmrRegistros = false; }
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult CambioEstadoRdP()
        {
            var respuesta = "";
            try
            {
                var IdSolicitud = HttpContext.Request.Params["IdSolicitud"];
                var EstadoSelec = HttpContext.Request.Params["EstadoSelec"];
                var Accion = HttpContext.Request.Params["Accion"];
                var Observacion = HttpContext.Request.Params["Observación"];
                var EncargadoContratacion = HttpContext.Request.Params["EmpContratacion"];
                var TipoConcurso = HttpContext.Request.Params["TipoConcurso"];
                var MtvSolicitud = HttpContext.Request.Params["MtvSolicitudID2"];
                var EmpContratacion = 0;
                if (Accion == "GestiónHumana")
                {
                    if (EncargadoContratacion == "")
                    {
                        return Json(new { respuesta = "La persona encargada de la Contratación debe ser Seleccionada", isRedirect = false });
                    }
                    EmpContratacion = Convert.ToInt32(EncargadoContratacion);
                }

                var Id = Convert.ToInt32(IdSolicitud);


                var userId = Session["Empleado"];
                var userlog = Convert.ToInt32(userId);
                var Solicitud = db.RequerimientosDelPersonal.FirstOrDefault(x => x.Id == Id);

                Aprobacioneslog Model = new Aprobacioneslog();
                Model.IdRequerimiento = Id;
                Model.Fecha = DateTime.Now;
                Model.Observación = Observacion;
                Model.Usuario = userlog;
                Model.EstadoAnterior = Solicitud.EstadoID;

                RequerimientosDelPersonal model = new RequerimientosDelPersonal();
                model.TipoConcurso = TipoConcurso;
                if (MtvSolicitud == "NOVEDAD")
                {
                    model.TipoConcurso = "INTERNO";
                }
                var val = _repo.Aprobar(Id, Accion, TipoConcurso, Model, EmpContratacion);
                if (Accion == "DirectorArea")
                {
                    var Idemp = Session["Empleado"];
                    var id = Convert.ToInt32(Id);
                    var nombreemp = db.Empleados.Find(id);
                    var Correo = "";
                    if (Solicitud.MtvSolicitudID == 2 || Solicitud.MtvSolicitudID == 7 || Solicitud.MtvSolicitudID == 3 || Solicitud.MtvSolicitudID == 8)
                    {
                        Correo = db.Configuraciones.First(s => s.Parametro == "CORREOGERENCIA").Valor.ToString();
                        EnviarCorreoNotifiRequerimientoNuevoCargo(Solicitud.Id, nombreemp.Nombres, Correo, Solicitud.Cargo, Solicitud.Area, Solicitud.EmpresaId, Solicitud.NumeroPresonas);
                    }
                }
                if (val == "Válido")
                {
                    return Json(new { respuesta = "Se ha aprobado con exito la Solicitud: " + Id, isRedirect = true });
                }
                else
                {
                    return Json(new { respuesta = "Se ha producido un error: " + val, isRedirect = false });
                }

            }
            catch (SystemException ex)
            {
                return Json(new { respuesta = respuesta + "//" + ex });
            }

        }

        public JsonResult AnularSolicitudRdP()
        {
            var message = "";
            try
            {
                var IdSolicitud = HttpContext.Request.Params["IdSolicitud"];
                var Observacion = HttpContext.Request.Params["Observación"];

                var Id = Convert.ToInt32(IdSolicitud);
                var userId = Session["Empleado"];
                var userlog = Convert.ToInt32(userId);
                var Solicitud = db.RequerimientosDelPersonal.FirstOrDefault(x => x.Id == Id);

                Aprobacioneslog Model = new Aprobacioneslog();
                Model.IdRequerimiento = Id;
                Model.Fecha = DateTime.Now;
                Model.Observación = Observacion;
                Model.Usuario = userlog;
                Model.EstadoAnterior = Solicitud.EstadoID;

                message = _repo.Anular(Id, Model);
                if (message == "Válido") { return Json(new { respuesta = "Se ha Anulado la solicitud" + Id + " " + "de manera Exitosa", isRedirect = true }); }
                else { return Json(new { respuesta = message, isRedirect = false }); }

            }
            catch (SystemException ex)
            {
                return Json(new { respuesta = ex });
            }
        }
        public ActionResult EditarEstSelec(int id, int estado)
        {
            //var model = db.RequerimientosDelPersonal.Where(a=> a.Id == id);
            var model = _repo.ObtenerTodos2(id);
            ViewBag.EstadoSelec = db.EstadoSeleccionRdP.ToList();
            ViewBag.Requerimiento = model;
            return PartialView(model);
        }
        public JsonResult ModifiEstadoSelec()
        {
            var message = "";
            try
            {
                var IdSolicitud = HttpContext.Request.Params["IdSolicitud"];
                var EstadoSelec = Convert.ToInt32(HttpContext.Request.Params["EstadoSelec"]);
                var estadosel = db.EstadoSeleccionRdP.Where(a=> a.Id == EstadoSelec).FirstOrDefault();
                var Id = Convert.ToInt32(IdSolicitud);
                var userId = Session["Empleado"];
                var userlog = Convert.ToInt32(userId);
                var Solicitud = db.RequerimientosDelPersonal.FirstOrDefault(x => x.Id == Id);
                var estadosele = estadosel.Id;
                Aprobacioneslog Model = new Aprobacioneslog();
                Model.IdRequerimiento = Id;
                Model.Fecha = DateTime.Now;
                Model.Observación = "Modificacion Estado Seleccion a "+ estadosel.Nombre;
                Model.Usuario = userlog;
                Model.EstadoAnterior = Solicitud.EstadoID;

                message = _repo.CambiarEstadoSeleccion(Id, estadosele, Model);
                if (message == "Válido") { return Json(new { respuesta = "Se ha Modificado el Estado de Seleccion de la solicitud" + Id + " " + "de manera Exitosa", isRedirect = true }); }
                else { return Json(new { respuesta = message, isRedirect = false }); }

            }
            catch (SystemException ex)
            {
                return Json(new { respuesta = ex });
            }
        }
        
        public JsonResult ReactivarSolicitud()
        {
            var message = "";
            try
            {
                var IdSolicitud = HttpContext.Request.Params["IdSolicitud"];
                var Observacion = HttpContext.Request.Params["Observación"];

                var Id = Convert.ToInt32(IdSolicitud);
                var userId = Session["Empleado"];
                var userlog = Convert.ToInt32(userId);
                var Solicitud = db.RequerimientosDelPersonal.FirstOrDefault(x => x.Id == Id);

                Aprobacioneslog Model = new Aprobacioneslog();
                Model.IdRequerimiento = Id;
                Model.Fecha = DateTime.Now;
                Model.Observación = "Reactivar Solicitud";
                Model.Usuario = userlog;
                Model.EstadoAnterior = Solicitud.EstadoID;

                message = _repo.Reactivar(Id, Model);
                if (message == "Válido") { return Json(new { respuesta = "Se ha Reactivado la Solicitud" + Id + " " + "de Manera Exitosa", isRedirect = true }); }
                else { return Json(new { respuesta = message, isRedirect = false }); }

            }
            catch (SystemException ex)
            {
                return Json(new { respuesta = ex });
            }
        }
        public JsonResult DetalleContratacion2()
        {
            var IdSolicitud = HttpContext.Request.Params["IdSolicitud"];
            var Observacion = HttpContext.Request.Params["Observación"];
            var Contratados = HttpContext.Request.Params["Contratados"];
            var Cantidadempleados = HttpContext.Request.Params["Cantidadempleados"];
            var filas = Contratados.Split(',');
            var Id = Convert.ToInt32(IdSolicitud);
            var userId = Session["Empleado"];
            var userlog = Convert.ToInt32(userId);
            var Solicitud = db.RequerimientosDelPersonal.FirstOrDefault(x => x.Id == Id);
            var Registros = db.DetalleRequerimiento.Where(x => x.RequerimientoId == Id && x.Contratado != null).Count();
            if (Convert.ToInt32(Registros) > Solicitud.NumeroPresonas)
            {
                return Json(new { respuesta = "La cantidad de empleados contratados debe ser menor o igual a la cantidad de empleados solicitados.", isRedirect = false });
            }
            else
            {
                Aprobacioneslog Model = new Aprobacioneslog();
                Model.IdRequerimiento = Id;
                Model.Fecha = DateTime.Now;
                Model.Observación = Observacion;
                Model.Usuario = userlog;
                Model.EstadoAnterior = Solicitud.EstadoID;
                var val = "";
                if (filas.Count() + Registros == Solicitud.NumeroPresonas)
                {
                    val = _repo.Cerrar(Id, Model, filas);
                }
                else if (filas.Count() + Registros > Solicitud.NumeroPresonas)
                {
                    return Json(new { respuesta = "La Cantidad de Personal Contratado Supera el requerimiento." });
                }
                else
                {
                    val = _repo.Cerrar_Parcial(Id, Model, filas);
                }
                if (val == "Válido")
                {
                    return Json(new { respuesta = "La solicitud se ha Tramitado de Manera Exitosa", isRedirect = true });
                }
                else
                {
                    return Json(new { respuesta = "" + val, isRedirect = false });
                }
            }
        }

        public ActionResult InformeRdP(int? EstadoId, string NmrSolicitud, string MtvSolicitud, string Sociedad, string FechaRI, string FechaRF, string EmpleadoRegistraId)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("InformeGeneralRdP"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var Id = Session["Empleado"];
            var empleado = db.Empleados.Find(Id);
            var Areas = db.Empleados.Where(x => x.Area != null && x.Area != "" && x.Jefe == empleado.NroEmpleado).Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
            using (var db = new AutogestionContext())
            {
                ViewBag.Empresa = db.Sociedad.Select(y => new { y.Id, y.Descripcion }).ToList();
                var estados = db.EstadoRdP.Select(y => new { y.Id, y.Nombre }).ToList();
                estados.Add(new { Id = 0, Nombre = "Todos" });

                ViewBag.Estado = estados;
                ViewBag.Empleado = db.Empleados.Select(e => new { e.Id, e.Nombres, e.Activo, nomcodigo = e.Id + "-" + e.Nombres }).Where(f => f.Activo != "NO").ToList();
                List<RequerimientosDelPersonal> Datos = new List<RequerimientosDelPersonal>();
                //List<RequerimientosDelPersonal> Requerimientos = new List<RequerimientosDelPersonal>();
                DateTime Fecha1 = DateTime.Now;
                DateTime Fecha2 = DateTime.Now;
                var empleadoId = 0;
                var idSociedad = 0;

                if (EmpleadoRegistraId != "")
                {
                    empleadoId = Convert.ToInt32(EmpleadoRegistraId);
                }
                if (Sociedad != "")
                {
                    idSociedad = Convert.ToInt32(Sociedad);
                }

                if (!DateTime.TryParse(FechaRI, out Fecha1))
                {
                    Fecha1 = new DateTime();
                }

                if (!DateTime.TryParse(FechaRF, out Fecha2))
                {
                    Fecha2 = DateTime.Now;
                }
                var Id_Est = EstadoId.ToString();
                var Requerimientos = db.RequerimientosDelPersonal.Where(e => DbFunctions.TruncateTime(e.Fecha) >= Fecha1 && DbFunctions.TruncateTime(e.Fecha) <= Fecha2);
                if (!string.IsNullOrEmpty(Id_Est))
                {
                    Requerimientos = Requerimientos.Where(e => SqlFunctions.StringConvert((decimal)e.EstadoID).Contains(Id_Est));
                }
                if (!string.IsNullOrEmpty(EmpleadoRegistraId))
                {
                    Requerimientos = Requerimientos.Where(e => SqlFunctions.StringConvert((decimal)e.EmpleadoRegistraId).Contains(EmpleadoRegistraId));
                }
                if (!string.IsNullOrEmpty(Sociedad))
                {
                    Requerimientos = Requerimientos.Where(e => SqlFunctions.StringConvert((decimal)e.EmpresaId).Contains(Sociedad));
                }
                if (!string.IsNullOrEmpty(NmrSolicitud))
                {
                    Requerimientos = Requerimientos.Where(e => SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrSolicitud));
                }

                if (funciones.Contains("RdPAprobacionDirectorArea"))
                {
                    foreach (var i in Areas)
                    {
                        foreach (var item in Requerimientos)
                        {
                            if (item.Area == i.Key)
                            {
                                Datos.Add(item);
                            }
                        }
                    }
                }
                else if (funciones.Contains("RdPGerencia"))
                {
                    foreach (var item in Requerimientos)
                    {
                        if (item.MtvSolicitudID == 2 || item.MtvSolicitudID == 3 || item.MtvSolicitudID == 7 || item.MtvSolicitudID == 8)
                        {
                            Datos.Add(item);
                        }
                    }
                }
                else if (funciones.Contains("RdPGestionHumana"))
                {
                    foreach (var item in Requerimientos)
                    {
                        Datos.Add(item);
                    }
                }
                else if (funciones.Contains("EncargadoContratacionRdP") || funciones.Contains("RdPEncargadoSeleccionNovedad"))
                {
                    foreach (var item in Requerimientos)
                    {
                        if (item.EncargadoContratacion == empleado.Id)
                        {
                            Datos.Add(item);
                        }
                    }
                }
                else if (funciones.Contains("InformeGeneralRdP")) // Jefe de Area
                {
                    foreach (var item in Requerimientos)
                    {
                        if (item.EmpleadoRegistraId == empleado.Id)
                        {
                            Datos.Add(item);
                        }
                    }
                }
                
                foreach (RequerimientosDelPersonal Item in Datos)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
                    Item.Empresa = db.Sociedad.FirstOrDefault(e => e.Id == Item.EmpresaId);
                    Item.EstadoRdP = db.EstadoRdP.FirstOrDefault(e => e.Id == Item.EstadoID);
                    Item.EstadoSeleccionRdP = db.EstadoSeleccionRdP.FirstOrDefault(e => e.Id == Item.EstadoSeleccion);
                    Item.Jornada = db.Jornada.FirstOrDefault(e => e.Id == Item.JornadaRequeridaId);
                    Item.Horario = db.Horario.FirstOrDefault(e => e.Id == Item.HorarioTrabajoId);
                    //Item.MotivoEgreso = db.MotivoEgreso.FirstOrDefault(e => e.Id == Item.MotivoEgresoId);
                    Item.MtvSolicitud = db.MtvSolicitud.FirstOrDefault(e => e.Id == Item.MtvSolicitudID);

                }
                return View(Datos);
            }
        }

        public ActionResult DashBoard()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("RequerimientosPersonalDashboard"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var Cant_Cubiertos_Princi = db.RequerimientosDelPersonal.ToList();
            var Total_Cub = 0;
            var CntRdPSolicitado = 0;
            var CntRdPAprobadoDirectorArea = 0;
            var CntRdPAprobadoGerencia = 0;
            var CntRdPSelección = 0;
            var Cant_Cerrados = 0;
            var Cant_Anulados = 0;
            var Total = 0;
            var Id = Session["Empleado"];
            var empleado = db.Empleados.Find(Id);
            var Areas = db.Empleados.Where(x => x.Area != null && x.Area != "" && x.Jefe == empleado.NroEmpleado).Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
            
            if (funciones.Contains("RdPAprobacionDirectorArea"))
            {
                foreach (var i in Areas)
                {
                    foreach (var item in Cant_Cubiertos_Princi)
                    {
                        if (item.Area == i.Key)
                        {
                            var Cant_Solicitada = Convert.ToInt32(item.NumeroPresonas);
                            var Cant_Contratada = db.DetalleRequerimiento.Where(a => a.Contratado != null && a.RequerimientoId == item.Id).Count();
                            if (Cant_Solicitada == Cant_Contratada)
                            {
                                Total_Cub++;
                            }
                            if (item.EstadoID == 6)
                            {
                                Cant_Cerrados++;
                            }
                            else if (item.EstadoID == 1)
                            {
                                CntRdPSolicitado++;
                            }
                            else if (item.EstadoID == 3)
                            {
                                CntRdPAprobadoDirectorArea++;
                            }
                            else if (item.EstadoID == 4)
                            {
                                CntRdPAprobadoGerencia++;
                            }
                            else if (item.EstadoID == 5)
                            {
                                CntRdPSelección++;
                            }
                            else if (item.EstadoID == 7)
                            {
                                Cant_Anulados++;
                            }
                            Total++;                           
                            //Datos.Add(item);
                        }
                    }
                }
            }
            else if (funciones.Contains("RdPGerencia"))
            {
                foreach (var item in Cant_Cubiertos_Princi)
                {
                    if (item.MtvSolicitudID == 2 || item.MtvSolicitudID == 3 || item.MtvSolicitudID == 7 || item.MtvSolicitudID == 8)
                    {
                        var Cant_Solicitada = Convert.ToInt32(item.NumeroPresonas);
                        var Cant_Contratada = db.DetalleRequerimiento.Where(a => a.Contratado != null && a.RequerimientoId == item.Id).Count();
                        if (Cant_Solicitada == Cant_Contratada)
                        {
                            Total_Cub++;
                        }
                        if (item.EstadoID == 6)
                        {
                            Cant_Cerrados++;
                        }
                        else if (item.EstadoID == 1)
                        {
                            CntRdPSolicitado++;
                        }
                        else if (item.EstadoID == 3)
                        {
                            CntRdPAprobadoDirectorArea++;
                        }
                        else if (item.EstadoID == 4)
                        {
                            CntRdPAprobadoGerencia++;
                        }
                        else if (item.EstadoID == 5)
                        {
                            CntRdPSelección++;
                        }
                        else if (item.EstadoID == 7)
                        {
                            Cant_Anulados++;
                        }
                        Total++;
                        //Datos.Add(item);
                    }
                }
            }
            else if (funciones.Contains("RdPGestionHumana"))
            {
                foreach (var item in Cant_Cubiertos_Princi)
                {
                    var Cant_Solicitada = Convert.ToInt32(item.NumeroPresonas);
                    var Cant_Contratada = db.DetalleRequerimiento.Where(a => a.Contratado != null && a.RequerimientoId == item.Id).Count();
                    if (Cant_Solicitada == Cant_Contratada)
                    {
                        Total_Cub++;
                    }
                    if (item.EstadoID == 6)
                    {
                        Cant_Cerrados++;
                    }
                    else if (item.EstadoID == 1)
                    {
                        CntRdPSolicitado++;
                    }
                    else if (item.EstadoID == 3)
                    {
                        CntRdPAprobadoDirectorArea++;
                    }
                    else if (item.EstadoID == 4)
                    {
                        CntRdPAprobadoGerencia++;
                    }
                    else if (item.EstadoID == 5)
                    {
                        CntRdPSelección++;
                    }
                    else if (item.EstadoID == 7)
                    {
                        Cant_Anulados++;
                    }
                    Total++;
                    //Datos.Add(item);
                }
            }
            else if (funciones.Contains("EncargadoContratacionRdP") || funciones.Contains("RdPEncargadoSeleccionNovedad"))
            {
                foreach (var item in Cant_Cubiertos_Princi)
                {
                    if (item.EncargadoContratacion == empleado.Id)
                    {
                        var Cant_Solicitada = Convert.ToInt32(item.NumeroPresonas);
                        var Cant_Contratada = db.DetalleRequerimiento.Where(a => a.Contratado != null && a.RequerimientoId == item.Id).Count();
                        if (Cant_Solicitada == Cant_Contratada)
                        {
                            Total_Cub++;
                        }
                        if (item.EstadoID == 6)
                        {
                            Cant_Cerrados++;
                        }
                        else if (item.EstadoID == 1)
                        {
                            CntRdPSolicitado++;
                        }
                        else if (item.EstadoID == 3)
                        {
                            CntRdPAprobadoDirectorArea++;
                        }
                        else if (item.EstadoID == 4)
                        {
                            CntRdPAprobadoGerencia++;
                        }
                        else if (item.EstadoID == 5)
                        {
                            CntRdPSelección++;
                        }
                        else if (item.EstadoID == 7)
                        {
                            Cant_Anulados++;
                        }
                        Total++;
                        //Datos.Add(item);
                    }
                }
            }
            else if (funciones.Contains("InformeGeneralRdP")) // Jefe de Area
            {
                foreach (var item in Cant_Cubiertos_Princi)
                {
                    if (item.EmpleadoRegistraId == empleado.Id)
                    {
                        var Cant_Solicitada = Convert.ToInt32(item.NumeroPresonas);
                        var Cant_Contratada = db.DetalleRequerimiento.Where(a => a.Contratado != null && a.RequerimientoId == item.Id).Count();
                        if (Cant_Solicitada == Cant_Contratada)
                        {
                            Total_Cub++;
                        }
                        if (item.EstadoID == 6)
                        {
                            Cant_Cerrados++;
                        }
                        else if (item.EstadoID == 1)
                        {
                            CntRdPSolicitado++;
                        }
                        else if (item.EstadoID == 3)
                        {
                            CntRdPAprobadoDirectorArea++;
                        }
                        else if (item.EstadoID == 4)
                        {
                            CntRdPAprobadoGerencia++;
                        }
                        else if (item.EstadoID == 5)
                        {
                            CntRdPSelección++;
                        }
                        else if (item.EstadoID == 7)
                        {
                            Cant_Anulados++;
                        }
                        Total++;
                        //Datos.Add(item);
                    }
                }
            }
            ViewBag.Total_Cub = Total_Cub;
            ViewBag.CntRdPSolicitado = CntRdPSolicitado;
            ViewBag.CntRdPAprobadoDirectorArea = CntRdPAprobadoDirectorArea;
            ViewBag.CntRdPAprobadoGerencia = CntRdPAprobadoGerencia;
            ViewBag.CntRdPSelección = CntRdPSelección;
            ViewBag.Cant_Cerrados = Cant_Cerrados;
            ViewBag.Cant_Anulados = Cant_Anulados;
            ViewBag.Total = Total;
            return View();
        }
        public ActionResult Semaforo(string color)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            List<RequerimientosDelPersonal> Solicitudes = null;
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("RequerimientosPersonalSemaforo"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var idemp =Convert.ToInt32(Session["Empleado"]);
            var FEC = DateTime.Now.AddMonths(-11);
            if (funciones.Contains("EncargadoContratacionRdP") || funciones.Contains("RdPEncargadoSeleccionNovedad"))
            {
                Solicitudes = db.RequerimientosDelPersonal.Where(x =>  x.EstadoID != 6 && x.EstadoID != 7 && x.Fecha >= FEC && x.EncargadoContratacion == idemp).OrderByDescending(a => a.Fecha).ToList();
            }
            else 
            {
                Solicitudes = db.RequerimientosDelPersonal.Where(x => x.EstadoID != 5 && x.EstadoID != 6 && x.EstadoID != 7 /*&& x.Fecha >= FEC*/).OrderByDescending(a => a.Fecha).ToList();
            }            
            foreach (var Item in Solicitudes)
            {
                var R = VerificarFechaN(Item.Fecha);
                var colortiempo = R.Split(';');
                var dift = DateTime.Now - Item.Fecha;
                Item.Color = colortiempo[0];
                Item.Tiempo = colortiempo[1];
                Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
                Item.Empresa = db.Sociedad.FirstOrDefault(e => e.Id == Item.EmpresaId);
                Item.EstadoRdP = db.EstadoRdP.FirstOrDefault(e => e.Id == Item.EstadoID);
                Item.Jornada = db.Jornada.FirstOrDefault(e => e.Id == Item.JornadaRequeridaId);
                Item.Horario = db.Horario.FirstOrDefault(e => e.Id == Item.HorarioTrabajoId);
                Item.MtvSolicitud = db.MtvSolicitud.FirstOrDefault(e => e.Id == Item.MtvSolicitudID);
            }
            ViewBag.Verde = Solicitudes.Where(x => x.Color == "verde").Count();
            ViewBag.Amarillo = Solicitudes.Where(x => x.Color == "amarillo").Count();
            ViewBag.Rojo = Solicitudes.Where(x => x.Color == "rojo").Count();
            ViewBag.Total = Solicitudes.Count();
            if (color != "" && color != null)
            {
                Solicitudes = Solicitudes.Where(x => x.Color.Contains(color)).ToList();
            }
            return View(Solicitudes);
        }
        public ActionResult EncargadoContratacion()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EncargadoContratacionRdP") && !funciones.Contains("RdPEncargadoSeleccionNovedad"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var Id = Session["Empleado"];
            var Emplog = Convert.ToInt32(Id);

            var Items = _repo.ObtenerTodos4(Emplog);
            return View(Items);
        }

        public string VerificarFechaN(DateTime FechaNac)
        {
            var Validar = "Válido";
            if (FechaNac > DateTime.Now)
            {
                Validar = "La Fecha no puede ser Mayor a la fecha actual";
            }
            else
            {
                var Tiempo = 0;
                int dif = 0;
                int fes = 0;
                var momento1 = FechaNac.Date;
                var momento2 = DateTime.Now.Date;
                var dia = momento1.DayOfWeek;
                if (momento1.DayOfWeek == DayOfWeek.Monday || momento1.DayOfWeek == DayOfWeek.Tuesday || momento1.DayOfWeek == DayOfWeek.Wednesday || momento1.DayOfWeek == DayOfWeek.Thursday)
                {
                    momento1 = momento1.AddDays(1);
                }
                else
                {
                    if (momento1.DayOfWeek == DayOfWeek.Friday)
                    {
                        momento1 = momento1.AddDays(3);
                    }
                    else if (momento1.DayOfWeek == DayOfWeek.Saturday)
                    {
                        momento1 = momento1.AddDays(2);
                    }
                    else if (momento1.DayOfWeek == DayOfWeek.Sunday)
                    {
                        momento1 = momento1.AddDays(1);
                    }

                    var festivos = db.DiasFestivos.Where(e => DbFunctions.TruncateTime(e.festivo) == momento1).ToList();
                    if (festivos.Count != 0)
                    {
                        momento1 = momento1.AddDays(1);
                    }
                }
                for (var day = momento1.Date; day.Date <= momento2.Date; day = day.AddDays(1))
                {
                    var dia1 = day.DayOfWeek;
                    if (day.DayOfWeek == DayOfWeek.Monday || day.DayOfWeek == DayOfWeek.Tuesday || day.DayOfWeek == DayOfWeek.Wednesday || day.DayOfWeek == DayOfWeek.Thursday || day.DayOfWeek == DayOfWeek.Friday)
                    {
                        dif = dif + 1;
                    }
                    var festivos = db.DiasFestivos.Where(e => DbFunctions.TruncateTime(e.festivo) == day).ToList();
                    if (festivos.Count != 0)
                    {
                        fes = fes + 1;
                    }
                }
                Tiempo = dif - fes;
                if (Tiempo > 20)
                {
                    Validar = "rojo;" + Tiempo;
                }
                if (Tiempo <= 20 && Tiempo >= 10)
                {
                    Validar = "amarillo;" + Tiempo;
                }
                if (Tiempo < 10)
                {
                    Validar = "verde;" + Tiempo;
                }
            }
            return Validar;
        }
        public String EnviarEmailFirma(int id)
        {

            using (var db = new AutogestionContext())
            {

                try
                {

                    var Detalle = db.RequerimientosDelPersonal.FirstOrDefault(x => x.Id == id);

                    _servicios.EnviarEmailRdP(Detalle);

                    return "OK";
                }
                catch (Exception ex)
                {

                    return ex.Message.ToString();
                }


            }

        }

        [HttpGet]
        public ActionResult FirmaSolicitud(string str)
        {

            try
            {

                var base64EncodedBytes = System.Convert.FromBase64String(str);

                string[] valores = System.Text.Encoding.UTF8.GetString(base64EncodedBytes).Split('|');
                var id = Convert.ToInt16(valores[0]);
                var id_empleado = Convert.ToInt16(valores[1]);
                var fuente = valores[2];


                RequerimientosDelPersonal detalle = new RequerimientosDelPersonal();
                Aprobacioneslog Aprobacion = new Aprobacioneslog();
                Empleado empleado = new Empleado();
                if (fuente != "email")
                {


                    ViewBag.Message = "no valido para firmar";

                }
                using (var db = new AutogestionContext())
                {
                    detalle = db.RequerimientosDelPersonal.FirstOrDefault(x => x.Id == id && x.EstadoID == 1);


                    if (detalle != null)
                    {

                        empleado = db.Empleados.FirstOrDefault(x => x.Id == id_empleado);

                        if (empleado != null)
                        {
                            Aprobacion.Fecha = DateTime.Now;
                            Aprobacion.EstadoAnterior = 1;
                            Aprobacion.EstadoNuevo = 3;
                            Aprobacion.IdRequerimiento = detalle.Id;
                            Aprobacion.Usuario = empleado.Id;
                            detalle.EstadoID = 3;
                            db.Aprobacioneslog.Add(Aprobacion);
                            db.SaveChanges();
                            ViewBag.Message = "Se ha Firmado la  Solicitud " + detalle.Id;
                        }
                        else
                        {

                            ViewBag.Message = "Colaborador no existe";
                        }
                    }
                    else
                    {

                        ViewBag.Message = "La Solicitud ya ha sido firmada o no existe.";
                    }

                }

            }
            catch (Exception ex)
            {
                ViewBag.Message = "Ha Ocurrio un Error:" + ex.Message;
            }
            ViewBag.firma = "Se termino proceso de Firma";
            return View();
        }


        public ActionResult FirmaSolicitud2(string str)
        {
            RequerimientosDelPersonal ModeloRdP = new RequerimientosDelPersonal();
            try
            {

                var base64EncodedBytes = System.Convert.FromBase64String(str);

                string[] valores = System.Text.Encoding.UTF8.GetString(base64EncodedBytes).Split('|');
                var id = Convert.ToInt16(valores[0]);
                var id_empleado = Convert.ToInt16(valores[1]);
                var fuente = valores[2];



                RequerimientosDelPersonal detalle = new RequerimientosDelPersonal();
                Empleado empleado = new Empleado();
                if (fuente != "email")
                {


                    ViewBag.Message = "no valido para firmar";

                }
                using (var db = new AutogestionContext())
                {
                    detalle = db.RequerimientosDelPersonal.FirstOrDefault(x => x.Id == id && x.EstadoID == 1);
                    //ViewBag.Requerimiento = detalle.Id;
                    ModeloRdP = detalle;


                    if (detalle != null)
                    {

                        empleado = db.Empleados.FirstOrDefault(x => x.Id == id_empleado);

                        if (empleado != null)
                        {



                        }
                        else
                        {

                            ViewBag.Message = "Colaborador no existe";
                        }
                    }
                    else
                    {

                        ViewBag.Message = "La Solicitud ya ha sido firmada o no existe.";
                    }

                }

            }
            catch (Exception ex)
            {
                ViewBag.Message = "Ocurrio un Error:" + ex.Message;
            }
            ViewBag.firma = "Se termino proceso de Firma";
            return View(ModeloRdP);
        }
        //______________________________________________________________________________________________________________//




        //---------------- Correo Notificacion nuevo Requerimiento de Personal --------------------
        public bool EnviarCorreoNotifiRequerimiento(string NombreJefe, string Correo, string Cargo, string Area, int EmpresaId, int NumeroPresonas)
        {
            string textocorreo = "";
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
            using (var db = new AutogestionContext())
            {
                var texto = db.Configuraciones.First(s => s.Parametro == "TEXTOCORREOCREAREQ").Valor.ToString();
                var link = db.Configuraciones.First(s => s.Parametro == "LINKREQPER").Valor.ToString();
                texto = texto.Replace("$JEFESOLICITUD", NombreJefe);
                texto = texto.Replace("$CARGOREQUERIMIENTO", Cargo);
                texto = texto.Replace("$AREAREQUERIMIENTO", Area);
                texto = texto.Replace("$EMPRESAREQUERIMIETNO", EmpresaId.ToString());
                texto = texto.Replace("$NUMEROPERSONAS", NumeroPresonas.ToString());
                texto = texto.Replace("$LINK", link);
                textocorreo = texto;
            }
            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);
                if (Correo != null)
                {
                    correo.To.Add(Correo);
                    correo.Subject = "Notificación Registro Nuevo Requerimiento de Personal";
                    correo.Body = textocorreo;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;
                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    smtp.Send(correo);
                }
                confirmacion = true;
                return confirmacion;
            }
            catch (Exception ex)
            {
                confirmacion = false;
                return confirmacion;
            }
        }
        //---------------- Correo Notificacion nuevo Cargo y Requerimiento de Personal --------------------
        public bool EnviarCorreoNotifiRequerimientoNuevoCargo(int? RequerimientoId,string NombreJefe, string Correo, string Cargo, string Area, int EmpresaId, int NumeroPresonas)
        {
            string textocorreo = "";
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
            using (var db = new AutogestionContext())
            {
                var texto = db.Configuraciones.First(s => s.Parametro == "TEXTOCORREOCREAREQNCARGO").Valor.ToString();
                var link = db.Configuraciones.First(s => s.Parametro == "LINKREQPER").Valor.ToString();
                texto = texto.Replace("$JEFESOLICITUD", NombreJefe);
                texto = texto.Replace("$CARGOREQUERIMIENTO", Cargo);
                texto = texto.Replace("$AREAREQUERIMIENTO", Area);
                texto = texto.Replace("$EMPRESAREQUERIMIETNO", EmpresaId.ToString());
                texto = texto.Replace("$NUMEROPERSONAS", NumeroPresonas.ToString());
                texto = texto.Replace("$LINK", link);
                textocorreo = texto;
            }
            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);
                //if (Correo != null)
                //{
                    var CorreosNuevoCargo = db.Configuraciones.First(s => s.Parametro == "RDPCORREOSNUEVOCARGO").Valor.ToString();
                    var arc = db.RequerimientosDelPersonal.Where(a => a.Id == RequerimientoId).FirstOrDefault();
                    string agregar = Server.MapPath("~/AnexosRequerimientosPersonal/"+ arc.Archivo);
                    correo.Attachments.Add(new Attachment(agregar));
                    //char[] Separador = { ' ', , '.', ':', '\t' };
                    string[] CorreosNC = CorreosNuevoCargo.Split(',');
                    foreach (var co in CorreosNC)
                    {
                        correo.To.Add(co);
                    }                    
                    //correo.To.Add("margarita.pena@foscal.com.co,claudia.ramirez@foscal.com.co");
                    correo.Subject = "Notificación Registro Nuevo Cargo Para Requerimiento de Personal";
                    correo.Body = textocorreo;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;
                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    smtp.Send(correo);
                //}
                confirmacion = true;
                return confirmacion;
            }
            catch (Exception ex)
            {
                confirmacion = false;
                return confirmacion;
            }
        }





        //-----------------------************ Reportes Gaficos Requerimientos de personal *****************-----------------------------------
        //---------------- Clase Para los Graficos --------
        public class Graficos
        {
            //------------- Graficos Circulares -----------
            public int Cantidad_Solicitados { get; set; }
            public int Cantidad_Apro_Director { get; set; }
            public int Cantidad_Apro_Gerencia { get; set; }
            public int Cantidad_Seleccion { get; set; }
            public int Cantidad_Cerrados { get; set; }
            public int Cantidad_Anulados { get; set; }
            public int Cant_Cubiertos { get; set; }
            public int Cant_Por_Cubrir { get; set; }
            public int Total_Circular { get; set; }
            //**********************************************


            //------------- Graficos Tipo Barra ------------
            public int Total_Por_Cargo_Cont { get; set; }
            public string Total_Por_Cargo_Nomb { get; set; }
            //----------------------------------------------            
            public int Total_Por_Area_Cont { get; set; }
            public string Total_Por_Area_Nomb { get; set; }
            //----------------------------------------------
            public int Total_Por_Mes_Cont { get; set; }
            public string Total_Por_Mes_Nomb { get; set; }
            //----------------------------------------------
            public int Total_Por_Enca_Cont { get; set; }
            public string Total_Por_Enca_Nomb { get; set; }
            //----------------------------------------------        
            public int Total_Por_Mtv_Cont { get; set; }
            public string Total_Por_Mtv_Nomb { get; set; }
            //----------------------------------------------                   
            public int Total_Cargos_Por_Area_Cont { get; set; }
            public string Total_Cargos_Por_Area_Nomb { get; set; }
            //----------------------------------------------
            public string Total_Cargos_Por_Mes_Nomb { get; set; }
            public string Total_Cargos_Por_Mes_Nomb2 { get; set; }

            public int Total_Cargos_Por_Mes_Cont { get; set; }

            //**********************************************
        }
        //*************--------------------*****************

        //************* Graficos Circulares ****************
        [HttpPost]
        public JsonResult Dashboard_Graficos_Principal()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            List<Graficos> Items = new List<Graficos>();
            //var Totales = db.RequerimientosDelPersonal.Count();
            //var Solicitados = db.RequerimientosDelPersonal.Where(x => x.EstadoID == 1).Count();
            //var Apro_Dir = db.RequerimientosDelPersonal.Where(x => x.EstadoID == 3).Count();
            //var Apro_Ger = db.RequerimientosDelPersonal.Where(x => x.EstadoID == 4).Count();
            //var Cant_Sel = db.RequerimientosDelPersonal.Where(x => x.EstadoID == 5).Count();
            //var Cant_Cerrados = db.RequerimientosDelPersonal.Where(x => x.EstadoID == 6).Count();
            //var Cant_Anulados = db.RequerimientosDelPersonal.Where(x => x.EstadoID == 7).Count();
            //var Cant_Cubiertos_Princi = db.RequerimientosDelPersonal.ToList();
            //var Total_Cub = 0;
            var Total_X_Cubrir = 0;


            var Cant_Cubiertos_Princi = db.RequerimientosDelPersonal.ToList();
            var Total_Cub = 0;
            var CntRdPSolicitado = 0;
            var CntRdPAprobadoDirectorArea = 0;
            var CntRdPAprobadoGerencia = 0;
            var CntRdPSelección = 0;
            var Cant_Cerrados = 0;
            var Cant_Anulados = 0;
            var Total = 0;
            var Id = Session["Empleado"];
            var empleado = db.Empleados.Find(Id);
            var Areas = db.Empleados.Where(x => x.Area != null && x.Area != "" && x.Jefe == empleado.NroEmpleado).Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();

            if (funciones.Contains("RdPAprobacionDirectorArea"))
            {
                foreach (var i in Areas)
                {
                    foreach (var item in Cant_Cubiertos_Princi)
                    {
                        if (item.Area == i.Key)
                        {
                            var Cant_Solicitada = Convert.ToInt32(item.NumeroPresonas);
                            var Cant_Contratada = db.DetalleRequerimiento.Where(a => a.Contratado != null && a.RequerimientoId == item.Id).Count();
                            if (Cant_Solicitada == Cant_Contratada)
                            {
                                Total_Cub++;
                            }
                            else
                            {
                                Total_X_Cubrir++;
                            }
                            if (item.EstadoID == 6)
                            {
                                Cant_Cerrados++;
                            }
                            else if (item.EstadoID == 1)
                            {
                                CntRdPSolicitado++;
                            }
                            else if (item.EstadoID == 3)
                            {
                                CntRdPAprobadoDirectorArea++;
                            }
                            else if (item.EstadoID == 4)
                            {
                                CntRdPAprobadoGerencia++;
                            }
                            else if (item.EstadoID == 5)
                            {
                                CntRdPSelección++;
                            }
                            else if (item.EstadoID == 7)
                            {
                                Cant_Anulados++;
                            }
                            Total++;
                            //Datos.Add(item);
                        }
                    }
                }
            }
            else if (funciones.Contains("RdPGerencia"))
            {
                foreach (var item in Cant_Cubiertos_Princi)
                {
                    if (item.MtvSolicitudID == 2 || item.MtvSolicitudID == 3 || item.MtvSolicitudID == 7 || item.MtvSolicitudID == 8)
                    {
                        var Cant_Solicitada = Convert.ToInt32(item.NumeroPresonas);
                        var Cant_Contratada = db.DetalleRequerimiento.Where(a => a.Contratado != null && a.RequerimientoId == item.Id).Count();
                        if (Cant_Solicitada == Cant_Contratada)
                        {
                            Total_Cub++;
                        }
                        else
                        {
                            Total_X_Cubrir++;
                        }


                        if (item.EstadoID == 6)
                        {
                            Cant_Cerrados++;
                        }
                        else if (item.EstadoID == 1)
                        {
                            CntRdPSolicitado++;
                        }
                        else if (item.EstadoID == 3)
                        {
                            CntRdPAprobadoDirectorArea++;
                        }
                        else if (item.EstadoID == 4)
                        {
                            CntRdPAprobadoGerencia++;
                        }
                        else if (item.EstadoID == 5)
                        {
                            CntRdPSelección++;
                        }
                        else if (item.EstadoID == 7)
                        {
                            Cant_Anulados++;
                        }
                        Total++;
                        //Datos.Add(item);
                    }
                }
            }
            else if (funciones.Contains("RdPGestionHumana"))
            {
                foreach (var item in Cant_Cubiertos_Princi)
                {
                    var Cant_Solicitada = Convert.ToInt32(item.NumeroPresonas);
                    var Cant_Contratada = db.DetalleRequerimiento.Where(a => a.Contratado != null && a.RequerimientoId == item.Id).Count();
                    if (Cant_Solicitada == Cant_Contratada)
                    {
                        Total_Cub++;
                    }
                    else
                    {
                        Total_X_Cubrir++;
                    }

                    if (item.EstadoID == 6)
                    {
                        Cant_Cerrados++;
                    }
                    else if (item.EstadoID == 1)
                    {
                        CntRdPSolicitado++;
                    }
                    else if (item.EstadoID == 3)
                    {
                        CntRdPAprobadoDirectorArea++;
                    }
                    else if (item.EstadoID == 4)
                    {
                        CntRdPAprobadoGerencia++;
                    }
                    else if (item.EstadoID == 5)
                    {
                        CntRdPSelección++;
                    }
                    else if (item.EstadoID == 7)
                    {
                        Cant_Anulados++;
                    }
                    Total++;
                    //Datos.Add(item);
                }
            }
            else if (funciones.Contains("EncargadoContratacionRdP") || funciones.Contains("RdPEncargadoSeleccionNovedad"))
            {
                foreach (var item in Cant_Cubiertos_Princi)
                {
                    if (item.EncargadoContratacion == empleado.Id)
                    {
                        var Cant_Solicitada = Convert.ToInt32(item.NumeroPresonas);
                        var Cant_Contratada = db.DetalleRequerimiento.Where(a => a.Contratado != null && a.RequerimientoId == item.Id).Count();
                        if (Cant_Solicitada == Cant_Contratada)
                        {
                            Total_Cub++;
                        }
                        else
                        {
                            Total_X_Cubrir++;
                        }

                        if (item.EstadoID == 6)
                        {
                            Cant_Cerrados++;
                        }
                        else if (item.EstadoID == 1)
                        {
                            CntRdPSolicitado++;
                        }
                        else if (item.EstadoID == 3)
                        {
                            CntRdPAprobadoDirectorArea++;
                        }
                        else if (item.EstadoID == 4)
                        {
                            CntRdPAprobadoGerencia++;
                        }
                        else if (item.EstadoID == 5)
                        {
                            CntRdPSelección++;
                        }
                        else if (item.EstadoID == 7)
                        {
                            Cant_Anulados++;
                        }
                        Total++;
                        //Datos.Add(item);
                    }
                }
            }
            else if (funciones.Contains("InformeGeneralRdP")) // Jefe de Area
            {
                foreach (var item in Cant_Cubiertos_Princi)
                {
                    if (item.EmpleadoRegistraId == empleado.Id)
                    {
                        var Cant_Solicitada = Convert.ToInt32(item.NumeroPresonas);
                        var Cant_Contratada = db.DetalleRequerimiento.Where(a => a.Contratado != null && a.RequerimientoId == item.Id).Count();
                        if (Cant_Solicitada == Cant_Contratada)
                        {
                            Total_Cub++;
                        }else
                        {
                            Total_X_Cubrir++;
                        }

                        if (item.EstadoID == 6)
                        {
                            Cant_Cerrados++;
                        }
                        else if (item.EstadoID == 1)
                        {
                            CntRdPSolicitado++;
                        }
                        else if (item.EstadoID == 3)
                        {
                            CntRdPAprobadoDirectorArea++;
                        }
                        else if (item.EstadoID == 4)
                        {
                            CntRdPAprobadoGerencia++;
                        }
                        else if (item.EstadoID == 5)
                        {
                            CntRdPSelección++;
                        }
                        else if (item.EstadoID == 7)
                        {
                            Cant_Anulados++;
                        }
                        Total++;
                        //Datos.Add(item);
                    }
                }
            }



            //foreach (var item in Cant_Cubiertos_Princi)
            //{
            //    var Cant_Solicitada = Convert.ToInt32(item.NumeroPresonas);
            //    var Cant_Contratada = db.DetalleRequerimiento.Where(a => a.Contratado != null && a.RequerimientoId == item.Id).Count();
            //    if (Cant_Solicitada == Cant_Contratada)
            //    {
            //        Total_Cub++;
            //    }
            //    else
            //    {
            //        Total_X_Cubrir++;
            //    }

            //};

            Items.Add(new Graficos()
            {
                Cantidad_Solicitados = CntRdPSolicitado,
                Cantidad_Apro_Director = CntRdPAprobadoDirectorArea,
                Cantidad_Apro_Gerencia = CntRdPAprobadoGerencia,
                Cantidad_Seleccion = CntRdPSelección,
                Cantidad_Cerrados = Cant_Cerrados,
                Cantidad_Anulados = Cant_Anulados,
                Total_Circular = Total,
                Cant_Cubiertos = Total_Cub,
                Cant_Por_Cubrir = Total_X_Cubrir

            });
            return Json(Items.ToArray());
        }
        //------------------**************------------------



        //************* Graficos Lineales ******************
        [HttpPost]
        public JsonResult Dashboard_Chart_Total_Cargo()
        {

            List<Graficos> Items2 = new List<Graficos>();
            var Total_Por_Cargo = db.RequerimientosDelPersonal.ToList();

            var Total_Por_Cargo2 = Total_Por_Cargo.GroupBy(e => e.Cargo).OrderByDescending(x => x.Count()).ToList();


            foreach (var cr in Total_Por_Cargo2)
            {
                Items2.Add(new Graficos()
                {
                    Total_Por_Cargo_Nomb = cr.Key,
                    Total_Por_Cargo_Cont = cr.Count(),
                });
            }

            return Json(Items2.ToArray());

        }
        [HttpPost]
        public JsonResult Dashboard_Chart_Total_Area()
        {
            List<Graficos> Results = new List<Graficos>();
            var Total_Por_Area = db.RequerimientosDelPersonal.ToList();
            var Total_Por_Area2 = Total_Por_Area.GroupBy(e => e.Area).OrderByDescending(x => x.Count()).ToList();
            foreach (var cr in Total_Por_Area2)
            {
                Results.Add(new Graficos()
                {
                    Total_Por_Area_Nomb = cr.Key,
                    Total_Por_Area_Cont = cr.Count(),
                });
            }
            return Json(Results.ToArray());
        }
        [HttpPost]
        public JsonResult Dashboard_Chart_Total_Mes()
        {
            List<Graficos> Items2 = new List<Graficos>();
            DateTime FechaAct = DateTime.Now.AddMonths(-11); ;
            var total = 0;
            var Total = db.RequerimientosDelPersonal.Where(a => a.Fecha >= FechaAct).ToList();

            var Total_Por_Mes = Total.OrderByDescending(a => a.Fecha).GroupBy(e => e.Fecha.Month).ToList();
            foreach (var cr in Total_Por_Mes)
            {
                var Total_Por_Cargo = Total.Where(a => a.Fecha.Month == cr.Key).GroupBy(e => e.Cargo).ToList();
                //foreach (var ca in Total_Por_Cargo)
                //{                         
                //        total = ca.Count();
                //}
                var fecmes = Total.Where(a => a.Fecha.Month == cr.Key).FirstOrDefault();
                var mes = Convert.ToDateTime(fecmes.Fecha).ToString("dd/MMMM/yyyy");
                char[] Separador = { '/' };
                string[] FechaMes = mes.Split(Separador);
                Items2.Add(new Graficos()
                {
                    Total_Por_Mes_Nomb = FechaMes[1].ToString(),
                    Total_Por_Mes_Cont = Total_Por_Cargo.Count(),
                });
            }
            return Json(Items2.ToArray());
        }

        [HttpPost]
        public JsonResult Dashboard_Chart_Total_Encargado()
        {

            List<Graficos> Results = new List<Graficos>();
            var Total_Por_Encargado = db.RequerimientosDelPersonal.ToList();

            var Total_Por_Encargado2 = Total_Por_Encargado.GroupBy(e => e.EncargadoContratacion).OrderByDescending(x => x.Count()).ToList();


            foreach (var cr in Total_Por_Encargado2)
            {
                if (cr.Key > 0)
                {
                    var empleado = db.Empleados.Where(a => a.Id == cr.Key).FirstOrDefault();
                    Results.Add(new Graficos()
                    {
                        Total_Por_Enca_Nomb = empleado.Nombres,
                        Total_Por_Enca_Cont = cr.Count(),
                    });
                }
                else
                {
                    Results.Add(new Graficos()
                    {
                        Total_Por_Enca_Nomb = "Sin Encargado Asignado",
                        Total_Por_Enca_Cont = cr.Count(),
                    });
                }
            }

            return Json(Results.ToArray());

        }

        [HttpPost]
        public JsonResult Dashboard_Chart_Total_Motivo()
        {

            List<Graficos> Results = new List<Graficos>();
            var Total_Por_Motivo = db.RequerimientosDelPersonal.ToList();

            var Total_Por_Motivo2 = Total_Por_Motivo.GroupBy(e => e.MtvSolicitudID).OrderByDescending(x => x.Count()).ToList();


            foreach (var cr in Total_Por_Motivo2)
            {
                var IdMovi = db.MtvSolicitud.Where(a => a.Id == cr.Key).FirstOrDefault();
                Results.Add(new Graficos()
                {
                    Total_Por_Mtv_Nomb = IdMovi.Nombre,
                    Total_Por_Mtv_Cont = cr.Count(),
                });
            }

            return Json(Results.ToArray());

        }

        [HttpPost]
        public JsonResult Dashboard_Chart_Total_CargosXArea()
        {

            List<Graficos> Results = new List<Graficos>();
            var Total_Por_Area = db.RequerimientosDelPersonal.ToList();
            var Total_Por_Area2 = Total_Por_Area.GroupBy(e => e.Area).OrderByDescending(x => x.Count()).ToList();

            

            foreach (var cr in Total_Por_Area2)
            {
                var totalCargo = db.RequerimientosDelPersonal.Where(x => x.Area == cr.Key).ToList();
                var Total_CargosXArea = totalCargo.GroupBy(e => e.Cargo).OrderByDescending(x => x.Count()).ToList();
                foreach (var carg in Total_CargosXArea)
                {
                    Results.Add(new Graficos()
                    {
                        Total_Cargos_Por_Area_Nomb = carg.Key + " Solicitado por " + cr.Key,
                        Total_Cargos_Por_Area_Cont = carg.Count(),
                    });
                }
            }

            return Json(Results.ToArray());

        }
        [HttpPost]
        public JsonResult Dashboard_Chart_Total_CargosXMes()
        {

            List<Graficos> Items2 = new List<Graficos>();
            DateTime FechaAct = DateTime.Now.AddMonths(-11);
            
            var Total = db.RequerimientosDelPersonal.Where(a => a.Fecha >= FechaAct).ToList();
            List<RequerimientosDelPersonal> Datos = new List<RequerimientosDelPersonal>();
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            var Id = Session["Empleado"];
            var empleado = db.Empleados.Find(Id);
            var Areas = db.Empleados.Where(x => x.Area != null && x.Area != "" && x.Jefe == empleado.NroEmpleado).Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
            if (funciones.Contains("RdPAprobacionDirectorArea"))
            {
                foreach (var i in Areas)
                {
                    foreach (var item in Total)
                    {
                        if (item.Area == i.Key)
                        {
                            Datos.Add(item);
                        }
                    }
                }
            }
            else if (funciones.Contains("RdPGerencia"))
            {
                foreach (var item in Total)
                {
                    if (item.MtvSolicitudID == 2 || item.MtvSolicitudID == 3 || item.MtvSolicitudID == 7 || item.MtvSolicitudID == 8)
                    {
                        Datos.Add(item);
                    }
                }
            }
            else if (funciones.Contains("RdPGestionHumana"))
            {
                foreach (var item in Total)
                {
                    Datos.Add(item);
                }
            }
            else if (funciones.Contains("EncargadoContratacionRdP") || funciones.Contains("RdPEncargadoSeleccionNovedad"))
            {
                foreach (var item in Total)
                {
                    if (item.EncargadoContratacion == empleado.Id)
                    {
                        Datos.Add(item);
                    }
                }
            }
            else if (funciones.Contains("InformeGeneralRdP")) // Jefe de Area
            {
                foreach (var item in Total)
                {
                    if (item.EmpleadoRegistraId == empleado.Id)
                    {
                        Datos.Add(item);
                    }
                }
            }

            var Total_Por_Mes1 = Datos.GroupBy(e => e.Fecha.Month).ToList();
            foreach (var cr in Total_Por_Mes1)
            {
                var Total_Por_Cargo = Datos.Where(a => a.Fecha.Month == cr.Key).GroupBy(e => e.Cargo).ToList();
                foreach (var ca in Total_Por_Cargo)
                {
                    var Total_Por_Mes = Datos.Where(a => a.Cargo == ca.Key && a.Fecha.Month == cr.Key).GroupBy(e => e.Fecha);
                    var Tot_Por_Mes = Datos.Where(a => a.Cargo == ca.Key && a.Fecha.Month == cr.Key).FirstOrDefault();
                    var fec = Convert.ToDateTime(Tot_Por_Mes.Fecha);
                    var mes = Convert.ToDateTime(Tot_Por_Mes.Fecha).ToString("dd/MMMM/yyyy");
                    char[] Separador = { '/' };
                    string[] FechaMes = mes.Split(Separador);
                    Items2.Add(new Graficos()
                    {
                        Total_Cargos_Por_Mes_Nomb = ca.Key + " Solicitados en " + FechaMes[1].ToString(),
                        Total_Cargos_Por_Mes_Nomb2 = FechaMes[1].ToString(),
                        Total_Cargos_Por_Mes_Cont = Total_Por_Mes.Count(),
                    });

                }
            }
            return Json(Items2.ToArray());
        }
        //------------------**************------------------
        //-----------------------************-----------------------------------------------***************-----------------------------------
    }
}