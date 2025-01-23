using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
namespace Adm_AutoGestion.Controllers
{
    public class GraficasController : Controller
    {
        //
        // GET: /Graficas/

        private class indicator
        {
            public int count { get; set; }
            public string type { get; set; }
            public string var { get; set; }
        }

        private class indicator2
        {
            public int count { get; set; }
            public string type { get; set; }
            public string variable { get; set; }
        }
        public ActionResult Index(string message = " ")
        {

            ViewBag.Message = message;
            return View();
        }
        

        [HttpPost]
        public JsonResult Index()
        {
            List<indicator> Items = new List<indicator>();

            using (AutogestionContext db = new AutogestionContext())
            {
                var Transporte = db.EncabezadoEncuesta.GroupBy(e => e.Transporte).Select(e => new { Name = e.Key, Count = e.Count() }).ToArray();
                var ModoTrabajo = db.EncabezadoEncuesta.GroupBy(e => e.ModoTrabajo).Select(e => new { Name = e.Key, Count = e.Count() }).ToArray();
                var Sospechoso = db.EncabezadoEncuesta.GroupBy(e => e.Sospechoso).Select(e => new { Name = e.Key, Count = e.Count() }).ToArray();

                foreach (var item in Transporte) { Items.Add(new indicator() { count = item.Count, type = "Transporte", var = item.Name }); }
                foreach (var item in ModoTrabajo) { Items.Add(new indicator() { count = item.Count, type = "Modo de trabajo", var = item.Name }); }
                foreach (var item in Sospechoso) { Items.Add(new indicator() { count = item.Count, type = "Sospechoso", var = item.Name == true ? "Si" : "No" }); }
            }

            return Json(Items.ToArray());
        }

        [HttpPost]
        public JsonResult GraficaSospechosos()
        {
            List<indicator2> Items = new List<indicator2>();

            using (AutogestionContext db = new AutogestionContext())
            {
                var Sospechoso = db.EncabezadoEncuesta.Where(e => e.Sospechoso == true).GroupBy(e => DbFunctions.TruncateTime(e.Fecha)).Select(e => new { Name = e.Key, Count = e.Count() }).ToArray();
    
                foreach (var item in Sospechoso) { Items.Add(new indicator2() { count = item.Count, type = "Sospechoso", variable = item.Name.ToString()  }); }
            }

            return Json(Items.ToArray());
        }
        [HttpPost]
        public JsonResult GraficaNoSospechosos()
        {
            List<indicator2> Items = new List<indicator2>();

            using (AutogestionContext db = new AutogestionContext())
            {
                var Sospechoso = db.EncabezadoEncuesta.Where(e => e.Sospechoso == false).GroupBy(e => DbFunctions.TruncateTime(e.Fecha)).Select(e => new { Name = e.Key, Count = e.Count() }).ToArray();

                foreach (var item in Sospechoso) { Items.Add(new indicator2() { count = item.Count, type = "Sospechoso", variable = item.Name.ToString() }); }
            }

            return Json(Items.ToArray());
        }
        //
        // GET: /Graficas/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Graficas/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Graficas/Create

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
        // GET: /Graficas/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Graficas/Edit/5

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
        // GET: /Graficas/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Graficas/Delete/5

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
