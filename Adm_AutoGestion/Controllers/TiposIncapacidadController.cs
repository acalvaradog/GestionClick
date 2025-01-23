using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Controllers
{
    public class TiposIncapacidadController : Controller
    {

        private TiposIncapacidadRepository _repo;

        public TiposIncapacidadController()
        {
            _repo = new TiposIncapacidadRepository();

        }

        public ActionResult Index()
        {
            var model = _repo.ObtenerTodos();
            return View(model);
        }

        //
        // GET: /TiposIncapacidad/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /TiposIncapacidad/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /TiposIncapacidad/Create

        [HttpPost]
        public ActionResult Create(TiposIncapacidad model)
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
        // GET: /TiposIncapacidad/Edit/5

        public ActionResult Edit(int id)
        {
            using (var db = new AutogestionContext())
            {
                TiposIncapacidad tipo = db.TiposIncapacidad.Find(id);
                if (tipo == null)
                {
                    return HttpNotFound();
                }
                return View(tipo);
            }
        }

        //
        // POST: /TiposIncapacidad/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, TiposIncapacidad model)
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
        // GET: /TiposIncapacidad/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /TiposIncapacidad/Delete/5

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
