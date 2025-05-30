using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using SAP.Middleware.Connector;
using System.Web.Script.Serialization;
using System.Net;
using System.Text;
using System.IO;
using OfficeOpenXml;
using Adm_AutoGestion.Models.EvaDesempeno;
using Adm_AutoGestion.Migrations;
using System.Globalization;
using System.Web.Services.Description;


namespace Adm_AutoGestion.Controllers
{



    public class ControlVacacionesJefe
    {
        public int Id { get; set; }
        public string Nombres { get; set; }
        public string Documento { get; set; }
        public string NroEmpleado { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Cargo { get; set; }
        public DateTime? FechaIngreso { get; set; }

    }

    public class CalendarioVacaciones
    {

        public string Nombres { get; set; }
        public DateTime FechaInicial { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; }

    }


    public class VacacionesController : Controller
    {


        private VacacionesRepository _repo;
        private Api.VacacionesController _ApiVac;

        public VacacionesController()
        {
            _repo = new VacacionesRepository();
            _ApiVac = new Api.VacacionesController();
        }




        //
        // GET: /Vacaciones/

        public ActionResult Index()
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("Vacaciones"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var model = _repo.ObtenerTodos();
            return View(model);
        }

        //
        // GET: /Vacaciones/Details/5

        public ActionResult Details(int Id)
        {
            using (var db = new AutogestionContext())
            {
                Empleado empleado = db.Empleados.Find(Id);
                if (empleado == null)
                {
                    return HttpNotFound();
                }
                return View(empleado);
            }
        }

        //
        // GET: /Vacaciones/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Vacaciones/Create

        [HttpPost]
        public ActionResult Create(Vacaciones model)
        {
            string message = "";
            int IdUsuarioM = 0;
            var Datos2 = new Vacaciones();
            try
            {


                using (var db = new AutogestionContext())
                {
                    string Nro = string.Format("{0}", model.EmpleadoId);
                    Empleado Codigo = db.Empleados.FirstOrDefault(e => e.NroEmpleado == Nro);
                    model.EmpleadoId = Codigo.Id;
                    model.Empresa = Codigo.Empresa;
                    string modifica = String.Format("{0}", Session["Empleado"]);
                    Int32.TryParse(modifica, out IdUsuarioM);

                    List<Vacaciones> Datos = new List<Vacaciones>();
                    Datos = db.Vacaciones.OrderByDescending(e => e.Fecha).ToList();
                    Datos2 = Datos.FirstOrDefault(e => e.EmpleadoId == model.EmpleadoId);
                    HabilitarVacaciones autoriza = db.HabilitarVacaciones.FirstOrDefault(e => e.EmpleadoId == model.EmpleadoId);

                    if (Datos2.EstadoId != 5 && Datos2.EstadoId != 12)
                    {

                        if (model.FechaFin > model.FechaInicial)
                        {


                            if (autoriza.Pagadas == "NO" && model.VacacionesPagadas == "SI")
                            {

                                message = "El empleado no tiene autorización para solicitar vacaciones Pagas.";
                            }
                            else
                            {

                                if (autoriza.Anticipadas == "NO" && model.VacacionesAdelantadas == "SI")
                                {
                                    message = "El empleado no tiene autorización para solicitar vacaciones Adelantadas.";
                                }
                                else
                                {
                                    // TODO: Add insert logic here
                                    if (ModelState.IsValid)
                                    {
                                        model.Empresa = Codigo.Empresa;
                                        model.IdModifica = IdUsuarioM;
                                        model.EmpleadoId = Codigo.Id;
                                        _repo.Crear(model);
                                        return RedirectToAction("Index");
                                    }
                                }
                            }


                        }
                        else
                        {
                            message = "La fecha Final debe ser mayor a la fecha Inicial.";
                        }

                        message = "No es posible realizar una nueva solicitud, debido a que ya tiene una en proceso.";

                    }

                }
            }
            catch
            {

            }

            Session["message"] = message;
            return View();
        }


        public ActionResult CreateXJefe()
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("CreateXJefe"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            string EmpleadoId = String.Format("{0}", Session["Empleado"]);
            Empleado datos = new Empleado();
            int Id = 0;
            Int32.TryParse(EmpleadoId, out Id);

            Vacaciones model = new Vacaciones();
            using (var db = new AutogestionContext())
            {
                datos = db.Empleados.Find(Id);
                model.ListadoEmpleadosJefe = db.Empleados.Where(e => e.Jefe == datos.NroEmpleado).ToList();

            }

            return View(model);
        }

        //
        // POST: /Vacaciones/Create

        [HttpPost]
        public JsonResult Crear()
        {


            string message = "";
            int IdUsuarioM = 0;

            var Datos2 = new Vacaciones();
            var EmpleadoId = HttpContext.Request.Params["Vacaciones.EmpleadoId"];
            var FechaInicial = HttpContext.Request.Params["Vacaciones.FechaInicial"];
            var FechaFinal = HttpContext.Request.Params["Vacaciones.FechaFin"];
            var CantDiasSolicitados = HttpContext.Request.Params["Vacaciones.CantDiasSolicitados"];
            var CantDiasPendientes = HttpContext.Request.Params["Vacaciones.CantDiasPendientes"];
            var VacacionesPagas = HttpContext.Request.Params["Vacaciones.VacacionesPagadas"];
            var VacacionesAdelanta = HttpContext.Request.Params["Vacaciones.VacacionesAdelantadas"];
            var VacacionesMayor6 = HttpContext.Request.Params["Vacaciones.VacacionesDiasMayor6"];
            var Cantdias = HttpContext.Request.Params["Vacaciones.Cantdias"];


            Vacaciones Datosvacaciones = new Vacaciones();
            Datosvacaciones.EmpleadoId = Convert.ToInt16(EmpleadoId);
            Datosvacaciones.FechaInicial = DateTime.Parse(FechaInicial);
            Datosvacaciones.FechaFin = DateTime.Parse(FechaFinal); ;
            Datosvacaciones.CantDiasSolicitados = CantDiasSolicitados;
            Datosvacaciones.CantDiasPendientes = CantDiasPendientes;
            Datosvacaciones.Fecha = DateTime.Now;
            Datosvacaciones.EstadoId = 1;
            Datosvacaciones.VacacionesPagadas = VacacionesPagas;
            Datosvacaciones.VacacionesAdelantadas = VacacionesAdelanta;
            Datosvacaciones.VacacionesDiasMayor6 = VacacionesMayor6;

            try
            {


                using (var db = new AutogestionContext())
                {
                    //string Nro = string.Format("{0}", model.EmpleadoId);
                    //Empleado Codigo = db.Empleados.FirstOrDefault(e => e.NroEmpleado == Nro);
                    //model.EmpleadoId = Codigo.Id;
                    string modifica = String.Format("{0}", Session["Empleado"]);
                    Int32.TryParse(modifica, out IdUsuarioM);
                    int DSol = 0;
                    int DPen = 0;
                    Int32.TryParse(Datosvacaciones.CantDiasSolicitados, out DSol);
                    Int32.TryParse(Cantdias, out DPen);

                    List<Vacaciones> Datos = new List<Vacaciones>();
                    Datos = db.Vacaciones.OrderByDescending(e => e.Fecha).ToList();
                    Datos2 = Datos.FirstOrDefault(e => e.EmpleadoId == Datosvacaciones.EmpleadoId);
                    HabilitarVacaciones Autoriza = db.HabilitarVacaciones.FirstOrDefault(e => e.EmpleadoId == Datosvacaciones.EmpleadoId);

                    if (Datos2 != null)
                    {
                        if (Datos2.EstadoId != 5 && Datos2.EstadoId != 4)
                        {
                            message = "No es posible realizar una nueva solicitud, debido a que ya tiene una en proceso.";
                            return Json(message);

                        }
                    }



                    if (Autoriza == null)
                    {

                        Datosvacaciones.VacacionesPagadas = "NO";
                        Datosvacaciones.VacacionesAdelantadas = "NO";
                        Datosvacaciones.VacacionesDiasMayor6 = "NO";

                        if (DSol > DPen)
                        {
                            message = "Los dias solicitados exceden de la cantidad de dias que tiene pendientes por disfrutar. Favor solicitar autorización.";
                            return Json(message);
                        }
                    }

                    //var httpPostedFile = Request.Files["Adjunto"];

                    //if (httpPostedFile != null)
                    //{

                    //    var extension = httpPostedFile.FileName.Split('.');

                    //    //if (extension[1] != "jpg" && extension[1] != "jpeg" && extension[1] != "png" && extension[1] != "doc" && extension[1] != "docx" && extension[1] != "pdf" && extension[1] != "odt" && extension[1] != "ods")
                    //    if (extension[1] != "doc" && extension[1] != "docx" && extension[1] != "pdf" && extension[1] != "odt" && extension[1] != "ods")
                    //    {
                    //        message = "El tipo de archivo " + extension[1] + " no es permitido.";
                    //        return Json(message);
                    //    }

                    //    var size = httpPostedFile.ContentLength / 1024;
                    //    if (size > 3000)
                    //    {
                    //        message = "El archivo supera el tamaño permitido de carga.";
                    //        return Json(message);
                    //    }

                    //    // Validate the uploaded image(optional)
                    //    DateTime date1 = DateTime.Now;
                    //    var nombrearchivo = date1.ToString("yyyyMMddHHmmssffff").ToString() + "-" + httpPostedFile.FileName;

                    //    var fileSavePath = Path.Combine(Server.MapPath("~/AnexosVacaciones"), nombrearchivo);
                    //    Datosvacaciones.Adjunto = nombrearchivo;
                    //    httpPostedFile.SaveAs(fileSavePath);
                    //}
                    // TODO: Add insert logic here
                    if (ModelState.IsValid)
                    {
                        Datosvacaciones.IdModifica = IdUsuarioM;
                        message = _repo.Crear(Datosvacaciones);
                        message = "true";
                        return Json(message);
                    }
                }
            }
            catch
            {

            }


            return Json(message);
        }



        //
        // GET: /Vacaciones/Edit/5


        public ActionResult AprobacionSuperior(string Id, string Area, string Sociedad, string all)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            string EmpresaSesion = Convert.ToString(Session["Empresa"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("AprobacionSuperior"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            List<SelectListItem> lst = new List<SelectListItem>();
            List<SelectListItem> lst2 = new List<SelectListItem>();
            List<Vacaciones> model = new List<Vacaciones>();
            
            Session["filtrosVac"] = String.Format("{0},{1}", Area, Sociedad);

            using (AutogestionContext db = new AutogestionContext())
            {

                var empleadoid = Session["Empleado"];
                var empleado = db.Empleados.Find(empleadoid);

                //var lista = db.PersonalActivo.Where(x => x.Superior == empleado.NroEmpleado || x.Jefe == empleado.NroEmpleado || x.Director == empleado.NroEmpleado).Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                var lista = db.Empleados.Where(x => x.Jefe == empleado.NroEmpleado && x.Activo == "SI" && empleado.Empresa == EmpresaSesion).Select(x => new { x.AreaDescripcion }).GroupBy(b => b.AreaDescripcion).ToList();
                foreach (var x in lista)
                {
                    lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }

                ViewBag.Opciones = lst;

                //var lista2 = db.PersonalActivo.Where(x => x.Jefe == empleado.NroEmpleado || x.Superior == empleado.NroEmpleado || x.Director == empleado.NroEmpleado).Select(x => new { x.Empresa }).GroupBy(b => b.Empresa).ToList();
                var lista2 = db.Empleados.Where(x => x.Jefe == empleado.NroEmpleado && x.Activo == "SI").Select(x => new { x.Empresa }).GroupBy(b => b.Empresa).ToList();
                foreach (var x in lista2)
                {
                    lst2.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }


                //ViewBag.Empresas = lst2;

                ViewBag.Empresas =   db.Sociedad.Where(x => x.Codigo == EmpresaSesion).ToList();


                int id = -1;

                Int32.TryParse(Id, out id);
                string EmpleadoId = String.Format("{0}", Session["Empleado"]);

                if (all == "SI")
                {
                    Int32.TryParse(EmpleadoId, out id);
                    Empleado datos = new Empleado();
                    datos = db.Empleados.Find(id);
                    model = db.Vacaciones.Where(e => e.EstadoId == 1 &&  e.Empleado.Jefe == datos.NroEmpleado).ToList();
                    if (id > 0)
                    {
                        string opcion = "Aprobar";
                        model = _repo.ObtenerTodos2(opcion, EmpleadoId, Area, EmpresaSesion);
                        ViewBag.Vacaciones = model.FirstOrDefault(e => e.Id == id);
                    }

                    if (ViewBag.Vacaciones == null)
                    {
                        ViewBag.Vacaciones = new Models.Vacaciones();
                        ViewBag.Vacaciones.Empleado = new Empleado();
                    }
                }
                else
                {
                    if (Area != null || Sociedad != null)
                    {
                        model = _repo.ObtenerTodos4(EmpleadoId, Area, Sociedad);
                    }
                    if (id > 0)
                    {
                        string opcion = "Aprobar";
                        model = _repo.ObtenerTodos2(opcion, EmpleadoId, Area, Sociedad);
                        ViewBag.Vacaciones = model.FirstOrDefault(e => e.Id == id);
                    }

                    if (ViewBag.Vacaciones == null)
                    {
                        ViewBag.Vacaciones = new Models.Vacaciones();
                        ViewBag.Vacaciones.Empleado = new Empleado();
                    }

                }


            }



            return View(model);
        }

        public ActionResult BusquedaAprobacion(string Area, string Sociedad)
        {
            return View();
        }



        public ActionResult AprobacionSuperior1(string Id, string Empleado, string Estado, string Observacion, string FechaInicial, string FechaFin, string Cds, string Cdp, string ObservacionTra)
        {
            int IdUsuarioM = 0;
            string message = null;

            try
            {
                int id = 0;
                Int32.TryParse(Id, out id);
                int empleadoid = 0;
                Int32.TryParse(Empleado, out empleadoid);
                string modifica = String.Format("{0}", Session["Empleado"]);
                Int32.TryParse(modifica, out IdUsuarioM);
                string opcion = "JEFE";

                if (Id != "0")
                {
                    // TODO: Add update logic here
                    _repo.Modificar(Observacion, Id, Empleado, Estado, IdUsuarioM, FechaInicial, FechaFin, Cds, Cdp, ObservacionTra,opcion);
                    if (Estado == "5")
                    {
                        using (var db = new AutogestionContext())
                        {
                            Empleado empleado = new Empleado();
                            empleado = db.Empleados.FirstOrDefault(e => e.Id == empleadoid);
                            var jefe = db.Empleados.FirstOrDefault(s => s.NroEmpleado == empleado.Jefe);
                            NotificacionCorreo("Rechazo", Empleado, Observacion, empleado.Nombres, empleado.Correo, jefe.Correo);
                        }
                    }
                }
                else
                {
                    message = "Antes de Guardar, debe seleccionar un registro de la tabla.";
                }

                string filtros = string.Format("{0}", Session["filtrosVac"]);

                string[] datos = filtros.Split(',');
                Session.Remove("filtrosVac");
                Session["message"] = message;
                return RedirectToAction("AprobacionSuperior", "Vacaciones", new { Area = datos[0], Sociedad = datos[1] });

                //Session["message"] = message;
                //return RedirectToAction("AprobacionSuperior");
            }

            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
                Session["message"] = message;
                return View();
            }



        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Vacaciones/Edit/5

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
        // GET: /Vacaciones/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Vacaciones/Delete/5

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



        public ActionResult Confirmacion(string Id, string FechaIni, string FechaFin, string Empresa)
        {

            List<Vacaciones> model = new List<Vacaciones>();

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("Confirmacion"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }


            int id = -1;
            int EmpId = 0;
            Int32.TryParse(Id, out id);
            string EmpleadoId = String.Format("{0}", Session["Empleado"]);
            Int32.TryParse(EmpleadoId, out EmpId);

            using (var db = new AutogestionContext())
            {
                var empleado = db.Empleados.FirstOrDefault(e => e.Id == EmpId);
                ViewBag.Empresa = empleado.Empresa;
            }

            if (FechaIni != null && FechaFin != null)
            {
                if (FechaIni != "" && FechaFin != "")
                {
                    model = _repo.ObtenerTodos5(EmpleadoId, FechaIni, FechaFin, Empresa);

                }
            }



            Session["filtrosV"] = String.Format("{0},{1},{2}", FechaIni, FechaFin, Empresa);



            return View(model);
        }


        public ActionResult Confirmacion1(string Observacion, string Id, string NroEmpleado, string FechaInicial, string FechaFin, string Cds, string Cdp, string Empleado, string Estado, string ObservacionTra)
        {

            string message = null;
            int IdUsuarioM = 0;

            DataTable Datos = new DataTable();

            InMemoryDestinationConfiguration DestinacionConfiguracion = new InMemoryDestinationConfiguration();

            String contraseña = Properties.Settings.Default.Contraseña.ToString();
            var encodedTextBytes = Convert.FromBase64String(contraseña);

            contraseña = Encoding.UTF8.GetString(encodedTextBytes);

            RfcConfigParameters config = new RfcConfigParameters();

            try
            {
                int id = 0;
                Int32.TryParse(Id, out id);
                string modifica = String.Format("{0}", Session["Empleado"]);
                Int32.TryParse(modifica, out IdUsuarioM);



                using (var db = new AutogestionContext())
                {

                    if (Id != "0")
                    {

                        Vacaciones vacaciones = db.Vacaciones.Find(id);

                        if (vacaciones.VacacionesAdelantadas == "NO")
                        {

                            //if (Estado == "4")
                            //{


                            //    config.Add(RfcConfigParameters.Name, "SAP");
                            //    config.Add(RfcConfigParameters.AppServerHost, Properties.Settings.Default.Servidor.ToString());
                            //    config.Add(RfcConfigParameters.SystemNumber, Properties.Settings.Default.Id.ToString());
                            //    config.Add(RfcConfigParameters.User, Properties.Settings.Default.Usuario.ToString());
                            //    config.Add(RfcConfigParameters.Password, contraseña);
                            //    config.Add(RfcConfigParameters.Client, Properties.Settings.Default.Mandante.ToString());
                            //    config.Add(RfcConfigParameters.Language, "ES");
                            //    config.Add(RfcConfigParameters.SAPRouter, Properties.Settings.Default.Saprouter.ToString());
                            //    config.Add(RfcConfigParameters.LogonGroup, "Foscal");

                            //    RfcDestinationManager.RegisterDestinationConfiguration(DestinacionConfiguracion);
                            //    DestinacionConfiguracion.AddOrEditDestination(config);
                            //    RfcDestination destination = RfcDestinationManager.GetDestination("SAP");

                            //    RfcRepository repository = destination.Repository;
                            //    IRfcFunction function = repository.CreateFunction("ZMF_INFO_VACACIONES");
                            //    //PARAMETROS IMPORT

                            //    DateTime fecha1 = DateTime.Parse(FechaInicial);
                            //    DateTime fecha2 = DateTime.Parse(FechaFin);
                            //    function.SetValue("I_PERNR", NroEmpleado);
                            //    function.SetValue("I_FECHAINI", fecha1);
                            //    function.SetValue("I_FECHAFIN", fecha2);

                            //    function.Invoke(destination);
                            //    //OBTENER RESPUESTA 

                            //    object per = function.GetValue("E_RESPUESTA");


                            //    if (per == "1")
                            //    {
                            //        message = "Se a grabado con exito. No se realiza registro en SAP debido a que es una solicitud de vacaciones adelantadas.";
                            //    }

                            //    if (per == "2")
                            //    {
                            //        message = "Empleado inactivo.";
                            //    }

                            //    if (per == "3")
                            //    {
                            //        message = "Empleado no Existe.";
                            //    }

                            //    if (per == "4")
                            //    {
                            //        message = "Fecha Fin menor.";
                            //    }


                            //}


                        }
                        else
                        {


                        }
                        string opcion = "GESTIONH";
                        _repo.Modificar(Observacion, Id, Empleado, Estado, IdUsuarioM, FechaInicial, FechaFin, Cds, Cdp, ObservacionTra, opcion);
                        Empleado empleado = new Empleado();
                        empleado = db.Empleados.FirstOrDefault(e => e.NroEmpleado == NroEmpleado);
                        var jefe = db.Empleados.FirstOrDefault(s => s.NroEmpleado == empleado.Jefe);
                        if (Estado == "5")
                        {
                            NotificacionCorreo("Rechazo", NroEmpleado, Observacion, empleado.Nombres, empleado.Correo, jefe.Correo);
                        }
                        if (Estado == "11")
                        {
                            var correojefeGH = db.Configuraciones.FirstOrDefault(e => e.Parametro == "CORREOJEFEGH");
                            var Obser = "";
                            NotificacionCorreo("Aprobar", NroEmpleado, Obser, empleado.Nombres, correojefeGH.Valor, correojefeGH.Valor);
                        }

                    }
                    else
                    {
                        message = "Antes de Guardar, debe seleccionar un registro de la tabla.";
                    }

                    // TODO: Add update logic here

                    Session["message"] = message;

                    string filtros = string.Format("{0}", Session["filtrosV"]);

                    string[] datos = filtros.Split(',');
                    Session.Remove("filtrosV");
                    return RedirectToAction("Confirmacion", "Vacaciones", new { FechaIni = datos[0], FechaFin = datos[1], Empresa = datos[2] });


                }

            }
            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
                Session["message"] = message;
                return View("Confirmacion");
            }
        }

        public ActionResult Confirmacion2(string Id)
        {

            Vacaciones model = new Vacaciones();
            int id = -1;
            Int32.TryParse(Id, out id);
            string EmpleadoId = String.Format("{0}", Session["Empleado"]);

            using (var db = new AutogestionContext())
            {

                model = db.Vacaciones.FirstOrDefault(e => e.Id == id);
                model.Empleado = db.Empleados.FirstOrDefault(e => e.Id == model.EmpleadoId);
                ViewBag.Vacaciones = model;

                //if (id > 0)
                //{
                //    string opcion = "Confirmar";
                //    model = _repo.ObtenerTodos2(opcion, EmpleadoId);
                //    ViewBag.Vacaciones = model.FirstOrDefault(e => e.Id == id);
                //}

                //if (ViewBag.Vacaciones == null)
                //{
                //    ViewBag.Vacaciones = new Models.Vacaciones();
                //    ViewBag.Vacaciones.Empleado = new Empleado();
                //}


                return PartialView(model);
            }
        }


        public ActionResult AprobarGestionH(string Id)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            string Area = "";
            string Sociedad = "";
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("AprobarGestionH"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            int id = -1;
            string opcion = "Gestion";
            Int32.TryParse(Id, out id);
            string EmpleadoId = String.Format("{0}", Session["Empleado"]);
            var model = _repo.ObtenerTodos2(opcion, EmpleadoId, Area, Sociedad);


            if (id > 0)
            {
                ViewBag.Vacaciones = model.FirstOrDefault(e => e.Id == id);
            }

            if (ViewBag.Vacaciones == null)
            {
                ViewBag.Vacaciones = new Models.Vacaciones();
                ViewBag.Vacaciones.Empleado = new Empleado();
            }

            return View(model);
        }

        public ActionResult AprobarGestionH1(string Observacion, string Id, string Empleado, string Estado, string FechaInicial, string FechaFin, string Cds, string Cdp, string ObservacionTra)
        {
            string message = null;
            int IdUsuarioM = 0;

            try
            {
                int id = 0;
                Int32.TryParse(Id, out id);
                int empleadoid = 0;
                Int32.TryParse(Empleado, out empleadoid);
                string modifica = String.Format("{0}", Session["Empleado"]);
                Int32.TryParse(modifica, out IdUsuarioM);
                string opcion = "";

                if (Id != "0")
                {
                    // TODO: Add update logic here
                    _repo.Modificar(Observacion, Id, Empleado, Estado, IdUsuarioM, FechaInicial, FechaFin, Cds, Cdp, ObservacionTra, opcion);
                    if (Estado == "5")
                    {
                        using (var db = new AutogestionContext())
                        {
                            Empleado empleado = new Empleado();
                            empleado = db.Empleados.FirstOrDefault(e => e.Id == empleadoid);
                            var jefe = db.Empleados.FirstOrDefault(s => s.NroEmpleado == empleado.Jefe);
                            NotificacionCorreo("Rechazo", Empleado, Observacion, empleado.Nombres, empleado.Correo, jefe.Correo);
                        }
                    }
                }
                else
                {
                    message = "Antes de Guardar, debe seleccionar un registro de la tabla.";
                }


                Session["message"] = message;
                return RedirectToAction("AprobarGestionH");
            }
            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
                Session["message"] = message;
                return View("AprobarGestionH");
            }
        }

        public ActionResult MostrarImagen(string archivo)
        {
            try
            {

                ViewBag.imagen = string.Format("~/AnexosVacaciones/{0}", archivo);
                // TODO: Add update logic here
                //_repo.Modificar(Id, Empleado, Estado);
                return View("MostrarImagen");
            }
            catch
            {
                return View("AprobarGestionH");
            }
        }


        public FileResult Download(string archivo)
        {

            //byte[] fileBytes = System.IO.File.ReadAllBytes(@"C:\Users\mayra\Desktop\Adm_AutoGestion\Adm_AutoGestion\AnexosVacaciones\" + archivo);
            var fileBytes = Server.MapPath(@"..\AnexosVacaciones\" + archivo);
            string fileName = archivo;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }


        public ActionResult Informe(string FechaIni, string FechaFin, string Empleado, string Estado, string Empresa, string Codigo)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("VacacionesInforme"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }


            List<Vacaciones> Items = new List<Vacaciones>();

            using (var db = new AutogestionContext())
            {



                var lista = db.Sociedad.ToList();
                ViewBag.Sociedad = lista;


                DateTime Fecha1 = DateTime.Now;
                DateTime Fecha2 = DateTime.Now;
                int estado = 0;
                int.TryParse(Estado, out estado);

                if (!DateTime.TryParse(FechaIni, out Fecha1))
                {
                    Fecha1 = new DateTime();
                }

                if (!DateTime.TryParse(FechaFin, out Fecha2))
                {
                    Fecha2 = DateTime.Now;
                }

                if (estado == 0)
                {
                    Items = db.Vacaciones.Where(e => e.Empleado.Nombres.Contains(Empleado) &&
                                               e.Empleado.Empresa.Contains(Empresa)&&
                                               e.Empleado.NroEmpleado.Contains(Codigo) &&
                                               DbFunctions.TruncateTime(e.Fecha) >= Fecha1 &&
                                               DbFunctions.TruncateTime(e.Fecha) <= Fecha2).ToList();

                }
                if (estado != 0)
                {
                    Items = db.Vacaciones.Where(e => e.Empleado.Nombres.Contains(Empleado) &&
                                                e.Empleado.Empresa.Contains(Empresa) &&
                                                e.Empleado.NroEmpleado.Contains(Codigo) &&
                                               DbFunctions.TruncateTime(e.Fecha) >= Fecha1 &&
                                               DbFunctions.TruncateTime(e.Fecha) <= Fecha2 &&
                                               e.EstadoId == estado).ToList();
                }

                foreach (Vacaciones Item in Items)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.EstadoVacaciones = db.EstadoVacaciones.FirstOrDefault(x => x.Id == Item.EstadoId);
                    Item.personal = db.PersonalActivo.FirstOrDefault(s => s.CodigoEmpleado == Item.Empleado.NroEmpleado);
                    Item.HistorialVacaciones = db.HistorialVacaciones.Where(e => e.VacacionesId == Item.Id).OrderByDescending(x => x.Fecha).FirstOrDefault();
                    Item.Empleado2 = db.Empleados.FirstOrDefault(e => e.Id == Item.HistorialVacaciones.UsuarioModifica);


                }

            }
            return View(Items);
        }



        public ActionResult Informe2(string FechaIni, string FechaFin, string Empleado, string Estado, string Empresa)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("VacacionesInforme2"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }



            List<Vacaciones> Items = new List<Vacaciones>();

            using (var db = new AutogestionContext())
            {

                var list = db.Sociedad.ToList();
                ViewBag.Sociedad = list;

                int Usuario = 0;
                string usuariosesion = String.Format("{0}", Session["Empleado"]);
                Int32.TryParse(usuariosesion, out Usuario);


                var empleado = db.Empleados.FirstOrDefault(e => e.Id == Usuario);
                List<Empleado> lista = db.Empleados.Where(s => s.Jefe == empleado.NroEmpleado && s.Activo == "SI").ToList();
                ViewBag.Empleados = lista;

                DateTime Fecha1 = DateTime.Now;
                DateTime Fecha2 = DateTime.Now;
                int estado = 0;
                int.TryParse(Estado, out estado);

                if (!DateTime.TryParse(FechaIni, out Fecha1))
                {
                    Fecha1 = new DateTime();
                }

                if (!DateTime.TryParse(FechaFin, out Fecha2))
                {
                    //Fecha1 = DateTime.Now;
                    Fecha2 = DateTime.Now;
                }

                if (estado == 0)
                {
                    Items = db.Vacaciones.Where(e => e.Empleado.Nombres.Contains(Empleado) &&
                                               e.Empleado.Empresa.Contains(Empresa) &&
                                               DbFunctions.TruncateTime(e.Fecha) >= Fecha1 &&
                                               DbFunctions.TruncateTime(e.Fecha) <= Fecha2 &&
                                               e.Empleado.Activo == "SI" &&
                                               e.Empleado.Jefe == empleado.NroEmpleado).ToList();

                }
                if (estado != 0)
                {
                    Items = db.Vacaciones.Where(e => e.Empleado.Nombres.Contains(Empleado) &&
                                                e.Empleado.Empresa.Contains(Empresa) &&
                                               DbFunctions.TruncateTime(e.Fecha) >= Fecha1 &&
                                               DbFunctions.TruncateTime(e.Fecha) <= Fecha2 &&
                                               e.Empleado.Jefe == empleado.NroEmpleado &&
                                               e.Empleado.Activo == "SI" &&
                                               e.EstadoId == estado).ToList();
                }

                foreach (Vacaciones Item in Items)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.EstadoVacaciones = db.EstadoVacaciones.FirstOrDefault(x => x.Id == Item.EstadoId);
                    Item.personal = db.PersonalActivo.FirstOrDefault(s => s.CodigoEmpleado == Item.Empleado.NroEmpleado);
                }

            }
            return View(Items);
        }


        public static DataTable GetDataTableFromRFCTable(IRfcTable myrfcTable)
        {
            DataTable loTable = new DataTable();
            int liElement = 0;
            for (liElement = 0; liElement <= myrfcTable.ElementCount - 1; liElement++)
            {
                RfcElementMetadata metadata = myrfcTable.GetElementMetadata(liElement);
                loTable.Columns.Add(metadata.Name);
            }
            foreach (IRfcStructure Row in myrfcTable)
            {
                DataRow ldr = loTable.NewRow();
                for (liElement = 0; liElement <= myrfcTable.ElementCount - 1; liElement++)
                {
                    RfcElementMetadata metadata = myrfcTable.GetElementMetadata(liElement);
                    ldr[metadata.Name] = Row.GetString(metadata.Name);
                }
                loTable.Rows.Add(ldr);
            }
            return loTable;
        }


        public ActionResult HistorialVac(string Id)
        {

            var model = _repo.ObtenerTodos3(Id);

            return View(model);
        }


        public bool NotificacionCorreo(string asunto, string documento, string Observacion, string nombres, string email, string emailjefe)
        {

            string textocorreo = "";
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);

            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

                if (asunto == "Rechazo")
                {

                    email += ", " + emailjefe;
                    correo.To.Add(email);

                    textocorreo = "Su solicitud fue rechazada. <BR><BR>" + Observacion + "<br><br><br>Cordialmente,<br><H1>Servicios Digitales Foscal</H1><img width='145' height='156' src='http://www.foscal.com.co/correo/logo-correo-foscal.jpg' alt='Logo FOSCAL Internacional' hspace='5' class='CToWUd'>";
                    correo.Subject = "Notificación de Rechazo de Solicitud de vacaciones";
                    correo.Body = "Hola " + nombres + "," + "<BR><BR>" + textocorreo;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;
                }
                if (asunto == "Aprobar")
                {
                    correo.To.Add(emailjefe);

                    textocorreo = "Tiene Pendiente por aprobar una solicitud de vacaciones anticipadas o en dinero del trabajador " + nombres + ".<BR><BR><br>Cordialmente,<br><H1>Servicios Digitales Foscal</H1><img width='145' height='156' src='http://www.foscal.com.co/correo/logo-correo-foscal.jpg' alt='Logo FOSCAL Internacional' hspace='5' class='CToWUd'>";
                    correo.Subject = "Pendiente por Aprobar Vacaciones";
                    correo.Body = "Hola," + "<BR><BR>" + textocorreo;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;
                }


                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                smtp.Host = "smtp-relay.gmail.com";
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


        public ActionResult ControlVacacionesTrabajadores(string Empleado)
        {
            List<ControlVacacionesJefe> resultado = new List<ControlVacacionesJefe>();
            //ControlVacacionesJefe resultado = new ControlVacacionesJefe();
            int Usuario = 0;
            int empleadobusca = 0;
            Int32.TryParse(Empleado, out empleadobusca);
            int Documento = 0;
            string usuariosesion = String.Format("{0}", Session["Empleado"]);
            Int32.TryParse(usuariosesion, out Usuario);
            List<Vacaciones> Items = new List<Vacaciones>();
            ViewBag.DatosSap = "";
            ViewBag.Vacaciones = "";
            ViewBag.NEmpleado = Empleado;

            using (var db = new AutogestionContext())
            {

                var empleado = db.Empleados.FirstOrDefault(e => e.Id == Usuario);
                List<Empleado> lista = db.Empleados.Where(s => s.Jefe == empleado.NroEmpleado).ToList();
                ViewBag.Empleados = lista;

                if (Empleado != null)
                {
                    string consulta = "select e.Id, e.Nombres, e.Documento, e.NroEmpleado, e.FechaNacimiento, e.Telefono, e.Correo, e.Cargo, e.FechaIngreso from Empleadoes as e where e.Id = " + Empleado;
                    var resultados = db.Database.SqlQuery<ControlVacacionesJefe>(consulta).ToList();
                    resultado = resultados;
                    var documento = resultado[0].NroEmpleado;
                    Int32.TryParse(documento, out Documento);

                    //Items = db.Vacaciones.Where(e => e.EmpleadoId == empleadobusca).ToList();
                    Vacaciones item = db.Vacaciones.OrderByDescending(e => e.Fecha).ToList().FirstOrDefault(e => e.EmpleadoId == empleadobusca);
                    if (item != null)
                    {
                        item.EstadoVacaciones = db.EstadoVacaciones.FirstOrDefault(x => x.Id == item.EstadoId);
                    }
                    ViewBag.Vacaciones = item;


                    DataTable Datos = new DataTable();
                    var Result = "";
                    InMemoryDestinationConfiguration DestinacionConfiguracion = new InMemoryDestinationConfiguration();

                    String contraseña = Properties.Settings.Default.Contraseña.ToString();
                    var encodedTextBytes = Convert.FromBase64String(contraseña);

                    contraseña = Encoding.UTF8.GetString(encodedTextBytes);

                    RfcConfigParameters config = new RfcConfigParameters();

                    try
                    {

                        config.Add(RfcConfigParameters.Name, "SAP");
                        config.Add(RfcConfigParameters.AppServerHost, Properties.Settings.Default.Servidor.ToString());
                        config.Add(RfcConfigParameters.SystemNumber, Properties.Settings.Default.Id.ToString());
                        config.Add(RfcConfigParameters.User, Properties.Settings.Default.Usuario.ToString());
                        config.Add(RfcConfigParameters.Password, contraseña);
                        config.Add(RfcConfigParameters.Client, Properties.Settings.Default.Mandante.ToString());
                        config.Add(RfcConfigParameters.Language, "ES");
                        config.Add(RfcConfigParameters.SAPRouter, Properties.Settings.Default.Saprouter.ToString());
                        config.Add(RfcConfigParameters.LogonGroup, "Foscal");

                        RfcDestinationManager.RegisterDestinationConfiguration(DestinacionConfiguracion);
                        DestinacionConfiguracion.AddOrEditDestination(config);
                        RfcDestination destination = RfcDestinationManager.GetDestination("SAP");

                        RfcRepository repository = destination.Repository;
                        IRfcFunction function = repository.CreateFunction("ZMF_CONTING_ABSENTISMO");
                        //PARAMETROS IMPORT
                        function.SetValue("I_PERNR", documento);

                        function.Invoke(destination);
                        //OBTENER RESPUESTA 

                        IRfcTable Tabla = function.GetTable("TB_PA2006");

                        Datos = GetDataTableFromRFCTable2(Tabla);

                        int count = Datos.Rows.Count;
                        int s = count - 1;
                        decimal suma = 0;
                        decimal resta = 0;

                        for (var f = 0; f < count; f++)
                        {
                            if (Datos.Rows[f]["KTART"].ToString() == "10" && Datos.Rows[f]["DEEND"].ToString() == "9999-12-31")
                            {
                                Result += String.Format(Datos.Rows[f]["BEGDA"].ToString() + ";" + Datos.Rows[f]["ENDDA"].ToString() + ";" + Math.Round(Convert.ToDecimal(Datos.Rows[f]["ANZHL"].ToString())) + ";" + Math.Round(Convert.ToDecimal(Datos.Rows[f]["KVERB"].ToString())) + ",");

                                resta = (Math.Round(Convert.ToDecimal(Datos.Rows[f]["ANZHL"].ToString())) - Math.Round(Convert.ToDecimal(Datos.Rows[f]["KVERB"].ToString())));
                                suma = suma + resta;
                            }
                        }


                        Result = Result.Substring(0, Result.Length - 1);
                        ViewBag.DatosSap = Result;
                        string[] filas = Result.Split(',');
                        var ultimafila = filas.Last();
                        var filas2 = ultimafila.Split(';');
                        ViewBag.Diaspendientes = filas2[2];
                        ViewBag.DiasDisfrutados = filas2[3];
                        ViewBag.Disfrutados = suma;

                    }
                    catch (SystemException ex)
                    {
                        // ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
                    }
                    catch (RfcLogonException ex)
                    {
                        //ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
                    }
                    catch (RfcAbapException ex)
                    {
                        //ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
                    }
                    finally
                    {
                        RfcDestinationManager.UnregisterDestinationConfiguration(DestinacionConfiguracion);
                        // DestinacionConfiguracion.RemoveDestination("SAP");
                    }

                }

            }

            return View(resultado);
        }

        public ActionResult ListaEmpleados(string Area, string Sociedad)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ListaEmpleados"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }


            List<ControlVacacionesJefe> resultado = new List<ControlVacacionesJefe>();
            List<SelectListItem> lst = new List<SelectListItem>();
            List<SelectListItem> lst2 = new List<SelectListItem>();
            int Usuario = 0;
            string usuariosesion = String.Format("{0}", Session["Empleado"]);
            Int32.TryParse(usuariosesion, out Usuario);
            List<Vacaciones> vacaciones = new List<Vacaciones>();
            string cantdp = "";


            DataTable Datos = new DataTable();
            var Result = "";



            using (var db = new AutogestionContext())
            {

                var empleadoid = Session["Empleado"];
                var empleados = db.Empleados.Find(empleadoid);

                //var lista = db.PersonalActivo.Where(x => x.Jefe == empleados.NroEmpleado ).Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                var lista = db.Empleados.Where(x => x.Jefe == empleados.NroEmpleado && x.Activo == "SI").Select(x => new { x.AreaDescripcion }).GroupBy(b => b.AreaDescripcion).ToList();
                foreach (var x in lista)
                {
                    lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }

                ViewBag.Opciones = lst;

                //var listas2 = db.PersonalActivo.Where(x => x.Jefe == empleados.NroEmpleado).Select(x => new { x.Empresa }).GroupBy(b => b.Empresa).ToList();
                var listas2 = db.Empleados.Where(x => x.Jefe == empleados.NroEmpleado && x.Activo == "SI").Select(x => new { x.Empresa }).GroupBy(b => b.Empresa).ToList();
                foreach (var x in listas2)
                {
                    lst2.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }


                ViewBag.Empresas = lst2;





                var empleado = db.Empleados.FirstOrDefault(e => e.Id == Usuario);
                //List<Empleado> lista = db.Empleados.Where(s => s.Superior == empleado.NroEmpleado || s.Jefe == empleado.NroEmpleado && s.Activo != "NO").Select(x => new { x.Nombres }).GroupBy(b => b.Nombres).ToList();
                List<Empleado> lista2 = db.Empleados.Where(s => s.Jefe == empleado.NroEmpleado && s.Activo != "NO")
                                                    .Where(s => s.AreaDescripcion.Contains(Area))
                                                    .Where(s => s.Empresa.Contains(Sociedad)).OrderBy(b => b.Nombres).ToList();
                InMemoryDestinationConfiguration DestinacionConfiguracion = new InMemoryDestinationConfiguration();



                foreach (Empleado datosemple in lista2)
                {

                    try
                    {
                        var datosemp = db.Empleados.FirstOrDefault(e => e.Id == datosemple.Id);
                        ViewBag.NEmpleado = datosemple.Id;


                        //*********************************
                        //String contraseña = Properties.Settings.Default.Contraseña.ToString();
                        //var encodedTextBytes = Convert.FromBase64String(contraseña);

                        //contraseña = Encoding.UTF8.GetString(encodedTextBytes);

                        //RfcConfigParameters config = new RfcConfigParameters();


                        //config.Add(RfcConfigParameters.Name, "SAP");
                        //config.Add(RfcConfigParameters.AppServerHost, Properties.Settings.Default.Servidor.ToString());
                        //config.Add(RfcConfigParameters.SystemNumber, Properties.Settings.Default.Id.ToString());
                        //config.Add(RfcConfigParameters.User, Properties.Settings.Default.Usuario.ToString());
                        //config.Add(RfcConfigParameters.Password, contraseña);
                        //config.Add(RfcConfigParameters.Client, Properties.Settings.Default.Mandante.ToString());
                        //config.Add(RfcConfigParameters.Language, "ES");
                        //config.Add(RfcConfigParameters.SAPRouter, Properties.Settings.Default.Saprouter.ToString());
                        //config.Add(RfcConfigParameters.LogonGroup, "Foscal");

                        //RfcDestinationManager.RegisterDestinationConfiguration(DestinacionConfiguracion);
                        //DestinacionConfiguracion.AddOrEditDestination(config);
                        //RfcDestination destination = RfcDestinationManager.GetDestination("SAP");

                        //RfcRepository repository = destination.Repository;
                        //IRfcFunction function = repository.CreateFunction("ZMF_CONTING_ABSENTISMO");
                        ////PARAMETROS IMPORT
                        //function.SetValue("I_PERNR", datosemple.NroEmpleado);

                        //function.Invoke(destination);
                        ////OBTENER RESPUESTA 

                        //IRfcTable Tabla = function.GetTable("TB_PA2006");

                        //Datos = GetDataTableFromRFCTable2(Tabla);

                        //int count = Datos.Rows.Count;
                        //int s = count - 1;
                        //decimal suma = 0;
                        //decimal resta = 0;

                        //for (var f = 0; f < count; f++)
                        //{
                        //    if (Datos.Rows[f]["KTART"].ToString() == "10" && Datos.Rows[f]["DEEND"].ToString() == "9999-12-31")
                        //    {
                        //        Result += String.Format(Datos.Rows[f]["BEGDA"].ToString() + ";" + Datos.Rows[f]["ENDDA"].ToString() + ";" + Math.Round(Convert.ToDecimal(Datos.Rows[f]["ANZHL"].ToString())) + ";" + Math.Round(Convert.ToDecimal(Datos.Rows[f]["KVERB"].ToString())) + ",");

                        //        resta = (Math.Round(Convert.ToDecimal(Datos.Rows[f]["ANZHL"].ToString())) - Math.Round(Convert.ToDecimal(Datos.Rows[f]["KVERB"].ToString())));
                        //        suma = suma + resta;
                        //    }
                        //}
                        //cantdp = string.Format("{0}", suma);

                        //******************************************

                        vacaciones.Add(new Vacaciones() { CantDiasPendientes = cantdp, Empleado = datosemp });

                    }
                    catch (SystemException ex)
                    {
                        // ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
                    }
                    catch (RfcLogonException ex)
                    {
                        //ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
                    }
                    catch (RfcAbapException ex)
                    {
                        //ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
                    }
                    finally
                    {
                        RfcDestinationManager.UnregisterDestinationConfiguration(DestinacionConfiguracion);
                        //DestinacionConfiguracion.RemoveDestination("SAP");
                    }


                }
            }



            return View(vacaciones);
        }

        [HttpPost]
        public String ObtenerFotoEmpleado(string id)
        {
            try
            {
                Byte[] b = null;
                try
                {
                    b = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/AnexosFotos/" + id + ".png"));
                }
                catch
                {
                    try
                    {
                        b = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/AnexosFotos/" + id + ".jpg"));
                    }
                    catch
                    {
                        try
                        {
                            b = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/AnexosFotos/" + id + ".jpeg"));
                        }
                        catch
                        {


                        }

                    }

                }


                return Convert.ToBase64String(b);

            }
            catch (Exception e)
            {

                return "error";
            }


        }


        public static DataTable GetDataTableFromRFCTable2(IRfcTable myrfcTable)
        {
            DataTable loTable = new DataTable();
            int liElement = 0;
            for (liElement = 0; liElement <= myrfcTable.ElementCount - 1; liElement++)
            {
                RfcElementMetadata metadata = myrfcTable.GetElementMetadata(liElement);
                loTable.Columns.Add(metadata.Name);
            }
            foreach (IRfcStructure Row in myrfcTable)
            {
                DataRow ldr = loTable.NewRow();
                for (liElement = 0; liElement <= myrfcTable.ElementCount - 1; liElement++)
                {
                    RfcElementMetadata metadata = myrfcTable.GetElementMetadata(liElement);
                    ldr[metadata.Name] = Row.GetString(metadata.Name);
                }
                loTable.Rows.Add(ldr);
            }
            return loTable;
        }


        public static string DataSetToJSON(DataTable dt)
        {

            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in dt.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            return jsSerializer.Serialize(parentRow);
        }


        [HttpPost]
        public JsonResult CantidadDIasDisponibles(int Empleado)
        {
            decimal suma = 0;

            using (AutogestionContext db = new AutogestionContext())
            {

                var empleado = db.Empleados.Find(Empleado);

                DataTable Datos = new DataTable();
                var Result = "";
                InMemoryDestinationConfiguration DestinacionConfiguracion = new InMemoryDestinationConfiguration();

                String contraseña = Properties.Settings.Default.Contraseña.ToString();
                var encodedTextBytes = Convert.FromBase64String(contraseña);

                contraseña = Encoding.UTF8.GetString(encodedTextBytes);

                RfcConfigParameters config = new RfcConfigParameters();

                try
                {

                    config.Add(RfcConfigParameters.Name, "SAP");
                    config.Add(RfcConfigParameters.AppServerHost, Properties.Settings.Default.Servidor.ToString());
                    config.Add(RfcConfigParameters.SystemNumber, Properties.Settings.Default.Id.ToString());
                    config.Add(RfcConfigParameters.User, Properties.Settings.Default.Usuario.ToString());
                    config.Add(RfcConfigParameters.Password, contraseña);
                    config.Add(RfcConfigParameters.Client, Properties.Settings.Default.Mandante.ToString());
                    config.Add(RfcConfigParameters.Language, "ES");
                    config.Add(RfcConfigParameters.SAPRouter, Properties.Settings.Default.Saprouter.ToString());
                    config.Add(RfcConfigParameters.LogonGroup, "Foscal");

                    RfcDestinationManager.RegisterDestinationConfiguration(DestinacionConfiguracion);
                    DestinacionConfiguracion.AddOrEditDestination(config);
                    RfcDestination destination = RfcDestinationManager.GetDestination("SAP");

                    RfcRepository repository = destination.Repository;
                    IRfcFunction function = repository.CreateFunction("ZMF_CONTING_ABSENTISMO");
                    //PARAMETROS IMPORT
                    function.SetValue("I_PERNR", empleado.NroEmpleado);

                    function.Invoke(destination);
                    //OBTENER RESPUESTA 

                    IRfcTable Tabla = function.GetTable("TB_PA2006");

                    Datos = GetDataTableFromRFCTable2(Tabla);

                    int count = Datos.Rows.Count;
                    int s = count - 1;

                    decimal resta = 0;

                    for (var f = 0; f < count; f++)
                    {
                        if (Datos.Rows[f]["KTART"].ToString() == "10" && Datos.Rows[f]["DEEND"].ToString() == "9999-12-31")
                        {
                            //Result += String.Format(Datos.Rows[f]["BEGDA"].ToString() + ";" + Datos.Rows[f]["ENDDA"].ToString() + ";" + Math.Round(Convert.ToDecimal(Datos.Rows[f]["ANZHL"].ToString())) + ";" + Math.Round(Convert.ToDecimal(Datos.Rows[f]["KVERB"].ToString())) + ",");

                            resta = (Math.Round(Convert.ToDecimal(Datos.Rows[f]["ANZHL"].ToString())) - Math.Round(Convert.ToDecimal(Datos.Rows[f]["KVERB"].ToString())));
                            suma = suma + resta;
                        }
                    }




                }
                catch (SystemException ex)
                {
                    // ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
                }
                catch (RfcLogonException ex)
                {
                    //ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
                }
                catch (RfcAbapException ex)
                {
                    //ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
                }
                finally
                {
                    RfcDestinationManager.UnregisterDestinationConfiguration(DestinacionConfiguracion);
                    // DestinacionConfiguracion.RemoveDestination("SAP");
                }


                return Json(suma);
            }

        }

        [HttpPost]
        public string validarFechaini(string fechaini)
        {
            string respuesta = "Ok";
            DateTime fechaSelec = DateTime.Now;
            DateTime.TryParse(fechaini, out fechaSelec);

            if (fechaSelec.DayOfWeek == DayOfWeek.Saturday || fechaSelec.DayOfWeek == DayOfWeek.Sunday)
            {
                respuesta = "Error";
            }

            using (AutogestionContext db = new AutogestionContext())
            {
                var festivos = db.DiasFestivos.Where(e => DbFunctions.TruncateTime(e.festivo) == fechaSelec).ToList();
                if (festivos.Count != 0)
                {
                    respuesta = "Error";
                }
            }

            return (respuesta);
        }

        [HttpPost]
        public string Festivos(string fechaini, string diferencia)
        {
            string respuesta = "";
            var contador = 0;
            var i = 0;
            var j = 0;
            int dif = 0;
            DateTime fechaSelec = DateTime.Now;

            Int32.TryParse(diferencia, out dif);


            DateTime.TryParse(fechaini, out fechaSelec);
            //fechaSelec = fechaSelec.AddDays(1);
            using (AutogestionContext db = new AutogestionContext())
            {
                for (i = 0; i < dif; i++)
                {
                    var dia = fechaSelec.DayOfWeek;
                    if (fechaSelec.DayOfWeek == DayOfWeek.Saturday || fechaSelec.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dif = dif + 1;
                        fechaSelec = fechaSelec.AddDays(1);
                    }
                    else
                    {
                        fechaSelec = fechaSelec.AddDays(1);
                    }


                    var festivos = db.DiasFestivos.Where(e => DbFunctions.TruncateTime(e.festivo) == fechaSelec).ToList();
                    var dia4 = fechaSelec.DayOfWeek;
                    if (festivos.Count != 0)
                    {
                        var dia2 = fechaSelec.DayOfWeek;
                        if (fechaSelec.DayOfWeek == DayOfWeek.Saturday || fechaSelec.DayOfWeek == DayOfWeek.Sunday)
                        {
                        }
                        else
                        {
                            dif = dif + 1;
                        }
                    }
                }




                if (fechaSelec.DayOfWeek == DayOfWeek.Saturday)
                {

                    fechaSelec = fechaSelec.AddDays(2);
                }

                if (fechaSelec.DayOfWeek == DayOfWeek.Sunday)
                {

                    fechaSelec = fechaSelec.AddDays(1);
                }

                var festivos3 = db.DiasFestivos.Where(e => DbFunctions.TruncateTime(e.festivo) == fechaSelec).ToList();
                if (festivos3.Count != 0)
                {
                    fechaSelec = fechaSelec.AddDays(1); ;
                }
            }

            respuesta = String.Format("{0}", fechaSelec);

            return (respuesta);

        }

        public ActionResult EditarModoTrabajo(int id)
        {
            Session["IdEmpleado"] = id;

            using (AutogestionContext db = new AutogestionContext())
            {
                var model = db.Empleados.FirstOrDefault(x => x.Id == id);
                return PartialView(model);
            }

        }

        [HttpPost]
        //public ActionResult GuardarModoTrabajo(string ModoTrabajo, string ObservacionModoTrabajo)
        public JsonResult GuardarModoTrabajo()
        {

            var respuesta = "true";
            try
            {

                var ModoTrabajo = HttpContext.Request.Params["Empleado.ModoTrabajo"];
                var ObservacionModoTrabajo = HttpContext.Request.Params["Empleado.ObservacionModoTrabajo"];
                var RequiereDesplazamiento = HttpContext.Request.Params["Empleado.RequiereDesplazamiento"];


                int id = 0;
                string numero = String.Format("{0}", Session["IdEmpleado"]);
                Int32.TryParse(numero, out id);
                _repo.CambioModoTrabajo(id, ModoTrabajo, ObservacionModoTrabajo, RequiereDesplazamiento);
                //return RedirectToAction("ListaEmpleados");
                var url = Url.Action("");
                url = Url.Action("ListaEmpleados");
                return Json(new
                {
                    redirectUrl = url,
                    isRedirect = true,
                    respuesta = "true"
                });
            }
            catch
            {
                return Json(new { respuesta });
            }
        }


        public ActionResult SolicitarAutorizacionVacaciones(string Empl)
        {

            string Empleado = String.Format("{0}", Session["Empleado"]);
            Empleado datos = new Empleado();
            int Id = 0;
            Int32.TryParse(Empleado, out Id);


            using (var db = new AutogestionContext())
            {
                datos = db.Empleados.Find(Id);
                ViewBag.ListadoEmpleadosJefe = db.Empleados.Where(e => e.Jefe == datos.NroEmpleado).ToList();

            }

            return PartialView();
        }


        public ActionResult GrabarSolicitudAutorVac(string VacacionesPagadas, string VacacionesAdelantadas, string VacacionesMayores6, string EmpleadoP)
        {
            string res = "";
            string message = "";
            string JefeSolicita = "TXTSOLAUTJEFE";
            int empleadoid = 0;
            Int32.TryParse(EmpleadoP, out empleadoid);
            string Empleado = String.Format("{0}", Session["Empleado"]);
            int Id = 0;
            Int32.TryParse(Empleado, out Id);

            using (var db = new AutogestionContext())
            {
                var EmpleadoSol = db.Empleados.FirstOrDefault(e => e.Id == empleadoid);
                var Jefe = db.Empleados.FirstOrDefault(e => e.Id == Id);


                var envio = _ApiVac.NotificarAutorizacion(EmpleadoSol.Nombres, EmpleadoSol.Documento, EmpleadoSol.NroEmpleado, VacacionesPagadas, VacacionesAdelantadas, VacacionesMayores6, EmpleadoSol.Empresa, Jefe.Correo, JefeSolicita);
                if (envio == true)
                {
                    message = "Se registro correctamente la solicitud de autorización, la respuesta sera enviada por correo electrónico.";
                    return RedirectToAction("CreateXJefe", new { message = message });
                }
                else { res = "false"; }
            }
            //return View("CreateXJefe");
            return RedirectToAction("CreateXJefe", new { message = message });
        }



        public ActionResult ExportarInforme(String Area)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ExportarInforme"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            List<ControlVacacionesJefe> resultado = new List<ControlVacacionesJefe>();
            List<SelectListItem> lst = new List<SelectListItem>();
            int Usuario = 0;
            string usuariosesion = String.Format("{0}", Session["Empleado"]);
            Int32.TryParse(usuariosesion, out Usuario);
            List<Vacaciones> vacaciones = new List<Vacaciones>();
            string cantdp = "";
            string periodo = "";


            DataTable Datos = new DataTable();
            var Result = "";


            using (AutogestionContext db = new AutogestionContext())
            {

                var empleadoid = Session["Empleado"];
                var empleado = db.Empleados.Find(empleadoid);

                //var lista = db.PersonalActivo.Where(x =>  x.Jefe == empleado.NroEmpleado).Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                var lista = db.Empleados.Where(x => x.Jefe == empleado.NroEmpleado && x.Activo == "SI").Select(x => new { x.AreaDescripcion }).GroupBy(b => b.AreaDescripcion).ToList();
                foreach (var x in lista)
                {
                    lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }


                ViewBag.Opciones = lst;

                if (Area != null)
                {

                    var empleados = db.Empleados.FirstOrDefault(e => e.Id == Usuario);
                    //List<Empleado> lista = db.Empleados.Where(s => s.Superior == empleado.NroEmpleado || s.Jefe == empleado.NroEmpleado && s.Activo != "NO").Select(x => new { x.Nombres }).GroupBy(b => b.Nombres).ToList();
                    List<Empleado> lista2 = db.Empleados.Where(s => (s.Jefe == empleados.NroEmpleado) && s.Activo != "NO" && s.AreaDescripcion == Area).OrderBy(b => b.Nombres).ToList();
                    InMemoryDestinationConfiguration DestinacionConfiguracion = new InMemoryDestinationConfiguration();


                    foreach (Empleado datosemple in lista2)
                    {



                        try
                        {
                            var datosemp = db.Empleados.FirstOrDefault(e => e.Id == datosemple.Id);
                            ViewBag.NEmpleado = datosemple.Id;


                            //*********************************
                            String contraseña = Properties.Settings.Default.Contraseña.ToString();
                            var encodedTextBytes = Convert.FromBase64String(contraseña);

                            contraseña = Encoding.UTF8.GetString(encodedTextBytes);

                            RfcConfigParameters config = new RfcConfigParameters();


                            config.Add(RfcConfigParameters.Name, "SAP");
                            config.Add(RfcConfigParameters.AppServerHost, Properties.Settings.Default.Servidor.ToString());
                            config.Add(RfcConfigParameters.SystemNumber, Properties.Settings.Default.Id.ToString());
                            config.Add(RfcConfigParameters.User, Properties.Settings.Default.Usuario.ToString());
                            config.Add(RfcConfigParameters.Password, contraseña);
                            config.Add(RfcConfigParameters.Client, Properties.Settings.Default.Mandante.ToString());
                            config.Add(RfcConfigParameters.Language, "ES");
                            config.Add(RfcConfigParameters.SAPRouter, Properties.Settings.Default.Saprouter.ToString());
                            config.Add(RfcConfigParameters.LogonGroup, "Foscal");

                            RfcDestinationManager.RegisterDestinationConfiguration(DestinacionConfiguracion);
                            DestinacionConfiguracion.AddOrEditDestination(config);
                            RfcDestination destination = RfcDestinationManager.GetDestination("SAP");

                            RfcRepository repository = destination.Repository;
                            IRfcFunction function = repository.CreateFunction("ZMF_CONTING_ABSENTISMO");
                            //PARAMETROS IMPORT
                            function.SetValue("I_PERNR", datosemple.NroEmpleado);

                            function.Invoke(destination);
                            //OBTENER RESPUESTA 

                            IRfcTable Tabla = function.GetTable("TB_PA2006");

                            Datos = GetDataTableFromRFCTable2(Tabla);

                            int count = Datos.Rows.Count;
                            int s = count - 1;
                            decimal suma = 0;
                            decimal resta = 0;
                            var fechas = "";

                            for (var f = 0; f < count; f++)
                            {
                                if (Datos.Rows[f]["KTART"].ToString() == "10" && Datos.Rows[f]["DEEND"].ToString() == "9999-12-31")
                                {
                                    Result += String.Format(Datos.Rows[f]["BEGDA"].ToString() + ";" + Datos.Rows[f]["ENDDA"].ToString() + ";" + Math.Round(Convert.ToDecimal(Datos.Rows[f]["ANZHL"].ToString())) + ";" + Math.Round(Convert.ToDecimal(Datos.Rows[f]["KVERB"].ToString())) + ",");

                                    resta = (Math.Round(Convert.ToDecimal(Datos.Rows[f]["ANZHL"].ToString())) - Math.Round(Convert.ToDecimal(Datos.Rows[f]["KVERB"].ToString())));
                                    suma = suma + resta;
                                }
                                fechas = String.Format(Datos.Rows[f]["BEGDA"].ToString() + " / " + Datos.Rows[f]["ENDDA"].ToString());
                            }
                            periodo = fechas;
                            cantdp = string.Format("{0}", suma);

                            //******************************************

                            vacaciones.Add(new Vacaciones() { CantDiasPendientes = cantdp, Empleado = datosemp, Periodo = periodo });

                        }
                        catch (SystemException ex)
                        {
                            // ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
                        }
                        catch (RfcLogonException ex)
                        {
                            //ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
                        }
                        catch (RfcAbapException ex)
                        {
                            //ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
                        }
                        finally
                        {
                            RfcDestinationManager.UnregisterDestinationConfiguration(DestinacionConfiguracion);
                            //DestinacionConfiguracion.RemoveDestination("SAP");
                        }


                    }



                }

            }
            return View(vacaciones);
        }


        public ActionResult ExportarInformeGH()
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ExportarInformeGH"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            List<ControlVacacionesJefe> resultado = new List<ControlVacacionesJefe>();
            List<SelectListItem> lst = new List<SelectListItem>();
            int Usuario = 0;
            string usuariosesion = String.Format("{0}", Session["Empleado"]);
            Int32.TryParse(usuariosesion, out Usuario);
            List<Vacaciones> vacaciones = new List<Vacaciones>();
            string cantdp = "";
            string periodo = "";


            DataTable Datos = new DataTable();
            var Result = "";
            string accion = "abrir";
            ViewBag.Ventana = "2";


            using (AutogestionContext db = new AutogestionContext())
            {



                var empleadoid = Session["Empleado"];
                var empleado = db.Empleados.Find(empleadoid);

                var lista = db.PersonalActivo.Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                foreach (var x in lista)
                {
                    lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }


                ViewBag.Area = lst;

                ViewBag.Empleados = db.Empleados.Select(e => new { e.Id, e.Nombres, e.Activo, nomcodigo = e.Id + "-" + e.Nombres, e.FechaFin }).Where(f => f.Activo != "NO" || f.FechaFin >= DateTime.Now).ToList();


            }

            return View(vacaciones);
        }



        public JsonResult DatosExportar(string Area, string Empresa, string Empleado)
        {


            List<SelectListItem> lst = new List<SelectListItem>();
            List<Vacaciones> vacaciones = new List<Vacaciones>();
            string cantdp = "";
            string periodo = "";


            DataTable Datos = new DataTable();
            var Result = "";


            using (AutogestionContext db = new AutogestionContext())
            {



                var empleadoid = Session["Empleado"];
                var empleado = db.Empleados.Find(empleadoid);

                //var lista = db.PersonalActivo.Where(x => x.Jefe == empleado.NroEmpleado).Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                var lista = db.Empleados.Where(x => x.Jefe == empleado.NroEmpleado && x.Activo == "SI").Select(x => new { x.AreaDescripcion }).GroupBy(b => b.AreaDescripcion).ToList();
                foreach (var x in lista)
                {
                    lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }


                ViewBag.Area = lst;

                ViewBag.Empleados = db.Empleados.Select(e => new { e.Id, e.Nombres, e.Activo, nomcodigo = e.Id + "-" + e.Nombres, e.FechaFin }).Where(f => f.Activo != "NO" || f.FechaFin >= DateTime.Now).ToList();



                if (Area != null)
                {



                    List<Empleado> lista2 = db.Empleados.Where(s => s.AreaDescripcion.Contains(Area))
                                                                .Where(s => s.Empresa.Contains(Empresa))
                                                                .Where(s => s.Nombres.Contains(Empleado))
                                                                .Where(s => s.Activo == "SI")
                                                               .OrderBy(b => b.Nombres).ToList();




                    foreach (Empleado datosemple in lista2)
                    {


                        InMemoryDestinationConfiguration DestinacionConfiguracion = new InMemoryDestinationConfiguration();
                        try
                        {


                            var datosemp = db.Empleados.FirstOrDefault(e => e.Id == datosemple.Id);
                            ViewBag.NEmpleado = datosemple.Id;


                            //*********************************
                            String contraseña = Properties.Settings.Default.Contraseña.ToString();
                            var encodedTextBytes = Convert.FromBase64String(contraseña);

                            contraseña = Encoding.UTF8.GetString(encodedTextBytes);

                            RfcConfigParameters config = new RfcConfigParameters();


                            config.Add(RfcConfigParameters.Name, "SAP");
                            config.Add(RfcConfigParameters.AppServerHost, Properties.Settings.Default.Servidor.ToString());
                            config.Add(RfcConfigParameters.SystemNumber, Properties.Settings.Default.Id.ToString());
                            config.Add(RfcConfigParameters.User, Properties.Settings.Default.Usuario.ToString());
                            config.Add(RfcConfigParameters.Password, contraseña);
                            config.Add(RfcConfigParameters.Client, Properties.Settings.Default.Mandante.ToString());
                            config.Add(RfcConfigParameters.Language, "ES");
                            config.Add(RfcConfigParameters.SAPRouter, Properties.Settings.Default.Saprouter.ToString());
                            config.Add(RfcConfigParameters.LogonGroup, "Foscal");

                            RfcDestinationManager.RegisterDestinationConfiguration(DestinacionConfiguracion);
                            DestinacionConfiguracion.AddOrEditDestination(config);
                            RfcDestination destination = RfcDestinationManager.GetDestination("SAP");

                            RfcRepository repository = destination.Repository;
                            IRfcFunction function = repository.CreateFunction("ZMF_CONTING_ABSENTISMO");
                            //PARAMETROS IMPORT
                            function.SetValue("I_PERNR", datosemple.NroEmpleado);

                            function.Invoke(destination);
                            //OBTENER RESPUESTA

                            IRfcTable Tabla = function.GetTable("TB_PA2006");

                            Datos = GetDataTableFromRFCTable2(Tabla);

                            int count = Datos.Rows.Count;
                            int s = count - 1;
                            decimal suma = 0;
                            decimal resta = 0;
                            var fechas = "";

                            for (var f = 0; f < count; f++)
                            {
                                if (Datos.Rows[f]["KTART"].ToString() == "10" && Datos.Rows[f]["DEEND"].ToString() == "9999-12-31")
                                {
                                    Result += String.Format(Datos.Rows[f]["BEGDA"].ToString() + ";" + Datos.Rows[f]["ENDDA"].ToString() + ";" + Math.Round(Convert.ToDecimal(Datos.Rows[f]["ANZHL"].ToString())) + ";" + Math.Round(Convert.ToDecimal(Datos.Rows[f]["KVERB"].ToString())) + ",");

                                    resta = (Math.Round(Convert.ToDecimal(Datos.Rows[f]["ANZHL"].ToString())) - Math.Round(Convert.ToDecimal(Datos.Rows[f]["KVERB"].ToString())));
                                    suma = suma + resta;
                                }
                                fechas = String.Format(Datos.Rows[f]["BEGDA"].ToString() + " / " + Datos.Rows[f]["ENDDA"].ToString());
                            }
                            periodo = fechas;
                            cantdp = string.Format("{0}", suma);

                            //******************************************

                            vacaciones.Add(new Vacaciones() { CantDiasPendientes = cantdp, Empleado = datosemp, Periodo = periodo });
                            RfcDestinationManager.UnregisterDestinationConfiguration(DestinacionConfiguracion);


                        }
                        catch (RfcInvalidStateException ex)
                        {
                            try
                            {
                                RfcDestinationManager.UnregisterDestinationConfiguration(DestinacionConfiguracion);
                            }
                            catch
                            {

                            }
                        }
                        catch (SystemException ex)
                        {
                            try
                            {
                                RfcDestinationManager.UnregisterDestinationConfiguration(DestinacionConfiguracion);
                            }
                            catch
                            {

                            }
                        }
                        catch (RfcLogonException ex)
                        {
                            try
                            {
                                RfcDestinationManager.UnregisterDestinationConfiguration(DestinacionConfiguracion);
                            }
                            catch
                            {

                            }
                        }
                        catch (RfcAbapException ex)
                        {
                            try
                            {
                                RfcDestinationManager.UnregisterDestinationConfiguration(DestinacionConfiguracion);
                            }
                            catch
                            {

                            }
                        }
                        finally
                        {
                            try
                            {
                                RfcDestinationManager.UnregisterDestinationConfiguration(DestinacionConfiguracion);
                            }
                            catch
                            {

                            }

                            //DestinacionConfiguracion.RemoveDestination("SAP");
                        }

                    }

                }

            }

            return Json(vacaciones);
        }


        public ActionResult Calendario()
        {

            return View();
        }


        public string Datoscalendario()
        {
            List<CalendarioVacaciones> Items = new List<CalendarioVacaciones>();
            string Result = "";
            string datos = "";
            string Fecha1 = "";
            string Fecha2 = "";

            using (var db = new AutogestionContext())
            {
                var empleadoid = Session["Empleado"];
                var empleado = db.Empleados.Find(empleadoid);


                string consulta = "select e.Nombres, Vacaciones.FechaInicial, Vacaciones.FechaFin, es.Nombre as Estado from Vacaciones inner join Empleadoes as e on e.Id = Vacaciones.EmpleadoId inner join EstadoVacaciones as es on es.Id = Vacaciones.EstadoId where (e.Jefe = " + empleado.NroEmpleado + ") and Vacaciones.EstadoId != 5 ";
                var resultados = db.Database.SqlQuery<CalendarioVacaciones>(consulta).ToList();

                foreach (CalendarioVacaciones fila in resultados)
                {
                    fila.FechaFin = fila.FechaFin.AddDays(1);
                    Fecha1 = fila.FechaInicial.ToString("yyyy-MM-dd");
                    Fecha2 = fila.FechaFin.ToString("yyyy-MM-dd");


                    Result += String.Format(fila.Nombres + ";" + Fecha1 + ";" + Fecha2 + ";" + fila.Estado + ",");
                }


                Result = Result.Substring(0, Result.Length - 1);
            }

            return Result;
        }


        public string Anular_Vacaciones(int id)
        {
            var message = "";
            using (var db = new AutogestionContext())
            {
                var jsonDeserialize = new JavaScriptSerializer();
                DateTime Fecha2 = DateTime.Now;

                try
                {
                    var UsuarioModificaid = Session["Empleado"];
                    Vacaciones Vacaciones = new Vacaciones();
                    Vacaciones = db.Vacaciones.FirstOrDefault(e => e.Id == id);
                    db.Vacaciones.Attach(Vacaciones);
                    Vacaciones.EstadoId = 5;
                    db.SaveChanges();


                    HistorialVacaciones Historial = new HistorialVacaciones();

                    Historial.VacacionesId = Convert.ToInt32(id);
                    Historial.Accion = "5";
                    Historial.Fecha = Fecha2;
                    Historial.EmpleadoId = Vacaciones.EmpleadoId;
                    Historial.UsuarioModifica = Convert.ToInt32(UsuarioModificaid);
                    Historial.Observaciones = "Registro anulado Correctamente";
                    db.HistorialVacaciones.Add(Historial);
                    db.SaveChanges();
                    message = "Registro Anulado Correctamente";


                }
                catch
                {
                    message = "Error Al Anular El Registro";
                }

                var json = jsonDeserialize.Serialize(message);
                return json;
            }

        }



        //Carga Masiva periodo Vacaciones

        private static List<List<string>> _datosTemporales;
        public ActionResult CargarPeriodosVacaciones(HttpPostedFileBase archivoExcel)
        {

            string message = "";
            var empleadoid = Session["Empleado"];

            if (archivoExcel != null && archivoExcel.ContentLength > 0)
            {
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        archivoExcel.InputStream.CopyTo(stream);
                        stream.Position = 0;

                        using (var paquete = new ExcelPackage(stream))
                        {

                            if (paquete.Workbook.Worksheets.Count >= 1) // Verifica si hay al menos una hoja
                            {
                                var hoja = paquete.Workbook.Worksheets[0]; // Suponiendo que la primera hoja es la que quieres cargar
                                int totalFilas = hoja.Dimension.Rows;
                                int totalColumnas = hoja.Dimension.Columns;

                                _datosTemporales = new List<List<string>>();
                                List<string> columnas = new List<string>();

                                // Obtener nombres de columnas (primera fila)
                                for (int col = 1; col <= totalColumnas; col++)
                                {
                                    columnas.Add(hoja.Cells[1, col].Value?.ToString());
                                }
                                ViewBag.Columnas = columnas;

                                // Obtener datos (a partir de la segunda fila)
                                for (int fila = 2; fila <= totalFilas; fila++)
                                {
                                    List<string> filaDatos = new List<string>();
                                    for (int col = 1; col <= totalColumnas; col++)
                                    {
                                        filaDatos.Add(hoja.Cells[fila, col].Value?.ToString());
                                    }
                                    _datosTemporales.Add(filaDatos);
                                }

                                ViewBag.Datos = _datosTemporales;

                                List<PeriodoVacacionesEmpleado> model = new List<PeriodoVacacionesEmpleado>();
                                using (var db = new AutogestionContext())
                                {

                                    if (_datosTemporales != null)
                                    {
                                        foreach (var fila in _datosTemporales)
                                        {
                                            DateTime? fechaingresoreal = null;
                                            DateTime fechaingreso = DateTime.MinValue;
                                            DateTime periodoini = DateTime.MinValue;
                                            DateTime peridofin = DateTime.MinValue;
                                            string[] formatos = { "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy", "yyyy/MM/dd H:mm:ss", "yyyy/MM/dd" };

                                            // Función local para intentar parsear la fecha de forma segura
                                            DateTime? ParsearFechaSegura(string valor)
                                            {
                                                if (!string.IsNullOrEmpty(valor))
                                                    if (!string.IsNullOrEmpty(valor))
                                                    {
                                                        if (double.TryParse(valor, out double excelSerialDate))
                                                        {
                                                            try
                                                            {
                                                                return DateTime.FromOADate(excelSerialDate);
                                                            }
                                                            catch (Exception)
                                                            {
                                                                Console.WriteLine($"Advertencia: No se pudo convertir la fecha numérica de Excel '{valor}'.");
                                                                // Si falla la conversión de OADate, podríamos intentar parsear como texto igualmente
                                                            }
                                                        }

                                                       if (DateTime.TryParseExact(valor, formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime resultado))
                                                        {
                                                            return resultado;
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine($"Advertencia: No se pudo parsear la fecha '{valor}'. Valor: '{valor}'");
                                                            return null;
                                                        }
                                                    }
                                                return null;
                                            }

                                            // Procesar fila[1] (fechaingresoreal)
                                            fechaingresoreal = ParsearFechaSegura(fila[1]);

                                            // Procesar fila[2] (fechaingreso)
                                            DateTime? fechaIngresoNullable = ParsearFechaSegura(fila[2]);
                                            if (fechaIngresoNullable.HasValue)
                                            {
                                                fechaingreso = fechaIngresoNullable.Value;
                                            }
                                            else
                                            {
                                                Console.WriteLine($"Advertencia: No se pudo parsear la fecha de ingreso '{fila[2]}'. Se usará el valor predeterminado: {fechaingreso}");
                                                // Considera si este es el comportamiento deseado
                                            }

                                            // Procesar fila[3] (periodoini)
                                            DateTime? periodoIniNullable = ParsearFechaSegura(fila[3]);
                                            if (periodoIniNullable.HasValue)
                                            {
                                                periodoini = periodoIniNullable.Value;
                                            }
                                            else
                                            {
                                                Console.WriteLine($"Advertencia: No se pudo parsear la fecha de inicio de periodo '{fila[3]}'. Se usará el valor predeterminado: {periodoini}");
                                                // Considera si este es el comportamiento deseado
                                            }

                                            // Procesar fila[4] (peridofin)
                                            DateTime? periodoFinNullable = ParsearFechaSegura(fila[4]);
                                            if (periodoFinNullable.HasValue)
                                            {
                                                peridofin = periodoFinNullable.Value;
                                            }
                                            else
                                            {
                                                Console.WriteLine($"Advertencia: No se pudo parsear la fecha de fin de periodo '{fila[4]}'. Se usará el valor predeterminado: {peridofin}");
                                                // Considera si este es el comportamiento deseado
                                            }



                                            //if (_datosTemporales != null)

                                            //    foreach (var fila in _datosTemporales)
                                            //    {

                                            //        {
                                            //            DateTime? fechaingresoreal = null; // Inicializar como null
                                            //            DateTime fechaingreso = DateTime.MinValue;
                                            //            DateTime periodoini = DateTime.MinValue;
                                            //            DateTime peridofin = DateTime.MinValue;
                                            //            DateTime fechaTemporal;
                                            //            //DateTime fechaTemporal = DateTime.MinValue; // O cualquier otro valor predeterminado que tenga sentido en tu contexto
                                            //            string[] formatos = { "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy", "yyyy/MM/dd H:mm:ss", "yyyy/MM/dd" };

                                            //            if (!string.IsNullOrEmpty(fila[1]))
                                            //            {

                                            //                DateTime.TryParseExact(fila[1], formatos, null, DateTimeStyles.None, out fechaTemporal);

                                            //            }



                                            //            if (!string.IsNullOrEmpty(fila[2]))
                                            //            {
                                            //                DateTime.TryParseExact(fila[2], formatos, null, DateTimeStyles.None, out fechaTemporal);
                                            //                fechaingreso = fechaTemporal;
                                            //            }

                                            //            fechaTemporal = DateTime.MinValue;
                                            //            if (!string.IsNullOrEmpty(fila[3]))
                                            //            {
                                            //                DateTime.TryParseExact(fila[3], formatos, null, DateTimeStyles.None, out fechaTemporal);
                                            //                periodoini = fechaTemporal;
                                            //            }

                                            //            fechaTemporal = DateTime.MinValue;
                                            //            if (!string.IsNullOrEmpty(fila[4]))
                                            //            {
                                            //                DateTime.TryParseExact(fila[4], formatos, null, DateTimeStyles.None, out fechaTemporal);
                                            //                peridofin = fechaTemporal;
                                            //            }

                                            var empleado = db.Empleados.AsEnumerable().FirstOrDefault(x => x.NroEmpleado == fila[0]);


                                                model.Add(new PeriodoVacacionesEmpleado()
                                                {
                                                    EmpleadoId = empleado.Id, // Suponiendo que el nombre está en la primera column
                                                    FechaIngresoReal = fechaingresoreal, // Suponiendo que el precio está en la segunda columna
                                                    FechaIngreso = fechaingreso,
                                                    PeriodoInicio=  periodoini,
                                                    PeriodoFin = peridofin,
                                                    Dias = Convert.ToInt32(fila[5]),// Suponiendo que la cantidad está en la tercera columna
                                                    FechaRegistro = DateTime.Now,
                                                    EmpleadoIdRegistra =  Convert.ToInt32(empleadoid),
                                                    DiasporDisfrutar = Convert.ToInt32(fila[6]),
                                                });
                                                

                                            }

                                        }

                                    db.PeriodoVacacionesEmpleado.AddRange(model);
                                    db.SaveChanges();

                                    message = String.Format("Archivo cargado correctamente.");
                                    Session["message"] = message;
                                  
                                    return View(model);



                                }


                            }

                        }




                    }
                }
                catch (Exception ex)
                {
                    string errorMessage = "Ocurrió un error:\n";
                    errorMessage += $"Mensaje: {ex.Message}\n";
                    errorMessage += $"StackTrace:\n{ex.StackTrace}\n";

                    Exception innerException = ex.InnerException;
                    while (innerException != null)
                    {
                        errorMessage += $"--- Excepción Interna ---\n";
                        errorMessage += $"Mensaje: {innerException.Message}\n";
                        errorMessage += $"StackTrace:\n{innerException.StackTrace}\n";
                        innerException = innerException.InnerException;
                    }


                    message = String.Format("Error al cargar el archivo: {0}", ex.Message);
                    Session["message"] =errorMessage;

                  
                }
            }
            else
            {
                message = String.Format("Por favor, selecciona un archivo Excel");
                Session["message"] = message;
               
            }



            return View();




        }




    }

}
