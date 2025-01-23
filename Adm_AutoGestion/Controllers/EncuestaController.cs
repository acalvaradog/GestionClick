using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Controllers
{
    public class EncuestaController : Controller
    {
        //
        // GET: /Encuesta/


        private EncuestaRepository _repo;

        public EncuestaController()
        {
            _repo = new EncuestaRepository();

        }


        public ActionResult Index()
        {
            var model = _repo.ObtenerTodos();
            return View(model);
        }

        //
        // GET: /Encuesta/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Encuesta/Create

        public ActionResult Create()
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("Encuesta-Crear"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var model = new Encuesta();
            using (var db = new AutogestionContext())
            {
                model.ListadoPreguntas = db.Preguntas.Where(e => e.Activo == true).ToList();

            }

            List<SelectListItem> lst = new List<SelectListItem>();

            lst.Add(new SelectListItem() { Text = "SI", Value = "SI" });
            lst.Add(new SelectListItem() { Text = "NO", Value = "NO" });

            model.Opciones = lst;
           
            return View(model);
        }

        //
        // POST: /Encuesta/Create

        [HttpPost]
        public ActionResult Create(Encuesta model)
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
        // GET: /Encuesta/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Encuesta/Edit/5

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
        // GET: /Encuesta/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Encuesta/Delete/5

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
