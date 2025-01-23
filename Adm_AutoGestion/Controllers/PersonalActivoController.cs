using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Adm_AutoGestion.Models;


namespace Adm_AutoGestion.Controllers
{
    public class PersonalActivoController : Controller
    {

        public ActionResult Consulta(string message = " ")
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            List<SelectListItem> lst2 = new List<SelectListItem>();
           
            using (AutogestionContext db = new AutogestionContext())
            {

                var empleadoid = Session["Empleado"];
                var empleado = db.Empleados.Find(empleadoid);

                var lista = db.PersonalActivo.Where(x => x.Superior == empleado.NroEmpleado || x.Jefe == empleado.NroEmpleado || x.Director == empleado.NroEmpleado).Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                foreach (var x in lista)
                {
                    lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }


                //var lista = db.PersonalActivo.Where(x => x.Superior == empleado.NroEmpleado).Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                //foreach (var x in lista)
                //{
                //    lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                //}

                // lista = db.PersonalActivo.Where(x => x.Jefe == empleado.NroEmpleado).Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                //foreach (var x in lista)
                //{
                //    lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                //}


                //lista = db.PersonalActivo.Where(x => x.Director == empleado.NroEmpleado).Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                //foreach (var x in lista)
                //{
                //    lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                //}


                ViewBag.Opciones = lst;



                var lista2 = db.PersonalActivo.Where(x => x.Jefe == empleado.NroEmpleado).Select(x => new { x.Empresa }).GroupBy(b => b.Empresa).ToList();
                foreach (var x in lista2)
                {
                    lst2.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }

                if (lst2.Count < 1)
                {
                    lista2 = db.PersonalActivo.Where(x => x.Superior == empleado.NroEmpleado).Select(x => new { x.Empresa }).GroupBy(b => b.Empresa).ToList();
                    foreach (var x in lista2)
                    {
                        lst2.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                    }

                }

                lista2 = db.PersonalActivo.Where(x => x.Director == empleado.NroEmpleado).Select(x => new { x.Empresa }).GroupBy(b => b.Empresa).ToList();
                foreach (var x in lista2)
                {
                    lst2.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }

                ViewBag.Empresas = lst2;

            }

          

            ViewBag.Message = message;
            return View();
        }

        public ActionResult ConsultaGlobal(string message = " ")
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            List<SelectListItem> lst2 = new List<SelectListItem>();

            using (AutogestionContext db = new AutogestionContext())
            {

                var empleadoid = Session["Empleado"];
                var empleado = db.Empleados.Find(empleadoid);

                var lista = db.PersonalActivo.Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                foreach (var x in lista)
                {
                    lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }

             

                ViewBag.Opciones = lst;



                var lista2 = db.PersonalActivo.Select(x => new { x.Empresa }).GroupBy(b => b.Empresa).ToList();
                foreach (var x in lista2)
                {
                    lst2.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }

                        

                ViewBag.Empresas = lst2;

            }



            ViewBag.Message = message;
            return View();
        }


        public ActionResult Respuestas(string message = " ")
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            List<SelectListItem> lst2 = new List<SelectListItem>();

            using (AutogestionContext db = new AutogestionContext())
            {

                var empleadoid = Session["Empleado"];
                var empleado = db.Empleados.Find(empleadoid);

                var lista = db.PersonalActivo.Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                foreach (var x in lista)
                {
                    lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }



                ViewBag.Opciones = lst;



                var lista2 = db.PersonalActivo.Select(x => new { x.Empresa }).GroupBy(b => b.Empresa).ToList();
                foreach (var x in lista2)
                {
                    lst2.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }



                ViewBag.Empresas = lst2;

            }



            ViewBag.Message = message;
            return View();
        }


       public ActionResult ConsultaEnfermeria(string message = " ")
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            List<SelectListItem> lst2 = new List<SelectListItem>();
           
            using (AutogestionContext db = new AutogestionContext())
            {
                


                var empleadoid = Session["Empleado"];
                var empleado = db.Empleados.FirstOrDefault(x=> x.NroEmpleado == "40000019");

                var lista = db.PersonalActivo.Where(x => x.Superior == empleado.NroEmpleado).Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                foreach (var x in lista)
                {
                    lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }

                 lista = db.PersonalActivo.Where(x => x.Jefe == empleado.NroEmpleado).Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                foreach (var x in lista)
                {
                    lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }


                lista = db.PersonalActivo.Where(x => x.Director == empleado.NroEmpleado).Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                foreach (var x in lista)
                {
                    lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }


                ViewBag.Opciones = lst;



                var lista2 = db.PersonalActivo.Where(x => x.Superior == empleado.NroEmpleado).Select(x => new { x.Empresa }).GroupBy(b => b.Empresa ).ToList();
                foreach (var x in lista2)
                {
                    lst2.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }

                if (lst2.Count < 1) { 
                 lista2 = db.PersonalActivo.Where(x => x.Jefe == empleado.NroEmpleado).Select(x => new { x.Empresa }).GroupBy(b => b.Empresa).ToList();
                foreach (var x in lista2)
                {
                    lst2.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }

                }

                lista2 = db.PersonalActivo.Where(x => x.Director == empleado.NroEmpleado).Select(x => new { x.Empresa }).GroupBy(b => b.Empresa).ToList();
                foreach (var x in lista2)
                {
                    lst2.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }

                ViewBag.Empresas = lst2;

            }

          

            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        public JsonResult DatosSinAutoevaluacion(string area,string fechainicial,string fechafinal,string empresa)
        {

            DateTime FechaInicial = Convert.ToDateTime(fechainicial);
            DateTime FechaFinal = Convert.ToDateTime(fechafinal);
            List<indicator> Items = new List<indicator>();
          
            using (AutogestionContext db = new AutogestionContext())
            {
                //var Transporte = db.EncabezadoEncuesta.GroupBy(e => e.Transporte).Select(e => new { Name = e.Key, Count = e.Count() }).ToArray();
                //var ModoTrabajo = db.EncabezadoEncuesta.GroupBy(e => e.ModoTrabajo).Select(e => new { Name = e.Key, Count = e.Count() }).ToArray();
                //var Sospechoso = db.EncabezadoEncuesta.GroupBy(e => e.Sospechoso).Select(e => new { Name = e.Key, Count = e.Count() }).ToArray();

                //foreach (var item in Transporte) { Items.Add(new indicator() { count = item.Count, type = "Transporte", var = item.Name }); }
                //foreach (var item in ModoTrabajo) { Items.Add(new indicator() { count = item.Count, type = "Modo de trabajo", var = item.Name }); }
                //foreach (var item in Sospechoso) { Items.Add(new indicator() { count = item.Count, type = "Sospechoso", var = item.Name }); }
                if (empresa == "")
                {
                    var personal = db.PersonalActivo
                                    .Where(c => !db.EncabezadoEncuesta
                                    .Where(b => DbFunctions.TruncateTime(b.Fecha) <= FechaFinal)
                                    .Where(b => DbFunctions.TruncateTime(b.Fecha) >= FechaInicial)
                                    .Select(b => b.Empleado.Documento)
                                    .Contains(c.Documento)
                                    ).Where(x => x.Area == area ).ToArray();
                                                        return Json(personal);
                }
                else { 
                               var personal = db.PersonalActivo
                                                    .Where(c => !db.EncabezadoEncuesta
                                                        .Where(b => DbFunctions.TruncateTime(b.Fecha) <= DbFunctions.TruncateTime(FechaFinal))
                                                        .Where(b => DbFunctions.TruncateTime(b.Fecha) >= DbFunctions.TruncateTime(FechaInicial))
                                                        .Select(b => b.Empleado.Documento)
                                                        .Contains(c.Documento)
                                                    ).Where(x => x.Area == area && x.Empresa == empresa ).ToArray();
                               return Json(personal);
                }

               
            }

        
        }


        [HttpPost]
        public JsonResult DatosSinContestar( string fechainicial, string fechafinal, string empresa)
        {

            DateTime FechaInicial = Convert.ToDateTime(fechainicial);
            DateTime FechaFinal = Convert.ToDateTime(fechafinal);
            List<indicator> Items = new List<indicator>();

            using (AutogestionContext db = new AutogestionContext())
            {
              if (empresa == "")
                {
                    var personal = db.PersonalActivo
.Where(c => !db.EncabezadoEncuesta
.Where(b => DbFunctions.TruncateTime(b.Fecha) <= FechaFinal)
.Where(b => DbFunctions.TruncateTime(b.Fecha) >= FechaInicial)
.Select(b => b.Empleado.Documento)
.Contains(c.Documento)
).ToArray();
                    return Json(personal);
                }
                else
                {
                    var personal = db.PersonalActivo
.Where(c => !db.EncabezadoEncuesta
.Where(b => DbFunctions.TruncateTime(b.Fecha) <= FechaFinal)
.Where(b => DbFunctions.TruncateTime(b.Fecha) >= FechaInicial)
.Select(b => b.Empleado.Documento)
.Contains(c.Documento)
).Where(x => x.Empresa == empresa).ToArray();
                    return Json(personal);
                }


            }


        }

        [HttpPost]
        public JsonResult DatosSinEncuesta(string fechainicial, string fechafinal, string empresa)
        {

            DateTime FechaInicial = Convert.ToDateTime(fechainicial);
            DateTime FechaFinal = Convert.ToDateTime(fechafinal);
            List<indicator> Items = new List<indicator>();

            using (AutogestionContext db = new AutogestionContext())
            {
                if (empresa == "")
                {
                    var personal = db.PersonalActivo
.Where(c => !db.EncabezadoEncuesta
.Select(b => b.Empleado.Documento)
.Contains(c.Documento)
).ToArray();
                    return Json(personal);
                }
                else
                {
                    var personal = db.PersonalActivo
.Where(c => !db.EncabezadoEncuesta
.Select(b => b.Empleado.Documento)
.Contains(c.Documento)
).Where(x => x.Empresa == empresa).ToArray();
                    return Json(personal);
                }


            }


        }

        [HttpPost]
        public JsonResult DatosConAutoevaluacion(string area, string fechainicial, string fechafinal,string empresa)
        {

            DateTime FechaInicial = Convert.ToDateTime(fechainicial);
            DateTime FechaFinal = Convert.ToDateTime(fechafinal);
            List<indicator> Items = new List<indicator>();

            using (AutogestionContext db = new AutogestionContext())
            {

                if (empresa == "")
                {
                    var personal = db.EncabezadoEncuesta.Join(db.PersonalActivo,
s => s.Empleado.NroEmpleado,
sa => sa.CodigoEmpleado,
(s, sa) => new { encuesta = s, personal = sa })
.Where(b => DbFunctions.TruncateTime(b.encuesta.Fecha) <= FechaFinal)
.Where(b => DbFunctions.TruncateTime(b.encuesta.Fecha) >= FechaInicial)
.Where(b => b.personal.Area == area)
.Select(x => new { x.personal.Area, x.personal.Cargo, x.personal.Nombres, x.encuesta.Fecha }).ToArray();


                    return Json(personal);
                }
                else {
                    var personal = db.EncabezadoEncuesta.Join(db.PersonalActivo,
s => s.Empleado.NroEmpleado,
sa => sa.CodigoEmpleado,
(s, sa) => new { encuesta = s, personal = sa })
.Where(b => DbFunctions.TruncateTime(b.encuesta.Fecha) <= FechaFinal)
.Where(b => DbFunctions.TruncateTime(b.encuesta.Fecha) >= FechaInicial)
.Where(b => b.personal.Area == area && b.personal.Empresa == empresa)
.Select(x => new { x.personal.Area, x.personal.Cargo, x.personal.Nombres, x.encuesta.Fecha }).ToArray();


                    return Json(personal);
                
                }
              
   
            }


        }


        [HttpPost]
        public JsonResult TotalPorDias(string area,string empresa)
        {

            
            List<indicator> Items = new List<indicator>();

            using (AutogestionContext db = new AutogestionContext())
            {

                if (empresa == "")
                {
                    var personal = db.EncabezadoEncuesta.Join(db.PersonalActivo,
s => s.Empleado.NroEmpleado,
sa => sa.CodigoEmpleado,
(s, sa) => new { encuesta = s, personal = sa })
        .Where(b => b.personal.Area == area)
       .GroupBy(p => DbFunctions.TruncateTime(p.encuesta.Fecha))
       .Select(g => new { Fecha = g.Key, count = g.Count() }).ToArray();
                    return Json(personal);
                }
                else {
                    var personal = db.EncabezadoEncuesta.Join(db.PersonalActivo,
s => s.Empleado.NroEmpleado,
sa => sa.CodigoEmpleado,
(s, sa) => new { encuesta = s, personal = sa })
            .Where(b => b.personal.Area == area && b.personal.Empresa == empresa)
           .GroupBy(p => DbFunctions.TruncateTime(p.encuesta.Fecha))
           .Select(g => new { Fecha = g.Key, count = g.Count() }).ToArray();
                    return Json(personal);
                
                }  
    



              
            }


        }

        class QueryResult
        {
            public DateTime Fecha { get; set; }
            public int Verde { get; set; }
            public int Amarillo { get; set; }
            public int Rojo { get; set; }
        }
        [HttpPost]
        public JsonResult TotalColores(string area,string empresa)
        {
            List<indicator2> Items = new List<indicator2>();

            using (AutogestionContext db = new AutogestionContext())

            {
                if (empresa == "")
                {
                    var Sospechoso = db.EncabezadoEncuesta.Where(e => e.Sospechoso == "Rojo").GroupBy(e => DbFunctions.TruncateTime(e.Fecha)).Select(e => new { Name = e.Key, Count = e.Count() }).ToArray();

                    string sequenceMaxQuery = "select Fecha,Verde,Amarillo,Rojo from v_todosxarea where area = '" + area + "'";


                    var resultados = db.Database.SqlQuery<QueryResult>(sequenceMaxQuery).ToArray();


                    foreach (var item in resultados) { Items.Add(new indicator2() { Fecha = item.Fecha, Verde = item.Verde, Amarillo = item.Amarillo, Rojo = item.Rojo }); }
                    return Json(Items.ToArray());
                }
                else {
                    var Sospechoso = db.EncabezadoEncuesta.Where(e => e.Sospechoso == "Rojo").GroupBy(e => DbFunctions.TruncateTime(e.Fecha)).Select(e => new { Name = e.Key, Count = e.Count() }).ToArray();

                    string sequenceMaxQuery = "select Fecha,Verde,Amarillo,Rojo from v_todosxareaempresa where area = '" + area + "' and empresa = '" + empresa+ "'";


                    var resultados = db.Database.SqlQuery<QueryResult>(sequenceMaxQuery).ToArray();


                    foreach (var item in resultados) { Items.Add(new indicator2() { Fecha = item.Fecha, Verde = item.Verde, Amarillo = item.Amarillo, Rojo = item.Rojo }); }
                    return Json(Items.ToArray());
                
                }
           
            
            }

           
        }

        [HttpPost]
        public JsonResult TotalColoresGlobal( string empresa)
        {
            List<indicator2> Items = new List<indicator2>();

            using (AutogestionContext db = new AutogestionContext())
            {
             
                    var Sospechoso = db.EncabezadoEncuesta.Where(e => e.Sospechoso == "Rojo").GroupBy(e => DbFunctions.TruncateTime(e.Fecha)).Select(e => new { Name = e.Key, Count = e.Count() }).ToArray();

                    string sequenceMaxQuery = "select Fecha,Verde,Amarillo,Rojo from v_todosxareaempresa'";


                    var resultados = db.Database.SqlQuery<QueryResult>(sequenceMaxQuery).ToArray();


                    foreach (var item in resultados) { Items.Add(new indicator2() { Fecha = item.Fecha, Verde = item.Verde, Amarillo = item.Amarillo, Rojo = item.Rojo }); }
                    return Json(Items.ToArray());

               


            }


        }

        //
        // GET: /PersonalActivo/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /PersonalActivo/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /PersonalActivo/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /PersonalActivo/Create

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

        //
        // GET: /PersonalActivo/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /PersonalActivo/Edit/5

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

        //
        // GET: /PersonalActivo/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /PersonalActivo/Delete/5

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
        //Graficass
        private class indicator
        {
            public int count { get; set; }
            public string type { get; set; }
            public string var { get; set; }
        }
        private class indicator2
        {
            public DateTime Fecha { get; set; }
            public int Verde { get; set; }
            public int Amarillo { get; set; }
            public int Rojo { get; set; }
        }

        
    }
}
