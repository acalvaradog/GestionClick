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
    public class EntregaEPPController : Controller

    {

        private EntregaEPPRepository _repo;
        private ServiciosRepository _servicios;
        public EntregaEPPController()
        {
            _repo = new EntregaEPPRepository();
            _servicios = new ServiciosRepository();

        }

       
        //
        // GET: /EntregaEPP/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListaPorCompletar()
        {
            
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ListadoCompletarEPP"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            string usuario = String.Format("{0}", Session["Empleado"]);

            var Entrega = _repo.ObtenerTodos(usuario);

          
            return View(Entrega);
        }

        [HttpPost]
        public String CerrarActa(int id)
        {
            int cont = 0;
            string opcion = "cerrar";

            using (var db = new AutogestionContext())
            {

                try
                {

                    EntregaEPP entrega = db.EntregaEPP.Find(id);

                    if(entrega.Estado != "Cerrado"){

                        

                    List<DetalleEntregaEPP> detalle = db.DetalleEntregaEPP.Where(e => e.NumeroEntrega == id).ToList();

                    foreach (DetalleEntregaEPP Item in detalle)
                    {
                        if (Item.FechaFirma == null)
                        {
                            if (Item.Estado != "Anulado")
                            {
                                cont = cont + 1;
                            }
                        }

                    }

                    if (cont == 0)
                    {

                        _repo.modificar(id, opcion);
                        return "El acta de Entrega de EPP fue Cerrada.";

                    }
                    else
                    {
                       return "No es posible cerrar el acta, debido a que aun cuenta con elementos sin firmar.";
                    }

                    }
                    return "El acta ya se encuentra en estado Cerrado.";
                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }
            }

        }

        //
        // GET: /EntregaEPP/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /EntregaEPP/Create

        public ActionResult Create()
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("CrearEPP"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }


            using (var db = new AutogestionContext())
            {

                List<SelectListItem> lst = new List<SelectListItem>();
                var lista = db.Empleados.Where(x=> x.Activo == "SI" && x.Area!="" && x.Area != null).Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                foreach (var x in lista)
                {
                    lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }
               
                ViewBag.Areas = lst;
                ViewBag.Sociedad = db.Sociedad.ToList();
            }


            return View();
        }

        //
        // POST: /EntregaEPP/Create

        [HttpPost]
        public ActionResult Create(EntregaEPP model)
        {
            try
            {
                if (model.Area == null || model.Sociedad == null) {
                    Session["message"] = "Faltan campos por diligenciar";
                    return RedirectToAction("Create");
                }

                    // TODO: Add insert logic here
                    if (ModelState.IsValid)
                    {
                        string usuario = String.Format("{0}", Session["Empleado"]);
                        _repo.Crear(model, usuario);

                    }
                    return RedirectToAction("ListaPorCompletar");
                
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /EntregaEPP/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /EntregaEPP/Edit/5

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
        // GET: /EntregaEPP/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /EntregaEPP/Delete/5

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
        public String ObtenerQR(string id, string fecha, string empleado)
        {
            try {

                string textoqr = id + "|" + fecha + "|" + empleado;
                byte[] ImagenQR = _servicios.GenerarQR(textoqr);
                return Convert.ToBase64String(ImagenQR);

        }catch(Exception e){

        return "error" + e.Message.ToString();
        }
        
        }



        public ActionResult InformeCerradas(string FechaIni, string FechaFin) 
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("InformeCerradasEPP"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }


            List<EntregaEPP> Items = new List<EntregaEPP>();

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
                    //Fecha1 = DateTime.Now;
                    Fecha2 = DateTime.Now;
                }

                string usuario = String.Format("{0}", Session["Empleado"]);
                int empleadoId = 0;
                Int32.TryParse(usuario, out empleadoId);

               
                    Items = db.EntregaEPP.Where(e => DbFunctions.TruncateTime(e.Fecha) >= Fecha1 &&
                                                   DbFunctions.TruncateTime(e.Fecha) <= Fecha2 &&
                                                   e.Estado == "Cerrado" &&
                                                   e.EmpleadoId == empleadoId).ToList();

                
                
                
                foreach (EntregaEPP Item in Items)
                {
                    int soc = 0;
                    Int32.TryParse(Item.Sociedad, out soc);
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.Sociedades = db.Sociedad.FirstOrDefault(e => e.Id == soc);
                }


            }


            //var model = _repo.ObtenerCerradas();



            return View(Items);
        }


        [HttpPost]
        public String EnviarEmailFirma(int id)
        {

            List<DetalleEntregaEPP> Entrega = new List<DetalleEntregaEPP>();

            using (var db = new AutogestionContext())
            {

                try
                {

                    Entrega = db.DetalleEntregaEPP.Where(x => x.NumeroEntrega == id).ToList();

                    foreach (var item in Entrega)
                    {
                        Empleado empleado = new Empleado();
                        empleado = db.Empleados.FirstOrDefault(x => x.Id == item.EmpleadoId);

                        if (item.FechaFirma == null)
                        {
                            _servicios.EnviarEmailEPP(empleado, item);
                        }
                    }

                    return "OK";
                }
                catch (Exception ex)
                {

                    return ex.Message.ToString();
                }


            }

        }


        [HttpPost]
        public String AnularActa(int id)
        {
            string opcion = "anular";
            using (var db = new AutogestionContext())
            {
                try
                {

                    EntregaEPP Entrega = new EntregaEPP();
                    Entrega = db.EntregaEPP.FirstOrDefault(e => e.Id == id);

                    if (Entrega.Estado == "Cerrado" || Entrega.Estado == "Anulado")
                    {
                        return "No es posible Anular el acta debido a que se encuentra en estado " + Entrega.Estado + ".";
                    }
                    else {

                        _repo.modificar(id, opcion);
                        return "El acta fue anulada.";
                    }
                }
                catch (Exception ex)
                {

                    return ex.Message.ToString();
                }

            }
        }



        public ActionResult PendientePorFirmar(int id)
        {
            using (var db = new AutogestionContext())
            {
                
                List<DetalleEntregaEPP> detalle = new List<DetalleEntregaEPP>();
                detalle =  db.DetalleEntregaEPP.Where(x => x.NumeroEntrega == id && x.FechaFirma == null && x.Estado == "Activo").ToList();

                foreach (DetalleEntregaEPP Item in detalle)
                {

                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.ElementosProtecionPersonal = db.ElementosProtecionPersonal.FirstOrDefault(s => s.Id == Item.EPP);
                }

                if (detalle == null)
                {
                    return HttpNotFound();
                }
                return PartialView(detalle);
            }
        }

        
        [HttpGet]
        public ActionResult GenerarImpresionPDF(int id, string fecha, string empleado)
        {

            string Id = string.Format("{0}", id);
            var Data = new DataSet2();
            var ReportViewer = new Microsoft.Reporting.WebForms.ReportViewer();
            
            string textoqr = Id + "|" + fecha + "|" + empleado;
            byte[] ImagenQR = _servicios.GenerarQR(textoqr);

            using (var db = new AutogestionContext())
            {
                var Datos = (from d in db.DetalleEntregaEPP
                             join e in db.EntregaEPP on d.NumeroEntrega equals e.Id
                             join el in db.ElementosProtecionPersonal on d.EPP equals el.Id
                             join ep in db.Empleados on e.EmpleadoId equals ep.Id
                             join soc in db.Sociedad on e.Sociedad equals soc.Id.ToString()
                             join em in db.Empleados on d.EmpleadoId equals em.Id
                             where d.NumeroEntrega == id && d.Estado != "Anulado"
                             select new
                             {
                                 NumeroEnt = d.NumeroEntrega,
                                 EmpleadoCA = ep.Nombres,
                                 FechaCA = e.Fecha,
                                 AreaCA = e.Area,
                                 SociedadCA = soc.Descripcion,
                                 EstadoCA = e.Estado,
                                 DocumentoRE = em.Documento,
                                 EmpleadoRE = em.Nombres,
                                 CargoRE = em.Cargo,
                                 AreaRE = em.Area,
                                 CantidadRE = d.Cantidad,
                                 ElementoRE = el.Nombre,
                                 FechaRE = d.Fecha,
                                 FechafinRE = d.FechaFin
                             }).ToList();

                foreach(var Item in Datos)
                    Data.Tables[0].Rows.Add(new object[] { Item.NumeroEnt, Item.EmpleadoCA, Item.FechaCA, Item.AreaCA, Item.SociedadCA, Item.EstadoCA, Item.DocumentoRE, Item.EmpleadoRE, Item.CargoRE, Item.AreaRE, Item.CantidadRE, Item.ElementoRE, Item.FechaRE, Item.FechafinRE, ImagenQR });

                ReportViewer.LocalReport.DataSources.Clear();
                ReportViewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet2", Data.Tables[0]));
                ReportViewer.LocalReport.ReportEmbeddedResource = "Report3.rdlc";
                ReportViewer.LocalReport.ReportPath = "Report3.rdlc";
                ReportViewer.LocalReport.Refresh();


                var buffer = ReportViewer.LocalReport.Render("PDF");

                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = String.Empty;
                Response.AddHeader("content-disposition", "attachment; filename=ActaEPP.pdf");
                Response.BinaryWrite(buffer);
                Response.Flush();
                Response.End();
                return Json("ok");



            }
        }







    }
}
