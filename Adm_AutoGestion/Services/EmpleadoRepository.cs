using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Adm_AutoGestion.Models;
using System.Data.Entity;
using SAP.Middleware.Connector;
using SAPExtractorDotNET;
using System.Data;
using System.Text;
using System.IO;
using System.Text;
using System.Web.DynamicData;
using System.Data.Common;
using Adm_AutoGestion.Controllers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Documents;
using Adm_AutoGestion.Models.EvaDesempeno;
using System.Windows.Controls;

namespace Adm_AutoGestion.Services
{
    public class EmpleadoRepository
    {
        public List<Empleado> ObtenerTodos()
        {
            using (var db = new AutogestionContext())
            {

            
                return db.Empleados.ToList();
                //return preguntas();

            }
        }

        public List<Empleado> ObtenerTodos_Empresa(string empresa)
        {
            using (var db = new AutogestionContext())
            {


                return db.Empleados.Where(x => x.Empresa== empresa).ToList();
                //return preguntas();

            }
        }

        public List<Empleado> ObtenerTodosNoSAP()
        {
            using (var db = new AutogestionContext())
            {

              
                var Config = db.Configuraciones.Where(x => x.Parametro == "EMPRESAS_SAP").FirstOrDefault();
                char[] delimiterChars = { '%' };
                List<string> Empresas = Config.Valor.Split(delimiterChars).ToList();

                return db.Empleados.Where(x => x.Externo == true).ToList(); ;
               // return db.Empleados.Where(x => Empresas.Contains(x.Empresa) == false).ToList(); ;
                //return preguntas();

            }
        }
        public List<Empleado> ObtenerxCodigo(string codigo)
        {
            using (var db = new AutogestionContext())
            {


                return db.Empleados.Where(x=> x.NroEmpleado == codigo).ToList();
                //return preguntas();

            }
        }

        internal void Crear(Empleado model)
        {

            using (var db = new AutogestionContext())
            {
                try
                {
                    db.Empleados.Add(model);
                    db.SaveChanges();
                }
                catch
                {
                }
            }

        }


        internal void Editar(Empleado model)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    if (model.Contraseña == null)
                    {
                        model.Contraseña = "";
                    }

                    //GUARDAR Y MODIFICAR UNICAMENTE LOS DATOS CAMBIADOS
                    bool cambio = false;
                    Empleado EmpDatos = db.Empleados.Where(x=>x.Id==model.Id).FirstOrDefault();
                    db.Empleados.Attach(EmpDatos);
                    if (EmpDatos.Documento!=model.Documento){ EmpDatos.Documento = model.Documento; cambio = true; }
                    if (EmpDatos.Nombres != model.Nombres) {EmpDatos.Nombres = model.Nombres; cambio = true; }
                    if (EmpDatos.NroEmpleado != model.NroEmpleado) {EmpDatos.NroEmpleado = model.NroEmpleado; cambio = true; }
                    if (EmpDatos.FechaNacimiento != model.FechaNacimiento){EmpDatos.FechaNacimiento = model.FechaNacimiento; cambio = true; }
                    if (EmpDatos.Genero != model.Genero){EmpDatos.Genero = model.Genero; cambio = true; }
                    if (EmpDatos.Telefono != model.Telefono) {EmpDatos.Telefono = model.Telefono; cambio = true; }
                    if (EmpDatos.Correo != model.Correo) { EmpDatos.Correo = model.Correo; cambio = true; }
                    if (EmpDatos.CorreoPersonal != model.CorreoPersonal) { EmpDatos.CorreoPersonal = model.CorreoPersonal; cambio = true; }
                    if (EmpDatos.Cargo != model.Cargo) { EmpDatos.Cargo = model.Cargo; cambio = true; }
                    if (EmpDatos.Area != model.Area) { EmpDatos.Area = model.Area; cambio = true; }
                    if (EmpDatos.AreaDescripcion != model.AreaDescripcion) { EmpDatos.AreaDescripcion = model.AreaDescripcion; cambio = true; }
                    if (EmpDatos.TipoArea != model.TipoArea) { EmpDatos.TipoArea = model.TipoArea; cambio = true; }
                    if (EmpDatos.Empresa != model.Empresa) { EmpDatos.Empresa = model.Empresa; cambio = true; }
                    if (EmpDatos.FechaIngreso != model.FechaIngreso) { EmpDatos.FechaIngreso = model.FechaIngreso; cambio = true; }
                    if (EmpDatos.Jefe != model.Jefe) { EmpDatos.Jefe = model.Jefe; cambio = true; }
                    if (EmpDatos.Superior != model.Superior) { EmpDatos.Superior = model.Superior; cambio = true; }
                    if (EmpDatos.Director != model.Director) { EmpDatos.Director = model.Director; cambio = true; }
                    if (EmpDatos.Lider != model.Lider) { EmpDatos.Lider = model.Lider; cambio = true; }
                    if (EmpDatos.UnidadOrganizativa != model.UnidadOrganizativa) { EmpDatos.UnidadOrganizativa = model.UnidadOrganizativa; cambio = true; }
                    if (EmpDatos.Contraseña != model.Contraseña) { EmpDatos.Contraseña = model.Contraseña; cambio = true; }
                    if (EmpDatos.ModoTrabajo != model.ModoTrabajo) { EmpDatos.ModoTrabajo = model.ModoTrabajo; cambio = true; }
                    if (EmpDatos.Activo != model.Activo) { EmpDatos.Activo = model.Activo; cambio = true; }
                    if (EmpDatos.RH != model.RH) { EmpDatos.RH = model.RH; cambio = true; }
                     //db.Entry(model).State = EntityState.Modified;
                    if (cambio == true) 
                    {
                        EmpDatos.A_UsuarioModifica = model.A_UsuarioModifica;
                        EmpDatos.A_Modificacion = model.A_Modificacion;
                        db.SaveChanges();
                    }
                 


                }
                catch
                { }
            }
        }


        public List<Empleado> Mostrar(int id)
        {
            using (var db = new AutogestionContext())
            {

                
                return db.Empleados.Where(e => e.Id == id).ToList();
                

            }
        }
        // MAESTRO
        public void ProcesoActualizacionyRetirados()
        {
            List<Empleado> Empleado = new List<Empleado>();
            PersonalActivo ps = new PersonalActivo();
            DateTime Fecha = Convert.ToDateTime(DateTime.Today);
            Empleado ItemEmpleado = new Empleado();
            using (var db = new AutogestionContext())
            {

                try
                {
                    Configuraciones ConfigEmpresas = db.Configuraciones.Where(x => x.Parametro == "EMPRESAS_SAP").FirstOrDefault();
                    char[] delimiterChars = { '%' };
                    List<string> Empresas = ConfigEmpresas.Valor.Split(delimiterChars).ToList();

                    //Retirar empleados que ya no esten activos en sap
                    Empleado = db.Empleados.Where(x => x.Activo != "NO" && Empresas.Contains(x.Empresa)).ToList();
                    DataTable Dt = new DataTable();
                    EmpleadoRepository EmpleadoServices = new EmpleadoRepository();
                    foreach (var item in Empleado)
                    {
                        try
                        {
                        Dt = EmpleadoServices.ValidarEmpleadoActivo(item.Documento);

                        if (Dt.Rows.Count < 1)
                        {
                            item.Activo = "NO";
                            item.FechaFin = DateTime.Now.Date;
                            item.A_Modificacion = DateTime.Now;
                            item.A_UsuarioModifica = "MAESTRO9";
                            db.SaveChanges();
                            ps = db.PersonalActivo.FirstOrDefault(e => e.CodigoEmpleado == item.NroEmpleado);
                            if (ps != null)
                            {
                                db.PersonalActivo.Remove(ps);
                                db.SaveChanges();
                            }


                        }
                        }
                        catch (Exception ex)
                        {
                            var rutaarchivo = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/LogJob"), "logJobUPDEmpleado.txt");
                            using (StreamWriter w = File.AppendText(rutaarchivo))
                            {
                                w.Write(Environment.NewLine);
                                w.Write("[{0} {1}]", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                                w.Write("\t");
                                w.WriteLine(" {0}", "Error empleado: " + ex.Message.ToString() + DateTime.Now + item.Nombres);
                                w.WriteLine("-----------------------");
                            }
                        }
                   


                    }


                    //Retirar empleados que ya no esten activos en sap
                    Empleado = db.Empleados.Where(x => x.Activo == "NO" && Empresas.Contains(x.Empresa)).ToList();
                    DataTable Dt2 = new DataTable();
                    EmpleadoRepository EmpleadoServices2 = new EmpleadoRepository();
                    foreach (var item in Empleado)
                    {
                        try
                        {
                            Dt2 = EmpleadoServices2.ValidarEmpleadoActivo(item.Documento);

                        if (Dt2.Rows.Count >= 1)
                        {
                            item.Activo = "SI";
                            item.FechaFin = null;
                            item.A_Modificacion = DateTime.Now;
                            item.A_UsuarioModifica = "MAESTRO9";
                            db.SaveChanges();

                        }
                        }
                        catch (Exception ex)
                        {

                            var rutaarchivo = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/LogJob"), "logJobUPDEmpleado.txt");
                            using (StreamWriter w = File.AppendText(rutaarchivo))
                            {
                                w.Write(Environment.NewLine);
                                w.Write("[{0} {1}]", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                                w.Write("\t");
                                w.WriteLine(" {0}", "Error empleado: " + ex.Message.ToString() + DateTime.Now + item.Nombres);
                                w.WriteLine("-----------------------");
                            }
                        }
                  


                    }

                    //Eliminar de la tabla personal activo los trabadores retirados
                    Empleado = db.Empleados.Where(x => x.Activo == "NO" && DbFunctions.TruncateTime(x.FechaFin.Value) < Fecha).ToList();

                    foreach (var item in Empleado)
                    {
                        ps = db.PersonalActivo.FirstOrDefault(e => e.CodigoEmpleado == item.NroEmpleado);
                        if (ps != null)
                        {
                            db.PersonalActivo.Remove(ps);
                            db.SaveChanges();
                        }

                    }

                    //Actualizar datos empleados activos
                    Empleado = db.Empleados.Where(x => x.Activo != "NO" && Empresas.Contains(x.Empresa)).ToList();
                    EmpleadoRepository services = new EmpleadoRepository();
                    foreach (var item in Empleado)
                    {


                        services.ActualizarDatosEmpleado(item.NroEmpleado);
                    }

                    var fileSavePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/LogJob"), "logJobUPDEmpleado.txt");


                    // Write the string array to a new file named "WriteLines.txt".
                    using (StreamWriter w = File.AppendText(fileSavePath))
                    {
                        w.Write(Environment.NewLine);
                        w.Write("[{0} {1}]", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                        w.Write("\t");
                        w.WriteLine(" {0}", "Ejecución correcta Empl: " + DateTime.Now);
                        w.WriteLine("-----------------------");
                    }


                }
                catch (Exception ex)
                {
                    var fileSavePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/LogJob"), "logJobUPDEmpleado.txt");

                    // Write the string array to a new file named "WriteLines.txt".
                    using (StreamWriter w = File.AppendText(fileSavePath))
                    {
                        w.Write(Environment.NewLine);
                        w.Write("[{0} {1}]", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                        w.Write("\t");
                        w.WriteLine(" {0}", "Error Empl: " + ex.Message.ToString() + DateTime.Now);
                        w.WriteLine("-----------------------");
                    }


                }


            }
        
        }
        // CONSOLA
        public void ProcesoActualizacionyRetiradosIndividual(Empleado empleado)
        {
            Empleado Empleado = new Empleado();
            PersonalActivo ps = new PersonalActivo();
            DateTime Fecha = Convert.ToDateTime(DateTime.Today);
            Empleado ItemEmpleado = new Empleado();
            
            using (var db = new AutogestionContext())
            {
                Configuraciones ConfigEmpresas = db.Configuraciones.Where(x => x.Parametro == "EMPRESAS_SAP").FirstOrDefault();
                char[] delimiterChars = { '%' };
                List<string> Empresas = ConfigEmpresas.Valor.Split(delimiterChars).ToList();
                string EmpresaEmp = Empresas.Where(x => x.Equals(empleado.Empresa)).FirstOrDefault();
                try
                {


                    //Retirar empleados que ya no esten activos en sap
                    Empleado = db.Empleados.First(x => x.Id == empleado.Id);
                    DataTable Dt = new DataTable();
                    EmpleadoRepository EmpleadoServices = new EmpleadoRepository();

                    if (Empleado != null && (EmpresaEmp!="" && EmpresaEmp!=null))
                    {
                        if (Empleado.Activo != "NO") { 
                            Dt = EmpleadoServices.ValidarEmpleadoActivo(Empleado.Documento);

                            if (Dt.Rows.Count < 1)
                            {
                              Empleado.Activo = "NO";
                               Empleado.FechaFin = DateTime.Now.Date;
                                Empleado.A_UsuarioModifica = "CONSOLA9";
                                Empleado.A_Modificacion = DateTime.Now;
                                db.SaveChanges();
                                ps = db.PersonalActivo.FirstOrDefault(e => e.Documento == Empleado.Documento);
                                if (ps != null)
                                {
                                    db.PersonalActivo.Remove(ps);
                                    db.SaveChanges();
                                }


                            }

                    }
                    }



                    //Retirar empleados que ya no esten activos en sap
                    Empleado = db.Empleados.First(x => x.Id == empleado.Id);
                    DataTable Dt2 = new DataTable();
                    EmpleadoRepository EmpleadoServices2 = new EmpleadoRepository();

                    if (Empleado != null && (EmpresaEmp != "" && EmpresaEmp != null)) { 
                        if (Empleado.Activo == "NO") {
                            Dt2 = EmpleadoServices2.ValidarEmpleadoActivo(Empleado.Documento);

                            if (Dt2.Rows.Count >= 1)
                            {
                                Empleado.Activo = "SI";
                                Empleado.FechaFin = null;
                                Empleado.A_UsuarioModifica = "CONSOLA9";
                                Empleado.A_Modificacion = DateTime.Now;
                                db.SaveChanges();

                            }
                        }
                    }





                    //Eliminar de la tabla personal activo los trabadores retirados
                    Empleado = db.Empleados.First(x => x.Id == empleado.Id);

                    if (Empleado != null) {
                        if (Empleado.Activo == "NO" && Empleado.FechaFin.Value.Date < Fecha) { 
                        
                        
                        ps = db.PersonalActivo.FirstOrDefault(e => e.CodigoEmpleado == Empleado.NroEmpleado);
                        if (ps != null)
                        {
                            db.PersonalActivo.Remove(ps);
                            db.SaveChanges();
                        }
                        }
                    }

                    //Actualizar datos empleados activos
                    Empleado = db.Empleados.First(x => x.Id == empleado.Id);
                    EmpleadoRepository services = new EmpleadoRepository();

                        if (Empleado != null && (EmpresaEmp != "" && EmpresaEmp != null))
                        {
                            if (Empleado.Activo != "NO") { 
                                       services.ActualizarDatosEmpleado(Empleado.NroEmpleado);

                            //var query = from row in Dt.AsEnumerable()
                            //            where row.Field<string>("PERNR") == Empleado.NroEmpleado
                            //            select new
                            //            {
                            //                Sociedad = row.Field<string>("BUKRS")
                            //            };


                            
                            var EMPArray2 = Dt.AsEnumerable().ToList();
                            foreach (var item in EMPArray2) 
                            {
                                /*var EMPArray = EMPArray2.AsEnumerable().FirstOrDefault().ItemArray*/;
                                var EMPArray = item.ItemArray;

                                Empleado = db.Empleados.First(x => x.Id == empleado.Id);
                                //int NroEmpleado = Convert.ToInt32(Empleado.NroEmpleado);
                                if (EMPArray[0].ToString()== Empleado.NroEmpleado) 
                                {
                                
                                
                                //Empleado.Empresa = query.FirstOrDefault().Sociedad;
                                Empleado.Empresa = EMPArray[3].ToString();
                                Empleado.UnidadOrganizativa = EMPArray[7].ToString();
                                Empleado.Area = EMPArray[8].ToString();
                                Empleado.A_UsuarioModifica = "CONSOLA9";
                                Empleado.A_Modificacion = DateTime.Now;
                                db.SaveChanges();

                                if (db.PersonalActivo.Where(x => x.CodigoEmpleado == Empleado.NroEmpleado).ToList().Count() < 1)
                                {

                                    var personalActivo = new PersonalActivo();
                                    personalActivo.Area = Empleado.Area;
                                    personalActivo.Cargo = Dt.Rows[0]["CARGO"].ToString();
                                    personalActivo.CodigoEmpleado = Empleado.NroEmpleado;
                                    personalActivo.Documento = Empleado.Documento;
                                    personalActivo.Empresa = Empleado.Empresa;
                                    personalActivo.Nombres = Empleado.Nombres;
                                    personalActivo.UnidadOrganizativa = Empleado.UnidadOrganizativa;
                                    db.PersonalActivo.Add(personalActivo);
                                    db.SaveChanges();
                                }
                                }
                            }
                            
                            
                        }
                        }
                 

                    var fileSavePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/LogJob"), "logJobUPDEmpleado.txt");


                    // Write the string array to a new file named "WriteLines.txt".
                    using (StreamWriter w = File.AppendText(fileSavePath))
                    {
                        w.Write(Environment.NewLine);
                        w.Write("[{0} {1}]", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                        w.Write("\t");
                        w.WriteLine(" {0}", "Ejecución correcta Empl: " + DateTime.Now);
                        w.WriteLine("-----------------------");
                    }


                }
                catch (Exception ex)
                {
                    var fileSavePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/LogJob"), "logJobUPDEmpleado.txt");

                    // Write the string array to a new file named "WriteLines.txt".
                    using (StreamWriter w = File.AppendText(fileSavePath))
                    {
                        w.Write(Environment.NewLine);
                        w.Write("[{0} {1}]", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                        w.Write("\t");
                        w.WriteLine(" {0}", "Error Empl: " + ex.Message.ToString() + DateTime.Now);
                        w.WriteLine("-----------------------");
                    }


                }


            }

        }

        public void  ActualizarDatosEmpleado(string CodigoEmpleado)
        {
           
       
            using (var db = new AutogestionContext())
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
                            function.SetValue("I_PERNR", CodigoEmpleado);

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
                             string tipoarea = function.GetStructure("T_RESULT").GetString("PTEXT");
                             string cargo = function.GetStructure("T_RESULT").GetString("CARGO");
                            string Documento = function.GetStructure("T_RESULT").GetString("PERID"); 
                    if (PrimerNombre != "")
                            {

                                DateTime enteredDate = DateTime.Parse(InicioContrato);
                                Empleado empleado = new Empleado();
                                empleado = db.Empleados.FirstOrDefault(x => x.NroEmpleado == CodigoEmpleado);
                                empleado.FechaIngreso = enteredDate;
                                empleado.TipoArea = tipoarea;
                                empleado.AreaDescripcion = AreaPersonal;
                                empleado.Cargo = cargo;
                                if (empleado.Documento.Trim() != Documento.Trim()) 
                                {
                                    //empleado.Documento = Documento.Trim();
                                }
                                //empleado.A_UsuarioModifica = "ADE99999";
                                //empleado.A_Modificacion = DateTime.Now;
                                db.SaveChanges();

                                PersonalActivo personal = new PersonalActivo();
                                personal = db.PersonalActivo.FirstOrDefault(x => x.CodigoEmpleado == CodigoEmpleado);

                        //AnaA
                                //personal.Area = AreaPersonal;
                                //db.SaveChanges();

                          
                            }
                            else
                            {
                           
                            }

                        }
                        catch (SystemException ex)
                        {
                            // ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
                        }
                        catch (RfcLogonException ex)
                        {
                     
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
                }



        public  DataTable ValidarEmpleadoActivo(string documento)
        {

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
                config.Add(RfcConfigParameters.LogonGroup, "FoscalHana");

              //  if (DestinacionConfiguracion != null) RfcDestinationManager.UnregisterDestinationConfiguration(DestinacionConfiguracion);

                if (RfcDestinationManager.IsDestinationConfigurationRegistered() == false) { 
                  RfcDestinationManager.RegisterDestinationConfiguration(DestinacionConfiguracion);
                }
              
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


           
            }
            catch (RfcInvalidStateException ex)
            {
                try
                {
                    RfcDestinationManager.UnregisterDestinationConfiguration(DestinacionConfiguracion);
                }
                catch
                {

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
            if(DestinacionConfiguracion!= null) RfcDestinationManager.UnregisterDestinationConfiguration(DestinacionConfiguracion);

                // DestinacionConfiguracion.RemoveDestination("SAP");
            }

            string resultado = "";


            //}


            return Datos;

        }


        public void ActualizarEstructuraJerarquicaEmpleado()
        {
            
            List<EstructuraJerarquica> estructura = new List<EstructuraJerarquica>();
            DateTime Fecha = Convert.ToDateTime(DateTime.Today);
            Empleado ItemEmpleado = new Empleado();
            using (var db = new AutogestionContext())
            {

                try
                {

                    estructura = db.EstructuraJerarquica.Where(x=>x.UnidadOrg !="" && x.UnidadOrg != null).ToList();
                    List<Empleado> Items = db.Empleados.Where(e => e.Activo =="SI").ToList();
                    List<Empleado> EmpleadosActualizar = new List<Empleado>();
                    List<EstructuraJerarquica> EstructurasActualizar = new List<EstructuraJerarquica>();
                    var LisEmpleadosxAreas = db.Empleados.Where(s => s.Activo == "SI" && (s.AreaDescripcion != null && s.AreaDescripcion != "")).ToList();
                    

                    foreach (var model in estructura)
                    {
                        string UnidadOrg ="" + model.UnidadOrg;
                        if (UnidadOrg.Length < 8) 
                        {
                            int n = UnidadOrg.Length;
                            while (n < 8)
                            {
                                UnidadOrg = "0" + UnidadOrg;
                                n++;
                            }
                            model.A_Modificacion = DateTime.Now;
                            model.A_UsuarioModifica = "STR99999";
                            model.UnidadOrg = UnidadOrg;
                            db.SaveChanges();
                        }
                        var listaAreas2 = LisEmpleadosxAreas.Where(s => s.Empresa == model.Sociedad&& s.UnidadOrganizativa == UnidadOrg).GroupBy(b => b.UnidadOrganizativa).FirstOrDefault();
                        if (listaAreas2 != null) 
                        {
                            var Area = listaAreas2.FirstOrDefault();
                            if (Area.AreaDescripcion != model.Area && Area.AreaDescripcion!=null && Area.AreaDescripcion !="")
                            {
                                model.Area = Area.AreaDescripcion;
                                model.A_Modificacion = DateTime.Now;
                                model.A_UsuarioModifica = "STR99999";
                                
                                //EstructurasActualizar.Add(model);
                                db.SaveChanges();
                            }
                        }
                       
                      
                        List<Empleado> Items2 = Items.Where(e => e.UnidadOrganizativa == UnidadOrg && e.Empresa == model.Sociedad).ToList();
                        if (Items2 != null)
                        {
                            foreach (Empleado Item in Items2)
                            {
                                bool Añadir = false;
                                db.Empleados.Attach(Item);
                                if (Item.NroEmpleado == model.Jefe)
                                {
                                    if (Item.Jefe != model.Superior || Item.Superior != model.Superior || Item.Director != model.Director || Item.Lider != model.Lider) 
                                    {
                                        Item.Jefe = model.Superior;
                                        Item.Superior = model.Superior;
                                        Item.Director = model.Director;
                                        Item.A_Modificacion = DateTime.Now;
                                        Item.A_UsuarioModifica = "STR99999";
                                        if (Item.NroEmpleado != model.Lider) 
                                        {
                                            Item.Lider = model.Lider;
                                        }
                                        else
                                        {
                                            Item.Lider = "";
                                        }

                                        Añadir = true;
                                    }
                                    
                                }
                                else
                                {
                                    if (Item.Jefe != model.Jefe || Item.Superior != model.Superior || Item.Director != model.Director || Item.Lider != model.Lider) 
                                    {
                                        Item.Jefe = model.Jefe;
                                        Item.Superior = model.Superior;
                                        Item.Director = model.Director;
                                        Item.A_Modificacion = DateTime.Now;
                                        Item.A_UsuarioModifica = "STR99999";
                                        if (Item.NroEmpleado != model.Lider)
                                        {
                                            Item.Lider = model.Lider;
                                        }
                                        else
                                        {
                                            Item.Lider = "";
                                        }
                                        Añadir = true;
                                    }
                                   
                                }
                             
                                Empleado duplicado = EmpleadosActualizar.Where(x=>x.Id == Item.Id).FirstOrDefault();
                                if (duplicado == null && Añadir ==true) 
                                {
                                    //EmpleadosActualizar.Add(Item);
                                    db.SaveChanges();
                                }

                            }
                        }

                        //List<PersonalActivo> Items2 = db.PersonalActivo.Where(e => e.Area == model.Area && e.Empresa == model.Sociedad).ToList();
                        //if (Items2 != null)
                        //{
                        //    foreach (PersonalActivo Item2 in Items2)
                        //    {
                        //        db.PersonalActivo.Attach(Item2);
                        //        if (Item2.CodigoEmpleado == model.Jefe)
                        //        {
                        //            Item2.Jefe = model.Superior;
                        //            Item2.Superior = model.Superior;
                        //            Item2.Director = model.Director;
                        //        }
                        //        else
                        //        {
                        //            Item2.Jefe = model.Jefe;
                        //            Item2.Superior = model.Superior;
                        //            Item2.Director = model.Director;
                        //        }
                        //        db.SaveChanges();
                        //    }
                        //}
                    
                    
                    }
                    //if (EmpleadosActualizar.Count() > 0 || EstructurasActualizar.Count()>0) 
                    //{
                    //    db.SaveChanges();
                    //}
                    var fileSavePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/LogJob"), "logJobUPDEmpleado.txt");


                    // Write the string array to a new file named "WriteLines.txt".
                    using (StreamWriter w = File.AppendText(fileSavePath))
                    {
                        w.Write(Environment.NewLine);
                        w.Write("[{0} {1}]", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                        w.Write("\t");
                        w.WriteLine(" {0}", "Ejecución correcta Estrc: " + DateTime.Now);
                        w.WriteLine("-----------------------");
                    }


                }
                catch (Exception ex) {

                    var fileSavePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/LogJob"), "logJobUPDEmpleado.txt");

                    // Write the string array to a new file named "WriteLines.txt".
                    using (StreamWriter w = File.AppendText(fileSavePath))
                    {
                        w.Write(Environment.NewLine);
                        w.Write("[{0} {1}]", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                        w.Write("\t");
                        w.WriteLine(" {0}", "Error Estrc: " + ex.Message.ToString() + DateTime.Now);
                        w.WriteLine("-----------------------");
                    }
                
                }
            }
        
        }
        public class AreasAdmAutogestion
        {
            public string UnidadOrganizativa { get; set; }
            public string NombreArea { get; set; }
            public string Sociedad { get; set; }
            public string Estructura { get; set; }
        }

        public string ActualizarEstructuraJerarquica_unidadOr()
        {

            List<EstructuraJerarquica> ListEstructuras = new List<EstructuraJerarquica>();
            DateTime Fecha = Convert.ToDateTime(DateTime.Today);
            Empleado ItemEmpleado = new Empleado();
            using (var db = new AutogestionContext())
            {

                try
                {
                    ListEstructuras = db.EstructuraJerarquica.ToList();
                    List<Empleado> Items = db.Empleados.Where(e => e.Activo == "SI").ToList();
                    List<EstructuraJerarquica> EstructurasActualizar = new List<EstructuraJerarquica>();
                    List<EstructuraJerarquica> EstructurasEliminar = new List<EstructuraJerarquica>();

                    List<AreasAdmAutogestion> Areas = new List<AreasAdmAutogestion>();
                    var lista1000 = db.Empleados.Where(s => s.Empresa == "1000" && s.Activo == "SI" && (s.AreaDescripcion != null || s.AreaDescripcion != "")).GroupBy(b => b.UnidadOrganizativa).ToList();
                    foreach (var x in lista1000)
                    {
                        
                        Empleado Datos = x.FirstOrDefault();
                        AreasAdmAutogestion area = new AreasAdmAutogestion();
                        area.NombreArea = Datos.AreaDescripcion;
                        area.UnidadOrganizativa = Datos.UnidadOrganizativa;
                        area.Sociedad = Datos.Empresa;
                        Areas.Add(area);
                       
                    }

                    var lista2000 = db.Empleados.Where(s => s.Empresa == "2000" && s.Activo == "SI" && (s.AreaDescripcion != null || s.AreaDescripcion != "")).GroupBy(b => b.UnidadOrganizativa).ToList();
                    foreach (var x in lista2000)
                    {
                        Empleado Datos = x.FirstOrDefault();
                        AreasAdmAutogestion area = new AreasAdmAutogestion();
                        area.NombreArea = Datos.AreaDescripcion;
                        area.UnidadOrganizativa = Datos.UnidadOrganizativa;
                        area.Sociedad = Datos.Empresa;
                        Areas.Add(area);
                    }

                    foreach (var Area in Areas)
                    {
                        EstructuraJerarquica EstructuraArea = ListEstructuras.Where(x => x.Area == Area.NombreArea && x.Sociedad == Area.Sociedad).FirstOrDefault();
                        List<EstructuraJerarquica> EstructuraAreaD = ListEstructuras.Where(x => x.Area == Area.NombreArea && x.Sociedad == Area.Sociedad && x.Id != EstructuraArea.Id).ToList();
                        
                        if (EstructuraAreaD.Count()>=1) 
                        {
                            EstructurasEliminar.AddRange(EstructuraAreaD);
                        }
                        if (EstructuraArea != null)
                        {
                            db.EstructuraJerarquica.Attach(EstructuraArea);
                            EstructuraArea.UnidadOrg = Area.UnidadOrganizativa;
                            EstructuraArea.A_Modificacion = DateTime.Now;
                            EstructuraArea.A_UsuarioModifica = "ACTESTR9";
                            EstructurasActualizar.Add(EstructuraArea);
                        }



                    }

                    if (EstructurasEliminar.Count()>0) 
                    {
                        db.EstructuraJerarquica.RemoveRange(EstructurasEliminar);
                        db.SaveChanges();
                    }

                    if (EstructurasActualizar.Count() > 0)
                    {
                        
                        db.SaveChanges();
                    }
                    //var fileSavePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/LogJob"), "logJobUPDEmpleado.txt");


                    //// Write the string array to a new file named "WriteLines.txt".
                    //using (StreamWriter w = File.AppendText(fileSavePath))
                    //{
                    //    w.Write(Environment.NewLine);
                    //    w.Write("[{0} {1}]", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                    //    w.Write("\t");
                    //    w.WriteLine(" {0}", "Ejecución correcta Estrc: " + DateTime.Now);
                    //    w.WriteLine("-----------------------");
                    //}
                    return "";

                }
                catch (Exception ex)
                {
                    return "" + ex;
                    //var fileSavePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/LogJob"), "logESTRJerarquica.txt");

                    //// Write the string array to a new file named "WriteLines.txt".
                    //using (StreamWriter w = File.AppendText(fileSavePath))
                    //{
                    //    w.Write(Environment.NewLine);
                    //    w.Write("[{0} {1}]", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                    //    w.Write("\t");
                    //    w.WriteLine(" {0}", "Error Estrc: " + ex.Message.ToString() + DateTime.Now);
                    //    w.WriteLine("-----------------------");
                    //}

                }
            }

        }

        public async Task<bool> GenerarCertificadoIngresos(string url, string documento, string correo) {
            try
            {

         

             HttpClient httpClient = new HttpClient();

        var formData = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "entry.1876140230", documento },
            { "entry.464028261",  correo}
            });

             httpClient.Timeout = TimeSpan.FromSeconds(5);
            var response = await httpClient.PostAsync(url, formData);
            var statusCode = (int)response.StatusCode;

            if (statusCode == 0 || statusCode == 200)
            {

                return true;
            }
            return false;
            }
            catch (TaskCanceledException ex)
            {

                return false;
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
            }

       
        }





