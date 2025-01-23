using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Controllers
{
    public class HabilitarVacacionesController : Controller
    {

        private HabilitarVacacionesRepository _repo;

        public HabilitarVacacionesController()
        {
            _repo = new HabilitarVacacionesRepository();
        }
        //
        // GET: /HabilitarVacaciones/

        public ActionResult Index()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("HabilitarVacaciones"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var usuario = string.Format("{0}", Session["Empleado"]);
            var model = _repo.ObtenerTodos(usuario);
            return View(model);
        }

        //
        // GET: /HabilitarVacaciones/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /HabilitarVacaciones/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /HabilitarVacaciones/Create

        [HttpPost]
        public ActionResult Create(HabilitarVacaciones model)
        {

            try
            {

                using (var db = new AutogestionContext())
                {

                // TODO: Add insert logic here
                string Nro = string.Format("{0}", model.EmpleadoId);
                Empleado Codigo = db.Empleados.FirstOrDefault(e => e.NroEmpleado == Nro);
                model.EmpleadoId = Codigo.Id;
                
                    model.UsuarioRegistra = string.Format("{0}", Session["Empleado"]);
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
        // GET: /HabilitarVacaciones/Edit/5

        public ActionResult Edit(int id)
        {
            using (var db = new AutogestionContext())
            {
                HabilitarVacaciones tipo = db.HabilitarVacaciones.Find(id);
                if (tipo == null)
                {
                    return HttpNotFound();
                }
                return View(tipo);
            }
        }

        //
        // POST: /HabilitarVacaciones/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, HabilitarVacaciones model)
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
        // GET: /HabilitarVacaciones/Delete/5

        public ActionResult Delete(int id)
        {
            using (var db = new AutogestionContext())
            {
                HabilitarVacaciones tipo = db.HabilitarVacaciones.Find(id);
                if (tipo == null)
                {
                    return HttpNotFound();
                }
                return View(tipo);
            }
        }

        //
        // POST: /HabilitarVacaciones/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, HabilitarVacaciones model)
        {
            try
            {
                // TODO: Add update logic here

                _repo.Eliminar(id);

                return RedirectToAction("Index");
            }
            catch
            {

            }
            return View();
        }
    }
}
