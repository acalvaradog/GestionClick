using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Adm_AutoGestion.Services;
using Adm_AutoGestion.Models;
using System.IO;
using System.Net.Mime;

namespace Adm_AutoGestion.Controllers
{
    public class EmpleadoController : Controller
    {

        private EmpleadoRepository _repo;
        private ServiciosRepository _servicios;
        private SoporteHojaDeVidaRepository _repsoportes;
        private AutogestionContext db = new AutogestionContext();

        public EmpleadoController()
        {

            _repo = new EmpleadoRepository();
            _servicios = new ServiciosRepository();
            _repsoportes = new SoporteHojaDeVidaRepository();
        }

        //
        // GET: /Empleado/

        public ActionResult Index()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("Empleado"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            
            
            int IdUsuarioM = 0;
            string modifica = String.Format("{0}", Session["Empleado"]);
            Int32.TryParse(modifica, out IdUsuarioM);

            var empleado_sesion = db.Empleados.FirstOrDefault(x => x.Id== IdUsuarioM);


            var model = _repo.ObtenerTodos_Empresa(empleado_sesion.Empresa);
            return View(model);
        }

        public ActionResult Index2()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EmpleadoNOSAP"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var model = _repo.ObtenerTodosNoSAP();
            return View(model);
        }
        //
        // GET: /Empleado/Details/5

        public ActionResult Details(int id)
        {
            using (var db = new AutogestionContext())
            {
                Empleado empleado = db.Empleados.Find(id);
                
                if (empleado == null)
                {
                    return HttpNotFound();
                }
                return View(empleado);
            }
        }

        public ActionResult Details2(int id)
        {
            using (var db = new AutogestionContext())
            {
                Empleado empleado = db.Empleados.Find(id);
                if (empleado == null)
                {
                    return HttpNotFound();
                }
                return PartialView(empleado);
            }
        }

        //
        // GET: /Empleado/Create

        public ActionResult Create()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EmpleadoCrear"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        public ActionResult Create2()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EmpleadoCrearNOSAP"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index2", "Login");
            }
            List<SelectListItem> lst = new List<SelectListItem>();
            var db = new AutogestionContext();
            var lista = db.Empleados.Where(s => s.Activo == "SI" && (s.TipoArea != null && s.TipoArea != "")).GroupBy(b => b.TipoArea).ToList();
            foreach (var x in lista)
            {
                Empleado Item = x.FirstOrDefault();
                lst.Add(new SelectListItem() { Value = Item.TipoArea, Text = Item.TipoArea });
            }
            ViewBag.TiposdeArea = lst;
            return View();
        }
        [HttpPost]
        public ActionResult Create2(Empleado model , string NombresEmp, string Apellidos)
        {
            try
            {

                model.Nombres = Apellidos + " " + NombresEmp;
                // TODO: Add insert logic here
                if (model.Nombres != "" && model.NroEmpleado !="" && model.Documento != "" && model.Empresa != "") 
                {

                    if (model.Nombres != null && model.NroEmpleado != null && model.Documento != null && model.Empresa != null) 
                    {
                    
                        var idEmp = Session["Empleado"];
                    int Id = Convert.ToInt32(idEmp);
                    using (var db = new AutogestionContext())
                    {
                        Empleado UsuarioCreador = db.Empleados.Where(x => x.Id == Id).FirstOrDefault();
                        model.A_UsuarioCreador = UsuarioCreador.NroEmpleado;
                        model.A_Creacion = DateTime.Now;
                    }

                        _repo.Crear(model);
                    }
                }
                return RedirectToAction("Index2");
                

            }
            catch
            {

            }
            return View();
        }


        public ActionResult ActualizarEmpleados()
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ActualizacionMasivaEmpleados"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }



            EmpleadoRepository er = new EmpleadoRepository();
            er.ProcesoActualizacionyRetirados();

            return View();
        }

        public ActionResult ActualizarEstructura()
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("ActualizacionMasivaEstructura"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }


            EmpleadoRepository er = new EmpleadoRepository();
            er.ActualizarEstructuraJerarquicaEmpleado();

            return View();
        }


        //
        // POST: /Empleado/Create

        [HttpPost]
        public ActionResult Create(Empleado model)
        {
            try
            {


                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    var idEmp = Session["Empleado"];
                    int Id = Convert.ToInt32(idEmp);
                    using (var db = new AutogestionContext()) 
                    {
                        Empleado UsuarioCreador = db.Empleados.Where(x => x.Id == Id).FirstOrDefault();
                        model.A_UsuarioCreador = UsuarioCreador.NroEmpleado;
                        model.A_Creacion = DateTime.Now;
                    }
                        
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
        // GET: /Empleado/Edit/5

        public ActionResult Edit(int id)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EmpleadoEditar"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            using (var db = new AutogestionContext())
            {
                Empleado empleado = db.Empleados.Find(id);
                if (empleado == null)
                {
                    return HttpNotFound();
                }

                

                return PartialView(empleado);
            }
            
        }

        //
        // POST: /Empleado/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, Empleado model)
        {
            try
            {
                // TODO: Add update logic here

                if (ModelState.IsValid)
                {
                    var idEmp = Session["Empleado"];
                    int Id = Convert.ToInt32(idEmp);
                    using (var db = new AutogestionContext())
                    {
                        Empleado UsuarioModificador = db.Empleados.Where(x => x.Id == Id).FirstOrDefault();
                        model.A_UsuarioModifica = UsuarioModificador.NroEmpleado;
                        model.A_Modificacion = DateTime.Now;
                    }
                    _repo.Editar(model);

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                
            }
            return View();
        }

        public ActionResult Edit2(int id)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EmpleadoEditarNOSAP"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            using (var db = new AutogestionContext())
            {
                Empleado empleado = db.Empleados.Find(id);
                if (empleado == null)
                {
                    return HttpNotFound();
                }



                return PartialView(empleado);
            }

        }

        //
        // POST: /Empleado/Edit/5

        [HttpPost]
        public ActionResult Edit2(int id, Empleado model)
        {
            try
            {
                // TODO: Add update logic here

                if (ModelState.IsValid)
                {
                    var idEmp = Session["Empleado"];
                    int Id = Convert.ToInt32(idEmp);
                    using (var db = new AutogestionContext())
                    {
                        Empleado UsuarioModificador = db.Empleados.Where(x => x.Id == Id).FirstOrDefault();
                        model.A_UsuarioModifica = UsuarioModificador.NroEmpleado;
                        model.A_Modificacion = DateTime.Now;
                    }
                    _repo.Editar(model);

                    return RedirectToAction("Index2");
                }
            }
            catch
            {

            }
            return View();
        }
        //
        // GET: /Empleado/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Empleado/Delete/5

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
        public String ObtenerQREmpleado(string id)
        {
            try
            {
                var idempleado = Convert.ToInt32(id);
                using (var db = new AutogestionContext())
                {

                    var model_empleado = db.Empleados.FirstOrDefault(x => x.Id == idempleado);

                    if (model_empleado != null)


                    {

                        var personal = db.Empleados.FirstOrDefault(x=> x.Documento == model_empleado.Documento);
                        string cargo = "";
                        if (personal != null){
                        cargo = personal.Cargo;
                        }

                        string textoqr = model_empleado.NroEmpleado + "|" + model_empleado.Nombres + "|" + model_empleado.Documento + "|" + model_empleado.Empresa + "|" + cargo;
                        byte[] ImagenQR = _servicios.GenerarQREmpleado(textoqr);
                        return Convert.ToBase64String(ImagenQR);
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
        public String ObtenerFotoEmpleado(string id)
        {
           try
            {
                Byte[] b = null;
                try
                {
                    b = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/AnexosFotos/" + id + ".png"));
                }
                catch {
                    try
                    {
                        b = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/AnexosFotos/" + id + ".jpg"));
                    }
                    catch
                    {
                        try
                        {
                            b = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/AnexosFotos/" + id + ".jpeg"));
                        }
                        catch
                        {


                        }

                    }
                
                }
               

                return Convert.ToBase64String(b);
                
            }
            catch (Exception e)
            {

                return "error";
            }


        }

            [HttpPost]
            public String EnviarEmailFirma(int id)
            {
                List<DetalleEntregaEPP> entregas = new List<DetalleEntregaEPP>();

                using (var db = new AutogestionContext())
                {

                    entregas = db.DetalleEntregaEPP.Where(x => x.NumeroEntrega == id).ToList();

                    foreach(var item in entregas){
                    
                    
                    }


                }


                return "OK";
            }

        public ActionResult SoporteHojaVida(int id)
        {
            ViewBag.TiposSoporte = _repsoportes.ListarTipoSoporte();
            var soportes = _repsoportes.ObtenerSoportes(id);
            return View(soportes);
        }

        [HttpPost]
        public ActionResult GuardarSoporte(SoportesHojaDeVida soportehv) {


            SoportesHojaDeVida soporte = new SoportesHojaDeVida();
            soporte.Archivo = System.Web.HttpContext.Current.Request.Files["Archivo"];
            soporte.EmpleadoId = Convert.ToInt32(System.Web.HttpContext.Current.Request.Params["EmpleadoId"].ToString());
            soporte.Titulo = System.Web.HttpContext.Current.Request.Params["Titulo"];
            soporte.TipoSoporteId = Convert.ToInt32(System.Web.HttpContext.Current.Request.Params["TipoSoporte"].ToString());
            soporte.NombreArchivo = System.Web.HttpContext.Current.Request.Params["NombreArchivo"].ToString();
            _repsoportes.GuardarSoporte(soporte);

            return RedirectToAction("Index", "Home");
        }

        //public ActionResult SoporteHojaVida()
        //{
        //     // Llamada al método para obtener los tipos de soporte y asignarlos a ViewBag

        //    return View();
        //}

    }

}

