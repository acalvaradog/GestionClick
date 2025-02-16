using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Services.Description;

namespace Adm_AutoGestion.Services
{
    public class EventosRepository
    {
        private ServiciosRepository _services;
        public EventosRepository()
        {
            _services = new ServiciosRepository();
        }

        public string Crear(Eventos model, EventosRelacionados model2)
        {
            string message = "";
            using (var db = new AutogestionContext()) {
                try
                {
                    if (model.DirigidoA == "General")
                    {
                        model.Cupo = 0;
                        model.RegistroRequerido = false;
                        model.LinkEncuestaAsistidos = null;
                        model.LinkEncuestaNoAsistidos = null;
                        model.FechaCierre = null;
                        model.HoraCierre = null;
                        model.ParentescoPermitido = "Todos";
                        model.EdadLimite = 0;
                    }

                    if (model.EdadLimite <= -1)
                    {
                        model.EdadLimite = 0;
                    }
                    if (model.ParentescoPermitido == "")
                    {
                        model.ParentescoPermitido = "Todos";
                    }

                    if (model.TipoEvento == "1" || model.TipoEvento == "3")
                    {
                        model.FechaFin = model.FechaInicio;
                    }
                    model.FechaPublicacion = DateTime.Now;
                    model.Mes = DateTime.Now.ToString("MMMM", CultureInfo.CreateSpecificCulture("es"));
                    model.Estado = "NoPublicado";
                    model.Presupuesto = "0";
                    db.Eventos.Add(model);
                    db.SaveChanges();
                    model2.EventosId2 = model.Id;
                    db.EventosRelacionados.Add(model2);
                    db.SaveChanges();
                    message = "true";
                } catch (Exception ex)
                {
                    message = "Error, " + ex;
                }
            } return message;
        }

        internal bool BorrarFamiliar(int FamiliarId)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    var registro = db.Familiar.Find(FamiliarId);
                    db.Familiar.Remove(registro);
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }

            }
        }

        internal bool BorrarInscripcionEmpleado(int EmpleadoId, int EventoId)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    var evento = db.Eventos.Find(EventoId);
                    var fechaCierre = Convert.ToDateTime(evento.FechaCierre);
                    var horaCierre = Convert.ToDateTime(evento.HoraCierre);
                    var fechaActual = DateTime.Now;
                    DateTime fechaCombinada = fechaCierre.Date.Add(horaCierre.TimeOfDay);
                    var EsFechaLimite = fechaCombinada <= fechaActual;
                    if (!EsFechaLimite)
                    {
                        var registro = db.DetalleEventos.FirstOrDefault(e => e.EventosId == EventoId && e.EmpleadoId == EmpleadoId && e.FamiliarId == null);
                        db.DetalleEventos.Remove(registro);
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }

            }
        }

        internal bool BorrarInscripcionFamiliar(int FamiliarId, string EventoId)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    var id = Convert.ToString(FamiliarId);
                    var eventoid = Convert.ToInt32(EventoId);
                    var evento = db.Eventos.Find(eventoid);
                    var fechaCierre = Convert.ToDateTime(evento.FechaCierre);
                    var horaCierre = Convert.ToDateTime(evento.HoraCierre);
                    var fechaActual = DateTime.Now;
                    DateTime fechaCombinada = fechaCierre.Date.Add(horaCierre.TimeOfDay);
                    var EsFechaLimite = fechaCombinada <= fechaActual;
                    if (!EsFechaLimite)
                    {
                        var registro = db.DetalleEventos.FirstOrDefault(e => e.EventosId == eventoid && e.FamiliarId == id);
                        db.DetalleEventos.Remove(registro);
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }

            }
        }

        internal string GuardarEventoEmpleado(DetalleEventos model)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    db.DetalleEventos.Add(model);
                    db.SaveChanges();
                    return "true";
                }
                catch
                {
                    return "false";
                }

            }
        }

        internal string GuardarEventoFamiliar(DetalleEventos model)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    db.DetalleEventos.Add(model);
                    db.SaveChanges();
                    return "true";
                }
                catch
                {
                    return "false";
                }
            }
        }

        internal bool GuardarFamiliar(Familiar model)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    db.Familiar.Add(model);
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }

            }
        }

        internal bool ActualizarEdad(int EmpleadoId)
        {
            try
            {
                using (var db = new AutogestionContext())
                {
                    List<Familiar> familiares = db.Familiar.Where(e => e.EmpleadoId == EmpleadoId).ToList();

                    foreach (var i in familiares)
                    {
                        DateTime FechaNac = Convert.ToDateTime(i.FechaNacimiento);
                        var Nac = int.Parse(FechaNac.ToString("yyyyMMdd"));
                        var FechaActual = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                        var Edad = (FechaActual - Nac) / 10000;

                        Familiar familiar = new Familiar();
                        familiar = db.Familiar.FirstOrDefault(e => e.Id == i.Id);
                        db.Familiar.Attach(familiar);

                        familiar.Edad = Edad;
                        db.SaveChanges();

                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal string SubirImagen(string Id, string NombreImg)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    Eventos eventos = new Eventos();
                    int.TryParse(Id, out int id);
                    eventos = db.Eventos.FirstOrDefault(e => e.Id == id);
                    db.Eventos.Attach(eventos);

                    eventos.Imagen = NombreImg;
                    db.SaveChanges();
                    return "true";
                }
                catch
                {
                    return "false";
                }
            }

        }

        internal void Modificar(string Id, string Cupo, string FechaCierre, string HoraCierre, string HoraInicio, string HoraFin, string FechaInicio, string FechaFin, string Descripcion)
        {
            using (var db = new AutogestionContext())
            {
                try
                {

                    Eventos eventos = new Eventos();
                    int.TryParse(Id, out int id);
                    int.TryParse(Cupo, out int cupo);
                    DateTime.TryParse(HoraInicio, out DateTime horainicio);
                    DateTime.TryParse(HoraFin, out DateTime horafin);
                    DateTime.TryParse(FechaInicio, out DateTime fechainicio);

                    eventos = db.Eventos.FirstOrDefault(e => e.Id == id);
                    db.Eventos.Attach(eventos);

                    if (FechaFin != null)
                    {
                        DateTime.TryParse(FechaFin, out DateTime fechafin);
                        eventos.FechaFin = fechafin;
                    }
                    if (FechaCierre != null)
                    {
                        DateTime.TryParse(FechaCierre, out DateTime fechacierre);
                        eventos.FechaCierre = fechacierre;
                    }
                    if (HoraCierre != null)
                    {
                        DateTime.TryParse(HoraCierre, out DateTime horacierre);
                        eventos.HoraCierre = horacierre;
                    }
                    
                    eventos.Cupo = cupo;
                    eventos.HoraInicio = horainicio;
                    eventos.HoraFin = horafin;
                    eventos.FechaInicio = fechainicio;
                    eventos.Descripcion = Descripcion;
                    db.SaveChanges();
                }
                catch
                {

                }
            }
        }

        internal void Modificar1(string Id, string LinkEncuestaAsistidos)
        {
            using(var db = new AutogestionContext())
            {
                try
                {
                    Eventos eventos = new Eventos();
                    int.TryParse(Id, out int id);

                    eventos = db.Eventos.FirstOrDefault(e => e.Id == id);
                    db.Eventos.Attach(eventos);

                    eventos.LinkEncuestaAsistidos = LinkEncuestaAsistidos;
                    db.SaveChanges();
                }
                catch
                {

                }
            }
        }

        internal void Modificar2(string Id, string LinkEncuestaNoAsistidos)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    Eventos eventos = new Eventos();
                    int.TryParse(Id, out int id);

                    eventos = db.Eventos.FirstOrDefault(e => e.Id == id);
                    db.Eventos.Attach(eventos);

                    eventos.LinkEncuestaNoAsistidos = LinkEncuestaNoAsistidos;
                    db.SaveChanges();
                }
                catch
                {

                }
            }
        }

        internal void Modificar3(string Id, string Presupuesto)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    Eventos eventos = new Eventos();
                    int.TryParse(Id, out int id);

                    eventos = db.Eventos.FirstOrDefault(e => e.Id == id);
                    db.Eventos.Attach(eventos);

                    eventos.Presupuesto = Presupuesto;
                    db.SaveChanges();
                }
                catch
                {

                }
            }
        }

        internal string CambiarEstadoEvento(string Id)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    Eventos eventos = new Eventos();
                    int.TryParse(Id, out int id);
                    eventos = db.Eventos.FirstOrDefault(e => e.Id == id);
                    if(eventos.Imagen != null)
                    {
                        if (eventos.Estado == "NoPublicado")
                        {
                            db.Eventos.Attach(eventos);
                            eventos.Estado = "Activo";
                            eventos.FechaPublicacion = DateTime.Now;
                            db.SaveChanges();
                            return "exito";
                        }
                        else
                        {
                            db.Eventos.Attach(eventos);
                            eventos.Estado = "NoPublicado";
                            db.SaveChanges();
                            return "exito";
                        }
                    }
                    else
                    {
                        return "noimagen";
                    }
                }
                catch (Exception ex)
                {
                    return "Error: "+ex;
                }
            }
        }

        internal void AnularEvento(string Id)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    Eventos eventos = new Eventos();
                    int.TryParse(Id, out int id);
                    eventos = db.Eventos.FirstOrDefault(e => e.Id == id);
                    db.Eventos.Attach(eventos);
                    eventos.Estado = "Anulado";
                    db.SaveChanges();
                }
                catch
                {

                }
            }
        }

        internal void CambiarFechaAsistencia(int id, int eventoId)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    DetalleEventos eventos = new DetalleEventos();
                    
                    eventos = db.DetalleEventos.Find(id);
                    db.DetalleEventos.Attach(eventos);

                    eventos.EventosId = eventoId;
                    db.SaveChanges();
                }
                catch
                {

                }
            }
        }

        internal void CerrarEvento(string Id)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    Eventos eventos = new Eventos();
                    int.TryParse(Id, out int id);

                    eventos = db.Eventos.FirstOrDefault(e => e.Id == id);
                    db.Eventos.Attach(eventos);

                    eventos.Estado = "Cerrado";
                    db.SaveChanges();
                }
                catch
                {

                }
            }
        }

        internal string FirmarManual(int Id)
        {
            using (var db = new AutogestionContext())
            {
                string message = "";
                try
                {
                    DetalleEventos eventos = new DetalleEventos();
                    eventos = db.DetalleEventos.FirstOrDefault(e => e.Id == Id);
                    db.DetalleEventos.Attach(eventos);

                    var fechaHoy = DateTime.Now.ToString();
                    eventos.FechaFirma = fechaHoy + "| Manual";
                    db.SaveChanges();
                    message = "true";
                }
                catch (Exception ex)
                {
                    message = "Error" + ex;
                    return message;
                }
                return message;
            }
        }
        internal string FirmarQR(string id, int idevento/*, int idempleado*/)
        {
            using (var db = new AutogestionContext())
            {
                string message = "";

                try
                {
                    string datosqr = _services.desencriptar(id);
                    string[] datostrabajador = datosqr.Split('|');
                    var NroTrabajador = datostrabajador[0];
                    
                    DetalleEventos query = new DetalleEventos();
                    //Query para obtener el registro del empleado que se quiere firmar
                    Empleado queryempleado = db.Empleados.FirstOrDefault(e=> e.NroEmpleado == NroTrabajador);
                    var EmpleadoId = queryempleado.Id;
                    //Comprobar que el QR sea del empleado correcto
                    if (queryempleado != null)
                    {
                            query = db.DetalleEventos.FirstOrDefault(e => e.EventosId == idevento && e.EmpleadoId == EmpleadoId && e.FamiliarId == null);

                        if (query != null)
                        {
                            if (query.FechaFirma != null)
                            {
                                message = "-3";
                                return message;

                            }
                            db.DetalleEventos.Attach(query);
                            var fechaHoy = DateTime.Now.ToString();
                            query.FechaFirma = fechaHoy + "| QR";
                            db.SaveChanges();
                            message = "true";
                        }
                        else
                        {
                            message = "-1";
                        }
                    }
                    else
                    {
                        return message = "-2";
                    }
                }
                catch (Exception ex)
                {
                    message = "Error" + ex;
                    return message;
                }
                return message;
            }
        }

        public string EnvioCorreoGeneral(int id, string Asunto, string Texto)
        {
            string message = "";
            using (var db = new AutogestionContext())
            {
                string txtde = Properties.Settings.Default.Correo.ToString();
                string contraseñacorreo = "";
                var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
                
                var evento = db.DetalleEventos.Where(e=> e.EventosId == id).ToList();
                var CantidadCorreos = evento.Count();
                var correos = "";
                var contador = 0;
                foreach (var i in evento)
                {
                    var emp = db.Empleados.FirstOrDefault(e => e.Id == i.EmpleadoId);
                    var correoEmpleado = emp.Correo;
                    correos += correoEmpleado;
                    contador++;
                    if (contador != CantidadCorreos)
                    {
                        correos += ",";
                    }
                }

                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

                try
                {
                    correo.To.Add(correos);
                    correo.Subject = Asunto;
                    correo.Body = Texto;
                    correo.IsBodyHtml = true;

                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp-relay.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    smtp.Send(correo);
                    message = "true";
                    
                }
                catch (Exception ex)
                {
                    message = "Error, " + ex;
                }
            }return message;
        }

        public string EnvioCorreosEncuestasAsistentes(int id)
        {
            var message = "";
            using (var db = new AutogestionContext())
            {
                string txtde = Properties.Settings.Default.Correo.ToString();
                string contraseñacorreo = "";
                var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);

                var evento = db.Eventos.Find(id);

                if (evento.LinkEncuestaAsistidos == "")
                {
                    return "NoLink";
                }
                else
                {
                    Configuraciones RangoCorreos = db.Configuraciones.Where(x => x.Parametro == "EVENTOS_RANGO_CORREO").FirstOrDefault();
                    Configuraciones TextoCorreo = db.Configuraciones.Where(x => x.Parametro == "EVEN_TXTCORREOENCUESTA").FirstOrDefault();
                    int RGCorreo = Convert.ToInt32(RangoCorreos.Valor);
                    var Asunto = "Encuesta por Asistencia al Evento " + evento.NombreEvento;
                    
                    var Texto = TextoCorreo.Valor;
                    Texto = Texto.Replace("$NOMBREEVENTO", evento.NombreEvento);
                    Texto = Texto.Replace("$LINK", evento.LinkEncuestaAsistidos);
                    var Asistentes = db.DetalleEventos.Where(e => e.EventosId == id && e.FechaFirma != null).ToList();

                    var CantidadCorreos = Asistentes.Count();
                    var correos = "";
                    var contador = 0;
                    var contadorTotal = 0;
                    var CntEnvio = 1;
                    foreach (var i in Asistentes)
                    {
                        var emp = db.Empleados.FirstOrDefault(e => e.Id == i.EmpleadoId);
                        contador++;
                        contadorTotal++;
                        if (emp.Correo != null && emp.Correo!="") 
                        {

                            var correoEmpleado = emp.Correo;
                            bool Valid = _services.EmailValido(correoEmpleado);
                            if (Valid==true) 
                            {
                                correos += correoEmpleado;

                                if ((contador != RGCorreo) && (contadorTotal != CantidadCorreos))
                                {
                                    correos += " , ";
                                }
                            }
                            
                        }
                       
                        
                       
                        //Envio correo Por rangos
                        if (contador == RGCorreo) 
                        {
                            try
                            {
                                System.Net.Mail.MailMessage correoTG = new System.Net.Mail.MailMessage();
                                correoTG.From = new System.Net.Mail.MailAddress(txtde);
                                correoTG.To.Add(correos);
                                correoTG.Subject = Asunto;
                                correoTG.Body = Texto;
                                correoTG.IsBodyHtml = true;

                                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                                smtp.Host = "smtp-relay.gmail.com";
                                smtp.Port = 587;
                                smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                                smtp.EnableSsl = true;
                                //if (CntEnvio > 1) 
                                //{
                                //    smtp.Send(correoTG);
                                //}
                                smtp.Send(correoTG);
                                //RESETEO DE VARIABLES
                                correos = "";
                                contador = 0;
                                CntEnvio++;

                            }
                            catch (Exception ex)
                            {
                                message += "Error, " + ex;
                            }
                        }
                    }

                   
                    if (correos!="") 
                    {
                        try
                        {
                            System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                            correo.From = new System.Net.Mail.MailAddress(txtde);
                            correo.To.Add(correos);
                            correo.Subject = Asunto;
                            correo.Body = Texto;
                            correo.IsBodyHtml = true;
                            FormatException formatException = new FormatException();
                            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                            smtp.Host = "smtp-relay.gmail.com";
                            smtp.Port = 587;
                            smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                            smtp.EnableSsl = true;
                            smtp.Send(correo);
                            return "True";
                        }
                        catch (Exception ex)
                        {
                            message= "Error, " + ex;
                        }
                    }
                    message = "True";
                    return message;
                }
            } 
        }

        public string EnvioCorreosEncuestasNoAsistentes(int id)
        {
            using (var db = new AutogestionContext())
            {
                string txtde = Properties.Settings.Default.Correo.ToString();
                string contraseñacorreo = "";
                var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);

                var evento = db.Eventos.Find(id);

                if (evento.LinkEncuestaNoAsistidos == "")
                {
                    return "NoLink";
                }
                else
                {
                    Configuraciones RangoCorreos = db.Configuraciones.Where(x => x.Parametro == "EVENTOS_RANGO_CORREO").FirstOrDefault();
                    Configuraciones TextoCorreo = db.Configuraciones.Where(x => x.Parametro == "EVEN_TXTCORREOENCUESTA").FirstOrDefault();
                    int RGCorreo = Convert.ToInt32(RangoCorreos.Valor);
                    var Asunto = "Encuesta de No Asistencia al Evento " + evento.NombreEvento;
                    var Texto = TextoCorreo.Valor;
                    Texto = Texto.Replace("$NOMBREEVENTO", evento.NombreEvento);
                    Texto = Texto.Replace("$LINK", evento.LinkEncuestaAsistidos);

                    var NoAsistentes = db.DetalleEventos.Where(e => e.EventosId == id && e.FechaFirma == null).ToList();

                    var CantidadCorreos = NoAsistentes.Count();
                    var correos = "";
                    var contador = 0;
                    var contadorTotal = 0;
                    foreach (var i in NoAsistentes)
                    {
                        var emp = db.Empleados.FirstOrDefault(e => e.Id == i.EmpleadoId);
                        contador++;
                        contadorTotal++;
                        if (emp.Correo != null && emp.Correo != "")
                        {
                            var correoEmpleado = emp.Correo;
                            bool Valid = _services.EmailValido(correoEmpleado);
                            if (Valid == true)
                            {
                                correos += correoEmpleado;
                                if ((contador != RGCorreo) && (contadorTotal != CantidadCorreos))
                                {
                                    correos += ",";
                                }
                            }
                        }
                        

                       
                        //Envio correo Por rangos
                        if (contador == RGCorreo)
                        {
                            try
                            {
                                System.Net.Mail.MailMessage correoTG = new System.Net.Mail.MailMessage();
                                correoTG.From = new System.Net.Mail.MailAddress(txtde);
                                correoTG.To.Add(correos);
                                correoTG.Subject = Asunto;
                                correoTG.Body = Texto;
                                correoTG.IsBodyHtml = true;

                                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                                smtp.Host = "smtp-relay.gmail.com";
                                smtp.Port = 587;
                                smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                                smtp.EnableSsl = true;
                                smtp.Send(correoTG);
                                //RESETEO DE VARIABLES
                                correos = "";
                                contador = 0;


                            }
                            catch (Exception ex)
                            {
                                return "Error, " + ex;
                            }
                        }

                    }

                    System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                    correo.From = new System.Net.Mail.MailAddress(txtde);
                    if (correos != "")
                    {
                        try
                        {
                            correo.To.Add(correos);
                            correo.Subject = Asunto;
                            correo.Body = Texto;
                            correo.IsBodyHtml = true;

                            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                            smtp.Host = "smtp-relay.gmail.com";
                            smtp.Port = 587;
                            smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                            smtp.EnableSsl = true;
                            smtp.Send(correo);
                            return "True";

                        }
                        catch (Exception ex)
                        {
                            return "Error, " + ex;
                        }
                    }
                    return "True";
                }
            }
        }
    
    
    
    
    }
}