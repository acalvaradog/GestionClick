using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Adm_AutoGestion.Models.EvaDesempeno;

namespace Adm_AutoGestion.Controllers
{
    public class ReconocimientoController : Controller
    {
        private AutogestionContext db = new AutogestionContext();
        private ReconocimientoRepository _repo;
        public ReconocimientoController()
        {
            _repo = new ReconocimientoRepository();
        }
        // GET: Reconocimiento
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DetalleReconocimientoGestion(int Id)
        {
            using (var db = new AutogestionContext())
            {
                Reconocimiento Items = new Reconocimiento();
                var reconocimiento = _repo.ObtenerDetallesReconocimiento(Id);
                ViewBag.Reconocimiento = reconocimiento;
                

                // Pasar el Id a la vista
                ViewBag.ReconocimientoId = Id;
                
                return PartialView("DetalleReconocimientoGestion");
            }
        }
        public ActionResult DetalleReconocimiento(int Id)
        {
            using (var db = new AutogestionContext())
            {
                Reconocimiento Items = new Reconocimiento();
                var reconocimiento = _repo.ObtenerDetallesReconocimiento(Id);
                ViewBag.Reconocimiento = reconocimiento;


                
                

                return PartialView("DetalleReconocimiento");
            }
        }
        public JsonResult RespuestajsonGestion()
        {

            int ReconocimientoId = 0;
            int IdUsuarioM = 0;
            var IdSolicitudParam = HttpContext.Request.Params["ReconocimientoId"];
            var Activo = HttpContext.Request.Params["Activo"];


            TipoReconocimiento TipoReconocimiento = new TipoReconocimiento();
            TipoReconocimiento = db.TipoReconocimiento.FirstOrDefault(o => o.Id == ReconocimientoId);
            Reconocimiento Reconocimiento = new Reconocimiento();

            var idUserlog = Convert.ToInt32(Session["Empleado"]);
            Empleado userlog = db.Empleados.Where(d => d.Id == idUserlog).FirstOrDefault();
            var Fechaactual = DateTime.Now;
            if (!int.TryParse(IdSolicitudParam, out ReconocimientoId) || ReconocimientoId == 0)
            {
                // Manejo de error: el valor no es un entero válido o es 0
                return Json(new
                {
                    respuesta = "Error: El valor de HorasExtraId no es válido."
                });
            }
            Reconocimiento = db.Reconocimientos.FirstOrDefault(o => o.Id == ReconocimientoId);
            var respuesta = "";
            string message = null;
            try
            {
                string modifica = String.Format("{0}", Session["Empleado"]);
                Int32.TryParse(modifica, out IdUsuarioM);
                var url = Url.Action("GestionHumana");
                using (var db = new AutogestionContext())
                {

                    if (ReconocimientoId != 0)
                    {
                        if (Activo != "Seleccionado...")
                        {
                            if (Activo == "Desactivar")
                            {
                                Activo = "false";
                            }
                            

                            var a = _repo.ModificarGH(ReconocimientoId, IdUsuarioM);
                            if (a)
                            {
                                var Opcion = "GestionHumana";
                                

                            }
                            else
                            {

                                return Json(new
                                {
                                    redirectUrl = url,
                                    isRedirect = true,
                                    respuesta = "Guardado Exitoso"
                                });
                            }

                        }

                    }

                }
                return Json(new
                {
                    redirectUrl = url,
                    isRedirect = true,
                    respuesta = "Guardado Exitoso"
                });
            }
            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
                Session["message"] = message;
                respuesta = "Error durante el guardado";
                return Json(new { respuesta });
            }
        }
        public ActionResult GestionHumana(string JefeID, string TrabajadorS, string FechaI, string FechaF, string TipoReconocimiento, string NmrRegistro, string AreaDescripcion, string Empresa, string Documento, string NroEmpleado, string Activo)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);


            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ReconocimientoGestionHumana"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var Empleadolog = Session["Empleado"];
            var TipoReconocimientoId = 0;
            int JefeID2 = 0;
            bool? activo = null;
            if (!string.IsNullOrEmpty(Activo))
            {
                activo = Convert.ToBoolean(Activo);
            }
            if (JefeID == null || JefeID == "")
            {
                JefeID2 = Convert.ToInt32(Empleadolog);
            }
            List<Reconocimiento> Proceso = new List<Reconocimiento>();


            using (var db = new AutogestionContext())
            {

                //----------------List Empleados------------------------------//
                var Jefe = db.Empleados.FirstOrDefault(e => e.Id == JefeID2);
                ViewBag.Empleado = db.Empleados.Where(f => f.Activo != "NO").ToList();
                ViewBag.TipoReconocimiento = db.TipoReconocimiento.ToList();
                var areaDescripcionGroups = db.Empleados.Where(f => f.Activo != "NO" && f.AreaDescripcion != null && f.AreaDescripcion != "").GroupBy(b => b.AreaDescripcion).ToList();
                ViewBag.AreaDescripcion = new List<SelectListItem>();
                foreach (var x in areaDescripcionGroups)
                {
                    Empleado Item = x.FirstOrDefault();
                    ViewBag.AreaDescripcion.Add(new SelectListItem() { Value = Item.AreaDescripcion, Text = Item.AreaDescripcion });
                }
                DateTime Fecha1 = DateTime.Now;
                DateTime Fecha2 = DateTime.Now;

                List<SelectListItem> lst = new List<SelectListItem>();
                Int32 IdProceso = new Int32();


                if (TipoReconocimiento != null && TipoReconocimiento != "") { TipoReconocimientoId = Convert.ToInt32(TipoReconocimiento); }


                if (NmrRegistro != "")
                {
                    IdProceso = Convert.ToInt32(NmrRegistro);
                }

                if (!DateTime.TryParse(FechaI, out Fecha1))
                {
                    Fecha1 = new DateTime();
                }

                if (!DateTime.TryParse(FechaF, out Fecha2))
                {
                    Fecha2 = DateTime.Now;
                }
                Fecha2 = Fecha2.AddDays(1);
                if (TrabajadorS == "")
                {
                    if (TipoReconocimiento == "")
                    {
                        Proceso = db.Reconocimientos.Where(e => DbFunctions.TruncateTime(e.Fecha) >= Fecha1 &&
                                          DbFunctions.TruncateTime(e.Fecha) <= Fecha2

                                          && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrRegistro)
                                          ).ToList();
                    }
                    else
                    {
                        Proceso = db.Reconocimientos.Where(e => DbFunctions.TruncateTime(e.Fecha) >= Fecha1 &&
                                     e.TipoReconocimientoId == TipoReconocimientoId
                                     &&
                                     DbFunctions.TruncateTime(e.Fecha) <= Fecha2
                                     && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrRegistro)
                                     ).ToList();
                    }

                }
                if (TrabajadorS != "" && TrabajadorS != null)
                {

                    if (TipoReconocimiento == "")
                    {
                        int id = -1;
                        Int32.TryParse(TrabajadorS, out id);


                        //int[] Ids = db.Reconocimientos.Where(x => x.EmpleadoReconocidoId == id).Select(x => x.Id).ToArray();

                        Proceso = db.Reconocimientos.Where(e => 
                                    e.EmpleadoReconocidoId == id && e.Fecha >= Fecha1 && e.Fecha <= Fecha2
                                     && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrRegistro)).ToList();

                    }
                    else
                    {
                        int id = -1;
                        Int32.TryParse(TrabajadorS, out id);
                        //int[] Ids = db.Reconocimientos.Where(x => x.EmpleadoReconocidoId.Equals(id)).Select(x => x.Id).ToArray();
                        Proceso = db.Reconocimientos.Where(e =>
                                    e.EmpleadoReconocidoId == id && e.Fecha >= Fecha1 && e.Fecha <= Fecha2
                                      &&
                                    e.TipoReconocimientoId == TipoReconocimientoId && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrRegistro)).ToList();

                        



                    }

                }
                List<Empleado> Empleados_Jefe = db.Empleados.Where(x => x.Jefe == Jefe.NroEmpleado && x.Activo == "SI").ToList();
                foreach (Reconocimiento Item in Proceso.Reverse<Reconocimiento>())
                {
                    var x = Item.Id;
                    string Id = "" + x;
                    var Implicados = _repo.ObtenerTodos2(Id);
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.EmpleadoReconocido = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoReconocidoId);

                    Item.TipoReconocimiento = db.TipoReconocimiento.FirstOrDefault(e => e.Id == Item.TipoReconocimientoId);

                }
                if (!string.IsNullOrWhiteSpace(AreaDescripcion) || !string.IsNullOrWhiteSpace(Empresa) || !string.IsNullOrWhiteSpace(Documento) || !string.IsNullOrWhiteSpace(NroEmpleado) || !string.IsNullOrWhiteSpace(Activo))
                {
                    
                    List<Reconocimiento> nuevaLista = new List<Reconocimiento>();

                    
                    foreach (var item in Proceso)
                    {
                        
                        if ((!string.IsNullOrWhiteSpace(AreaDescripcion) && item.EmpleadoReconocido.AreaDescripcion != AreaDescripcion) ||
                            (!string.IsNullOrWhiteSpace(Empresa) && item.EmpleadoReconocido.Empresa != Empresa) ||
                            (!string.IsNullOrWhiteSpace(Documento) && item.EmpleadoReconocido.Documento != Documento) ||
                            (!string.IsNullOrWhiteSpace(NroEmpleado) && item.EmpleadoReconocido.NroEmpleado != NroEmpleado) ||
                            (!string.IsNullOrWhiteSpace(Activo) && item.Activo != bool.Parse(Activo)))
                        {
                            
                            continue;
                        }

                        
                        nuevaLista.Add(item);
                    }

                    // Actualizar la lista de reconocimientos con la nueva lista filtrada
                    Proceso = nuevaLista;
                }

                if (string.IsNullOrWhiteSpace(FechaI) || string.IsNullOrWhiteSpace(FechaF) || !DateTime.TryParse(FechaI, out Fecha1) || !DateTime.TryParse(FechaF, out Fecha2))
                {
                    return View(new List<Reconocimiento>());
                }

            }
            return View(Proceso);

        }
        public ActionResult JefeDirecto(string JefeID, string TrabajadorS, string FechaI, string FechaF, string TipoReconocimiento, string NmrRegistro, string AreaDescripcion, string Empresa, string Documento, string NroEmpleado)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);


            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ReconocimientoGestionHumana"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var Empleadolog = Session["Empleado"];
            var TipoReconocimientoId = 0;
            int JefeID2 = 0;
            if (JefeID == null || JefeID == "")
            {
                JefeID2 = Convert.ToInt32(Empleadolog);
            }
            List<Reconocimiento> Proceso = new List<Reconocimiento>();


            using (var db = new AutogestionContext())
            {

                //----------------List Empleados------------------------------//
                var Jefe = db.Empleados.FirstOrDefault(e => e.Id == JefeID2);
                ViewBag.Empleado = db.Empleados.Where(f => f.Activo != "NO" && f.Jefe == Jefe.NroEmpleado).ToList();
                var empleadosDelJefe = db.Empleados.Where(f => f.Activo != "NO" && f.Jefe == Jefe.NroEmpleado).Select(e => e.Id).ToList();
                ViewBag.TipoReconocimiento = db.TipoReconocimiento.ToList();
                var areaDescripcionGroups = db.Empleados.Where(f => f.Activo != "NO" && f.AreaDescripcion != null && f.AreaDescripcion != "").GroupBy(b => b.AreaDescripcion).ToList();
                ViewBag.AreaDescripcion = new List<SelectListItem>();
                foreach (var x in areaDescripcionGroups)
                {
                    Empleado Item = x.FirstOrDefault();
                    ViewBag.AreaDescripcion.Add(new SelectListItem() { Value = Item.AreaDescripcion, Text = Item.AreaDescripcion });
                }
                DateTime Fecha1 = DateTime.Now;
                DateTime Fecha2 = DateTime.Now;

                List<SelectListItem> lst = new List<SelectListItem>();
                Int32 IdProceso = new Int32();


                if (TipoReconocimiento != null && TipoReconocimiento != "") { TipoReconocimientoId = Convert.ToInt32(TipoReconocimiento); }


                if (NmrRegistro != "")
                {
                    IdProceso = Convert.ToInt32(NmrRegistro);
                }

                if (!DateTime.TryParse(FechaI, out Fecha1))
                {
                    Fecha1 = new DateTime();
                }

                if (!DateTime.TryParse(FechaF, out Fecha2))
                {
                    Fecha2 = DateTime.Now;
                }
                Fecha2 = Fecha2.AddDays(1);
                if (TrabajadorS == "")
                {
                    if (TipoReconocimiento == "")
                    {
                        Proceso = db.Reconocimientos.Where(e => empleadosDelJefe.Contains(e.EmpleadoReconocidoId ?? 0) && e.Activo && DbFunctions.TruncateTime(e.Fecha) >= Fecha1 &&
                                          DbFunctions.TruncateTime(e.Fecha) <= Fecha2

                                          && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrRegistro)
                                          ).ToList();
                    }
                    else
                    {
                        Proceso = db.Reconocimientos.Where(e => empleadosDelJefe.Contains(e.EmpleadoReconocidoId ?? 0) && e.Activo && DbFunctions.TruncateTime(e.Fecha) >= Fecha1 &&
                                     e.TipoReconocimientoId == TipoReconocimientoId
                                     &&
                                     DbFunctions.TruncateTime(e.Fecha) <= Fecha2
                                     && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrRegistro)
                                     ).ToList();
                    }

                }
                if (TrabajadorS != "")
                {

                    if (TipoReconocimiento == "")
                    {
                        int id = -1;
                        Int32.TryParse(TrabajadorS, out id);


                        //int[] Ids = (from item in db.Reconocimientos where item.EmpleadoReconocidoId.Equals(id) select item.Id).ToArray();

                        Proceso = db.Reconocimientos.Where(e => empleadosDelJefe.Contains(e.EmpleadoReconocidoId ?? 0) && e.Activo &&
                                    e.EmpleadoReconocidoId == id && e.Fecha >= Fecha1 && e.Fecha <= Fecha2
                                     && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrRegistro)).ToList();

                    }
                    else
                    {
                        int id = -1;
                        Int32.TryParse(TrabajadorS, out id);
                        //int[] Ids = (from item in db.Reconocimientos where item.EmpleadoReconocidoId.Equals(id) select item.Id).ToArray();
                        Proceso = db.Reconocimientos.Where(e => empleadosDelJefe.Contains(e.EmpleadoReconocidoId ?? 0) && e.Activo &&
                                    e.EmpleadoReconocidoId == id && e.Fecha >= Fecha1 && e.Fecha <= Fecha2
                                      &&
                                    e.TipoReconocimientoId == TipoReconocimientoId && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrRegistro)).ToList();

                        



                    }

                }
                List<Empleado> Empleados_Jefe = db.Empleados.Where(x => x.Jefe == Jefe.NroEmpleado && x.Activo == "SI").ToList();
                foreach (Reconocimiento Item in Proceso.Reverse<Reconocimiento>())
                {
                    var x = Item.Id;
                    string Id = "" + x;
                    var Implicados = _repo.ObtenerTodos2(Id);
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.EmpleadoReconocido = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoReconocidoId);

                    Item.TipoReconocimiento = db.TipoReconocimiento.FirstOrDefault(e => e.Id == Item.TipoReconocimientoId);

                }
                if (AreaDescripcion != null && AreaDescripcion != "" || (Empresa != null && Empresa != "") || (Documento != null && Documento != "") || (NroEmpleado != null && NroEmpleado != ""))
                {
                    List<Reconocimiento> nuevaLista = new List<Reconocimiento>();
                    foreach (var item in Proceso.Reverse<Reconocimiento>())
                    {
                        bool cumpleCriterio = true;

                        if (item.EmpleadoReconocido.AreaDescripcion != AreaDescripcion && !string.IsNullOrEmpty(AreaDescripcion))
                        {
                            cumpleCriterio = false;
                        }

                        if (item.EmpleadoReconocido.Empresa != Empresa && !string.IsNullOrEmpty(Empresa))
                        {
                            cumpleCriterio = false;
                        }

                        if (item.EmpleadoReconocido.Documento != Documento && !string.IsNullOrEmpty(Documento))
                        {
                            cumpleCriterio = false;
                        }

                        if (item.EmpleadoReconocido.NroEmpleado != NroEmpleado && !string.IsNullOrEmpty(NroEmpleado))
                        {
                            cumpleCriterio = false;
                        }

                        if (cumpleCriterio)
                        {
                            nuevaLista.Add(item);
                        }
                    }
                    Proceso = nuevaLista;

                }
                if (string.IsNullOrWhiteSpace(FechaI) || string.IsNullOrWhiteSpace(FechaF) || !DateTime.TryParse(FechaI, out Fecha1) || !DateTime.TryParse(FechaF, out Fecha2))
                {
                    return View(new List<Reconocimiento>());
                }

            }
            return View(Proceso);

        }


        // GET: Reconocimiento/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Reconocimiento/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Reconocimiento/Create
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

        // GET: Reconocimiento/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Reconocimiento/Edit/5
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

        // GET: Reconocimiento/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Reconocimiento/Delete/5
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
