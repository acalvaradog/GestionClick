using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Controllers
{
    public class GrupoEmpleadosController : Controller
    {
        //
        // GET: /GrupoEmpleados/

        private GrupoEmpleadosRepository _repo;

        public GrupoEmpleadosController()
        {
            _repo = new GrupoEmpleadosRepository();

        }



        public ActionResult Index()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("GrupoEmpleados"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var model = _repo.ObtenerTodos();
            return View(model);
        }

        //
        // GET: /GrupoEmpleados/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /GrupoEmpleados/Create

        public ActionResult Create()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("GrupoEmpleados"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        //
        // POST: /GrupoEmpleados/Create

        [HttpPost]
        public ActionResult Create(GrupoEmpleados model)
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
        // GET: /GrupoEmpleados/Edit/5

        public ActionResult Edit(int id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("GrupoEmpleados"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var model = new GrupoEmpleados();



            using (var db = new AutogestionContext())
            {
                model = db.GrupoEmpleados.Find(id);
       

            }
            return View(model);
        }

        //
        // POST: /GrupoEmpleados/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, GrupoEmpleados model)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {

                    _repo.Editar(model);

                    return RedirectToAction("Index");
                }
            }
            catch
            {

            }
            return View();
        }

        //
        // GET: /GrupoEmpleados/Delete/5

        public ActionResult Delete(int id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("GrupoEmpleados"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        //
        // POST: /GrupoEmpleados/Delete/5

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
