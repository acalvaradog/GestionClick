using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using SAP.Middleware.Connector;
using SAPExtractorDotNET;
using System.Text;
using System.Data.Entity;
using Adm_AutoGestion.Models.EvaDesempeno;
using System.Drawing.Printing;

namespace Adm_AutoGestion.Controllers
{
    public class IncapacidadesController : Controller
    {
        //
        // GET: /Incapacidades/
        private IncapacidadesRepository _repo;

        public IncapacidadesController()
        {
            _repo = new IncapacidadesRepository();

        }

        public ActionResult Index()
        {
            var opcion = "Index";

            var model = _repo.ObtenerTodos(opcion);
            return View(model);
        }

        //
        // GET: /Incapacidades/Details/5

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
        // GET: /Incapacidades/Create

        public ActionResult Create()
        {

            var model = new Incapacidades();
            using (var db = new AutogestionContext())
            {
                model.ListadoTiposInc = db.TiposIncapacidad.Where(e => e.EstadoId == 4).ToList();
                var diagnostico = db.Diagnostico.Select(e => new { e.Codigo, e.Nombre, nomcodigo = e.Codigo + "-" + e.Nombre }).ToList();

                ViewBag.diagnostico = diagnostico;

            }
            return View(model);
        }

        //
        // POST: /Incapacidades/Create

        [HttpPost]
        public ActionResult Create(Incapacidades model)
        {
            try
            {


                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {

                    _repo.Crear(model);
                    return RedirectToAction("Index");
                }

            }
            catch
            {

            }
            return View();
        }

        //
        // GET: /Incapacidades/Edit/5



        public ActionResult AprobarIncapacidad(string Id)
        {


            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("AprobarIncapacidad"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            int id = -1;
            string opcion = "Aprobar";
            Int32.TryParse(Id, out id);

            var model = _repo.ObtenerTodos(opcion);

            if (id > 0)
            {
                ViewBag.Incapacidades = model.FirstOrDefault(e => e.Id == id);
            }

            if (ViewBag.Incapacidades == null)
            {
                ViewBag.Incapacidades = new Models.Incapacidades();
                ViewBag.Incapacidades.Empleado = new Empleado();
                ViewBag.Incapacidades.ListadoEps = new EPS();
            }

            return View(model);
        }

        public ActionResult AprobarIncapacidad1(string Id, string Empleado, string Observacion, string Estado)
        {
            string message = null;

            try
            {

                // TODO: Add update logic here
                _repo.Modificar(Id, Empleado, Observacion, Estado);
                if (Estado == "3")
                { //Si la incapacidad se rechaza

                    using (var db = new AutogestionContext())
                    {
                        int empleadodatos = 0;
                        Int32.TryParse(Empleado, out empleadodatos);

                        Empleado empleado = db.Empleados.FirstOrDefault(e => e.Id == empleadodatos);
                        string nroEmpleadoDelJefe = empleado.Jefe;
                        string nroEmpleadoDelLider = empleado.Lider;


                        Empleado jefe = db.Empleados.FirstOrDefault(e => e.NroEmpleado == nroEmpleadoDelJefe);
                        Empleado lider = db.Empleados.FirstOrDefault(e => e.NroEmpleado == nroEmpleadoDelLider);
                        string correoLider = lider != null ? lider.Correo : null; // Cambia este correo por uno que consideres apropiado

                        //**************************************
                        if (notificar_incapacidad(empleado.Documento, empleado.Nombres, empleado.Correo, empleado.Empresa, Observacion) == false)
                        {
                            message = "Error al momento de enviar correo de notificación de rechazo de incapacidad a Empleado.";
                        }

                        if (notificar_incapacidad_rechazo_jefe(empleado.Documento, empleado.Nombres, jefe.Correo, empleado.Empresa, Observacion) == false)
                        {
                            message = "Error al momento de enviar correo de notificación de rechazo de incapacidad a Jefe.";
                        }

                        if (!String.IsNullOrWhiteSpace(correoLider))
                        {
                            if (notificar_incapacidad_rechazo_jefe(empleado.Documento, empleado.Nombres, correoLider, empleado.Empresa, Observacion) == false)
                            {
                                message = "Error al momento de enviar correo de notificación de rechazo de incapacidad a Lider.";
                            }
                        }



                    }

                }
                if (Estado == "2") //Si la incapacidad se aprueba
                {
                    using (var db = new AutogestionContext())
                    {
                        int empleadodatos = 0;
                        Int32.TryParse(Empleado, out empleadodatos);


                        Empleado empleado = db.Empleados.FirstOrDefault(e => e.Id == empleadodatos);

                        string nroEmpleadoDelJefe = empleado.Jefe;
                        int IdIncapacidad = 0;
                        Int32.TryParse(Id, out IdIncapacidad);

                        Empleado jefe = db.Empleados.FirstOrDefault(e => e.NroEmpleado == nroEmpleadoDelJefe);
                        Incapacidades incapacidad = db.Incapacidades.FirstOrDefault(e => e.Id == IdIncapacidad);

                        string FechaInicio = String.Format("{0:dd-MM-yyyy}", incapacidad.FechaInicio);
                        string FechaFin = String.Format("{0:dd-MM-yyyy}", incapacidad.FechaFin);

                        //**************************************
                        //notificar_incapacidad_jefe(empleado.Documento, empleado.Nombres, jefe.Correo, empleado.Empresa, FechaInicio, FechaFin);
                        if (notificar_incapacidad_empleado(empleado.Documento, empleado.Nombres, empleado.Correo, empleado.Empresa, FechaInicio, FechaFin) == false)
                        {
                            message = "Error al momento de enviar correo de notificación de aprobación de incapacidad a Empleado.";
                        }

                    }
                }

                if (message == null)
                {
                    string estadoDescripcion = string.Empty;
                    switch (Estado)
                    {
                        case "2":
                            estadoDescripcion = "aprobado";
                            break;
                        case "3":
                            estadoDescripcion = "rechazado";
                            break;
                        default:
                            estadoDescripcion = "procesado";
                            break;
                    }

                    message = $"Se ha {estadoDescripcion} el registro de incapacidad satisfactoriamente.";
                }

                Session["message"] = message;

                return RedirectToAction("AprobarIncapacidad");
            }
            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
                Session["message"] = message;
                return View();
            }
        }




        public ActionResult CargarAseguradora(string Id)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("CargarAseguradoraIncapacidades"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            int id = -1;
            string opcion = "Cargar";
            Int32.TryParse(Id, out id);

            var model = _repo.ObtenerTodos(opcion);

            if (id > 0)
            {
                ViewBag.Incapacidades = model.FirstOrDefault(e => e.Id == id);
            }

            if (ViewBag.Incapacidades == null)
            {
                ViewBag.Incapacidades = new Models.Incapacidades();
                ViewBag.Incapacidades.Empleado = new Empleado();
                ViewBag.Incapacidades.ListadoEps = new EPS();
            }

            return View(model);


        }

        public ActionResult CargarAseguradora1(string Id, string Empleado, string Observacion, string Prorroga, string Estado)
        {
            try
            {

                // TODO: Add update logic here
                _repo.Modificar(Id, Empleado, Observacion, Estado);
                return RedirectToAction("CargarAseguradora");
            }
            catch
            {
                return View();
            }
        }



        public ActionResult Adjunto(string Id, string Empleado, int IndJefe)
        {

            var model = _repo.ObtenerAdjuntos(Id, IndJefe);
            int empleado = 0;
            Int32.TryParse(Empleado, out empleado);
            using (var db = new AutogestionContext())
            {
                ViewBag.Empleado = db.Empleados.FirstOrDefault(e => e.Id == empleado);

            }


            return View(model);
        }

        //************************  Tabla Descarga Adjuntos ********************
        public string draw2 = "";
        public string start2 = "";
        public string length2 = "";
        public string sortColumn2 = "";
        public string sortColumnDir2 = "";
        public string searchValue2 = "";
        public int pageSize, skip, recordsTotal;
        [HttpPost]
        public ActionResult Tabla_Descarga_Json_Incapacidades(int Id, int Empleado, int IndJefe)
        {

            //logistica datatable
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();


            pageSize = length != null ? Convert.ToInt32(length) : 0;
            skip = start != null ? Convert.ToInt32(start) : 0;
            recordsTotal = 0;

            var IdIncapacidad = Convert.ToString(Id);
            var model = _repo.ObtenerAdjuntos(IdIncapacidad, IndJefe);
            recordsTotal = model.Count();

            /*int empleado = 0;
            Int32.TryParse(Empleado, out empleado);*/
            //conexión base de datos


            using (var db = new AutogestionContext())
            {
                ViewBag.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Empleado);

            }


            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = model, Empleado = ViewBag.Empleado });



        }
        //****************---------------------*****************************


        public FileResult Download(string archivo)
        {

            //byte[] fileBytes = System.IO.File.ReadAllBytes(@"C:\Users\mayra\Desktop\Adm_AutoGestion\Adm_AutoGestion\AnexosVacaciones\" + archivo);
            var fileBytes = Server.MapPath(@"..\AnexosIncapacidades\" + archivo);
            string fileName = archivo;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }


        public ActionResult Edit(int id)
        {
            return View();
        }


        public ActionResult InformeGeneral(string FechaIni, string FechaFin, string Empleado, string Estado, string Empresa, string CodigoEmpleado)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("InformeIncapacidadesGeneral"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

        
            List<Incapacidades> Items = new List<Incapacidades>();

            using (var db = new AutogestionContext())
            {
                Empleado empleado = new Empleado();
                var Empleadolog = Session["Empleado"];

                int IdEmpleado = 0;
                IdEmpleado = Convert.ToInt32(Empleadolog);

                empleado =  empleado = db.Empleados.FirstOrDefault(e => e.Id == IdEmpleado);


                ViewBag.Empleado = db.Empleados.Where(f => f.Activo != "NO"  && f.Empresa == empleado.Empresa).OrderBy(x => x.Nombres).ToList();
                ViewBag.Empresa =  db.Sociedad.Where(x => x.Codigo== empleado.Empresa).ToList();

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
                    //Fecha2 = DateTime.Now;
                    Fecha2 = DateTime.Now.AddYears(5);

                }

                if (Empleado != null || Empleado != "") {
                    int idemp = 0;
                    int.TryParse(Empleado, out idemp);
                   
                    if (idemp != 0) { 
                    var NombreEmpleado = db.Empleados.FirstOrDefault(x => x.Id == idemp);

                    Empleado = NombreEmpleado.Nombres;
                    }
                }

                if (estado == 0)
                {
                    Items = db.Incapacidades.Where(e => e.Empleado.Nombres.Contains(Empleado) &&
                                                e.Empleado.Empresa.Contains(Empresa) &&
                                                e.Empleado.NroEmpleado.Contains(CodigoEmpleado) &&
                                               DbFunctions.TruncateTime(e.FechaInicio) >= Fecha1 &&
                                               DbFunctions.TruncateTime(e.FechaFin) <= Fecha2).ToList();

                }
                if (estado != 0)
                {
                    Items = db.Incapacidades.Where(e => e.Empleado.Nombres.Contains(Empleado) &&
                                             e.Empleado.Empresa.Contains(Empresa) &&
                                             e.Empleado.NroEmpleado.Contains(CodigoEmpleado) &&
                                               DbFunctions.TruncateTime(e.FechaInicio) >= Fecha1 &&
                                               DbFunctions.TruncateTime(e.FechaFin) <= Fecha2 &&
                                               e.EstadoId == estado).ToList();
                }

                foreach (Incapacidades Item in Items)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.EstadosIncapacidades = db.EstadosIncapacidades.FirstOrDefault(x => x.Id == Item.EstadoId);
                    Item.PersonalActivo = db.PersonalActivo.FirstOrDefault(j => j.CodigoEmpleado == Item.Empleado.NroEmpleado);
                }
                ViewBag.EmpresaSeleccionada = Empresa;
                ViewBag.EstadoSeleccionado = Estado;

            }
            return View(Items);



        }

        public ActionResult InformeJefe(string FechaIni, string FechaFin, string Empleado, string Estado, string Empresa, string CodigoEmpleado, int IndVigentes = 0)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            var empleadoId = Convert.ToInt32(Session["Empleado"]);





            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("InformeIncapacidadesJefe"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }


            List<Incapacidades> Items = new List<Incapacidades>();

            using (var db = new AutogestionContext())
            {

                Empleado empleado = new Empleado();
                var Empleadolog = Session["Empleado"];

                int IdEmpleado = 0;
                IdEmpleado = Convert.ToInt32(Empleadolog);

                empleado =  empleado = db.Empleados.FirstOrDefault(e => e.Id == IdEmpleado);

                var NroUsuario = db.Empleados
                        .Where(e => e.Id == empleadoId)
                        .Select(e => e.NroEmpleado)
                        .FirstOrDefault();

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
                    //Fecha2 = DateTime.Now;
                    Fecha2 = DateTime.Now.AddYears(5);
                }

                //Para traer los datos de los empleados a cargo al cargar la página inicialmente
                /*Empleado = Empleado ?? "";
                Empresa = Empresa ?? "";
                CodigoEmpleado = CodigoEmpleado ?? "";*/

                ViewBag.EmpleadosJefe = db.Empleados.Where(e => e.Jefe == NroUsuario && e.Activo == "SI" && e.Empresa == empleado.Empresa || e.Lider == NroUsuario && e.NroEmpleado != NroUsuario && e.Activo == "SI" && e.Empresa == empleado.Empresa).OrderBy(x => x.Nombres).ToList();
                ViewBag.Empresa =  db.Sociedad.Where(x => x.Codigo== empleado.Empresa).ToList();
                var EmpleadosJ = db.Empleados.Where(e => e.Jefe == NroUsuario).Count();
                var EmpleadosL = db.Empleados.Where(e => e.Lider == NroUsuario).Count();
                DateTime hoy = DateTime.Today;

                if (EmpleadosJ > 0)
                {
                    if (estado == 0)
                    {
                        Items = db.Incapacidades.Where(e => e.Empleado.Nombres.Contains(Empleado) &&
                                                            e.Empleado.Empresa.Contains(Empresa) &&
                                                            e.Empleado.NroEmpleado.Contains(CodigoEmpleado) &&
                                                            e.Empleado.Jefe == NroUsuario && // Filtro adicional para empleados a cargo
                                                            DbFunctions.TruncateTime(e.FechaInicio) >= Fecha1 &&
                                                            DbFunctions.TruncateTime(e.FechaFin) <= Fecha2).ToList();
                    }
                    else // estado != 0
                    {
                        Items = db.Incapacidades.Where(e => e.Empleado.Nombres.Contains(Empleado) &&
                                                            e.Empleado.Empresa.Contains(Empresa) &&
                                                            e.Empleado.NroEmpleado.Contains(CodigoEmpleado) &&
                                                            e.Empleado.Jefe == NroUsuario && // Filtro adicional para empleados a cargo
                                                            DbFunctions.TruncateTime(e.FechaInicio) >= Fecha1 &&
                                                            DbFunctions.TruncateTime(e.FechaFin) <= Fecha2 &&
                                                            e.EstadoId == estado).ToList();
                    }
                    if (IndVigentes == 1)
                    {
                        Items = db.Incapacidades.Where(e => e.EstadoId == 2 &&
                                                            e.Empleado.Jefe == NroUsuario &&
                                                            (DbFunctions.TruncateTime(e.Fecha) >= hoy ||
                                                            DbFunctions.TruncateTime(e.FechaInicio) >= hoy ||
                                                            DbFunctions.TruncateTime(e.FechaFin) >= hoy)).ToList();

                    }
                }

                if (EmpleadosL > 0)
                {
                    if (estado == 0)
                    {
                        Items = db.Incapacidades.Where(e => e.Empleado.Nombres.Contains(Empleado) &&
                                                            e.Empleado.Empresa.Contains(Empresa) &&
                                                            e.Empleado.NroEmpleado.Contains(CodigoEmpleado) &&
                                                            e.Empleado.Lider == NroUsuario && // Filtro adicional para empleados a cargo
                                                            DbFunctions.TruncateTime(e.FechaInicio) >= Fecha1 &&
                                                            DbFunctions.TruncateTime(e.FechaFin) <= Fecha2).ToList();
                    }
                    else // estado != 0
                    {
                        Items = db.Incapacidades.Where(e => e.Empleado.Nombres.Contains(Empleado) &&
                                                            e.Empleado.Empresa.Contains(Empresa) &&
                                                            e.Empleado.NroEmpleado.Contains(CodigoEmpleado) &&
                                                            e.Empleado.Lider == NroUsuario && // Filtro adicional para empleados a cargo
                                                            DbFunctions.TruncateTime(e.FechaInicio) >= Fecha1 &&
                                                            DbFunctions.TruncateTime(e.FechaFin) <= Fecha2 &&
                                                            e.EstadoId == estado).ToList();
                    }
                    if (IndVigentes == 1)
                    {
                        Items = db.Incapacidades.Where(e => e.EstadoId == 2 &&
                                                            e.Empleado.Lider == NroUsuario &&
                                                            (DbFunctions.TruncateTime(e.Fecha) >= hoy ||
                                                            DbFunctions.TruncateTime(e.FechaInicio) >= hoy ||
                                                            DbFunctions.TruncateTime(e.FechaFin) >= hoy)).ToList();

                    }
                }



                foreach (Incapacidades Item in Items)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.EstadosIncapacidades = db.EstadosIncapacidades.FirstOrDefault(x => x.Id == Item.EstadoId);
                    Item.PersonalActivo = db.PersonalActivo.FirstOrDefault(j => j.CodigoEmpleado == Item.Empleado.NroEmpleado);
                }
                ViewBag.EmpresaSeleccionada = Empresa;
                ViewBag.EstadoSeleccionado = Estado;
                ViewBag.EmpleadoSeleccionado = Empleado;
                ViewBag.EmpresaEmpleado = Empresa;
                ViewBag.IndVigentes = IndVigentes;


            }
            return View(Items);



        }

        //
        // POST: /Incapacidades/Edit/5

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
        // GET: /Incapacidades/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Incapacidades/Delete/5

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


        public bool notificar_incapacidad(string cedula, string nombres, string email, string empresa, string observaciones)
        {

            string textocorreo = "";
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);


            using (var db = new AutogestionContext())
            {

                var texto = db.Configuraciones.First(s => s.Parametro == "TXTRECHAZOINCAP").Valor.ToString();
                textocorreo = texto;
                textocorreo = textocorreo.Replace("$observaciones$", observaciones);
            }



            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);


                correo.To.Add(email);


                correo.Subject = "Notificación Rechazo de Incapacidad";
                correo.Body = "Hola " + nombres + "," + "<BR><BR>" + textocorreo;
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

        public bool notificar_incapacidad_jefe(string cedula, string nombres, string email_jefe, string empresa, string FechaInicio, string FechaFin)
        {

            string textocorreo = "";
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);

            using (var db = new AutogestionContext())
            {

                var texto = db.Configuraciones.First(s => s.Parametro == "TXTAPRUEBAINCAPJEFE").Valor.ToString();
                textocorreo = texto;
                textocorreo = textocorreo.Replace("$nombres$", nombres);
                textocorreo = textocorreo.Replace("$FechaInicio$", FechaInicio);
                textocorreo = textocorreo.Replace("$FechaFin$", FechaFin);

            }


            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);


                correo.To.Add(email_jefe);


                correo.Subject = "Notificación incapacidad de empleado aprobada";


                correo.Body = textocorreo;
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

        public bool notificar_incapacidad_empleado(string cedula, string nombres, string email, string empresa, string FechaInicio, string FechaFin)
        {

            string textocorreo = "";
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);

            using (var db = new AutogestionContext())
            {

                var texto = db.Configuraciones.First(s => s.Parametro == "TXTINCAPAPRO").Valor.ToString();
                textocorreo = texto;
                textocorreo = textocorreo.Replace("$FechaInicio$", FechaInicio);
                textocorreo = textocorreo.Replace("$FechaFin$", FechaFin);

            }


            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);


                correo.To.Add(email);


                correo.Subject = "Notificación incapacidad aprobada";


                correo.Body = textocorreo;
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


        public bool notificar_incapacidad_rechazo_jefe(string cedula, string nombres, string email_jefe, string empresa, string observaciones)
        {

            string textocorreo = "";
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);

            using (var db = new AutogestionContext())
            {

                var texto = db.Configuraciones.First(s => s.Parametro == "TXTRECHAZOINCAPJEFE").Valor.ToString();
                textocorreo = texto;
                textocorreo = textocorreo.Replace("$nombres$", nombres);
                textocorreo = textocorreo.Replace("$documento$", cedula);
                textocorreo = textocorreo.Replace("$observaciones$", observaciones);

            }


            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);


                correo.To.Add(email_jefe);


                correo.Subject = "Notificación incapacidad de empleado rechazada";


                correo.Body = textocorreo;
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



    }
}