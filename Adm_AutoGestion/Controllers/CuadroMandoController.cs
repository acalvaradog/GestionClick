using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Controllers
{
    public class CuadroMandoController : Controller
    {
        private AutogestionContext db = new AutogestionContext();
        private CuadroMandoRepository _repo = new CuadroMandoRepository();

        // GET: CuadroMando
        public ActionResult Dashboard()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("CuadroMandoDashboard") && !funciones.Contains("CuadroMandoJefeDashboard"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            if (funciones.Contains("CuadroMandoJefeDashboard"))
            {
                var Id = Session["Empleado"];
                var empleado = db.Empleados.Find(Id);
                ViewBag.Areas = db.Empleados.Where(x => x.Area != null && x.Area != "" && x.Jefe == empleado.NroEmpleado).Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                ViewBag.Cargos = db.Empleados.Where(x => x.Cargo != null && x.Cargo != "" && x.Jefe == empleado.NroEmpleado).Select(x => new { x.Cargo }).GroupBy(b => b.Cargo).ToList();
            }
            else
            {
                ViewBag.Areas = db.Empleados.Where(e => e.Area != null && e.Area != "").Select(e => new { e.Area }).GroupBy(b => b.Area);
                ViewBag.Cargos = db.Empleados.Where(e => e.Cargo != null && e.Cargo != "").Select(e => new { e.Cargo }).GroupBy(b => b.Cargo);
            }

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> DatosDashboard()
        {
            var Id = Session["Empleado"].ToString();
            var FInicio = HttpContext.Request.Params["FechaInicio"];
            var FFin = HttpContext.Request.Params["FechaFin"];
            var Cargo = HttpContext.Request.Params["Cargo"];
            var Area = HttpContext.Request.Params["Area"];
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (funciones.Contains("CuadroMandoJefeDashboard"))
            {
                var respuesta = await _repo.ObtenerDatosGrafica(Id, FInicio, FFin, true, Area, Cargo);
                return Json(respuesta);
            }
            else
            {
                var respuesta = await _repo.ObtenerDatosGrafica(Id, FInicio, FFin, false, Area, Cargo);
                return Json(respuesta);
            }
        }
    }
}
