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



namespace Adm_AutoGestion.Controllers.Api
    
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class IncapacidadesController : ApiController
    {


        private AutogestionContext db = new AutogestionContext();

        // GET api/incapacidades
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/incapacidades/5
        public IHttpActionResult Get(int id)
        {
            using (var db = new AutogestionContext())
            {
                var model = db.Incapacidades.Where(x => x.EmpleadoId == id).ToList();

                foreach (var item in model){
                    var estadoNombre = db.EstadosIncapacidades
                         .Where(e => e.Id == item.EstadoId)
                         .Select(e => e.Nombre)
                         .FirstOrDefault();
                    item.Estado= estadoNombre;
                }
                var jsonDeserialize = new JavaScriptSerializer();
                var json = jsonDeserialize.Serialize(model);
                return Json(model);


            
            }

        }


        //AnaA 28-08-2020 /api/Incapacidades/Diagnostico

        [HttpGet]
        [Route("api/Incapacidades/Diagnostico/{Id}")]
        public IEnumerable <string> Consulta(string Id)
        {

          
            using (var db = new AutogestionContext())
            {
                var  datos = db.Diagnostico.FirstOrDefault(e => e.Codigo == Id);

                if (datos != null)
                {

                    return new string[] { "0", datos.Nombre.ToString() };

                }
                else {
                    return new string[] { "1", "Diagnostico  no existe" };
                }

            }

            return new string[] { "value", "value2" };
        }


        //AnaA 31-08-2020 /api/Incapacidades/Diagnostico

        [HttpGet]
        [Route("api/Incapacidades/TipoIncapacidad")]
        public IHttpActionResult Consulta()
        {


            using (var db = new AutogestionContext())
            {
                var datos = db.TiposIncapacidad.Where(e => e.EstadoId == 4).ToList();

                 return Json(datos); 
           

            }
              
        }

        [HttpGet]
        [Route("api/Incapacidades/EPS")]
        public IHttpActionResult Eps()
        {


            using (var db = new AutogestionContext())
            {
                var datos = db.EPS.Where(e => e.EstadoId == 4).ToList();

                return Json(datos);


            }

        }


        //AnaA 31-08-2020  Tipo Adjunto

        [HttpGet]
        [Route("api/Incapacidades/TipoAdjunto/{tipoIncapacidadId}/{epsId}")]
        public IHttpActionResult Consulta(int tipoIncapacidadId, int epsId)
        {

            using (var db = new AutogestionContext())
            {
                var datos = db.Adjunto.Where(e => e.IdTiposIncapacidad == tipoIncapacidadId ).ToList();

                //if (epsId == 1)
                //{
                //    // Filtra o modifica los datos según las condiciones específicas para Suramericana
                //    datos = datos.Where(context => context.Id != 30 && context.Id != 31 && context.Id != 32 && context.Id != 33 && context.Id != 34 && context.Id != 35).ToList();
                //}
                //else
                //{
                //    // Filtra o modifica los datos según las condiciones específicas para otros valores de EPSId
                //    datos = datos.Where(context => context.Id != 1 && context.Id != 2 && context.Id != 3 && context.Id != 13 && context.Id != 5
                //        && context.Id != 7 && context.Id != 10 && context.Id != 11 && context.Id != 25 && context.Id != 26 && context.Id != 27 && context.Id != 28).ToList();
                //}

                return Json(datos);


            }

        }


               

        // POST api/incapacidades
        [HttpPost]
        [Route("api/incapacidadess")]
        public IHttpActionResult Post()
        {

            //bool respuesta = true;
            string respuesta = "true";

            try
            {
            var Datos2 = new Incapacidades();
            var filesToDelete = HttpContext.Current.Request.Params["probar"];
            var EmpleadoId = HttpContext.Current.Request.Params["Incapacidades.EmpleadoId"];
            var FechaInicial = HttpContext.Current.Request.Params["Incapacidades.FechaInicio"];
            var FechaFinal = HttpContext.Current.Request.Params["Incapacidades.FechaFin"];
            var CantDiasSolicitados = HttpContext.Current.Request.Params["Incapacidades.CantidadDias"];
            var Diagnostico = HttpContext.Current.Request.Params["Incapacidades.Diagnostico"];
            var CantidadAdjuntos = HttpContext.Current.Request.Params["Incapacidades.CantidadAdjuntos"];
            var EPS = HttpContext.Current.Request.Params["Incapacidades.EPS"];
            var Prorroga = HttpContext.Current.Request.Params["Incapacidades.Prorroga"];
            var TipoIncapacidad = HttpContext.Current.Request.Params["IncapacidadAdjuntos.TipoIncapacidad"];

            int Idempl = 0;
            Int32.TryParse(EmpleadoId, out Idempl);
            var empl = db.Empleados.Find(Idempl);

            Incapacidades DatosIncapacidades = new Incapacidades();
            DatosIncapacidades.EmpleadoId = Convert.ToInt16(EmpleadoId);
            DatosIncapacidades.FechaInicio = DateTime.Parse(FechaInicial);
            DatosIncapacidades.FechaFin = DateTime.Parse(FechaFinal); ;
            DatosIncapacidades.CantidadDias = CantDiasSolicitados;
            DatosIncapacidades.Diagnostico = Diagnostico;
            DatosIncapacidades.EPS = EPS;
            DatosIncapacidades.Prorroga = Prorroga;
            DatosIncapacidades.Fecha = DateTime.Now;
            DatosIncapacidades.EstadoId = 1;
            DatosIncapacidades.Empresa = empl.Empresa;
       

         int cantidad = Convert.ToInt16(CantidadAdjuntos);

         List<IncapacidadAdjuntos> Adjuntos = new List<IncapacidadAdjuntos>();
            

           

         for (int i = 0; i < cantidad; i++)
         {

             IncapacidadAdjuntos Adjunto = new IncapacidadAdjuntos();
             string nombreadjunto = "Adjunto" + i;
             var httpPostedFile = HttpContext.Current.Request.Files[nombreadjunto];
             var TipoAdjunto = HttpContext.Current.Request.Params["TipoAdjunto"+i];

             var lastDotIndex = httpPostedFile.FileName.LastIndexOf('.');
             
             if (lastDotIndex != -1 && lastDotIndex < httpPostedFile.FileName.Length - 1)
             {
                 var extension = httpPostedFile.FileName.Substring(lastDotIndex + 1).ToLower();
             
                 if (extension != "jpg" && extension != "jpeg" && extension != "png" && extension != "doc" && extension != "docx" && extension != "pdf")
                 {
                     respuesta = "El tipo de archivo " + extension + " no es permitido.";
                     return Json(respuesta);
                 }
             }
             else
             {
                 // Manejo del caso en que el archivo no tiene extensión
                 respuesta = "El archivo no tiene una extensión válida.";
                 return Json(respuesta);
             }
             
             var size = httpPostedFile.ContentLength / (1024 * 1024); //MB
             var pesoMaximoStr = db.Configuraciones.First(s => s.Parametro == "LIMITEPESOARCHIVO").Valor.ToString();
             int pesoMaximo = 0;
             Int32.TryParse(pesoMaximoStr, out pesoMaximo);

            if (size > pesoMaximo)
             {
                 respuesta = "El archivo supera el tamaño permitido de carga.";
                 return Json(respuesta);

             }


             // Validate the uploaded image(optional)
             DateTime date1 = DateTime.Now;
             var nombrearchivo = date1.ToString("yyyyMMddHHmmssffff" + i).ToString() + "-" + httpPostedFile.FileName;
             // Get the complete file path
             var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/AnexosIncapacidades"), nombrearchivo);
           
             //// Save the uploaded file to "UploadedFiles" folder
             httpPostedFile.SaveAs(fileSavePath);
             Adjunto.IncapacidadId = DatosIncapacidades.Id;
             Adjunto.Adjunto = nombrearchivo;
             Adjunto.TipoIncapacidad = TipoIncapacidad;
             Adjunto.TipoAdjunto = TipoAdjunto;
             Adjuntos.Add(Adjunto);
             db.IncapacidadAdjuntos.Add(Adjunto);
          

         }


          DatosIncapacidades.ListadoAdjuntos = Adjuntos;


            var deserializer = new JavaScriptSerializer();
            //var results = deserializer.Deserialize<Vacaciones>(encuesta);

            

               db.Incapacidades.Add(DatosIncapacidades);

               db.SaveChanges();
                int empleadodatos = 0;
                Int32.TryParse(EmpleadoId, out empleadodatos);
                

                Empleado empleado = db.Empleados.FirstOrDefault(e => e.Id == empleadodatos);
                Empleado jefe = db.Empleados.FirstOrDefault(e => e.NroEmpleado == empleado.Jefe);
                Empleado lider = db.Empleados.FirstOrDefault(e => e.NroEmpleado == empleado.Lider);

                var correoIncapacidades = db.Configuraciones.First(s => s.Parametro == "CORREOINCAPACIDADES").Valor.ToString();


                if (lider  == null) {
                
                    lider = db.Empleados.FirstOrDefault(e => e.NroEmpleado == empleado.Jefe);
                }

                //**************************************
                if (notificar_incapacidad(empleado.Documento, empleado.Nombres, lider.Correo, jefe.Correo, correoIncapacidades, empleado.Empresa, FechaInicial, FechaFinal) == false)
                {
                    respuesta = "Error al momento de enviar correo de notificación.";

                    return Json(respuesta);
                }
                

                return Json(respuesta);
            }
            catch (SystemException ex)
            {
                respuesta = String.Format("Error al guardar. {0}", ex.Message);
                return Json(respuesta);
            }


        }




        // PUT api/incapacidades/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/incapacidades/5
        public void Delete(int id)
        {
        }


        public bool notificar_incapacidad(string cedula, string nombres, string email_lider, string email_jefe, string email_nomina, string empresa, string FechaInicio, string FechaFin) { 
        
        string textocorreo = "";
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);

            string emails_envio = email_jefe + ", " + email_nomina;

            using (var db = new AutogestionContext())
            {

                var texto = db.Configuraciones.First(s => s.Parametro == "TXTREGISTROINCAP").Valor.ToString();


                texto = texto.Replace("$NOMBRE", nombres);
                texto = texto.Replace("$DOCUMENTO", cedula);
                texto = texto.Replace("$FechaInicio$", FechaInicio);
                texto = texto.Replace("$FechaFin$", FechaFin);

                textocorreo = texto;
            }


            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

                correo.To.Add(emails_envio);
                correo.CC.Add(email_lider);

                correo.Subject = "Notificación Registro de Incapacidad";
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


        [HttpGet]
        [Route("api/Incapacidades/ConsultarFechaIngreso/{id}")]
        public IHttpActionResult ConsultarFechaIngreso(int id)
        {
            try
            {
                using (var db = new AutogestionContext())
                {
                    var empleado = db.Empleados.FirstOrDefault(x => x.Id == id);

                    if (empleado != null && empleado.FechaIngreso.HasValue)
                    {
                        // Serializa el objeto a JSON y lo devuelve
                        return Ok(empleado.FechaIngreso.Value);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }





    }
}
