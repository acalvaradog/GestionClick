using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Controllers
{
    public class MunicipiosController : Controller
    {
        private AutogestionContext db = new AutogestionContext();
        private MunicipiosRepository _repo = new MunicipiosRepository();
        // GET: Municipios
        public ActionResult Index()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("MunicipioCRUD"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var List = _repo.Obtenertodo();
             return View(List);
        }

        // GET: Municipios/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Municipios/Create
        public ActionResult Create()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("MunicipioCrear"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Departamentos = db.Departamento.Where(x => x.Estado == "Activo").ToList();
            return View();
        }

        // POST: Municipios/Create
        [HttpPost]
        public ActionResult Create(Municipio Model)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {

                    var result = _repo.Crear(Model);
                    if (result == "Guardado")
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        throw new Exception("" + result);
                    }

                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: Municipios/Edit/5
        public ActionResult Edit(int id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("MunicipioEdit"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var Model = db.Municipio.Where(x => x.Id == id).FirstOrDefault();
            ViewBag.Departamentos = db.Departamento.Where(x => x.Estado == "Activo").ToList();
            return View(Model);
        }

        // POST: Municipios/Edit/5
        [HttpPost]
        public ActionResult Edit(Municipio Model)
        {
            try
            {
                // TODO: Add update logic here
                var result = _repo.Edit(Model);
                if (result == "Guardado")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    throw (new Exception("" + result));

                }


            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: Municipios/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Municipios/Delete/5
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
