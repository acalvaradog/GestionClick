using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.IO;
using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System.Globalization;
using System.Data.Entity.SqlServer;
using Microsoft.Ajax.Utilities;
using WebGrease.Css.Extensions;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using System.Web.WebPages;
using System.Collections;
using System.Web.Http.Results;
using System.Diagnostics;

namespace Adm_AutoGestion.Controllers
{
    public class EventosController : Controller
    {
        private AutogestionContext db = new AutogestionContext();
        private EventosRepository _repo;
        
        public EventosController()
        {
        
            _repo = new EventosRepository();
        }

        // GET: Eventos/Create
        public ActionResult Create()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }else if (!Acceso.EsAnonimo && !funciones.Contains("CrearEvento"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            ArrayList lst = new ArrayList();

            var eventos = db.Eventos.Where(e => e.EsEventoPrincipal == true).Select(e => new { Id = e.Id, NombreEvento = e.NombreEvento}).ToArray();

            foreach (var i in eventos)
            {
                lst.Add(i);
            }

            ViewBag.EventosId = lst;

            return View();
        }

        [HttpPost]
        public JsonResult Crear()
        {
            string message = "";

            var NombreEvento = HttpContext.Request.Params["Eventos.NombreEvento"];
            var DirigidoA = HttpContext.Request.Params["Eventos.DirigidoA"];
            var Cupo = HttpContext.Request.Params["Eventos.Cupo"];
            var TipoEvento = HttpContext.Request.Params["Eventos.TipoEvento"];
            var FechaInicio = HttpContext.Request.Params["Eventos.FechaInicio"];
            var FechaFin = HttpContext.Request.Params["Eventos.FechaFin"];
            var HoraInicio = HttpContext.Request.Params["Eventos.HoraInicio"];
            var HoraFin = HttpContext.Request.Params["Eventos.HoraFin"];
            var FechaCierre = HttpContext.Request.Params["Eventos.FechaCierre"];
            var HoraCierre = HttpContext.Request.Params["Eventos.HoraCierre"];
            var RegistroRequerido = HttpContext.Request.Params["Eventos.RegistroRequerido"];
            var LinkEncuestaAsistidos= HttpContext.Request.Params["Eventos.LinkEncuestaAsistidos"];
            var LinkEncuestaNoAsistidos = HttpContext.Request.Params["Eventos.LinkEncuestaNoAsistidos"];
            var Descripcion= HttpContext.Request.Params["Eventos.Descripcion"];
            var ParentescoPermitido = HttpContext.Request.Params["Eventos.ParentescoPermitido"];
            var EdadLimite = HttpContext.Request.Params["Eventos.EdadLimite"];
            var Relacionar = HttpContext.Request.Params["Eventos.Relacionar"];
            var EsEventoPrincipal = HttpContext.Request.Params["Eventos.EsEventoPrincipal"];

            Eventos evento = new Eventos();
            EventosRelacionados eventoR = new EventosRelacionados();

            if (DirigidoA == "Trabajadores" || DirigidoA == "Familiares" || DirigidoA == "Ambos" || DirigidoA == "General")
            {
                evento.DirigidoA = DirigidoA;
            }
            else
            {
                return Json("Se detectó un valor no válido en el campo Dirigido A");
            }

            if (TipoEvento == "1" || TipoEvento == "2" || TipoEvento == "3")
            {
                evento.TipoEvento = TipoEvento;
            }
            else
            {
                return Json("Se detectó un valor no válido en el campo Tipo Evento");
            }

            if (ParentescoPermitido == "" || ParentescoPermitido == "NucleoFamiliar" || ParentescoPermitido == "Todos" || ParentescoPermitido == "Hijos")
            {
                evento.ParentescoPermitido = ParentescoPermitido;
            }
            else
            {
                return Json("Se detectó un valor no válido en el campo Parentesco Permitido");
            }

            evento.NombreEvento = NombreEvento;

            evento.FechaInicio = DateTime.Parse(FechaInicio);

            if (FechaFin != "" && FechaFin != null)
            {
                evento.FechaFin = DateTime.Parse(FechaFin);
            }else { }

            evento.HoraInicio = DateTime.Parse(HoraInicio);

            if (HoraFin != "" && HoraFin != null)
            {
                evento.HoraFin = DateTime.Parse(HoraFin);
            }else { }

            if (FechaCierre != "" && FechaCierre != null)
            {
                evento.FechaCierre = DateTime.Parse(FechaCierre);
            }else { }

            if (HoraCierre != "" && HoraCierre != null)
            {
                evento.HoraCierre = DateTime.Parse(HoraCierre);
            }else { }

            if (Cupo != "" && Cupo != null)
            {
                evento.Cupo = Convert.ToInt32(Cupo);
            }
            else { }

            if (EdadLimite == null || EdadLimite == "")
            {
                EdadLimite = "0";
                evento.EdadLimite = Convert.ToInt32(EdadLimite);
            }
            else
            {
                evento.EdadLimite = Convert.ToInt32(EdadLimite);
            }
            if (TipoEvento == "3")
            {
                evento.EsEventoPrincipal = Convert.ToBoolean(EsEventoPrincipal);
            }
            evento.RegistroRequerido = Convert.ToBoolean(RegistroRequerido);
            evento.LinkEncuestaAsistidos = LinkEncuestaAsistidos;
            evento.LinkEncuestaNoAsistidos = LinkEncuestaNoAsistidos;
            evento.Descripcion = Descripcion;
            if (Relacionar != "")
            {
                eventoR.EventosId = Convert.ToInt32(Relacionar);
            }

            try
            {
                using (var db = new AutogestionContext())
                {
                    var httpPostedFile = Request.Files["Imagen"];

                    if (httpPostedFile != null)
                    {
                        var extension = httpPostedFile.FileName.Split('.');
                        if (extension[1] != "jpg" && extension[1] != "jpeg" && extension[1] != "png" && extension[1] != "JPG" && extension[1] != "JPEG" && extension[1] != "PNG")
                        {
                            message = "El tipo de archivo " + extension[1] + " no es permitido.";
                            return Json(message);
                        }
                        var size = httpPostedFile.ContentLength / 1024;
                        if (size > 3000)
                        {
                            message = "El archivo supera el tamaño permitido de carga (3Mbytes maximo).";
                            return Json(message);
                        }

                        var nombreArchivo = DateTime.Now.ToString("yyyyMMHHmmssffff").ToString() + "-" + httpPostedFile.FileName;

                        var fileSavePath = Path.Combine(Server.MapPath("~/AnexosEventos"), nombreArchivo);
                        evento.Imagen = nombreArchivo;
                        httpPostedFile.SaveAs(fileSavePath);

                        message = _repo.Crear(evento,eventoR);
                        message = "true";
                        return Json(message);
                    }
                    else
                    {
                        message = _repo.Crear(evento, eventoR);
                        message = "true";
                        return Json(message);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(message + ex);
            }
        }

        public ActionResult EventosRegistrados()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EventosRegistrados"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var errMsg = TempData["ErrorMessage"] as string;
            ViewBag.Message = errMsg;
            return View(db.Eventos.ToList());
        }

        public ActionResult VerDetalles(int? id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EventosRegistrados"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            Eventos evento = db.Eventos.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            return PartialView(evento);
            
        }
        [HttpPost]
        public ActionResult VerDetalles(string Id, string Cupo, string FechaCierre, string HoraCierre, string HoraInicio, string HoraFin, string FechaInicio, string FechaFin, string Descripcion)
        {
            try
            {
                int.TryParse(Id, out int id);
                if (Id != "0")
                {
                    if (FechaFin == null)
                    {
                        FechaFin = FechaInicio;
                        _repo.Modificar(Id, Cupo, FechaCierre, HoraCierre, HoraInicio, HoraFin, FechaInicio, FechaFin, Descripcion);
                    }
                    else
                    {
                        _repo.Modificar(Id, Cupo, FechaCierre, HoraCierre, HoraInicio, HoraFin, FechaInicio, FechaFin, Descripcion);
                    }
                }
                return RedirectToAction("EventosRegistrados");
            }
            catch
            {

            }
            return PartialView();
        }
        [HttpPost]
        public JsonResult ActualizarImagen()
        {
            string message = "Debe subir la imagen primero";
            var Id = HttpContext.Request.Params["EventoId"];
            try
            {
                using (var db = new AutogestionContext())
                {
                    var httpPostedFile = Request.Files["Imagen"];

                    if (httpPostedFile != null)
                    {
                        var extension = httpPostedFile.FileName.Split('.');
                        if (extension[1] != "jpg" && extension[1] != "jpeg" && extension[1] != "png" && extension[1] != "JPG" && extension[1] != "JPEG" && extension[1] != "PNG")
                        {
                            message = "El tipo de archivo " + extension[1] + " no es permitido.";
                            return Json(message);
                        }
                        var size = httpPostedFile.ContentLength / 1024;
                        if (size > 3000)
                        {
                            message = "El archivo supera el tamaño permitido de carga (3Mbytes maximo).";
                            return Json(message);
                        }

                        var nombreArchivo = DateTime.Now.ToString("yyyyMMHHmmssffff").ToString() + "-" + httpPostedFile.FileName;

                        var fileSavePath = Path.Combine(Server.MapPath("~/AnexosEventos"), nombreArchivo);
                        httpPostedFile.SaveAs(fileSavePath);

                        message = _repo.SubirImagen(Id,nombreArchivo);
                        message = "true";
                        return Json(message);
                    }
                    return Json(message);
                }
            }
            catch (Exception ex)
            {
                return Json(message + ex);
            }
        }


        public ActionResult LinkAsistidos(int? id)
        {
            Eventos evento = db.Eventos.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            return PartialView(evento);

        }
        [HttpPost]
        public ActionResult LinkAsistidos(string Id, string LinkEncuestaAsistidos)
        {
            try
            {
                int.TryParse(Id, out int id);
                if (Id != "0"){
                    _repo.Modificar1(Id, LinkEncuestaAsistidos);
                }
                return RedirectToAction("EventosRegistrados");
            }
            catch { 
            }
            return PartialView();
        }

        public ActionResult LinkNoAsistidos(int? id)
        {
            Eventos evento = db.Eventos.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            return PartialView(evento);

        }
        [HttpPost]
        public ActionResult LinkNoAsistidos(string Id, string LinkEncuestaNoAsistidos)
        {
            try
            {
                int.TryParse(Id, out int id);
                if (Id != "0")
                {
                    _repo.Modificar2(Id, LinkEncuestaNoAsistidos);
                }
                return RedirectToAction("EventosRegistrados");
            }
            catch
            {
            }
            return PartialView();
        }

        public ActionResult CerrarEvento(int? id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EventosRegistrados"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            Eventos evento = db.Eventos.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            return PartialView(evento);

        }
        [HttpPost]
        public ActionResult CerrarEvento(string Id, string Presupuesto)
        {
            try
            {
                int.TryParse(Id, out int id);
                Eventos evento = db.Eventos.Find(id);
                if (Id != "0")
                {
                    _repo.Modificar3(Id, Presupuesto);
                }
                return RedirectToAction("EventosRegistrados");
            }
            catch
            {
            }
            return PartialView();
        }

        public ActionResult PublicarEvento(int? id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EventosRegistrados"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            Eventos evento = db.Eventos.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            return PartialView(evento);

        }
        [HttpPost]
        public ActionResult PublicarEvento(string Id)
        {
            try
            {
                int.TryParse(Id, out int id);
                Eventos evento = db.Eventos.Find(id);
                if (Id != "0")
                {
                    var v = _repo.CambiarEstadoEvento(Id);
                    TempData["ErrorMessage"] = v;
                }
                return RedirectToAction("EventosRegistrados");
            }
            catch
            {
            }
            return PartialView();
        }

        public ActionResult AnularEvento(int? id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EventosRegistrados"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            Eventos evento = db.Eventos.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            return PartialView(evento);

        }
        [HttpPost]
        public ActionResult AnularEvento(string Id)
        {
            try
            {
                int.TryParse(Id, out int id);
                Eventos evento = db.Eventos.Find(id);
                if (Id != "0")
                {
                    _repo.AnularEvento(Id);
                }
                return RedirectToAction("EventosRegistrados");
            }
            catch
            {

            }
            return PartialView();
        }


        public ActionResult ConfirmarCierre(int? id)
        {
            Eventos evento = db.Eventos.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            return PartialView(evento);

        }
        [HttpPost]
        public ActionResult ConfirmarCierre(string Id)
        {
            try
            {
                int.TryParse(Id, out int id);
                if (Id != "0")
                {
                    _repo.CerrarEvento(Id);
                }
                return RedirectToAction("EventosRegistrados");
            }
            catch
            {

            }
            return PartialView();
        }


        [HttpPost]
        public JsonResult FirmaManual()
        {
            string message = "";
            var id = HttpContext.Request.Params["id"];
            var Id = Convert.ToInt32(id);

            try
            {
                if (Id != 0)
                {
                    message = _repo.FirmarManual(Id);
                    return Json(message);
                }
            }
            catch
            {

            }
            return Json("Error");
        }

        public ActionResult PendienteFirma2(string id, string UserInput)
        {

            if (id == null)
            {
                return HttpNotFound();
            }
            
            if (UserInput == null || UserInput == "")
            {
                ViewBag.Acceder = false;
                return View();
            }

            var Id = Convert.ToInt32(id);

            var evento = db.Eventos.Find(Id);
            var FechaEvento = Convert.ToDateTime(evento.FechaPublicacion).ToString("ddMMyyyy");
            var EventoId = Convert.ToString(Id);
            var Clave = FechaEvento + EventoId;

            if (UserInput == Clave)
            {
                ViewBag.Acceder = true;
                List<DetalleEventos> detalle = db.DetalleEventos.Where(e => e.EventosId == Id && e.FechaFirma == null).ToList();
                List<DetalleEventos> asistentes = new List<DetalleEventos>();
                ViewBag.EventoId = Id;
                ViewBag.ps = Clave;
                if (evento.DirigidoA == "Ambos")
                {
                    ViewBag.EsAmbos = true;
                }
                else if (evento.DirigidoA == "Familiares")
                {
                    ViewBag.EsFamiliar = true;
                }
                else if (evento.DirigidoA == "Trabajadores")
                {
                    ViewBag.EsFamiliar = false;
                }
                foreach (DetalleEventos item in detalle)
                {
                    item.Empleado = db.Empleados.FirstOrDefault(x => x.Id == item.EmpleadoId);
                    var FamiliarId = Convert.ToInt32(item.FamiliarId);
                    if (FamiliarId.ToString() != "0")
                    {
                        item.Familiar = db.Familiar.Find(FamiliarId);
                        if (item.Familiar != null)
                        {
                            asistentes.Add(item);
                        }
                    }
                    else
                    {
                        item.Observaciones = "Inscripción de trabajador";
                        asistentes.Add(item);
                    }
                }
                return View(asistentes);
            }
            else if (UserInput != Clave )
            {
                ViewBag.Acceder = false;
                ViewBag.BadPw = true;
                return View();
            }
            return View();
        }

        [HttpPost]
        public JsonResult FirmaQR()
        {
            string message = "";

            var id = HttpContext.Request.Params["result"];
            var Idevento = HttpContext.Request.Params["idevento"];
            //var Idempleado = HttpContext.Request.Params["idempleado"];

            var idevento = Convert.ToInt32(Idevento);
            //var idempleado = Convert.ToInt32(Idempleado);

            try
            {
                if (id != "" || idevento != 0/* || idempleado != 0*/)
                {
                    message = _repo.FirmarQR(id, idevento);
                    return Json(message);
                }
            }
            catch
            {

            }
            return Json("Error");
        }
        public ActionResult CambioFechaAsistencia(int? id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EventosRegistrados"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            if (id == null)
            {
                return HttpNotFound();
            }

            using (AutogestionContext db = new AutogestionContext())
            {
                var detalle = db.DetalleEventos.Find(id);
                var FamiliarId = Convert.ToInt32(detalle.FamiliarId);
                detalle.Familiar = db.Familiar.Find(FamiliarId);
                detalle.Empleado = db.Empleados.Find(detalle.EmpleadoId);
                var evento = db.Eventos.Find(detalle.EventosId);
                ViewBag.Id = evento.Id;
                ViewBag.NombreEvento = evento.NombreEvento;

                if (detalle.FamiliarId != null)
                {
                    ViewBag.EsFamiliar = true;
                }
                else
                {
                    ViewBag.EsFamiliar = false;
                }

                ArrayList lst = new ArrayList();

                if (evento.EsEventoPrincipal == true)
                {
                    List<EventosRelacionados> ERP = db.EventosRelacionados.Where(e => e.EventosId == detalle.EventosId).ToList();

                    foreach (var item in ERP)
                    {
                        var eventos = db.Eventos.Where(e => e.Id == item.EventosId2).Select(e => new { Id = e.Id, NombreEvento = e.NombreEvento }).ToArray();

                        foreach (var i in eventos)
                        {
                            lst.Add(i);
                        }
                        ViewBag.EventosId = lst;
                    }
                }
                else
                {
                    var BuscarEvento = db.EventosRelacionados.FirstOrDefault(e => e.EventosId2 == detalle.EventosId);
                    var EventoPrincipal = BuscarEvento.EventosId;
                    List<EventosRelacionados> ERP = db.EventosRelacionados.Where(e => e.EventosId == EventoPrincipal).ToList();

                    foreach (var item in ERP)
                    {
                        var eventos = db.Eventos.Where(e => e.Id == item.EventosId2).Select(e => new { Id = e.Id, NombreEvento = e.NombreEvento }).ToArray();

                        foreach (var i in eventos)
                        {
                            if (item.EventosId2 == detalle.EventosId)
                            {
                                continue;
                            }
                            lst.Add(i);
                        }

                    }
                    var eventoP = db.Eventos.Where(e => e.Id == EventoPrincipal).Select(e => new { Id = e.Id, NombreEvento = e.NombreEvento }).ToArray();
                    foreach (var i in eventoP)
                    {
                        lst.Add(i);
                    }
                    ViewBag.EventosId = lst;
                }



                return View(detalle);
            }
        }

        [HttpPost]
        public ActionResult CambioFechaAsistencia(int id,int eventoId)
        {
            try
            {
                if (id != 0)
                {
                    _repo.CambiarFechaAsistencia(id, eventoId);
                }
                return RedirectToAction("CambioFechaAsistencia", id);

            }
            catch
            {
                return HttpNotFound();
            }
        }

        public ActionResult VerAsistentes(int? id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EventosRegistrados"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            using (AutogestionContext db = new AutogestionContext())
            {
                List<DetalleEventos> detalle = db.DetalleEventos.Where(e => e.EventosId == id).ToList();
                List<DetalleEventos> asistentes = new List<DetalleEventos>();
                var Id = Convert.ToInt32(id);
                var evento = db.Eventos.Find(Id);
                if (evento.DirigidoA == "Ambos")
                {
                    ViewBag.EsAmbos = true;
                }
                else if (evento.DirigidoA == "Familiares")
                {
                    ViewBag.EsFamiliar = true;
                }
                else if (evento.DirigidoA == "Trabajadores")
                {
                    ViewBag.EsFamiliar = false;
                }

                foreach (DetalleEventos item in detalle)
                {
                    item.Empleado = db.Empleados.FirstOrDefault(x => x.Id == item.EmpleadoId);
                    //item.Eventos = db.Eventos.FirstOrDefault(x => x.Id == id);
                    var FamiliarId = Convert.ToInt32(item.FamiliarId);
                    if (FamiliarId.ToString() != "0")
                    {
                        item.Familiar = db.Familiar.Find(FamiliarId);
                        if (item.Familiar != null)
                        {
                            asistentes.Add(item);
                        }
                    }
                    else
                    {
                        item.Observaciones = "Inscripción de trabajador";
                        asistentes.Add(item);
                    }

                }

                if (detalle == null)
                {
                    return HttpNotFound();
                }
                return View(asistentes);
            }
        }

        public ActionResult GenerarEnlace(int? id)
        {
            Eventos evento = db.Eventos.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            return PartialView(evento);

        }

        public ActionResult Informe(string NombreEvento, string FechaInicio, string FechaFin, string DirigidoA, string TipoEvento, string Estado)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("InformeEventos"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            using (var db = new AutogestionContext())
            {
                List<Eventos> eventos = new List<Eventos>();
                List<Eventos> evento = new List<Eventos>();
                ViewBag.NombreEvento = db.Eventos.Select(n => new { n.Id, n.NombreEvento }).ToList();

                if (NombreEvento != "" || FechaInicio != "" || FechaFin != "" || DirigidoA != "" || TipoEvento != "" || Estado != "")
                {
                    DateTime Fecha1 = DateTime.Now;
                    DateTime Fecha2 = DateTime.Now;


                    if (!DateTime.TryParse(FechaInicio, out Fecha1))
                    {
                        Fecha1 = new DateTime();
                    }

                    if (!DateTime.TryParse(FechaFin, out Fecha2))
                    {
                        Fecha2 = DateTime.Now;
                    }

                    eventos = db.Eventos.Where(e => DbFunctions.TruncateTime(e.FechaInicio) >= Fecha1 &&
                                    DbFunctions.TruncateTime(e.FechaInicio) <= Fecha2 &&
                                    e.NombreEvento.Contains(NombreEvento) &&
                                    e.DirigidoA.Contains(DirigidoA) &&
                                    e.TipoEvento.Contains(TipoEvento) &&
                                    e.Estado.Contains(Estado)
                                    ).ToList();

                    foreach (var i in eventos)
                    {
                        var Inscritos = db.DetalleEventos.Where(e => e.EventosId == i.Id).Count();
                        var Asistentes = db.DetalleEventos.Where(e => e.EventosId == i.Id && e.FechaFirma != null).Count();
                        i.Asistentes = Asistentes;
                        i.Inscritos = Inscritos;
                        evento.Add(i);
                    }

                    return View(evento);
                }
                return View(eventos);
            }
        }

        public ActionResult Dashboard()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("DashboardEventos"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        private class Indicator
        {
            public int count { get; set; }
            public string type { get; set; }
            public string variable { get; set; }
        }

        [HttpPost]
        public JsonResult GraficaEventosPorMes()
        {
            List<Indicator> Items = new List<Indicator>();

            using (AutogestionContext db = new AutogestionContext())
            {
                var Evento = db.Eventos.Where(e => e.Estado == "Cerrado").GroupBy(e => e.Mes).Select(e => new { Name = e.Key, Count = e.Count() }).ToArray();

                foreach (var item in Evento) {
                    Items.Add(new Indicator()
                    { 
                        count = item.Count, variable = item.Name.ToString()
                    });
                }
            }

            return Json(Items.ToArray());
        }

        [HttpPost]
        public JsonResult GraficaIndicadorAsistentes()
        {

            List<Eventos> t = db.Eventos.Where(x=> x.Estado != "Anulado").ToList();

            using (AutogestionContext db = new AutogestionContext())
            {
                foreach (var item in t)
                {
                    item.Inscritos = db.DetalleEventos.Where(e => e.EventosId == item.Id).Count();
                    item.Asistentes = db.DetalleEventos.Where(e => e.FechaFirma != null && e.EventosId == item.Id).Count();
                    if (item.Inscritos >= 1)
                    {
                        item.Indicador = item.Asistentes * 100 / item.Inscritos;
                    }
                    else
                    {
                        item.Indicador = 0;
                    }
                }
            }
            return Json(t.ToArray());
        }

        public ActionResult EnvioCorreo(int? id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EventosRegistrados"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            if (id == null)
            {
                return HttpNotFound();
            }

            ViewBag.Id = id;

            return PartialView();
        }

        [HttpPost]
        public JsonResult EnvioCorreo()
                
        {
            string message = "";
            try
            {
                var Id = HttpContext.Request.Params["id"];
                var Asunto = HttpContext.Request.Params["Asunto"];
                var Texto = HttpContext.Request.Params["Texto"];

                var id = Convert.ToInt32(Id);
                message = _repo.EnvioCorreoGeneral(id, Asunto, Texto);
                message = "true";
                return Json(message);
            }
            catch
            {
            }
            return Json(message);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [HttpGet]
        public ActionResult EnvioEncuestasAsitestesNoAsistentes(int? IdEvento , string Parametro)
        {
            var message = "";
            try 
            {
                var Result = "";
                var Result2 = "";
                
                if (IdEvento!=null) 
                {
                    var evento = db.Eventos.FirstOrDefault(x => x.Id == IdEvento);
                    if (evento.EncuestaEnviada ==true) 
                    {
                        message += "La encuesta ya ha sido enviada anteriormente";
                    }
                    else 
                    {
                                     
                    if (evento.LinkEncuestaAsistidos == "")
                    {
                        Result = "NoLink";
                    }
                    if (evento.LinkEncuestaNoAsistidos == "")
                    {
                        Result2 = "NoLink";
                    }
                    int Id =Convert.ToInt32(IdEvento);
                        if (Result == "" ) 
                        {
                                if (Parametro=="Asistidos") 
                                {
                                    Result = _repo.EnvioCorreosEncuestasAsistentes(Id);

                                }      
                        }
                        if (Result2 == "") 
                        {
                            if (Parametro == "NOAsistidos")
                            {
                                Result2 = _repo.EnvioCorreosEncuestasNoAsistentes(Id);
                            }
                        }
                    if (Result == "True" && Result2 == "True")
                    {
                        
                        evento.EncuestaEnviada = true;
                        db.SaveChanges();
                        message+= "Se enviaron los correos correctamente. ";
                        Session["message"] = message;
                    }
                    if (Result == "True" && Parametro == "Asistidos")
                    {
                        message += "La encuesta a los Asistentes se envió correctamente.";
                    }
                    if (Result2 == "True" && Parametro == "NOAsistidos")
                    {
                        message += "La encuesta a los No Asistentes se envió correctamente.";
                    }
                    if (Result == "NoLink" && Parametro == "Asistidos")
                    {
                        message += "No se envió la Encuesta a los Asistentes porque el evento no tiene un enlace establecido. ";
                    }
                    if (Result2 == "NoLink" && Parametro == "NOAsistidos")
                    {
                        message += "No se envió la Encuesta a los No Asistentes porque el evento no tiene un enlace establecido. ";
                    }
                    }
                }
                

            }
            catch (Exception ex) 
            {
                message = " " + ex;
                Session["message"] = message;
                return RedirectToAction("EventosRegistrados");
            }

            Session["message"] = message;
            return RedirectToAction("EventosRegistrados");

        }
    }
}
