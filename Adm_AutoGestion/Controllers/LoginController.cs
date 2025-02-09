using Adm_AutoGestion.Controllers.Api;
using Adm_AutoGestion.Models;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using static System.Net.WebRequestMethods;

namespace Adm_AutoGestion.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        //private class indicator
        //{
        //    public int count { get; set; }
        //    public string type { get; set; }
        //    public string var { get; set; }
        //}

        public ActionResult Index(string message) {
                        
           ViewBag.Message = message;
            return View();
        }


        //[HttpPost]
        //public JsonResult Index()
        //{
        //    List<indicator> Items = new List<indicator>();

        //    using (AutogestionContext db = new AutogestionContext())
        //    {
        //        var Transporte = db.EncabezadoEncuesta.GroupBy(e => e.Transporte).Select(e => new { Name = e.Key, Count = e.Count() }).ToArray();
        //        var ModoTrabajo = db.EncabezadoEncuesta.GroupBy(e => e.ModoTrabajo).Select(e => new { Name = e.Key, Count = e.Count() }).ToArray();
        //        var Sospechoso = db.EncabezadoEncuesta.GroupBy(e => e.Sospechoso).Select(e => new { Name = e.Key, Count = e.Count() }).ToArray();

        //        foreach (var item in Transporte) { Items.Add(new indicator() { count = item.Count, type = "Transporte", var = item.Name }); }
        //        foreach (var item in ModoTrabajo) { Items.Add(new indicator() { count = item.Count, type = "Modo de trabajo", var = item.Name }); }
        //        foreach (var item in Sospechoso) { Items.Add(new indicator() { count = item.Count, type = "Sospechoso", var = item.Name == true ? "Si" : "No" }); }
        //    }

        //    return Json(Items.ToArray());
        //}

        public ActionResult Login(string message = "") {
            Session.Clear();
            ViewBag.Message = message;

            return View();
        }
        [HttpGet]
        public ActionResult Login2(string Codigo , string documento) 
        {
            try
            {
                using (var db = new AutogestionContext()) 
                {
                    ViewBag.Usuarios = db.Empleados.Where(x=>x.Documento== documento).ToList();
                }
               
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", new { message = "Sin datos" });
            }
            
        }

        [HttpPost]
        public ActionResult Login2(string Codigo)
        {
            try
            {
                // TODO: Add insert logic here
                if (Codigo != null && Codigo != "")
                {
                    AutogestionContext db = new AutogestionContext();
                    var user = db.Empleados.FirstOrDefault(e => e.NroEmpleado == Codigo.Trim() && e.Activo == "SI");
                    Session.Add("Empleado", user.Id);
                    Session.Add("Empresa", user.Empresa);
                    Session.Add("NombreEmpleado", user.Nombres);//Nombre usuario nav derecho
                    return RedirectToAction("Dashboard", "Login", new { Inicio = 1 });
                }

                Session.Add("message", "No se encontro Rol asignado");
               // return RedirectToAction("Index");
               return RedirectToAction("Index", new { message = "No se encontro Rol asignado" });
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Logout() {
            Session.Clear();
            return View("Login");
        }

        public ActionResult Validar(string alias, string password, string accion)
        {
            if (!string.IsNullOrEmpty(alias) && !string.IsNullOrEmpty(password))
            {
                AutogestionContext db = new AutogestionContext();
                var multiusuarios = db.Empleados.Where(e => e.Documento == alias.Trim() && e.Activo == "SI").ToList();
                var user = db.Empleados.FirstOrDefault(e => e.Documento == alias.Trim() && e.Contraseña == password.Trim() && e.Activo =="SI");

                
                if (user != null)
                {
                    if (accion == "permisos")
                    {
                        Session.Add("Empleado", user.Id);
                        Session.Add("Empresa", user.Empresa);
                        Session.Add("NombreEmpleado", user.Nombres);//Nombre usuario nav derecho
                        return RedirectToAction("AprobacionPer", "Permisos");
                    }
                    if (multiusuarios.Count <= 1)
                    {
                        Session.Add("Empleado", user.Id);
                        Session.Add("Empresa", user.Empresa);
                        Session.Add("NombreEmpleado", user.Nombres);//Nombre usuario nav derecho

                        return RedirectToAction("Dashboard", "Login", new { Inicio = 1 });
                    }
                    else
                    {




                        return RedirectToAction("Login2", "Login", new { Codigo = "", documento = alias.Trim() });
                    }
                    

                }
                else
                {
                    ViewBag.message = "Nro de Documento o Contraseña incorrectos.";
                    Session.Add("message", "Nro de Documento o Contraseña incorrectos.");
                    return RedirectToAction("Login");

                }
            }
            else
            {
                return RedirectToAction("Login", new { message = "Sin datos" });
            }
            
        }



        //Graficass
        private class indicator
        {
            public string type { get; set; }
            public int countsi { get; set; }
            public int countno { get; set; }
            //public string fecha { get; set; }
        }
 

        public ActionResult Dashboard(string Inicio)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("DashboardPendientesJefe"))
            {
                if (Inicio != "1")
                {
                    Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                }

                //Session.Add("message", "No se encontro rol asignado");
                return RedirectToAction("Index", "Login");
            }

            string empleado = String.Format("{0}", Session["Empleado"]);
            int empl = 0;
            Int32.TryParse(empleado, out empl);
            Empleado datos = new Empleado();
            
            EvaDesempenoController EvaDesempeño= new EvaDesempenoController();

            using (var db = new AutogestionContext())
            {
                datos = db.Empleados.Find(empl);
                DateTime hoy = DateTime.Today;

                var EmpleadosJ = db.Empleados.Where(e => e.Jefe == datos.NroEmpleado).Count();
                var EmpleadosL = db.Empleados.Where(e => e.Lider == datos.NroEmpleado).Count();

                ViewBag.CntVacaciones = db.Vacaciones.Where(e => e.EstadoId == 1 &&  e.Empleado.Jefe == datos.NroEmpleado).Count();
                ViewBag.CntHorasExtra = db.HorasExtra.Where(e => e.Estado == 1 && e.Empleado.Jefe == datos.NroEmpleado).Count();
                if (EmpleadosJ > 0)
                {
                    ViewBag.CntIncapacidades = db.Incapacidades.Where(e => e.EstadoId == 2 && e.Empleado.Jefe == datos.NroEmpleado && (DbFunctions.TruncateTime(e.Fecha) >= hoy || DbFunctions.TruncateTime(e.FechaInicio) >= hoy || DbFunctions.TruncateTime(e.FechaFin) >= hoy)).Count();

                }
                if (EmpleadosL > 0)
                {
                    ViewBag.CntIncapacidades = db.Incapacidades.Where(e => e.EstadoId == 2 && e.Empleado.Lider == datos.NroEmpleado && (DbFunctions.TruncateTime(e.Fecha) >= hoy || DbFunctions.TruncateTime(e.FechaInicio) >= hoy || DbFunctions.TruncateTime(e.FechaFin) >= hoy)).Count();

                }
                ViewBag.CntPermisos = db.Permisos.Where(e => e.EstadoId == 1 &&  e.Empleado.Jefe == datos.NroEmpleado).Count();
                ViewBag.CntProcesosDisc = db.ProcesoDisciplinario.Where(x => x.Estado == "Cerrado" && x.Sanciones == "Suspension" && x.EmpleadoRegistraId == datos.Id && x.FechaSuspencion == null ).Count();
                ViewBag.CntViaticos = db.Viaticos.Where(x => x.Estado == 1 && x.Empleado.Jefe == datos.NroEmpleado).Count();
                //List<EvaDesempenoController.RestEmp> listEva = EvaDesempeño.ConsultaEmpleado(datos.NroEmpleado,"");
                //ViewBag.CntEvaDesempeñoEnt = listEva.Where(x=>x.EstadoE== 1).Count();
                //ViewBag.CntEvaDesempeñoPer = listEva.Where(x => x.EstadoP == 1).Count();




            }
            return View();
        }

        //[HttpPost]
        //public JsonResult DatosGraficas(string fecha)
        //{
        //    List<indicator> Items = new List<indicator>();
        //    List<SelectListItem> lst = new List<SelectListItem>();
        //    string dia = "";
        //    string mes = "";
        //    string anio = "";

        //    var extraer = fecha.Split('-');
        //    dia = extraer[2];
        //    mes = extraer[1];
        //    anio = extraer[0];

            


        //    using (var db = new AutogestionContext())
        //    {

        //        var empleadoid = Session["Empleado"];
        //        var empleado = db.Empleados.Find(empleadoid);

        //        var lista = db.PersonalActivo.Where(x => x.Superior == empleado.NroEmpleado || x.Jefe == empleado.NroEmpleado || x.Director == empleado.NroEmpleado).Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
        //        foreach (var x in lista)
        //        {
        //            lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
        //        }


        //        foreach (var ar in lst)
        //        {

        //            string consulta = "select PersonalActivoes.Area as type, count(*) as countsi, ((select COUNT(*) from PersonalActivoes as per where per.Area = personalActivoes.area group by per.Area) - COUNT(*) ) as countno from PersonalActivoes inner join Empleadoes on PersonalActivoes.CodigoEmpleado = Empleadoes.NroEmpleado left join EncabezadoEncuestas on Empleadoes.Id = EncabezadoEncuestas.EmpleadoId where PersonalActivoes.Area = '"+ ar.Text +"' and YEAR(EncabezadoEncuestas.Fecha) = " + anio + " and MONTH(EncabezadoEncuestas.Fecha) = " + mes + " and DAY(EncabezadoEncuestas.Fecha) = " + dia + " group by PersonalActivoes.Area";
        //            var resultados = db.Database.SqlQuery<indicator>(consulta).ToList();

        //            foreach (var item in resultados) { Items.Add(new indicator() { countsi = item.countsi , type = ar.Text, countno = item.countno }); }
                    
        //        }
                
                   
                
        //    }

        //    return Json(Items.ToArray());
        //}
    }
}
