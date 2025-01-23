using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System.Data.Entity;

namespace Adm_AutoGestion.Controllers
{
    public class PazySalvoController : Controller
    {
        //
        // GET: /PazySalvo/
         private PazySalvoRepository _repo;

         public PazySalvoController()
        {
            _repo = new PazySalvoRepository();

        }

        public ActionResult Index()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("PazySalvo"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }


            AreasPazySalvo area = new AreasPazySalvo();
            List<PazySalvo> model = new List<PazySalvo>();
            string usuario = String.Format("{0}", Session["Empleado"]);
            using (var db = new AutogestionContext())
            {
                area = db.AreasPazySalvo.FirstOrDefault(e => e.Responsable == usuario);
                if (area != null)
                {
                    Session["Area"] = area.Area;
                    model = _repo.ObtenerTodos(area.Area);
                }
            }
                
            
            return View(model);
        }

        //
        // GET: /PazySalvo/Details/5
        public string Firmar(string usuario, string password, int Id, string Obs)
        { 
            string message = "";
            string fuente = "App";
            string area = String.Format("{0}", Session["Area"]);
            message = _repo.FirmarPazySalvo(usuario, password, Id, area, Obs, fuente);


            return message;
        }

        public ActionResult Informe(string Documento, string Codigo, string Estado, string FechaIni, string FechaFin)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("PazySalvoInforme"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }


            List<PazySalvo> Items = new List<PazySalvo>();

            using (var db = new AutogestionContext())
            {

                DateTime Fecha1 = DateTime.Now;
                DateTime Fecha2 = DateTime.Now;

                

                    if (!DateTime.TryParse(FechaIni, out Fecha1))
                    {
                        Fecha1 = new DateTime();
                    }

                    if (!DateTime.TryParse(FechaFin, out Fecha2))
                    {
                        //Fecha2 = DateTime.Now;
                        string fec = "31/12/9999";
                        Fecha2 = DateTime.Parse(fec);
                    }

                if(Estado == "0"){
                    Items = db.PazySalvo.Where(e => e.Empleado.Documento.Contains(Documento)
                                                && e.Empleado.NroEmpleado.Contains(Codigo)
                                                && DbFunctions.TruncateTime(e.Fecha) >= Fecha1
                                                && DbFunctions.TruncateTime(e.Fecha) <= Fecha2).ToList();


                }
                if (Estado != "0")
                {

                    Items = db.PazySalvo.Where(e => e.Empleado.Documento.Contains(Documento)
                                                && e.Empleado.NroEmpleado.Contains(Codigo)
                                                && DbFunctions.TruncateTime(e.Fecha) >= Fecha1
                                                && DbFunctions.TruncateTime(e.Fecha) <= Fecha2
                                                && e.Estado == Estado
                                                    ).ToList();
                
                }

                foreach (PazySalvo Item in Items)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                }
            }

            return View(Items);
        }

        public ActionResult DetallePazySalvo(int id)
        {
            List<DetallePazySalvo> lista = new List<DetallePazySalvo>();
            using (var db = new AutogestionContext())
            {

                lista = db.DetallePazySalvo.Where(e => e.IdPazySalvo == id).ToList();
                foreach (DetallePazySalvo item in lista)
                {
                    item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == item.Responsable);
                
                }
            }


            return PartialView(lista);
        }


        [HttpGet]
        public ActionResult firmacorreo(string str)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(str);

            string[] valores = System.Text.Encoding.UTF8.GetString(base64EncodedBytes).Split('|');
            var id = Convert.ToInt32(valores[0]);
            var fuente = valores[1];
            ViewBag.Id = id;
            ViewBag.fuente = fuente;

            return View();
        }


        
        public ActionResult firmarPazySalvo(string signatureId, string signatureSrNm, string signatureCcss, string signatureObs, string fuente)
        {

            List<DetallePazySalvo> detalle = new List<DetallePazySalvo>();
            int idpys = 0;
            Int32.TryParse(signatureId, out idpys);
            

            try
            {

            using (var db = new AutogestionContext())
            {

                var empleado = db.Empleados.FirstOrDefault(x => x.Documento == signatureSrNm);
                if (empleado.Contraseña == signatureCcss)
                {
                    var area = db.AreasPazySalvo.FirstOrDefault(s => s.Responsable == empleado.Id.ToString());
                    if (area != null)
                    {
                        detalle = db.DetallePazySalvo.Where(e => e.IdPazySalvo == idpys && e.Firma == null).ToList();
                        if (detalle.Count > 0)
                        {
                            ViewBag.Message = _repo.FirmarPazySalvo(signatureSrNm, signatureCcss, idpys, area.Area, signatureObs, fuente);
                        }
                        else { ViewBag.Message = "El paz y salvo ya fue firmado."; }
                    }
                }
                else
                {
                    ViewBag.Message = "Contraseña Incorrecta.";
                }
            }



            }
            catch (Exception ex)
            {
                ViewBag.Message = "Ocurrio un Error:" + ex.Message;
            }
            ViewBag.firma = "Se termino proceso de Firma";
            return View();
        }


        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /PazySalvo/Create

        public ActionResult Create()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ResponsablePazySalvo"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            using (var db = new AutogestionContext())
            {

                List<SelectListItem> lst = new List<SelectListItem>();
                var lista = db.PersonalActivo.Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                foreach (var x in lista)
                {
                    lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }
                ViewBag.Areas = lst;
                ViewBag.Empleado = db.Empleados.Select(e => new { e.Id, e.Nombres, e.Activo, nomcodigo = e.Id + "-" + e.Nombres, e.FechaFin }).Where(f => f.Activo != "NO" || f.FechaFin >= DateTime.Now).ToList();



            }
            var model = _repo.ObtenerAreas();

            return View(model);
        }

        //
        // POST: /PazySalvo/Create

        //[HttpPost]
        public ActionResult Create1(AreasPazySalvo model)
        {
            try
            {
                // TODO: Add insert logic here
                var message = _repo.CrearAreasResp(model);
                Session["message"] = message;
                return RedirectToAction("Create");
            }
            catch
            {
                return View(model);
            }
        }


        //
        // GET: /PazySalvo/Edit/5

        public ActionResult Edit(int id)
        {

            return View();
        }

        //
        // POST: /PazySalvo/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, AreasPazySalvo model)
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

        //
        // GET: /PazySalvo/Delete/5

        public ActionResult Delete(int id)
        {
            using (var db = new AutogestionContext())
            {
                AreasPazySalvo tipo = db.AreasPazySalvo.Find(id);
                if (tipo == null)
                {
                    return HttpNotFound();
                }
                return View(tipo);
            }
        }

        //
        // POST: /PazySalvo/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, AreasPazySalvo model)
        {

            try
            {
                // TODO: Add update logic here
                _repo.Eliminar(id);
                return RedirectToAction("Create");
            }
            catch
            {
                return View();
            }

            
        }


        [HttpPost]
        public String AnularPazySalvo(int id)
        {
            string opcion = "anular";
            using (var db = new AutogestionContext())
            {
                try
                {

                    PazySalvo pys = new PazySalvo();
                    pys = db.PazySalvo.FirstOrDefault(s => s.Id == id);

                    if (pys.Estado == "Cerrado" || pys.Estado == "Anulado")
                    {
                        return "No es posible Anular el Paz y Salvo debido a que se encuentra en estado " + pys.Estado + ".";
                    }
                    else
                    {

                        _repo.modificar(id, opcion);
                        return "El Paz y Salvo fue anulado.";
                    }
                }
                catch (Exception ex)
                {

                    return ex.Message.ToString();
                }

            }
        }





    }
}
