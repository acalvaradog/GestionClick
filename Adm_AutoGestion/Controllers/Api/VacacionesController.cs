using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.Data.Entity;
using Adm_AutoGestion.Services;
using Adm_AutoGestion.Models;
using System.Web.Http.Cors;
using System.Web;
using System.Web.Script.Serialization;
using System.IO;
using System.Data;
using SAP.Middleware.Connector;
using SAPExtractorDotNET;
using Newtonsoft.Json;
//using System.Web.Mvc;

namespace Adm_AutoGestion.Controllers.Api
{
      [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class VacacionesController : ApiController
    {
          private AutogestionContext db = new AutogestionContext();
         
        // GET api/vacaciones
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/vacaciones/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/vacaciones
        [HttpPost]
        [Route("api/vacaciones")]
        public IHttpActionResult Post()
        {

            //bool respuesta = true;
            string respuesta = "true";
            var Datos2 = new Vacaciones();
            var Datos3 = new Vacaciones();
            //try
            //{
            //    var filesToDelete = HttpContext.Current.Request.Params["probar"];
            //}catch
            //{
            //    respuesta = "El archivo supera el tamaño permitido de carga.";
            //    return Json(respuesta);
            //}
            var EmpleadoId = HttpContext.Current.Request.Params["Vacaciones.EmpleadoId"];
            var FechaInicial = HttpContext.Current.Request.Params["Vacaciones.FechaInicial"];
            var FechaFinal = HttpContext.Current.Request.Params["Vacaciones.FechaFin"];
            var CantDiasSolicitados = HttpContext.Current.Request.Params["Vacaciones.CantDiasSolicitados"];
            var CantDiasPendientes = HttpContext.Current.Request.Params["Vacaciones.CantDiasPendientes"];
            var VacacionesPagas = HttpContext.Current.Request.Params["Vacaciones.VacacionesPagadas"];
            var VacacionesAdelanta = HttpContext.Current.Request.Params["Vacaciones.VacacionesAdelantadas"];
            var VacacionesMayor6 = HttpContext.Current.Request.Params["Vacaciones.VacacionesDiasMayor6"];
            var diaspendientes = HttpContext.Current.Request.Params["CantPendt"];
            var Observacion = HttpContext.Current.Request.Params["Vacaciones.Observacion"];

            Vacaciones Datosvacaciones = new Vacaciones();
            
            
            Datosvacaciones.EmpleadoId =Convert.ToInt16(EmpleadoId);
            Datosvacaciones.FechaInicial = DateTime.Parse(FechaInicial);
            Datosvacaciones.FechaFin = DateTime.Parse(FechaFinal); ;
            Datosvacaciones.CantDiasSolicitados = CantDiasSolicitados;
            Datosvacaciones.CantDiasPendientes = CantDiasPendientes;
            Datosvacaciones.Fecha = DateTime.Now;
            Datosvacaciones.EstadoId = 1;
            Datosvacaciones.Observacion = Observacion;
            Datosvacaciones.VacacionesPagadas = VacacionesPagas;
            Datosvacaciones.VacacionesAdelantadas = VacacionesAdelanta;

            Empleado datosempleado = db.Empleados.FirstOrDefault(e => e.Id == Datosvacaciones.EmpleadoId);
            var datosefe = db.Empleados.FirstOrDefault(e => e.NroEmpleado == datosempleado.Jefe);
            Datosvacaciones.Empresa = datosempleado.Empresa;
            Datosvacaciones.VacacionesDiasMayor6 = VacacionesMayor6;
            

            List<Vacaciones> Datos = new List<Vacaciones>();
            //Datos = db.Vacaciones.OrderByDescending(e => e.Fecha).ToList();
            Datos = db.Vacaciones.OrderByDescending(e => e.Fecha).Where(e => e.EstadoId != 5 && e.EstadoId != 4 && e.EmpleadoId == Datosvacaciones.EmpleadoId).ToList();
            Datos2 = Datos.FirstOrDefault(e => e.EmpleadoId == Datosvacaciones.EmpleadoId && e.VacacionesPagadas == "NO");
            Datos3 = Datos.FirstOrDefault(e => e.EmpleadoId == Datosvacaciones.EmpleadoId && e.VacacionesPagadas == "SI");

            if (Datos2 != null)
            {

                if (Datosvacaciones.VacacionesPagadas == "NO")
                {
                    respuesta = "No es posible realizar una nueva solicitud, debido a que ya tiene una en proceso.";
                    return Json(respuesta);

                }

            }
            
            if (Datos3 != null)
            {

                if (Datosvacaciones.VacacionesPagadas == "SI")
                {
                    respuesta = "No es posible realizar una nueva solicitud, debido a que ya tiene una en proceso.";
                    return Json(respuesta);
                }
            }

            
            var Autoriza = db.HabilitarVacaciones.FirstOrDefault(x => x.EmpleadoId == Datosvacaciones.EmpleadoId);

            if (Autoriza == null)
            {
                Datosvacaciones.VacacionesPagadas = "NO";
                Datosvacaciones.VacacionesAdelantadas = "NO";
                Datosvacaciones.VacacionesDiasMayor6 = "NO";

                int cantdiassoli = 0;
                int diaspend = 0;
                Int32.TryParse(CantDiasSolicitados, out cantdiassoli);
                Int32.TryParse(diaspendientes, out diaspend);

                if (cantdiassoli > diaspend)
                {
                    respuesta = "Los dias solicitados exceden de la cantidad de dias que tiene pendientes por disfrutar.";
                    return Json(respuesta);
                }
               
            }

            //var deserializer = new JavaScriptSerializer();
            ////var results = deserializer.Deserialize<Vacaciones>(encuesta);


            ////var httpPostedFile = HttpContext.Current.Request.Files["UploadedImage"];
            //var httpPostedFile = HttpContext.Current.Request.Files["Adjunto"];


            //var extension = httpPostedFile.FileName.Split('.');

            //if (extension[1] != "jpg" && extension[1] != "jpeg" && extension[1] != "png" && extension[1] != "doc" && extension[1] != "docx" && extension[1] != "pdf")
            //{
            //    respuesta = "El tipo de archivo " + extension[1] + " no es permitido.";
            //    return Json(respuesta);
            //}

            //var size = httpPostedFile.ContentLength / 1024;
            //if (size > 1500)
            //{
            //    respuesta = "El archivo supera el tamaño permitido de carga.";
            //    return Json(respuesta);

            //}


            //// Validate the uploaded image(optional)
            //DateTime date1 = DateTime.Now;
            //var nombrearchivo = date1.ToString("yyyyMMddHHmmssffff").ToString() + "-" + httpPostedFile.FileName;
            //// Get the complete file path
            //var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/AnexosVacaciones"), nombrearchivo);
            //Datosvacaciones.Adjunto = nombrearchivo;
            ////// Save the uploaded file to "UploadedFiles" folder
            //httpPostedFile.SaveAs(fileSavePath);


            try
            {

                HistorialVacaciones Historial = new HistorialVacaciones();

                Historial.VacacionesId = Datosvacaciones.Id;
                Historial.Fecha = Datosvacaciones.Fecha;
                Historial.Accion = "1";
                Historial.EmpleadoId = Datosvacaciones.EmpleadoId;
                
                db.Vacaciones.Add(Datosvacaciones);
                db.HistorialVacaciones.Add(Historial);
                db.SaveChanges();

                
                NotificarRegistro(datosempleado.Documento, datosempleado.Nombres, datosefe.Correo, Datosvacaciones.FechaInicial, Datosvacaciones.FechaFin, Datosvacaciones.CantDiasSolicitados);
                if (Datosvacaciones.VacacionesPagadas == "SI") 
                {
                    NotificarRegistroVacPag(datosempleado.Documento, datosempleado.Nombres, datosempleado.Correo);
                }

            return Json(respuesta);
            }
            catch (SystemException ex)
            {
                respuesta = String.Format("Error al guardar. {0}", ex.Message);
                return Json(respuesta);
            }


        }



        [HttpGet]
        [Route("api/Vacaciones/Consulta/{Id}")]
        public IHttpActionResult Consulta(string Id)
        {
            
            string Result = "";
            using (var db = new AutogestionContext())
            {
                int id = 0;
                Int32.TryParse(Id, out id);

                List<Vacaciones> Datos = new List<Vacaciones>();
                var datos3 = db.Vacaciones.Where(e => e.EmpleadoId == id && e.VacacionesPagadas == "NO").OrderByDescending(y => y.Fecha).FirstOrDefault();
                if (datos3 != null)
                {
                    EstadoVacaciones estado = db.EstadoVacaciones.FirstOrDefault(x => x.Id == datos3.EstadoId);
                    datos3.EstadoVacaciones = estado;
                }

                var datos4 = db.Vacaciones.Where(e => e.EmpleadoId == id && e.VacacionesPagadas == "SI").OrderByDescending(y => y.Fecha).FirstOrDefault();
                if (datos4 != null)
                {
                    EstadoVacaciones estado = db.EstadoVacaciones.FirstOrDefault(x => x.Id == datos4.EstadoId);
                    datos4.EstadoVacaciones = estado;
                }

                string DatNorm = JsonConvert.SerializeObject(datos3);
                string DatDin = JsonConvert.SerializeObject(datos4);
                Result = DatNorm + ";" + DatDin; 

            }

            

            return Json(Result);
        }



        [HttpGet]
        [Route("api/Vacaciones/Autoriza/{Id}")]
        public string Autoriza(string Id)
        {
            string respuesta = "";
            int id = 0;
            Int32.TryParse(Id, out id);

                var Autoriza = db.HabilitarVacaciones.FirstOrDefault(x => x.EmpleadoId == id);


                if (Autoriza != null)
                {

                    if (Autoriza.DiasMayor6 == "SI" )
                    {
                        respuesta = respuesta + "DsMayor";
                        //return (respuesta);
                    }

                    if (Autoriza.Anticipadas == "SI")
                    {
                        respuesta = respuesta + "Ant";
                        //return (respuesta);
                    }

                    if (Autoriza.Pagadas == "SI")
                    {
                        respuesta = respuesta + "Pag";
                        //return (respuesta);
                    }

                    //if (Autoriza.DiasMayor6 == "NO" && Autoriza.Anticipadas == "NO")
                    //{
                    //    respuesta = "SinAutPagAde";
                    //    return (respuesta);
                    //}
                    return (respuesta);
                }
                respuesta = "SinAut";
                return (respuesta);
               
        }


        [HttpGet]
        [Route("api/ValidarDias/{Codigo}")]
        public string ValidarDias(string Codigo)
        { 
        DataTable Datos = new DataTable();
            var Result = "";
            //var Diferencia = ""; 
            InMemoryDestinationConfiguration DestinacionConfiguracion = new InMemoryDestinationConfiguration();

            if (Codigo.Length < 5)
            {
                int Id = 0;
                Int32.TryParse(Codigo, out Id);
                var emp = db.Empleados.Find(Id);
                Codigo = emp.NroEmpleado;
            }

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
                IRfcFunction function = repository.CreateFunction("ZMF_CONTING_ABSENTISMO");
                //PARAMETROS IMPORT
                function.SetValue("I_PERNR", Codigo);

                function.Invoke(destination);
                //OBTENER RESPUESTA 

                IRfcTable Tabla = function.GetTable("TB_PA2006");

                Datos = GetDataTableFromRFCTable(Tabla);
                

                int count = Datos.Rows.Count;
                int s = count - 1;
                decimal suma = 0;
                decimal resta = 0;
               

                for (var f = 0; f < count; f++ )
                {
                    if (Datos.Rows[f]["KTART"].ToString() == "10" && Datos.Rows[f]["DEEND"].ToString() == "9999-12-31")
                    {
                        resta = (Math.Round(Convert.ToDecimal(Datos.Rows[f]["ANZHL"].ToString())) - Math.Round(Convert.ToDecimal(Datos.Rows[f]["KVERB"].ToString())));
                        
                        if (Datos.Rows[f]["KVERB"].ToString() != Datos.Rows[f]["ANZHL"].ToString())
                        {
                            if (f == 1 || f == s)
                            {
                               
                                Result += string.Format("{0}-{1},", resta, Math.Round(Convert.ToDecimal(Datos.Rows[f]["KVERB"].ToString())));
                            }
                            else
                            {
                                Result += string.Format("{0},", resta);
                            }
                        }

                    }
                }

                Result = Result.Substring(0, Result.Length - 1);

            }
            catch (SystemException ex)
            {
                // ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
            }
            catch (RfcLogonException ex)
            {
                //ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
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

            string resultado = "";


            //}

            //resultado = DataSetToJSON(Datos);
            resultado = (Result);
            return resultado;
        
        }

        [HttpGet]
        [Route("api/Historial/{documento}")]
        public string Historial(string documento)
        {

            DataTable Datos = new DataTable();
            var Result = "";
            //var Diferencia = ""; 
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
                IRfcFunction function = repository.CreateFunction("ZMF_CONTING_ABSENTISMO");
                //PARAMETROS IMPORT
                function.SetValue("I_PERNR", documento);

                function.Invoke(destination);
                //OBTENER RESPUESTA 

                IRfcTable Tabla = function.GetTable("TB_PA2006");

                Datos = GetDataTableFromRFCTable(Tabla);
                

                int count = Datos.Rows.Count;
                int s = count - 1;
                int cont = 6;
                decimal suma = 0;
                decimal resta = 0;
               

                for (var f = 0; f < count; f++ )
                {
                    
                        if (Datos.Rows[f]["KTART"].ToString() == "02" && Datos.Rows[f]["DEEND"].ToString() == "9999-12-31")
                        {
                            Result += String.Format("<tr><td>" + Datos.Rows[f]["BEGDA"].ToString() + "</td><td>" + Datos.Rows[f]["ENDDA"].ToString() + "</td><td>" + Math.Round(Convert.ToDecimal(Datos.Rows[f]["ANZHL"].ToString())) + "</td><td>" + Math.Round(Convert.ToDecimal(Datos.Rows[f]["KVERB"].ToString())) + "</td></tr>");

                            resta = (Math.Round(Convert.ToDecimal(Datos.Rows[f]["ANZHL"].ToString())) - Math.Round(Convert.ToDecimal(Datos.Rows[f]["KVERB"].ToString())));
                            suma = suma + resta;

                        }
                    
                }

                Result += String.Format(";" + suma);

                if (Datos.Rows.Count > 0)
                {



                    using (var db = new AutogestionContext())
                    {
                        var empleado = db.Empleados.FirstOrDefault(x => x.Documento == documento);

                        if (empleado == null)
                        {

                            var nuevoempleado = new Empleado();
                            nuevoempleado.Documento = documento;
                            nuevoempleado.NroEmpleado = Datos.Rows[0]["PERNR"].ToString();
                            nuevoempleado.Nombres = Datos.Rows[0]["ENAME"].ToString();
                            nuevoempleado.Empresa = Datos.Rows[0]["BUKRS"].ToString();
                            nuevoempleado.Contraseña = "";
                            db.Empleados.Add(nuevoempleado);
                            db.SaveChanges();


                        }
                    }
                }

            }
            catch (SystemException ex)
            {
                // ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
            }
            catch (RfcLogonException ex)
            {
                //ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
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

            string resultado = "";


            //}

            //resultado = DataSetToJSON(Datos);
            resultado = (Result);
            return resultado;

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



        [HttpGet]
        [Route("api/Festivos/{fechaini}/{diferencia}")]
        public string Festivos(string fechaini, string diferencia)
        {
            string respuesta = "";
            var contador = 0;
            var i = 0;
            var j = 0;
            int dif = 0;
            DateTime fechaSelec = DateTime.Now;
            
            Int32.TryParse(diferencia, out dif);
          

            DateTime.TryParse(fechaini, out fechaSelec);
            //fechaSelec = fechaSelec.AddDays(1);

            for (i = 0; i < dif; i++)
            {
                var dia = fechaSelec.DayOfWeek;
                if (fechaSelec.DayOfWeek == DayOfWeek.Saturday || fechaSelec.DayOfWeek == DayOfWeek.Sunday)
                {
                    dif = dif + 1;
                    fechaSelec = fechaSelec.AddDays(1);
                }
                else {
                    fechaSelec = fechaSelec.AddDays(1);
                }

                var festivos = db.DiasFestivos.Where(e => DbFunctions.TruncateTime(e.festivo) == fechaSelec).ToList();
                var dia4 = fechaSelec.DayOfWeek;
                    if (festivos.Count != 0) {
                        var dia2 = fechaSelec.DayOfWeek;
                        if (fechaSelec.DayOfWeek == DayOfWeek.Saturday || fechaSelec.DayOfWeek == DayOfWeek.Sunday)
                        {
                        }
                        else {
                            dif = dif + 1;
                        }
                    }
                    
            }

            


            if (fechaSelec.DayOfWeek == DayOfWeek.Saturday)
            {

                fechaSelec = fechaSelec.AddDays(2);
            }

            if (fechaSelec.DayOfWeek == DayOfWeek.Sunday)
            {

                fechaSelec = fechaSelec.AddDays(1);
            }

            var festivos3 = db.DiasFestivos.Where(e => DbFunctions.TruncateTime(e.festivo) == fechaSelec).ToList();
            if (festivos3.Count != 0)
            {
                fechaSelec = fechaSelec.AddDays(1); ;
            }


            respuesta = String.Format("{0}", fechaSelec);

            return (respuesta);

        }

        [HttpPost]
        [Route("api/AutorizaAnticipadasPagas")]
        public string AutorizaAnticipadasPagas() 
        {
            string res = "";
            string JefeSolicita = "TXTSOLAUTEMP";
            var EmpleadoId = HttpContext.Current.Request.Params["Autoriza.EmpleadoId"];
            var Dinero = HttpContext.Current.Request.Params["Autoriza.Dinero"];
            var Anticipadas = HttpContext.Current.Request.Params["Autoriza.Anticipadas"];
            var Mayor6 = HttpContext.Current.Request.Params["Autoriza.Mayor6"];

            int empleadoid = 0;
            Int32.TryParse(EmpleadoId, out empleadoid);

            using (var db = new AutogestionContext())
            {
                var EmpleadoSol = db.Empleados.FirstOrDefault(e => e.Id == empleadoid);

                var envio = NotificarAutorizacion(EmpleadoSol.Nombres, EmpleadoSol.Documento, EmpleadoSol.NroEmpleado, Dinero, Anticipadas, Mayor6, EmpleadoSol.Empresa, EmpleadoSol.Correo, JefeSolicita);
                if (envio == true)
                {
                    res = "true";
                }
                else { res = "false"; }
            }



            return res = "true";
        }


        [HttpGet]
        [Route("api/validarFechaini/{fechaini}")]
        public string validarFechaini(string fechaini)
        {
            string respuesta = "Ok";
            DateTime fechaSelec = DateTime.Now;
            DateTime.TryParse(fechaini, out fechaSelec);

            if (fechaSelec.DayOfWeek == DayOfWeek.Saturday || fechaSelec.DayOfWeek == DayOfWeek.Sunday)
            {
                respuesta = "Error";
            }

            var festivos = db.DiasFestivos.Where(e => DbFunctions.TruncateTime(e.festivo) == fechaSelec).ToList();
            if (festivos.Count != 0)
            {
                respuesta = "Error";
            }


            return (respuesta);
        }

        public bool NotificarRegistroVacPag(string documento, string nombres, string email)
        {
            string textocorreo = "";
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
            var configuracion = db.Configuraciones.First(x => x.Parametro == "TEXTOREGISTROVACAPA");


            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);


                correo.To.Add(email);
                textocorreo = configuracion.Valor;
                //textocorreo = "De manera atenta se notifica que el registro de su solicitud de vacaciones fue exitoso. <BR><BR><i>NOTA: Favor tener en cuenta que cuando solicita vacaciones en DINERO, estas pasan a estudio para aprobación por parte de la jefatura de Gestión Humana.</i>  <br><br><br>Cordialmente,<br><H1>Servicios Digitales Foscal</H1><img width='145' height='156' src='http://www.foscal.com.co/correo/logo-correo-foscal.jpg' alt='Logo FOSCAL Internacional' hspace='5' class='CToWUd'>";
                correo.Subject = "Registro Solicitud de Vacaciones";
                correo.Body = "Cordial saludo," + nombres + "<BR><BR>" + textocorreo;
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

        public bool NotificarRegistro(string documento, string nombres, string emailjefe, DateTime fechainicio, DateTime fechafin, string cantidaddias)
        {

            string textocorreo = "";
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
            var configuracion = db.Configuraciones.First(x => x.Parametro == "TEXTOREGISTROVACA");

            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

                
                correo.To.Add(emailjefe);
                textocorreo = configuracion.Valor;
                textocorreo = textocorreo.Replace("$USUARIO", nombres);
                textocorreo = textocorreo.Replace("$DOCUMENTO", documento);

                textocorreo = textocorreo.Replace("$FECHAINICIO", fechainicio.ToString("dd-MM-yyyy"));
                textocorreo = textocorreo.Replace("$FECHAFIN", fechafin.ToString("dd-MM-yyyy"));
                textocorreo = textocorreo.Replace("$CANTIDADDIAS", cantidaddias);
                //textocorreo = "Se registró la siguiente solicitud de vacaciones del empleado: <BR><BR> Nombre Empleado: " + nombres + "<BR>Documento Empleado: " + documento + "<BR>Fecha Inicio: " + fechainicio + "<BR>Fecha Reintegro: " + fechafin + "<BR>Cantidad Dias Solicitados: " + cantidaddias + "<br><br><br>Cordialmente,<br><H1>Servicios Digitales Foscal</H1><img width='145' height='156' src='http://www.foscal.com.co/correo/logo-correo-foscal.jpg' alt='Logo FOSCAL Internacional' hspace='5' class='CToWUd'>";
                correo.Subject = "Registro Solicitud de Vacaciones";
                correo.Body = "Cordial Saludo," + "<BR><BR>" + textocorreo;
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

        public bool NotificarAutorizacion(string Nombres, string Documento, string NroEmpleado, string Dinero, string Anticipadas, string Mayor6, string Empresa, string Correo, string JefeSolicita)
        {

            string textocorreo = "";
            string email = "";

            using (var db = new AutogestionContext())
            {
                var correo = db.Configuraciones.FirstOrDefault(e => e.Parametro == Empresa);
                email = correo.Valor;

                var texto = db.Configuraciones.FirstOrDefault(e => e.Parametro == JefeSolicita).Valor.ToString();
                texto = texto.Replace("$NOMBRE", Nombres);
                texto = texto.Replace("$DOCUMENTO", Documento);
                texto = texto.Replace("$CODIGO", NroEmpleado);
                texto = texto.Replace("$DINERO", Dinero);
                texto = texto.Replace("$ANTICIPADAS", Anticipadas);
                texto = texto.Replace("$MAYOR6", Mayor6);
                texto = texto.Replace("$CORREO", Correo);
                
                textocorreo = texto;
            }

            
           
            bool confirmacion = true;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);

            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);


                correo.To.Add(email);

                
                correo.Subject = "SOLICITUD DE AUTORIZACION PARA REGISTRO DE VACACIONES " + NroEmpleado;
                correo.Body = textocorreo;
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


        // PUT api/vacaciones/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/vacaciones/5
        public void Delete(int id)
        {
        }


        [HttpGet]
        [Route("api/Vacaciones/anularRegistro/{Id}")]
        public string AnularRegistro(int id)
        {
            var message = "";
            using (var db = new AutogestionContext())
            {
                var jsonDeserialize = new JavaScriptSerializer();
                DateTime Fecha2 = DateTime.Now;

                try
                {

                    Vacaciones Vacaciones = new Vacaciones();
                    Vacaciones = db.Vacaciones.FirstOrDefault(e => e.Id == id);
                    db.Vacaciones.Attach(Vacaciones);
                    Vacaciones.EstadoId = 5;
                    Vacaciones.Observacion = "Registro anulado por el Trabajador.";

                    db.SaveChanges();


                    HistorialVacaciones Historial = new HistorialVacaciones();

                    Historial.VacacionesId = Convert.ToInt32(id);
                    Historial.Accion= "5";
                    Historial.Fecha = Fecha2;
                    Historial.EmpleadoId = Vacaciones.EmpleadoId;
                    Historial.UsuarioModifica = Vacaciones.EmpleadoId;
                    Historial.Observaciones = "Registro anulado por el Trabajador.";
                    db.HistorialVacaciones.Add(Historial);
                    db.SaveChanges();
                    message = "True";


                }
                catch (Exception ex)
                {
                    message = "False";
                }

                var json = jsonDeserialize.Serialize(message);
                return json;

            }

        }

    }
}
