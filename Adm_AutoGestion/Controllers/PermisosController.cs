using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using SAP.Middleware.Connector;
using SAPExtractorDotNET;
using System.Text;
using System.Data.Entity.SqlServer;


namespace Adm_AutoGestion.Controllers
{
    public class CalendarioPermisos
    {

        public string Nombres { get; set; }
        public DateTime FechaPermiso { get; set; }
        public DateTime FechaFinPermiso { get; set; }
        public string Estado { get; set; }

    }

    public class PermisosController : Controller
    {
        private PermisosRepository _repo;

        public PermisosController()
        {
            _repo = new PermisosRepository(); 
        }

        //
        // GET: /Permisos/


        //************************  Tabla Historico ********************
        public string draw = "";
        public string start = "";
        public string length = "";
        public string sortColumn = "";
        public string sortColumnDir = "";
        public string searchValue = "";
        public int pageSize, skip, recordsTotal;
        [HttpPost]
        public ActionResult Tabla_Historico_Json(int Id)
        {
            List<HistorialPermisos> lst = new List<HistorialPermisos>();

            //logistica datatable
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();


            pageSize = length != null ? Convert.ToInt32(length) : 0;
            skip = start != null ? Convert.ToInt32(start) : 0;
            recordsTotal = 0;


            //conexión base de datos
            using (var db = new AutogestionContext())
            {
                var query = db.HistorialPermisos.Join(db.Empleados, hipe => hipe.Usuario_Modifica, emple => emple.Id,
                    (hipe, emple) => new { hipe, emple }).Where(hipe => hipe.hipe.PermisoId == Id).Join(db.EstadosPermiso, HistPer => HistPer.hipe.Estado, estper => estper.Id.ToString(),
                    (HistPer, estper) => new { HistPer, estper }).ToList();
                recordsTotal = query.Count();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = query });
            }


        }
        //****************---------------------**************************

        //************************  Tabla Descarga Adjuntos ********************
        public string draw2 = "";
        public string start2 = "";
        public string length2 = "";
        public string sortColumn2 = "";
        public string sortColumnDir2 = "";
        public string searchValue2 = "";
        public int pageSize2, skip2, recordsTotal2;
        [HttpPost]
        public ActionResult Tabla_Descarga_Json(int Id)
        {

            List<PermisosAdjuntos> lst = new List<PermisosAdjuntos>();
            //logistica datatable
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();


            pageSize = length != null ? Convert.ToInt32(length) : 0;
            skip = start != null ? Convert.ToInt32(start) : 0;
            recordsTotal = 0;


            //conexión base de datos
            using (var db = new AutogestionContext())
            {
                var query = db.PermisosAdjuntos.Where(e => e.PermisosId == Id).ToList();
                recordsTotal = query.Count();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = query });
            }


        }
        //****************---------------------*****************************




        //
        // GET: /Permisos/
        //************************ Informe Permisos ************************
        public ActionResult Informe(string FechaRegIni, string FechaRegFin, string Jornada, string MotivoPermiso, string Empleado, string Estado, string Empresa, string IdEmpleado)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            string EmpresaSesion = Convert.ToString(Session["Empresa"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("PermisoInforme"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Login", "Login");
            }
            List<Permiso> Items = new List<Permiso>();

            using (var db = new AutogestionContext())
            {

                ViewBag.Empresa = db.Sociedad.Where(x => x.Codigo == EmpresaSesion).ToList();


                ViewBag.motivos = db.MotivosPermiso.Where(e => e.Activo == true).ToList();


                DateTime Fecha1 = DateTime.Now;
                DateTime Fecha2 = DateTime.Now;


                if (!DateTime.TryParse(FechaRegIni, out Fecha1))
                {
                    Fecha1 = new DateTime();
                }

                if (!DateTime.TryParse(FechaRegFin, out Fecha2))
                {
                    Fecha2 = DateTime.Now.AddYears(5);
                }


               int? Estadoid;
                int? MotivoPermisoId;
                if (string.IsNullOrEmpty(MotivoPermiso) && string.IsNullOrEmpty(Estado))
                {
                    Items = db.Permisos.Where(e =>
                                                  DbFunctions.TruncateTime(e.FechaPermiso) >= Fecha1
                                                  &&
                                                  DbFunctions.TruncateTime(e.FechaFinPermiso) <= Fecha2
                                                  &&
                                                  e.Jornada.Contains(Jornada)
                                                  &&
                                                  SqlFunctions.StringConvert((decimal)e.MotivoId).Contains(MotivoPermiso)
                                                  &&
                                                  e.Empleado.Nombres.Contains(Empleado)
                                                  &&
                                                  SqlFunctions.StringConvert((decimal)e.EstadoId).Contains(Estado)
                                                  &&
                                                  e.Empleado.Empresa == EmpresaSesion
                                                  &&
                                                  e.Empleado.NroEmpleado.Contains(IdEmpleado)                                                 
                                                  &&
                                                  e.Empleado.Activo == "SI"
                                                  ).ToList();
                }          //Consulta normal los dos parametros vacios


        else if (!string.IsNullOrEmpty(MotivoPermiso) && !string.IsNullOrEmpty(Estado))
                {
                    Estadoid = Convert.ToInt32(Estado);
                    MotivoPermisoId = Convert.ToInt32(MotivoPermiso);
                    Items = db.Permisos.Where(e =>
                                                  DbFunctions.TruncateTime(e.FechaPermiso) >= Fecha1
                                                  &&
                                                  DbFunctions.TruncateTime(e.FechaFinPermiso) <= Fecha2
                                                  &&
                                                  e.Jornada.Contains(Jornada)
                                                  &&
                                                  e.MotivoId == MotivoPermisoId
                                                  &&
                                                  e.Empleado.Nombres.Contains(Empleado)
                                                  &&
                                                  e.EstadoId == Estadoid
                                                  &&
                                                   e.Empleado.Empresa == EmpresaSesion
                                                  &&
                                                  e.Empleado.NroEmpleado.Contains(IdEmpleado)
                                                  &&
                                                  e.Empleado.Activo == "SI"
                                                ).ToList();
                }  //Consulta con los dos parametros con datos

            else if (string.IsNullOrEmpty(MotivoPermiso) && !string.IsNullOrEmpty(Estado)) 
                {
                    Estadoid = Convert.ToInt32(Estado);
                    Items = db.Permisos.Where(e =>
                                                  DbFunctions.TruncateTime(e.FechaPermiso) >= Fecha1
                                                  &&
                                                  DbFunctions.TruncateTime(e.FechaFinPermiso) <= Fecha2
                                                  &&
                                                  e.Jornada.Contains(Jornada)
                                                  &&
                                                  SqlFunctions.StringConvert((decimal)e.MotivoId).Contains(MotivoPermiso)
                                                  &&
                                                  e.Empleado.Nombres.Contains(Empleado)
                                                  &&
                                                  e.EstadoId == Estadoid
                                                  &&
                                                   e.Empleado.Empresa == EmpresaSesion
                                                  &&
                                                  e.Empleado.NroEmpleado.Contains(IdEmpleado)
                                                  &&
                                                  e.Empleado.Activo == "SI"
                                                ).ToList();
                }  //Consulta con datos en el parametro Estado y vacio el parametro MotivoPermiso


                else if (!string.IsNullOrEmpty(MotivoPermiso) && string.IsNullOrEmpty(Estado))
                {
                    MotivoPermisoId = Convert.ToInt32(MotivoPermiso);
                    Items = db.Permisos.Where(e =>
                                                  DbFunctions.TruncateTime(e.FechaPermiso) >= Fecha1
                                                  &&
                                                  DbFunctions.TruncateTime(e.FechaFinPermiso) <= Fecha2
                                                  &&
                                                  e.Jornada.Contains(Jornada)
                                                  &&
                                                  e.MotivoId == MotivoPermisoId
                                                  &&
                                                  e.Empleado.Nombres.Contains(Empleado)
                                                  &&
                                                  SqlFunctions.StringConvert((decimal)e.EstadoId).Contains(Estado)
                                                  &&
                                                  e.Empleado.Empresa == EmpresaSesion
                                                  &&
                                                  e.Empleado.NroEmpleado.Contains(IdEmpleado)
                                                  &&
                                                  e.Empleado.Activo == "SI"
                                                ).ToList();
                } //Consulta con datos en el parametro MotivoPermiso y vacio el parametro Estado




                foreach (Permiso Item in Items)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId && e.Empresa ==EmpresaSesion);
                    Item.EstadoPermiso = db.EstadosPermiso.FirstOrDefault(s => s.Id == Item.EstadoId);
                    Item.PersonalActivo = db.PersonalActivo.FirstOrDefault(j => j.CodigoEmpleado == Item.Empleado.NroEmpleado);
                    var catd = (Item.FechaFinPermiso.Date - Item.FechaPermiso.Date).Days;
                    Item.cantdias = string.Format("{0}", catd);


                }


            }
            return View(Items);

        }

        
        //****************---------------------*****************************

        //************************ Informe Permisos Jefe ************************
        public ActionResult Informe2(string FechaRegIni, string FechaRegFin, string Jornada, string MotivoPermiso, string Empleado, string Estado, string Empresa, string IdEmpleado)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            string EmpresaSesion = Convert.ToString(Session["Empresa"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("InformePermisoJefe"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Login", "Login");
            }
            List<Permiso> Items = new List<Permiso>();

            using (var db = new AutogestionContext())
            {

                ViewBag.Empresa = db.Sociedad.Where(x => x.Codigo == EmpresaSesion).ToList();


                ViewBag.motivos = db.MotivosPermiso.Where(e => e.Activo == true).ToList();


                DateTime Fecha1 = DateTime.Now;
                DateTime Fecha2 = DateTime.Now;


                if (!DateTime.TryParse(FechaRegIni, out Fecha1))
                {
                    Fecha1 = new DateTime(2008, 8, 29, 19, 27, 15, 18);
                }

                if (!DateTime.TryParse(FechaRegFin, out Fecha2))
                {
                    
                    Fecha2 = DateTime.Now.AddYears(5);

                    

                }
               

                int Usuario = 0;
                string usuariosesion = String.Format("{0}", Session["Empleado"]);
                Int32.TryParse(usuariosesion, out Usuario);
                var NroUsuario = db.Empleados.Where(e => e.Id == Usuario).Single();  
                
                ViewBag.EmpleadosJefe = db.Empleados.Where(e => e.Jefe == NroUsuario.NroEmpleado && e.Activo == "SI" && e.Empresa == EmpresaSesion || e.Lider == NroUsuario.NroEmpleado && e.NroEmpleado != NroUsuario.NroEmpleado && e.Activo == "SI" && e.Empresa == EmpresaSesion).ToList();
                var EmpleadosJ = db.Empleados.Where(e => e.Jefe == NroUsuario.NroEmpleado && e.Empresa == EmpresaSesion).ToList();
                var EmpleadosL = db.Empleados.Where(e => e.Lider == NroUsuario.NroEmpleado && e.Empresa == EmpresaSesion).ToList();

                int? Estadoid;
                int? MotivoPermisoId;
                if (string.IsNullOrEmpty(MotivoPermiso) && string.IsNullOrEmpty(Estado)) //Consulta normal los dos parametros vacios
                {
                    if (EmpleadosL.Count > 0)
                    {
                        Items = db.Permisos.Where(e =>
                                                  DbFunctions.TruncateTime(e.FechaPermiso) >= Fecha1
                                                  &&
                                                  DbFunctions.TruncateTime(e.FechaFinPermiso) <= Fecha2
                                                  &&
                                                  e.Jornada.Contains(Jornada)
                                                  &&
                                                  SqlFunctions.StringConvert((decimal)e.MotivoId).Contains(MotivoPermiso)
                                                  &&
                                                  SqlFunctions.StringConvert((decimal)e.Empleado.Id).Contains(Empleado)
                                                  &&
                                                  SqlFunctions.StringConvert((decimal)e.EstadoId).Contains(Estado)
                                                  &&
                                                  e.Empresa.Contains(Empresa)
                                                  &&
                                                  e.Empleado.NroEmpleado.Contains(IdEmpleado)
                                                  &&
                                                  e.Empleado.Lider.Contains(NroUsuario.NroEmpleado)
                                                  &&
                                                  e.Empleado.NroEmpleado != NroUsuario.NroEmpleado
                                                  &&
                                                  e.Empleado.Activo == "SI"
                                                  ).ToList();
                    }


                    if (EmpleadosJ.Count > 0)
                    {
                        Items = db.Permisos.Where(e =>
                                                DbFunctions.TruncateTime(e.FechaPermiso) >= Fecha1
                                                &&
                                                DbFunctions.TruncateTime(e.FechaFinPermiso) <= Fecha2
                                                &&
                                                e.Jornada.Contains(Jornada)
                                                &&
                                                SqlFunctions.StringConvert((decimal)e.MotivoId).Contains(MotivoPermiso)
                                                &&
                                                SqlFunctions.StringConvert((decimal)e.Empleado.Id).Contains(Empleado)
                                                &&
                                                SqlFunctions.StringConvert((decimal)e.EstadoId).Contains(Estado)
                                                &&
                                                e.Empresa.Contains(Empresa)
                                                &&
                                                e.Empleado.NroEmpleado.Contains(IdEmpleado)
                                                &&
                                                e.Empleado.Jefe.Contains(NroUsuario.NroEmpleado)
                                                &&
                                                e.Empleado.NroEmpleado != NroUsuario.NroEmpleado
                                                &&
                                                 e.Empleado.Activo == "SI"
                                                ).ToList();
                    }
                }

                else { 

                        if (!string.IsNullOrEmpty(MotivoPermiso) && string.IsNullOrEmpty(Estado)) //Consulta con datos en Motivo permiso
                        {
                                MotivoPermisoId = Convert.ToInt32(MotivoPermiso);
                            if (EmpleadosL.Count > 0)
                            {
                                Items = db.Permisos.Where(e =>
                                                          DbFunctions.TruncateTime(e.FechaPermiso) >= Fecha1
                                                          &&
                                                          DbFunctions.TruncateTime(e.FechaFinPermiso) <= Fecha2
                                                          &&
                                                          e.Jornada.Contains(Jornada)
                                                          &&
                                                          e.MotivoId == MotivoPermisoId
                                                          &&
                                                          SqlFunctions.StringConvert((decimal)e.Empleado.Id).Contains(Empleado)
                                                          &&
                                                          SqlFunctions.StringConvert((decimal)e.EstadoId).Contains(Estado)
                                                          &&
                                                          e.Empresa.Contains(Empresa)
                                                          &&
                                                          e.Empleado.NroEmpleado.Contains(IdEmpleado)
                                                          &&
                                                          e.Empleado.Lider.Contains(NroUsuario.NroEmpleado)
                                                          &&
                                                          e.Empleado.NroEmpleado != NroUsuario.NroEmpleado
                                                          &&
                                                          e.Empleado.Activo == "SI"
                                                          ).ToList();
                            }   
                 
             if (EmpleadosJ.Count > 0)
                                {
                                    Items = db.Permisos.Where(e =>
                                                            DbFunctions.TruncateTime(e.FechaPermiso) >= Fecha1
                                                            &&
                                                            DbFunctions.TruncateTime(e.FechaFinPermiso) <= Fecha2
                                                            &&
                                                            e.Jornada.Contains(Jornada)
                                                            &&
                                                            e.MotivoId == MotivoPermisoId
                                                            &&
                                                            SqlFunctions.StringConvert((decimal)e.Empleado.Id).Contains(Empleado)
                                                            &&
                                                            SqlFunctions.StringConvert((decimal)e.EstadoId).Contains(Estado)
                                                            &&
                                                            e.Empresa.Contains(Empresa)
                                                            &&
                                                            e.Empleado.NroEmpleado.Contains(IdEmpleado)
                                                            &&
                                                            e.Empleado.Jefe.Contains(NroUsuario.NroEmpleado)
                                                            &&
                                                            e.Empleado.NroEmpleado != NroUsuario.NroEmpleado
                                                            &&
                                                             e.Empleado.Activo == "SI"
                                                            ).ToList();
                                }
                        }

                    if (string.IsNullOrEmpty(MotivoPermiso) && !string.IsNullOrEmpty(Estado))//Consulta con datos en Estado
                        {
                                Estadoid = Convert.ToInt32(Estado);
                                if (EmpleadosL.Count > 0)
                                {
                                    Items = db.Permisos.Where(e =>
                                                              DbFunctions.TruncateTime(e.FechaPermiso) >= Fecha1
                                                              &&
                                                              DbFunctions.TruncateTime(e.FechaFinPermiso) <= Fecha2
                                                              &&
                                                              e.Jornada.Contains(Jornada)
                                                              &&
                                                              SqlFunctions.StringConvert((decimal)e.Empleado.Id).Contains(Empleado)
                                                              &&
                                                              e.EstadoId == Estadoid
                                                              &&
                                                              e.Empresa.Contains(Empresa)
                                                              &&
                                                              e.Empleado.NroEmpleado.Contains(IdEmpleado)
                                                              &&
                                                              e.Empleado.Lider.Contains(NroUsuario.NroEmpleado)
                                                              &&
                                                              e.Empleado.NroEmpleado != NroUsuario.NroEmpleado
                                                              &&
                                                              e.Empleado.Activo == "SI"
                                                              ).ToList();
                                }

                        if (EmpleadosJ.Count > 0)
                                {
                                    Items = db.Permisos.Where(e =>
                                                            DbFunctions.TruncateTime(e.FechaPermiso) >= Fecha1
                                                            &&
                                                            DbFunctions.TruncateTime(e.FechaFinPermiso) <= Fecha2
                                                            &&
                                                            e.Jornada.Contains(Jornada)
                                                            &&
                                                            SqlFunctions.StringConvert((decimal)e.MotivoId).Contains(MotivoPermiso)
                                                            &&
                                                            SqlFunctions.StringConvert((decimal)e.Empleado.Id).Contains(Empleado)
                                                            &&
                                                            e.EstadoId == Estadoid
                                                            &&
                                                            e.Empresa.Contains(Empresa)
                                                            &&
                                                            e.Empleado.NroEmpleado.Contains(IdEmpleado)
                                                            &&
                                                            e.Empleado.Jefe.Contains(NroUsuario.NroEmpleado)
                                                            &&
                                                            e.Empleado.NroEmpleado != NroUsuario.NroEmpleado
                                                            &&
                                                             e.Empleado.Activo == "SI"
                                                            ).ToList();
                                }
                         }
          
            if (!string.IsNullOrEmpty(MotivoPermiso) && !string.IsNullOrEmpty(Estado))//Consulta con datos en Motivo permiso y Estado
                        {
                            Estadoid = Convert.ToInt32(Estado);
                            MotivoPermisoId = Convert.ToInt32(MotivoPermiso);
                            if (EmpleadosL.Count > 0)
                            {
                                Items = db.Permisos.Where(e =>
                                                          DbFunctions.TruncateTime(e.FechaPermiso) >= Fecha1
                                                          &&
                                                          DbFunctions.TruncateTime(e.FechaFinPermiso) <= Fecha2
                                                          &&
                                                          e.Jornada.Contains(Jornada)
                                                          &&
                                                          e.MotivoId == MotivoPermisoId
                                                          &&
                                                          SqlFunctions.StringConvert((decimal)e.Empleado.Id).Contains(Empleado)
                                                          &&
                                                          e.EstadoId == Estadoid
                                                          &&
                                                          e.Empresa.Contains(Empresa)
                                                          &&
                                                          e.Empleado.NroEmpleado.Contains(IdEmpleado)
                                                          &&
                                                          e.Empleado.Lider.Contains(NroUsuario.NroEmpleado)
                                                          &&
                                                          e.Empleado.NroEmpleado != NroUsuario.NroEmpleado
                                                          &&
                                                          e.Empleado.Activo == "SI"
                                                          ).ToList();
                            }

                            if (EmpleadosJ.Count > 0)
                            {
                                Items = db.Permisos.Where(e =>
                                                        DbFunctions.TruncateTime(e.FechaPermiso) >= Fecha1
                                                        &&
                                                        DbFunctions.TruncateTime(e.FechaFinPermiso) <= Fecha2
                                                        &&
                                                        e.Jornada.Contains(Jornada)
                                                        &&
                                                        e.MotivoId == MotivoPermisoId
                                                        &&
                                                        SqlFunctions.StringConvert((decimal)e.Empleado.Id).Contains(Empleado)
                                                        &&
                                                        e.EstadoId == Estadoid
                                                        &&
                                                        e.Empresa.Contains(Empresa)
                                                        &&
                                                        e.Empleado.NroEmpleado.Contains(IdEmpleado)
                                                        &&
                                                        e.Empleado.Jefe.Contains(NroUsuario.NroEmpleado)
                                                        &&
                                                        e.Empleado.NroEmpleado != NroUsuario.NroEmpleado
                                                        &&
                                                         e.Empleado.Activo == "SI"
                                                        ).ToList();
                            }
                        }

                }





                foreach (Permiso Item in Items)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId && e.Empresa == EmpresaSesion);
                    Item.EstadoPermiso = db.EstadosPermiso.FirstOrDefault(s => s.Id == Item.EstadoId);
                    Item.PersonalActivo = db.PersonalActivo.FirstOrDefault(j => j.CodigoEmpleado == Item.Empleado.NroEmpleado && j.Empresa == EmpresaSesion);
                    var catd = (Item.FechaFinPermiso.Date - Item.FechaPermiso.Date).Days;
                    Item.cantdias = string.Format("{0}", catd);


                }


            }
            return View(Items);


        }
        //*

        public ActionResult CreateXJefe()
        {
            string EmpleadoId = String.Format("{0}", Session["Empleado"]);
            Empleado datos = new Empleado();
            int Id = 0;
            Int32.TryParse(EmpleadoId, out Id);

            Permiso model = new Permiso();
            using (var db = new AutogestionContext())
            {
                datos = db.Empleados.Find(Id); 
                //model.ListadoEmpleadosJefe = db.Empleados.Where(e => e.Jefe == datos.NroEmpleado).ToList();
                model.ListadoEmpleadosJefe = db.Empleados.Where(e => e.Jefe == "40001088" ).ToList();
            }
            return View(model);
        }

        //
        // POST: /Permisos/CreateXJefe

        [HttpPost]
        public ActionResult CreateXJefe(Permiso model)
        {
            string message = "";
            int IdUsuarioM = 0;
            var Datos2 = new Permiso();
            try
            {
                using (var db = new AutogestionContext())
                {
                    string Nro = string.Format("{0}", model.EmpleadoId);
                    Empleado Codigo = db.Empleados.FirstOrDefault(e => e.NroEmpleado == Nro);
                    model.EmpleadoId = Codigo.Id;
                    model.Empresa = Codigo.Empresa;
                    string modifica = String.Format("{0}", Session["Empleado"]);
                    Int32.TryParse(modifica, out IdUsuarioM);

                    List<Permiso> Datos = new List<Permiso>();
                    Datos = db.Permisos.OrderByDescending(e => e.Fecha).ToList();
                    Datos2 = Datos.FirstOrDefault(e => e.EmpleadoId == model.EmpleadoId);
                }
            }
            catch
            {

            }

            Session["message"] = message;
            return View();
        }

        //
        // POST: /Permisos/ConfirmacionPer

        public ActionResult ConfirmacionPer(string Id)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ConfirmacionPer"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }


            var EmpreUsu = Convert.ToInt32(Session["Empresa"]);
            var db = new AutogestionContext();
            int id = -1;
            string opcion = "Confirmar";
            Int32.TryParse(Id, out id);
            string EmpleadoId = String.Format("{0}", Session["Empleado"]);
            var model = _repo.ObtenerTodos2(opcion, EmpleadoId, EmpreUsu);

            if (id > 0)
            {
                var permiso = model.FirstOrDefault(e => e.Id == id);
                ViewBag.Permiso = permiso;
                ViewData["Remunerado"] = permiso.Remunerado;
                var Estado = db.EstadosPermiso.FirstOrDefault(e => e.Id == permiso.EstadoId);
                ViewBag.estado = Estado;
            }

            if (ViewBag.Permiso == null)
            {

                ViewBag.Permiso = new Models.Permiso();
                ViewBag.Permiso.Empleado = new Empleado();
                ViewBag.Permiso.MotivoPermiso = new MotivoPermiso();
            }

            return View(model);
        }


        public JsonResult Obtener_Per(int id)
        {
            using (var db = new AutogestionContext())
            {
                Permiso permiso = db.Permisos.Where(a => a.Id == id).Single();
                if (permiso == null)
                {
                    return Json(JsonRequestBehavior.AllowGet);
                }
                ViewBag.permiso = permiso;
                return Json(permiso, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // POST: /Permisos/ConfirmacionPer1

        //*********************** Modal Detalle Empleado *******************
        public JsonResult Detalles(int id)
        {
            using (var db = new AutogestionContext())
            {
                Empleado empleado = db.Empleados.Find(id);
                if (empleado == null)
                {
                    return Json(JsonRequestBehavior.AllowGet);
                }

                return Json(empleado, JsonRequestBehavior.AllowGet);
            }
        }
        //**********************--------------------************************

        public ActionResult ConfirmacionPerNom(string Id, string Fecha, string FechaPermiso, string FechaFinPermiso)
        {

            List<Permiso> model = new List<Permiso>();
            var db = new AutogestionContext();
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ConfirmacionPerNom"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var EmpreUsu = Convert.ToInt32(Session["Empresa"]);
            int id = -1;
            string opcion = "Confirmar_Nom";
            Int32.TryParse(Id, out id);
            string EmpleadoId = String.Format("{0}", Session["Empleado"]);
            if (Fecha != null || FechaPermiso != null && FechaFinPermiso != null)
            {
                if (Fecha != null || FechaPermiso != "" && FechaFinPermiso != "")
                {
                    model = _repo.ObtenerTodos3(EmpleadoId, Fecha, FechaPermiso, FechaFinPermiso);
                }
            }
            

            //if (id > 0)
            //{
            //    var permiso = model.FirstOrDefault(e => e.Id == id);
            //    if (permiso != null)
            //    {
            //        ViewBag.Permiso = permiso;
            //        var Estado = db.EstadosPermiso.FirstOrDefault(e => e.Id == permiso.EstadoId);
            //        ViewBag.estado = Estado;
            //        ViewBag.Modal = true;
            //    }
            //    else
            //    {
            //        ViewBag.Permiso = permiso;
            //    }
            //}

            //if (ViewBag.Permiso == null)
            //{
            //    ViewBag.Permiso = new Models.Permiso();
            //    ViewBag.Permiso.Empleado = new Empleado();
            //    ViewBag.Permiso.MotivoPermiso = new MotivoPermiso();
            //}
            Session["filtrosP"] = String.Format("{0},{1},{2}", Fecha, FechaPermiso, FechaFinPermiso);

            return View(model);
        }

        public ActionResult ConfirmacionPerNom2(string Id)
        {
            var EmpreUsu = Convert.ToInt32(Session["Empresa"]);
            int id = -1;
            Int32.TryParse(Id, out id);
            string EmpleadoId = String.Format("{0}", Session["Empleado"]);

            Permiso model = new Permiso();

            
                using (var db = new AutogestionContext())
            {

                model = db.Permisos.FirstOrDefault(e => e.Id == id);
                model.Empleado = db.Empleados.FirstOrDefault(e => e.Id == model.EmpleadoId);
                model.EstadoPermiso = db.EstadosPermiso.FirstOrDefault(e => e.Id == model.EstadoId);
                model.MotivoPermiso = db.MotivosPermiso.FirstOrDefault(e => e.Id == model.MotivoId);
                ViewBag.Permiso = model;
       

            return PartialView(model);
             }

        }

        //
        // POST: /Permisos/ConfirmacionPer1

        public ActionResult ConfirmacionPer1(int tipo, string Id1, string Observacion, string Id, string Empleado, string Estado, string Jornada, string Remunerado)
        {
            string message = null;
            if (Observacion == "" && Estado == "3" || Observacion == "" && Estado == "10")
            {

                message = "Antes de Guardar con este estado, debe llenar la observación.";

            }
            else
            {

                if (Jornada == " ")
                {
                    message = "Antes de Guardar debe seleccionar la jornada.";
                }
                else
                {

                    if (Remunerado == "")
                    {
                        message = "Antes de Guardar debe seleccionar si es remunerado.";
                    }
                    else
                    {


                        int IdUsuarioM = 0;

                        try
                        {
                            if (Estado != "")
                            {
                                int id = 0;
                                int empleadoid = 0;
                                if (id > 0) { Int32.TryParse(Id, out id); } else { Int32.TryParse(Id1, out id); }

                                string modifica = String.Format("{0}", Session["Empleado"]);
                                Int32.TryParse(modifica, out IdUsuarioM);
                                Int32.TryParse(Empleado, out empleadoid);
                                string opcion = "2";
                                var usumod = Convert.ToInt32(Session["Empleado"]);

                                using (var db = new AutogestionContext())
                                {
                                    if (Id != "0")
                                    {

                                        Permiso permisos = db.Permisos.Find(id);
                                        _repo.Modifica(Observacion, Id, Empleado, Estado, IdUsuarioM, Remunerado, opcion, Jornada, tipo, permisos.MotivoId, usumod);
                                        Empleado empleado = db.Empleados.FirstOrDefault(s => s.Id == empleadoid);
                                        //if (Estado == "4" && Jornada != "SI" || Estado == "10" || Estado == "3" || tipo == 1)
                                        //{
                                        if (Estado == "3" || Estado == "10")
                                        {
                                            if (enviar_correo_rechazado(empleado.Documento, empleado.Nombres, empleado.Correo, empleado.Id, Estado, Observacion , permisos) == false)
                                            {
                                                message = "Error al momento de enviar correo de notificación.";

                                            }
                                        }
                                        else if (Estado == "4")
                                        {
                                            if (enviar_correo(empleado.Documento, empleado.Nombres, empleado.Correo) == false)
                                            {
                                                message = "Error al momento de enviar correo de notificación.";

                                            }
                                        }
                                        //}


                                    }
                                    else
                                    {
                                        message = "Antes de Guardar, debe seleccionar un registro de la tabla.";
                                    }

                                }

                            }
                            else
                            {
                                message = "Antes de Guardar, debe llenar el campo Estado.";
                            }



                        }

                        catch (SystemException ex)
                        {
                            message = String.Format("Se genero un error. {0}", ex.Message);
                            Session["message"] = message;

                            //Se redirecciona el resultado dependiendo desde donde venga
                            if (tipo == 1)
                            {
                                string filtros = string.Format("{0}", Session["filtrosP"]);

                                string[] datos = filtros.Split(',');
                                Session.Remove("filtrosP");
                                return RedirectToAction("ConfirmacionPerNom", "Permisos", new { Fecha = datos[0], FechaPermiso = datos[1], FechaFinPermiso = datos[2] });// El 1 a la Pagina de nomina
                            }
                            else if (tipo == 2)
                            {
                                return RedirectToAction("ConfirmacionPer");   // El 2 a la Pagina de Gstion Humana
                            }
                            else
                            {
                                return RedirectToAction("Informe");   // El 3 a la Pagina de Informe
                            }
                        }//Fin Try

                      }
                     }
                    }
                    Session["message"] = message;


                    //Se redirecciona el resultado dependiendo desde donde venga
                    if (tipo == 1)
                    {
                        string filtros = string.Format("{0}", Session["filtrosP"]);

                        string[] datos = filtros.Split(',');
                        Session.Remove("filtrosP");
                        return RedirectToAction("ConfirmacionPerNom", "Permisos", new { Fecha = datos[0], FechaPermiso = datos[1], FechaFinPermiso = datos[2] });// El 1 a la Pagina de nomina
                        //return RedirectToAction("ConfirmacionPerNom");// El 1 a la Pagina de nomina
                    }
                    else if (tipo == 2)
                    {
                        return RedirectToAction("ConfirmacionPer");   // El 2 a la Pagina de Gstion Humana
                    }
                    else
                    {
                        return RedirectToAction("Informe");   // El 3 a la Pagina de Informe
                    }

                
        }

        //
        // POST: /Permisos/AprobacionPer

        public ActionResult AprobacionPer(string Id, string AreaDescripcion, string Empresa)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("AprobacionSuperiorPermisos"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            int id = -1;

            
            var EmpreUsu = Convert.ToInt32(Session["Empresa"]);
           string EmpresaSesion = Convert.ToString(Session["Empresa"]);
            string opcion = "Aprobar";
            Int32.TryParse(Id, out id);
            var model = _repo.ObtenerTodos2(opcion, Convert.ToString(Session["Empleado"]), EmpreUsu);

            if (id > 0)
            {
                ViewBag.Permiso = model.FirstOrDefault(e => e.Id == id);
            }

            if (ViewBag.Permiso == null)
            {
                ViewBag.Permiso = new Models.Permiso();
                ViewBag.Permiso.Empleado = new Empleado();
                ViewBag.Permiso.MotivoPermiso = new MotivoPermiso();
            }

            using (var db = new AutogestionContext())
            {
                var areaDescripcionGroups = db.Empleados
                    .Where(f => f.Activo != "NO" && f.AreaDescripcion != null && f.AreaDescripcion != "" )
                    .GroupBy(b => b.AreaDescripcion).ToList();

                ViewBag.AreaDescripcion = new List<SelectListItem>();

                foreach (var x in areaDescripcionGroups)
                {
                    Empleado Item = x.FirstOrDefault();
                    
                        ViewBag.AreaDescripcion.Add(new SelectListItem() { Value = Item.AreaDescripcion, Text = Item.AreaDescripcion });
                    
                }
                var Sociedades = db.Empleados
                    .Where(f => f.Activo != "NO" && f.Empresa != null && f.Empresa == EmpresaSesion)
                    .GroupBy(b => b.Empresa).ToList();

                ViewBag.Empresa = db.Sociedad.Where(x => x.Codigo == EmpresaSesion).ToList();

                //ViewBag.Empresa = new List<SelectListItem>();

                //foreach (var x in Sociedades)
                //{
                //    Empleado Item = x.FirstOrDefault();
                //    ViewBag.Empresa.Add(new SelectListItem() { Value = Item.Empresa, Text = Item.Empresa });
                //}
            }


            var Empleadolog = Session["Empleado"];
            var EstadoId = 0;
            

            using (var db = new AutogestionContext())
            {
                
                if (AreaDescripcion != null && AreaDescripcion != "" || (Empresa != null && Empresa != ""))
                {
                    List<Permiso> nuevaLista = new List<Permiso>();
                    foreach (var item in model.Reverse<Permiso>())
                    {
                        bool cumpleCriterio = true;

                        if (item.Empleado.AreaDescripcion != AreaDescripcion && !string.IsNullOrEmpty(AreaDescripcion))
                        {
                            cumpleCriterio = false;
                        }

                        if (item.Empleado.Empresa != Empresa && !string.IsNullOrEmpty(Empresa))
                        {
                            cumpleCriterio = false;
                        }

                        if (cumpleCriterio)
                        {
                            nuevaLista.Add(item);
                        }
                    }
                    model = nuevaLista;
                }
                
            }
            ViewBag.SelectedArea = AreaDescripcion;
            ViewBag.Sociedad = Empresa;

            return View(model);
        }


        public ActionResult AprobacionPer1(string Id, string Empleado, string Estado, string Observacion, string Remunerado, string Jornada, int tipo, string AreaDescripcion, string Empresa)
        {
            int IdUsuarioM = 0;
            string message = null;

            try
            {
                var db = new AutogestionContext();
                int id = 0;
                Int32.TryParse(Id, out id);//-------- Id Permiso
                Permiso permisos = db.Permisos.Find(id);
                string modifica = String.Format("{0}", Session["Empleado"]);
                Int32.TryParse(modifica, out IdUsuarioM);
                string opcion = "1";
                int IdEmpleado = 0;
                Int32.TryParse(Empleado, out IdEmpleado);
                Empleado empleado = db.Empleados.FirstOrDefault(s => s.Id == IdEmpleado);
                var usumod = Convert.ToInt32(Session["Empleado"]);

                if (Id != "" && Id!="0")
                {
                    // TODO: Add update logic here
                    if (Estado == "3")
                    {
                        if (enviar_correo_rechazado(empleado.Documento, empleado.Nombres, empleado.Correo, empleado.Id, Estado, Observacion, permisos) == false)
                        {
                            message = "Error al momento de enviar correo de notificación.";

                        }
                    }
                    
                        // TODO: Add update logic here
                    _repo.Modifica(Observacion, Id, Empleado, Estado, IdUsuarioM, Remunerado, opcion, Jornada, tipo, permisos.MotivoId, usumod);
                    

                }
                else
                {
                    message = "Antes de Guardar, debe seleccionar un registro de la tabla.";
                }

                Session["message"] = message;
                return RedirectToAction("AprobacionPer", new { AreaDescripcion = AreaDescripcion, Empresa = Empresa });
            }

            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
                Session["message"] = message;
                return View();
            }
        }


        public ActionResult MostrarImagen(string archivo)
        {
            try
            {
                ViewBag.imagen = string.Format("~/AnexosPermisos/{0}", archivo);
                // TODO: Add update logic here
                //_repo.Modificar(Id, Empleado, Estado);
                return View("MostrarImagen");
            }
            catch
            {
                return View();
            }
        }

        //****************************** Descarga de Pdf Permiso *******************************
        public FileResult Download(string archivo)
        {

            //byte[] fileBytes = System.IO.File.ReadAllBytes(@"C:\Users\mayra\Desktop\Adm_AutoGestion\Adm_AutoGestion\AnexosVacaciones\" + archivo);
            var fileBytes = Server.MapPath(@"..\AnexosPermisos\" + archivo);
            string fileName = archivo;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }


        public bool enviar_correo(string cedula, string nombres, string email)
        {
            string textocorreo = "";
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
            using (var db = new AutogestionContext())
            {
                var configuracion = db.Configuraciones.First(x => x.Parametro == "TEXTOAPROBPERMISO");

                textocorreo = configuracion.Valor;

                textocorreo = textocorreo.Replace("$USUARIO", nombres);
                textocorreo = textocorreo.Replace("$DOCUMENTO", cedula);



                try
                {
                    System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                    correo.From = new System.Net.Mail.MailAddress(txtde);

                    var CorreoPer = db.Empleados.First(c => c.Documento == cedula);

                    correo.To.Add(CorreoPer.Correo); //Correo Laboral del Empleado   
                    //correo.To.Add(CorreoPer.CorreoPersonal); //Correo Personal del Empleado  
                    //correo.CC.Add(email);//Copia al Correo del Jefe
                    correo.To.Add(email);




                    correo.Subject = "Notificación de Aprobación de Permiso";
                    correo.Body = textocorreo;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;



                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    smtp.Send(correo);
                    confirmacion = true;
                    return confirmacion;
                }

                catch (Exception ex)
                {

                    confirmacion = false;
                    return confirmacion;
                }
            }
        }


        public bool enviar_correo_rechazado(string cedula, string nombres, string email, int Id, string Estado, string Observacion, Permiso permisos)
        {
            var db = new AutogestionContext();
            string textocorreo = "";
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);

            var configuracion = db.Configuraciones.First(x => x.Parametro == "TEXTORECHAZOPERMISO");
            //var permiso_obs = db.HistorialPermisos.FirstOrDefault(x => x.PermisoId.ToString() == Id1);
            textocorreo = configuracion.Valor;

            textocorreo = textocorreo.Replace("$USUARIO", nombres);
            textocorreo = textocorreo.Replace("$DOCUMENTO", cedula);

            textocorreo = textocorreo.Replace("$FECHADESDE", permisos.FechaPermiso.ToString("dd-MM-yyyy"));
            textocorreo = textocorreo.Replace("$FECHAHASTA", permisos.FechaFinPermiso.ToString("dd-MM-yyyy"));
            textocorreo = textocorreo.Replace("$HORADDESDE", permisos.HoraInicioPermiso.ToString());
            textocorreo = textocorreo.Replace("$HORAHASTA", permisos.HoraFinPermiso.ToString());
            if (Estado == "3")
            {
                textocorreo = textocorreo.Replace("$ESTADO", "RECHAZADO");
                textocorreo = textocorreo.Replace("$OBSERVACION", Observacion);
            }
            else
            {
                textocorreo = textocorreo.Replace("$ESTADO", "ANULADO");
                textocorreo = textocorreo.Replace("$OBSERVACION", Observacion);
            }


            var empleado = db.Empleados.FirstOrDefault(x => x.Id == Id);
            var correojefe = "";
            var correojefeinmediato = "";
            try
            {
                var Jefe = db.Empleados.FirstOrDefault(x => x.NroEmpleado == empleado.Jefe);



                if (Object.ReferenceEquals(null, Jefe))
                {
                    correojefeinmediato = "";

                }
                else
                {
                    correojefeinmediato = Jefe.Correo.ToString();
                }
            }


            catch (Exception ex)
            {
                correojefe = "";
            }
            try
            {

                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

                var CorreoPer = db.Empleados.First(c => c.Documento == cedula);

                correo.To.Add(CorreoPer.Correo); //Correo Laboral del Empleado   
                //correo.To.Add(CorreoPer.CorreoPersonal); //Correo Personal del Empleado  
                correo.CC.Add(email);//Copia al Correo del Jefe
                if (Estado == "3") { correo.Subject = "Notificación de Rechazo del Permiso"; } else { correo.Subject = "Notificación de Anulación del Permiso"; }


                correo.Body = textocorreo;
                correo.Priority = System.Net.Mail.MailPriority.Normal;
                correo.IsBodyHtml = true;



                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                smtp.EnableSsl = true;
                smtp.Send(correo);
                confirmacion = true;
                return confirmacion;
            }
            catch (Exception ex)
            {

                confirmacion = false;
                return confirmacion;
            }
        }
        //**********************************---------------------------**************************
        public ActionResult Calendario()
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("CalendarioPermisos"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        public string Datoscalendario1()
        {
            List<CalendarioPermisos> Items = new List<CalendarioPermisos>();
            string Result = "";
            string datos = "";
            string Fecha1 = "";
            string Fecha2 = "";

            using (var db = new AutogestionContext())
            {
                var empleadoid = Session["Empleado"];
                string EmpresaSesion = Convert.ToString(Session["Empresa"]);
                var empleado = db.Empleados.Find(empleadoid);


                string consulta = "select e.Nombres, Permisoes.FechaPermiso, Permisoes.FechaFinPermiso, es.Nombre as Estado from Permisoes inner join Empleadoes as e on e.Id = Permisoes.EmpleadoId inner join EstadoVacaciones as es on es.Id = Permisoes.EstadoId where (e.Jefe = " + empleado.NroEmpleado + ") and (Permisoes.EstadoId != 3 and Permisoes.EstadoId != 5 and Permisoes.EstadoId != 10  ) and (e.Empresa = " + EmpresaSesion + ")";
                var resultados = db.Database.SqlQuery<CalendarioPermisos>(consulta).ToList();

                foreach (CalendarioPermisos fila in resultados)
                {
                    fila.FechaFinPermiso = fila.FechaFinPermiso.AddDays(1);
                    Fecha1 = fila.FechaPermiso.ToString("yyyy-MM-dd");
                    Fecha2 = fila.FechaFinPermiso.ToString("yyyy-MM-dd");


                    Result += String.Format(fila.Nombres + ";" + Fecha1 + ";" + Fecha2 + ";" + fila.Estado + ",");
                }


                Result = Result.Substring(0, Result.Length - 1);
            }

            return Result;
        }



    }
    }

