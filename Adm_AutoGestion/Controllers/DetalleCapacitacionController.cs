using Adm_AutoGestion.DTO;
using Adm_AutoGestion.Models;
using Adm_AutoGestion.Models.EvaDesempeno;
using Adm_AutoGestion.Services;
using K4os.Hash.xxHash;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Controllers
{
    public class DetalleCapacitacionController : Controller
    {
        private AutogestionContext db= new AutogestionContext();
        private DetalleCapacitacionRepository _repo;
        public EduFoscalRepository _EduFoscalRepository = new EduFoscalRepository();
        public DetalleCapacitacionController()
        {
            _repo = new DetalleCapacitacionRepository();
        }
        //
        // GET: /DetalleCapacitacion/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /DetalleCapacitacion/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /DetalleCapacitacion/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /DetalleCapacitacion/Create
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
        // GET: /DetalleCapacitacion/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /DetalleCapacitacion/Edit/5
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
        // GET: /DetalleCapacitacion/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /DetalleCapacitacion/Delete/5
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

        public ActionResult RegistrarCapacitacion(int? id, List<string> Area, List<string> Cargo, string Empresa, string TipoArea)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }

            if (id == null)
            {
                return HttpNotFound();
            }

            ViewBag.SelectedEmpresa = Empresa;
            ViewBag.SelectedArea = Area;
            ViewBag.SelectedCargo = Cargo;
            ViewBag.SelectedTipoArea = TipoArea;

            var cap = db.Capacitacion.FirstOrDefault(x => x.Id == id);

            if (cap == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCap = id;
            var FiltroEmpresa = cap.Empresa;

            ViewBag.Areas = db.Empleados.Where(x => !string.IsNullOrEmpty(x.Area) && !string.IsNullOrEmpty(x.AreaDescripcion) && x.Activo == "SI" && x.Empresa == FiltroEmpresa).Select(x => new { x.AreaDescripcion }).GroupBy(x => x.AreaDescripcion).ToList();
            ViewBag.Cargo = "";
            ViewBag.Empresa = db.Sociedad.Where(x => x.Codigo == FiltroEmpresa).Select(x => new { x.Codigo, x.Descripcion }).ToList();

            //var dirigidoA = cap.DirigidoASelect.Split(',')
            //                 .Select(int.Parse)
            //                 .ToList();

            var tiposAreas = new List<SelectListItem>
            {
                new SelectListItem { Value = "Todos", Text = "Todos los colaboradores" },
                new SelectListItem { Value = "Jefes", Text = "Jefes y Coordinadores" }
            };

            //if (dirigidoA.Contains(1))
            //{
                //tiposAreas.Add(new SelectListItem { Value = "Todos", Text = "Todos los colaboradores" });
                //tiposAreas.Add(new SelectListItem { Value = "Administrativos CO", Text = "Administrativos CO" });
                //tiposAreas.Add(new SelectListItem { Value = "Asistenciales CO", Text = "Asistenciales CO" });
                //tiposAreas.Add(new SelectListItem { Value = "Adtivos/Asisten CO", Text = "Adtivos/Asisten CO" });
                //tiposAreas.Add(new SelectListItem { Value = "Expuestos Radiac CO", Text = "Expuestos Radiac CO" });
                //tiposAreas.Add(new SelectListItem { Value = "Pensionado Activo CO", Text = "Pensionado Activo CO" });
                //tiposAreas.Add(new SelectListItem { Value = "Programa 40000", Text = "Programa 40000" });
                //tiposAreas.Add(new SelectListItem { Value = "Jefes", Text = "Jefes y Coordinadores" });
            //}
            //if (dirigidoA.Contains(2))
            //{
            //    tiposAreas.Add(new SelectListItem { Value = "Jefes", Text = "Jefes y Coordinadores" });
            //}
            //if (dirigidoA.Contains(3))
            //{
            //    tiposAreas.Add(new SelectListItem { Value = "Asistenciales CO", Text = "Asistenciales CO" });
            //}
            //if (dirigidoA.Contains(4))
            //{
            //    tiposAreas.Add(new SelectListItem { Value = "Administrativos CO", Text = "Administrativos CO" });
            //}

            ViewBag.TipoArea = tiposAreas;

            if (Area == null && Cargo == null && string.IsNullOrEmpty(Empresa) && string.IsNullOrEmpty(TipoArea))
            {
                ViewBag.Empleados = "";
            }
            else
            {
                var registrosValidos = new List<int>();

                if (cap.IdCursoNormativo != 0)
                {
                    var posiblesRegistrosValidos = db.Certificados.Where(x => x.IdCursoNormativo == cap.IdCursoNormativo && x.Estado == "Aceptado").ToList();

                    foreach (var r in posiblesRegistrosValidos)
                    {
                        if (r.FechaCaducidad != null)
                        {
                            if (DateTime.Now < r.FechaCaducidad)
                            {
                                registrosValidos.Add(r.EmpleadoId);
                            }
                        }
                        else
                        {
                            registrosValidos.Add(r.EmpleadoId);
                        }
                    }
                }

                var datos = _repo.ObtenerEmpleados(Area, Cargo, Empresa, TipoArea, registrosValidos);
                ViewBag.Empleados = datos;
            }

            if (cap.DirigidoASelect.Contains("1") || cap.DirigidoASelect.Contains("2") || cap.DirigidoASelect.Contains("3") || cap.DirigidoASelect.Contains("4"))
            {
                ViewBag.Empleado = "";
            }
            else
            {
                ViewBag.Empleado = null;
            }

            if (cap.DirigidoASelect.Contains("5"))
            {
                ViewBag.Terceros = "";
            }
            else
            {
                ViewBag.Terceros = null;
            }

            if (cap.DirigidoASelect.Contains("6"))
            {
                ViewBag.Estudiantes = "";
            }
            else
            {
                ViewBag.Estudiantes = null;
            }

            ViewBag.Fecha = cap.FechaCapacitacion;

            return View();
        }

        public JsonResult ObtenerAreasPorPerfil(string empresa, string perfil)
        {
            if (string.IsNullOrEmpty(empresa) || string.IsNullOrEmpty(perfil))
            {
                return Json(new { Success = false, Message = "Empresa o Perfil no especificada" }, JsonRequestBehavior.AllowGet);
            }

            using (var db = new AutogestionContext())
            {

                if (perfil == "Jefes")
                {
                    var codigosJefes = db.Empleados.Where(x => !string.IsNullOrEmpty(x.Jefe) && x.Activo == "SI").Select(x => x.Jefe).Distinct().ToList();
                    var codigosSuperiores = db.Empleados.Where(x => !string.IsNullOrEmpty(x.Superior) && x.Activo == "SI").Select(x => x.Superior).Distinct().ToList();
                    var Codigos = codigosJefes.Union(codigosSuperiores).ToList();

                    var areas = db.Empleados
                        .Where(x => x.Empresa == empresa && Codigos.Contains(x.NroEmpleado) && !string.IsNullOrEmpty(x.AreaDescripcion) && x.Activo == "SI")
                        .Select(x => new { x.AreaDescripcion })
                        .GroupBy(x => x.AreaDescripcion)
                        .Select(g => new { Area = g.Key })
                        .ToList();

                    return Json(new { Success = true, Areas = areas }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var areas = db.Empleados
                        .Where(x => x.Empresa == empresa && !string.IsNullOrEmpty(x.AreaDescripcion) && x.Activo == "SI")
                        .Select(x => new { x.AreaDescripcion })
                        .GroupBy(x => x.AreaDescripcion)
                        .Select(g => new { Area = g.Key })
                        .ToList();

                    return Json(new { Success = true, Areas = areas }, JsonRequestBehavior.AllowGet);
                }

            }
        }

        public JsonResult ObtenerCargosPorArea(List<string> area, string empresa, string perfil)
        {
            if (string.IsNullOrEmpty(empresa) || string.IsNullOrEmpty(perfil))
            {
                return Json(new { Success = false, Message = "Empresa o Perfil no especificada" }, JsonRequestBehavior.AllowGet);
            }

            using (var db = new AutogestionContext())
            {

                if (perfil == "Jefes")
                {
                    var codigosJefes = db.Empleados.Where(x => !string.IsNullOrEmpty(x.Jefe) && x.Activo == "SI").Select(x => x.Jefe).Distinct().ToList();
                    var codigosSuperiores = db.Empleados.Where(x => !string.IsNullOrEmpty(x.Superior) && x.Activo == "SI").Select(x => x.Superior).Distinct().ToList();
                    var Codigos = codigosJefes.Union(codigosSuperiores).ToList();

                    if (area == null)
                    {
                        var cargos = db.Empleados
                        .Where(x => !string.IsNullOrEmpty(x.Cargo) && x.Empresa == empresa && Codigos.Contains(x.NroEmpleado) && x.Activo == "SI")
                        .Select(x => new { x.Cargo })
                        .GroupBy(x => x.Cargo)
                        .Select(g => new { Cargo = g.Key })
                        .ToList();

                        return Json(new { Success = true, Cargos = cargos }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var cargos = db.Empleados
                            .Where(x => area.Contains(x.AreaDescripcion) && x.Empresa == empresa && Codigos.Contains(x.NroEmpleado) && x.Activo == "SI")
                            .Select(x => new { x.Cargo })
                            .GroupBy(x => x.Cargo)
                            .Select(g => new { Cargo = g.Key })
                            .ToList();

                        return Json(new { Success = true, Cargos = cargos }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    if (area == null)
                    {
                        var cargos = db.Empleados
                            .Where(x => !string.IsNullOrEmpty(x.Cargo) && x.Empresa == empresa && x.Activo == "SI")
                            .Select(x => new { x.Cargo })
                            .GroupBy(x => x.Cargo)
                            .Select(g => new { Cargo = g.Key })
                            .ToList();

                        return Json(new { Success = true, Cargos = cargos }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var cargos = db.Empleados
                            .Where(x => area.Contains(x.AreaDescripcion) && x.Empresa == empresa && x.Activo == "SI")
                            .Select(x => new { x.Cargo })
                            .GroupBy(x => x.Cargo)
                            .Select(g => new { Cargo = g.Key })
                            .ToList();

                        return Json(new { Success = true, Cargos = cargos }, JsonRequestBehavior.AllowGet);
                    }

                }

            }
        }


        public class MoodleClient
        {
            private static readonly HttpClient client = new HttpClient();
            private readonly string token;
            private readonly string moodleUrl;


            public MoodleClient(string moodleUrl, string token)
            {
                this.moodleUrl = moodleUrl;
                this.token = token;
            }

            public async Task<int> GetUserIdByUsername(string username)
            {
                var function = "core_user_get_users";
                var url = $"{moodleUrl}/webservice/rest/server.php";

                var parameters = new Dictionary<string, string>
                {
                    {"wstoken", token },
                    {"wsfunction", function },
                    {"moodlewsrestformat", "json" },
                    {"criteria[0][key]", "username" },
                    {"criteria[0][value]", username }
                };

                var content = new FormUrlEncodedContent(parameters);

                var response = await client.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = JObject.Parse(responseString);
                    var users = responseJson["users"];
                    if (users != null && users.HasValues)
                    {
                        return (int)users[0]["id"];
                    }
                }
                return 0;
            }

            public async Task<string> EnrollUserInCourse(string courseId, int userId, string roleId = "5")
            {
                var function = "enrol_manual_enrol_users";
                var url = $"{moodleUrl}/webservice/rest/server.php";

                var parameters = new Dictionary<string, string>
                {
                    {"wstoken", token },
                    {"wsfunction", function },
                    {"moodlewsrestformat", "json" },
                    {"enrolments[0][roleid],", roleId },
                    {"enrolments[0][userid]", userId.ToString() },
                    {"enrolments[0][courseid]", courseId }
                };

                var content = new FormUrlEncodedContent(parameters);

                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return "true";
                }
                else
                {
                    return "false";
                }
            }

        }

        [HttpPost]
        public async Task<JsonResult> RegistrarParticipantes()
        {
            var respuesta = "";
            try
            {
                var id = HttpContext.Request.Params["IdCap"];
                var Empleados = HttpContext.Request.Params["Empleados"];
                //var Terceros = HttpContext.Request.Params["Terceros"];
                //var CantidadTer = HttpContext.Request.Params["CantidadTer"];
                var CantidadEmp = HttpContext.Request.Params["CantidadEmp"];
                var IdCap = Convert.ToInt32(id);
                var cantidadEmp = Convert.ToInt32(CantidadEmp);
                //var cantidadTer = Convert.ToInt32(CantidadTer);

                var capacitacion = db.Capacitacion.FirstOrDefault(x => x.Id == IdCap);
                var CursoId = int.Parse(capacitacion.CursoId);
                var model = new List<DetalleCapacitacion>();

                var empleadosOmitidos = 0;

                if (!string.IsNullOrEmpty(Empleados))
                {
                    string[] Emps = Empleados.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    var moodleClient = new MoodleClient("https://edufoscal.com/talentofoscal", "685a460d9a1e98f914d148a0de4fd487");

                    for (var i = 0; i < cantidadEmp; i++)
                    {
                        var detalle = new DetalleCapacitacion();
                        string[] Emps2 = Emps[i].Split(new[] { ';' }).ToArray();
                        var empleadoId = Convert.ToInt32(Emps2[0]);
                        var username = Emps2[2];

                        if (!db.DetalleCapacitacion.Any(x => x.CapacitacionId == IdCap && x.EmpleadoId == empleadoId))
                        {
                            detalle.Estado = "Activo";
                            detalle.EsTercero = "NO";
                            detalle.EmpleadoId = empleadoId;
                            detalle.Cargo = Emps2[4];
                            detalle.Area = Emps2[3];
                            detalle.CapacitacionId = IdCap;
                            //detalle.SedeId = capacitacion.IdSede;
                            model.Add(detalle);

                            if (capacitacion.CursoId != "0")
                            {
                                var moodleUserId = await moodleClient.GetUserIdByUsername(username);
                                
                                if (moodleUserId != 0)
                                {
                                    var respuestaInscripcion = await moodleClient.EnrollUserInCourse(capacitacion.CursoId, moodleUserId);
                                }
                            }

                        }
                        else if (db.DetalleCapacitacion.Any(x => x.CapacitacionId == IdCap && x.EmpleadoId == empleadoId && x.Estado == "Anulado"))
                        {
                            detalle = db.DetalleCapacitacion.FirstOrDefault(x => x.CapacitacionId == IdCap && x.EmpleadoId == empleadoId);
                            detalle.Estado = "Activo";
                            db.SaveChanges();
                        }
                        else if (db.DetalleCapacitacion.Any(x => x.CapacitacionId == IdCap && x.EmpleadoId == empleadoId && x.Estado == "Activo"))
                        {
                            empleadosOmitidos++;

                        }

                    }

                    cantidadEmp -= empleadosOmitidos;
                }

                var dbrespuesta = _repo.Crear(model, IdCap, cantidadEmp, false);

                if (dbrespuesta == "true")
                {
                    var mensaje = $"Se registraron los participantes correctamente.<br>{empleadosOmitidos} empleados omitidos por inscripción duplicada.";
                    respuesta = mensaje;
                }
                else
                {
                    respuesta = dbrespuesta;
                }

            }
            catch (SystemException ex)
            {
                respuesta = String.Format("Error al guardar. {0}", ex.Message);
                return Json(respuesta);
            }

            return Json(respuesta);
        }

        public ActionResult RegistrarCapacitacionExt(string id)
        {

            var model = new DetalleCapacitacion();
            using (var db = new AutogestionContext())
            {
                ArrayList lst = new ArrayList();
                //model.ListadoTiposInc = db.TiposIncapacidad.Where(e => e.EstadoId == 13).ToList();
                var terceros = db.Tercero.Select(e => new { e.Id, e.Nombres, e.Superior, e.Documento, e.Area, e.Cargo, nomcodigo = e.Id + "-" + e.Documento + "-" + e.Area + "-" + e.Cargo + "-" + e.Superior + "-" + "SI" }).ToArray();
                foreach (var item in terceros)
                {
                    lst.Add(item);

                }
                var emp3 = (from emp in db.Empleados
                            join e in db.Empleados on emp.Lider equals e.NroEmpleado into personal
                            from ps in personal.DefaultIfEmpty()
                            where emp.Activo != "NO" || emp.FechaFin >= DateTime.Now
                            select new { emp.Id, emp.Documento, emp.Cargo, Jefe = emp.Lider, emp.Nombres, emp.Activo, nomcodigo = emp.Id + "-" + emp.Documento + "-" + emp.Area + "-" + emp.Cargo + "-" + ps.Nombres + "-" + "NO", emp.FechaFin }
                  ).ToArray();

                foreach (var item in emp3)
                {
                    lst.Add(item);

                }


                ViewBag.Empleado = lst;

                //ViewBag.Empleado = db.Empleados.Select(e => new { e.Id, e.Nombres, e.Activo, nomcodigo = e.Id + "-" + e.Nombres, e.FechaFin }).Where(f => f.Activo != "NO" || f.FechaFin >= DateTime.Now).ToList();
                ViewBag.Numero = id;

            }

            return View(model);
        }

        public ActionResult DetallesCapacitacion(int id) 
        {
            //List<string> funciones = Acceso.Validar(Session["Empleado"]);

            //if (Acceso.EsAnonimo)
            //{
            //    return RedirectToAction("Login", "Login");
            //}
           

            var entrega = _repo.ObtenerTodos(id);
            ViewBag.NumeroEnt = id;
            return View(entrega);
        }
        [HttpGet]
        public ActionResult FirmaAsistencia(string str)
        {

            try
            {

                var base64EncodedBytes = System.Convert.FromBase64String(str);

                string[] valores = System.Text.Encoding.UTF8.GetString(base64EncodedBytes).Split('|');
                var id = Convert.ToInt16(valores[0]);
                var id_empleado = Convert.ToInt16(valores[1]);
                var fuente = valores[2];
             

                DetalleCapacitacion detalle = new DetalleCapacitacion();
                Empleado empleado = new Empleado();
                if (fuente != "email")
                {


                    ViewBag.Message = "no valido para firmar";

                }
                using (var db = new AutogestionContext())
                {
                    detalle = db.DetalleCapacitacion.FirstOrDefault(x => x.Id == id && (x.FechaFirma == null || x.FechaFirma == ""));
                   

                    if (detalle != null)
                    {
                        var cap = db.Capacitacion.FirstOrDefault(z => z.Id == detalle.CapacitacionId);
                        empleado = db.Empleados.FirstOrDefault(x => x.Id == id_empleado);

                        if (empleado != null)
                        {
                            detalle.FechaFirma = DateTime.Now + "|" + "email";
                            db.SaveChanges();
                            ViewBag.Message = "Se ha Firmado la asistencia a la capacitación " + cap.Nombre;
                        }
                        else
                        {

                            ViewBag.Message = "Colaborador no existe";
                        }
                    }
                    else
                    {

                        ViewBag.Message = "La asistenia a la capacitación ya ha sido firmada o no existe.";
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



        [HttpPost]
        public String AnularRegistro(int id)
        {
            string opcion = "anular";
            using (var db = new AutogestionContext())
            {
                try
                {

                    DetalleCapacitacion detalle = new DetalleCapacitacion();
                    detalle = db.DetalleCapacitacion.FirstOrDefault(e => e.Id == id);

                    if (detalle.Estado == "Anulado")
                    {
                        return "No es posible Anular el registro debido a que se encuentra en estado " + detalle.Estado + ".";
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


    }
}
