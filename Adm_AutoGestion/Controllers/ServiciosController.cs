using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAP.Middleware.Connector;
using SAPExtractorDotNET;
using Adm_AutoGestion.Services;
using System.Web.Script.Serialization;
using System.Text;
using System.Web.Http.Cors;
using System.Data;
using Adm_AutoGestion.Models;
using System.Web;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Ajax.Utilities;
using System.Windows;


namespace Adm_AutoGestion.Controllers
{
     [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ServiciosController : ApiController
    {

         [HttpGet]
         [Route("api/certificacion/{nroempleado}/{consalario}/{dirigido}")]
       

        public string certificacion(string nroempleado, string consalario, string dirigido)
         {


             var valorconsalario = consalario.Replace("'", "");
             var valordirigido = dirigido.Replace("'", "");
             byte[] PDF = null;
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
                 IRfcFunction function = repository.CreateFunction("ZMF_CERTIFICADO_LABORAL");
                 //PARAMETROS IMPORT
                 function.SetValue("I_PERNR", nroempleado);
                 function.SetValue("RDB_CON", valorconsalario);
                 function.SetValue("I_DIRIGIDO", valordirigido);


                 function.Invoke(destination);
                 //OBTENER RESPUESTA 

                 PDF = function.GetByteArray("E_PDF");


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


             string base64String = Convert.ToBase64String(PDF, 0, PDF.Length);

             return base64String;
         }
        [HttpGet]
        [Route("api/comprobante/{nroempleado}/{fecha_inicial}/{fecha_final}")]
        public string comprobante(string nroempleado, string fecha_inicial, string fecha_final)
        {

            DateTime fechainicial = DateTime.Parse(fecha_inicial);
            DateTime fechafinal = DateTime.Parse(fecha_final);
            byte[] PDF = null;
            InMemoryDestinationConfiguration DestinacionConfiguracion = new InMemoryDestinationConfiguration();

            String contraseña = Properties.Settings.Default.Contraseña.ToString();
            var encodedTextBytes = Convert.FromBase64String(contraseña);

            contraseña = Encoding.UTF8.GetString(encodedTextBytes);

            RfcConfigParameters config = new RfcConfigParameters();
            try
            {
                String cadena = "HD001";
                Configuraciones Param = new Configuraciones();
                using (var db = new AutogestionContext())
                {
                    Param = db.Configuraciones.FirstOrDefault(e => e.Parametro == cadena.ToString());
                }

                if (Param.Valor == "NO")
                {

                    DateTime FechaActual = DateTime.Today;

                    //if (FechaActual > fechainicial && FechaActual <= fechafinal)
                    //{
                    //    return "";
                    //}


                    if (FechaActual.Month == fechainicial.Month && FechaActual.Year == fechainicial.Year)
                    {
                        return "";
                    }
                }
                

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
                IRfcFunction function = repository.CreateFunction("ZMF_DESPRENDIBLE_NOMINA");
                //PARAMETROS IMPORT
                function.SetValue("I_PERNR", nroempleado);
                function.SetValue("I_FECHAINI", fechainicial);
                function.SetValue("I_FECHAFIN", fechafinal);
                function.Invoke(destination);
                //OBTENER RESPUESTA 

                PDF = function.GetByteArray("E_PDF");


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



            string base64String = Convert.ToBase64String(PDF, 0, PDF.Length);

            return base64String;

        }

        [HttpGet]
        [Route("api/comprobanteContingencia/{nroempleado}/{fecha_inicial}/{fecha_final}/{empresa_emp}")]
        public string comprobanteContingencia(string nroempleado, string fecha_inicial, string fecha_final, string empresa_emp)
        {

            DateTime fechainicial = DateTime.Parse(fecha_inicial);
            DateTime fechafinal = DateTime.Parse(fecha_final); 
            byte[] PDF = null;
            InMemoryDestinationConfiguration DestinacionConfiguracion = new InMemoryDestinationConfiguration();

            String contraseña = Properties.Settings.Default.Contraseña.ToString();
            var encodedTextBytes = Convert.FromBase64String(contraseña);

            contraseña = Encoding.UTF8.GetString(encodedTextBytes);

            RfcConfigParameters config = new RfcConfigParameters();
            try
            {
                String cadena = "HD001";
                Configuraciones Param = new Configuraciones();
                using (var db = new AutogestionContext())
                {
                    Param = db.Configuraciones.FirstOrDefault(e => e.Parametro == cadena.ToString());
                }

                if (Param.Valor == "NO")
                {

                    DateTime FechaActual = DateTime.Today;

                    //if (FechaActual > fechainicial && FechaActual <= fechafinal)
                    //{
                    //    return "";
                    //}


                    if (FechaActual.Month == fechainicial.Month && FechaActual.Year == fechainicial.Year)
                    {
                        return "";
                    }
                }
                if (fechainicial.Year == 2024 && fechainicial.Month == 1  && fechainicial.Day == 1 & empresa_emp=="2000")
                {

                        EmpleadoController RepoEmp = new EmpleadoController();              
                    PDF = System.IO.File.ReadAllBytes(System.Web.Hosting.HostingEnvironment.MapPath("~/Comprobantes/" + nroempleado + ".pdf"));
                    string fileName = nroempleado;
                    string base64String2 = Convert.ToBase64String(PDF, 0, PDF.Length);

                    return base64String2;
                }


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
                IRfcFunction function = repository.CreateFunction("ZMF_DESPRENDIBLE_NOMINA");
                //PARAMETROS IMPORT
                function.SetValue("I_PERNR", nroempleado);
                function.SetValue("I_FECHAINI", fechainicial);
                function.SetValue("I_FECHAFIN", fechafinal);
                function.Invoke(destination);
                //OBTENER RESPUESTA 

                PDF = function.GetByteArray("E_PDF");


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



            string base64String = Convert.ToBase64String(PDF, 0, PDF.Length);

            return base64String;

        }

        [HttpGet]
        [Route("api/empleado/{documento}")]
        public string empleado(string documento)
        {
            using (var db = new AutogestionContext())
            {
                Empleado EmpDB = db.Empleados.Where(x => x.Documento == documento).FirstOrDefault();
                string SociedadEmp = "";
                Configuraciones ConfigEmpresas = db.Configuraciones.Where(x=>x.Parametro=="EMPRESAS_SAP").FirstOrDefault();
                char[] delimiterChars = { '%' };
                List<string> Campos = ConfigEmpresas.Valor.Split(delimiterChars).ToList();
                
                DataTable Datos = new DataTable();
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
                    IRfcFunction function = repository.CreateFunction("ZMF_VALIDAR_EMP_ACTIVO_3000");
                    //PARAMETROS IMPORT
                    function.SetValue("I_CEDULA", documento);

                    function.Invoke(destination);
                    //OBTENER RESPUESTA 

                    IRfcTable Tabla = function.GetTable("T_DATOS");

                    Datos = GetDataTableFromRFCTable(Tabla);

                    if (Datos.Rows.Count > 0)
                    {
                        var empleados = db.Empleados.Where(x => x.Documento == documento).ToList().OrderBy(x=>x.Empresa);
                        var count =0;
                        if (empleados.Count() == 0 || empleados.Count() < Datos.Rows.Count)
                        {
                            while (count< Datos.Rows.Count) 
                            {
                                var EMPDUP = empleados.Where(x => x.NroEmpleado == Datos.Rows[count]["PERNR"].ToString()).FirstOrDefault();
                                if (EMPDUP == null) 
                                {
                                    var nuevoempleado = new Empleado();
                                    nuevoempleado.Documento = documento;
                                    nuevoempleado.NroEmpleado = Datos.Rows[count]["PERNR"].ToString();
                                    nuevoempleado.Nombres = Datos.Rows[count]["ENAME"].ToString();
                                    nuevoempleado.Empresa = Datos.Rows[count]["BUKRS"].ToString();
                                    nuevoempleado.Area = Datos.Rows[count]["AREA"].ToString();
                                    nuevoempleado.Contraseña = "";
                                    nuevoempleado.UnidadOrganizativa = Datos.Rows[count]["ORGEH"].ToString();
                                    //13 ENERO ALVARO NUEVOS CAMPOS
                                    nuevoempleado.RH = Datos.Rows[count]["RH"].ToString();
                                    nuevoempleado.Cargo = Datos.Rows[count]["CARGO"].ToString();
                                    nuevoempleado.Genero = Datos.Rows[count]["GESCH"].ToString();
                                    nuevoempleado.FechaNacimiento = DateTime.Parse(Datos.Rows[0]["GBDAT"].ToString());
                                    nuevoempleado.Activo = "SI";
                                    nuevoempleado.A_Creacion = DateTime.Now;
                                    nuevoempleado.A_UsuarioCreador = "LOG99999";
                                    nuevoempleado.EPS = Datos.Rows[count]["EPS"].ToString();
                                    db.Empleados.Add(nuevoempleado);
                                    db.SaveChanges();

                                    var personalActivo = new PersonalActivo();
                                    personalActivo.Area = nuevoempleado.Area;
                                    personalActivo.Cargo = Datos.Rows[count]["CARGO"].ToString();
                                    personalActivo.CodigoEmpleado = nuevoempleado.NroEmpleado;
                                    personalActivo.Documento = nuevoempleado.Documento;
                                    personalActivo.Empresa = nuevoempleado.Empresa;
                                    personalActivo.Nombres = nuevoempleado.Nombres;
                                    personalActivo.UnidadOrganizativa = nuevoempleado.UnidadOrganizativa;
                                    db.PersonalActivo.Add(personalActivo);
                                    db.SaveChanges();
                                   
                                }
                                count++;
                            }
                               


                        }
                        else 
                        {
                            foreach (var empleado in empleados)
                            {

                                if (empleado.NroEmpleado != Datos.Rows[count]["PERNR"].ToString())
                                {
                                    empleado.Activo = "SI";
                                    empleado.FechaFin = null;
                                }
                                empleado.NroEmpleado = Datos.Rows[count]["PERNR"].ToString();
                                empleado.UnidadOrganizativa = Datos.Rows[count]["ORGEH"].ToString();
                                empleado.RH = Datos.Rows[count]["RH"].ToString();
                                empleado.Cargo = Datos.Rows[count]["CARGO"].ToString();
                                empleado.Genero = Datos.Rows[count]["GESCH"].ToString();
                                empleado.FechaNacimiento = DateTime.Parse(Datos.Rows[count]["GBDAT"].ToString());
                                empleado.Area = Datos.Rows[count]["AREA"].ToString();
                                empleado.Empresa = Datos.Rows[count]["BUKRS"].ToString();
                                empleado.A_Modificacion = DateTime.Now;
                                empleado.A_UsuarioModifica = "LOG99999";
                                empleado.EPS = Datos.Rows[count]["EPS"].ToString();
                                db.SaveChanges();
                                RfcDestinationManager.UnregisterDestinationConfiguration(DestinacionConfiguracion);
                                EmpleadoRepository services = new EmpleadoRepository();
                                services.ActualizarDatosEmpleado(empleado.NroEmpleado);

                                var personalActivo = new PersonalActivo();
                                personalActivo = db.PersonalActivo.FirstOrDefault(x => x.CodigoEmpleado == empleado.NroEmpleado);

                                if (personalActivo == null && empleado.Activo != "NO")
                                {
                                    var newpersonalactivo = new PersonalActivo();
                                    newpersonalactivo.Area = empleado.Area;
                                    newpersonalactivo.Cargo = Datos.Rows[count]["CARGO"].ToString();
                                    newpersonalactivo.CodigoEmpleado = empleado.NroEmpleado;
                                    newpersonalactivo.Documento = empleado.Documento;
                                    newpersonalactivo.Empresa = empleado.Empresa;
                                    newpersonalactivo.Nombres = empleado.Nombres;
                                    newpersonalactivo.UnidadOrganizativa = empleado.UnidadOrganizativa;
                                    db.PersonalActivo.Add(newpersonalactivo);
                                    db.SaveChanges();
                                }




                                count++;
                            }
                        }
                       
                                

                    }
                        
                        else if (EmpDB!= null)
                         {
                        SociedadEmp = Campos.Where(x => x.Equals(EmpDB.Empresa)).FirstOrDefault();
                        if (SociedadEmp == "" || SociedadEmp == null)
                        {
                            var Result = GenerarDatatableEmpleadosNoSAP(EmpDB);
                            if (Result.Rows.Count > 0)
                            {
                                Datos = Result;
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

                resultado = DataSetToJSON(Datos);
                return resultado;
            }
        }


           [HttpGet]
           [Route("api/pazysalvo1/{cedula}")]
           public string pazysalvo1(string cedula)
           {
               var resultado = "";
               
               using (var db = new AutogestionContext())
               {
                   var empleado = db.Empleados.FirstOrDefault(e => e.Documento == cedula);
                   List<PazySalvo> registro = db.PazySalvo.Where(e => e.EmpleadoId == empleado.Id && e.Estado == "Activo").ToList();
                   
                   if (registro.Count > 0) { resultado = "SI"; }
                   else { resultado = "NO"; }
               }

               return resultado; 
           }


           [HttpGet]
           [Route("api/ValidarCargo/{cedula}")]
           public string ValidarCargo(string cedula)
           {
               var resultado = "";

               using (var db = new AutogestionContext())
               {
                   var empleado = db.Empleados.FirstOrDefault(e => e.Documento == cedula && e.Cargo.Contains("APRENDIZ"));
                   

                   if (empleado == null) { resultado = "NO"; }
                   else { resultado = "SI"; }
               }

               return resultado;
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

            public static DataTable GenerarDatatableEmpleadosNoSAP(Empleado Emp)
            {
                DataTable loTable = new DataTable();
                int liElement = 0;
                using(var db = new AutogestionContext())
                {
                Configuraciones ConfigColumnnas = db.Configuraciones.Where(x => x.Parametro == "Login_CAMPOS_SAP").FirstOrDefault() ;
                char[] delimiterChars = { '%' };
                string [] Campos = ConfigColumnnas.Valor.Split(delimiterChars);

                //for (liElement = 0; liElement <= myrfcTable.ElementCount - 1; liElement++)
                //{
                //    RfcElementMetadata metadata = myrfcTable.GetElementMetadata(liElement);
                //    loTable.Columns.Add(metadata.Name);
                //}
                foreach (var item in Campos) 
                {
                    loTable.Columns.Add(item);
                }
                //foreach (IRfcStructure Row in myrfcTable)
                //{
                //    DataRow ldr = loTable.NewRow();
                //    for (liElement = 0; liElement <= myrfcTable.ElementCount - 1; liElement++)
                //    {
                //        RfcElementMetadata metadata = myrfcTable.GetElementMetadata(liElement);
                //        ldr[metadata.Name] = Row.GetString(metadata.Name);
                //    }
                //    loTable.Rows.Add(ldr);
                //}
                DataRow ldr = loTable.NewRow();
                ldr[Campos[0]] = Emp.NroEmpleado;
                ldr[Campos[1]] = "Estado SAP";
                ldr[Campos[2]] = Emp.FechaFin;
                ldr[Campos[3]] = Emp.Empresa;
                ldr[Campos[4]] = Emp.Nombres;
                ldr[Campos[5]] = Emp.Cargo;
                ldr[Campos[6]] = "";
                ldr[Campos[7]] = Emp.UnidadOrganizativa;
                ldr[Campos[8]] = Emp.Area;
                ldr[Campos[9]] = "EPS";
                ldr[Campos[10]] = Emp.RH;
                ldr[Campos[11]] = Emp.Genero;
                ldr[Campos[12]] = Emp.FechaNacimiento;
                ldr[Campos[13]] = "";
                ldr[Campos[14]] = "";
                ldr[Campos[15]] = Emp.FechaIngreso;
                ldr[Campos[16]] = Emp.Documento;
                loTable.Rows.Add(ldr);
                return loTable;
                }
            }

        [HttpGet]
           [Route("api/enviar_correo/{codempleado}/{nombres}/{empresa}")]
           public bool enviar_correo(string codempleado, string nombres, string empresa)
           {
               bool confirmacion;
               string txtde = Properties.Settings.Default.Correo.ToString();
               string contraseñacorreo = "";
               var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
               contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);

               string Para = Properties.Settings.Default.Correo1000.ToString();
               if (empresa == "2000"){
                   Para = Properties.Settings.Default.Correo2000.ToString();
               }

               try
               {
                   System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                   correo.From = new System.Net.Mail.MailAddress(txtde);
                   correo.To.Add(Para);
                   correo.Subject = "Solicitud Certificación";
                   correo.Body = Properties.Settings.Default.TextoCorreo.ToString() + " Codigo Empleado: " + codempleado + "   Nombres: "  + nombres;
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


           [HttpGet]
           [Route("api/Encuestanavidad/{Id}")]
           public string Encuestanavidad(string Id)
           {

               string Result = "";
               using (var db = new AutogestionContext())
               {
                   int id = 0;
                   Int32.TryParse(Id, out id);

                   Vacaciones Datos = new Vacaciones();
                   var empleado = db.Empleados.Find(id);


                   var link = db.Configuraciones.First(s => s.Parametro == "LINKNAVIDAD").Valor.ToString();

                   
                   link = link.Replace("NOMBRE", empleado.Nombres);
                   link = link.Replace("DOCUMENTO", empleado.Documento);
                   link = link.Replace("CARGO", empleado.Cargo);
                   link = link.Replace("AREA", empleado.AreaDescripcion);
                   link = link.Replace("EMPRESA", empleado.Empresa);
                   link = link.Replace("CORREO", empleado.Correo);
                   link = link.Replace("TELEFONO", empleado.Telefono);


                   Result = (link);

               }



               return (Result);
           }
        [HttpGet]
        [Route("api/ConsultarConfiguracion/{parametro}")]
        public string ConsultarConfiguracion(string parametro)
        {
            string Result = "";
            try 
            {
                using (var db = new AutogestionContext()) 
                {
                var config = db.Configuraciones.Where(x=>x.Parametro == parametro).FirstOrDefault();
                 if (config != null)
                    {
                        Result = config.Valor;
                    }
                    else 
                    {
                        throw new Exception("No se encuentra el parametro establecido");
                    }
                }          
            }
            catch (Exception ex) 
            {
                Result = "Error al consultar parametro: " + ex.Message;
            }
  
            return (Result);
        }

        [HttpGet]
        [Route("api/EmailValido/{email}")]
        public bool EmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

    }
}
