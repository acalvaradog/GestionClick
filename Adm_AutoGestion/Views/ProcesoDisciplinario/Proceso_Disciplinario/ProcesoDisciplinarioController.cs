using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace Adm_AutoGestion.Controllers
{
    public class ProcesoDisciplinarioController : Controller
    {
        private AutogestionContext db = new AutogestionContext();
        private ProcesoDisciplinarioRepository _repo;
       

        public ProcesoDisciplinarioController() { 
        _repo=new ProcesoDisciplinarioRepository();
        }
        //
        // GET: /ProcesoDisciplinario/
        public ActionResult Index()
        {
            var model = _repo.ObtenerTodos();
            return View(model);
           
        }

        //
        // GET: /ProcesoDisciplinario/Details/5
        public ActionResult Details2(int Id)
        {
            using (var db = new AutogestionContext())
            {
                Empleado empleado = db.Empleados.Find(Id);
                if (empleado == null)
                {
                    return HttpNotFound();
                }
                return View(empleado);
            }
        }
        public ActionResult Anexo1(string Id, string Id2)
        {
          
            using (var db = new AutogestionContext())
            {
                List<PDAnexos> Items = new List<PDAnexos>();
                int id = 0;
                int emp = 0;
                Int32.TryParse(Id, out id);
                Int32.TryParse(Id2, out emp);

                ViewBag.Empleado = db.Empleados.FirstOrDefault(e => e.Id == emp);

                Items = db.PDAnexos.Where(x => x.IdProcesoDisciplinario == id).ToList();
                if (Items == null)
                {
                    return HttpNotFound();
                }
                return View(Items);
            }
     
        }

        public ActionResult Prueba1(string Id, string Id2)
        {

            using (var db = new AutogestionContext())
            {
                List<PDPruebas> Items = new List<PDPruebas>();
                int id = 0;
                int emp = 0;
                Int32.TryParse(Id, out id);
                Int32.TryParse(Id2, out emp);

                ViewBag.Empleado = db.Empleados.FirstOrDefault(e => e.Id == emp);
                Items = db.PDPruebas.Where(x => x.IdProcesoDisciplinario == id).ToList();
                if (Items == null)
                {
                    return HttpNotFound();
                }
                return View(Items);
            }

        }

        public ActionResult Implicado1(string Id, string Id2)
        {
            var emp = 0;

            var model = _repo.ObtenerTodos3(Id);
            using (var db = new AutogestionContext()) 
            {
                Int32.TryParse(Id2, out emp);

                ViewBag.Empleado = db.Empleados.FirstOrDefault(e => e.Id == emp);
            }

            return View(model);

        }

        public ActionResult Fundamentos(string Id)
        {
            var p = 0;
            using (var db = new AutogestionContext())
            {
                Int32.TryParse(Id, out p);

                ViewBag.Proceso = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == p);
            }
            return View();
        }
        public ActionResult Pretencion(string Id)
        {
            var p = 0;
            var model = _repo.ObtenerDatos(Id);
            using (var db = new AutogestionContext())
            {
                Int32.TryParse(Id, out p);

                ViewBag.Proceso = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == p);
            }
            return View(model);
        }

        //
        // GET: /ProcesoDisciplinario/Create
        public ActionResult Create()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ProcesoDisciplinarioCrear"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

         

            var model = new ProcesoDisciplinario();
            using (var db = new AutogestionContext())
            {
          
           
                ViewBag.Empleado = db.Empleados.Select(e => new { e.Id, e.Documento, e.Cargo, e.Jefe, e.Nombres, e.Activo, nomcodigo = e.Id + "-" + e.Cargo + "-" + e.Area, e.FechaFin }).Where(f => f.Activo != "NO" || f.FechaFin >= DateTime.Now).ToList();
              

            }

            return View(model);
        }


        //public ActionResult Anexos(string Id, string Id2)
        //{
        //    var model = _repo.ObtenerAnexos(Id);
        //    var emp = 0;
        //    Int32.TryParse(Id2, out emp);
        //    using (var db = new AutogestionContext())
        //    {
        //         ViewBag.Empleado = db.Empleados.FirstOrDefault(e => e.Id == emp);

        //    }
        //     return View(model);
        //}


        //public ActionResult Pruebas(string Id, string Id2)
        //{
        //    var model = _repo.ObtenerPruebas(Id);
        //    var emp = 0;
        //    Int32.TryParse(Id2, out emp);
        //    using (var db = new AutogestionContext())
        //    {
        //        ViewBag.Empleado = db.Empleados.FirstOrDefault(e => e.Id == emp);
        //    }
        //    return View(model);
        //}

        //public ActionResult EmpleadoImplicado(string Id)
        //{

        //    var model = _repo.ObtenerTodos3(Id);

        //    return View(model);
        //}


        public FileResult Download1(string archivo)
        {

            var fileBytes = Server.MapPath(@"..\AnexosProcesosDisciplinarios\Anexos\" + archivo);
            string fileName = archivo;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }


        public FileResult Download2(string archivo)
        {

             var fileBytes = Server.MapPath(@"..\AnexosProcesosDisciplinarios\Pruebas\" + archivo);
            string fileName = archivo;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }


        public ActionResult DetalleProcesoDisciplinario(string EmpleadoCP, string EmpleadoI, string FechaProcesoI, string FechaProcesoF,string Estado)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ProcesoDisciplinarioInforme"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            List<ProcesoDisciplinario> Proceso = new List<ProcesoDisciplinario>();
             List<PDTrabajador> implicados = new List<PDTrabajador>();

            using (var db = new AutogestionContext())
            {               
                ViewBag.Empleado = db.Empleados.Select(e => new { e.Id, e.Nombres, e.Activo, nomcodigo = e.Id + "-" + e.Nombres }).Where(f => f.Activo != "NO").ToList();

                DateTime Fecha1 = DateTime.Now;
                DateTime Fecha2 = DateTime.Now;

                List<SelectListItem> lst = new List<SelectListItem>();

                if (!DateTime.TryParse(FechaProcesoI, out Fecha1))
                    {
                        Fecha1 = new DateTime();
                    }

                if (!DateTime.TryParse(FechaProcesoF, out Fecha2))
                    {
                        Fecha2 = DateTime.Now;
                    }
               
                    if (EmpleadoI == "")
                    {
                        if (Estado == "Todos")
                        {
                        Proceso = db.ProcesoDisciplinario.Where(e => DbFunctions.TruncateTime(e.FechaRegistro) >= Fecha1 &&
                                          DbFunctions.TruncateTime(e.FechaRegistro) <= Fecha2
                                          &&
                                          SqlFunctions.StringConvert((decimal)e.EmpleadoRegistraId).Contains(EmpleadoCP)
                                          ).ToList();
                        }
                        else
                        {
                             Proceso = db.ProcesoDisciplinario.Where(e => DbFunctions.TruncateTime(e.FechaRegistro) >= Fecha1 &&
                                  e.Estado == Estado
                                          &&
                                          DbFunctions.TruncateTime(e.FechaRegistro) <= Fecha2
                                          &&
                                          SqlFunctions.StringConvert((decimal)e.EmpleadoRegistraId).Contains(EmpleadoCP)
                                          ).ToList();
                            
                        }
                        foreach (ProcesoDisciplinario Item in Proceso)
                        {
                            Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
                        }
                    }
                    if (EmpleadoI != "")
                    {
                       
                        if (Estado == "Todos")
                        {
                            int id = -1;
                            Int32.TryParse(EmpleadoI, out id);
                            implicados = (from p in db.PDTrabajador
                                          where (p.EmpleadoId == id)
                                          select p).ToList();
                            //Proceso=  db.ProcesoDisciplinario.Where(e => e.PDTrabajador2.EmpleadoId==id).ToList();
                            int[] Ids = (from item in db.ProcesoDisciplinario join item1 in db.PDTrabajador on item.Id equals item1.ProcesoDisciplinarioId where item1.EmpleadoId.Equals(id) select item.Id).ToArray();
                            Proceso = db.ProcesoDisciplinario.Where(e =>
                                                                 Ids.Contains(e.Id) && e.FechaRegistro >= Fecha1 && e.FechaRegistro <= Fecha2
                                                                   &&
                                        SqlFunctions.StringConvert((decimal)e.EmpleadoRegistraId).Contains(EmpleadoCP)).ToList();
                        }
                        else
                        {
                            int id = -1;
                            Int32.TryParse(EmpleadoI, out id);
                            implicados = (from p in db.PDTrabajador
                                          where (p.EmpleadoId == id)
                                          select p).ToList();
                            //Proceso=  db.ProcesoDisciplinario.Where(e => e.PDTrabajador2.EmpleadoId==id).ToList();
                            int[] Ids = (from item in db.ProcesoDisciplinario join item1 in db.PDTrabajador on item.Id equals item1.ProcesoDisciplinarioId where item1.EmpleadoId.Equals(id) select item.Id).ToArray();
                            Proceso = db.ProcesoDisciplinario.Where(e =>
                                                                 Ids.Contains(e.Id) && e.FechaRegistro >= Fecha1 && e.FechaRegistro <= Fecha2
                                                                   && e.Estado == Estado &&    
                                        SqlFunctions.StringConvert((decimal)e.EmpleadoRegistraId).Contains(EmpleadoCP)).ToList();
                        }
                        foreach (ProcesoDisciplinario Item in Proceso)
                        {
                            Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
                        }
                    }

                }
            return View(Proceso);
            
        }


        public ActionResult DetalleProcesoDisciplinario1(string EmpleadoCP, string EmpleadoI, string FechaProcesoI, string FechaProcesoF, string Estado)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ProcesoDisciplinarioInformeJefe"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var Empleadolog = Session["Empleado"];
            if (EmpleadoCP == null || EmpleadoCP == "")
            {
                EmpleadoCP = Convert.ToString(Empleadolog);
            }
            List<ProcesoDisciplinario> Proceso = new List<ProcesoDisciplinario>();
            List<PDTrabajador> implicados = new List<PDTrabajador>();

            using (var db = new AutogestionContext())
            {
                int Usuario=0;
                var usuariolog =String.Format("{0}", Session["Empleado"]);
                Int32.TryParse(usuariolog, out Usuario);
                var Jefe = db.Empleados.FirstOrDefault(e => e.Id == Usuario);

                ViewBag.Empleado = db.Empleados.Where(f => f.Activo != "NO" && f.Superior==Jefe.NroEmpleado).ToList();

                DateTime Fecha1 = DateTime.Now;
                DateTime Fecha2 = DateTime.Now;

                List<SelectListItem> lst = new List<SelectListItem>();

                if (!DateTime.TryParse(FechaProcesoI, out Fecha1))
                {
                    Fecha1 = new DateTime();
                }

                if (!DateTime.TryParse(FechaProcesoF, out Fecha2))
                {
                    Fecha2 = DateTime.Now;
                }

                if (EmpleadoI == "")
                {
                    if (Estado == "Todos")
                    {
                        Proceso = db.ProcesoDisciplinario.Where(e => DbFunctions.TruncateTime(e.FechaRegistro) >= Fecha1 &&
                                          DbFunctions.TruncateTime(e.FechaRegistro) <= Fecha2
                                          &&
                                          SqlFunctions.StringConvert((decimal)e.EmpleadoRegistraId).Contains(EmpleadoCP)
                                          ).ToList();
                    }
                    else
                    {
                        Proceso = db.ProcesoDisciplinario.Where(e => DbFunctions.TruncateTime(e.FechaRegistro) >= Fecha1 &&
                             e.Estado == Estado
                                     &&
                                     DbFunctions.TruncateTime(e.FechaRegistro) <= Fecha2
                                     &&
                                     SqlFunctions.StringConvert((decimal)e.EmpleadoRegistraId).Contains(EmpleadoCP)
                                     ).ToList();

                    }
                    foreach (ProcesoDisciplinario Item in Proceso)
                    {
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
                    }
                }
                if (EmpleadoI != "")
                {

                    if (Estado == "Todos")
                    {
                        int id = -1;
                        Int32.TryParse(EmpleadoI, out id);
                        implicados = (from p in db.PDTrabajador
                                      where (p.EmpleadoId == id)
                                      select p).ToList();
                        //Proceso=  db.ProcesoDisciplinario.Where(e => e.PDTrabajador2.EmpleadoId==id).ToList();
                        int[] Ids = (from item in db.ProcesoDisciplinario join item1 in db.PDTrabajador on item.Id equals item1.ProcesoDisciplinarioId where item1.EmpleadoId.Equals(id) select item.Id).ToArray();
                        Proceso = db.ProcesoDisciplinario.Where(e =>
                                                             Ids.Contains(e.Id) && e.FechaRegistro >= Fecha1 && e.FechaRegistro <= Fecha2
                                                               &&
                                    SqlFunctions.StringConvert((decimal)e.EmpleadoRegistraId).Contains(EmpleadoCP)).ToList();
                    }
                    else
                    {
                        int id = -1;
                        Int32.TryParse(EmpleadoI, out id);
                        implicados = (from p in db.PDTrabajador
                                      where (p.EmpleadoId == id)
                                      select p).ToList();
                        //Proceso=  db.ProcesoDisciplinario.Where(e => e.PDTrabajador2.EmpleadoId==id).ToList();
                        int[] Ids = (from item in db.ProcesoDisciplinario join item1 in db.PDTrabajador on item.Id equals item1.ProcesoDisciplinarioId where item1.EmpleadoId.Equals(id) select item.Id).ToArray();
                        Proceso = db.ProcesoDisciplinario.Where(e =>
                                                             Ids.Contains(e.Id) && e.FechaRegistro >= Fecha1 && e.FechaRegistro <= Fecha2
                                                               && e.Estado == Estado &&
                                    SqlFunctions.StringConvert((decimal)e.EmpleadoRegistraId).Contains(EmpleadoCP)).ToList();
                    }
                    foreach (ProcesoDisciplinario Item in Proceso)
                    {
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
                    }
                }

            }
            return View(Proceso);

        }

        //
        // POST: /ProcesoDisciplinario/Create
        [HttpPost]
        public ActionResult Create(ProcesoDisciplinario model)
        {
            try
            {
                // TODO: Add insert logic here

                if (ModelState.IsValid)
                {

                    _repo.Crear(model);
                    return RedirectToAction("Index");
                }
            }
            catch 
            {
                
            }
            return View();
        }

        //
        // GET: /ProcesoDisciplinario/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /ProcesoDisciplinario/Edit/5
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
        // GET: /ProcesoDisciplinario/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /ProcesoDisciplinario/Delete/5
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

        [HttpPost]
        public JsonResult LoadUpdate() {
            string respuesta = "true";
            //var Datos2=new ProcesoDisciplinario();
            var FechaRegistro = HttpContext.Request.Params["ProcesoDisciplinario.FechaRegistro"];
            var EmpleadoId = HttpContext.Request.Params["Empleados"];
            var Fundamentos = HttpContext.Request.Params["ProcesoDisciplinario.Fundamentos"];
            var Pretencion = HttpContext.Request.Params["ProcesoDisciplinario.Pretencion"];
            var CntAnexos = HttpContext.Request.Params["Cantidadanexos"];
            var CntPruebas = HttpContext.Request.Params["Cantidadpruebas"];
            var CntEmpleados = HttpContext.Request.Params["Cantidadempleados"];
            var Empleadolog = Session["Empleado"];
        
            ProcesoDisciplinario DatosProcesoDisciplinario = new ProcesoDisciplinario();

            DatosProcesoDisciplinario.EmpleadoRegistraId = Convert.ToInt16(Empleadolog);
            DatosProcesoDisciplinario.FechaRegistro = DateTime.Parse(FechaRegistro);
            DatosProcesoDisciplinario.Pretencion = Pretencion;
            DatosProcesoDisciplinario.Fundamentos = Fundamentos;

            List<PDAnexos> AdjuntosAnexos = new List<PDAnexos>();
            List<PDPruebas> AdjuntosPruebas = new List<PDPruebas>();
            List<PDTrabajador> CEmpleados = new List<PDTrabajador>();
             int cantidada = Convert.ToInt16(CntAnexos);
             int cantidadp = Convert.ToInt16(CntPruebas);
             int cantidade = Convert.ToInt16(CntEmpleados);

             for (int i = 0; i < cantidade; i++)
             {
                 PDTrabajador empleado = new PDTrabajador();
                 int[] Emps = EmpleadoId.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

                 //string[] Emps = EmpleadoId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                 var idempleado = Emps[i];
                empleado.ProcesoDisciplinarioId = DatosProcesoDisciplinario.Id;
                empleado.EmpleadoId = idempleado;
                CEmpleados.Add(empleado);
                db.PDTrabajador.Add(empleado);
             }

             for (int i = 1; i <= cantidada; i++)
             {

                 PDAnexos ArchivoAnexo = new PDAnexos();
                 string nombreadjunto = "Anexo" + i;
                 var httpPostedFile = HttpContext.Request.Files[nombreadjunto];


                 var extension = httpPostedFile.FileName.Split('.');
            
                     if (extension[1] != "jpg" && extension[1] != "jpeg" && extension[1] != "png" && extension[1] != "doc" && extension[1] != "docx" && extension[1] != "pdf")
                     {
                         respuesta = "El tipo de archivo ." + extension[1] + " no es permitido.";
                         return Json(new { respuesta });
                     }
                 
                 var size = httpPostedFile.ContentLength / 1024;
                 if (size > 800)
                 {
                     respuesta = "El archivo supera el tamaño permitido de carga.";
                     return Json(new { respuesta });

                 }


                 // Validate the uploaded image(optional)
                 DateTime date1 = DateTime.Now;
                 var nombrearchivo = date1.ToString("yyyyMMddHHmmssffff" + i).ToString() + "-" + httpPostedFile.FileName;
                 // Get the complete file path
                 var fileSavePath = Path.Combine(HttpContext.Server.MapPath("~/AnexosProcesosDisciplinarios/Anexos"), nombrearchivo);


                 // Save the uploaded file to "UploadedFiles" folder
                 httpPostedFile.SaveAs(fileSavePath);
                 ArchivoAnexo.IdProcesoDisciplinario = DatosProcesoDisciplinario.Id;
                 ArchivoAnexo.Archivo = nombrearchivo;
                 AdjuntosAnexos.Add(ArchivoAnexo);
                 db.PDAnexos.Add(ArchivoAnexo);
             }
             for (int i = 1; i <= cantidadp; i++)
             {
                 
                 PDPruebas ArchivoPrueba = new PDPruebas();
                 string nombreadjunto = "Adjunto" + i;
                 var PruebaType = HttpContext.Request.Params["PDPruebas.TipoPrueba"+i];
                 var httpPostedFile = HttpContext.Request.Files[nombreadjunto];


                 var extension = httpPostedFile.FileName.Split('.');

                 if (extension[1] != "jpg" && extension[1] != "jpeg" && extension[1] != "png" && extension[1] != "doc" && extension[1] != "docx" && extension[1] != "pdf")
                 {
                     respuesta = "El tipo de archivo ." + extension[1] + " no es permitido.";
                     return Json(respuesta);
                 }

                 var size = httpPostedFile.ContentLength / 1024;
                 if (size > 800)
                 {
                     respuesta = "El archivo supera el tamaño permitido de carga.";
                     return Json(respuesta);

                 }


                 // Validate the uploaded image(optional)
                 DateTime date1 = DateTime.Now;
                 var nombrearchivo = date1.ToString("yyyyMMddHHmmssffff" + i).ToString() + "-" + httpPostedFile.FileName;
                 // Get the complete file path
                 var fileSavePath = Path.Combine(HttpContext.Server.MapPath("~/AnexosProcesosDisciplinarios/Pruebas"), nombrearchivo);


                 // Save the uploaded file to "UploadedFiles" folder
                 httpPostedFile.SaveAs(fileSavePath);
                 ArchivoPrueba.IdProcesoDisciplinario = DatosProcesoDisciplinario.Id;
                 ArchivoPrueba.TipoPrueba = PruebaType;
                 ArchivoPrueba.Adjunto = nombrearchivo;
                 AdjuntosPruebas.Add(ArchivoPrueba);
                 db.PDPruebas.Add(ArchivoPrueba);
             }
             var Estado = "Activo";
             DatosProcesoDisciplinario.Estado=Estado;
             DatosProcesoDisciplinario.PDPruebas = AdjuntosPruebas;
             DatosProcesoDisciplinario.PDAnexos = AdjuntosAnexos;
             DatosProcesoDisciplinario.PDTrabajador = CEmpleados;
             try
             {
                 db.ProcesoDisciplinario.Add(DatosProcesoDisciplinario);

                 db.SaveChanges();


                 List<string> funciones = Acceso.Validar(Session["Empleado"]);
                 var url = Url.Action("");
                 if (!Acceso.EsAnonimo && !funciones.Contains("ProcesoDisciplinarioInformeJefe"))
                 {
                     url = Url.Action("DetalleProcesoDisciplinario1");
                 }
                 if (!Acceso.EsAnonimo && !funciones.Contains("ProcesoDisciplinarioInforme"))
                 {

                     url = Url.Action("DetalleProcesoDisciplinario");
                 }

                 respuesta = String.Format("Se ha guardado de manera exitosa");
                 return Json(new
                 {
                     redirectUrl = url,
                     isRedirect = true,
                     respuesta
                 });
             }
             catch (SystemException ex) {
                 respuesta = String.Format("Error al guardar. {0}", ex.Message);
                 return Json(respuesta);
             }
         

        }

        public ActionResult Respuesta(string Id)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ProcesoDisciplinarioRespuestaJuridica"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }


            int id = -1;
            string opcion = "Respuesta";
            Int32.TryParse(Id, out id);
            string EmpleadoId = String.Format("{0}", Session["Empleado"]);
            var model = _repo.ObtenerTodos2(opcion, EmpleadoId);


            if (id > 0)
            {
                ViewBag.ProcesoDisciplinario = model.FirstOrDefault(e => e.Id == id);
            }

            if (ViewBag.ProcesoDisciplinario == null)
            {
                ViewBag.ProcesoDisciplinario = new Models.ProcesoDisciplinario();
                ViewBag.ProcesoDisciplinario.Empleado = new Empleado();
            }
            return View(model);
        }

        public ActionResult GestionHumana(string Id)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ProcesoDisciplinarioConfirmarGH"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }


            int id = -1;
            string opcion = "Confirmar";
            Int32.TryParse(Id, out id);
            string EmpleadoId = String.Format("{0}", Session["Empleado"]);
            var model = _repo.ObtenerTodos2(opcion, EmpleadoId);


            if (id > 0)
            {
                ViewBag.ProcesoDisciplinario = model.FirstOrDefault(e => e.Id == id);
            }

            if (ViewBag.ProcesoDisciplinario == null)
            {
                ViewBag.ProcesoDisciplinario = new Models.ProcesoDisciplinario();
                ViewBag.ProcesoDisciplinario.Empleado = new Empleado();
            }
            return View(model);
        }

        public ActionResult Gestion1 (string Id)
        {
            var process = 0;
            Int32.TryParse(Id, out process);
            using (var db = new AutogestionContext())
            {
                ProcesoDisciplinario Items = new ProcesoDisciplinario();
                ViewBag.ProcesoDisciplinario = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == process);


                Items = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == process);
                Items.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Items.EmpleadoRegistraId);
                return View(Items);
            }

        }
        public ActionResult Gestion2(string RJuridica, string Id, string NroEmpleado, string Empleado, string Estado)
        {

            string message = null;
            int IdUsuarioM = 0;
            InMemoryDestinationConfiguration DestinacionConfiguracion = new InMemoryDestinationConfiguration();


            try
            {
                int id = 0;
                Int32.TryParse(Id, out id);
                string modifica = String.Format("{0}", Session["Empleado"]);
                Int32.TryParse(modifica, out IdUsuarioM);
                using (var db = new AutogestionContext())
                {

                    if (Id != "0")
                    {

                        ProcesoDisciplinario vacaciones = db.ProcesoDisciplinario.Find(id);
                        _repo.Modificar(RJuridica, Id, Empleado, Estado, IdUsuarioM);

                    }
                    else
                    {
                        message = "Antes de Guardar, debe seleccionar un registro de la tabla.";
                    }

                    // TODO: Add update logic here

                    Session["message"] = message;
                    return RedirectToAction("GestionHumana");


                }

            }
            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
                Session["message"] = message;
                return View("Confirmacion");
            }
        }


        public ActionResult Respuesta1(string RJuridica, string Id, string NroEmpleado, string Empleado, string Estado)
        {

            string message = null;
            int IdUsuarioM = 0;
            try
            {
                int id = 0;
                Int32.TryParse(Id, out id);
                string modifica = String.Format("{0}", Session["Empleado"]);
                Int32.TryParse(modifica, out IdUsuarioM);



                using (var db = new AutogestionContext())
                {

                    if (Id != "0")
                    {

                        ProcesoDisciplinario vacaciones = db.ProcesoDisciplinario.Find(id);
                        _repo.Modificar(RJuridica,Id, Empleado, Estado, IdUsuarioM);

                    }
                    else
                    {
                        message = "Antes de Guardar, debe seleccionar un registro de la tabla.";
                    }

                    // TODO: Add update logic here

                    Session["message"] = message;
                    return RedirectToAction("Respuesta");


                }

            }
            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
                Session["message"] = message;
                return View("Confirmacion");
            }
        }

        public ActionResult Respuesta3(string Id) 
        {
            var process = 0;
            Int32.TryParse(Id, out process);
            using (var db = new AutogestionContext())
            {
                ProcesoDisciplinario Items = new ProcesoDisciplinario();
                ViewBag.ProcesoDisciplinario = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == process);


                Items = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == process);
                Items.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Items.EmpleadoRegistraId);
                return View(Items);
            }

        }

        public ActionResult Dashboard()
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ProcesoDisciplinarioDashboard"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }



            ViewBag.CntProcesosAc = db.ProcesoDisciplinario.Where(x => x.Estado == "Activo").Count();
            ViewBag.CntProcesosC = db.ProcesoDisciplinario.Where(x => x.Estado == "Cerrado").Count();
            ViewBag.CntProcesosAn = db.ProcesoDisciplinario.Where(x => x.Estado == "Anulado").Count();
            ViewBag.CntProcesosRJ = db.ProcesoDisciplinario.Where(x => x.Estado == "Remitido a Juridica").Count();

            
            return View();
        }

}
}
