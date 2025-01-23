using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Controllers
{
    public class DetalleEncabezadoPrestamoController : Controller
    {
        //
        // GET: /DetalleEntregaPrenda/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /DetalleEntregaPrenda/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /DetalleEntregaPrenda/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /DetalleEntregaPrenda/Create

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
        // GET: /DetalleEntregaPrenda/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /DetalleEntregaPrenda/Edit/5

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
        // GET: /DetalleEntregaPrenda/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /DetalleEntregaPrenda/Delete/5

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
