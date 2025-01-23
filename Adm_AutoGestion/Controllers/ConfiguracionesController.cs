using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Controllers
{
    public class ConfiguracionesController : Controller
    {


         private ConfiguracionesRepository _repo;

         public ConfiguracionesController()
        {
            _repo = new ConfiguracionesRepository();
        }

        //
        // GET: /Configuraciones/

        public ActionResult Index()
        {
            var model = _repo.ObtenerTodos();
            return View(model);
        }

        //
        // GET: /Configuraciones/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Configuraciones/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Configuraciones/Create

        [HttpPost]
        public ActionResult Create(Configuraciones model)
        {
            try
            {
                List<string> funciones = Acceso.Validar(Session["Empleado"]);

                if (Acceso.EsAnonimo)
                {
                    return RedirectToAction("Login", "Login");
                }
                else if (!Acceso.EsAnonimo && !funciones.Contains("Configuraciones"))
                {
                    Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                    return RedirectToAction("Index", "Login");
                }

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
        // GET: /Configuraciones/Edit/5

        public ActionResult Edit(int id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("Configuraciones"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            using (var db = new AutogestionContext())
            {
                Configuraciones datos = db.Configuraciones.Find(id);
                if (datos == null)
                {
                    return HttpNotFound();
                }
                return View(datos);
            }
        }

        //
        // POST: /Configuraciones/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, Configuraciones model)
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
        // GET: /Configuraciones/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Configuraciones/Delete/5

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

       
        
        public ActionResult HabilitarDescarga(string Valor)
        {
            try
            {
                if(Valor != null){
                _repo.Modificar(Valor);
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                
            }
            return View();
        }



    }
}
