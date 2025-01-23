using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Controllers
{
    public class PerfilController : Controller
    {
        //
        // GET: /Perfil/
         private PerfilRepository _repo;

         public PerfilController()
        {
            _repo = new PerfilRepository();

        }



        public ActionResult Index()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("Perfil"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var model = _repo.ObtenerTodos();
            return View(model);
        }

        //
        // GET: /Perfil/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Perfil/Create

        public ActionResult Create()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("Perfil"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var model = new Perfil();
            using (var db = new AutogestionContext())
            {
                model.ListadoGrupos = db.GrupoEmpleados.ToList();

            }

       

            return View(model);
       
        }

        //
        // POST: /Perfil/Create

        [HttpPost]
        public ActionResult Create(Perfil model)
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
        // GET: /Perfil/Edit/5

        public ActionResult Edit(int id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("Perfil"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var model = new Perfil();

            

            using (var db = new AutogestionContext())
            {
                model = db.Perfil.Find(id);
                model.ListadoGrupos = db.GrupoEmpleados.ToList();

            }



            return View(model);
        }

        //
        // POST: /Perfil/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, Perfil model)
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
        // GET: /Perfil/Delete/5

        public ActionResult Delete(int id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("Perfil"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            using (var db = new AutogestionContext())
            {
                Perfil tipo = db.Perfil.Find(id);
                tipo.GrupoEmpleados = db.GrupoEmpleados.Where(x => x.Id == tipo.GrupoId).FirstOrDefault();
                if (tipo == null)
                {
                    return HttpNotFound();
                }
                return View(tipo);
            }
        }

        //
        // POST: /Perfil/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, Perfil model)
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
