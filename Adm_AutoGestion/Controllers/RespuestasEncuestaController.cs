using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Controllers
{
    public class RespuestasEncuestaController : Controller
    {
        //
        // GET: /RespuestasEncuesta/
        private RespuestasEncuestaRepository _repo;

         public RespuestasEncuestaController()
        {
            _repo = new RespuestasEncuestaRepository();

        }



        public ActionResult Index( int Id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("RespuestasEncuesta"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
          var model = _repo.ObtenerTodos(Id);

          ViewBag.Empleado = new Models.Empleado();
          if (model.Count > 0) {
              using (var db = new Models.AutogestionContext())
              {
                  //Models.Encuesta Item = model[0];
                  //Models.EncabezadoEncuesta Item1 = db.EncabezadoEncuesta.FirstOrDefault(e => e.Id == Item.Id);
                  Models.EncabezadoEncuesta Item1 = db.EncabezadoEncuesta.FirstOrDefault(e => e.Id == Id);
                  ViewBag.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item1.EmpleadoId);
                  ViewBag.EncabezadoEncuesta = db.EncabezadoEncuesta.FirstOrDefault(e => e.Id == Item1.Id);
              }
 
          }
          
          return View(model);
        }

        //
        // GET: /RespuestasEncuesta/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /RespuestasEncuesta/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /RespuestasEncuesta/Create

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

        //
        // GET: /RespuestasEncuesta/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /RespuestasEncuesta/Edit/5

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
        // GET: /RespuestasEncuesta/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /RespuestasEncuesta/Delete/5

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
