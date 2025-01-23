using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Controllers
{
    public class DepartamentoController : Controller
    {
        private AutogestionContext db = new AutogestionContext();
        private DepartamentoRepository _repo = new DepartamentoRepository();
        // GET: Departamento
        public ActionResult Index()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("DepartamentoCRUD"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            List<Departamento> List = db.Departamento.Where(x => x.Estado == "Activo").OrderBy(x => x.Nombre).ToList();

            return View(List);
        }

        // GET: Departamento/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Departamento/Create
        public ActionResult Create()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("DepartamentoCrear"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        // POST: Departamento/Create
        [HttpPost]
        public ActionResult Create(Departamento Model)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {

                    var result = _repo.Crear(Model);
                    if (result=="Guardado") 
                    {
                        return RedirectToAction("Index");
                    }
                    else 
                    {
                        throw new Exception(""+ result);
                    }
                    
                }
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                return View();
            }
        }

        // GET: Departamento/Edit/5
        public ActionResult Edit(int id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("DepartamentoEdit"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var Model = db.Departamento.Where(x => x.Id == id).FirstOrDefault();
            return View(Model);
        }

        // POST: Departamento/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Departamento Model)
        {
            try
            {
                // TODO: Add update logic here
                var result = _repo.Edit(Model);
                if (result == "Guardado")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    throw (new Exception("" + result));
                 
                }
              

            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: Departamento/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Departamento/Delete/5
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
