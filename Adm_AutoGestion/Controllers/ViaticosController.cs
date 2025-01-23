using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI.WebControls.Expressions;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace Adm_AutoGestion.Controllers
{
    public class ViaticosController : Controller
    {
        private AutogestionContext db = new AutogestionContext();
        private ViaticoRepository _repo;

        public ViaticosController()
        {
            _repo = new ViaticoRepository();
        }
        // GET: Viaticos
        public ActionResult Index()
        {
            return View();
        }

        // GET: Viaticos/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Viaticos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Viaticos/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Viaticos/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Viaticos/Edit/5
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

        // GET: Viaticos/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Viaticos/Delete/5
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

        public ActionResult JefeDirecto() 
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ViaticoJefeArea"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }


            var opcion = "JefeDirecto";
            var Empleadolog = Convert.ToString(Session["Empleado"]);
            var model = _repo.ObtenerTodos(opcion, Empleadolog);
            return View(model);


           
        }

        public ActionResult GestionHumana()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ViaticoGestionHumana"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var Empleadolog = Convert.ToString(Session["Empleado"]);
            var opcion = "GestionHumana";

            var model = _repo.ObtenerTodos(opcion, Empleadolog);
            return View(model);



        }
        public ActionResult Tesoreria()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ViaticoTesoreria"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var opcion = "Tesoreria";
            var Empleadolog = Convert.ToString(Session["Empleado"]);

            var model = _repo.ObtenerTodos(opcion, Empleadolog);
            return View(model);



        }
        public ActionResult Nomina()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ViaticoNominaContabilización"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var opcion = "Nomina";
            var Empleadolog = Convert.ToString(Session["Empleado"]);

            var model = _repo.ObtenerTodos(opcion, Empleadolog);
            return View(model);



        }

        public ActionResult NominaCargue()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ViaticoNominaCargue"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var opcion = "NominaCarge";
            var Empleadolog = Convert.ToString(Session["Empleado"]);

            var model = _repo.ObtenerTodos(opcion, Empleadolog);
            return View(model);



        }

        public ActionResult DetallesViatico(string Id)
        {
            var ViaticoId = 0;
            Int32.TryParse(Id, out ViaticoId);
            using (var db = new AutogestionContext())
            {
                Viaticos Items = new Viaticos();
                ViewBag.Viaticos = db.Viaticos.FirstOrDefault(e => e.Id == ViaticoId);
                var Viatico = _repo.ObtenerDetails(Id);
               
                Items = db.Viaticos.FirstOrDefault(e => e.Id == ViaticoId);
                Items.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Items.EmpleadoId);
                Items.DestinoViatico = db.DestinoViaticos.FirstOrDefault(e=>e.Id== Items.DestinoViaticoID);
                return PartialView(Items);
            }

        }
  
        public JsonResult RespuestajsonJefe()
        {
           
            int id = 0;
            var IdSolicitud = HttpContext.Request.Params["Id"];
            var Estado = HttpContext.Request.Params["Estado"];
            var Observaciones = HttpContext.Request.Params["Observaciones"];
            Int32.TryParse(IdSolicitud, out id);
            Viaticos viatico = new Viaticos();
            viatico= db.Viaticos.FirstOrDefault(o => o.Id == id);
            var idUserlog= Convert.ToInt32(Session["Empleado"]);
            Empleado userlog = db.Empleados.Where(d=>d.Id== idUserlog).FirstOrDefault();
            var Fechaactual = DateTime.Now;

            var respuesta = "";
            string message = null;
            try
            {
                var url = Url.Action("JefeDirecto");
                using (var db = new AutogestionContext())
                {
                   
                    if (id != 0 )
                    {
                        if (Estado!="Seleccionado...") 
                        {
                            if (Estado== "Aprobar") 
                            {
                                Estado = "2";
                            }
                            if (Estado == "Denegar") 
                            {
                                Estado = "4";
                            }

                           var a = _repo.Modificar(id, Estado, Observaciones);
                            if (a==true)
                            {
                                var Opcion = "JefeDirecto";
                                Viaticos viaticos = db.Viaticos.Where(x=>x.Id ==id).FirstOrDefault();
                                DestinoViatico Destino= db.DestinoViaticos.Where(x => x.Id == viaticos.DestinoViaticoID).FirstOrDefault();
                             
                                Empleado Emp = db.Empleados.Where(x=>x.Id == viaticos.EmpleadoId).FirstOrDefault();
                                _repo.GenerarLog(id, userlog.Id , Convert.ToInt32(Estado), Opcion, Observaciones);
                                if (Estado=="4") 
                                { _repo.notificar_Cierre(id, Emp.Correo, viatico.EmpleadoId, viatico.FechaInicio, viatico.MtvViaje, "Denegado por el Jefe Directo porque: '" + Observaciones, Destino.Nombre+"'"); }
                                if (Estado == "2")
                                {
                                    var Correo = db.Configuraciones.Where(x => x.Parametro == "EMAILGESTIONHUMANA").FirstOrDefault();
                                    _repo.notificar_Solicitud(id, Correo.Valor, viatico.EmpleadoId, viaticos.FechaInicio, viatico.MtvViaje, "AprovadoJefe");
                                    //_repo.NotificarARL(viaticos.Id, viaticos.EmpleadoId, viaticos.FechaInicio, viaticos.FechaFin);
                                    //if (viaticos.Hospedaje == true)
                                    //{ _repo.NotificarBienestar(viaticos.Id, viaticos.EmpleadoId, viaticos.FechaInicio, viaticos.FechaFin); }


                                }

                            }
                            else 
                            {
                       
                                return Json(new
                                {
                                    redirectUrl = url,
                                    isRedirect = true,
                                    respuesta = "Guardado Exitoso"
                                });
                            }

                        }                    

                    }

                }

                
                
                return Json(new
                {
                    redirectUrl = url,
                    isRedirect = true,
                    respuesta = "Guardado Exitoso"
                });
            }


            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
                Session["message"] = message;
                respuesta = "Error durante el guardado";
                return Json(new { respuesta });
            }



            

        }

        public ActionResult DetalleViaticoGestion(string Id)
        {
            var ViaticoId = 0;
            Int32.TryParse(Id, out ViaticoId);
            using (var db = new AutogestionContext())
            {
                Viaticos Items = new Viaticos();
                ViewBag.Viaticos = db.Viaticos.FirstOrDefault(e => e.Id == ViaticoId);
                var Viatico = _repo.ObtenerDetails(Id);

                Items = db.Viaticos.FirstOrDefault(e => e.Id == ViaticoId);
                Items.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Items.EmpleadoId);
                Items.DestinoViatico = db.DestinoViaticos.FirstOrDefault(e => e.Id == Items.DestinoViaticoID);
                return PartialView(Items);
            }

        }

        public JsonResult RespuestajsonGestion()
        {
            
            int id = 0;
            var IdViatico = HttpContext.Request.Params["IdViatico"];
            var Estado = HttpContext.Request.Params["Estado"];
            var GastosA = HttpContext.Request.Params["GastoAlimentacion"];
            var GastosT = HttpContext.Request.Params["GastoTransporte"];
            var Empleadolog = Convert.ToString(Session["Empleado"]);
            var Observaciones = HttpContext.Request.Params["Observaciones"];


            var Fechaactual = DateTime.Now;



            Int32.TryParse(IdViatico, out id);
            Viaticos viatico = new Viaticos();
            viatico = db.Viaticos.FirstOrDefault(o => o.Id == id);
            Empleado userlog = db.Empleados.Where(d => d.Id == viatico.EmpleadoId).FirstOrDefault();

            var respuesta = "";
            string message = null;
            try
            {
                using (var db = new AutogestionContext())
                {
                    if (id != 0)
                    {
                        if (Estado != "Seleccionado...")
                        {
                            if (Estado == "Aprobar")
                            {
                                Estado = "3";
                            }
                            if (Estado == "Denegar")
                            {
                                Estado = "4";
                            }
                            if (GastosA=="" || GastosT=="")
                            {
                                throw new ArgumentNullException(paramName:"Error", message: "Gastos de Alimentación y Gatos de Trasnporte son obligatorias");
                            }
                            var a =_repo.ModificarGestion(id, Estado, GastosA, GastosT, Empleadolog, Observaciones);
                           
                            if (a == true)
                            {
                                DestinoViatico Destino = db.DestinoViaticos.Where(x => x.Id == viatico.DestinoViaticoID).FirstOrDefault();
                                var observacion = "";
                                if (Estado=="3") 
                                { observacion = "aprobado por Gestión Humana";
                                    _repo.NotificarNóminaContabilizacion(viatico, viatico.EmpleadoId);
                                    _repo.NotificarARL(viatico.Id, viatico.EmpleadoId, viatico.FechaInicio, viatico.FechaFin);
                                    if (viatico.Hospedaje == true)
                                    { _repo.NotificarBienestar(viatico.Id, viatico.EmpleadoId, viatico.FechaInicio, viatico.FechaFin); }

                                }
                                if (Estado == "4") 
                                { observacion = "denegado por Gestión Humana porque: " + Observaciones; }
                                _repo.notificar_Cierre(id, userlog.Correo, viatico.EmpleadoId, Fechaactual, viatico.MtvViaje, observacion, Destino.Nombre);
                             
                                
                            }
                        }

                    }

                }

                var url = Url.Action("GestionHumana");

                return Json(new
                {
                    redirectUrl = url,
                    isRedirect = true,
                    respuesta = "Guardado Exitoso"
                });
            } catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
                Session["message"] = message;
                respuesta =  message;
                return Json(new { respuesta });
            }





        }

        public ActionResult DetallesViaticoTes(string Id)
        {
          
          
            var ViaticoId2 = 0;
         
            Int32.TryParse(Id, out ViaticoId2);
            using (var db = new AutogestionContext())
            {
                
                Viaticos Items = new Viaticos();
                ViewBag.Viaticos = db.Viaticos.FirstOrDefault(e => e.Id == ViaticoId2);
 
                


                Items = db.Viaticos.FirstOrDefault(e => e.Id == ViaticoId2);
                Items.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Items.EmpleadoId);
                Items.DestinoViatico = db.DestinoViaticos.FirstOrDefault(e => e.Id == Items.DestinoViaticoID);
                return PartialView(Items);
            }

        }

        public ActionResult DetallesViaticoNom(string Id)
        {


            var ViaticoId2 = 0;

            Int32.TryParse(Id, out ViaticoId2);
            using (var db = new AutogestionContext())
            {

                Viaticos Items = new Viaticos();
                ViewBag.Viaticos = db.Viaticos.FirstOrDefault(e => e.Id == ViaticoId2);




                Items = db.Viaticos.FirstOrDefault(e => e.Id == ViaticoId2);
                Items.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Items.EmpleadoId);
                Items.DestinoViatico = db.DestinoViaticos.FirstOrDefault(e => e.Id == Items.DestinoViaticoID);
                return PartialView(Items);
            }

        }
        public ActionResult DetallesViaticoNomCargue(string Id)
        {


            var ViaticoId2 = 0;

            Int32.TryParse(Id, out ViaticoId2);
            using (var db = new AutogestionContext())
            {

                Viaticos Items = new Viaticos();
                ViewBag.Viaticos = db.Viaticos.FirstOrDefault(e => e.Id == ViaticoId2);




                Items = db.Viaticos.FirstOrDefault(e => e.Id == ViaticoId2);
                Items.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Items.EmpleadoId);
                Items.DestinoViatico = db.DestinoViaticos.FirstOrDefault(e => e.Id == Items.DestinoViaticoID);
                return PartialView(Items);
            }

        }

        public JsonResult RespuestajsonTes()
        {

            int id = 0;
            var IdSolicitud = HttpContext.Request.Params["Id"];
            var check = HttpContext.Request.Params["check"];
            var Empleadolog = Convert.ToInt32(Session["Empleado"]);

            Int32.TryParse(IdSolicitud, out id);
            Viaticos viatico = new Viaticos();
            viatico = db.Viaticos.FirstOrDefault(o => o.Id == id);

            string CodContabilizacionSAP = "";
            var respuesta = "";
            string message = null;
            string url = null;
            try
            {
                using (var db = new AutogestionContext())
                {
                    if (id != 0)
                    {
                        if (check == "Nomina" || check== "Tesoreria") { 

                            _repo.ModificarTesNom(id, check, Empleadolog, CodContabilizacionSAP);

                        }
                   

                    }

                }
                if (check == "Nomina") { url = Url.Action("Nomina"); }
                if (check == "Tesoreria") { url = Url.Action("Tesoreria"); }


                return Json(new
                {
                    redirectUrl = url,
                    isRedirect = true,
                    respuesta = "Guardado Exitoso"
                });
            }


            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
                Session["message"] = message;
                respuesta = "Error durante el guardado";
                return Json(new { respuesta });
            }





        }

        public JsonResult RespuestajsonNom()
        {

            int id = 0;
            string IdSolicitud = HttpContext.Request.Params["Id"];
            string check = HttpContext.Request.Params["check"];
            var Empleadolog = Convert.ToInt32(Session["Empleado"]);
            string CodContabilizacionSAP = HttpContext.Request.Params["CodContabilizacionSAP"];

            Int32.TryParse(IdSolicitud, out id);
            Viaticos viatico = new Viaticos();
            viatico = db.Viaticos.FirstOrDefault(o => o.Id == id);


            var respuesta = "";
            string message = null;
            string url = null;
            try
            {
                using (var db = new AutogestionContext())
                {
                    if (id != 0)
                    {
                        if (check == "Nomina" || check == "Tesoreria" || check == "NominaCargue")
                        {

                            _repo.ModificarTesNom(id, check, Empleadolog, CodContabilizacionSAP);

                        }


                    }

                }
                if (check == "Nomina") { url = Url.Action("Nomina"); }
                if (check == "Tesoreria") { url = Url.Action("Tesoreria"); }
                if (check == "NominaCargue") { url = Url.Action("NominaCargue"); }
                


                return Json(new
                {
                    redirectUrl = url,
                    isRedirect = true,
                    respuesta = "Guardado Exitoso"
                });
            }


            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
                Session["message"] = message;
                respuesta = "Error durante el guardado";
                return Json(new { respuesta });
            }





        }

        public ActionResult ViaticosLogDetails(string Id)
        {


            var ViaticoId2 = 0; 

            Int32.TryParse(Id, out ViaticoId2);
            using (var db = new AutogestionContext())
            {

                List<ViaticosLog> Items = new List<ViaticosLog>();
                List<ViaticosLog> Items2 = new List<ViaticosLog>();
               Items = db.ViaticosLogs.Where(e => e.ViaticoId == ViaticoId2).ToList();
                foreach (var item in Items) 
                {                  
                    item.Empleado = db.Empleados.FirstOrDefault(e => e.NroEmpleado == item.Usuario);              
                }

                Items2 = db.ViaticosLogs.Where(e => e.ViaticoId == ViaticoId2).ToList();
                
                return PartialView(Items);
            }

        }

        public ActionResult InformeViaticos(string JefeID, string TrabajadorS, string FechaI, string FechaF, string Estado, string NmrRegistro)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            
         
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ViaticosInforme"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var Empleadolog = Session["Empleado"];
            var EstadoId = 0;
            int JefeID2 = 0;
            if (JefeID == null || JefeID == "")
            {
                JefeID2 = Convert.ToInt32(Empleadolog);
            }
            List<Viaticos> Proceso = new List<Viaticos>();
          

            using (var db = new AutogestionContext())
            {

                //----------------List Empleados------------------------------//
                var Jefe = db.Empleados.FirstOrDefault(e => e.Id == JefeID2);
                ViewBag.Empleado = db.Empleados.Where(f => f.Activo != "NO" && f.Jefe == Jefe.NroEmpleado).ToList();

                DateTime Fecha1 = DateTime.Now;
                DateTime Fecha2 = DateTime.Now;

                List<SelectListItem> lst = new List<SelectListItem>();
                Int32 IdProceso = new Int32();

                //------------Denifir variable EstadoId------------------------//
                if (Estado == "Solicitado") { EstadoId = 1; }
                else if(Estado == "Aprobado Jefe directo") { EstadoId = 2; }
                else if (Estado == "Cerrado") { EstadoId = 3; }
                else if (Estado == "Rechazado") { EstadoId = 4; };


                if (NmrRegistro != "")
                {
                    IdProceso = Convert.ToInt32(NmrRegistro);
                }

                if (!DateTime.TryParse(FechaI, out Fecha1))
                {
                    Fecha1 = new DateTime();
                }

                if (!DateTime.TryParse(FechaF, out Fecha2))
                {
                    Fecha2 = DateTime.Now;
                }

                if (TrabajadorS == "")
                {
                    if (Estado == "Todos")
                    {
                        Proceso = db.Viaticos.Where(e => DbFunctions.TruncateTime(e.FechaRegistro) >= Fecha1 &&
                                          DbFunctions.TruncateTime(e.FechaRegistro) <= Fecha2
                                         
                                          && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrRegistro)
                                          ).ToList();
                    }                  
                    else
                    {
                        Proceso = db.Viaticos.Where(e => DbFunctions.TruncateTime(e.FechaRegistro) >= Fecha1 &&
                                     e.Estado == EstadoId
                                     &&
                                     DbFunctions.TruncateTime(e.FechaRegistro) <= Fecha2                                   
                                     && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrRegistro)
                                     ).ToList();
                    }
                    
                }
                if (TrabajadorS != "")
                {

                    if (Estado == "Todos")
                    {
                        int id = -1;
                        Int32.TryParse(TrabajadorS, out id);
                       
                        
                        int[] Ids = (from item in db.Viaticos where item.EmpleadoId.Equals(id) select item.Id).ToArray();
                       
                        Proceso = db.Viaticos.Where(e =>
                                    Ids.Contains(e.Id) && e.FechaRegistro >= Fecha1 && e.FechaRegistro <= Fecha2
                                    &&
                                    SqlFunctions.StringConvert((decimal)e.EmpleadoId).Contains(TrabajadorS)  && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrRegistro)).ToList();

                    }
                        else
                        {
                        int id = -1;
                        Int32.TryParse(TrabajadorS, out id);
                        int[] Ids = (from item in db.Viaticos where item.EmpleadoId.Equals(id) select item.Id).ToArray();

                        Proceso = db.Viaticos.Where(e =>
                                    Ids.Contains(e.Id) && e.FechaRegistro >= Fecha1 && e.FechaRegistro <= Fecha2
                                    &&
                                    SqlFunctions.StringConvert((decimal)e.EmpleadoId).Contains(TrabajadorS) && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrRegistro)
                                    &&
                                    e.Estado == EstadoId).ToList();



                    }

                }
                List<Empleado> Empleados_Jefe =  db.Empleados.Where(x=> x.Jefe == Jefe.NroEmpleado && x.Activo=="SI").ToList();
                foreach (Viaticos Item in Proceso.Reverse<Viaticos>())
                {
                    var x = Item.Id;
                    string Id = "" + x;
                    var Implicados = _repo.ObtenerTodos2(Id);
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    var Emp = Empleados_Jefe.Where(s => s.Id == Item.EmpleadoId).FirstOrDefault();                 
                    Item.EstadosViaticos = db.EstadosViaticos.FirstOrDefault(e => e.id == Item.Estado);
                    if (Emp == null)
                    {
                        Proceso.Remove(Item);
                    }
                }



            }
            return View(Proceso);

        }

        public ActionResult InformeViaticosGH(string EmpleadoCP, string TrabajadorS, string FechaI, string FechaF, string Estado, string NmrRegistro)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);


            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ViaticoGestionHumanaInforme"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var Empleadolog = Session["Empleado"];
            var EstadoId = 0;

            if (EmpleadoCP == null || EmpleadoCP == "")
            {
                EmpleadoCP = Convert.ToString(Empleadolog);
            }
            List<Viaticos> Proceso = new List<Viaticos>();


            using (var db = new AutogestionContext())
            {

                //----------------List Empleados------------------------------//
                int Usuario = 0;
                var usuariolog = String.Format("{0}", Session["Empleado"]);
                Int32.TryParse(usuariolog, out Usuario);
                var Jefe = db.Empleados.FirstOrDefault(e => e.Id == Usuario);
                ViewBag.Empleado = db.Empleados.Where(f => f.Activo != "NO").ToList();

                DateTime Fecha1 = DateTime.Now;
                DateTime Fecha2 = DateTime.Now;

                List<SelectListItem> lst = new List<SelectListItem>();
                Int32 IdProceso = new Int32();

                //------------Denifir variable EstadoId------------------------//
                if (Estado == "Solicitado") { EstadoId = 1; }
                else if (Estado == "Aprobado Jefe directo") { EstadoId = 2; }
                else if (Estado == "Cerrado") { EstadoId = 3; }
                else if (Estado == "Rechazado") { EstadoId = 4; };


                if (NmrRegistro != "")
                {
                    IdProceso = Convert.ToInt32(NmrRegistro);
                }

                if (!DateTime.TryParse(FechaI, out Fecha1))
                {
                    Fecha1 = new DateTime();
                }

                if (!DateTime.TryParse(FechaF, out Fecha2))
                {
                    Fecha2 = DateTime.Now;
                }

                if (TrabajadorS == "")
                {
                    if (Estado == "Todos")
                    {
                        Proceso = db.Viaticos.Where(e => DbFunctions.TruncateTime(e.FechaRegistro) >= Fecha1 &&
                                          DbFunctions.TruncateTime(e.FechaRegistro) <= Fecha2

                                          && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrRegistro)
                                          ).ToList();
                    }
                    else
                    {
                        Proceso = db.Viaticos.Where(e => DbFunctions.TruncateTime(e.FechaRegistro) >= Fecha1 &&
                                     e.Estado == EstadoId
                                     &&
                                     DbFunctions.TruncateTime(e.FechaRegistro) <= Fecha2
                                     && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrRegistro)
                                     ).ToList();
                    }

                }
                if (TrabajadorS != "")
                {

                    if (Estado == "Todos")
                    {
                        int id = -1;
                        Int32.TryParse(TrabajadorS, out id);


                        int[] Ids = (from item in db.Viaticos where item.EmpleadoId.Equals(id) select item.Id).ToArray();

                        Proceso = db.Viaticos.Where(e =>
                                    Ids.Contains(e.Id) && e.FechaRegistro >= Fecha1 && e.FechaRegistro <= Fecha2
                                    &&
                                    SqlFunctions.StringConvert((decimal)e.EmpleadoId).Contains(TrabajadorS) && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrRegistro)).ToList();

                    }
                    else
                    {
                        int id = -1;
                        Int32.TryParse(TrabajadorS, out id);
                        int[] Ids = (from item in db.Viaticos where item.EmpleadoId.Equals(id) select item.Id).ToArray();

                        Proceso = db.Viaticos.Where(e =>
                                    Ids.Contains(e.Id) && e.FechaRegistro >= Fecha1 && e.FechaRegistro <= Fecha2
                                    &&
                                    SqlFunctions.StringConvert((decimal)e.EmpleadoId).Contains(TrabajadorS) && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrRegistro)
                                    &&
                                    e.Estado == EstadoId).ToList();



                    }

                }
                foreach (Viaticos Item in Proceso)
                {
                    var x = Item.Id;
                    string Id = "" + x;
                    var Implicados = _repo.ObtenerTodos2(Id);
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);

                    Item.EstadosViaticos = db.EstadosViaticos.FirstOrDefault(e => e.id == Item.Estado);
                }



            }
            return View(Proceso);

        }

        public ActionResult PlacasVinculadas(string Id) 
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);


            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            //else if (!Acceso.EsAnonimo && !funciones.Contains("ViaticoGestionHumana"))
            //{
            //    Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
            //    return RedirectToAction("Index", "Login");
            //}
            int IdINT =Convert.ToInt32(Id);
            Viaticos ViaticoOrg = db.Viaticos.Where(x=>x.Id == IdINT).FirstOrDefault();
            List<Viaticos> Viaticoslist = db.Viaticos.Where(x=>x.Placa== ViaticoOrg.Placa && x.FechaInicio == ViaticoOrg.FechaInicio && x.Estado==1).ToList();

            return PartialView(Viaticoslist);
        }

        public ActionResult ConfirmacionViaje(string cadena)
        {
            string Respuesta = "";
            try
            {
                if (cadena != null) 
                {
                    Respuesta = _repo.VerificarViaje(cadena);
                }
                
                if (Respuesta== "Confirmación exitosa") 
                {
                     ViewBag.Respuesta = "true";
                }
                else if (Respuesta == "ViajeRealizado")
                {
                    ViewBag.Respuesta = "viajerealizado";
                }
                else
                {
                    ViewBag.Respuesta = "false";
                }
                return View();
            } 
            catch
            {
                return View();
            }
            
        }
        [HttpGet]  
        public ActionResult ConfirmarViajeJefe (Viaticos Viatico) {
            try
            {
                ViewBag.ViaticosId = Viatico.Id;
                return PartialView(Viatico);
            }
            catch (SystemException ex)
            { 
                return PartialView(Viatico);
            }   
        }
        [HttpPost]
        public ActionResult ConfirmarViajeJefe2(string ViajeRealizado , string Observaciones, int Id)
        {
            string respuesta = "";
            try
            {
                bool ViajeR = false;
                string IdJefe = Convert.ToString(Session["Empleado"]);
                if (ViajeRealizado =="" || ViajeRealizado == null) 
                {
                    respuesta = "Falta definir si se ha realizado el viaje";
                }
                else 
                {
                    if (ViajeRealizado=="Si") 
                    {
                        ViajeR = true;
                    }
                    else 
                    {
                        ViajeR=false;
                    }
                }
                if (Observaciones =="" || Observaciones ==null) 
                {
                    respuesta = "Falta definir una obsevación";
                }
                if (Id == 0 || Id == null)
                {
                    respuesta = "Error en en la seleccion del viatico";
                }
                if (respuesta !="") 
                {
                    Session["message"] = respuesta;
                }
                else 
                {
                    _repo.VerificarViajeManual(Id, Observaciones, ViajeR, IdJefe);
                }

                return RedirectToAction("InformeViaticos");
            }
            catch (SystemException ex)
            {
                respuesta =""+ ex;
                return RedirectToAction("InformeViaticos");
            }
        }
        public ActionResult AnularViajeModal(Viaticos Viatico)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);


            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            string respuesta = "";
            try
            {
                //Viaticos Viatico = db.Viaticos.Where(x => x.Id == Id).FirstOrDefault();
                Empleado Emp = db.Empleados.Where(x => x.Id == Viatico.EmpleadoId).FirstOrDefault();
                DestinoViatico Destino =db.DestinoViaticos.Where(x => x.Id == Viatico.DestinoViaticoID).FirstOrDefault();
                Viatico.Empleado = Emp;
                Viatico.Destino = Destino.Nombre;
                ViewBag.Viatico = Viatico;




                return PartialView(Viatico);
            }
            catch (SystemException ex)
            {
                respuesta = "" + ex;
                return PartialView();
            }
        }
        [HttpPost]
        public ActionResult AnularViaje(int Id)
        {
            string respuesta = "";
            try
            {
                bool ViajeR = false;
                string IdJefe = Convert.ToString(Session["Empleado"]);
             
                if (Id == 0 || Id == null)
                {
                    respuesta = "Error en en la seleccion del viatico";
                }
                if (respuesta != "")
                {
                    Session["message"] = respuesta;
                }
                else
                {
                    _repo.AnularViaje(Id, IdJefe);
                }

                return RedirectToAction("InformeViaticos");
            }
            catch (SystemException ex)
            {
                respuesta = "" + ex;
                return RedirectToAction("InformeViaticos");
            }
        }

       

    }
}
