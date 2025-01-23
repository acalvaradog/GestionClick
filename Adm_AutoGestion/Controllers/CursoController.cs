using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Controllers
{
    public class CursoController : Controller
    {
        CursoRepository _repo = new CursoRepository();
        AutogestionContext db = new AutogestionContext();

        // GET: Curso
        public ActionResult RegistroParticipantes(string Area, string Cargo, string Empresa, string TipoArea)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("CrearCurso"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            //ViewBag.Empleados = db.Empleados.Where(x=> x.Activo == "SI").Select(x=> new { x.Id, x.Nombres}).ToList();
            ViewBag.Cursos = db.Curso.Select(x=> new { x.Id, x.FullName}).ToList();
            ViewBag.Areas = db.Empleados.Where(x => x.Area != null && x.Area != "").Select(x=> new { x.Area }).GroupBy(x=> x.Area).ToList();
            ViewBag.Cargo = db.Empleados.Where(x => x.Cargo != null && x.Cargo != "").Select(x => new { x.Cargo }).GroupBy(x => x.Cargo).ToList();
            ViewBag.Empresa = db.Empleados.Where(x => x.Empresa != null && x.Empresa != "").Select(x => new { x.Empresa }).GroupBy(x => x.Empresa).ToList();
            ViewBag.TipoArea = db.Empleados.Where(x => x.TipoArea != null && x.TipoArea != "").Select(x => new { x.TipoArea }).GroupBy(x => x.TipoArea).ToList();

            var datos = _repo.ObtenerEmpleados(Area, Cargo, Empresa, TipoArea);

            ViewBag.Empleados = datos;

            return View();
        }

        public JsonResult RegistrarEmpleados()
        {

            var Empleados = HttpContext.Request.Params["Empleados"];
            var Curso = HttpContext.Request.Params["Curso"];

            try
            {
                return Json(_repo.RegistrarEmpleados(Empleados, Curso));
            }
            catch (Exception ex)
            {
                return Json("Error:" + ex);
            }
        }

        // GET: Curso/Create
        public ActionResult Crear()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("CrearCurso"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var v = TempData["Msg"] as string;
            ViewBag.Confirmacion = v;
            return View();
        }

        // POST: Curso/Create
        [HttpPost]
        public ActionResult Crear(string FullName, string Categoria, bool EsObligatorio, string Modalidad, DateTime StartDate, DateTime EndDate)
        {
            try
            {
                var v =_repo.CrearCurso(FullName, Categoria, EsObligatorio, Modalidad, StartDate, EndDate);
                TempData["Msg"] = v;
                return RedirectToAction("Crear");
            }
            catch
            {
                return View();
            }
        }

    }
}
