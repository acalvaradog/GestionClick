using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Controllers
{
    public class EstructuraJerarquicaController : Controller
    {


        private EstructuraJerarquicaRepository _repo;

        public EstructuraJerarquicaController()
        {
            _repo = new EstructuraJerarquicaRepository();

        }
        //
        // GET: /EstructuraJerarquica/

        public ActionResult Index()
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EstructuraJerarquica"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var model = _repo.ObtenerTodos();
            return View(model);

        }

        //
        // GET: /EstructuraJerarquica/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /EstructuraJerarquica/Create

        public ActionResult Create()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EstructuraJerarquicaCrear"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            

            
            using (var db = new AutogestionContext())
            {

                ViewBag.Sociedad = db.Sociedad.ToList();

                ViewBag.Empleado = db.Empleados.Select(e => new { e.NroEmpleado, e.Nombres, e.Activo, nomcodigo = e.NroEmpleado + "-" + e.Nombres, e.FechaFin }).Where(f => f.Activo != "NO" || f.FechaFin >= DateTime.Now).ToList();
            }

            return View();
        }

        //
        // POST: /EstructuraJerarquica/Create

        [HttpPost]
        public JsonResult llenarArea(string id)
        {

            using (AutogestionContext db = new AutogestionContext())
            {

                var cargoapoyo = db.CargoApoyo.ToList();

                List<SelectListItem> lst = new List<SelectListItem>();

                var lista = db.Empleados.Where(s => s.Empresa == id && s.Activo == "SI" && (s.AreaDescripcion != null && s.AreaDescripcion != "")).GroupBy(b => b.UnidadOrganizativa).ToList();
                foreach (var x in lista)
                {
                    Empleado Item = x.FirstOrDefault();
                    lst.Add(new SelectListItem() { Value = Item.UnidadOrganizativa, Text = Item.AreaDescripcion });
                }



                return Json(lst);
            }

        }

        [HttpPost]
        public ActionResult Create(EstructuraJerarquica model)
        {
            try
            {
                // TODO: Add insert logic here
                if (model.Area != null && model.Area!="" && model.UnidadOrg !=null && model.UnidadOrg !="" && model.Jefe !=null && model.Jefe != "" && model.Superior != null && model.Superior != "" && model.Director != null && model.Director != "") 
                {
                    if ( model.Sociedad != null && model.Sociedad != "") 
                    {
                        using (AutogestionContext db = new AutogestionContext())
                        {
                            
                            EstructuraJerarquica Duplicado = db.EstructuraJerarquica.Where(x => x.UnidadOrg == model.UnidadOrg && x.Sociedad == model.Sociedad).FirstOrDefault();
                            if (Duplicado ==null)
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
                              
                                    Session["message"] = "Ya hay un estructura para el area seleccionada en la sociedad "+ model.Sociedad;
                     

                                return RedirectToAction("Create");
                            }

                        }
                    }
                    
                }

                return RedirectToAction("Index");
                
                
            }
            catch (Exception ex)
            {
                return RedirectToAction("Create");
            }
               

        }

        //
        // GET: /EstructuraJerarquica/Edit/5

        public ActionResult Edit(int id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EstructuraJerarquicaEditar"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            using (var db = new AutogestionContext())
            {
                EstructuraJerarquica estructura = db.EstructuraJerarquica.Find(id);
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
        public ActionResult Edit(int id, EstructuraJerarquica model)
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
                    _repo.Editar(id,model);
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                
            }
            return View();
        }

        //
        // GET: /EstructuraJerarquica/Delete/5

        public ActionResult Delete(int id)
        {
            using (var db = new AutogestionContext())
            {
                var Estr = db.EstructuraJerarquica.Where(x => x.Id == id).FirstOrDefault();
                return PartialView(Estr);
            }
                
        }

        //
        // POST: /EstructuraJerarquica/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                _repo.Delete(id);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }



    }
}
