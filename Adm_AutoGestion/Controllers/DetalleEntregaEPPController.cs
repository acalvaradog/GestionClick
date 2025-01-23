using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Data.Entity;

namespace Adm_AutoGestion.Controllers
{
    public class DetalleEntregaEPPController : Controller
    {

        private DetalleEntregaRepository _repo;
        private ServiciosRepository _servicios;
        private AutogestionContext db1 = new AutogestionContext();

        public DetalleEntregaEPPController()
        {
            _repo = new DetalleEntregaRepository();
            _servicios = new ServiciosRepository();

        }

        //
        // GET: /DetalleEntregaEPP/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult RegistrarEntrega(string id)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("RegistrarEntregaEPP"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }




            var model = new DetalleEntregaEPP();
            using (var db = new AutogestionContext())
            {
                //model.ListadoTiposInc = db.TiposIncapacidad.Where(e => e.EstadoId == 13).ToList();
                ViewBag.Empleado = db.Empleados.Select(e => new { e.Id, e.Nombres, e.Activo, nomcodigo = e.Id + "-" + e.Nombres,e.FechaFin }).Where(f => f.Activo != "NO" || f.FechaFin >= DateTime.Now ).ToList();
                var Elementos = db.ElementosProtecionPersonal.Select(e => new { e.Id, e.Nombre, e.Activo, nomcodigo = e.Id + "-" + e.Nombre }).ToList().Where(f => f.Activo == "1");
                ViewBag.Elementos = Elementos;
                ViewBag.Numero = id;

            }

            return View(model);
        }


        [HttpPost]
        public String RegistrarEntrega(List<DetalleEntregaEPP> model)
        {
            string mensj = "";
            try
            {
                // TODO: Add insert logic here

                _repo.Crear(model);
                mensj = "Los datos fueron guardados correctamente.";
                
                


            }
            catch (Exception e)
            {
                mensj = "Error."; ;
            }
            //return View();
            return mensj;
        }


        public ActionResult DetalleActa(int id)
        {
            
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("DetalleActa"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var entrega = _repo.ObtenerTodos(id);
            ViewBag.NumeroEnt = id;
            return View(entrega);
        }


        public ActionResult DetalleCadaRegistro(string EmpleadoCA, string EmpleadoRE, string AreaEntrega, string FechaActaD, string FechaActaH, string FechaEntregaD, string FechaEntregaH, string Anulados)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("DetalleCadaRegistro"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            DateTime Fecha1;
            DateTime Fecha2;
            DateTime Fecha3;
            DateTime Fecha4;
            List<DetalleEntregaEPP> Entrega = new List<DetalleEntregaEPP>();

            DateTime.TryParse(FechaActaD, out Fecha1);
            DateTime.TryParse(FechaActaH, out Fecha2);
            DateTime.TryParse(FechaEntregaD, out Fecha3);
            DateTime.TryParse(FechaEntregaH, out Fecha4);



             using (var db = new AutogestionContext())
                {

                    ViewBag.Empleado = db.Empleados.Select(e => new { e.Id, e.Nombres, e.Activo, nomcodigo = e.Id + "-" + e.Nombres }).Where(f => f.Activo != "NO").ToList();

                    List<SelectListItem> lst = new List<SelectListItem>();
                    var lista = db.Empleados.Where(x=> x.Activo == "SI" && x.AreaDescripcion != null).Select(x => new { x.AreaDescripcion }).GroupBy(b => b.AreaDescripcion).ToList();
                    foreach (var x in lista)
                    {
                        lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                    }

                    ViewBag.Areas = lst;

                    if (Anulados == null)
                    {
                        Anulados = "Anulado";
                    }
                   
                 
               
                 //char s = (char)9;
                    //if (String.IsNullOrWhiteSpace(EmpleadoRE)) EmpleadoRE = s.ToString();
                    int[] Ids = (from item in db.Empleados join item1 in db.DetalleEntregaEPP on item.Id equals item1.EmpleadoId where item.Nombres.Contains(EmpleadoRE) select item.Id).ToArray();

                    if (FechaActaD != "" && FechaActaH != "" || FechaEntregaD != "" && FechaEntregaH != "")
                    {

                        if (!DateTime.TryParse(FechaActaD, out Fecha1))
                        {
                            Fecha1 = new DateTime();
                        }

                        if (!DateTime.TryParse(FechaActaH, out Fecha2))
                        {
                            Fecha2 = DateTime.Now;
                        }


                        if (!DateTime.TryParse(FechaEntregaD, out Fecha3))
                        {
                            Fecha3 = new DateTime();
                        }

                        if (!DateTime.TryParse(FechaEntregaH, out Fecha4))
                        {
                            Fecha4 = DateTime.Now;
                        }



                        if (EmpleadoRE == "")
                        {

                            Entrega = db.DetalleEntregaEPP.Where(e =>
                                                                e.EntregaEPP.Area.Contains(AreaEntrega) && e.EntregaEPP.Empleado.Nombres.Contains(EmpleadoCA)
                                                                && e.EntregaEPP.Fecha >= Fecha1 && e.EntregaEPP.Fecha <= Fecha2
                                                                && e.Fecha >= Fecha3 && e.Fecha <= Fecha4 && e.EntregaEPP.Estado != Anulados && e.Estado != Anulados
                                                                ).ToList();

                        }
                        else
                        {
                            Entrega = db.DetalleEntregaEPP.Where(e =>
                                                                e.EntregaEPP.Area.Contains(AreaEntrega) && e.EntregaEPP.Empleado.Nombres.Contains(EmpleadoCA)
                                                                && Ids.Contains(e.EmpleadoId) && e.EntregaEPP.Fecha >= Fecha1 && e.EntregaEPP.Fecha <= Fecha2
                                                                && e.Fecha >= Fecha3 && e.Fecha <= Fecha4 && e.EntregaEPP.Estado != Anulados && e.Estado != Anulados
                                                                ).ToList();

                        }

                    }
                    else
                    {

                        if (EmpleadoRE == "")
                        {
                            Entrega = db.DetalleEntregaEPP.Where(e => e.EntregaEPP.Area.Contains(AreaEntrega)
                                                                && e.EntregaEPP.Empleado.Nombres.Contains(EmpleadoCA) && e.EntregaEPP.Estado != Anulados
                                                                && e.Estado != Anulados).ToList();
                        }
                        else
                        {
                            Entrega = db.DetalleEntregaEPP.Where(e => e.EntregaEPP.Area.Contains(AreaEntrega)
                                                                    && e.EntregaEPP.Empleado.Nombres.Contains(EmpleadoCA)
                                                                    && Ids.Contains(e.EmpleadoId) && e.EntregaEPP.Estado != Anulados && e.Estado != Anulados).ToList();
                        }




                    }
                    foreach (var Item in Entrega)
                    {
                        int soc = 0;
                        
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                        //Item.Empleado1 = db.PersonalActivo.FirstOrDefault(e => e.CodigoEmpleado == Item.Empleado.NroEmpleado);
                        Item.EntregaEPP = db.EntregaEPP.FirstOrDefault(e => e.Id == Item.NumeroEntrega);
                        Item.ElementosProtecionPersonal = db.ElementosProtecionPersonal.FirstOrDefault(e => e.Id == Item.EPP);
                        Item.EmpleadoCA = db.Empleados.FirstOrDefault(e => e.Id == Item.EntregaEPP.EmpleadoId);
                        Int32.TryParse(Item.EntregaEPP.Sociedad, out soc);
                        Item.Sociedades = db.Sociedad.FirstOrDefault(e => e.Id == soc);
                        Item.EmpleadoSoc = db.Sociedad.FirstOrDefault(e => e.Codigo == Item.Empleado.Empresa);
                    }

                }




                //var Entregas = db1.DetalleEntregaEPP.Include(a => a.EntregaEPP).Where(a => a.EntregaEPP.Area.Contains(AreaEntrega)).Where(a => a.Fecha >= Fecha3 && a.Fecha <= Fecha4);
            //var Entregas = db1.DetalleEntregaEPP.Join(db1.EntregaEPP,
            //                                            detalle => detalle.NumeroEntrega,
            //                                            entrega => entrega.Id,
            //                                            (detalle, entrega) => new { DetalleEntregaEPP = detalle, EntregaEPP = entrega }).Join(
            //                                            db1.Empleados,
            //                                            ent => ent.EntregaEPP.EmpleadoId,
            //                                            empleado => empleado.Id,
            //                                            (ent, empleado) => new { ent.DetalleEntregaEPP, ent.EntregaEPP, Empleado = empleado.Id }).Where(
            //                                            e => e.EntregaEPP.Empleado.Nombres.Contains(EmpleadoCA)).Select(
            //                                            s => new {
            //                                            s.DetalleEntregaEPP.NumeroEntrega,
            //                                            s.EntregaEPP.Fecha,
            //                                            s.EntregaEPP.Empleado.Nombres,
            //                                            s.EntregaEPP.Area,
            //                                            s.EntregaEPP.Estado,
            //                                            s.DetalleEntregaEPP.ElementosProtecionPersonal.Nombre,
            //                                            s.DetalleEntregaEPP.EmpleadoId,
            //                                            s.DetalleEntregaEPP.Cantidad,
            //                                            s.DetalleEntregaEPP.MotivoEntrega,
            //                                            FF = s.DetalleEntregaEPP.Fecha,
            //                                            s.DetalleEntregaEPP.FechaFin,
            //                                            s.DetalleEntregaEPP.Observacion,
            //                                            EstadoD = s.DetalleEntregaEPP.Estado
            //                                            });

               
               
                
            //return View(Entregas.ToList());
             return View(Entrega);
            
        }

        [HttpPost]
        public String AnularRegistro(int id)
        {
            string opcion = "anular";
            using (var db = new AutogestionContext())
            {
                try
                {

                    DetalleEntregaEPP Entrega = new DetalleEntregaEPP();
                    Entrega = db.DetalleEntregaEPP.FirstOrDefault(e => e.Id == id);

                    if (Entrega.Estado == "Anulado")
                    {
                        return "No es posible Anular el registro debido a que se encuentra en estado " + Entrega.Estado + ".";
                    }
                    else
                    {

                        _repo.modificar(id, opcion);
                        return "El registro fue anulado.";
                    }
                }
                catch (Exception ex)
                {

                    return ex.Message.ToString();
                }

            }
        }



        [HttpGet]
        public ActionResult firmaentrega(string str)
        {

            try
            {

                var base64EncodedBytes = System.Convert.FromBase64String(str);

                string[] valores = System.Text.Encoding.UTF8.GetString(base64EncodedBytes).Split('|');
                var id = Convert.ToInt32(valores[0]);
                var id_empleado = Convert.ToInt32(valores[1]);
                var fuente = valores[2];
                var elemento = "";

                DetalleEntregaEPP entrega = new DetalleEntregaEPP();
                Empleado empleado = new Empleado();
                if (fuente != "email")
                {


                    ViewBag.Message = "no valido para firmar";

                }
                using (var db = new AutogestionContext())
                {
                    entrega = db.DetalleEntregaEPP.FirstOrDefault(x => x.Id == id && (x.FechaFirma == null || x.FechaFirma == "" ));

                    if (entrega != null)
                    {

                        empleado = db.Empleados.FirstOrDefault(x => x.Id == id_empleado);

                        if (empleado != null)
                        {
                            var epp = db.ElementosProtecionPersonal.FirstOrDefault(x => x.Id == entrega.EPP);
                            elemento = epp.Nombre;
                            entrega.FechaFirma = DateTime.Now + "|" + "email";
                            db.SaveChanges();
                            ViewBag.Message = "Se ha Firmado la entrega de " + entrega.Cantidad + " " + epp.Nombre;
                        }
                        else
                        {

                            ViewBag.Message = "Colaborador no existe";
                        }
                    }
                    else
                    {

                        ViewBag.Message = "La entrega ya ha sido firmada o no existe.";
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


        public ActionResult CopiarActa(int id, string valor, string vista) 
        {

            
          
            List<DetalleEntregaEPP> model = new List<DetalleEntregaEPP>();

            using (var db = new AutogestionContext())
            {

                if (valor == "1")
                {

                    List<DetalleEntregaEPP> Entrega = new List<DetalleEntregaEPP>();
                    Entrega = db.DetalleEntregaEPP.Where(e => e.NumeroEntrega == id && e.Estado != "Anulado").ToList();
                    if (Entrega.Count != 0 )
                    {

                        model = _repo.CrearCopia(Entrega, id);
                        model = model.OrderBy(e => e.Empleado.Nombres).ToList();
                        return RedirectToAction("CopiarActa", new { id = model[0].NumeroEntrega, valor = "2", vista = "1" });
                    }
                    else 
                    {
                        Session["message"] = "El acta que intenta copiar no tiene registros.";

                        if (vista == "2")
                        {
                            return RedirectToAction("InformeCerradas", "EntregaEPP");
                        }
                        else { return RedirectToAction("ListaPorCompletar", "EntregaEPP"); }
                        
                    }
                }
                else {

                    model = db.DetalleEntregaEPP.Where(e => e.NumeroEntrega == id && e.Estado != "Anulado").ToList();
                    foreach (DetalleEntregaEPP Item in model)
                    {
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                        Item.ElementosProtecionPersonal = db.ElementosProtecionPersonal.FirstOrDefault(e => e.Id == Item.EPP);
                    }
                    model = model.OrderBy(e => e.Empleado.Nombres).ToList();
                }

                if (id != 0)
                {
                    ViewBag.NumeroEntrega = model[0].NumeroEntrega;
                }
               

            }


            return View(model);
        }


        public ActionResult EditarCopiaActa(int id) 
        {

            using (var db = new AutogestionContext())
            {
                DetalleEntregaEPP detalle = db.DetalleEntregaEPP.Find(id);
                if (detalle == null)
                {
                    return HttpNotFound();
                }

                ViewBag.Empleado = db.Empleados.Select(e => new { e.Id, e.Nombres, e.Activo, nomcodigo = e.Id + "-" + e.Nombres, e.FechaFin }).Where(f => f.Activo != "NO" || f.FechaFin >= DateTime.Now).ToList();
                ViewBag.Elementos =  db.ElementosProtecionPersonal.Select(e => new { e.Id, e.Nombre, e.Activo, nomcodigo = e.Id + "-" + e.Nombre }).ToList().Where(f => f.Activo == "1");

                var fechafin = detalle.FechaFin;
                ViewBag.fecha = fechafin;
                return View(detalle);
            }

            
        }



        [HttpPost]
        public ActionResult EditarCopiaActa(int id, DetalleEntregaEPP model)
        {
            try
            {
               
                    int numero = _repo.EditarCopia(id, model);

                    return RedirectToAction("CopiarActa", new { id = numero, valor = "2", vista = "1" });
                
            }
            catch
            {
                return View();
            }

            
        }

        public ActionResult EliminarActa(int id)
        {
            using (var db = new AutogestionContext())
            {
                DetalleEntregaEPP tipo = db.DetalleEntregaEPP.Find(id);
                
                    tipo.Empleado = db.Empleados.FirstOrDefault(e => e.Id == tipo.EmpleadoId);
                    tipo.ElementosProtecionPersonal = db.ElementosProtecionPersonal.FirstOrDefault(e => e.Id == tipo.EPP);
                
                if (tipo == null)
                {
                    return HttpNotFound();
                }
                return View(tipo);
            }
        }

        //
        // POST: /DetalleEntregaEPP/Delete/5

        [HttpPost]
        public ActionResult EliminarActa(int id, DetalleEntregaEPP model)
        {
            try
            {
                // TODO: Add delete logic here
                int numereo = _repo.Eliminar(id);
                return RedirectToAction("CopiarActa", new { id = numereo, valor = "2", vista = "1" });
            }
            catch
            {
                return View();
            }
        }


        public ActionResult ModificarFechas(int id)
        {
            Session["NumeroEntrega"] = id;
            return PartialView();
        }

        public ActionResult Guardafechas(string Fecha, string FechaFin)
        {
            try
            {
                int id = 0;
                string numero = String.Format("{0}", Session["NumeroEntrega"]);
                Int32.TryParse(numero, out id);
                _repo.Cambiofechas(id, Fecha, FechaFin);
                return RedirectToAction("CopiarActa", new { id = id, valor = "2", vista = "1" });
            }
            catch
            {
                return View();
            }

            
        }

        public ActionResult DetalleActa2(int id)
        {

            
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("DetalleActa2"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

           

            var entrega = _repo.ObtenerTodos(id);
            ViewBag.Empleado = entrega[0].Empleado2.Nombres;
            ViewBag.Fecha = entrega[0].EntregaEPP.Fecha;
            ViewBag.Area = entrega[0].EntregaEPP.Area;
            ViewBag.Sociedad = entrega[0].Sociedades.Descripcion;

            string textoqr = id + "|" + entrega[0].EntregaEPP.Fecha + "|" + entrega[0].EntregaEPP.Empleado;
            byte[] ImagenQR = _servicios.GenerarQR(textoqr);
            ViewBag.Codigo = Convert.ToBase64String(ImagenQR);
            
            return View(entrega);
        }

        //
        // GET: /DetalleEntregaEPP/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /DetalleEntregaEPP/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /DetalleEntregaEPP/Create

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
        // GET: /DetalleEntregaEPP/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /DetalleEntregaEPP/Edit/5

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
        // GET: /DetalleEntregaEPP/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /DetalleEntregaEPP/Delete/5

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
    }
}
