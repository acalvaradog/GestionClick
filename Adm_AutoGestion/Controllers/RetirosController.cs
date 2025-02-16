using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Adm_AutoGestion.Controllers
{
    public class RetirosController : Controller
    {


         private RetirosRepository _repo;

         public RetirosController()
        {
            _repo = new RetirosRepository();

        }



        //
        // GET: /Retiros/

         public ActionResult Index(string IniContraFI, string IniContraFF, string FechaTermFI, string FechaTermFF, string CodigoEmpleado, string NombreEmpleado, string MotivoCancelacion, string TipoContrato, string Liquidacion, string Estado, string message,string Empresa, string Cargo)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("Retiros"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            List<Retiros> Items = new List<Retiros>();

            using (var db = new AutogestionContext())
            {


            int empleado = 0;
            string usuario = String.Format("{0}", Session["Empleado"]);
            Int32.TryParse(usuario, out empleado);
            var datos = db.Empleados.FirstOrDefault(e => e.Id == empleado);
            ViewBag.Empresa = datos.Empresa;


            if (String.IsNullOrEmpty(IniContraFI)) { IniContraFI = ""; }
            if (String.IsNullOrEmpty(IniContraFF)) { IniContraFF = ""; }
            if (String.IsNullOrEmpty(FechaTermFI)) { FechaTermFI = ""; }
            if (String.IsNullOrEmpty(FechaTermFF)) { FechaTermFF = ""; }
            if (String.IsNullOrEmpty(CodigoEmpleado)) { CodigoEmpleado = ""; }
            if (String.IsNullOrEmpty(NombreEmpleado)) { NombreEmpleado = ""; }
            if (String.IsNullOrEmpty(MotivoCancelacion)) { MotivoCancelacion = ""; }
            if (String.IsNullOrEmpty(TipoContrato)) { TipoContrato = ""; }
            if (String.IsNullOrEmpty(Liquidacion)) { Liquidacion = ""; }
            if (String.IsNullOrEmpty(Estado)) { Estado = ""; }
            if (String.IsNullOrEmpty(Empresa)) { Empresa = ""; }

             var concat = IniContraFI + IniContraFF + FechaTermFI + FechaTermFF + CodigoEmpleado + NombreEmpleado + MotivoCancelacion + TipoContrato + Liquidacion + Estado + Empresa;

            

            Session["filtros"] = String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", IniContraFI, IniContraFF, FechaTermFI, FechaTermFF, CodigoEmpleado, NombreEmpleado, MotivoCancelacion, TipoContrato, Liquidacion, Estado, Empresa, Cargo);

             
             if (concat != "")
             {

                 

                     List<SelectListItem> lst = new List<SelectListItem>();
                     var lista = db.Empleados.Where(x => x.Cargo != null).Select(x => new { x.Cargo }).GroupBy(b => b.Cargo).ToList();
                     foreach (var x in lista)
                     {
                         lst.Add(new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
                     }

                     ViewBag.Cargos = lst;

                     


                     DateTime Fecha1IC = DateTime.Now;
                     DateTime Fecha2IC = DateTime.Now;
                     DateTime Fecha1FT = DateTime.Now;
                     DateTime Fecha2FT = DateTime.Now;

                     if (IniContraFI != "" && IniContraFF != "" || FechaTermFI != "" && FechaTermFF != "")
                     {

                         if (!DateTime.TryParse(IniContraFI, out Fecha1IC))
                         {
                             Fecha1IC = new DateTime();
                         }

                         if (!DateTime.TryParse(IniContraFF, out Fecha2IC))
                         {
                             Fecha2IC = DateTime.Now;
                         }


                         if (!DateTime.TryParse(FechaTermFI, out Fecha1FT))
                         {
                             Fecha1FT = new DateTime();
                         }

                         if (!DateTime.TryParse(FechaTermFF, out Fecha2FT))
                         {
                             string fec = "31/12/9999";
                             Fecha2FT = DateTime.Parse(fec);

                         }



                         Items = db.Retiros.Where(e => e.CodigoEmpleado.Contains(CodigoEmpleado) &&
                                                        DbFunctions.TruncateTime(e.Fecha) >= Fecha1IC &&
                                                        DbFunctions.TruncateTime(e.Fecha) <= Fecha2IC &&
                                                        DbFunctions.TruncateTime(e.FechaTerminacion) >= Fecha1FT &&
                                                        DbFunctions.TruncateTime(e.FechaTerminacion) <= Fecha2FT &&
                                                        e.Empleado.Nombres.Contains(NombreEmpleado) &&
                                                        e.Motivos.Nombre.Contains(MotivoCancelacion) &&
                                                        e.TipoContrato.Contains(TipoContrato) &&
                                                        e.Liquidacion.Contains(Liquidacion) &&
                                                        e.Estado.Contains(Estado) &&
                                                        e.Empleado.Empresa.Contains(Empresa)
                             //&&
                             //e.Empleado.Cargo == Cargo 
                                                       ).ToList();

                     }
                     else
                     {

                         Items = db.Retiros.Where(e => e.CodigoEmpleado.Contains(CodigoEmpleado) &&
                                                        e.Empleado.Nombres.Contains(NombreEmpleado) &&
                                                        e.Motivos.Nombre.Contains(MotivoCancelacion) &&
                                                        e.TipoContrato.Contains(TipoContrato) &&
                                                        e.Liquidacion.Contains(Liquidacion) &&
                                                        e.Estado.Contains(Estado) &&
                                                        e.Empleado.Empresa.Contains(Empresa)
                             //&&
                             //e.Empleado.Cargo == Cargo 


                                                       ).ToList();


                     }
                     foreach (Retiros Item in Items)
                     {
                         Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.IdEmpleado);
                         Item.Motivos = db.Motivos.FirstOrDefault(e => e.Id == Item.MotivoCancelacion);
                         Item.EmpleadoRegistra = db.Empleados.FirstOrDefault(e => e.Id == Item.UsuarioRegistra);
                         Item.EmpleadoModif = db.Empleados.FirstOrDefault(e => e.Id == Item.UsuarioModifica);
                     }


                 }
             }
            var Msg = TempData["EnvioEncuesta"] as string;
            var EncuestaId = TempData["EncuestaId"] as int?;
            ViewBag.Encuesta = Msg;
            ViewBag.EncuestaId = EncuestaId;
            Session["message"] = message;
             return View(Items);
        }

        //
        // GET: /Retiros/Details/5

        public ActionResult Details(int Id)
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

        public ActionResult DetalleRetiro(int Id)
        {
            //List<Retiros> Items = new List<Retiros>();
            using (var db = new AutogestionContext())
            {
                //Items = db.Retiros.Where(e => e.Id == Id).ToList();
                Retiros Items = db.Retiros.Find(Id);
                //foreach (Retiros Item in Items)
                //{
                    Items.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Items.IdEmpleado);
                    Items.Motivos = db.Motivos.FirstOrDefault(e => e.Id == Items.MotivoCancelacion);
                    Items.EmpleadoRegistra = db.Empleados.FirstOrDefault(e => e.Id == Items.UsuarioRegistra);
                    Items.EmpleadoModif = db.Empleados.FirstOrDefault(e => e.Id == Items.UsuarioModifica);
                //}

                if (Items == null)
                {
                    return HttpNotFound();
                }
                return View(Items);
            }
        
        }

        public ActionResult DetalleRetiro2(int Id)
        {
            //List<Retiros> Items = new List<Retiros>();
            using (var db = new AutogestionContext())
            {
                //Items = db.Retiros.Where(e => e.Id == Id).ToList();
                Retiros Items = db.Retiros.Find(Id);
                //foreach (Retiros Item in Items)
                //{
                Items.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Items.IdEmpleado);
                Items.Motivos = db.Motivos.FirstOrDefault(e => e.Id == Items.MotivoCancelacion);
                Items.EmpleadoRegistra = db.Empleados.FirstOrDefault(e => e.Id == Items.UsuarioRegistra);
                Items.EmpleadoModif = db.Empleados.FirstOrDefault(e => e.Id == Items.UsuarioModifica);
                //}

                if (Items == null)
                {
                    return HttpNotFound();
                }
                return PartialView(Items);
            }

        }

        //
        // GET: /Retiros/Create

        public ActionResult Create(string Id)
        {
            Retiros model = new Retiros();
            Empleado empleado = new Empleado();
            string message = "";
            Session.Remove("Nombres");
            using (var db = new AutogestionContext())
            {

                model.ListadoMotivos = db.Motivos.Where(e => e.Activo == true).ToList();
                empleado = db.Empleados.FirstOrDefault(s => s.NroEmpleado == Id);
            }

            if (Id != null )
            {
                if (Id != "") { 
                var size = Id.Length;

                if (size == 8)
                {

                    DataTable Datos = new DataTable();
                    var Result = "";
                    var datos2 = new Retiros();

                    InMemoryDestinationConfiguration DestinacionConfiguracion = new InMemoryDestinationConfiguration();

                    String contraseña = Properties.Settings.Default.Contraseña.ToString();
                    var encodedTextBytes = Convert.FromBase64String(contraseña);

                    contraseña = Encoding.UTF8.GetString(encodedTextBytes);

                    RfcConfigParameters config = new RfcConfigParameters();

                    try
                    {

                        config.Add(RfcConfigParameters.Name, "SAP");
                        config.Add(RfcConfigParameters.AppServerHost, Properties.Settings.Default.Servidor.ToString());
                        config.Add(RfcConfigParameters.SystemNumber, Properties.Settings.Default.Id.ToString());
                        config.Add(RfcConfigParameters.User, Properties.Settings.Default.Usuario.ToString());
                        config.Add(RfcConfigParameters.Password, contraseña);
                        config.Add(RfcConfigParameters.Client, Properties.Settings.Default.Mandante.ToString());
                        config.Add(RfcConfigParameters.Language, "ES");
                        config.Add(RfcConfigParameters.SAPRouter, Properties.Settings.Default.Saprouter.ToString());
                        config.Add(RfcConfigParameters.LogonGroup, "Foscal");

                        RfcDestinationManager.RegisterDestinationConfiguration(DestinacionConfiguracion);
                        DestinacionConfiguracion.AddOrEditDestination(config);
                        RfcDestination destination = RfcDestinationManager.GetDestination("SAP");

                        RfcRepository repository = destination.Repository;
                        IRfcFunction function = repository.CreateFunction("ZMF_ASIGNACION_ORGANIZ_3000");
                        //PARAMETROS IMPORT
                        function.SetValue("I_PERNR", Id);

                        function.Invoke(destination);
                        //OBTENER RESPUESTA 

                       
                            string PrimerApellido = function.GetStructure("T_RESULT").GetString("NACHN");
                            string SegundoApellido = function.GetStructure("T_RESULT").GetString("NACH2");
                            string PrimerNombre = function.GetStructure("T_RESULT").GetString("VORNA");
                            string SegundoNombre = function.GetStructure("T_RESULT").GetString("NAME2");
                            string TipoContrato = function.GetStructure("T_RESULT").GetString("CTTXT");
                            string InicioContrato = function.GetStructure("T_RESULT").GetString("BEGDA");
                            string AreaPersonal = function.GetStructure("T_RESULT").GetString("DPTO");
                            string NroEmpleado = function.GetStructure("T_RESULT").GetString("PERNR");


                            if (PrimerNombre != "")
                            {

                            DateTime enteredDate = DateTime.Parse(InicioContrato);

                            model.CodigoEmpleado = NroEmpleado;
                            model.TipoContrato = TipoContrato;
                            model.InicioContrato = enteredDate;
                            model.AreaPersonal = AreaPersonal;

                            Session["Nombres"] = PrimerNombre + " " + SegundoNombre + " " + PrimerApellido + " " + SegundoApellido;
                            ViewBag.Cargo = empleado.Cargo;
                            ViewBag.Telefono = empleado.Telefono;
                            ViewBag.CorreoI = empleado.Correo;
                            ViewBag.CorreoP = empleado.CorreoPersonal;
                        }
                        else {
                            message = "El usuario no se encuentra en SAP.";
                        }

                    }
                    catch (SystemException ex)
                    {
                        // ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
                    }
                    catch (RfcLogonException ex)
                    {
                        message = "A ocurrido un error. " + ex;
                    }
                    catch (RfcAbapException ex)
                    {
                        //ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
                    }
                    finally
                    {
                        RfcDestinationManager.UnregisterDestinationConfiguration(DestinacionConfiguracion);
                        // DestinacionConfiguracion.RemoveDestination("SAP");
                    }

                }
                else {
                    message = "El codigo del empleado no puede ser mayor o menor a 8 digitos.";
                }


            }else{
                message = "Debe digitar el codigo del empleado para realizar la busqueda.";
            }

            }

            Session["message"] = message;
            return View(model);
        }

        
        //public string firmaps(int id)
        //{
        // string m = "";
        // string asunto = "Creación Paz y Salvo";
        // string Nombres = "TORRES DURAN MAYRA ISABEL";
        // string Documento = "1095810734";
        // string Numero = "40003700";


        // _repo.NotificarCorreoPyS(asunto, Nombres, Documento, Numero, id);
        // return m;
        //}

        //
        // POST: /Retiros/Create

        [HttpPost]
        public ActionResult Create(Retiros model, string Telefono, string CorreoI, string CorreoP)
        {
            string message = "";
            

            try
            {

                using (var db = new AutogestionContext())
                {

                    List<Retiros> Retiros = new List<Retiros>();
                    model.ListadoMotivos = db.Motivos.Where(e => e.Activo == true).ToList();

                    if (ModelState.IsValid)
                    {

                        Empleado empleado = db.Empleados.FirstOrDefault(e => e.NroEmpleado == model.CodigoEmpleado);
                        var IdUsuario = Session["Empleado"].ToString();
                        var empleadoId = Convert.ToInt32(IdUsuario);
                        if (empleado != null)
                        {
                            Retiros = db.Retiros.Where(e => e.IdEmpleado == empleado.Id && e.Estado == "RETIRO VIGENTE").ToList();


                            int IdUsuarioR = 0;
                            string registra = String.Format("{0}", Session["Empleado"]);
                            Int32.TryParse(registra, out IdUsuarioR);
                            model.UsuarioRegistra = IdUsuarioR;
                            model.IdEmpleado = empleado.Id;
                            model.Empresa = empleado.Empresa;

                            //if (Retiros.Count == 0)
                            //{

                            // TODO: Add insert logic here
                                

                            message = _repo.Crear(model, Telefono, CorreoI, CorreoP);

                            var Id = db.Retiros.Where(x=> x.CodigoEmpleado == model.CodigoEmpleado && x.Estado == "RETIRO VIGENTE" && x.UsuarioRegistra == empleadoId).FirstOrDefault();

                            TempData["EnvioEncuesta"] = message;
                            TempData["EncuestaId"] = Id.Id;
                            Session.Remove("Nombres");
                            return RedirectToAction("Index", new { message = message });


                        //}
                        //else
                        //{
                        //    message = "No es posible realizar un nuevo registro de retiro para este empleado, ya que actualmente tiene un proceso de retiro activo.";
                        //}

                    }
                    else {
                        message = "El empleado no se encuentra registrado en Autogestion.";
                    }
                    

                }
                }

            }
            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);

            }
        
            Session["message"] = message;
            return View(model);
           
            
        }

        //
        // GET: /Retiros/Edit/5

        public ActionResult Edit(int id)
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("RetirosEditar"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            using (var db = new AutogestionContext())
            {
                Motivos motivo = new Motivos();
                Retiros retiros = db.Retiros.Find(id);
                retiros.Motivos = db.Motivos.FirstOrDefault(e => e.Id == retiros.MotivoCancelacion);
                Empleado Nombre = db.Empleados.FirstOrDefault(e => e.Id == retiros.IdEmpleado);
                retiros.ListadoMotivos = db.Motivos.Where(e => e.Activo == true).ToList();
                ViewBag.Nombres = Nombre.Nombres;
                ViewBag.Estado = retiros.Estado;
                ViewBag.Liquidacion = retiros.Liquidacion;
                ViewBag.Telefono = Nombre.Telefono;
                ViewBag.CorreoI = Nombre.Correo;
                ViewBag.CorreoP = Nombre.CorreoPersonal;
                if (retiros == null)
                {
                    return HttpNotFound();
                }
                return View(retiros);
            }

        }

        //
        // POST: /Retiros/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, Retiros model, string Telefono, string CorreoI, string CorreoP)
        {
            string message = "";
            try
            {
                //if (ModelState.IsValid)
                //{

                    using (var db = new AutogestionContext())
                    {
                        Retiros retiros = db.Retiros.Find(id);

                        if (retiros.Estado == "RETIRO ANULADO" && model.Estado == "RETIRO VIGENTE")
                        {
                            message = "No se permite realizar el cambio de estado de Retiro Anulado a Retiro Vigente.";
                        }
                        else 
                        {
                            int IdUsuarioM = 0;
                            string Modifica = String.Format("{0}", Session["Empleado"]);
                            Int32.TryParse(Modifica, out IdUsuarioM);
                            model.UsuarioModifica = IdUsuarioM;
                            _repo.Editar(model, Telefono, CorreoI, CorreoP);

                            string filtros = string.Format("{0}", Session["filtros"]);

                            string[] datos = filtros.Split(',');
                            Session.Remove("filtros");
                            return RedirectToAction("Index", "Retiros", new { IniContraFI = datos[0], IniContraFF = datos[1], FechaTermFI = datos[2], FechaTermFF = datos[3], CodigoEmpleado = datos[4], NombreEmpleado = datos[5], MotivoCancelacion = datos[6], TipoContrato = datos[7], Liquidacion = datos[8], Estado = datos[9], Empresa = datos[10], Cargo = datos[11] });
                            //return RedirectToAction("Index");
                        }
                    }
                //}
            }
            catch
            {
               
            }

            Session["message"] = message;
            return View();
        }

        //
        // GET: /Retiros/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Retiros/Delete/5

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


        public static DataTable GetDataTableFromRFCTable(IRfcTable myrfcTable)
        {
            DataTable loTable = new DataTable();
            int liElement = 0;
            for (liElement = 0; liElement <= myrfcTable.ElementCount - 1; liElement++)
            {
                RfcElementMetadata metadata = myrfcTable.GetElementMetadata(liElement);
                loTable.Columns.Add(metadata.Name);
            }
            foreach (IRfcStructure Row in myrfcTable)
            {
                DataRow ldr = loTable.NewRow();
                for (liElement = 0; liElement <= myrfcTable.ElementCount - 1; liElement++)
                {
                    RfcElementMetadata metadata = myrfcTable.GetElementMetadata(liElement);
                    ldr[metadata.Name] = Row.GetString(metadata.Name);
                }
                loTable.Rows.Add(ldr);
            }
            return loTable;
        }


        public static string DataSetToJSON(DataTable dt)
        {

            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in dt.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            return jsSerializer.Serialize(parentRow);
        }

          [HttpPost]
        public String EnvioEncuesta(int id)
        {

            try
            {
                using (var db = new AutogestionContext())
                {
                    Retiros retiros = db.Retiros.Find(id);
                    Empleado Empleado = db.Empleados.Find(retiros.IdEmpleado);

                    if (Empleado.CorreoPersonal == null || Empleado.CorreoPersonal == "")
                        return "SINCORREO";
                    else
                    {



                        if (enviar_correo_encuesta(Empleado.Documento, Empleado.Nombres, id, Empleado.CorreoPersonal, Empleado.Empresa) == false)
                        {
                            return "ERROR";
                        }
                        else
                        {
                            retiros.EnvioEncuesta = "SI";
                            _repo.ActualizarEnvioEncuesta(retiros);

                        }


                        return "OK";
                    }
                }

            }
            catch
            {
                return "ERROR";
            }

             

        }

        public bool enviar_correo_encuesta(string cedula, string nombres, int idretiro, string email, string empresa)
        {
            string textocorreo = "";
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
            using (var db = new AutogestionContext())
            {
                var configuracion = db.Configuraciones.First(x => x.Parametro == "TXTMAILRETIRO");



                var link = db.Configuraciones.First(x => x.Parametro == "LINKENCRET1000").Valor.ToString() ;
                
                if (empresa == "2000")
                {
                    link = db.Configuraciones.First(x => x.Parametro == "LINKENCRET2000").Valor.ToString();
                }

                link = link.Replace("nombres", nombres);
                link = link.Replace("cedula", cedula);
                link = link.Replace("nroretiro", idretiro.ToString()); 

            textocorreo = configuracion.Valor;
            textocorreo = textocorreo.Replace("$URL", link);
            }
            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

               
                 correo.To.Add(email);

                

                
                correo.Subject = "Encuesta retiro";
                correo.Body = "Hola " + nombres + "</BR>" + textocorreo ;
                correo.Priority = System.Net.Mail.MailPriority.Normal;
                correo.IsBodyHtml = true;

                

                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                smtp.Host = "smtp-relay.gmail.com";
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
}
