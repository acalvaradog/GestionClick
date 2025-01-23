using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using Microsoft.Ajax.Utilities;
using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
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
                return PartialView(Items);
            }
     
        }

        public ActionResult Resultado1(string Id)
        {
            var p = 0;
            using (var db = new AutogestionContext())
            {
                Int32.TryParse(Id, out p);

                ViewBag.Proceso = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == p);
            }
            return View();
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
                return PartialView(Items);
            }

        }



        public ActionResult Fundamentos(string Id)
        {
            var p = 0;
            using (var db = new AutogestionContext())
            {
                Int32.TryParse(Id, out p);

                ViewBag.Proceso = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == p);
            }
            return PartialView();
        }
      
        public ActionResult VerJefe1(string Id) 
        {
            string message = null;
            //int IdUsuarioM = 0;
            InMemoryDestinationConfiguration DestinacionConfiguracion = new InMemoryDestinationConfiguration();


            try
            {
                int id = 0;
                Int32.TryParse(Id, out id);
                //string modifica = String.Format("{0}", Session["Empleado"]);
                //Int32.TryParse(modifica, out IdUsuarioM);
                using (var db = new AutogestionContext())
                {

                    if (Id != "0")
                    {

                        ProcesoDisciplinario vacaciones = db.ProcesoDisciplinario.Find(id);
                        //_repo.Modificar();

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
                var DatosJefe = new Empleado();
                List<Empleado> Jefes = new List<Empleado>();
                //var emp = db.Empleados.Select(e => new { e.Id, e.Documento, e.Cargo, e.Jefe, e.Nombres, e.Activo, nomcodigo = e.Id + "-" + e.Cargo + "-" + e.Area + "-" + e.Jefe , e.FechaFin}).Where(f => f.Activo != "NO" || f.FechaFin >= DateTime.Now).ToList();          

                var emp3 = (from emp in db.Empleados 
                            join e in db.Empleados on emp.Jefe equals e.NroEmpleado into personal
                            from ps in personal.DefaultIfEmpty()
                            where emp.Activo != "NO" || emp.FechaFin >= DateTime.Now
                            select new { emp.Id, emp.Documento, emp.Cargo, emp.Jefe, emp.Nombres, emp.Activo, nomcodigo = emp.Id + "-" + emp.Cargo + "-" + emp.Area + "-" + ps.Nombres, emp.FechaFin }
                ).ToArray();

                var sociedad = db.Sociedad.Select(x => new { x.Codigo, x.Descripcion }).ToArray();



             ViewBag.Empleado = emp3;
             ViewBag.Sociedad = sociedad;
         

            }

            return View();
        }


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


        public FileResult Download3(string archivo)
        {

            var fileBytes = Server.MapPath(@"..\AnexosProcesosDisciplinarios\Notas\" + archivo);
            string fileName = archivo;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }


        public ActionResult DetalleProcesoDisciplinario(string EmpleadoCP, string EmpleadoI, string FechaProcesoI, string FechaProcesoF,string Estado,string NmrProceso,string Prioridad, string Sancion)
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
                int EmpleadoCPINT=0;
                if (EmpleadoCP!="") 
                {
                    EmpleadoCPINT = Convert.ToInt32(EmpleadoCP);
                }
               
                List<SelectListItem> lst = new List<SelectListItem>();
                Int32 IdProceso = new Int32();
                if(NmrProceso!="")
                {
                    IdProceso = Convert.ToInt32(NmrProceso);
                }
                if (!DateTime.TryParse(FechaProcesoI, out Fecha1))
                    {
                        Fecha1 = new DateTime();
                    }

                if (!DateTime.TryParse(FechaProcesoF, out Fecha2))
                    {
                        Fecha2 = DateTime.Now;
                    }
                if (EmpleadoCP!="")
                {
                    if (EmpleadoI == "")
                    {
                        if (Estado == "Todos")
                        {
                            Proceso = db.ProcesoDisciplinario.Where(e => DbFunctions.TruncateTime(e.FechaRegistro) >= Fecha1 &&
                                              DbFunctions.TruncateTime(e.FechaRegistro) <= Fecha2
                                              &&
                                              e.EmpleadoRegistraId == EmpleadoCPINT
                                               && e.NivelPrioridad.Contains(Prioridad) && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrProceso)
                                              
                                              ).ToList();
                        }
                        else
                        {
                            Proceso = db.ProcesoDisciplinario.Where(e => DbFunctions.TruncateTime(e.FechaRegistro) >= Fecha1 &&
                                 e.Estado == Estado
                                         &&
                                         DbFunctions.TruncateTime(e.FechaRegistro) <= Fecha2
                                         && e.EmpleadoRegistraId == EmpleadoCPINT
                                          
                                          && e.NivelPrioridad.Contains(Prioridad) && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrProceso)
                                         ).ToList();

                        }
                        foreach (ProcesoDisciplinario Item in Proceso.Reverse<ProcesoDisciplinario>())
                        {
                            var x = Item.Id;
                            string Id = "" + x;
                            var Implicados = _repo.ObtenerTodos3(Id);
                            List<Empleado> z = new List<Empleado>();
                            foreach (PDTrabajador Item2 in Implicados) { z.Add(Item2.Empleado2); }
                            Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
                            Item.Implicados = z;
                            if (Sancion != "")
                            {
                                if (Item.Sanciones != Sancion)
                                {
                                    Proceso.Remove(Item);
                                }

                            }
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
                                        e.EmpleadoRegistraId == EmpleadoCPINT && e.NivelPrioridad.Contains(Prioridad) && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrProceso)).ToList();
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
                                        e.EmpleadoRegistraId == EmpleadoCPINT && e.NivelPrioridad.Contains(Prioridad)  && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrProceso)).ToList();
                        }
                        foreach (ProcesoDisciplinario Item in Proceso.Reverse<ProcesoDisciplinario>())
                        {
                            var x = Item.Id;
                            string Id = "" + x;
                            var Implicados = _repo.ObtenerTodos3(Id);
                            List<Empleado> z = new List<Empleado>();
                                    foreach (PDTrabajador Item2 in Implicados)
                                    {
                                        z.Add(Item2.Empleado2);
                                    }                   
                            Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
                            Item.Implicados = z;
                            if (Sancion != "")
                            {
                                if (Item.Sanciones != Sancion)
                                {
                                    Proceso.Remove(Item);
                                }

                            }
                        }
                     
                    }
                }
                else if(EmpleadoCP=="")
                {
                    if (EmpleadoI == "")
                    {
                        if (Estado == "Todos")
                        {
                            Proceso = db.ProcesoDisciplinario.Where(e => DbFunctions.TruncateTime(e.FechaRegistro) >= Fecha1 &&
                                              DbFunctions.TruncateTime(e.FechaRegistro) <= Fecha2                                                                                     
                                               && e.NivelPrioridad.Contains(Prioridad) && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrProceso)
                                               
                                              ).ToList();
                        }
                        else
                        {
                            Proceso = db.ProcesoDisciplinario.Where(e => DbFunctions.TruncateTime(e.FechaRegistro) >= Fecha1 
                               &&  DbFunctions.TruncateTime(e.FechaRegistro) <= Fecha2
                               && e.Estado == Estado                                                                               
                                           
                                          && e.NivelPrioridad.Contains(Prioridad) && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrProceso)
                                         ).ToList();

                        }
                   
                        foreach (ProcesoDisciplinario Item in Proceso.Reverse<ProcesoDisciplinario>())
                        {
                         
                            var x = Item.Id;
                            string Id = "" + x;
                            var Implicados = _repo.ObtenerTodos3(Id);
                            List<Empleado> z = new List<Empleado>();
                            foreach (PDTrabajador Item2 in Implicados) { z.Add(Item2.Empleado2); }
                            Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
                            Item.Implicados = z;
                            if (Sancion != "")
                            {
                                if (Item.Sanciones != Sancion)
                                {
                                    Proceso.Remove(Item);
                                }

                            }
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
                                                                    && e.NivelPrioridad.Contains(Prioridad) && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrProceso)).ToList();
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
                                                                   && e.Estado == Estado  && e.NivelPrioridad.Contains(Prioridad) && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrProceso)).ToList();
                        }
                        foreach (ProcesoDisciplinario Item in Proceso.Reverse<ProcesoDisciplinario>())
                        {
                            var x = Item.Id;
                            string Id = "" + x;
                            var Implicados = _repo.ObtenerTodos3(Id);
                            List<Empleado> z = new List<Empleado>();

                                    foreach (PDTrabajador Item2 in Implicados)
                                    {
                                       z.Add(Item2.Empleado2);
                                    }

                            Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
                            Item.Implicados = z;
                            if (Sancion != "")
                            {
                                if (Item.Sanciones != Sancion)
                                {
                                    Proceso.Remove(Item);
                                }

                            }

                        }
                    }
                }
               

                }
            return View(Proceso);
            
        }


        public ActionResult DetalleProcesoDisciplinario1(string EmpleadoCP, string EmpleadoI, string FechaProcesoI, string FechaProcesoF, string Estado, string NmrProceso, string Prioridad, string Sancion, string FechaSuspencion)
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

            int EmpleadoCPINT = Convert.ToInt32(EmpleadoCP);
            List<ProcesoDisciplinario> Proceso = new List<ProcesoDisciplinario>();
            List<PDTrabajador> implicados = new List<PDTrabajador>();

            using (var db = new AutogestionContext())
            {
                int Usuario=0;
                string usuariolog =String.Format("{0}", Session["Empleado"]);
                Int32.TryParse(usuariolog, out Usuario);
                var Jefe = db.Empleados.FirstOrDefault(e => e.Id == Usuario);

                ViewBag.Empleado = db.Empleados.Where(f => f.Activo != "NO" && f.Jefe == Jefe.NroEmpleado).ToList();

                DateTime Fecha1 = DateTime.Now;
                DateTime Fecha2 = DateTime.Now;
                DateTime FechaSus = DateTime.Now;

                List<SelectListItem> lst = new List<SelectListItem>();
                Int32 IdProceso = new Int32();
                if (NmrProceso != "")
                {
                    IdProceso = Convert.ToInt32(NmrProceso);
                }

                if (!DateTime.TryParse(FechaProcesoI, out Fecha1))
                {
                    Fecha1 = new DateTime();
                }

                if (!DateTime.TryParse(FechaProcesoF, out Fecha2))
                {
                    Fecha2 = DateTime.Now;
                }
                
                if (!DateTime.TryParse(FechaSuspencion, out FechaSus))
                {
                    FechaSus = DateTime.Now;
                }
                if (EmpleadoI == "")
                {
                    if (Estado == "Todos")
                    {
                        Proceso = db.ProcesoDisciplinario.Where(e => DbFunctions.TruncateTime(e.FechaRegistro) >= Fecha1 &&
                                          DbFunctions.TruncateTime(e.FechaRegistro) <= Fecha2
                                          &&
                                          e.EmpleadoRegistraId==EmpleadoCPINT
                                          && e.Sanciones.Contains(Sancion)
                                          && e.NivelPrioridad.Contains(Prioridad) && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrProceso) 
                                          ).ToList();
                    }
                  else  if (FechaSuspencion == "" || FechaSuspencion == null)
                    {
                        Proceso = db.ProcesoDisciplinario.Where(e => DbFunctions.TruncateTime(e.FechaRegistro) >= Fecha1
                                          &&
                                          DbFunctions.TruncateTime(e.FechaRegistro) <= Fecha2
                                          &&
                                          e.EmpleadoRegistraId == EmpleadoCPINT
                                          && e.Sanciones.Contains(Sancion) 
                                          && DbFunctions.TruncateTime(e.FechaSuspencion) == null
                                          ).ToList();
                    }
                    else
                    {
                        Proceso = db.ProcesoDisciplinario.Where(e => DbFunctions.TruncateTime(e.FechaRegistro) >= Fecha1 &&
                             e.Estado == Estado
                                     &&
                                     DbFunctions.TruncateTime(e.FechaRegistro) <= Fecha2
                                     &&
                                     e.EmpleadoRegistraId == EmpleadoCPINT
                                     && e.Sanciones.Contains(Sancion)
                                    && e.NivelPrioridad.Contains(Prioridad) && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrProceso)
                                    && (DbFunctions.TruncateTime(e.FechaSuspencion) == FechaSus || DbFunctions.TruncateTime(e.FechaSuspencion) == null)
                                     ).ToList();

                    }
                    foreach (ProcesoDisciplinario Item in Proceso)
                    {
                        var x = Item.Id;
                        string Id = "" + x;
                        var Implicados = _repo.ObtenerTodos3(Id);
                        List<Empleado> z = new List<Empleado>();
                        foreach (PDTrabajador Item2 in Implicados)
                        {

                            z.Add(Item2.Empleado2);


                        }

                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
                        Item.Implicados = z;
                        
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
                                    e.EmpleadoRegistraId == EmpleadoCPINT && e.NivelPrioridad.Contains(Prioridad) && e.Sanciones.Contains(Sancion) && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrProceso)).ToList();
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
                                    e.EmpleadoRegistraId == EmpleadoCPINT && e.NivelPrioridad.Contains(Prioridad) && e.Sanciones.Contains(Sancion) && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrProceso)).ToList();
                    }
                    foreach (ProcesoDisciplinario Item in Proceso)
                    {
                        var x = Item.Id;
                        string Id = "" + x;
                        var Implicados = _repo.ObtenerTodos3(Id);
                        List<Empleado> z = new List<Empleado>();
                        foreach (PDTrabajador Item2 in Implicados)
                        {

                            z.Add(Item2.Empleado2);


                        }

                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
                        Item.Implicados = z;
                    }
                }
            
            /////End If
               
            }
            return View(Proceso);

        }

        //
        // POST: /ProcesoDisciplinario/Create
        //[HttpPost]
        //public ActionResult Create(ProcesoDisciplinario model)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        if (ModelState.IsValid)
        //        {

        //            _repo.Crear(model);
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    catch 
        //    {
                
        //    }
        //    return View();
        //}

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
            var CntAnexos = HttpContext.Request.Params["Cantidadanexos"];
            var CntPruebas = HttpContext.Request.Params["Cantidadpruebas"];
            var CntEmpleados = HttpContext.Request.Params["Cantidadempleados"];
            var FehaHechos = HttpContext.Request.Params["FechaHechos"];
            var NivelPrioridad = HttpContext.Request.Params["NivelPrioridad"];
            var Lugar=HttpContext.Request.Params["Lugar"];
            var Empleadolog = Session["Empleado"];
            var Empresa = HttpContext.Request.Params["Empresa"];
        
            ProcesoDisciplinario DatosProcesoDisciplinario = new ProcesoDisciplinario();

            DatosProcesoDisciplinario.EmpleadoRegistraId = Convert.ToInt16(Empleadolog);
            DatosProcesoDisciplinario.FechaRegistro = DateTime.Parse(FechaRegistro);
            DatosProcesoDisciplinario.Fundamentos = Fundamentos;
            DatosProcesoDisciplinario.FechaHechos = DateTime.Parse(FehaHechos);
            DatosProcesoDisciplinario.NivelPrioridad = NivelPrioridad;
            DatosProcesoDisciplinario.Lugar = Lugar;
            DatosProcesoDisciplinario.Empresa=Empresa;

            
            List<PDAnexos> AdjuntosAnexos = new List<PDAnexos>();
            List<PDPruebas> AdjuntosPruebas = new List<PDPruebas>();
            List<PDTrabajador> CEmpleados = new List<PDTrabajador>();
             int cantidada = Convert.ToInt16(CntAnexos);
             int cantidadp = Convert.ToInt16(CntPruebas);
             int cantidade = Convert.ToInt16(CntEmpleados);


             int pruebas = HttpContext.Request.ContentLength / 1024;
             if (pruebas > 7000)
             {
                 respuesta = "Los archivos cargados superan el tamaño permitido de carga (7 Mb). ";
                 return Json(new { respuesta });

             }

             for (int i = 0; i < cantidade; i++)
             {
                 PDTrabajador empleado = new PDTrabajador();
                 int[] Emps = EmpleadoId.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

                 
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
                 var ext = extension[1].ToString().ToLower();

                 if (ext != "jpg" && ext != "jpeg" && ext != "png" && ext != "doc" && ext != "docx" && ext != "pdf" && ext != "odt" && ext != "ods" && ext != "mp4")
                     {
                         respuesta = "El tipo de archivo ." + ext + " no es permitido.";
                         return Json(new { respuesta });
                     }
                 
                 var size = httpPostedFile.ContentLength / 1024;
                 if (size > 7000)
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
                 var Descripcion = HttpContext.Request.Params["Descripcion" + i];
                 var httpPostedFile = HttpContext.Request.Files[nombreadjunto];

                 if (httpPostedFile != null)
                 {
                     var extension = httpPostedFile.FileName.Split('.');
                     var ext = extension[1].ToString().ToLower();

                     if (ext != "jpg" && ext != "jpeg" && ext != "png" && ext != "doc" && ext != "docx" && ext != "pdf" && ext != "xlsx" && ext != "odt" && ext != "ods" && ext != "mp4")
                     {
                         respuesta = "El tipo de archivo ." + ext + " no es permitido.";
                         return Json(new
                         {
                             respuesta
                         });
                     }

                     var size = httpPostedFile.ContentLength / 1024;
                     if (size > 7000)
                     {
                         respuesta = "El archivo supera el tamaño permitido de carga.";
                         return Json(new
                         {
                             respuesta
                         });

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
                     ArchivoPrueba.Descripcion = Descripcion;
                     AdjuntosPruebas.Add(ArchivoPrueba);
                     db.PDPruebas.Add(ArchivoPrueba);
                 }
                 else 
                 {
                   
                     ArchivoPrueba.IdProcesoDisciplinario = DatosProcesoDisciplinario.Id;
                     ArchivoPrueba.TipoPrueba = PruebaType;
                     ArchivoPrueba.Adjunto = "SinAdjuntar";
                     ArchivoPrueba.Descripcion = Descripcion;
                     AdjuntosPruebas.Add(ArchivoPrueba);
                     db.PDPruebas.Add(ArchivoPrueba);
                 }
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
                 var d=DatosProcesoDisciplinario;
                 //int id = myNewObject.Id; 

                 List<string> funciones = Acceso.Validar(Session["Empleado"]);
                 var url = Url.Action("");
                 if (!Acceso.EsAnonimo && funciones.Contains("ProcesoDisciplinarioInformeJefe"))
                 {
                     url = Url.Action("DetalleProcesoDisciplinario1");
                 }
                 if (!Acceso.EsAnonimo && funciones.Contains("ProcesoDisciplinarioInforme"))
                 {

                     url = Url.Action("DetalleProcesoDisciplinario");
                 }

                 respuesta = String.Format("Se ha guardado de manera exitosa, Proceso Numero: "+d.Id);
                
                 return Json(new
                 {
                    
                     redirectUrl = url,
                     isRedirect = true,
                     respuesta
                 });
             }
             catch (SystemException ex) {
                 respuesta = String.Format("Error al guardar. {0}", ex.Message);
                 return Json(new
                 {
                     respuesta
                 });
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
                var Implicados = _repo.ObtenerTodos3(Id);
                List<PDTrabajador> z = new List<PDTrabajador>();
                foreach (PDTrabajador Item2 in Implicados)
                {
                    Item2.Jefe = db.Empleados.FirstOrDefault(e => e.NroEmpleado == Item2.Empleado2.Jefe);
                    z.Add(Item2);
                }

                Items = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == process);
                Items.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Items.EmpleadoRegistraId);
                Items.PDTrabajador = z;
                return PartialView(Items);
            }

        }

       
        public ActionResult Gestion2(string Justificacion, string Id, string Empleado, string Estado)
        {
            string message = null;
            try
            {
                int id = 0;
                Int32.TryParse(Id, out id);
                using (var db = new AutogestionContext())
                {
                    if(Estado=="Anulado" && (Justificacion==null || Justificacion==""))
                    {
                        message="Si desea anular el proceso, debe dar una justificación sobre la acción";
                        Session["message"] = message;
                        return RedirectToAction("GestionHumana");
                    
                    }

                    if (Id != "0")
                    {
                        _repo.Modificar(Justificacion, Id, Estado);
           
                        if (Estado == "Remitido a Juridica")
                        {
                           string EmpleadoId = String.Format("{0}", Session["Empleado"]);
                           List<string> array = new List<String>();
                           var fecha = Convert.ToString(DateTime.Now);
                           var user = Convert.ToInt32(EmpleadoId);
                           Empleado username=db.Empleados.FirstOrDefault(x=>x.Id==user);
                            List<PDTrabajador> proceso = _repo.ObtenerTodos3(Id);
                            foreach(PDTrabajador Item in proceso)
                            {
                                var x=Item.Empleado2.Nombres;
                                array.Add(x);
                                
                               
                            }
                    
                            notificar_Juridico(Id, username.Nombres,array,fecha);
                        }
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

        public ActionResult Gestion3(string Id)
        {
            var process = 0;
            Int32.TryParse(Id, out process);
            using (var db = new AutogestionContext())
            {
                ProcesoDisciplinario Items = new ProcesoDisciplinario();
                ViewBag.ProcesoDisciplinario = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == process);
                var Implicados = _repo.ObtenerTodos3(Id);
                List<PDTrabajador> z = new List<PDTrabajador>();
                foreach (PDTrabajador Item2 in Implicados)
                {
                    Item2.Jefe = db.Empleados.FirstOrDefault(e => e.NroEmpleado == Item2.Empleado2.Jefe);
                    z.Add(Item2);
                }

                Items = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == process);
                Items.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Items.EmpleadoRegistraId);
                Items.PDTrabajador = z;
                return PartialView(Items);
            }
        }
       
        public ActionResult AnularProceso(string Id)
        {
            var id=0;
            Int32.TryParse(Id, out id);
            ViewBag.Proceso = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == id);
            return PartialView();
        }
       
        public JsonResult Anular()
        {
            var Justificacion = HttpContext.Request.Params["Justificacion"];
            var Estado = HttpContext.Request.Params["Estado"];
            var Id = HttpContext.Request.Params["Id"];
            using (var db = new AutogestionContext())
            {
                _repo.Modificar(Justificacion, Id, Estado);
              
                return Json(new { 
                isRedirect=true,
                respuesta="Proceso Anulado de manera Exitosa"
                });
            }
        }


        public JsonResult Respuesta1()
        {
            var TipoSancion = HttpContext.Request.Params["TipoSancion"];
            var MotivoSancion = HttpContext.Request.Params["MotivoSancion"];
            var FechaDescargo = HttpContext.Request.Params["Fdescargo"];
            var FechaCDescargo = HttpContext.Request.Params["FCitaciondescargo"];
            var DiasSuspension = HttpContext.Request.Params["DiasSuspension"];
            var TipoFalta = HttpContext.Request.Params["Falta"];
            var Estado = HttpContext.Request.Params["Estado"];
            var RespuestaJuridica = HttpContext.Request.Params["RespuestaJ"];
            var Id = HttpContext.Request.Params["Id"];
            var FechaRespuestaJ = DateTime.Now;
            var fd = Convert.ToDateTime(FechaDescargo);
            var fcd = Convert.ToDateTime(FechaCDescargo);
            var fechadeR = Convert.ToString(FechaRespuestaJ);
            var Empresa = "";
            //if (fd < FechaRespuestaJ) 
            //{
            //    return Json(new
            //    {                
            //       isRedirect = false,
            //        respuesta = "La fecha de Descargo no es Valida"
            //    });
            //}
            //if (fcd < FechaRespuestaJ)
            //{
            //    return Json(new
            //    {
            //        isRedirect = false,
            //        respuesta = "La fecha de Citacion a Descargo no es Valida"
            //    });
            //}

            var respuesta = "true";
            string message = null;

            try
            {
                int id = 0;
                Int32.TryParse(Id, out id);
                using (var db = new AutogestionContext())
                {
                    
                    if (Id != "0")
                    {
                        string nombreadjunto = "Adjunto";
                        var httpPostedFile = HttpContext.Request.Files[nombreadjunto];


                        var extension = httpPostedFile.FileName.Split('.');
                        var ext = extension[1].ToString().ToLower();

                        if (ext != "jpg" && ext != "jpeg" && ext != "png" && ext != "doc" && ext != "docx" && ext != "pdf" && ext != "odt" && ext != "ods")
                        {
                            respuesta = "El tipo de archivo ." + ext + " no es permitido.";
                            return Json(new { respuesta });
                        }

                        var size = httpPostedFile.ContentLength / 1024;
                        if (size > 7000)
                        {
                            respuesta = "El archivo supera el tamaño permitido de carga.";
                            return Json(new { respuesta });

                        }


                        // Validate the uploaded image(optional)
                        DateTime date1 = DateTime.Now;
                        var nombrearchivo = date1.ToString("yyyyMMddHHmmssffff").ToString() + "-" + httpPostedFile.FileName;
                        // Get the complete file path
                        var fileSavePath = Path.Combine(HttpContext.Server.MapPath("~/AnexosProcesosDisciplinarios"), nombrearchivo);
                        //ProcesoDisciplinario vacaciones = db.ProcesoDisciplinario.Find(id);
                        // Save the uploaded file to "UploadedFiles" folder
                        httpPostedFile.SaveAs(fileSavePath);
                        _repo.Modificar1(Id, Estado, RespuestaJuridica, TipoFalta, TipoSancion, MotivoSancion, FechaCDescargo, DiasSuspension, FechaDescargo, FechaRespuestaJ, nombrearchivo);
                        if (Estado == "Cerrado")
                        {
                            string EmpleadoId = String.Format("{0}", Session["Empleado"]);

                            var user = Convert.ToInt32(EmpleadoId);
                            Empleado username = db.Empleados.FirstOrDefault(x => x.Id == user);
                            ProcesoDisciplinario Proceso = db.ProcesoDisciplinario.FirstOrDefault(x => x.Id == id);
                            Empresa = Proceso.Empresa;
                            var Empleadoregistra = Convert.ToInt32(Proceso.EmpleadoRegistraId);
                            Empleado user2 = db.Empleados.FirstOrDefault(x => x.Id == Empleadoregistra);
                            notificar_CierreP(Id, username.Nombres, RespuestaJuridica, fechadeR, nombrearchivo,user2.Correo,Empresa);
                        }

                    }
                    else
                    {
                        message = "Antes de Guardar, debe seleccionar un registro de la tabla.";
                    }

                    // TODO: Add update logic here
                    var empleado = Convert.ToInt16(Session["Empleado"]);                
                    var empleado2 = db.Empleados.FirstOrDefault(e => e.Id == empleado);
                     var usuario = empleado2.Nombres;
                    Session["message"] = message;
                   
                    
                    //notificar_RespuestaJ(Id, usuario, RespuestaJuridica, fechadeR);
                    var url = Url.Action("");
                    url = Url.Action("Respuesta");
                    return Json(new {
                        redirectUrl = url,
                        isRedirect = true,
                        respuesta="Guardado Exitoso"
                    });


                }

            }
            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
                Session["message"] = message;
                respuesta = "Error durante el guardado";
                return Json(new { respuesta });
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
                var Implicados = _repo.ObtenerTodos3(Id);
                List<PDTrabajador> z = new List<PDTrabajador>();
                foreach (PDTrabajador Item2 in Implicados)
                {
                    Item2.Jefe = db.Empleados.FirstOrDefault(e => e.NroEmpleado == Item2.Empleado2.Jefe);
                    z.Add(Item2);             
                }

                Items = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == process);
                Items.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Items.EmpleadoRegistraId);
                Items.PDTrabajador = z;
                return PartialView(Items);
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

           

            ViewBag.CntProcesosAc = db.ProcesoDisciplinario.Where(x => x.Estado == "Activo" ).Count();
            ViewBag.CntProcesosC = db.ProcesoDisciplinario.Where(x => x.Estado == "Cerrado" ).Count();
            ViewBag.CntProcesosAn = db.ProcesoDisciplinario.Where(x => x.Estado == "Anulado").Count();
            ViewBag.CntProcesosRJ = db.ProcesoDisciplinario.Where(x => x.Estado == "Remitido a Juridica").Count();

            
            return View();
        }

        public ActionResult DashboardJefe()
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ProcesoDisciplinarioDashboardJefe"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            string empleado = String.Format("{0}", Session["Empleado"]);
            int empl = 0;
            Int32.TryParse(empleado, out empl);
            Empleado datos = new Empleado();


            ViewBag.CntProcesosAc = db.ProcesoDisciplinario.Where(x => x.Estado == "Activo" && x.EmpleadoRegistraId == empl).Count();
            ViewBag.CntProcesosC = db.ProcesoDisciplinario.Where(x => x.Estado == "Cerrado" && x.EmpleadoRegistraId == empl).Count();
            ViewBag.CntProcesosAn = db.ProcesoDisciplinario.Where(x => x.Estado == "Anulado" && x.EmpleadoRegistraId == empl).Count();
            ViewBag.CntProcesosRJ = db.ProcesoDisciplinario.Where(x => x.Estado == "Remitido a Juridica" && x.EmpleadoRegistraId == empl).Count();

            return View();
        }


        public ActionResult NotaDetails(string Id)
        {
            var process = 0;
            Int32.TryParse(Id, out process);
            using (var db = new AutogestionContext())
            {
                List<NotasProcesos> Items = new List<NotasProcesos>();
                //var s = db.NotasProcesos.Where(x => x.ProcesoDisciplinarioId == process);
                Items = db.NotasProcesos.Where(x => x.ProcesoDisciplinarioId == process).ToList();
                foreach (var item in Items)
                {
                    item.Empleado2 = db.Empleados.FirstOrDefault(x => x.Id == item.UsuarioRegistraId);
                }
                return PartialView(Items);
            }
        }

        public ActionResult NotasCreate(string Id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            //else if (!Acceso.EsAnonimo && !funciones.Contains(""))
            //{
            //    Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
            //    return RedirectToAction("Index", "Login");
            //}
            var model = new NotasProcesos();
            var id = 0;
            Int32.TryParse(Id, out id);

            ViewBag.Proceso = db.ProcesoDisciplinario.FirstOrDefault(x => x.Id == id);

            return PartialView(model);
        }

        public ActionResult AgregarPruebas(string Id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
           
            var model = new PDPruebas();
            var id = 0;
            Int32.TryParse(Id, out id);

            ViewBag.Proceso = db.ProcesoDisciplinario.FirstOrDefault(x => x.Id == id);

            return PartialView(model);
        
        }

        public ActionResult AgregarAnexos(string Id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }

            var model = new PDAnexos();
            var id = 0;
            Int32.TryParse(Id, out id);

            ViewBag.Proceso = db.ProcesoDisciplinario.FirstOrDefault(x => x.Id == id);

            return PartialView(model);
        
        }

        [HttpPost]
        public JsonResult NotesUpload ()
        {
            var respuesta = "true";
            var Anotacion = HttpContext.Request.Params["Anotacion"];
            var Anexo = HttpContext.Request.Params["Anexo"];
            var Empleadolog = Session["Empleado"];
            var Fecha = DateTime.Now;
            var Id = HttpContext.Request.Params["IdProceso"];

            NotasProcesos DatosNota = new NotasProcesos();
            DatosNota.Anotacion = Anotacion;
            DatosNota.UsuarioRegistraId = Convert.ToInt16(Empleadolog);
            DatosNota.FechaHora = Fecha;
            DatosNota.ProcesoDisciplinarioId = Convert.ToInt32(Id);
            if (Anexo != "Sin anexar")
            {
                string nombreadjunto = "Anexo";
                var httpPostedFile = HttpContext.Request.Files[nombreadjunto];


                var extension = httpPostedFile.FileName.Split('.');
                var ext = extension[1].ToString().ToLower();

                if (ext != "jpg" && ext != "jpeg" && ext != "png" && ext != "doc" && ext != "docx" && ext != "pdf" && ext != "xlsx" && ext != "odt" && ext != "ods" && ext != "mp4")
                {
                    respuesta = "El tipo de archivo ." + extension[1] + " no es permitido.";
                    return Json(new
                    {
                        respuesta
                    });
                }


                var size = httpPostedFile.ContentLength / 1024;
                if (size > 7000)
                {
                    respuesta = "El archivo supera el tamaño permitido de carga.";
                    return Json(new
                    {
                        respuesta
                    });

                }
                DateTime date1 = DateTime.Now;
                var nombrearchivo = date1.ToString("yyyyMMddHHmmssffff").ToString() + "-" + httpPostedFile.FileName;
                // Get the complete file path
                var fileSavePath = Path.Combine(HttpContext.Server.MapPath("~/AnexosProcesosDisciplinarios/Notas"), nombrearchivo);
                DatosNota.Anexo = nombrearchivo;
                httpPostedFile.SaveAs(fileSavePath);
            }
            try
            {
                db.NotasProcesos.Add(DatosNota);
                db.SaveChanges();
                var url = Url.Action("");             
                respuesta = "Se ha creado una Nota de manera exitosa";
                return Json(new
                {
                    
                    isRedirect = true,
                    respuesta
                });
            }
            catch 
            {
                respuesta = "Hubo un error al guardar";
                return Json(new
                {
                    respuesta
                });
            }
            
        }

        [HttpPost]
        public JsonResult AgregarPruebas1()
        {

            var respuesta = "true";
            var TipoPrueba = HttpContext.Request.Params["TipoPrueba"];
            var Descripcion = HttpContext.Request.Params["Descripcion"];
            var Anexo = HttpContext.Request.Params["Anexo"];
            var Empleadolog = Session["Empleado"];
            var Id = HttpContext.Request.Params["IdProceso"];

            PDPruebas DatosPrueba = new PDPruebas();
            DatosPrueba.TipoPrueba = TipoPrueba;
            DatosPrueba.Descripcion = Descripcion;
            DatosPrueba.IdProcesoDisciplinario = Convert.ToInt32(Id);
            if (Anexo != "Sin anexar")
            {
                string nombreadjunto = "Anexo";
                var httpPostedFile = HttpContext.Request.Files[nombreadjunto];


                var extension = httpPostedFile.FileName.Split('.');
                var ext = extension[1].ToString().ToLower();
                if (ext != "jpg" && ext != "jpeg" && ext != "png" && ext != "doc" && ext != "docx" && ext != "pdf" && ext != "xlsx" && ext != "odt" && ext != "ods" && ext != "mp4")
                {
                    respuesta = "El tipo de archivo ." + extension[1] + " no es permitido.";
                    return Json(new
                    {
                        respuesta
                    });
                }


                var size = httpPostedFile.ContentLength / 1024;
                if (size > 7000)
                {
                    respuesta = "El archivo supera el tamaño permitido de carga.";
                    return Json(new
                    {
                        respuesta
                    });

                }
                DateTime date1 = DateTime.Now;
                var nombrearchivo = date1.ToString("yyyyMMddHHmmssffff").ToString() + "-" + httpPostedFile.FileName;
                // Get the complete file path
                var fileSavePath = Path.Combine(HttpContext.Server.MapPath("~/AnexosProcesosDisciplinarios/Pruebas"), nombrearchivo);
                DatosPrueba.Adjunto = nombrearchivo;
                httpPostedFile.SaveAs(fileSavePath);
            }
            try
            {
                db.PDPruebas.Add(DatosPrueba);
                db.SaveChanges();
                var url = Url.Action("");
                respuesta = "Se ha registrado el archivo de manera exitosa";
                return Json(new
                {

                    isRedirect = true,
                    respuesta
                });
            }
            catch
            {
                respuesta = "Hubo un error al guardar";
                return Json(new
                {
                    respuesta
                });
            }
        }

        [HttpPost]
        public JsonResult AgregarAnexos1 ()
        {

            var respuesta = "true";
            var Anexo = HttpContext.Request.Params["Anexo"];
            var Empleadolog = Session["Empleado"];
            var Id = HttpContext.Request.Params["IdProceso"];

            PDAnexos DatosAnexo = new PDAnexos();
            DatosAnexo.IdProcesoDisciplinario = Convert.ToInt32(Id);
            if (Anexo != "Sin anexar")
            {
                string nombreadjunto = "Anexo";
                var httpPostedFile = HttpContext.Request.Files[nombreadjunto];


                var extension = httpPostedFile.FileName.Split('.');
                var ext = extension[1].ToString().ToLower();
                if (ext != "jpg" && ext != "jpeg" && ext != "png" && ext != "doc" && ext != "docx" && ext != "pdf" && ext != "xlsx" && ext != "odt" && ext != "ods" && ext != "mp4")
                {
                    respuesta = "El tipo de archivo ." + extension[1] + " no es permitido.";
                    return Json(new
                    {
                        respuesta
                    });
                }


                var size = httpPostedFile.ContentLength / 1024;
                if (size > 7000)
                {
                    respuesta = "El archivo supera el tamaño permitido de carga.";
                    return Json(new
                    {
                        respuesta
                    });

                }
                DateTime date1 = DateTime.Now;
                var nombrearchivo = date1.ToString("yyyyMMddHHmmssffff").ToString() + "-" + httpPostedFile.FileName;
                // Get the complete file path
                var fileSavePath = Path.Combine(HttpContext.Server.MapPath("~/AnexosProcesosDisciplinarios/Anexos"), nombrearchivo);
                DatosAnexo.Archivo = nombrearchivo;
                httpPostedFile.SaveAs(fileSavePath);
            }
            try
            {
                db.PDAnexos.Add(DatosAnexo);
                db.SaveChanges();
                var url = Url.Action("");
                respuesta = "Se ha registrado el archivo de manera exitosa";
                return Json(new
                {

                    isRedirect = true,
                    respuesta
                });
            }
            catch
            {
                respuesta = "Hubo un error al guardar";
                return Json(new
                {
                    respuesta
                });
            }
        }

        public ActionResult RespuestaJefe(string Id)
        {
            var process = 0;
            Int32.TryParse(Id, out process);
            using (var db = new AutogestionContext())
            {
                ProcesoDisciplinario Items = new ProcesoDisciplinario();
                ViewBag.ProcesoDisciplinario = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == process);
                var Implicados = _repo.ObtenerTodos3(Id);
                List<PDTrabajador> z = new List<PDTrabajador>();
                foreach (PDTrabajador Item2 in Implicados)
                {
                    Item2.Jefe = db.Empleados.FirstOrDefault(e => e.NroEmpleado == Item2.Empleado2.Jefe);
                    z.Add(Item2);
                }

                Items = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == process);
                Items.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Items.EmpleadoRegistraId);
                Items.PDTrabajador = z;
                return PartialView(Items);
            }

        }
       
        public JsonResult LoadFiles() 
        {
            var Accion = HttpContext.Request.Params["TipoAccion"];
            var Id = HttpContext.Request.Params["id"];
            var id = 0;
            var respuesta = "TRUE";
            var ProcesoDisciplinario = new ProcesoDisciplinario();
            int.TryParse(Id, out id);

            ProcesoDisciplinario = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == id);
            db.ProcesoDisciplinario.Attach(ProcesoDisciplinario);

            if(Accion=="PRUEBA"){
                
            var CntPruebas = HttpContext.Request.Params["Cantidadpruebas"];
            List<PDPruebas> AdjuntosPruebas = new List<PDPruebas>();     
            int cantidadp = Convert.ToInt16(CntPruebas);
            for (int i = 1; i <= cantidadp; i++)
            {

                PDPruebas ArchivoPrueba = new PDPruebas();
                string nombreadjunto = "Adjunto" + i;
                ArchivoPrueba.IdProcesoDisciplinario = id;

                var PruebaType = HttpContext.Request.Params["PDPruebas.TipoPrueba" + i];
                var httpPostedFile = HttpContext.Request.Files[nombreadjunto];


                var extension = httpPostedFile.FileName.Split('.');
                var ext = extension[1].ToString().ToLower();

                if (ext != "jpg" && ext != "jpeg" && ext != "png" && ext != "doc" && ext != "docx" && ext != "pdf" && ext != "odt" && ext != "ods" && ext != "mp4")
                {
                    respuesta = "El tipo de archivo ." + extension[1] + " no es permitido.";
                    return Json(new
                    {
                        respuesta
                    });
                }

                var size = httpPostedFile.ContentLength / 1024;
                if (size > 7000)
                {
                    respuesta = "El archivo supera el tamaño permitido de carga.";
                    return Json(new
                    {
                        respuesta
                    });

                }


                // Validate the uploaded image(optional)
                DateTime date1 = DateTime.Now;
                var nombrearchivo = date1.ToString("yyyyMMddHHmmssffff" + i).ToString() + "-" + httpPostedFile.FileName;
                // Get the complete file path
                var fileSavePath = Path.Combine(HttpContext.Server.MapPath("~/AnexosProcesosDisciplinarios/Pruebas"), nombrearchivo);


                // Save the uploaded file to "UploadedFiles" folder
                httpPostedFile.SaveAs(fileSavePath);
                //ArchivoPrueba.IdProcesoDisciplinario = DatosProcesoDisciplinario.Id;
                ArchivoPrueba.TipoPrueba = PruebaType;
                ArchivoPrueba.Adjunto = nombrearchivo;
                AdjuntosPruebas.Add(ArchivoPrueba);
                db.PDPruebas.Add(ArchivoPrueba);
            }
            ProcesoDisciplinario.PDPruebas = AdjuntosPruebas;
            db.SaveChanges();
            respuesta = "Guardado Exitoso";

            }
            //----------------------------------------------------------------------------------------------//
            if (Accion == "ANEXO")
            {
          
                var CntAnexos = HttpContext.Request.Params["Cantidadanexos"];
                List<PDAnexos> AdjuntosAnexos = new List<PDAnexos>();

                int cantidada = Convert.ToInt16(CntAnexos);
                for (int i = 1; i <= cantidada; i++)
                {

                    PDAnexos ArchivoAnexo = new PDAnexos();
                    string nombreadjunto = "Anexo" + i;
                    var httpPostedFile = HttpContext.Request.Files[nombreadjunto];
                    ArchivoAnexo.IdProcesoDisciplinario=id;


                    var extension = httpPostedFile.FileName.Split('.');
                    var ext = extension[1].ToString().ToLower();

                    if (ext != "jpg" && ext != "jpeg" && ext != "png" && ext != "doc" && ext != "docx" && ext != "pdf" && ext != "odt" && ext != "ods" && ext != "mp4")
                    {
                        respuesta = "El tipo de archivo ." + extension[1] + " no es permitido.";
                        return Json(new { respuesta });
                    }

                    var size = httpPostedFile.ContentLength / 1024;
                    if (size > 7000)
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
                    //ArchivoAnexo.IdProcesoDisciplinario = DatosProcesoDisciplinario.Id;
                    ArchivoAnexo.Archivo = nombrearchivo;
                    AdjuntosAnexos.Add(ArchivoAnexo);
                    db.PDAnexos.Add(ArchivoAnexo);
                    
                }
               ProcesoDisciplinario.PDAnexos = AdjuntosAnexos;       
                 db.SaveChanges();
                 respuesta = "Guardado Exitoso";
            }
            return  Json(new
                 {
                    
                     //redirectUrl = url,
                     isRedirect = true,
                     respuesta
                 });
        }

        public JsonResult RespuestaJefe1() 
        {
            var Id = HttpContext.Request.Params["Id"];
            var FechaSuspencion = HttpContext.Request.Params["FechaSuspencion"];
            var respuesta = "true";
            string message = null;
            var FechaSuspencion2 = Convert.ToDateTime(FechaSuspencion);
            try
            {
                int id = 0;
                Int32.TryParse(Id, out id);
                using (var db = new AutogestionContext())
                {
                    var url = Url.Action("");
                    if(FechaSuspencion2<DateTime.Now)
                    {
                        return Json(new
                        {
                           
                            isRedirect = false,
                            respuesta = "La Fecha ingresada no es Valida"
                        });
                    }
                    if (Id != "0")
                    {

                        ProcesoDisciplinario vacaciones = db.ProcesoDisciplinario.Find(id);
                        _repo.Modificar2(Id, FechaSuspencion);

                    }
                    else
                    {
                        message = "Antes de Guardar, debe seleccionar un registro de la tabla.";
                    }

                    // TODO: Add update logic here

                    Session["message"] = message;
                    url = Url.Action("");
                    return Json(new
                    {
                        redirectUrl = url,
                        isRedirect = true,
                        respuesta = "Guardado Exitoso"
                    });


                }

            }
            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
                Session["message"] = message;
                respuesta = "Error durante el guardado";
                return Json(new { respuesta });
            }
        }

        //public bool notificar_RespuestaJ(string Id, string usuario, string respuesta, string fecha)
        //{

        //    string textocorreo = "";
        //    bool confirmacion;
        //    string txtde = Properties.Settings.Default.Correo.ToString();
        //    string contraseñacorreo = "";
        //    var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
        //    contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);

        //    var email = "";
        //    using (var db = new AutogestionContext())
        //    {

        //        var texto = db.Configuraciones.First(s => s.Parametro == "TXTRESPJURIDICA").Valor.ToString();
        //         email = db.Configuraciones.First(s => s.Parametro == "CORREORESPJURIDICA").Valor.ToString();

        //        texto = texto.Replace("$USUARIO", usuario);
        //        texto = texto.Replace("$ID", Id);
        //        texto = texto.Replace("$RESPUESTA", respuesta);
        //        texto = texto.Replace("$FECHA", fecha);


        //        textocorreo = texto;
        //    }
         
        //    try
        //    {
        //        System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
        //        correo.From = new System.Net.Mail.MailAddress(txtde);


        //        correo.To.Add(email);

        //        correo.Subject = "Notificación Respuesta Juridica";
        //        correo.Body = textocorreo;
        //        correo.Priority = System.Net.Mail.MailPriority.Normal;
        //        correo.IsBodyHtml = true;



        //        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
        //        smtp.Host = "smtp.gmail.com";
        //        smtp.Port = 587;
        //        smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
        //        smtp.EnableSsl = true;
        //        smtp.Send(correo);
        //        confirmacion = true;
        //        return confirmacion;
        //    }
        //    catch (Exception ex)
        //    {

        //        confirmacion = false;
        //        return confirmacion;
        //    }


        //}
        //--------------------------------V----------------------------//
        public bool notificar_Juridico(string Id, string usuario, List<string> implicados, string fecha)
        {

            string textocorreo = "";
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
            var trabajadores = "";
            var email = "";
            
            using (var db = new AutogestionContext())
            {

                var texto = db.Configuraciones.First(s => s.Parametro == "TEXTOPASOAJURIDICA").Valor.ToString();
                email = db.Configuraciones.First(s => s.Parametro == "CORREOPASOJURIDICA").Valor.ToString();
                foreach(var nom in implicados)
                {
                    trabajadores = trabajadores+"• " + nom + "<BR>";
                }

                texto = texto.Replace("$USUARIO", usuario);
                texto = texto.Replace("$ID", Id);
                texto = texto.Replace("$IMPLICADOS", trabajadores);
                texto = texto.Replace("$FECHA", fecha);


                textocorreo = texto;
            }

            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);


                correo.To.Add(email);

                correo.Subject = "Notificación Proceso Remitido a Juridica";
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

        public bool notificar_CierreP(string Id, string usuario, string respuesta, string fecha,string adjunto,string usercorreo, string empresa)
        {

            string textocorreo = "";
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
            var archivo=adjunto;
            int id = 0;
            Int32.TryParse(Id, out id);
           
            var email = "";
            var email2 = "";

            System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
            correo.From = new System.Net.Mail.MailAddress(txtde);


            using (var db = new AutogestionContext())
            {

                var texto = db.Configuraciones.First(s => s.Parametro == "TXTCIERREPROCESO").Valor.ToString();
                if(empresa=="1000"){
                    email = db.Configuraciones.First(s => s.Parametro == "CORREOCIERREPD1000").Valor.ToString() + ", " + usercorreo;
                }
                else if (empresa == "2000")
                {
                    email = db.Configuraciones.First(s => s.Parametro == "CORREOCIERREPD2000").Valor.ToString() +", "+ usercorreo;
                }

                texto = texto.Replace("$USUARIO", usuario);
                texto = texto.Replace("$ID", Id);
                texto = texto.Replace("$RESPUESTA", respuesta);
                texto = texto.Replace("$FECHA", fecha);


                textocorreo = texto;

                var pd = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == id);
                var EmpleadoRegis = db.Empleados.FirstOrDefault(e => e.Id == pd.EmpleadoRegistraId);
                if (EmpleadoRegis.Cargo == "COORDINADOR(A) ENFERMERIA HOSPITAL. (E)" || EmpleadoRegis.Cargo == "COORDINADOR(A) ENFERMERIA HOSPITALIZACIO" || EmpleadoRegis.Cargo == "COORDINADOR(A) DE ENFERMERIA CIRUGIA CAL" || EmpleadoRegis.Cargo == "COORDINADOR(A) DE ENFERMERIA UCI - P" || EmpleadoRegis.Cargo == "COORDINADOR(A) DE ENFERMERIA UCI - A" || EmpleadoRegis.Cargo == "COORDINADOR(A) ENFERMERIA" || EmpleadoRegis.Cargo == "COORDINADOR(A) DE HOSPITALIZACION")
                {
                    email2 = db.Configuraciones.First(s => s.Parametro == "CORREOCIERREPDJEFEASIST").Valor.ToString();
                    correo.CC.Add(email2);
                }
            }

            try
            {
                

                correo.To.Add(email);
                
                correo.Subject = "Notificación Cierrre del Proceso";
                correo.Body = textocorreo;
                correo.Priority = System.Net.Mail.MailPriority.Normal;
                correo.IsBodyHtml = true;
                //--------------------Ajuntar archivo---------------/
                var adjuntoJr=Server.MapPath(@"..\AnexosProcesosDisciplinarios\" + archivo);
                correo.Attachments.Add(new Attachment(adjuntoJr));
                //-------------------------------------------------/

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


        public ActionResult GestionJefe(string Id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ProcesoDisciplinarioGestionJefe"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }


            int id = -1;
            string opcion = "Jefe";
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

        public ActionResult Informe(string EmpleadoCP, string NroProceso, string Estado, string FechaInicio, string FechaFin)
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
            ViewBag.Empleado = db.Empleados.Where(e => e.Activo != "NO").Select(e => new { e.Id, e.Nombres }).ToList();

            var datos = _repo.ObtenerInforme(EmpleadoCP, NroProceso, Estado, FechaInicio, FechaFin);

            var empleadosIds = datos.Select(x => x.EmpleadoRegistraId).Distinct().ToList();
            var empleadosRegistrados = db.Empleados.Where(x => empleadosIds.Contains(x.Id)).ToList();

            var empleadosImplicados = datos
                .GroupJoin(
                    db.PDTrabajador,
                    p => p.Id,
                    pd => pd.ProcesoDisciplinarioId,
                    (p, pd) => new { Proceso = p, PD = pd }
                )
                .SelectMany(
                    x => x.PD,
                    (x, pd) => new { x.Proceso.Id, pd.EmpleadoId }
                )
                .ToList();

            var empleadosImplicadosDic = new Dictionary<int, List<string>>();
            foreach (var empleadoImplicado in empleadosImplicados)
            {
                if (!empleadosImplicadosDic.ContainsKey(empleadoImplicado.Id))
                {
                    empleadosImplicadosDic[empleadoImplicado.Id] = new List<string>();
                }

                var empleado = db.Empleados.FirstOrDefault(e => e.Id == empleadoImplicado.EmpleadoId);
                if (empleado != null)
                {
                    empleadosImplicadosDic[empleadoImplicado.Id].Add(empleado.Nombres);
                }
            }

            foreach (var registro in datos)
            {
                registro.EmpleadoNombres = empleadosRegistrados.FirstOrDefault(x => x.Id == registro.EmpleadoRegistraId)?.Nombres;
                registro.EmpleadoArea = empleadosRegistrados.FirstOrDefault(x => x.Id == registro.EmpleadoRegistraId)?.Area;

                var empleadoRegistrado = empleadosRegistrados.FirstOrDefault(x => x.Id == registro.EmpleadoRegistraId);
                var superiorEmpleado = empleadosRegistrados.FirstOrDefault(x => x.NroEmpleado == empleadoRegistrado?.Superior);
                registro.SuperiorEmpleado = superiorEmpleado?.Nombres;

                if (empleadosImplicadosDic.TryGetValue(registro.Id, out var nombresImplicados))
                {
                    registro.EmpleadosImplicados = string.Join(",", nombresImplicados);
                }
                else
                {
                    registro.EmpleadosImplicados = string.Empty;
                }
            }

            return View(datos);
        }
    }
}
