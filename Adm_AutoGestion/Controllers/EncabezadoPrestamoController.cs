using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System.Data.Entity;
using System.Web.Services.Description;

namespace Adm_AutoGestion.Controllers
{
    public class EncabezadoPrestamoController : Controller
    {
        private EncabezadoPrestamoRepository _repo;
        private ServiciosRepository _servicios;


        // GET: /EncabezadoEntregaPrenda/


        public EncabezadoPrestamoController()
        {
            _repo = new EncabezadoPrestamoRepository();
            _servicios = new ServiciosRepository();


        }


        //public ActionResult Index()
        //{
        //    return View();
        //}

        //
        // GET: /EncabezadoEntregaPrenda/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /EncabezadoEntregaPrenda/Create

        public class resultado {

            public string CodigoEmpleado { get; set; }
            public string Nombres { get; set; }
        }


        public ActionResult Create2()
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("Prestamo"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            List<SelectListItem> lst = new List<SelectListItem>();
            List<SelectListItem> lst2 = new List<SelectListItem>();
            using (var db = new AutogestionContext())

            {

                var lista = db.PersonalActivo.Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                foreach (var x in lista)
                {
                    lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }


                ViewBag.Area = lst;

                var lista2 = db.Empleados.Select(x => new { x.Cargo }).GroupBy(b => b.Cargo).ToList();
                foreach (var x in lista2)
                {
                    lst2.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }




                ViewBag.Cargo = lst2;
                ViewBag.Sociedad = db.Sociedad.ToList();


                string consulta = "select PersonalActivoes.Nombres, PersonalActivoes.CodigoEmpleado from PersonalActivoes where  PersonalActivoes.CodigoEmpleado in (SELECT  PersonalActivoes.Jefe FROM PersonalActivoes WHERE PersonalActivoes.Jefe <>''group by PersonalActivoes.Jefe)";
                var resultados = db.Database.SqlQuery<resultado>(consulta).ToList();

                ViewBag.Jefe = resultados;

            }

            return PartialView();
        }
        //
        // POST: /EncabezadoEntregaPrenda/Create

        //[HttpPost]
        //public ActionResult Create(Tercero model, string  PrimerApellido, string SegundoApellido, string PrimerNombre, string SegundoNombre)
        //{

        //    List<string> funciones = Acceso.Validar(Session["Empleado"]);

        //    if (Acceso.EsAnonimo)
        //    {
        //        return RedirectToAction("Login", "Login");
        //    }
        //    else if (!Acceso.EsAnonimo && !funciones.Contains("CrearEPP"))
        //    {
        //        Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
        //        return RedirectToAction("Index", "Login");
        //    }




        //    using (var db = new AutogestionContext())
        //         {


        //    string message = "";
        //    try
        //    {


        //            model.Nombres = (PrimerApellido + " " + SegundoApellido + " " + PrimerNombre + " " + SegundoNombre).ToUpper();
        //            int IdUsuarioR = 0;
        //            string registra = String.Format("{0}", Session["Empleado"]);
        //            Int32.TryParse(registra, out IdUsuarioR);
        //            model.UsuarioRegistraId = IdUsuarioR;
        //            model.FechaRegistro = DateTime.Now;



        //            Session["message"] = _repo.Crear(model);
        //           // message = _repo.Crear(model);
        //            return RedirectToAction("ListSolicitantes");



        //        // TODO: Add insert logic here


        //    }
        //    catch (SystemException ex)
        //    {
        //        message = String.Format("Se genero un error. {0}", ex.Message);
        //        return RedirectToAction("ListSolicitantes");
        //    }


        //    }

        //}

        public ActionResult _SolicitudPrestamo(string id, string Nombres, string Correo, string Telefono) {
            using (var db = new AutogestionContext())
            {

                List<SelectListItem> lst = new List<SelectListItem>();

                var lista = db.PersonalActivo.Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                foreach (var x in lista)
                {
                    lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }


                var prestamo = db.EncabezadoPrestamo.Where(x => x.Documento == id && x.Estado == "ACTIVO").ToList();

                if (prestamo.Count > 0)
                {

                    ViewBag.Ban = 1;
                    return PartialView();

                }
                else
                {

                    ViewBag.Ban = 0;
                    ViewBag.Area = lst;
                    ViewBag.TipoElemento = db.TipoElementos.ToList();
                    ViewBag.Documento = id;
                    ViewBag.Nombres = Nombres;
                    ViewBag.Correo = Correo;
                    ViewBag.Telefono = Telefono;
                    ViewBag.Sociedad = db.Sociedad.ToList();
                    ViewBag.Lugar = db.LugarEntrega.ToList();
                    return PartialView();
                }

            }
        }



        public String GuardarPrestamo(EncabezadoPrestamo model)
        {
            string message = "";
            Tercero tercero = new Tercero();
            using (var db = new AutogestionContext())
            {
                try

                {
                    int IdUsuarioR = 0;
                    string registra = String.Format("{0}", Session["Empleado"]);
                    Int32.TryParse(registra, out IdUsuarioR);

                    model.UsuarioRegistraId = IdUsuarioR;
                    model.Estado = "ACTIVO";


                    var correo = "";
                    var nombres = "";
                    Empleado empleado = new Empleado();
                    empleado = db.Empleados.FirstOrDefault(x => x.Documento == model.Documento);

                    if (empleado != null)
                    {
                        var personal = db.PersonalActivo.FirstOrDefault(x => x.Documento == empleado.Documento);
                        string cargo = "";
                        if (personal != null)
                        {
                            cargo = personal.Cargo;
                        }
                       
                        if (empleado.Activo=="NO")
                        {
                            throw new ArgumentOutOfRangeException(null, "El empleado no se encuentra actualmente Activo");
                        }
                        string textoqr = empleado.NroEmpleado + "|" + empleado.Nombres + "|" + empleado.Documento + "|" + empleado.Empresa + "|" + cargo;
                        string QR= _servicios.encriptar(textoqr);
                        correo = empleado.Correo;
                        nombres = empleado.Nombres;
                        if (model.QRPrestamos != QR)
                        {
                            throw new ArgumentOutOfRangeException(null, "El código QR no coincide con el del Trabajador");
                        }

                    }
                    else
                    {


                        tercero = db.Tercero.FirstOrDefault(x => x.Documento == model.Documento);
                        if (tercero.Activo == "NO")
                        {
                            throw new ArgumentOutOfRangeException(null, "El empleado no se encuentra actualmente Activo");
                        }
                        if (tercero != null)
                        {
                            correo = tercero.CorreoPersonal;
                            nombres = tercero.Nombres;

                        }
                        if (model.QRPrestamos != tercero.QRPrestamo)
                        {
                            throw new ArgumentOutOfRangeException(null, "El código QR no coincide con el del Trabajador");
                        }

                    }
                   


                    message = _repo.RegistrarPrestamo(model, correo, nombres);

                    return message;
                }
                catch (SystemException ex) {

                    return ex.Message.ToString();
                }
            }
        }
        public String GuardarTercero(Tercero model)
        {

            string message = "";
            using (var db = new AutogestionContext())
            {

                try
                {

                    int IdUsuarioR = 0;
                    string registra = String.Format("{0}", Session["Empleado"]);
                    Int32.TryParse(registra, out IdUsuarioR);
                    model.UsuarioRegistraId = IdUsuarioR;
                    model.FechaRegistro = DateTime.Now;




                    message = _repo.Crear(model);



                    return message;


                }
                catch (SystemException ex)
                {

                    return ex.Message.ToString();
                }

            }
        }


        public String ValidarPrestamoActivo(string Documento) {

            string message = "";

            using (var db = new AutogestionContext()) {

                try
                {


                    var prestamo = db.EncabezadoPrestamo.Where(x => x.Documento == Documento && x.Estado == "ACTIVO").ToList();

                    if (prestamo.Count > 0)
                    {

                        message = "El solicitante tiene un Prestamo Vigente";

                        return message;
                    }


                    return message;

                }
                catch (SystemException ex) {

                    return ex.Message.ToString();

                }

            }


        }

        public ActionResult _DevolucionPrestamo(string id, string Nombres, string Correo, string Telefono)
        {
            using (var db = new AutogestionContext())
            {

                var NoEntrega = db.EncabezadoPrestamo
                    .FirstOrDefault(x => x.Documento == id && x.Estado == "ACTIVO");

                if (NoEntrega == null)
                {

                    ViewBag.Ban = 1;
                    return PartialView();

                }
                else {
                    ViewBag.Ban = 0;
                    ViewBag.NoEntrega = NoEntrega.Id;
                    ViewBag.FechaEntrega = NoEntrega.FechaRegistro;
                    ViewBag.Documento = id;
                    ViewBag.Nombres = Nombres;
                    ViewBag.Correo = Correo;
                    ViewBag.Telefono = Telefono;
                    ViewBag.Sociedad = NoEntrega.Sociedad;

                    var DetallePrestamo = db.DetalleEncabezadoPrestamo.Include(x => x.TipoElementos).Where(x => x.IdEncabezadoPrestamo == NoEntrega.Id).ToList();
                    ViewBag.DetallePrestamo = DetallePrestamo;
                    return PartialView();


                }

            }

        }

        public class Devolucion
        {
            public string IdEntrega { get; set; }
            public string QR { get; set; }

        }
        public String GuardarDevolucion(Devolucion model)
        {
            int IdEntrega = Convert.ToInt32(model.IdEntrega);

            string message = "";

            using (var db = new AutogestionContext())
            {

                try
                {

                    EncabezadoPrestamo prestamo = new EncabezadoPrestamo();
                    Tercero tercero = new Tercero();

                    prestamo = db.EncabezadoPrestamo.FirstOrDefault(e => e.Id == IdEntrega);
                    tercero = db.Tercero.FirstOrDefault(e => e.Documento == prestamo.Documento);
                    //var QR2 = _servicios.desencriptar(model.QR);
                    //var QR1 = _servicios.desencriptar(tercero.QRPrestamo);
                    Empleado empleado = new Empleado();
                    empleado = db.Empleados.FirstOrDefault(x => x.Documento == prestamo.Documento);
                    if (empleado != null)
                    {
                        var personal = db.PersonalActivo.FirstOrDefault(x => x.Documento == empleado.Documento);
                        string cargo = "";
                        if (personal != null)
                        {
                            cargo = personal.Cargo;
                        }

                        string textoqr = empleado.NroEmpleado + "|" + empleado.Nombres + "|" + empleado.Documento + "|" + empleado.Empresa + "|" + cargo;
                        string QR = _servicios.encriptar(textoqr);

                        if (model.QR != QR)
                        {
                            throw new ArgumentOutOfRangeException(null, "El código QR no coincide con el del Trabajador");
                        }

                    }
                    else
                    {
                        if (tercero.QRPrestamo != model.QR)
                        {

                            throw new ArgumentOutOfRangeException("El QR no corresponde al Trabajador", "Error al Validar el QR");
                        }
                    }
                    
                    prestamo.Estado = "INACTIVO";
                    int IdUsuarioR = 0;
                    string registra = String.Format("{0}", Session["Empleado"]);
                    Int32.TryParse(registra, out IdUsuarioR);

                    message = _repo.RegistrarDevolucion(IdEntrega, prestamo, IdUsuarioR);

                    return message;



                }
                catch (SystemException ex) {

                    return ex.Message.ToString();
                }

            }
        }



        public String AnularPrestamo(string Documento)
        {

            string message = "";

            using (var db = new AutogestionContext())
            {
                try
                {


                    var Prestamo = db.EncabezadoPrestamo
                   .FirstOrDefault(x => x.Documento == Documento && x.Estado == "ACTIVO");

                    if (Prestamo == null)

                    {

                        message = "El Solicitante no tiene prestamos vigentes";
                        return message;
                    }
                    else
                    {

                        int IdUsuarioR = 0;
                        string registra = String.Format("{0}", Session["Empleado"]);
                        Int32.TryParse(registra, out IdUsuarioR);

                        message = _repo.AnularPrestamo(Documento, IdUsuarioR);
                        return message;
                    }
                }
                catch (Exception ex)
                {

                    return ex.Message.ToString();
                }

            }
        }

        public class Solicitantes {

            public string Documento { get; set; }
            public string Nombres { get; set; }
            public string Telefono { get; set; }
            public string Correo { get; set; }
            public string Area { get; set; }
            public string Jefe { get; set; }
            public string Cargo { get; set; }
            public string Activo { get; set; }
            public DateTime? FechaF { get; set; }
        }


        public ActionResult ListSolicitantes(string Documentos, string Nombres)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("Prestamo"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            using (var db = new AutogestionContext())


            {
                var hoy= DateTime.Now;
                //string consulta = "select  Documento,Nombres , Telefono,Correo,Area,Cargo,Superior, Activo from Empleadoes  union all select Documento, Nombres,Telefono, CorreoPersonal, Area, Cargo, Superior, Activo from  Terceroes";
                //var resultados = db.Database.SqlQuery<Solicitantes>(consulta).ToList();

                var consulta = (from e in db.Empleados
                                //where e.Activo == "SI"
                                select new Solicitantes { Documento = e.Documento, Nombres = e.Nombres, Cargo = e.Cargo, Area = e.Area, Jefe = e.Jefe, Correo = e.Correo, Activo = e.Activo, Telefono = e.Telefono, FechaF= e.FechaFin })
                                .Union(from t in db.Tercero
                                       //where t.Activo == "SI"
                                       select new Solicitantes { Documento = t.Documento, Nombres = t.Nombres, Cargo = t.Cargo, Area = t.Area, Jefe = t.Superior, Correo = t.CorreoPersonal, Activo = t.Activo, Telefono = t.Telefono, FechaF = t.FechaFin })
                                         .Where(e => e.Documento.Contains(Documentos))                                      
                                         .Where(e => (DbFunctions.TruncateTime(e.FechaF) >= hoy)|| e.Activo=="SI")
                                         .Where(e => e.Nombres.Contains(Nombres)).ToList();
                if (Documentos != null || Nombres != null) {

                    if (consulta.Count <= 0)
                    {
                        var consulta2 = (from e in db.Empleados
                                         //where e.Activo == "NO"
                                         select new Solicitantes { Documento = e.Documento, Nombres = e.Nombres, Cargo = e.Cargo, Area = e.Area, Jefe = e.Jefe, Correo = e.Correo, Activo = e.Activo, Telefono = e.Telefono, FechaF = e.FechaFin })
                                .Union(from t in db.Tercero
                                       //where t.Activo == "NO"
                                       select new Solicitantes { Documento = t.Documento, Nombres = t.Nombres, Cargo = t.Cargo, Area = t.Area, Jefe = t.Superior, Correo = t.CorreoPersonal, Activo = t.Activo, Telefono = t.Telefono, FechaF = t.FechaFin })
                                         .Where(e => e.Documento.Contains(Documentos))
                                         .Where(e => (DbFunctions.TruncateTime(e.FechaF) <= hoy) || e.Activo=="NO")
                                         .Where(e => e.Nombres.Contains(Nombres)).ToList();
                       
                     
                        if (consulta2.Count <= 0)
                        {
                            ViewBag.EstadoCosulta = 1;

                            Session["message"] = "El solicitante no existe en la base de datos, debe crearlo";
                        }
                        else 
                        {
                            Session["message"] = "El Trabajador se encuentra inactivo";
                        }
                    }



                }



                return View(consulta);
            }



        }

        public class reporteprestamos {
            public DateTime FechaRegistro { get; set; }
            public int NoPrestamo { get; set; }
            public string Sociedad { get; set; }
            public string TipoArea { get; set; }
            public string AreaDirige { get; set; }
            public string Estado { get; set; }
            public string Documento { get; set; }
            public string Nombres { get; set; }
            public string Telefono { get; set; }
            public string Correo { get; set; }
            public string Jefe { get; set; }



        }


        public ActionResult ReportePrestamos(string FechaInicio, string FechaFin, string Sociedad, string TipoArea, string Estado, string AreaDirige) {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("Prestamo"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            using (var db = new AutogestionContext()) {

                if (FechaInicio=="" ||FechaFin =="") 
                {
                    //throw new ArgumentOutOfRangeException(null, "Definir Rango de Fechas");
                    Session["message"] = "Se debe Definir el Rango de Fechas";
                }
                DateTime FechaI = DateTime.Now;
                DateTime.TryParse(FechaInicio, out FechaI);
                DateTime FechaF = DateTime.Now;
                DateTime.TryParse(FechaFin, out FechaF);
                List<reporteprestamos> consulta= new List<reporteprestamos>();
                if (Estado!="") {
                 consulta = (from ep in db.EncabezadoPrestamo
                                join e in db.Empleados on ep.Documento equals e.Documento
                                select new reporteprestamos { FechaRegistro = ep.FechaRegistro, NoPrestamo = ep.Id, Sociedad = ep.Sociedad, TipoArea = ep.TipoArea, AreaDirige = ep.AreaDirige, Estado = ep.Estado, Documento = e.Documento, Nombres = e.Nombres, Telefono = e.Telefono, Correo = e.Correo, Jefe = e.Superior })
                             .Union
                             (from ep in db.EncabezadoPrestamo
                              join t in db.Tercero on ep.Documento equals t.Documento
                              select new reporteprestamos { FechaRegistro = ep.FechaRegistro, NoPrestamo = ep.Id, Sociedad = ep.Sociedad, TipoArea = ep.TipoArea, AreaDirige = ep.AreaDirige, Estado = ep.Estado, Documento = t.Documento, Nombres = t.Nombres, Telefono = t.Telefono, Correo = t.CorreoPersonal, Jefe = t.Superior })
                              .Where(e => (DbFunctions.TruncateTime(e.FechaRegistro) >= FechaI && DbFunctions.TruncateTime(e.FechaRegistro) <= FechaF))
                              .Where(e => e.Sociedad.Contains(Sociedad))
                              .Where(e => e.TipoArea.Contains(TipoArea))
                              .Where(e => e.AreaDirige.Contains(AreaDirige))
                              .Where(e => e.Estado.Equals(Estado))
                              .ToList();
                }
                else {
                     consulta = (from ep in db.EncabezadoPrestamo
                                    join e in db.Empleados on ep.Documento equals e.Documento
                                    select new reporteprestamos { FechaRegistro = ep.FechaRegistro, NoPrestamo = ep.Id, Sociedad = ep.Sociedad, TipoArea = ep.TipoArea, AreaDirige = ep.AreaDirige, Estado = ep.Estado, Documento = e.Documento, Nombres = e.Nombres, Telefono = e.Telefono, Correo = e.Correo, Jefe = e.Superior })
                             .Union
                             (from ep in db.EncabezadoPrestamo
                              join t in db.Tercero on ep.Documento equals t.Documento
                              select new reporteprestamos { FechaRegistro = ep.FechaRegistro, NoPrestamo = ep.Id, Sociedad = ep.Sociedad, TipoArea = ep.TipoArea, AreaDirige = ep.AreaDirige, Estado = ep.Estado, Documento = t.Documento, Nombres = t.Nombres, Telefono = t.Telefono, Correo = t.CorreoPersonal, Jefe = t.Superior })
                              .Where(e => (DbFunctions.TruncateTime(e.FechaRegistro) >= FechaI && DbFunctions.TruncateTime(e.FechaRegistro) <= FechaF))
                              .Where(e => e.Sociedad.Contains(Sociedad))
                              .Where(e => e.TipoArea.Contains(TipoArea))
                              .Where(e => e.AreaDirige.Contains(AreaDirige))
                              
                              .ToList();
                }




                List<SelectListItem> lst = new List<SelectListItem>();

                var lista = db.PersonalActivo.Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                foreach (var x in lista)
                {
                    lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }

                ViewBag.Sociedad = db.Sociedad.ToList();
                ViewBag.Area = lst;

                return View(consulta);

            }



        }


        public ActionResult VerQR(int id) {

            using (var db = new AutogestionContext())
            {

                var consulta = db.Tercero.Where(x => x.Id == id).FirstOrDefault();


                return PartialView(consulta);
            }


        }

        public ActionResult Index(string Sociedad, string Documento)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("Prestamo"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            using (var db = new AutogestionContext())
            {

                ViewBag.Sociedad = db.Sociedad.ToList();


                var consulta = db.Tercero
                                .Where(e => e.SociedadCOD.Contains(Sociedad) && e.Documento.Contains(Documento)).ToList();



                return View(consulta);

            }



        }

        [HttpPost]
        public String ObtenerQRTercero(string id)
        {
            try
            {
                var idempleado = Convert.ToInt32(id);
                using (var db = new AutogestionContext())
                {

                    var model_empleado = db.Tercero.FirstOrDefault(x => x.Id == idempleado);

                    if (id != null)
                    {

                        string textoqr = model_empleado.QRPrestamo;
                        byte[] ImagenQR = _servicios.GenerarQR(textoqr);
                        return (Convert.ToBase64String(ImagenQR));
                    }
                    else
                    {
                        return "noexiste";

                    }

                }


            }
            catch (Exception e)
            {

                return "error";
            }


        }


        [HttpPost]
        public string ActualizarQR(string Documento)
        {
            var message = "";
            using (var db =new AutogestionContext() )         
            {
                Tercero tercero = db.Tercero.FirstOrDefault(x => x.Documento == Documento);
                string text = tercero.Nombres + "|" + tercero.Documento + "|" + tercero.Cargo;
                string respuesta = _servicios.encriptar(text);
                if (tercero.QRPrestamo!= respuesta) 
                {

                    message = _repo.ActualizarQR(tercero, respuesta);
                }
                else { message = "No hay cambios pendientes"; }
            }
         


            return message;
        }

        //------
    }
}
