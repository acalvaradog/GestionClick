using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Adm_AutoGestion.Services;
using System.Data;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;


namespace Adm_AutoGestion.Services
{
    public class RetirosRepository
    {

        public List<Retiros> ObtenerTodos()
        {
            using (var db = new AutogestionContext())
            {


                List<Retiros> Items = db.Retiros.ToList();
                foreach (Retiros Item in Items)
                {
                    Item.ListadoMotivos = db.Motivos.ToList();


                }
                return Items;
            }
        }



        public string Crear(Retiros model, string Telefono, string CorreoI, string CorreoP)
        {
            string message = "";
            string opcion = "Crear";
            using (var db = new AutogestionContext())
            {
                try
                {

                    Empleado empleado = new Empleado();
                    PersonalActivo ps = new PersonalActivo();
                    PazySalvo pazysalvo = new PazySalvo();
                    DetallePazySalvo detalle = new DetallePazySalvo();

                    empleado = db.Empleados.FirstOrDefault(e => e.Id == model.IdEmpleado);

                    model.Fecha = DateTime.Now;
                    model.Id = 0;
                    model.UsuarioModifica = null;
                    model.FechaModifica = null;
                    model.Estado = "RETIRO VIGENTE";
                    model.Empresa = empleado.Empresa;
                    db.Retiros.Add(model);
                    db.SaveChanges();

                    
                    db.Empleados.Attach(empleado);
                    //empleado.Activo = "NO";
                    empleado.Telefono = Telefono;
                    empleado.Correo = CorreoI;
                    empleado.CorreoPersonal = CorreoP;
                    //empleado.FechaFin = model.FechaTerminacion;

                    if (model.MotivoCancelacion == 8)
                    {
                        empleado.Activo = "SI";
                        if (empleado.Empresa == "1000")
                        {
                            empleado.Empresa = "2000";
                        }
                        else
                        {
                            empleado.Empresa = "1000";
                        }

                        db.SaveChanges();
                    }
                    else
                    {

                        db.SaveChanges();


                        ps = db.PersonalActivo.FirstOrDefault(e => e.Documento == empleado.Documento);
                        if (ps != null)
                        {
                            db.PersonalActivo.Remove(ps);
                            db.SaveChanges();
                        }
                    }

                    //---------------- paz y salvo ---------------//


                    //var areas = db.AreasPazySalvo.GroupBy(g => g.Area).Select(s => s.Key).ToList();
                    //string asunto = "Creación Paz y Salvo";
                    //pazysalvo.Fecha = DateTime.Now;
                    //pazysalvo.EmpleadoId = model.IdEmpleado;
                    //pazysalvo.Estado = "Activo";
                    //pazysalvo.RetiroId = model.Id;
                    //pazysalvo.Empresa = model.Empresa;
                    //db.PazySalvo.Add(pazysalvo);
                    //db.SaveChanges();
                    ////--------------paz y salvo ---------//
                    //NotificarCorreoPyS(asunto, empleado.Nombres, empleado.Documento, empleado.NroEmpleado, pazysalvo.Id, model.FechaTerminacion.ToString(), opcion);

                    //foreach (var fila in areas)
                    //{
                    //    detalle.IdPazySalvo = pazysalvo.Id;
                    //    detalle.Area = fila;
                    //    detalle.Responsable = 0;
                    //    db.DetallePazySalvo.Add(detalle);
                    //    db.SaveChanges();
                    //}

                    //**************************************//


                    message = "Ok";
                    
                }
                catch
                {
                    message = "Error";
                }
            }

            return message;

        }

        internal void Editar(Retiros model, string Telefono, string CorreoI, string CorreoP)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    Retiros Retiros = new Retiros();
                    Empleado empleado = new Empleado();
                    PazySalvo pys = new PazySalvo();


                    empleado = db.Empleados.FirstOrDefault(e => e.Id == model.IdEmpleado);
                    db.Empleados.Attach(empleado);
                    //empleado.Activo = "SI";
                    empleado.Telefono = Telefono;
                    empleado.Correo = CorreoI;
                    empleado.CorreoPersonal = CorreoP;
                    empleado.FechaFin = model.FechaTerminacion;
                    db.SaveChanges();


                    Retiros = db.Retiros.Find(model.Id);
                    db.Retiros.Attach(Retiros);
                    Retiros.MotivoCancelacion = model.MotivoCancelacion;
                    Retiros.Observacion = model.Observacion;
                    Retiros.Estado = model.Estado;
                    Retiros.FechaModifica = DateTime.Now;
                    Retiros.Liquidacion = model.Liquidacion;
                    Retiros.Empresa = empleado.Empresa;
                    Retiros.FechaTerminacion = model.FechaTerminacion;
                    //db.Entry(Retiros).State = EntityState.Modified;
                    db.SaveChanges();
                    

                    

                    if (model.Estado == "RETIRO ANULADO")
                    {
                        string opcion = "Anular";
                        string asunto = "Paz y Salvo Anulado";
                        pys = db.PazySalvo.FirstOrDefault(x => x.RetiroId == Retiros.Id);
                        db.PazySalvo.Attach(pys);
                        pys.Estado = "Anulado";
                        db.SaveChanges();
                        NotificarCorreoPyS(asunto, empleado.Nombres, empleado.Documento, empleado.NroEmpleado, pys.Id, model.FechaTerminacion.ToString(), opcion);
                    }
                    


                }
                catch
                { }
            }
        }


        internal void ActualizarEnvioEncuesta(Retiros model)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    Retiros Retiros = new Retiros();
                  
                    Retiros = model;

                    db.Entry(Retiros).State = EntityState.Modified;
                    db.SaveChanges();



                }
                catch
                { }
            }
        }

        public bool NotificarCorreoPyS(string asunto, string nombre, string Documento, string codigo, int id, string FechaTerminacion, string opcion)
        {

            string correos = "";
            var texto = "";
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);

            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

                using (var db = new AutogestionContext())
                {

                    var jefes = db.AreasPazySalvo.GroupBy(x => x.Responsable).ToList();

                    foreach (var grupo in jefes)
                    {
                        int Id = 0;
                        Int32.TryParse(grupo.Key, out Id);
                        var email = db.Empleados.FirstOrDefault(s => s.Id == Id);
                        if (email != null)
                        {
                            if (string.IsNullOrEmpty(correos)) { correos = email.CorreoPersonal; }
                            else { correos = correos + ", " + email.CorreoPersonal; }
                        }
                    }

                    correos = correos.Substring(0, correos.Length - 1);
                    

                    correo.To.Add(correos);

                    if (opcion == "Crear")
                    {

                        texto = db.Configuraciones.First(s => s.Parametro == "TXTMAILPAZYSALVO").Valor.ToString();

                        var Enc = id + "|email";
                        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Enc);
                        Enc = System.Convert.ToBase64String(plainTextBytes);
                        var link = db.Configuraciones.First(x => x.Parametro == "LINKFIRMAPYS").Valor.ToString();
                        link = link + "/Submit?str=" + Enc;
                        texto = texto.Replace("$LINK", link);

                    }
                    else {
                        texto = db.Configuraciones.First(s => s.Parametro == "TXTMAILANULARPYS").Valor.ToString();
                    }

                    texto = texto.Replace("$NOMBRE", nombre);
                    texto = texto.Replace("$DOCUMENTO", Documento);
                    texto = texto.Replace("$CODIGO", codigo.ToString());
                    texto = texto.Replace("$FECHAT", FechaTerminacion);

                    correo.Subject = asunto;
                    correo.Body = texto;
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
            }
            catch (Exception ex)
            {

                confirmacion = false;
                return confirmacion;
            }

        }







    }
}