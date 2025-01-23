using Adm_AutoGestion.Models;
using Adm_AutoGestion.Models.EvaDesempeno;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls.Expressions;
using System.Windows;


namespace Adm_AutoGestion.Services
{
    public class ViaticoRepository
    {
        ServiciosRepository _serv = new ServiciosRepository();
        internal void Crear(Viaticos model)
        {
            try { 
            using (var db = new AutogestionContext())
            {
                int numero = 1;
                model.Estado = numero ;
                model.FechaRegistro = DateTime.Now;
                model.FechaFin = model.FechaFin.AddHours(18);
                db.Viaticos.Add(model);
                db.SaveChanges();
                    Empleado Emp = db.Empleados.Where(x=>x.Id == model.EmpleadoId).FirstOrDefault();
                    Empleado Jefe = db.Empleados.Where(x=> x.NroEmpleado == Emp.Jefe).FirstOrDefault();
                    notificar_Solicitud(model.Id, Jefe.Correo, Emp.Id ,model.FechaRegistro,model.MtvViaje, "solicitado");




            }
            }catch (Exception ex) 
            {
              Console.WriteLine("" + ex);
            }
        }
        internal bool Cancelar(int Id, string Observacion)
        {
            try
            {
                using (var db = new AutogestionContext())
                {
                    Viaticos viatico = db.Viaticos.Where(e=>e.Id==Id).FirstOrDefault();

                    db.Viaticos.Attach(viatico);
                    viatico.Observacion = Observacion;
                    viatico.Estado = 5;

                    db.SaveChanges();
                    Empleado emp = db.Empleados.Where(e => e.Id == viatico.EmpleadoId).FirstOrDefault();
                    Empleado jefe = db.Empleados.Where(e => e.NroEmpleado == emp.Jefe).FirstOrDefault();
                    notificar_Solicitud(viatico.Id, jefe.Correo, viatico.EmpleadoId, DateTime.Now, viatico.MtvViaje, "cancelado");

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("" + ex);
                return false;
            }
        }

        public List<Viaticos> ObtenerTodos(string orden, string Empleadolog)
        {
            List<Viaticos> Items2 = new List<Viaticos>();
            using (var db = new AutogestionContext())
            {
                

                int id = -1;
                int id2 = -1;
                Int32.TryParse(orden, out id);
                Int32.TryParse(Empleadolog, out id2);

                //Items2 = db.Viaticos.Where(e => e.EstadosViaticos.id == id).ToList();

                if (orden == "JefeDirecto")
                {
                    var Jefecito = db.Empleados.FirstOrDefault(e => e.Id == id2);
                    var IdJefecito = Jefecito.NroEmpleado;


                    List<Empleado> EmpA = (from emp in db.Empleados
                                where emp.Jefe == IdJefecito && emp.Activo!="NO" select emp).ToList();

                    //Items2 = db.Viaticos.Where(e => e.Estado == 1).ToList();
                    foreach (var emp in EmpA) 
                    {
                        List<Viaticos> cont =db.Viaticos.Where(e=>e.EmpleadoId ==emp.Id && e.Estado==1).OrderBy(x => x.FechaInicio).ToList();

                        foreach (var viatic in cont) 
                        {
                        Items2.Add(viatic);
                        }
                        

                    }

                    foreach (Viaticos Item in Items2)
                    {
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                        Item.DestinoViatico = db.DestinoViaticos.FirstOrDefault(e => e.Id == Item.DestinoViaticoID);
                        Item.EstadosViaticos = db.EstadosViaticos.FirstOrDefault(e => e.id == Item.Estado);
                      


                    }
                }
                if (orden == "GestionHumana")
                {
                    Items2 = db.Viaticos.Where(e => e.Estado == 2).OrderByDescending(X => X.FechaInicio).ThenBy(x => x.Placa).ToList();
                    foreach (Viaticos Item in Items2)
                    {
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                        Item.DestinoViatico = db.DestinoViaticos.FirstOrDefault(e => e.Id == Item.DestinoViaticoID);
                        Item.EstadosViaticos = db.EstadosViaticos.FirstOrDefault(e => e.id == Item.Estado);



                    }
                }
                if (orden == "Tesoreria")
                {
                    Items2 = db.Viaticos.Where(e => e.Estado == 3 && e.CheckTesoreria == false && e.CheckNomina==true).OrderByDescending(X => X.FechaInicio).ThenBy(x => x.Placa).ToList();
                    foreach (Viaticos Item in Items2)
                    {
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                        Item.DestinoViatico = db.DestinoViaticos.FirstOrDefault(e => e.Id == Item.DestinoViaticoID);
                        Item.EstadosViaticos = db.EstadosViaticos.FirstOrDefault(e => e.id == Item.Estado);



                    }
                }
                if (orden == "Nomina")
                {
                    Items2 = db.Viaticos.Where(e => e.Estado == 3 && e.CheckNomina == false).OrderByDescending(X => X.FechaInicio).ThenBy(x=>x.Placa).ToList();
                    foreach (Viaticos Item in Items2)
                    {
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                        Item.DestinoViatico = db.DestinoViaticos.FirstOrDefault(e => e.Id == Item.DestinoViaticoID);
                        Item.EstadosViaticos = db.EstadosViaticos.FirstOrDefault(e => e.id == Item.Estado);



                    }
                }
                
                if (orden == "NominaCarge")
                {
                    Items2 = db.Viaticos.Where(e => e.Estado == 3 && e.CheckNomina == true && e.CheckNominaCargue == false).OrderByDescending(X => X.FechaInicio).ThenBy(x => x.Placa).ToList();
                    foreach (Viaticos Item in Items2)
                    {
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                        Item.DestinoViatico = db.DestinoViaticos.FirstOrDefault(e => e.Id == Item.DestinoViaticoID);
                        Item.EstadosViaticos = db.EstadosViaticos.FirstOrDefault(e => e.id == Item.Estado);



                    }
                }


            }
       
            return Items2;
        }

        public List<Viaticos> ObtenerTodos2( string ID)
        {
            using (var db = new AutogestionContext())
            {
                List<Viaticos> Items2=db.Viaticos.ToList();

                return Items2;
            }
        }
        public Viaticos ObtenerDetails(string EmpleadoId)
        {
            using (var db = new AutogestionContext())
            {
                Viaticos Items2 = new Viaticos();

                int id = -1;
                Int32.TryParse(EmpleadoId, out id);



                Items2 = db.Viaticos.FirstOrDefault(e => e.Id == id);

                Items2.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Items2.EmpleadoId);
                Items2.DestinoViatico = db.DestinoViaticos.FirstOrDefault(e => e.Id == Items2.DestinoViaticoID);
                Items2.EstadosViaticos = db.EstadosViaticos.FirstOrDefault(e => e.id == Items2.Estado);

                return Items2;


            }
        }

        internal bool Modificar(int Id, string Estado, string Observaciones)
        {

            using (var db = new AutogestionContext())
            {
                try
                {

                    Viaticos Viaticos = new Viaticos();

                    //int Ident = 2 ;
                    int Ident2 = 0;
                    
                 
                    int.TryParse(Estado, out Ident2);

                    Viaticos = db.Viaticos.FirstOrDefault(e => e.Id == Id);
                    db.Viaticos.Attach(Viaticos);
                    Viaticos.Estado = Ident2;
                    Viaticos.Observacion = Observaciones;
                    //Empleado emp = db.Empleados.Where(e => e.Id == Viaticos.EmpleadoId).FirstOrDefault();
                    //Empleado jefe = db.Empleados.Where(e => e.NroEmpleado == emp.NroEmpleado).FirstOrDefault();
                    //notificar_Solicitud(Viaticos.Id, jefe.Correo, Viaticos.EmpleadoId, Viaticos.FechaRegistro, Viaticos.MtvViaje, "AprovadoJefe");
                    db.SaveChanges();
                    return true;

                }
                catch
                {
                    return false;
                }
            }

        }

        internal bool ModificarGestion(int Id, string Estado, string GastosA, string GastosT, string Empleadolog, string Observaciones)
        {

            using (var db = new AutogestionContext())
            {
                try
                {
                  

                    Viaticos Viaticosvar = new Viaticos();
                    ViaticosLog Viaticoslog1= new ViaticosLog();
                    ViaticosLog Viaticoslog2 = new ViaticosLog();
                    ViaticosLog Viaticoslog3 = new ViaticosLog();

                    int Ident = 0;
                    int GTanterior = 0;
                    int GAanterior = 0;
                    int GT = 0;
                    int GA = 0;
                    int EmpId = Convert.ToInt32(Empleadolog);
                    Empleado EmpDatos = db.Empleados.Where(x=>x.Id== EmpId).FirstOrDefault();
                   
                    int.TryParse(Estado, out Ident);
                    int.TryParse(GastosT, out GT);
                    int.TryParse(GastosA, out GA);

                    

                    Viaticosvar = db.Viaticos.FirstOrDefault(e => e.Id == Id);
                    

                    GTanterior = Viaticosvar.GastosTransporte;
                    GAanterior = Viaticosvar.GastoAlimentacion;

                    if (GAanterior != GA && (Estado != "Denegar" || Estado != "4")) 
                    {
                        Viaticoslog1.Fecha = DateTime.Now;
                        Viaticoslog1.ViaticoId = Viaticosvar.Id;
                        Viaticoslog1.ValorAnterior = GAanterior;
                        Viaticoslog1.ValorNuevo = GA;
                        Viaticoslog1.Usuario = EmpDatos.NroEmpleado;
                        Viaticoslog1.ViaticoId = Viaticosvar.Id;
                        Viaticoslog1.Observaciones = "Se han hecho cambios en los Gasto Alimentación";

                        db.ViaticosLogs.Add(Viaticoslog1);
                        
                    }
                    if (GTanterior !=GT && (Estado != "Denegar" || Estado != "4")) 
                    {
                        Viaticoslog2.Fecha = DateTime.Now;
                        Viaticoslog2.ViaticoId = Viaticosvar.Id;
                        Viaticoslog2.ValorAnterior = GAanterior;
                        Viaticoslog2.ValorNuevo = GT;
                        Viaticoslog2.Usuario = EmpDatos.NroEmpleado;
                        Viaticoslog2.ViaticoId = Viaticosvar.Id;
                        Viaticoslog2.Observaciones = "Se han hecho cambios en los Gastos de transporte";

                        db.ViaticosLogs.Add(Viaticoslog2);
                     

                    }
                    var Opcion = "GestionHumana";
                    GenerarLog(Viaticosvar.Id, EmpId, Convert.ToInt32(Estado), Opcion, Observaciones);
                
                    db.Viaticos.Attach(Viaticosvar);
                    Viaticosvar.GastosTransporte = GT;
                    Viaticosvar.GastoAlimentacion = GA;
                    Viaticosvar.Estado = Ident;


                    db.SaveChanges();

                    return true;

                }
                catch
                {
                    return false;
                }
            }

        }
        internal void ModificarTesNom(int Id, string Check, int EmpleadoLog, string CodContabilizacionSAP)
        {
           
            using (var db = new AutogestionContext())
            {
               
                try
                {
                    string Observaciones = "";
                    Viaticos Viaticos = new Viaticos();
                    if (Check == "Nomina") {

                        Viaticos = db.Viaticos.FirstOrDefault(e => e.Id == Id);
                        db.Viaticos.Attach(Viaticos);
                        Viaticos.CheckNomina = true;
                        Viaticos.CodContabilizacionSAP = CodContabilizacionSAP;
                        GenerarLog(Viaticos.Id, EmpleadoLog, Viaticos.Estado, Check, Observaciones);
                        db.SaveChanges();
                        NotificarTesoreria(Viaticos , Viaticos.EmpleadoId);
                        NotificarNóminaCargue(Viaticos, Viaticos.EmpleadoId);
                    }
                    if (Check == "Tesoreria")
                    {

                        Viaticos = db.Viaticos.FirstOrDefault(e => e.Id == Id);
                        db.Viaticos.Attach(Viaticos);
                        Viaticos.CheckTesoreria = true;
                        if (Viaticos.CheckNominaCargue == true) 
                        {
                            Viaticos.Estado = 6;
                        }
                        GenerarLog(Viaticos.Id, EmpleadoLog, Viaticos.Estado, Check, Observaciones);
                        db.SaveChanges();
                    }
                    if (Check == "NominaCargue")
                    {

                        Viaticos = db.Viaticos.FirstOrDefault(e => e.Id == Id);
                        db.Viaticos.Attach(Viaticos);
                        Viaticos.CheckNominaCargue = true;
                        if (Viaticos.CheckTesoreria == true)
                        {
                            Viaticos.Estado = 6;
                        }
                        GenerarLog(Viaticos.Id, EmpleadoLog, Viaticos.Estado, Check, Observaciones);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());   
                }
            }

        }

        internal void GenerarLog (int Id, int User, int Estado, string Opcion, string ObservacionesC)
        {
            using (var db=  new AutogestionContext()) 
            { 
            
                var viaticoslog= new ViaticosLog();
                var trabajador=db.Empleados.FirstOrDefault(e => e.Id == User);
                var observaciones = "";
                if (Opcion == "JefeDirecto" && Estado==2) { observaciones = "El jefe directo ha aprovado la solicitud del viático"; }
                else if((Opcion == "JefeDirecto" && Estado != 2)) { observaciones = "El Jefe Directo ha verificado el viático y lo ha Denegado: "+ ObservacionesC; }
                if (Opcion == "Tesorería" ) { observaciones = "El area de Tesorería ha Verificado el viático"; }
                if (Opcion == "Nómina") { observaciones = "El area de Nómina ha verificado el viático"; }
                if (Opcion == "NominaCargue") { observaciones = "Se ha realizado el cague a SAP"; }               
                if (Opcion == "GestionHumana" && Estado == 3) { observaciones = "El area de Gestión ha verificado el viático y lo ha cerrado para continuar el proceso"; }
                else if (Opcion == "GestionHumana" && Estado != 3) { observaciones = "El área de Gestión Humana ha verificado el viático y lo ha Denegado. "+ ObservacionesC; }
                var est = db.EstadosViaticos.Where(e=>e.id== Estado).FirstOrDefault();
                viaticoslog.Usuario = trabajador.NroEmpleado;
                viaticoslog.Estado = est.Nombre;
                viaticoslog.Observaciones = observaciones;
                viaticoslog.Fecha=DateTime.Now;
                viaticoslog.ViaticoId = Id;


                db.ViaticosLogs.Add(viaticoslog);
                db.SaveChanges();

            }
        
        }
         public Uri Url { get; }
        public string VerificarViaje(string cadena)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    var cadena2 = _serv.desencriptar(cadena);
                    char[] delimiterChars1 = { '%' };
                    string[] words1 = cadena2.Split(delimiterChars1);
                    int Id = Convert.ToInt32(words1[0]);
                    int EmpID = Convert.ToInt32(words1[1]);
                    Viaticos Viatico = db.Viaticos.Where(x => x.Id == Id && x.EmpleadoId == EmpID).FirstOrDefault();
                    Empleado Emp = db.Empleados.Where(x => x.Id == EmpID).FirstOrDefault();
                    db.Viaticos.Attach(Viatico);
                    if (Viatico.ViajeRealizado == false)
                    {
                        Viatico.ViajeRealizado = true;

                        ViaticosLog newlog = new ViaticosLog();
                        newlog.Fecha = DateTime.Now;
                        newlog.Usuario = "" + Emp.NroEmpleado;
                        newlog.Observaciones = "El usuario ha confirmado que ha realizado el viaje";
                        newlog.Estado = "Cerrado";
                        string hostname = HttpContext.Current.Request.Url.Host;
                        db.SaveChanges();

                        return "Confirmación exitosa";
                    }
                    else
                    {
                        return "ViajeRealizado";
                    }
                }
                catch (Exception ex) 
                {
                  return ""+ex.Message;
                }
            }
        }

        public string VerificarViajeManual(int ViaticoId, string Observaciones, bool ViajeRealizado, string UserLog)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    int IdJefe = Convert.ToInt32(UserLog);
                    Viaticos Viatico = db.Viaticos.Where(x => x.Id == ViaticoId).FirstOrDefault();
                    Empleado Jefe = db.Empleados.Where(x => x.Id == IdJefe).FirstOrDefault();

                        Viatico.ViajeRealizado = ViajeRealizado;
                        Viatico.ViajeRealizadoObservacion = Observaciones;
                        ViaticosLog newlog = new ViaticosLog();
                        newlog.Fecha = DateTime.Now;
                        newlog.Usuario = Jefe.NroEmpleado;
                        newlog.Observaciones = "El usuario ha confirmado que ha realizado el viaje";
                        newlog.Estado = "Cerrado";
                        newlog.ViaticoId = Viatico.Id;
                        string hostname = HttpContext.Current.Request.Url.Host;
                        db.ViaticosLogs.Add(newlog);
                        db.SaveChanges();

                        return "Exito";
                }
                catch (Exception ex)
                {
                    return "" + ex.Message;
                }
            }
        }
        public string AnularViaje(int ViaticoId, string UserLog)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    int IdJefe = Convert.ToInt32(UserLog);
                    Viaticos Viatico = db.Viaticos.Where(x => x.Id == ViaticoId).FirstOrDefault();
                    Empleado Emp = db.Empleados.Where(x => x.Id == IdJefe).FirstOrDefault();
                    int EstadoViatico = Viatico.Estado;
                    Viatico.Estado = 5;
                    ViaticosLog newlog = new ViaticosLog();
                    newlog.Fecha = DateTime.Now;
                    newlog.Usuario = Emp.NroEmpleado;
                    newlog.Observaciones = "El Jefe ha Cancelado el proceso del viático";
                    newlog.Estado = "Anulado";
                    newlog.ViaticoId = Viatico.Id;
                    string hostname = HttpContext.Current.Request.Url.Host;
                    db.ViaticosLogs.Add(newlog);
                    db.SaveChanges();
                    if (EstadoViatico ==3 || EstadoViatico ==6) 
                    {
                        var res = NotificarVTCancelacion(Viatico, IdJefe, EstadoViatico);
                    }

                    return "Exito";
                }
                catch (Exception ex)
                {
                    return "" + ex.Message;
                }
            }
        }
        public bool notificar_Solicitud(int IdViatico, string EmailJefe, int usuarioid, DateTime fecha, string Motivo, string Solicitud)
        {
            using (var db = new AutogestionContext())
            {

                string textocorreo = "";
                bool confirmacion;
                string txtde = Properties.Settings.Default.Correo.ToString();
                string contraseñacorreo = "";
                var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
                var viatico = db.Viaticos.Where(e => e.Id == IdViatico).FirstOrDefault();
                var Fecha2 = String.Format("{0:dd-MM-yyyy}", fecha); ;
                var email = "";
                var AREASOLICITUD = "";
                Empleado EMP = db.Empleados.Where(x => x.Id == usuarioid).FirstOrDefault();
                DestinoViatico Destin = db.DestinoViaticos.Where(x => x.Id == viatico.DestinoViaticoID).FirstOrDefault() ;

                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

                string texto = "";
                if (Solicitud == "solicitado")
                {
                    AREASOLICITUD = "Jefe Directo";
                    texto = db.Configuraciones.First(s => s.Parametro == "TXTVIATICOCREATE").Valor.ToString();
                    //email = db.Configuraciones.First(s => s.Parametro == "EMAILGESTIONHUMANA").Valor.ToString(); // TEMPORAL    
                    //Codigo Email
                    email = EmailJefe;                                                                                             //email= EmailJefe;


                    texto = texto.Replace("$ID", Convert.ToString(IdViatico));
                    texto = texto.Replace("$FECHA", Fecha2);
                    texto = texto.Replace("$MOTIVO", Motivo);
                    texto = texto.Replace("$NOMBRE_EMP", EMP.Nombres);
                    texto = texto.Replace("$NMREMP", EMP.NroEmpleado);


                }
                if (Solicitud == "AprovadoJefe")
                {
                    AREASOLICITUD = "Gestión Humana";
                    texto = db.Configuraciones.First(s => s.Parametro == "TXTVIATICOCREATE").Valor.ToString();
                    email = db.Configuraciones.FirstOrDefault(s => s.Parametro == "EMAILGESTIONHUMANA").Valor.ToString();

                    texto = texto.Replace("$ID", Convert.ToString(IdViatico));
                    texto = texto.Replace("$FECHA", Fecha2);
                    texto = texto.Replace("$MOTIVO", Motivo);
                    texto = texto.Replace("$NOMBRE_EMP", EMP.Nombres);
                    texto = texto.Replace("$NMREMP", EMP.NroEmpleado);

                }
                if (Solicitud == "cancelado")//EMPLEADO QUE CANCELA LA SOLICITUD
                {
                    texto = db.Configuraciones.First(s => s.Parametro == "TXTVIATICOCANCEL").Valor.ToString();
                    AREASOLICITUD = "Cancelación";
                    email = EmailJefe;
                    texto = texto.Replace("$ID", Convert.ToString(IdViatico));
                    texto = texto.Replace("$FECHA", Fecha2);
                    texto = texto.Replace("$MOTIVO", Motivo);
                    texto = texto.Replace("$OBSERVACIONES", viatico.Observacion);
                    texto = texto.Replace("$NOMBRE_EMP", EMP.Nombres);
                    texto = texto.Replace("$NMREMP", EMP.NroEmpleado);
                    texto = texto.Replace("$DESTINO", Destin.Nombre);
                }




                textocorreo = texto;



                try
                {


                    correo.To.Add(email);

                    correo.Subject = "Notificación Solicitud Viático - "+ AREASOLICITUD;
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

        public bool notificar_Cierre(int IdViatico, string Emailemp, int usuarioid, DateTime fecha, string Motivo, string Solicitud, string Destino)
        {
            using (var db = new AutogestionContext())
            {
                Empleado Emp = db.Empleados.Where(x => x.Id == usuarioid).FirstOrDefault();
                string textocorreo = "";
                bool confirmacion;
                string txtde = Properties.Settings.Default.Correo.ToString();
                string contraseñacorreo = "";
                var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
                var Fecha2 = String.Format("{0:dd-MM-yyyy}", fecha); ;
                var email = "";               
                try
                {
                    System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                    correo.From = new System.Net.Mail.MailAddress(txtde);

                    string texto = "";
                    texto = db.Configuraciones.First(s => s.Parametro == "TXTVIATICOFINAL").Valor.ToString();
                    email = Emailemp;
                    texto = texto.Replace("$ID", Convert.ToString(IdViatico));
                    texto = texto.Replace("$FECHA", Fecha2);
                    texto = texto.Replace("$MOTIVO", Motivo);
                    texto = texto.Replace("$SOLICITUD", Solicitud.ToLower());
                    texto = texto.Replace("$DESTINO", Destino);
                    textocorreo = texto;


                    correo.To.Add(email);

                    correo.Subject = "Notificación Solicitud Viático - Solicitud " + Solicitud.ToUpper();
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

        public bool NotificarBienestar(int IdViatico, int usuarioid, DateTime fecha, DateTime fecha2) 
        {

                try
                {

                using (var db = new AutogestionContext())
                {
                    string textocorreo = "";
                    bool confirmacion;
                    string txtde = Properties.Settings.Default.Correo.ToString();
                    string contraseñacorreo = "";
                    var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                    contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
                    string FechaS1 = String.Format("{0:dd-MM-yyyy}", fecha);
                    string FechaS2 = String.Format("{0:dd-MM-yyyy}", fecha2);
                    var email = "";

                    Empleado Emp = db.Empleados.Where(x => x.Id == usuarioid).FirstOrDefault();
                    Viaticos VT = db.Viaticos.Where(x => x.Id == IdViatico).FirstOrDefault();
                    DestinoViatico destinoVT = db.DestinoViaticos.Where(x => x.Id == VT.DestinoViaticoID).FirstOrDefault();

                    System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                    correo.From = new System.Net.Mail.MailAddress(txtde);

                    string texto = "";

                    texto = db.Configuraciones.First(s => s.Parametro == "TXTVIATICOBIENESTAR").Valor.ToString();

                    email = db.Configuraciones.First(s => s.Parametro == "CORREOCOORDINADORBIENESTAR").Valor.ToString();
                    //Codigo Email
                    //email= Emailemp;
                    string Hospedaje = "";
                    if (VT.Hospedaje == true)
                    { Hospedaje = "SI"; }
                    else
                    { Hospedaje = "NO"; }

                    texto = texto.Replace("$EMPLEADO%", Emp.Nombres);
                    texto = texto.Replace("%CCNMR%", Emp.Documento);
                    texto = texto.Replace("%DESTINO%", destinoVT.Nombre);
                    texto = texto.Replace("%FECHAINICIO%", FechaS1);
                    texto = texto.Replace("%FECHAFIN%", FechaS2);
                    texto = texto.Replace("%HOSPEDAJE%", Hospedaje);

                    textocorreo = texto;

                    correo.To.Add(email);

                    correo.Subject = "Notificación Solicitud Viático - Bienestar";
                    correo.Body = textocorreo;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;

                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    smtp.Send(correo);

                    return true;
                }
            }
             catch(Exception ex) 
            {
                return false;
            }
        }
        public bool NotificarARL(int IdViatico, int usuarioid, DateTime fecha, DateTime fecha2)
        {

            try
            {

                using (var db = new AutogestionContext())
                {
                    string textocorreo = "";
                    string txtde = Properties.Settings.Default.Correo.ToString();
                    string contraseñacorreo = "";
                    var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                    contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
                    string FechaS1 = String.Format("{0:dd-MM-yyyy}", fecha);
                    string FechaS2 = String.Format("{0:dd-MM-yyyy}", fecha2);
                    var email = "";
                    Empleado Emp = db.Empleados.Where(x => x.Id == usuarioid).FirstOrDefault();
                    Viaticos VT = db.Viaticos.Where(x => x.Id == IdViatico).FirstOrDefault();
                    DestinoViatico destinoVT = db.DestinoViaticos.Where(x=>x.Id == VT.DestinoViaticoID).FirstOrDefault();
                    System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                    correo.From = new System.Net.Mail.MailAddress(txtde);

                    string texto = "";
                    texto = db.Configuraciones.First(s => s.Parametro == "TXT_VTNOTIFICACIONARL").Valor.ToString();

                    email = db.Configuraciones.First(s => s.Parametro == "VIAT_CORREOARL").Valor.ToString();
                    //Codigo Email
                    //email= Emailemp;
                    string Hospedaje = "";
                    if (VT.Hospedaje ==true) 
                    { Hospedaje = "SI";}
                    else 
                    { Hospedaje = "NO";}

                    texto = texto.Replace("$EMPLEADO%", Emp.Nombres);
                    texto = texto.Replace("%CCNMR%", Emp.Documento);
                    texto = texto.Replace("%DESTINO%", destinoVT.Nombre);
                    texto = texto.Replace("%FECHAINICIO%", FechaS1);
                    texto = texto.Replace("%FECHAFIN%", FechaS2);
                    texto = texto.Replace("%HOSPEDAJE%", Hospedaje);


                    textocorreo = texto;

                    correo.To.Add(email);

                    correo.Subject = "Notificación Solicitud Viático ARL";
                    correo.Body = textocorreo;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;

                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    smtp.Send(correo);

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool NotificarTesoreria(Viaticos VT, int usuarioid)
        {

            try
            {

                using (var db = new AutogestionContext())
                {
                    string textocorreo = "";
                    string txtde = Properties.Settings.Default.Correo.ToString();
                    string contraseñacorreo = "";
                    var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                    contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
                    var email = "";
                    Empleado Emp = db.Empleados.Where(x => x.Id == usuarioid).FirstOrDefault();
                    //Viaticos VT = db.Viaticos.Where(x => x.Id == IdViatico).FirstOrDefault();
                    DestinoViatico destinoVT = db.DestinoViaticos.Where(x => x.Id == VT.DestinoViaticoID).FirstOrDefault();
                    string FechaS1 = String.Format("{0:dd-MM-yyyy}", VT.FechaInicio);
                    string FechaS2 = String.Format("{0:dd-MM-yyyy}", VT.FechaFin);
                    System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                    correo.From = new System.Net.Mail.MailAddress(txtde);

                    string texto = "";
                    texto = db.Configuraciones.First(s => s.Parametro == "TXT_VTTESORERIA").Valor.ToString();

                    email = db.Configuraciones.First(s => s.Parametro == "VT_CORREO_TESORERIA").Valor.ToString();
                    texto = texto.Replace("$ID", "" + VT.Id);
                    texto = texto.Replace("$EMPLEADO", Emp.Nombres);
                    texto = texto.Replace("$DOCUMENTO", Emp.Documento);
                    texto = texto.Replace("$NMREMPLEADO", Emp.NroEmpleado);
                    texto = texto.Replace("$SOCIEDAD", Emp.Empresa);
                    texto = texto.Replace("$CARGO", Emp.Cargo);
                    texto = texto.Replace("$UNIDADORG", Emp.UnidadOrganizativa);
                    texto = texto.Replace("$AREA", Emp.AreaDescripcion);
                    texto = texto.Replace("$FECHAINICIO", FechaS1);
                    texto = texto.Replace("$FECHAFINAL", FechaS2);
                    texto = texto.Replace("%TPAGO", "" + VT.Total);
                    texto = texto.Replace("%CODCONT", "" + VT.CodContabilizacionSAP);

                    textocorreo = texto;

                    correo.To.Add(email);

                    correo.Subject = "Notificación Solicitud Viático - Tesorería";
                    correo.Body = textocorreo;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;

                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    smtp.Send(correo);

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool NotificarNóminaContabilizacion(Viaticos VT, int usuarioid)
        {

            try
            {

                using (var db = new AutogestionContext())
                {
                    string textocorreo = "";
                    string txtde = Properties.Settings.Default.Correo.ToString();
                    string contraseñacorreo = "";
                    var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                    contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
                    var email = "";
                    Empleado Emp = db.Empleados.Where(x => x.Id == usuarioid).FirstOrDefault();
                    string FechaS1 = String.Format("{0:dd-MM-yyyy}", VT.FechaInicio);
                    string FechaS2 = String.Format("{0:dd-MM-yyyy}", VT.FechaFin);
                    DestinoViatico destinoVT = db.DestinoViaticos.Where(x => x.Id == VT.DestinoViaticoID).FirstOrDefault();
                    System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                    correo.From = new System.Net.Mail.MailAddress(txtde);

                    string texto = "";
                    texto = db.Configuraciones.First(s => s.Parametro == "TXT_VTNOMINA").Valor.ToString();

                    email = db.Configuraciones.First(s => s.Parametro == "VT_CORREO_NOMINACONTABILIZACION").Valor.ToString();
                    texto = texto.Replace("$ID", "" + VT.Id);
                    texto = texto.Replace("$EMPLEADO", Emp.Nombres);
                    texto = texto.Replace("$DOCUMENTO", Emp.Documento);
                    texto = texto.Replace("$NMREMPLEADO", Emp.NroEmpleado);
                    texto = texto.Replace("$SOCIEDAD", Emp.Empresa);
                    texto = texto.Replace("$CARGO", Emp.Cargo);
                    texto = texto.Replace("$UNIDADORG", Emp.UnidadOrganizativa);
                    texto = texto.Replace("$AREA", Emp.AreaDescripcion);
                    texto = texto.Replace("$FECHAINICIO", FechaS1);
                    texto = texto.Replace("$FECHAFINAL", FechaS2);
                    texto = texto.Replace("%TPAGO", "" + VT.Total);



                    textocorreo = texto;

                    correo.To.Add(email);

                    correo.Subject = "Notificación Solicitud Viático - Nómina Contabilización";
                    correo.Body = textocorreo;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;

                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    smtp.Send(correo);

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool NotificarNóminaCargue(Viaticos VT, int usuarioid)
        {

            try
            {

                using (var db = new AutogestionContext())
                {
                    string textocorreo = "";
                    string txtde = Properties.Settings.Default.Correo.ToString();
                    string contraseñacorreo = "";
                    var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                    contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
                   
                    var email = "";
                    Empleado Emp = db.Empleados.Where(x => x.Id == usuarioid).FirstOrDefault();
                    //Viaticos VT = db.Viaticos.Where(x => x.Id == IdViatico).FirstOrDefault();
                    DestinoViatico destinoVT = db.DestinoViaticos.Where(x => x.Id == VT.DestinoViaticoID).FirstOrDefault();
                    string FechaS1 = String.Format("{0:dd-MM-yyyy}", VT.FechaInicio);
                    string FechaS2 = String.Format("{0:dd-MM-yyyy}", VT.FechaFin);
                    System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                    correo.From = new System.Net.Mail.MailAddress(txtde);

                    string texto = "";
                    texto = db.Configuraciones.First(s => s.Parametro == "TXT_VTNOMINA").Valor.ToString();

                    email = db.Configuraciones.First(s => s.Parametro == "VT_CORREO_NOMINACARGUE").Valor.ToString();
                    texto = texto.Replace("$ID", "" + VT.Id);
                    texto = texto.Replace("$EMPLEADO", Emp.Nombres);
                    texto = texto.Replace("$DOCUMENTO", Emp.Documento);
                    texto = texto.Replace("$NMREMPLEADO", Emp.NroEmpleado);
                    texto = texto.Replace("$SOCIEDAD", Emp.Empresa);
                    texto = texto.Replace("$CARGO", Emp.Cargo);
                    texto = texto.Replace("$UNIDADORG", Emp.UnidadOrganizativa);
                    texto = texto.Replace("$AREA", Emp.AreaDescripcion);
                    texto = texto.Replace("$FECHAINICIO", FechaS1);
                    texto = texto.Replace("$FECHAFINAL", FechaS2);
                    texto = texto.Replace("%TPAGO", "" + VT.Total);


                    textocorreo = texto;

                    correo.To.Add(email);

                    correo.Subject = "Notificación Solicitud Viático - Cargue SAP";
                    correo.Body = textocorreo;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;

                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    smtp.Send(correo);

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool NotificarVTCancelacion(Viaticos VT, int JEFE, int Estado)
        {

            try
            {

                using (var db = new AutogestionContext())
                {
                    string textocorreo = "";
                    string txtde = Properties.Settings.Default.Correo.ToString();
                    string contraseñacorreo = "";
                    var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                    contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);

                    var email = "";
                    Empleado JEFE2 = db.Empleados.Where(x => x.Id == JEFE).FirstOrDefault();
                    Empleado Emp = db.Empleados.Where(x => x.Id == VT.EmpleadoId).FirstOrDefault();
                    //Viaticos VT = db.Viaticos.Where(x => x.Id == IdViatico).FirstOrDefault();
                    DestinoViatico destinoVT = db.DestinoViaticos.Where(x => x.Id == VT.DestinoViaticoID).FirstOrDefault();
                    string FechaS1 = String.Format("{0:dd-MM-yyyy}", VT.FechaInicio);
                    string FechaS2 = String.Format("{0:dd-MM-yyyy}", VT.FechaFin);
                    System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                    correo.From = new System.Net.Mail.MailAddress(txtde);

                    string texto = "";
                    texto = db.Configuraciones.First(s => s.Parametro == "TXT_VTCANCELACIONJEFE").Valor.ToString();
                    if (Estado == 2) 
                    {
                        email = email+" " + db.Configuraciones.First(s => s.Parametro == "EMAILGESTIONHUMANA").Valor.ToString();
                    }
                    if (VT.Hospedaje ==true && Estado >= 3) 
                    {
                        email = email + " " + db.Configuraciones.First(s => s.Parametro == "CORREOCOORDINADORBIENESTAR").Valor.ToString();
                    }
                    if (Estado >= 3)
                    {
                        email = email + " " + db.Configuraciones.First(s => s.Parametro == "VT_CORREO_NOMINACONTABILIZACION").Valor.ToString();
                    }
                    if (VT.CheckNomina == true)
                    {
                        email = email + " " + db.Configuraciones.First(s => s.Parametro == "VT_CORREO_TESORERIA").Valor.ToString();
                        email = email + " " + db.Configuraciones.First(s => s.Parametro == "VT_CORREO_NOMINACARGUE").Valor.ToString();
                    }
                    texto = texto.Replace("$JEFE", JEFE2.Nombres);
                    texto = texto.Replace("$NRMSOLICITUD", "" + VT.Id);
                    texto = texto.Replace("$EMPLEADO", Emp.Nombres);
                    texto = texto.Replace("$DOCUMENTO", Emp.Documento);
                    texto = texto.Replace("$NMREMPLEADO", Emp.NroEmpleado);
                    texto = texto.Replace("$SOCIEDAD", Emp.Empresa);
                    texto = texto.Replace("$CARGO", Emp.Cargo);
                    texto = texto.Replace("$UNIDADORG", Emp.UnidadOrganizativa);
                    texto = texto.Replace("$AREA", Emp.AreaDescripcion);
                    texto = texto.Replace("$FECHAINICIO", FechaS1);
                    texto = texto.Replace("$FECHAFINAL", FechaS2);
                    texto = texto.Replace("%TPAGO", "" + VT.Total);


                    textocorreo = texto;

                    correo.To.Add(email);

                    correo.Subject = "Notificación Solicitud Viático - Cancelación por el Jefe";
                    correo.Body = textocorreo;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;

                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    smtp.Send(correo);

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}