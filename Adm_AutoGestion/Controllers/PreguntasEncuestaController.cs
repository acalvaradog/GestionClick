using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Controllers
{
    public class PreguntasEncuestaController : Controller
    {
        //
        // GET: /PreguntasEncuesta/

        private PreguntasEncRepository _repo;

        public PreguntasEncuestaController()
        {
            _repo = new PreguntasEncRepository();

        }




        public ActionResult Index()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("PreguntasEncuesta"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var model = _repo.ObtenerTodos();
            return View(model);

            //return View();
        }

        //
        // GET: /PreguntasEncuesta/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /PreguntasEncuesta/Create

        public ActionResult Create()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("PreguntasEncuesta"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        //
        // POST: /PreguntasEncuesta/Create

        [HttpPost]
        public ActionResult Create(Preguntas model)
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
        // GET: /PreguntasEncuesta/Edit/5

        public ActionResult Edit(int id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("PreguntasEncuesta"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        //
        // POST: /PreguntasEncuesta/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, Preguntas model)
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
        // GET: /PreguntasEncuesta/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /PreguntasEncuesta/Delete/5

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
