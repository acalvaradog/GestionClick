using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Adm_AutoGestion.Services;
using Adm_AutoGestion.Models;

namespace Adm_AutoGestion.Controllers
{
 
    public class PreguntasController : Controller
    {

          private PreguntasRepository _repo;

        public PreguntasController()
        { 
        _repo = new PreguntasRepository();
        }

    
 


        public ActionResult Index()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("Preguntas"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var model = _repo.ObtenerTodos();
            return View(model);
            //using (var db = new AutogestionContext())
            //{

            //    var preguntas = db.PreguntasFrecuentes.Join(db.TemasPreguntas, pre => pre.TemaId, tema => tema.Id, (pre, tema) => new { pre, tema }).ToList();

            
            //  //  return db.PreguntasFrecuentes.ToList();
            //    //return preguntas;

            //}
        }


     
        //
        // GET: /Preguntas/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Preguntas/Create

        public ActionResult Create()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("Preguntas"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var model = new PreguntaFrecuente();
            using (var db = new AutogestionContext())
            {
                model.ListadoTemas = db.TemasPreguntas.ToList();

            }

       

            return View(model);
        }

        //
        // POST: /Preguntas/Create

        [HttpPost]
        public ActionResult Create(PreguntaFrecuente model)
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
        // GET: /Preguntas/Edit/5

        public ActionResult Edit(int id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("Preguntas"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        //
        // POST: /Preguntas/Edit/5

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
        // GET: /Preguntas/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Preguntas/Delete/5

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
