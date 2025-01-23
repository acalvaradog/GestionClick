using Adm_AutoGestion.Controllers.Api;
using Adm_AutoGestion.Models;
using Adm_AutoGestion.Models.EvaDesempeno;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Controllers
{
    public class EstructuraJerarquicaEVADESController : Controller
    {
        private EstructuraJerarquicaEVADESRepository _repo;
        private EvaDesempenoContext db2= new EvaDesempenoContext();
        private AutogestionContext db = new AutogestionContext();
        EvaDesempenoController APISEvaDesempeño = new EvaDesempenoController();
        public EstructuraJerarquicaEVADESController()
        {
            _repo = new EstructuraJerarquicaEVADESRepository();

        }
        //
        // GET: /EstructuraJerarquica/

        public ActionResult IndexEVADES()
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EstructuraJerarquicaEvadesh"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var model = _repo.ObtenerTodos();
            return View(model);

        }

        //
        // GET: /EstructuraJerarquica/Details/5

        public ActionResult DetailsEVADES(int id)
        {
            return View();
        }

        //
        // GET: /EstructuraJerarquica/Create

        public ActionResult CreateEVADES()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EstructuraJerarquicaCrearEvadesh"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }





                ViewBag.Empleado = db.Empleados.Select(e => new { e.NroEmpleado, e.Nombres, e.Activo, nomcodigo = e.NroEmpleado + "-" + e.Nombres, e.FechaFin }).Where(f => f.Activo != "NO" || f.FechaFin >= DateTime.Today).ToList();
            

            return View();
        }

        //
        // POST: /EstructuraJerarquica/Create

        [HttpPost]
        public JsonResult llenarAreaEVADES(string id)
        {

            

                var cargoapoyo = db.CargoApoyo.ToList();

                List<SelectListItem> lst = new List<SelectListItem>();

                var lista = db.Empleados.Where(s => s.Empresa == id && s.Activo == "SI" && (s.AreaDescripcion != null && s.AreaDescripcion != "") && s.TipoArea == "Asistenciales CO").GroupBy(b => b.UnidadOrganizativa).ToList();
                        foreach (var x in lista)
                        {
                            //lst.Add(new SelectListItem() { Value = x.Key.ToString(), Text = x.Key.ToString() });
                            Empleado Item = x.FirstOrDefault();
                            lst.Add(new SelectListItem() { Value = Item.UnidadOrganizativa, Text = Item.AreaDescripcion });
                        }



            return Json(lst);
            

        }

        [HttpPost]
        public ActionResult CreateEVADES(Estructura_Jerarquica_EVADES model)
        {
            try
            {
                // TODO: Add insert logic here
                //if (ModelState.IsValid)
                //{

                //    _repo.Crear(model);
                //    return RedirectToAction("IndexEVADES");
                //}
                if (model.Area != null && model.Area != "" && model.UnidadOrg != null && model.UnidadOrg != "" && model.Jefe != null && model.Jefe != "" && model.Superior != null && model.Superior != "" && model.Director != null && model.Director != "")
                {
                    if (model.Sociedad != null && model.Sociedad != "")
                    {
 

                            Estructura_Jerarquica_EVADES Duplicado = db2.Estructura_Jerarquica_EVADES.Where(x => x.UnidadOrg == model.UnidadOrg && x.Sociedad == model.Sociedad).FirstOrDefault();
                            if (Duplicado == null)
                        {
                                var idEmp = Session["Empleado"];
                                int Id = Convert.ToInt32(idEmp);

                                Empleado UsuarioCreador = db.Empleados.Where(x => x.Id == Id).FirstOrDefault();
                                model.A_UsuarioCreador = UsuarioCreador.NroEmpleado;
                                model.A_Creacion = DateTime.Now;
                                _repo.Crear(model);
                            }
                            else
                            {

                                Session["message"] = "Ya hay un estructura para el area seleccionada en la sociedad " + model.Sociedad;


                                return RedirectToAction("CreateEVADES");
                            }

                        
                    }

                }
                return RedirectToAction("CreateEVADES");
            }
            catch(Exception ex)
            {
                Session["message"] = ex.ToString();
                return RedirectToAction("CreateEVADES");
            }
            

        }

        //
        // GET: /EstructuraJerarquica/Edit/5

        public ActionResult EditEVADES(int id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EstructuraJerarquicaEditarEvadesh"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            using (var db = new AutogestionContext())
            {
                Estructura_Jerarquica_EVADES estructura = db2.Estructura_Jerarquica_EVADES.Find(id);
                if (estructura == null)
                {
                    return HttpNotFound();
                }

                ViewBag.Empleado = db.Empleados.Select(e => new { e.NroEmpleado, e.Nombres, e.Activo, nomcodigo = e.NroEmpleado + "-" + e.Nombres, e.FechaFin }).Where(f => f.Activo != "NO" || f.FechaFin >= DateTime.Now).ToList();


                return View(estructura);


            }
        }

        //
        // POST: /EstructuraJerarquica/Edit/5

        [HttpPost]
        public ActionResult EditEVADES(int id, Estructura_Jerarquica_EVADES model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var idEmp = Session["Empleado"];
                    int Id = Convert.ToInt32(idEmp);
                    using (var db = new AutogestionContext())
                    {
                        Empleado UsuarioModificador = db.Empleados.Where(x => x.Id == Id).FirstOrDefault();
                        model.A_UsuarioModifica = UsuarioModificador.NroEmpleado;
                        model.A_Modificacion = DateTime.Now;
                    }
                    _repo.Editar(id, model);
                    return RedirectToAction("IndexEVADES");
                }
            }
            catch
            {

            }
            return View();
        }

        //
        // GET: /EstructuraJerarquica/Delete/5

        public ActionResult DeleteEVADES(int id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EstructuraJerarquicaEditarEvadesh"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var Estr = db2.Estructura_Jerarquica_EVADES.Where(x => x.Id == id).FirstOrDefault();
                return PartialView(Estr);
            

        }

        //
        // POST: /EstructuraJerarquica/Delete/5

        [HttpPost]
        public ActionResult DeleteEVADES(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                _repo.Delete(id);

                return RedirectToAction("IndexEVADES");
            }
            catch
            {
                return View();
            }
        }
        
     //Actualización de evaluaciones masiva de forma "manual"
        public ActionResult ActualizacionMasivaEvadesh() 
        {
            try
            {
                List<string> funciones = Acceso.Validar(Session["Empleado"]);
                if (Acceso.EsAnonimo)
                {
                    return RedirectToAction("Login", "Login");
                }
                else if (!Acceso.EsAnonimo && !funciones.Contains("ActualizacionMasivaEstructura"))
                {
                    Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                    return RedirectToAction("Index", "Login");
                }
                var Respuesta = APISEvaDesempeño.ActualizacionEvaluacion();

                if (Respuesta != "true") 
                {
                    throw new Exception("" + Respuesta);
                }
                Session.Add("messageExito", "El proceso ha terminado de forma exitosa");
                return View();
            }
            catch(Exception ex)
            {
                Session.Add("messageERROR", " ha ocurrido el siguiente error " +ex);
                return View();
            }     
        
        }

    }
}
