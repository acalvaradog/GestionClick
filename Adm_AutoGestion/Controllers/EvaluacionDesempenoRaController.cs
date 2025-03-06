using Adm_AutoGestion.Models;
using Adm_AutoGestion.Models.EvaDesempeno;
using Adm_AutoGestion.Models.EvaluacionDesempenoRa;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Data.Entity;
using System.Web.Script.Serialization;
using OfficeOpenXml.Drawing.Chart;
using System.Globalization;


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

            ViewBag.SubArea = _context.EvaluacionSubArea.Where(x => x.UnidadOrganizativa == unidadorgan).ToList();

            ViewBag.Evaluador = _context.Empleados.Where(x => x.Empresa == empresa && x.Activo == "SI").OrderBy(x => x.Nombres).ToList();

            ViewBag.Periodo = _context.EvaluacionPeriodo.Where(x => x.Estado == true).ToList();

            ViewBag.Indicador  = _context.EvaluacionIndicador
                              .Where(x => x.UnidadOrganizativa == unidadorgan).ToList();

            return PartialView();
        }




        [HttpPost]
        public JsonResult SaveEvaluacion()

        {

            string respuesta = "";

            var empleadoId = HttpContext.Request.Form["EmpleadoId"];
            var evaluadorId = HttpContext.Request.Form["EvaluadorId"];
            var periodo = HttpContext.Request.Form["PeriodoEvaluacion"];
            var retroalimentacion = HttpContext.Request.Form["Retroalimentacion"];
            var planmejora = HttpContext.Request.Form["PlandeMejora"];
            var puntaje= HttpContext.Request.Form["PuntajeFinal"];
           


            // Guardar Evaluacion 
            List<EvaluacionDetalle> detalle = new List<EvaluacionDetalle>();

            EvaluacionEncabezado encabezado = new EvaluacionEncabezado();


            encabezado.EmpleadoId =  int.Parse(empleadoId);
            encabezado.EvaluadorId =  int.Parse(evaluadorId);
            encabezado.PeriodoEvaluacion = periodo;
            encabezado.FechaRegistro = DateTime.Now;
            encabezado.Retroalimentacion = retroalimentacion;
            encabezado.PlandeMejora = planmejora;
            encabezado.PuntajeFinal = float.Parse(puntaje);




            // Agregar la evaluación principal (encabezado)
            _context.EvaluacionEncabezado.Add(encabezado);

            // Guardar los cambios para obtener el EvaluacionId generado
            _context.SaveChanges();


            // Procesar los detalles de la evaluación
            List<EvaluacionDetalle> detallesEvaluacion = new List<EvaluacionDetalle>();
            foreach (string key in HttpContext.Request.Form.Keys)
            {
                if (key.StartsWith("DetallesEvaluacion"))
                {
                    var parts = key.Split('[');
                    if (parts.Length >= 2)
                    {
                        var indexPart = parts[1].Split(']');
                        if (indexPart.Length > 0 && int.TryParse(indexPart[0], out int index))
                        {
                            while (detallesEvaluacion.Count <= index)
                            {
                                detallesEvaluacion.Add(new EvaluacionDetalle());
                            }

                            var propertyParts = parts[1].Split('.');
                            if (propertyParts.Length >= 2)
                            {
                                var propertyName = propertyParts[1];
                                var value = HttpContext.Request.Form[key];

                                switch (propertyName)
                                {
                                    case "IndicadorId":
                                        if (int.TryParse(value, out int indicadorId))
                                        {
                                            detallesEvaluacion[index].IndicadorId = indicadorId;
                                        }
                                        break;
                                    case "BaseNumerador":
                                        if (int.TryParse(value, out int baseNumerador))
                                        {
                                            detallesEvaluacion[index].BaseNumerador = baseNumerador;
                                        }
                                        break;
                                    case "BaseDenominador":
                                        if (int.TryParse(value, out int baseDenominador))
                                        {
                                            detallesEvaluacion[index].BaseDenominador = baseDenominador;
                                        }
                                        break;
                                    case "IndicadorNumerador":
                                        if (int.TryParse(value, out int indicadorNumerador))
                                        {
                                            detallesEvaluacion[index].IndicadorNumerador = indicadorNumerador;
                                        }
                                        break;
                                    case "IndicadorDenominador":
                                        if (int.TryParse(value, out int indicadorDenominador))
                                        {
                                            detallesEvaluacion[index].IndicadorDenominador = indicadorDenominador;
                                        }
                                        break;
                                    case "Porcentaje":
                                        if (float.TryParse(value, out float porcentaje))
                                        {
                                            detallesEvaluacion[index].Porcentaje = porcentaje;
                                        }
                                        break;
                                }

                                detallesEvaluacion[index].EvaluacionId = encabezado.Id;
                            }
                        }


                       
                    }
                   

                }
            }


            _context.EvaluacionDetalle.AddRange(detallesEvaluacion);
            _context.SaveChanges();

            respuesta = "Datos Guardados correctamente.";

            return Json(new { respuesta });
        }
    

        }

}


    




















