using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Adm_AutoGestion.Services;
using Adm_AutoGestion.Models;
using System.Web.Script.Serialization;
using System.Data.Entity;
using System.Data;
using SAP.Middleware.Connector;
using SAPExtractorDotNET;
using System.Text;


namespace Adm_AutoGestion.Controllers

{


    public class resultado
    {
        public int Id { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int TipoRegistro { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string IdIndicadorActualizacion { get; set; }
       
         
    }
    public class DetallePersonalVacunaController : Controller
    {
        //
        // GET: /DetallePersonalSalud/
        private DetallePersonalVacunaRepository _repo;


        public DetallePersonalVacunaController()
        {
            _repo = new DetallePersonalVacunaRepository();
           
        }


        

        public ActionResult Index()

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



             using (var db = new AutogestionContext())
         {


           // List<resultado> consulta = db.DetallePersonalSalud.Select(e => new { e.NumeroIdentificacion, e.PrimerNombre }).ToList();

            string  consulta = "select Id, FechaRegistro,TipoRegistro, NumeroIdentificacion,PrimerApellido, SegundoApellido,PrimerNombre, SegundoNombre, IdIndicadorActualizacion from DetallePersonalSaluds union all  select Id, FechaRegistro, TipoRegistro, NumeroIdentificacion,PrimerApellido, SegundoApellido, PrimerNombre, SegundoNombre,IdIndicadorActualizacion from DetalleApoyoLogisticoSaluds";
            var resultados = db.Database.SqlQuery<resultado>(consulta).ToList();
            return View (resultados) ; 




           // var model = _repo.ObtenerActivos();
           //  return View(resultados);


        }
        }




        
        public ActionResult RegistrarPersonalVacuna(string documento, string nombre) {
        

            var lista = nombre.Split(' ');
            var Papellido = lista[0];
            ViewBag.Papellido = Papellido;

            var Sapellido = lista[1];
            ViewBag.Sapellido = Sapellido;

            var Pnombre = lista[2];
            ViewBag.Pnombre = Pnombre;
            var Snombre = lista[3];
            ViewBag.Snombre = Snombre;

            ViewBag.documento = documento; 
          

            var model = new DetallePersonalSalud();
            using (var db = new AutogestionContext()) {

                model.ListadoTipoDocumento = db.TipoDocumento.ToList();
                model.ListadoAreaCovid = db.AreaCovid.ToList();
                model.ListadoTiempoDedicacion = db.TiempoDedicacion.ToList();
                model.ListadoActualizacionRegistro = db.IndicadorActualizacionRegistro.ToList();

                var perfil = db.THSPerfil.Select(e => new { e.Id, e.Nombre, nomcodigo = e.Id + "-" + e.Nombre }).ToList();

                ViewBag.perfil = perfil;

                var servicio = db.ServiciosTHSCovid.Select(e => new { e.Id, e.Nombre, nomservicio = e.Id + "-" + e.Nombre }).ToList();
                ViewBag.servicio = servicio;
           
                
            }

            return View(model); 
        
        }


        public ActionResult RegistrarVacuna(string documento, string nombre) {
            var model = new DetallePersonalSalud();
            return View(model); 
        }

        [HttpPost]
        public JsonResult llenarcargoAsistencial() {

            using (AutogestionContext db = new AutogestionContext()) {

                var cargoasistencial = db.CargoAsistencial.ToList();

                return Json(cargoasistencial); 
            }
 
        }


        

        [HttpPost]
        public JsonResult llenarcargoApoyo()
        {

            using (AutogestionContext db = new AutogestionContext())
            {

                var cargoapoyo = db.CargoApoyo.ToList();

                return Json(cargoapoyo);
            }

        }



        [HttpPost]
        public JsonResult llenarEmpleado(int Id)
        {

            using (AutogestionContext db = new AutogestionContext())
            {

                var empleado = db.Empleados.Where(e => e.Id == Id).ToList(); 

                return Json(empleado);
            }

        }
        



        //
        // GET: /DetallePersonalSalud/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /DetallePersonalSalud/Create

        //public ActionResult Create(string documento, string nombre, string sociedad)
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



            var model = new DetallePersonalSalud();
            using (var db = new AutogestionContext())
            {

                model.ListadoTipoDocumento = db.TipoDocumento.ToList();
                model.ListadoAreaCovid = db.AreaCovid.ToList();
                model.ListadoTiempoDedicacion = db.TiempoDedicacion.ToList();
                model.ListadoActualizacionRegistro = db.IndicadorActualizacionRegistro.ToList();
                model.ListadoEmpleados = db.Empleados.Where(e => e.Activo == "SI").ToList();

                var perfil = db.THSPerfil.Select(e => new { e.Id, e.Nombre, nomcodigo = e.Id + "-" + e.Nombre }).ToList();

                ViewBag.perfil = perfil;

                var servicio = db.ServiciosTHSCovid.Select(e => new { e.Id, e.Nombre, nomservicio = e.Id + "-" + e.Nombre }).ToList();
                ViewBag.servicio = servicio;
              


            }

            return View(model); 



        }

        //
        // POST: /DetallePersonalSalud/Create

             [HttpPost]
             public ActionResult Create(DetallePersonalSalud model)
             {

                 using (var db = new AutogestionContext())
                 {


                     string message = "";
                     try
                     {

                         if (model.IdTipoDocumento == null || model.TipoRegistro == 0 || model.IdPerfilProfesional == null || model.IdServicioCovid == null || model.IdDedicacion == null || model.IdServicioCovid == null)
                         {
                             Session["message"] = "Debe Seleccionar Registros";
                             return RedirectToAction("Create");
                         }


                         var consulta = db.DetallePersonalSalud.Where(e => e.NumeroIdentificacion == model.NumeroIdentificacion).Count();

                         var consulta1 = db.DetalleApoyoLogisticoSalud.Where(e => e.NumeroIdentificacion == model.NumeroIdentificacion).Count();

                         var resultado = consulta + consulta1;

                         if (resultado > 0) {

                             Session["message"] = "Ya se encuentra registrado el colaborador para  vacuna";
                             return RedirectToAction("Create");
                         
                         }

                         
                        


                         if (model.Empresa == "2000")
                         {
                             model.CodigoEntidad = "682760442801";
                             model.NombreEntidad = "FUNDACION FOSUNAB";
                         }

                         if (model.Empresa == "1000")
                         {
                             model.CodigoEntidad = "682760166601";
                             model.NombreEntidad = "FUNDACION OFTALMOLOGICA DE SANTANDER";
                         }

                         int IdUsuarioR = 0;
                         string registra = String.Format("{0}", Session["Empleado"]);
                         Int32.TryParse(registra, out IdUsuarioR);
                         model.UsuarioRegistraId = IdUsuarioR;
                         model.FechaRegistro = DateTime.Now;
                         model.CodigoMunicipio = "68276";
                         model.UsuarioModificaId = null;
                         model.FechaModifica = null;
                         model.IdIndicadorActualizacion = "I";



                         message = _repo.Crear(model);


                         return RedirectToAction("Index");
                     }
                     catch (SystemException ex)
                     {

                         message = String.Format("Se genero un error. {0}", ex.Message);
                         return View();
                     }

                     //return RedirectToAction("Index");
                 }
             }
        

        //
        // GET: /DetallePersonalSalud/Edit/5

        public ActionResult Edit(int id, int Tipo)

        {
            
            using (var db = new AutogestionContext())
            {
                var perfil = db.THSPerfil.Select(e => new { e.Id, e.Nombre, nomcodigo = e.Id + "-" + e.Nombre }).ToList();

                ViewBag.perfil = perfil;

                var servicio = db.ServiciosTHSCovid.Select(e => new { e.Id, e.Nombre, nomservicio = e.Id + "-" + e.Nombre }).ToList();
                ViewBag.servicio = servicio;
                

               
                
                if (Tipo == 2)
                {

                    DetallePersonalSalud model = db.DetallePersonalSalud.Find(id);

                    if (model == null)
                    {
                        return HttpNotFound();
                    }

                    model.ListadoTipoDocumento = db.TipoDocumento.ToList();
                    model.ListadoAreaCovid = db.AreaCovid.ToList();
                    model.ListadoTiempoDedicacion = db.TiempoDedicacion.ToList();
                    model.ListadoActualizacionRegistro = db.IndicadorActualizacionRegistro.ToList();



                    var cargo = db.CargoAsistencial.Select(e => new { e.Id, e.Nombre, nomcargo = e.Id + "-" + e.Nombre }).ToList();
                    ViewBag.cargo = cargo;
                    
                    return View(model);
                }

                if (Tipo == 3)
                {

                    DetalleApoyoLogisticoSalud model = db.DetalleApoyoLogisticoSalud.Find(id);

                    if (model == null)
                    {
                        return HttpNotFound();
                    }

                    //model.ListadoTipoDocumento = db.TipoDocumento.ToList();
                    //model.ListadoAreaCovid = db.AreaCovid.ToList();
                    //model.ListadoTiempoDedicacion = db.TiempoDedicacion.ToList();
                    //model.ListadoActualizacionRegistro = db.IndicadorActualizacionregistro.ToList();

                    DetallePersonalSalud model1 = new DetallePersonalSalud();
                    model1.TipoRegistro = model.TipoRegistro;
                    model1.Empresa = model.Empresa;
                    model1.IdTipoDocumento = model.IdTipoDocumento;
                    model1.NumeroIdentificacion = model.NumeroIdentificacion;
                    model1.PrimerApellido = model.PrimerApellido;
                    model1.SegundoApellido = model.SegundoApellido;
                    model1.PrimerNombre = model.PrimerNombre;
                    model1.SegundoNombre = model.SegundoNombre; 
                    model1.IdCargo =  model.IdCargo;
                    model1.IdServicioCovid = model.IdServicioCovid;
                    model1.IdAreaCovid = model.IdAreaCovid;
                    model1.IdDedicacion = model.IdDedicacion;
                    model1.ListadoTipoDocumento = db.TipoDocumento.ToList();
                    model1.ListadoAreaCovid = db.AreaCovid.ToList();
                    model1.ListadoTiempoDedicacion = db.TiempoDedicacion.ToList();
                    model1.ListadoActualizacionRegistro = db.IndicadorActualizacionRegistro.ToList();
                    //model1.ListadoCargoApoyo = db.CargoApoyo.ToList(); 


                    var cargo = db.CargoApoyo.Select(e => new { e.Id, e.Nombre, nomcargo = e.Id + "-" + e.Nombre }).ToList();
                    ViewBag.cargo = cargo;
                  
                    return View(model1);
                }



                return View(); 
              
                }

            
        }

        //
        // POST: /DetallePersonalSalud/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, DetallePersonalSalud model )
        {


            string message = "";
            try
            {

                if (model.IdIndicadorActualizacion == "E")
                {


                    message =  "No es posible modificar  el registro debido a que se encuentra en estado " + model.IdIndicadorActualizacion + ".";
                  
                    return RedirectToAction("Index");

                }
                else
                {

                    int IdUsuarioR = 0;
                    string registra = String.Format("{0}", Session["Empleado"]);
                    Int32.TryParse(registra, out IdUsuarioR);
                    model.UsuarioModificaId = IdUsuarioR;
                    model.FechaModifica = DateTime.Now;
                    model.IdIndicadorActualizacion = "A";


                    message = _repo.Editar(id, model);



                    return RedirectToAction("Index");
                }  
            }
            catch
            {
                return View();
            }

            
        }



        [HttpPost]
        public String AnularRegistro(int id)
        {

            string message = "";

             using (var db = new AutogestionContext())
            {
            try
            {

                 DetallePersonalSalud detalle = new DetallePersonalSalud();
                    detalle = db.DetallePersonalSalud.FirstOrDefault(e => e.Id == id);

                if (detalle.IdIndicadorActualizacion == "E")
                {

                    return "No es posible Anular el registro debido a que se encuentra en estado " + detalle.IdIndicadorActualizacion + ".";
                }
                else {

                    int IdUsuarioR = 0;
                    string registra = String.Format("{0}", Session["Empleado"]);
                    Int32.TryParse(registra, out IdUsuarioR);
                    detalle.UsuarioEliminaId = IdUsuarioR;
                    detalle.FechaElimina = DateTime.Now;
                    detalle.IdIndicadorActualizacion = "E";


                      message = _repo.Eliminar(id, detalle);

                      return message; 
                 
                
                }                

            }
            catch (Exception ex)
            {
                return ex.Message.ToString(); 
            }

             }
        }

        //
        // GET: /DetallePersonalSalud/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /DetallePersonalSalud/Delete/5

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
