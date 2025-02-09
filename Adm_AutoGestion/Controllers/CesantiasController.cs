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
    public class CesantiasController : Controller
    {
        private readonly CesantiasRepository _cesantiasRepository;
        private readonly AutogestionContext _context;

        public CesantiasController()
        {
            _context = new AutogestionContext();
            _cesantiasRepository = new CesantiasRepository(_context);
        }

        public async Task<ActionResult> Index(int? empleadoId, DateTime? fechaInicio, DateTime? fechaFin,int? estado)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("CesantiasIndex"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var empresasesion = Session["Empresa"].ToString();
            var solicitudes = await _cesantiasRepository.ObtenerSolicitudesAsync(empleadoId, fechaInicio, fechaFin, estado,empresasesion);
            ViewBag.Empleados = _context.Empleados.Where(x => x.Empresa == empresasesion).ToList();
            ViewBag.Estados = _context.EstadoCesantia.ToList();
            ViewBag.EmpleadoId = empleadoId;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");
            return View(solicitudes);
        }

        public async Task<ActionResult> PorAprobar(int? empleadoId, DateTime? fechaInicio, DateTime? fechaFin)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("CesantiasPorAprobar"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var empresasesion = Session["Empresa"].ToString();
            var solicitudes = await _cesantiasRepository.ObtenerSolicitudesAsync(empleadoId, fechaInicio, fechaFin,1, empresasesion);
            ViewBag.Empleados = _context.Empleados.Where(x=> x.Empresa == empresasesion).ToList();
            ViewBag.EmpleadoId = empleadoId;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");
            return View(solicitudes);
        }

        public async Task<ActionResult> PorPagar(int? empleadoId, DateTime? fechaInicio, DateTime? fechaFin)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("CesantiasPorPagar"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var empresasesion = Session["Empresa"].ToString();
            var solicitudes = await _cesantiasRepository.ObtenerSolicitudesAsync(empleadoId, fechaInicio, fechaFin, 2, empresasesion);
            ViewBag.Empleados = _context.Empleados.Where(x => x.Empresa == empresasesion).ToList();
            ViewBag.EmpleadoId = empleadoId;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");
            return View(solicitudes);
        }

        public async Task<ActionResult> Detalle(int id)
        {
            var solicitud = await _cesantiasRepository.ObtenerSolicitudConDetallesAsync(id);
            if (solicitud == null)
                return HttpNotFound();

            var logs = await _cesantiasRepository.ObtenerLogsPorSolicitudAsync(id);
            ViewBag.Logs = logs;
            ViewBag.Estados = _context.EstadoCesantia.Where(x=> x.Id != 1).ToList();
            return PartialView("_DetalleModal", solicitud);
        }

        public async Task<ActionResult> DetallePorPagar(int id)
        {
            var solicitud = await _cesantiasRepository.ObtenerSolicitudConDetallesAsync(id);
            if (solicitud == null)
                return HttpNotFound();

            var logs = await _cesantiasRepository.ObtenerLogsPorSolicitudAsync(id);
            ViewBag.Logs = logs;
            ViewBag.Estados = _context.EstadoCesantia.Where(x => x.Id != 2).ToList();
            return PartialView("_DetallePorPagarModal", solicitud);
        }


        [HttpPost]
        public async Task<ActionResult> CambiarEstado(int id, int nuevoEstadoId)
        {
            var usuario = Session["NombreEmpleado"].ToString();  // Obtiene el nombre del usuario actual
            await _cesantiasRepository.ActualizarEstadoSolicitudAsync(id, nuevoEstadoId, usuario);
            return RedirectToAction("PorAprobar");
        }

        [HttpPost]
        public async Task<ActionResult> CambiarEstadoPagado(int id, int nuevoEstadoId)
        {
            var usuario = Session["NombreEmpleado"].ToString(); // Obtiene el nombre del usuario actual
            await _cesantiasRepository.ActualizarEstadoSolicitudAsync(id, nuevoEstadoId, usuario);
            return RedirectToAction("PorPagar");
        }
    }
}