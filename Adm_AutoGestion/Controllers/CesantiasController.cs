using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
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
            ViewBag.Empleados = _context.Empleados.Where(x => x.Empresa == empresasesion).ToList().OrderBy(x=> x.Nombres);
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
            ViewBag.Empleados = _context.Empleados.Where(x=> x.Empresa == empresasesion).ToList().OrderBy(x => x.Nombres);
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
            ViewBag.Empleados = _context.Empleados.Where(x => x.Empresa == empresasesion).ToList().OrderBy(x => x.Nombres);
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
            return PartialView("_Detalles", solicitud);
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

        [HttpPost]
        public async Task<ActionResult> SubirCartaFondo(int id, HttpPostedFileBase cartaFondo)
        {
            var solicitud = await _cesantiasRepository.ObtenerSolicitudConDetallesAsync(id);
            if (cartaFondo == null)
            {
                TempData["Error"] = "Por favor, seleccione un archivo PDF.";
                return PartialView("_DetallePorPagar", solicitud);
            }

            // Verificar que el archivo sea un PDF
            if (Path.GetExtension(cartaFondo.FileName).ToLower() != ".pdf")
            {
                TempData["Error"] = "El archivo debe ser un PDF.";
                return PartialView("_DetallePorPagar", solicitud);
            }

            // Guardar el archivo en el servidor

            var nombreArchivo = $"CartaFondo_{id}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            //var rutaArchivo = Path.Combine(rutaCarpeta, nombreArchivo);
            var rutaArchivo = Path.Combine(Server.MapPath("~/AnexosCesantias"), nombreArchivo);

            cartaFondo.SaveAs(rutaArchivo);

            // Guardar la ruta del archivo en la base de datos
           
            if (solicitud != null)
            {
                solicitud.CartaFondo = nombreArchivo;
                await _cesantiasRepository.ActualizarSolicitudAsync(solicitud);
            }

            //return RedirectToAction("Detalles", new { id = id }); // Redirige a la vista de detalles
            TempData["Success"] = "La carta del fondo se ha subido correctamente.";
            //return RedirectToAction("_DetallePorPagar", new { id = id });


            return PartialView("_DetallePorPagar", solicitud);
        }
    }
}