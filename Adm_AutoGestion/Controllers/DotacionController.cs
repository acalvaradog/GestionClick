using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using Microsoft.Ajax.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Controllers
{
    public class DotacionController : Controller
    {
        private AutogestionContext db = new AutogestionContext();
        private DotacionRepository _repo = new DotacionRepository();

        // GET: Dotacion
        public ActionResult Listado(string Year, string Cat, string Area, string Recibido)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("DotacionListado"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var datos = _repo.ObtenerListado(Year, Cat, Area, Recibido);

            var asistencialesCount = 0;

            var administrativosCount = 0;

            if (Area == "Adm")
            {
                administrativosCount = datos.Count();
            }
            if (Area == "Asi")
            {
                asistencialesCount = datos.Count();
            }

            var empleadosIds = datos.Select(x => x.EmpleadoId).Distinct().ToList();
            var empleados = db.Empleados.Where(x => empleadosIds.Contains(x.Id)).ToList();
            var hDotacion = db.HistorialDotacion.Where(x => empleadosIds.Contains(x.EmpleadoId)).ToList();

            foreach (var registro in datos)
            {
                registro.NombreEmpleado = empleados.FirstOrDefault(x => x.Id == registro.EmpleadoId)?.Nombres;
                registro.Categoria = empleados.FirstOrDefault(x => x.Id == registro.EmpleadoId)?.CategoriaDotacion;
                registro.TipoArea = empleados.FirstOrDefault(x => x.Id == registro.EmpleadoId)?.TipoArea;
                registro.Año = registro.Fecha.Value.Year;
                registro.Recibido = hDotacion.FirstOrDefault(x => x.Nro == registro.Nro)?.Recibido;
                registro.Empresa = empleados.FirstOrDefault(x => x.Id == registro.EmpleadoId)?.Empresa;
                registro.CantidadEntregas = hDotacion.FirstOrDefault(x => x.Nro == registro.Nro)?.CantidadPendiente;

                if (string.IsNullOrEmpty(Area))
                {
                    var a = empleados.Any(x => x.Id == registro.EmpleadoId && x.TipoArea == "Asistenciales CO");
                    var b = empleados.Any(x => x.Id == registro.EmpleadoId && x.TipoArea == "Administrativos CO");
                    if (a != false)
                    {
                        asistencialesCount += 1;
                    }
                    if (b != false)
                    {
                        administrativosCount += 1;
                    }
                }
            }

            var err = TempData["Msg"] as string;
            ViewBag.Message = err;
            ViewBag.Asistenciales = asistencialesCount;
            ViewBag.Administrativos = administrativosCount;

            return View(datos);
        }

        // GET: Dotacion/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Dotacion/Edit/5
        public ActionResult EditarCategoria()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("DotacionListado"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            ViewBag.Empleados = db.Empleados.Where(x => x.Activo == "SI").Select(x=> new { Nombre = x.Nombres, x.Id}).ToList();
            
            return View();
        }

        public ActionResult HistorialDotacion(string Nro, string Empleado)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("DotacionListado"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            ViewBag.Empleados = db.Empleados.Where(x => x.Activo == "SI" && x.TipoArea == "Asistenciales CO" || x.TipoArea == "Administrativos CO").Select(x => new { x.Id, x.Nombres }).ToList();
            var registros = db.HistorialDotacion.AsQueryable();

            if (!string.IsNullOrEmpty(Nro))
            {
                registros = registros.Where(x => x.Nro.Contains(Nro));
            }

            if (!string.IsNullOrEmpty(Empleado))
            {
                int.TryParse(Empleado, out int id);
                registros = registros.Where(x => x.EmpleadoId == id);
            }
            var registrosOrdenados = registros.OrderByDescending(x => x.Fecha).ToList();

            var empleadosIds = registros.Select(x => x.EmpleadoId).Distinct().ToList();
            var empleados = db.Empleados.Where(x => empleadosIds.Contains(x.Id)).ToList();

            foreach (var registro in registros)
            {
                registro.NombreEmpleado = empleados.FirstOrDefault(x => x.Id == registro.EmpleadoId)?.Nombres;
            }

            if (string.IsNullOrEmpty(Nro) && string.IsNullOrEmpty(Empleado))
            {
                return View(new List<HistorialDotacion>());
            }

            return View(registrosOrdenados);
        }

        public ActionResult AgregarRegistro()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("DotacionListado"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Empleados = db.Empleados.Where(x => x.Activo == "SI").Select(x=> new { x.Id, x.Nombres}).ToList();

            var errMsg = TempData["ErrorMessage"] as string;
            ViewBag.Message = errMsg;
            return View();
        }

        [HttpPost]
        public ActionResult AgregarRegistro(int Empleados,DateTime Fecha)
        {
            var v = _repo.AgregarRegistro(Empleados, Fecha);
            TempData["ErrorMessage"] = v;

            return RedirectToAction("AgregarRegistro");
        }

        [HttpPost]
        public JsonResult FirmaQR()
        {
            string message = "";

            var id = HttpContext.Request.Params["result"];
            var Nro = HttpContext.Request.Params["NroRegistro"];
            var Cantidad = HttpContext.Request.Params["CantidadEntrega"];
            
            try
            {
                message = _repo.FirmarQR(id, Nro, Cantidad);
                return Json(message);
            }
            catch (Exception ex)
            {
                return Json("Error" + ex);
            }
        }

        [HttpPost]
        public ActionResult NotificarDotacion()
        {
            try
            {
                var v = _repo.EnvioCorreoDerechoDotacion();
                TempData["Msg"] = v;
                return RedirectToAction("Listado");
            }
            catch
            {
                return RedirectToAction("Listado");
            }
        }
    }
}
