using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Controllers
{
    public class TemasPreguntaController : Controller
    {
        //
        // GET: /TemasPregunta/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /TemasPregunta/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /TemasPregunta/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /TemasPregunta/Create

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
        // GET: /TemasPregunta/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /TemasPregunta/Edit/5

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
        // GET: /TemasPregunta/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /TemasPregunta/Delete/5

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
