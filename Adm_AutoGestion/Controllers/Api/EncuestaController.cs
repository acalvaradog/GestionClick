using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Adm_AutoGestion.Services;
using Adm_AutoGestion.Models;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;
using System.Web.Helpers;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Http.ModelBinding.Binders;
using System.Text;
using System.Data.Entity;
namespace Adm_AutoGestion.Controllers.Api
{

      [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EncuestaController : ApiController
    {

        private AutogestionContext db = new AutogestionContext();

        [HttpGet]
        [Route("api/obtenersospechoso/{IdEmpleado}")]
        public string guardarencuesta(int IdEmpleado)
        {
            var fecha = DateTime.Today.Date;
            try
            {
                var Encuesta = db.EncabezadoEncuesta.OrderByDescending(e => e.Fecha).FirstOrDefault(x => x.EmpleadoId == IdEmpleado && DbFunctions.TruncateTime(x.Fecha) == fecha);

                  if (Object.ReferenceEquals(null, Encuesta))
                  {
                      return "null";
                  }

                 else{
                  return Encuesta.Sospechoso;

                 }


               
            }
            catch {

                return "error";
            }

           
        }



        
        [HttpGet]
        [Route("api/GuardarCertificado/{mensaje}/{Color}/{Empleado}")]
        public string GuardarCertificado(int mensaje, string Color, int Empleado)
        {

            var respuesta = "True";
            var fecha = DateTime.Today.Date;
            try
            {

                EncabezadoEncuesta encuesta = db.EncabezadoEncuesta.OrderByDescending(e => e.Fecha).FirstOrDefault(x => x.EmpleadoId == Empleado && DbFunctions.TruncateTime(x.Fecha) == fecha);

               
                db.EncabezadoEncuesta.Attach(encuesta);
                encuesta.Sospechoso = Color + mensaje;
                db.SaveChanges();
                


                return respuesta; 

            }
            catch
            {

                respuesta = "False";
                return respuesta;
            }


        }

        // GET api/encuesta
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/encuesta/5
        public string Get(int id)
        {
            return "value";
        }



        // POST api/encuesta
          [HttpPost]
        public IHttpActionResult Post([FromBody]EncabezadoEncuesta EncabezadoEncuesta)
        {

            bool respuesta = true;
            

            try
            {

                Encuesta Enc = new Encuesta();
                db.EncabezadoEncuesta.Add(EncabezadoEncuesta);
                db.SaveChanges();

                foreach (Encuesta encuesta in EncabezadoEncuesta.Encuesta)
                {
                    Enc.NumeroPregunta = encuesta.NumeroPregunta;
                    Enc.Respuesta = encuesta.Respuesta;
                    Enc.EncabezadoEncuesta_Id = EncabezadoEncuesta.Id;

                    db.Encuesta.Add(Enc);
                    db.SaveChanges();
                }

              
                var empleado = db.Empleados.FirstOrDefault(x => x.Id == EncabezadoEncuesta.EmpleadoId);
                if (EncabezadoEncuesta.Sospechoso == "Morado" || EncabezadoEncuesta.Sospechoso == "Amarillo" || EncabezadoEncuesta.Sospechoso == "Azul")
                {

                     var correojefe = "";
                
                    try
                    {
                           var Jefe = db.Empleados.FirstOrDefault(x => x.NroEmpleado == empleado.Superior);

                   

                     if (Object.ReferenceEquals(null, Jefe))
                     {
                         correojefe = "";

                     }else{
                         correojefe = Jefe.Correo.ToString();
                     }
                     }


            catch (Exception ex)
            {
                correojefe = "";
            }


                    var correojefeinmediato = "";
                    try
                    {
                           var Jefe = db.Empleados.FirstOrDefault(x => x.NroEmpleado == empleado.Jefe);

                   

                     if (Object.ReferenceEquals(null, Jefe))
                     {
                         correojefeinmediato = "";

                     }else{
                         correojefeinmediato = Jefe.Correo.ToString();
                     }
                     }


            catch (Exception ex)
            {
                correojefe = "";
            }


                    // si tiene alerta azul notificar a epidemiologia
                    if (EncabezadoEncuesta.Sospechoso == "Azul")
                    {
                        enviar_correo_epidemiologia(empleado.NroEmpleado, empleado.Nombres, EncabezadoEncuesta.Empresa, EncabezadoEncuesta.Sospechoso.ToString());


                    }
                    else {
                        enviar_correo(empleado.NroEmpleado, empleado.Nombres, EncabezadoEncuesta.Empresa, correojefe, correojefeinmediato, EncabezadoEncuesta.Sospechoso.ToString());
                   
                    }
                }
              
                return Json(respuesta); 


            }
            catch(Exception ex)
            {

                respuesta = false;
                return Json(respuesta); 
            }

        
        }

        // PUT api/encuesta/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/encuesta/5
        public void Delete(int id)
        {
        }

        public bool enviar_correo(string codempleado, string nombres, string empresa, string correojefe, string correojefeinmediato,string color)
        {
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);

            string Para = Properties.Settings.Default.Correo1000Sospechoso.ToString();
            if (empresa == "2000")
            {
                Para = Properties.Settings.Default.Correo2000Sospechoso.ToString();
            }

            string[] CorreosEnviar = Para.Split(';');



            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

                foreach (var correoenviar in CorreosEnviar)
                {
                    correo.To.Add(correoenviar);
                }

                correo.Subject = "Reporte colaborador sospechoso";
                correo.Body = Properties.Settings.Default.TextoCorreoSospechoso.ToString() + "<br><br> Codigo Empleado: " + codempleado + "   Nombres: " + nombres + " <br> Alerta:" + color ;
                correo.Priority = System.Net.Mail.MailPriority.Normal;
                correo.IsBodyHtml = true;

                if (correojefe != ""){
                 correo.CC.Add(correojefe);
                }

                if (correojefeinmediato != "")
                {
                    correo.CC.Add(correojefeinmediato);
                }

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
                public bool enviar_correo_epidemiologia(string codempleado, string nombres, string empresa,string color)
        {
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);

            string Para = Properties.Settings.Default.Correo1000Epidemiologia.ToString();
            if (empresa == "2000")
            {
                Para = Properties.Settings.Default.Correo2000Epidemiologia.ToString();
            }

            string[] CorreosEnviar = Para.Split(';');



            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

                foreach (var correoenviar in CorreosEnviar)
                {
                    correo.To.Add(correoenviar);
                }

                correo.Subject = "Reporte colaborador sospechoso";
                correo.Body = Properties.Settings.Default.TextoCorreoEpidemiologia.ToString() + "<br><br> Codigo Empleado: " + codempleado + "   Nombres: " + nombres + " <br> Alerta:" + color ;
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
}
