using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Adm_AutoGestion.Services;
using Adm_AutoGestion.Models;
using System.Text.RegularExpressions;

namespace Adm_AutoGestion.Controllers
{
    public class TercerosController : Controller
    {
        private TerceroRepository _repo;
        public TercerosController() 
        {
            _repo = new TerceroRepository();
        }
        //
        // GET: /Terceros/
        public ActionResult Index()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("Tercero"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var model = _repo.ObtenerTodos();

            return View(model);
        }

        //
        // GET: /Terceros/Details/5
        public ActionResult Details(int id)
        {
            using (var db = new AutogestionContext())
            {
                Tercero tercero = db.Tercero.Find(id);

                if (tercero == null)
                {
                    return HttpNotFound();
                }
                return View(tercero);
            }
        }
        
        public class resultado
        {

            public string CodigoEmpleado { get; set; }
            public string Nombres { get; set; }
        }
        //
        // GET: /Terceros/Create
        public ActionResult Create()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("TerceroCrear"))
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

                var lista2 = db.PersonalActivo.Select(x => new { x.Cargo }).GroupBy(b => b.Cargo).ToList();
                foreach (var x in lista2)
                {
                    lst2.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                }

                ViewBag.Cargo = lst2;


                string consulta = "select PersonalActivoes.Nombres, PersonalActivoes.CodigoEmpleado from PersonalActivoes where  PersonalActivoes.CodigoEmpleado in (SELECT  PersonalActivoes.Jefe FROM PersonalActivoes WHERE PersonalActivoes.Jefe <>''group by PersonalActivoes.Jefe)";
                var resultados = db.Database.SqlQuery<resultado>(consulta).ToList();

                ViewBag.Jefe = resultados;

            }

            return View();
        }

        //
        // POST: /Terceros/Create
        [HttpPost]
        public ActionResult Create(Tercero model, string PrimerApellido, string SegundoApellido, string PrimerNombre, string SegundoNombre)
        {
            var message = "";
            try
            {
                DateTime? FechaN = model.FechaNacimiento;
                DateTime? Fecha1 = model.FechaIngreso;
                DateTime? Fecha2 = model.FechaFin;




                if (FechaN != null)
                {
                    var a = Convert.ToDateTime(FechaN);
                    var ValidacionFN = VerificarFechaN(a);
                    if(ValidacionFN !="Válido")
                    {
                        message = "La Fecha de Nacimiento no es valida: "+ValidacionFN;
                        //return View(message);
                    }
                }
                else
                {
                    message = "La Fecha de Nacimiento no Puede estar vacía";
                    //return View(message);
                }

                if (model.Estudiante == "SI")
                {
                    if (Fecha1 != null && Fecha2 != null)
                    {
                        var b = Convert.ToDateTime(Fecha1);
                        var c = Convert.ToDateTime(Fecha2);
                        var ValidacionFecha = VerificarFechaEs(b, c);
                    }
                    else
                    {
                        message = message+"//"+" Las fechas de ingreso y salida deben ser diligenciadas";
                    }
                }
                
                if (model.Documento == null || model.Documento == "") { message = message + "//" + "El campo Documento es Obligatorio"; }
               
                if (model.Telefono == null || model.Telefono == "") { message = message + "//" + "El campo Teléfono es Obligatorio"; }

                if (model.CorreoPersonal == null || model.CorreoPersonal == "") { message = message + "//" + "El campo Correo Electónico es Obligatorio"; }
               var correcto = email_bien_escrito(model.CorreoPersonal);
               if ( correcto == false ) { message = message+"//"+ "El correo no cumple con los parámetros"; }

               if ( (PrimerNombre == null || PrimerNombre == "") && (PrimerApellido == null || PrimerApellido == "") ){message=message+"//"+ "El primero nombre y el Primer Apellido son Obligatorios";}
               
               if (model.Area==""||model.Area==null) {message=message+"//"+ "El campo Area es Obligatorio";}

               if (model.Superior == "" || model.Superior == null){message = message + "//" + "El Campo superior es obligatorio"; }

                model.Nombres = (PrimerApellido + " " + SegundoApellido + " " + PrimerNombre + " " + SegundoNombre).ToUpper();
                        var id = Session["Empleado"];
                        var emplog = Convert.ToInt32(id);

                if(message==""){
                    _repo.Crear(model, emplog);
                    return RedirectToAction("Index");
                }
                    
                    
             
            }
            catch(SystemException Ex)
            {
                message = "Error: " + Ex;
                Session["message"] = message;
                return RedirectToAction("Create");
            }
            Session["message"] = message;
            return RedirectToAction("Create");
        }

        //
        // GET: /Terceros/Edit/5
        public ActionResult Edit(int id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("EditarTercero"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            List<SelectListItem> lst = new List<SelectListItem>();
            List<SelectListItem> lst2 = new List<SelectListItem>();
            using (var db = new AutogestionContext())
            {
                Tercero Tercero = db.Tercero.Find(id);
                if (Tercero == null)
                {
                    return HttpNotFound();
                }
                var Nombres= Tercero.Nombres.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);;
                ViewBag.Apellido1 = Nombres[0];
                ViewBag.Apellido2 = Nombres[1];
                ViewBag.Nombre1 = Nombres[2];
                ViewBag.Nombre2 = Nombres[3];

                var lista = db.PersonalActivo.Select(x => new { x.Area }).GroupBy(b => b.Area).ToList();
                foreach (var x in lista)
                {
                    if (x.Key.ToString() == Tercero.Area)
                    {
                        lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString(), Selected = true });
                    }
                    else { lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() }); }
                }

                ViewBag.Area = lst;

                var lista2 = db.PersonalActivo.Select(x => new { x.Cargo }).GroupBy(b => b.Cargo).ToList();
                foreach (var x in lista2)
                {
                    if (x.Key.ToString() == Tercero.Cargo)
                    {
                        lst2.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString(), Selected = true });
                    }
                    else
                    {
                        lst2.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                    }

                }

                ViewBag.Cargo = lst2;


                string consulta = "select PersonalActivoes.Nombres, PersonalActivoes.CodigoEmpleado from PersonalActivoes where  PersonalActivoes.CodigoEmpleado in (SELECT  PersonalActivoes.Jefe FROM PersonalActivoes WHERE PersonalActivoes.Jefe <>''group by PersonalActivoes.Jefe)";
                var resultados = db.Database.SqlQuery<resultado>(consulta).ToList();
                ViewBag.Jefe = resultados;

                return View(Tercero);
            }
           
        }

        //
        // POST: /Terceros/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Tercero model,string Cargo2,string Area2,string PrimerNombre,string SegundoNombre,string PrimerApellido,string SegundoApellido)
        {
         
                // TODO: Add update logic here
                var message = "";
            try
            {
                DateTime? FechaN = model.FechaNacimiento;
                DateTime? Fecha1 = model.FechaIngreso;
                DateTime? Fecha2 = model.FechaFin;




                if (FechaN != null)
                {
                    var a = Convert.ToDateTime(FechaN);
                    var ValidacionFN = VerificarFechaN(a);
                    if(ValidacionFN !="Válido")
                    {
                        message = "La Fecha de Nacimiento no es valida: "+ValidacionFN;
                        //return View(message);
                    }
                }
                else
                {
                    message = "La Fecha de Nacimiento no Puede estar vacía";
                    //return View(message);
                }

                if (model.Estudiante == "SI")
                {
                    if (Fecha1 != null && Fecha2 != null)
                    {
                        var b = Convert.ToDateTime(Fecha1);
                        var c = Convert.ToDateTime(Fecha2);
                        var ValidacionFecha = VerificarFechaEs(b, c);
                    }
                    else
                    {
                        message = message+"//"+" Las fechas de ingreso y salida deben ser diligenciadas";
                    }
                }
                
                if (model.Documento == null || model.Documento == "") { message = message + "//" + "El campo Documento es Obligatorio"; }
               
                if (model.Telefono == null || model.Telefono == "") { message = message + "//" + "El campo Teléfono es Obligatorio"; }

                if (model.CorreoPersonal == null || model.CorreoPersonal == "") { message = message + "//" + "El campo Correo Electónico es Obligatorio"; }
               var correcto = email_bien_escrito(model.CorreoPersonal);
               if ( correcto == false ) { message = message+"//"+ "El correo no cumple con los parámetros"; }

               if ( (PrimerNombre == null || PrimerNombre == "") && (PrimerApellido == null || PrimerApellido == "") ){message=message+"//"+ "El primero nombre y el Primer Apellido son Obligatorios";}
               
               if (model.Area==""||model.Area==null) {message=message+"//"+ "El campo Area es Obligatorio";}

               if (model.Superior == "" || model.Superior == null){message = message + "//" + "El Campo superior es obligatorio"; }
                if (ModelState.IsValid)
                {
                    var Id = Session["Empleado"];
                    var emplog = Convert.ToInt32(Id);
                    model.FechaModifica = DateTime.Now;
                    if(model.Area!=Area2)
                    {
                        model.Area = Area2;
                    }
                    if(model.Cargo!=Cargo2)
                    {
                        model.Cargo = Cargo2;
                    }
                    var Nombres = (PrimerApellido + " " + SegundoApellido + " " + PrimerNombre + " " + SegundoNombre).ToUpper();
                    if (model.Nombres != Nombres)
                    {
                        model.Nombres = Nombres;
                    }
                    if (message == "")
                    {
                        _repo.Editar(model, emplog);
                    }
                    else
                    {
                        Session["message"] = message;
                        return RedirectToAction("Edit/"+model.Id);                    
                    }
                    return RedirectToAction("Index");
                }
                
            }
            catch (SystemException ex)
            {
                
            }
            return View();
        }

        //
        // GET: /Terceros/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Terceros/Delete/5
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

        public string VerificarFechaEs(DateTime Fecha1, DateTime Fecha2)
        {
            var Validar = "Válido";
            //var edad = DateTime.Today.AddTicks(-Fecha1.Ticks).Year - 1;
            if (Fecha1 >= Fecha2) {Validar="//La Fecha de ingreso no puede se mayor que fecha de salida"; }

            return Validar;

        }
        public string VerificarFechaN(DateTime FechaNac)
        {
            var Validar = "Válido";
            if (FechaNac > DateTime.Now)
            {
                Validar = " " + "La Fecha no puede ser Mayor a la fecha actual";
            }
            else
            {
                var edad = DateTime.Today.AddTicks(-FechaNac.Ticks).Year - 1;
                if (edad < 14)
                {
                    Validar = " " + "Edad insuficiente";
                }
            }
            
            return Validar;

        }

        private Boolean email_bien_escrito(String email)
        {
            String expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
