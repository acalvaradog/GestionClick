using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Adm_AutoGestion.Models.EvaluacionDesempenoRa;

namespace Adm_AutoGestion.Controllers
{
    public class EvaluacionCriteriosController : Controller
    {

        private readonly EvaluacionCriterioRepository _evaluacioncriteriosRepository;
        private readonly AutogestionContext _context;

        public EvaluacionCriteriosController()
        {
            _context = new AutogestionContext();
            _evaluacioncriteriosRepository = new EvaluacionCriterioRepository(_context);
        }

     

        // GET: EvaluacionCriterios
        public ActionResult Index()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EvaluacionCriterios"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var criterios = _evaluacioncriteriosRepository.GetAllCriterios();


            return View(criterios);
        }

        // GET: EvaluacionCriterios/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EvaluacionCriterios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EvaluacionCriterios/Create
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

        // GET: EvaluacionCriterios/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EvaluacionCriterios/Edit/5
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

        // GET: EvaluacionCriterios/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EvaluacionCriterios/Delete/5
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
