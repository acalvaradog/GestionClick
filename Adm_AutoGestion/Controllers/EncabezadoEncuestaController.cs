using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;


namespace Adm_AutoGestion.Controllers
{
    public class EncabezadoEncuestaController : Controller
    {
        //
        // GET: /EncabezadoEncuesta/

        private EncabezadoEncuestaRepository _repo;

        public EncabezadoEncuestaController()
        {
            _repo = new EncabezadoEncuestaRepository();

        }



       public ActionResult Index(string Unidad, string Cedula, string Sospechoso, string Transporte, string Trabajo, string Empresa, string FechaIni, string FechaFin, string Empleado, string Respuesta, string Pregunta)
        {
            List<EncabezadoEncuesta> Item = new List<EncabezadoEncuesta>();

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EncabezadoEncuesta"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            int pregunta1 = 0;
            Int32.TryParse(Pregunta, out pregunta1);

            List<EncabezadoEncuesta> enc = new List<EncabezadoEncuesta>();



            using (var db = new AutogestionContext())
            {

                ViewBag.Preguntas = db.Preguntas.Where(e => e.Id == 12 || e.Id == 13 ).ToList();


                DateTime Fecha1 = DateTime.Now;
                DateTime Fecha2 = DateTime.Now;


                if (!DateTime.TryParse(FechaIni, out Fecha1))
                {
                    Fecha1 = new DateTime();
                }

                if (!DateTime.TryParse(FechaFin, out Fecha2))
                {
                    Fecha2 = DateTime.Now;
                }

                if (pregunta1 != 0 && Respuesta != "")
                {
                    var Item2 = db.EncabezadoEncuesta.Join(db.Encuesta, EE => EE.Id, E => E.EncabezadoEncuesta_Id,
                                          (EE, E) => new { EE, E }).Where(EE => EE.EE.Empleado.AreaDescripcion.Contains(Unidad)
                                                                          && EE.EE.Empleado.Documento.Contains(Cedula)
                                                                          && EE.EE.Transporte.Contains(Transporte)
                                                                          && EE.EE.ModoTrabajo.Contains(Trabajo)
                                                                          && EE.EE.Empresa.Contains(Empresa)
                                                                          && DbFunctions.TruncateTime(EE.EE.Fecha) >= Fecha1
                                                                          && DbFunctions.TruncateTime(EE.EE.Fecha) <= Fecha2
                                                                          && EE.EE.Sospechoso.Contains(Sospechoso)
                                                                          && EE.EE.Empleado.Nombres.Contains(Empleado)
                                                                           && EE.E.NumeroPregunta == pregunta1
                                                                           && EE.E.Respuesta.Contains(Respuesta)).ToList();
                    foreach (var fila in Item2)
                    {

                        EncabezadoEncuesta encab = new EncabezadoEncuesta();
                        encab.Id = fila.EE.Id;
                        encab.EmpleadoId = fila.EE.EmpleadoId;
                        encab.Cargo = fila.EE.Cargo;
                        encab.UnidadOrganizativa = fila.EE.UnidadOrganizativa;
                        encab.Eps = fila.EE.Eps;
                        encab.Transporte = fila.EE.Transporte;
                        encab.ModoTrabajo = fila.EE.ModoTrabajo;
                        encab.Sospechoso = fila.EE.Sospechoso;
                        encab.Empresa = fila.EE.Empresa;
                        encab.Fecha = fila.EE.Fecha;
                        encab.Temperatura = fila.EE.Temperatura;
                        encab.Cerco = fila.EE.Cerco;
                        encab.ListadoEmpleado = db.Empleados.Where(e => e.Id == fila.EE.EmpleadoId).ToList();
                        encab.Encuesta = db.Encuesta.Where(e => e.EncabezadoEncuesta.Id == fila.EE.Id && (e.NumeroPregunta == 12 || e.NumeroPregunta == 13 || e.NumeroPregunta == 20 || e.NumeroPregunta == 21)).ToList();


                        enc.Add(encab);
                        Item = enc;
                    }

                }
                else
                {
                    Item = db.EncabezadoEncuesta.Where(e =>
                                                             e.Empleado.AreaDescripcion.Contains(Unidad)
                                                             && e.Empleado.Documento.Contains(Cedula)
                                                             && e.Transporte.Contains(Transporte)
                                                             && e.ModoTrabajo.Contains(Trabajo)
                                                             && e.Empresa.Contains(Empresa)
                                                             && DbFunctions.TruncateTime(e.Fecha) >= Fecha1
                                                             && DbFunctions.TruncateTime(e.Fecha) <= Fecha2
                                                             && e.Sospechoso.Contains(Sospechoso)
                                                              && e.Empleado.Nombres.Contains(Empleado)

                                                             ).ToList();


                    foreach (var fila in Item)
                    {

                        EncabezadoEncuesta encab = new EncabezadoEncuesta();
                        encab.Id = fila.Id;
                        encab.EmpleadoId = fila.EmpleadoId;
                        encab.Cargo = fila.Cargo;
                        encab.UnidadOrganizativa = fila.UnidadOrganizativa;
                        encab.Eps = fila.Eps;
                        encab.Transporte = fila.Transporte;
                        encab.ModoTrabajo = fila.ModoTrabajo;
                        encab.Sospechoso = fila.Sospechoso;
                        encab.Empresa = fila.Empresa;
                        encab.Fecha = fila.Fecha;
                        encab.Temperatura = fila.Temperatura;
                        encab.Cerco = fila.Cerco;
                        encab.ListadoEmpleado = db.Empleados.Where(e => e.Id == fila.EmpleadoId).ToList();
                        encab.Encuesta = db.Encuesta.Where(e => e.EncabezadoEncuesta.Id == fila.Id && (e.NumeroPregunta == 12 || e.NumeroPregunta == 13 || e.NumeroPregunta == 20 || e.NumeroPregunta == 21)).ToList();


                        enc.Add(encab);
                        Item = enc;
                    }
                }
                    return View(Item);
            }
        }


       public ActionResult Certificado(int Id)
       {
           var mensaje = "";
           var Color = "";
           var Letra = "";
           using (var db = new AutogestionContext())
           {
               List<EncabezadoEncuesta> Items = db.EncabezadoEncuesta.Where(e => e.Id == Id).ToList();
               foreach (EncabezadoEncuesta Item in Items)
               {
                   Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                   ViewBag.Fecha = Item.Fecha;
                   ViewBag.Nombres = Item.Empleado.Nombres;

                   if (Item.Sospechoso == "Verde1")
                   {
                       mensaje = "Verde1";
                       Color = "green";
                       Letra = "white";
                   }
                   if (Item.Sospechoso == "Morado2")
                   {
                       mensaje = "Morado2";
                       Color = "#7D2181";
                       Letra = "white";
                   }
                   if (Item.Sospechoso == "Naranja3")
                   {
                       mensaje = "Naranja3";
                       Color = "#ff8000 ";
                       Letra = "black";
                   }
                   if (Item.Sospechoso == "Morado4")
                   {
                       mensaje = "Morado4";
                       Color = "#7D2181 ";
                       Letra = "white";
                   }
                   if (Item.Sospechoso == "Verde5")
                   {
                       mensaje = "Verde5";
                       Color = "green ";
                       Letra = "white";
                   }
                   if (Item.Sospechoso == "Verde6")
                   {
                       mensaje = "Verde6";
                       Color = "green ";
                       Letra = "white";
                   }
                   if (Item.Sospechoso == "Verde7")
                   {
                       mensaje = "Verde7";
                       Color = "green ";
                       Letra = "white";
                   }
                   if (Item.Sospechoso == "Morado8")
                   {
                       mensaje = "Morado8";
                       Color = "#7D2181 ";
                       Letra = "white";
                   }
                   if (Item.Sospechoso == "Verde9")
                   {
                       mensaje = "Verde9";
                       Color = "green ";
                       Letra = "white";
                   }
                   if (Item.Sospechoso == "Morado10")
                   {
                       mensaje = "Morado10";
                       Color = "7D2181 ";
                       Letra = "white";
                   }
                   if (Item.Sospechoso == "Naranja11")
                   {
                       mensaje = "Naranja11";
                       Color = "ff8000 ";
                       Letra = "black";
                   }
                   if (Item.Sospechoso == "Morado12")
                   {
                       mensaje = "Morado12";
                       Color = "7D2181 ";
                       Letra = "white";
                   }
                   if (Item.Sospechoso == "Naranja13")
                   {
                       mensaje = "Naranja13";
                       Color = "ff8000 ";
                       Letra = "black";
                   }
                   if (Item.Sospechoso == "Morado14")
                   {
                       mensaje = "Morado14";
                       Color = "7D2181 ";
                       Letra = "white";
                   }
                   if (Item.Sospechoso == "Morado15")
                   {
                       mensaje = "Morado15";
                       Color = "7D2181 ";
                       Letra = "white";
                   }
                   if (Item.Sospechoso == "Morado16")
                   {
                       mensaje = "Morado16";
                       Color = "7D2181 ";
                       Letra = "white";
                   }

                   if (Item.Sospechoso == "Verde")
                   {
                       mensaje = "Verde";
                       Color = "green ";
                       Letra = "white";
                   }

                   if (Item.Sospechoso == "Amarillo")
                   {
                       mensaje = "Amarillo";
                       Color = "yellow ";
                       Letra = "white";
                   }

                   if (Item.Sospechoso == "Rojo")
                   {
                       mensaje = "Rojo";
                       Color = "red ";
                       Letra = "white";
                   }

                   ViewBag.datos = mensaje;
                   ViewBag.ColorF = Color;
                   ViewBag.ColorL = Letra;

               }
               return PartialView(Items);

           }
       }


        //public ActionResult Index(string Unidad, string Sospechoso, string Transporte, string Trabajo, string Empresa, string FechaIni, string FechaFin, string Empleado)
        //{
        //    List<EncabezadoEncuesta> Item = new List<EncabezadoEncuesta>();

        //    List<string> funciones = Acceso.Validar(Session["Empleado"]);

        //    if (Acceso.EsAnonimo)
        //    {
        //        return RedirectToAction("Login", "Login");
        //    }
        //    else if (!Acceso.EsAnonimo && !funciones.Contains("EncabezadoEncuesta"))
        //    {
        //        Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
        //        return RedirectToAction("Index", "Login");
        //    }


        //    using (var db = new AutogestionContext())
        //    {
        //        DateTime Fecha1 = DateTime.Now;
        //        DateTime Fecha2 = DateTime.Now;


        //        if (!DateTime.TryParse(FechaIni, out Fecha1))
        //        {
        //            Fecha1 = new DateTime();
        //        }

        //        if (!DateTime.TryParse(FechaFin, out Fecha2))
        //        {
        //            //Fecha1 = DateTime.Now;
        //            Fecha2 = DateTime.Now;
        //        }

        //        //if (!Infectado)
        //        //{
        //        Item = db.EncabezadoEncuesta.Where(e =>
        //                                                e.UnidadOrganizativa.Contains(Unidad)
        //                                                && e.Transporte.Contains(Transporte)
        //                                                && e.ModoTrabajo.Contains(Trabajo)
        //                                                && e.Empresa.Contains(Empresa)
        //                                                && DbFunctions.TruncateTime(e.Fecha) >= Fecha1
        //                                                && DbFunctions.TruncateTime(e.Fecha) <= Fecha2
        //                                                && e.Sospechoso.Contains(Sospechoso)
        //                                                &&
        //                                                e.Empleado.Nombres.Contains(Empleado)
        //                                                ).ToList();


        //        foreach (EncabezadoEncuesta Items in Item)
        //        {
        //            Items.ListadoEmpleado = db.Empleados.Where(e => e.Id == Items.EmpleadoId).ToList();

        //            List<SeguimientoSintomas> validar = new List<SeguimientoSintomas>();
        //            validar = db.SeguimientoSintomas.Where(e => e.EncabezadoEncuestaId == Items.Id).ToList();

        //            if (validar.Count == 0)
        //            {
        //                Items.sintomas = "0";
        //            }
        //            else
        //            {
        //                Items.sintomas = "1";
        //            }

        //        }

        //    }


        //    //}
        //    //else {
        //    //    ViewBag.ErrorData = "Los criterios de la consulta no son validos."; 
        //    //}


        //    return View(Item);
        //}


        //Graficass
        private class indicator
        {
            public int count { get; set; }
            public string type { get; set; }
            public string var { get; set; }
        }

        private class indicator2
        {
            public int count { get; set; }
            public string type { get; set; }
            public string variable { get; set; }
        }


        public ActionResult DatosGraficas(string message = " ")
        {

            ViewBag.Message = message;
            return View();
        }


        [HttpPost]
        public JsonResult DatosGraficas()
        {
            List<indicator> Items = new List<indicator>();

            using (AutogestionContext db = new AutogestionContext())
            {
                var Transporte = db.EncabezadoEncuesta.GroupBy(e => e.Transporte).Select(e => new { Name = e.Key, Count = e.Count() }).ToArray();
                var ModoTrabajo = db.EncabezadoEncuesta.GroupBy(e => e.ModoTrabajo).Select(e => new { Name = e.Key, Count = e.Count() }).ToArray();
                var Sospechoso = db.EncabezadoEncuesta.GroupBy(e => e.Sospechoso).Select(e => new { Name = e.Key, Count = e.Count() }).ToArray();

                foreach (var item in Transporte) { Items.Add(new indicator() { count = item.Count, type = "Transporte", var = item.Name }); }
                foreach (var item in ModoTrabajo) { Items.Add(new indicator() { count = item.Count, type = "Modo de trabajo", var = item.Name }); }
                foreach (var item in Sospechoso) { Items.Add(new indicator() { count = item.Count, type = "Sospechoso", var = item.Name }); }
            }

            return Json(Items.ToArray());
        }

        [HttpPost]
        public JsonResult GraficaSospechosos()
        {
            List<indicator2> Items = new List<indicator2>();

            using (AutogestionContext db = new AutogestionContext())
            {
                var Sospechoso = db.EncabezadoEncuesta.Where(e => e.Sospechoso == "Rojo").GroupBy(e => DbFunctions.TruncateTime(e.Fecha)).Select(e => new { Name = e.Key, Count = e.Count() }).ToArray();

                foreach (var item in Sospechoso) { Items.Add(new indicator2() { count = item.Count, type = "Sospechoso", variable = item.Name.ToString() }); }
            }

            return Json(Items.ToArray());
        }

        [HttpPost]
        public JsonResult GraficaNoSospechosos()
        {
            List<indicator2> Items = new List<indicator2>();

            using (AutogestionContext db = new AutogestionContext())
            {
                var Sospechoso = db.EncabezadoEncuesta.Where(e => e.Sospechoso == "Amarillo").GroupBy(e => DbFunctions.TruncateTime(e.Fecha)).Select(e => new { Name = e.Key, Count = e.Count() }).ToArray();

                foreach (var item in Sospechoso) { Items.Add(new indicator2() { count = item.Count, type = "Sospechoso", variable = item.Name.ToString() }); }
            }

            return Json(Items.ToArray());
        }

        [HttpPost]
        public JsonResult GraficaVerde()
        {
            List<indicator2> Items = new List<indicator2>();

            using (AutogestionContext db = new AutogestionContext())
            {
                var Sospechoso = db.EncabezadoEncuesta.Where(e => e.Sospechoso == "Verde").GroupBy(e => DbFunctions.TruncateTime(e.Fecha)).Select(e => new { Name = e.Key, Count = e.Count() }).ToArray();

                foreach (var item in Sospechoso) { Items.Add(new indicator2() { count = item.Count, type = "Sospechoso", variable = item.Name.ToString() }); }
            }

            return Json(Items.ToArray());
        }









        //
        // GET: /EncabezadoEncuesta/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /EncabezadoEncuesta/Create

        public ActionResult Create(string Id, string Empleado)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("SintomasCrear"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            ViewBag.Id = Id;
            ViewBag.EmpleadoId = Empleado;
            return View();
        }

        //
        // POST: /EncabezadoEncuesta/Create

        [HttpPost]
        public ActionResult Create(SeguimientoSintomas model, string Id, string EmpleadoId)
        {
            try
            {
                // TODO: Add insert logic here
                //if (ModelState.IsValid)
                //{

                    _repo.Crear(model,Id,EmpleadoId);
                    return RedirectToAction("Index");
                //}
            }
            catch
            {

            }
            return View();
        }

        //
        // GET: /EncabezadoEncuesta/Edit/5

        public ActionResult Edit(string Id, string Empleado)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("SintomasEditar"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }


            ViewBag.EncabezadoId = Id;
            ViewBag.EmpleadoId = Empleado;
            using (var db = new AutogestionContext())
            {
                int id = 0;
                Int32.TryParse(Id, out id);
                SeguimientoSintomas sintomas = db.SeguimientoSintomas.FirstOrDefault(e => e.EncabezadoEncuestaId == id); 
                //Empleado empleado = db.Empleados.Find(id);
                ViewBag.Idsintomas = sintomas.Id;
                if (sintomas == null)
                {
                    return HttpNotFound();
                }
                return View(sintomas);
            }
           

        }

        //
        // POST: /EncabezadoEncuesta/Edit/5

        [HttpPost]
        public ActionResult Edit(SeguimientoSintomas model, string EncabezadoId, string EmpleadoId, string Idsintomas)
        {
            try
            {
                // TODO: Add update logic here
                _repo.Editar(model, EncabezadoId, EmpleadoId, Idsintomas);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /EncabezadoEncuesta/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /EncabezadoEncuesta/Delete/5

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


        public ActionResult ResultadosXPregunta(string message = " ")
        {
            List<SelectListItem> lst = new List<SelectListItem>();
     
            using (AutogestionContext db = new AutogestionContext())
            {

            
                var lista = db.Preguntas.ToList();
                foreach (var x in lista)
                {
                    lst.Add(new SelectListItem() { Text = x.Pregunta.ToString(), Value = x.Id.ToString() });
                }



                ViewBag.Opciones = lst;




            }



            ViewBag.Message = message;
            return View();
        }

        public ActionResult RepuestasXPreguntas(string FechaIni, string FechaFin, int Pregunta)
        {
            List<Encuesta> Item = new List<Encuesta>();

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ResultadosXPreguntas"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            using (var db = new AutogestionContext())
            {
                DateTime Fecha1IC = DateTime.Now;
                DateTime Fecha2IC = DateTime.Now;
                var empleadoglobal = new Empleado();

                if (FechaIni != "" && FechaFin != "")
                {

                    if (!DateTime.TryParse(FechaIni, out Fecha1IC))
                    {
                        Fecha1IC = new DateTime();
                    }

                    if (!DateTime.TryParse(FechaFin, out Fecha2IC))
                    {
                        Fecha2IC = DateTime.Now;
                    }

                    //List<EncabezadoEncuesta> Encabezado = new List<EncabezadoEncuesta>();
                    //int numerop = 0;
                    //Int32.TryParse(Pregunta, out numerop);




                    var empleado = db.EncabezadoEncuesta.Join(db.Encuesta,
s => s.Id,
sa => sa.EncabezadoEncuesta.Id,
(s, sa) => new { encuesta = s, respuesta = sa })
.Where(b => DbFunctions.TruncateTime(b.encuesta.Fecha) <= Fecha1IC)
.Where(b => DbFunctions.TruncateTime(b.encuesta.Fecha) >= Fecha2IC)
.Where(b => b.respuesta.NumeroPregunta == Pregunta && b.respuesta.Respuesta == "si")
.Select(x => new { x.encuesta.Empleado.Documento, x.encuesta.Empleado.Nombres, x.encuesta.Empleado.Area, x.encuesta.Empleado.Empresa, x.encuesta.Empleado.Telefono, x.encuesta.Cargo, x.encuesta.Empleado.Correo }).ToArray();

                    return Json(empleado);

                }

                return Json(empleadoglobal);
            }




         
        }
    }
}
