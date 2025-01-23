using Adm_AutoGestion.DTO;
using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Adm_AutoGestion.Controllers
{
    public class DosimetriaController : Controller
    {
        private readonly DosimetriaRepository _repository;
        private readonly AutogestionContext _context;

        public DosimetriaController()
        {
            _context = new AutogestionContext();
            _repository = new DosimetriaRepository(_context);
        }

        public ActionResult Index()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("DosimentriaIndex"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var empresasesion = Session["Empresa"].ToString();
            ViewBag.Empleados = _context.Empleados.Where(x => x.Empresa == empresasesion).ToList();
            ViewBag.Sedes = _context.Sede.ToList();
            var registros = _repository.GetAll();
            return View(registros.ToList());
        }

        public PartialViewResult Create()
        {
            var empresasesion = Session["Empresa"].ToString();
            ViewBag.Empleados = _context.Empleados.Where(x => x.Empresa == empresasesion).ToList();
            ViewBag.Sedes = _context.Sede.ToList();
            ViewBag.Anios = Enumerable.Range(2024, 7).Select(a => new { Value = a, Text = a.ToString() }).ToList();
            ViewBag.Meses = Enumerable.Range(1, 12).Select(m => new
            {
                Value = m,
                Text = new DateTime(1, m, 1).ToString("MMMM", new CultureInfo("es-ES"))
            }).ToList();
            return PartialView("_CreateEdit", new Dosimetria());
        }

        public PartialViewResult Edit(int id)
        {
            var empresasesion = Session["Empresa"].ToString();
            var registro = _repository.GetById(id);
            ViewBag.Empleados = _context.Empleados.Where(x => x.Empresa == empresasesion).ToList();
            ViewBag.Sedes = _context.Sede.ToList();
            ViewBag.Anios = Enumerable.Range(2024, 7).Select(a => new { Value = a, Text = a.ToString() }).ToList();
            ViewBag.Meses = Enumerable.Range(1, 12).Select(m => new
            {
                Value = m,
                Text = new DateTime(1, m, 1).ToString("MMMM", new CultureInfo("es-ES"))
            }).ToList();
            return PartialView("_CreateEdit", registro);
        }

        [System.Web.Http.HttpPost]
        public JsonResult Save(Dosimetria model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    _repository.Add(model);
                }
                else
                {
                    _repository.Update(model);
                }
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
        }

        public ActionResult ReporteDosimetria(int? anio, int? mes, int? empleadoId, int? sedeId)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("DosimetriaReporte"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var empresasesion = Session["Empresa"].ToString();
            ViewBag.Empleados = _context.Empleados.Where(x => x.Empresa == empresasesion).ToList();
            ViewBag.Sedes = _context.Sede.ToList();
            if (!anio.HasValue)
            {
                ViewBag.Error = "El campo año es obligatorio.";
                return View(new List<ReporteDosimetriaViewModelDTO>());
            }

            // Obtener datos desde el repositorio
            var empleados = _repository.ObtenerReporteDosimetria(anio.Value, mes, empleadoId,sedeId);

            return View(empleados);
        }

        public ActionResult Dashboard(int anio = 2025, int? mes = null, int? empleadoId = null, int? sedeId = null)
        {
            var datos = _repository.ObtenerReporteDosimetria(anio, mes, empleadoId, sedeId);

            // Filtrar datos para gráficos de totales anuales
            var datosHp10 = datos.Where(d => d.TotalAnualHp10 > 0).ToList();
            var datosHp3 = datos.Where(d => d.TotalAnualHp3 > 0).ToList();

            // Filtrar datos para gráficos de acumulado
            var datosAcumuladoHp10 = datos.Where(d => d.TotalAcumuladoHp10 > 0).ToList();
            var datosAcumuladoHp3 = datos.Where(d => d.TotalAcumuladoHp3 > 0).ToList();

            var limites = new
            {
                LimiteMensualHp10 = 2.0m,
                LimiteMensualHp3 = 1.0m,
                LimiteAnualHp10 = 20.0m,
                LimiteAnualHp3 = 10.0m,
                LimiteAcumuladoHp10 = 100.0m,
                LimiteAcumuladoHp3 = 50.0m
            };

            return View(new DashboardViewModel
            {
                DatosHp10 = datosHp10,
                DatosHp3 = datosHp3,
                DatosAcumuladoHp10 = datosAcumuladoHp10,
                DatosAcumuladoHp3 = datosAcumuladoHp3,
                Limites = limites
            });
        }





    }
}
