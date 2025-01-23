using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Controllers
{
    public class TurnoDisponibilidadController : Controller
    {


        private TurnoDisponibilidadRepository _repo;

         public TurnoDisponibilidadController()
        {
            _repo = new TurnoDisponibilidadRepository();

        }


        //
        // GET: /TurnoDisponibilidad/

         public ActionResult Index(string Empleado, string FechaIni, string FechaFin, string FechaRIni, string FechaRFin, string Extras, string Liquidacion, string Estado, string Area)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("TurnoDisponibilidad"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

             int Usuario = 0;
            string usuariosesion = String.Format("{0}", Session["Empleado"]);
            Int32.TryParse(usuariosesion, out Usuario);
           
                

            List<TurnoDisponibilidad> Items = new List<TurnoDisponibilidad>();
            List<SelectListItem> lst = new List<SelectListItem>();
            using (var db = new AutogestionContext())
            {

                var lista = db.PersonalActivo.Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                foreach (var x in lista)
                {
                    lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }


                ViewBag.Opciones = lst;



                var empleado = db.Empleados.FirstOrDefault(e => e.Id == Usuario);
                ViewBag.Empleado = db.Empleados.Select(e => new { e.Id, e.Nombres, e.Activo, e.Superior, Jefe = e.Jefe, nomcodigo = e.Id + "-" + e.Nombres }).Where(f => f.Superior == empleado.NroEmpleado || f.Jefe == empleado.NroEmpleado && f.Activo != "NO").ToList();
                //var model = _repo.ObtenerTodos();


                DateTime Fecha1IC = DateTime.Now;
                DateTime Fecha2IC = DateTime.Now;
                DateTime Fecha1R = DateTime.Now;
                DateTime Fecha2R = DateTime.Now;

                if (FechaIni != "" && FechaFin != "" || FechaRIni != "" && FechaRFin != "")
                {

                    if (!DateTime.TryParse(FechaIni, out Fecha1IC))
                    {
                        Fecha1IC = new DateTime();
                    }

                    if (!DateTime.TryParse(FechaFin, out Fecha2IC))
                    {
                        string fec = "31/12/9999";
                        Fecha2IC = DateTime.Parse(fec);
                    }

                    if (!DateTime.TryParse(FechaRIni, out Fecha1R))
                    {
                        Fecha1R = new DateTime();
                    }

                    if (!DateTime.TryParse(FechaRFin, out Fecha2R))
                    {
                        string fec = "31/12/9999";
                        Fecha2R = DateTime.Parse(fec);
                    }



                    Items = db.TurnoDisponibilidad.Where(e => e.Empleado.Nombres.Contains(Empleado) &&
                                                             DbFunctions.TruncateTime(e.FechaInicio) >= Fecha1IC &&
                                                             DbFunctions.TruncateTime(e.FechaFin) <= Fecha2IC &&
                                                             DbFunctions.TruncateTime(e.FechaRegistro) >= Fecha1R &&
                                                             DbFunctions.TruncateTime(e.FechaRegistro) <= Fecha2R &&
                                                             e.Extras.Contains(Extras) &&
                                                             e.Empleado.Superior.Contains(empleado.NroEmpleado) &&
                                                             e.Liquidado.Contains(Liquidacion) &&
                                                             e.Empleado.Area.Contains(Area) &&
                                                             e.Estado.Contains(Estado)
                                                             ).ToList();

                }
                else {

                    Items = db.TurnoDisponibilidad.Where(e => e.Empleado.Nombres.Contains(Empleado) &&
                                                                 e.Extras.Contains(Extras) &&
                                                                 e.Empleado.Superior.Contains(empleado.NroEmpleado) &&
                                                                 e.Liquidado.Contains(Liquidacion) &&
                                                                 e.Empleado.Area.Contains(Area) &&
                                                                 e.Estado.Contains(Estado)
                                                                 ).ToList();
                
                }

                foreach (TurnoDisponibilidad Item in Items)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.TrabajadorId);
                }

                return View(Items);

            }
        }

        //
        // GET: /TurnoDisponibilidad/Details/5

         public ActionResult Informe(string FechaIni, string FechaFin, string FechaRIni, string FechaRFin, string Extras, string Liquidacion, string Estado, string Area)
         {
             List<string> funciones = Acceso.Validar(Session["Empleado"]);

             if (Acceso.EsAnonimo)
             {
                 return RedirectToAction("Login", "Login");
             }
             else if (!Acceso.EsAnonimo && !funciones.Contains("TurnoDisponibilidadInforme"))
             {
                 Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                 return RedirectToAction("Index", "Login");
             }

             int Usuario = 0;
             string usuariosesion = String.Format("{0}", Session["Empleado"]);
             Int32.TryParse(usuariosesion, out Usuario);



             List<TurnoDisponibilidad> Items = new List<TurnoDisponibilidad>();
             List<SelectListItem> lst = new List<SelectListItem>();
             using (var db = new AutogestionContext())
             {


                 var lista = db.PersonalActivo.Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                 foreach (var x in lista)
                 {
                     lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                 }


                 ViewBag.Opciones = lst;


                 //var empleado = db.Empleados.FirstOrDefault(e => e.Id == Usuario);
                 //ViewBag.Empleado = db.Empleados.Select(e => new { e.Id, e.Nombres, e.Activo, e.Superior, e.Jefe, nomcodigo = e.Id + "-" + e.Nombres }).Where(f => f.Superior == empleado.NroEmpleado || f.Jefe == empleado.NroEmpleado && f.Activo != "NO").ToList();
                 ////var model = _repo.ObtenerTodos();


                 DateTime Fecha1IC = DateTime.Now;
                 DateTime Fecha2IC = DateTime.Now;
                 DateTime Fecha1R = DateTime.Now;
                 DateTime Fecha2R = DateTime.Now;

                 if (FechaIni != "" && FechaFin != "" || FechaRIni != "" && FechaRFin != "")
                 {

                     if (!DateTime.TryParse(FechaIni, out Fecha1IC))
                     {
                         Fecha1IC = new DateTime();
                     }

                     if (!DateTime.TryParse(FechaFin, out Fecha2IC))
                     {
                         string fec = "31/12/9999";
                         Fecha2IC = DateTime.Parse(fec);
                     }

                     if (!DateTime.TryParse(FechaRIni, out Fecha1R))
                     {
                         Fecha1R = new DateTime();
                     }

                     if (!DateTime.TryParse(FechaRFin, out Fecha2R))
                     {
                         string fec = "31/12/9999";
                         Fecha2R = DateTime.Parse(fec);
                     }



                     Items = db.TurnoDisponibilidad.Where(e => 
                                                              DbFunctions.TruncateTime(e.FechaInicio) >= Fecha1IC &&
                                                              DbFunctions.TruncateTime(e.FechaFin) <= Fecha2IC &&
                                                              DbFunctions.TruncateTime(e.FechaRegistro) >= Fecha1R &&
                                                              DbFunctions.TruncateTime(e.FechaRegistro) <= Fecha2R &&
                                                              e.Extras.Contains(Extras) &&
                                                              e.Liquidado.Contains(Liquidacion) &&
                                                              e.Empleado.Area.Contains(Area) &&
                                                              e.Estado.Contains(Estado)
                                                              ).ToList();

                 }
                 else
                 {

                     Items = db.TurnoDisponibilidad.Where(e => 
                                                                  e.Extras.Contains(Extras) &&
                                                                  e.Liquidado.Contains(Liquidacion) &&
                                                                  e.Empleado.Area.Contains(Area) &&
                                                                  e.Estado.Contains(Estado)
                                                                  ).ToList();

                 }

                 foreach (TurnoDisponibilidad Item in Items)
                 {
                     Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.TrabajadorId);
                 }

                 return View(Items);

             }
         }

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /TurnoDisponibilidad/Create

        public ActionResult Create()
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("TurnoDisponibilidadCrear"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            int Usuario = 0;
            string usuariosesion = String.Format("{0}", Session["Empleado"]);
            Int32.TryParse(usuariosesion, out Usuario);
            using (var db = new AutogestionContext())
            {
                var empleado = db.Empleados.FirstOrDefault(e => e.Id == Usuario);
                ViewBag.Empleado = db.Empleados.Select(e => new { e.Id, e.Nombres, e.Activo, e.Superior, Jefe = e.Jefe, nomcodigo = e.Id + "-" + e.Nombres }).Where(f => f.Superior == empleado.NroEmpleado || f.Jefe == empleado.NroEmpleado && f.Activo != "NO").ToList();
            }

            return View();
        }

        //
        // POST: /TurnoDisponibilidad/Create

        [HttpPost]
        public ActionResult Create(TurnoDisponibilidad model)
        {
            string message = "";

            int Usuario = 0;
            string usuariosesion = String.Format("{0}", Session["Empleado"]);
            Int32.TryParse(usuariosesion, out Usuario);


            using (var db = new AutogestionContext())
            {
                var emp = db.Empleados.Find(model.TrabajadorId);
                model.Empresa = emp.Empresa;

            try
            {

                var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
             .Select(x => new { x.Key, x.Value.Errors })
             .ToArray();
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {

                    message = _repo.Crear(model);
                    Session["message"] = message;
                    return RedirectToAction("Index", new { message = message });
                }
            }
            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
            }

            var empleado = db.Empleados.FirstOrDefault(e => e.Id == Usuario);
            
                ViewBag.Empleado = db.Empleados.Select(e => new { e.Id, e.Nombres, e.Activo, e.Superior, Jefe = e.Jefe, nomcodigo = e.Id + "-" + e.Nombres }).Where(f => f.Superior == empleado.NroEmpleado || f.Jefe == empleado.NroEmpleado && f.Activo != "NO").ToList();
            }
            Session["message"] = message;
            return View(model);
        }



        public ActionResult RecibidoGestionHumana(string Id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("TurnoDisponibilidadRecibeGH"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }


            int id = -1;
            Int32.TryParse(Id, out id);
            var model = _repo.ObtenerTodos2();


            if (id > 0)
            {
                ViewBag.TDisp = model.FirstOrDefault(e => e.Id == id);
            }

            if (ViewBag.TDisp == null)
            {
                ViewBag.TDisp = new Models.TurnoDisponibilidad();
                ViewBag.TDisp.Empleado = new Empleado();
            }



            return View(model);

        }

        public ActionResult RecibidoGestionHumana1(string Id,string Liquidado, string Estado)
        {
            string message = null;

            try
            {
                int id = 0;
                Int32.TryParse(Id, out id);

                if (Id != "0")
                {
                    _repo.Modificar(Id, Liquidado, Estado);

                }else
                {
                  message = "Antes de Guardar, debe seleccionar un registro de la tabla.";
                }

                Session["message"] = message;
                return RedirectToAction("RecibidoGestionHumana");

            }
            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
                Session["message"] = message;
                return View();
            }


        }



        //
        // GET: /TurnoDisponibilidad/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /TurnoDisponibilidad/Edit/5

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
        // GET: /TurnoDisponibilidad/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /TurnoDisponibilidad/Delete/5

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
    }
}
