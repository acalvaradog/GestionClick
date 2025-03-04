using Adm_AutoGestion.Models;
using Adm_AutoGestion.Models.EvaDesempeno;
using Adm_AutoGestion.Models.EvaluacionDesempenoRa;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;


namespace Adm_AutoGestion.Controllers
{
    public class EvaluacionDesempenoRaController : Controller
    {
        // GET: EvaluacionDesempenoRa
        private readonly EvaluacionDesempenoRepository _evaluacionRepository;
        private readonly AutogestionContext _context;

        public EvaluacionDesempenoRaController()
        {
            _context = new AutogestionContext();
            _evaluacionRepository= new EvaluacionDesempenoRepository(_context);
        }


        public  ActionResult Index(string UnidadOrg, string TrabajadorS)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EvaluacionDesempenoRa"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }


            var empleadosession = Convert.ToString(Session["Empleado"]);
            var empresasesion = Session["Empresa"].ToString();
            
           
                ViewBag.Empleado = _context.Empleados.Where(f => f.Activo != "NO"  && f.Empresa == empresasesion ).OrderBy(x => x.Nombres).ToList();
                var areaDescripcionGroups = _context.Empleados.Where(f => f.Activo != "NO" && f.AreaDescripcion != null && f.AreaDescripcion != "" && f.Empresa == empresasesion).GroupBy(b => b.AreaDescripcion).ToList();
                ViewBag.AreaDescripcion = new List<SelectListItem>();
                foreach (var x in areaDescripcionGroups)
                {

                    Empleado Item = x.FirstOrDefault();
                    if (Item.UnidadOrganizativa != "00003103" || Item.UnidadOrganizativa != "00003105" || Item.UnidadOrganizativa != "00003109" ||  Item.UnidadOrganizativa != "00003110" ||  Item.UnidadOrganizativa != "00003117" || Item.UnidadOrganizativa != "00003625" || Item.UnidadOrganizativa != "00003700")
                    {

                        ViewBag.AreaDescripcion.Add(new SelectListItem() { Value = Item.UnidadOrganizativa, Text = "" + Item.AreaDescripcion });
                    }

                    // ViewBag.AreaDescripcion.Add(new SelectListItem() { Value = Item.AreaDescripcion, Text = Item.AreaDescripcion });
                }

                var empleados =  _evaluacionRepository.GetAllEmpleados(UnidadOrg, TrabajadorS); 

            return View(empleados);
        }

        // GET: EvaluacionDesempenoRa/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EvaluacionDesempenoRa/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EvaluacionDesempenoRa/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EvaluacionDesempenoRa/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EvaluacionDesempenoRa/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EvaluacionDesempenoRa/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EvaluacionDesempenoRa/Delete/5
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


        public ActionResult EvaluacionDesempeno(int id) {

            ViewBag.Empleado = _context.Empleados.FirstOrDefault(x => x.Id == id);

            string unidadorgan = ViewBag.Empleado.UnidadOrganizativa;
            string empresa = ViewBag.Empleado.Empresa;

            ViewBag.SubArea = _context.SubAreaEvaluacion.Where(x => x.UnidadOrganizativa == unidadorgan).ToList();

            ViewBag.Evaluador = _context.Empleados.Where(x => x.Empresa == empresa && x.Activo == "SI").OrderBy(x => x.Nombres).ToList();

            ViewBag.Periodo = _context.PeriodoEvaluado.Where(x => x.Estado == true).ToList();

            ViewBag.Indicador  = _context.Indicador
                              .Where(x => x.UnidadOrganizativa == unidadorgan).ToList();

            return PartialView();
        }





    }




//    public ActionResult GuardarEvaluacion(EncabezadoEvaluacion evaluacion)
//    {
//        if (ModelState.IsValid)
//        {
//            try
//            {
//                // Aquí puedes procesar los datos recibidos
//                // Por ejemplo, puedes guardarlos en una base de datos

//                // Si todo sale bien, puedes retornar un JSON con un mensaje de éxito
//                return Json(new { success = true, message = "Evaluación guardada correctamente" });
//            }
//            catch (Exception ex)
//            {
//                // Si ocurre un error, puedes retornar un JSON con un mensaje de error
//                return Json(new { success = false, message = "Error al guardar la evaluación: " + ex.Message });
//            }
//        }
//        else
//        {
//            // Si el modelo no es válido, puedes retornar un JSON con los errores de validación
//            return Json(new { success = false, message = "Error de validación", errors = ModelState.Values.SelectMany(v => v.Errors) });
//        }
//    }
//}

}


















