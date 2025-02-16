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
    public class PermisosController : ApiController
    {
        private AutogestionContext db = new AutogestionContext();
        // GET api/permisos
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/permisos/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/permisos
        [HttpPost]
        public IHttpActionResult Post()
        {
            string respuesta = "true";
            var Datos2 = new Permiso();
            var filesToDelete = HttpContext.Current.Request.Params["probar"];
            var EmpleadoId = HttpContext.Current.Request.Params["Permiso.EmpleadoId"];
            var MotivoId = HttpContext.Current.Request.Params["Permiso.MotivoPermiso"];
            var FechaPermiso = HttpContext.Current.Request.Params["Permiso.FechaPermiso"];
            var FechaFinPermiso = HttpContext.Current.Request.Params["Permiso.FechaFinPermiso"];
            var HoraInicioPermiso = HttpContext.Current.Request.Params["Permiso.HoraInicioPermiso"];
            var HoraFinPermiso = HttpContext.Current.Request.Params["Permiso.HoraFinPermiso"];
            var Jornada = HttpContext.Current.Request.Params["Permiso.Jornada"];
            var Parentesco = HttpContext.Current.Request.Params["Permiso.Parentesco"];
            var Observacion = HttpContext.Current.Request.Params["Permiso.Observacion"];
            var CantidadAdjuntos = HttpContext.Current.Request.Params["Permiso.CantidadAdjuntos"];
            var ArcVacio = HttpContext.Current.Request.Params["Permiso.ArchivoVacio"];
            

            int IdEmple = 0;
            Int32.TryParse(EmpleadoId, out IdEmple);
            var empl = db.Empleados.Find(IdEmple);
          
            int motivosper = 0;
            Int32.TryParse(MotivoId, out motivosper);

            Permiso Datospermisos = new Permiso();
            Datospermisos.EmpleadoId = Convert.ToInt16(EmpleadoId);
            Datospermisos.Fecha = DateTime.Now.Date;
            Datospermisos.FechaPermiso = DateTime.Parse(FechaPermiso);
            Datospermisos.FechaFinPermiso = DateTime.Parse(FechaFinPermiso);
            Datospermisos.HoraInicioPermiso = HoraInicioPermiso;
            Datospermisos.HoraFinPermiso = HoraFinPermiso;
            Datospermisos.Remunerado = "";
            Datospermisos.MotivoId = motivosper;
            Datospermisos.Jornada = Jornada;
            Datospermisos.RevisadoNomina = "NO";
            Datospermisos.Observacion = Observacion;
            Datospermisos.EstadoId = 1;
            Datospermisos.Empresa = empl.Empresa;

            


            int cantidad = Convert.ToInt16(CantidadAdjuntos);
            if (ArcVacio == "SI")
            {
                cantidad = 0;
            }

            List<PermisosAdjuntos> Adjuntos = new List<PermisosAdjuntos>();
            var Motivo = db.MotivosPermiso.Where(x => x.Id == Datospermisos.MotivoId).FirstOrDefault();
            if (Motivo.FechaInicialDisfrute !=null && Motivo.FechaFinalDisfrute != null) 
            {
                if (Motivo.FechaInicialDisfrute < DateTime.Parse(FechaPermiso) || Motivo.FechaFinalDisfrute > DateTime.Parse(FechaFinPermiso))
                {
                    respuesta = "Las fechas de disfrute no pueden superar la fecha " + Convert.ToDateTime(Motivo.FechaFinalDisfrute).ToString("dd/MM/yyyy");
                   
                    return Json(respuesta);
                }

            }

            for (int i = 0; i < cantidad; i++)
            {

                PermisosAdjuntos Adjunto = new PermisosAdjuntos();
                string nombreadjunto = "Adjunto" + i;
                var httpPostedFile = HttpContext.Current.Request.Files[nombreadjunto];


                var extension = httpPostedFile.FileName.Split('.');

                if (extension[1] != "jpg" && extension[1] != "jpeg" && extension[1] != "png" && extension[1] != "doc" && extension[1] != "docx" && extension[1] != "pdf")
                {
                    respuesta = "El tipo de archivo " + extension[1] + " no es permitido.";
                    return Json(respuesta);
                }

                var size = httpPostedFile.ContentLength / 1024;
                if (size > 2000)
                {
                    respuesta = "El archivo supera el tamaño permitido de carga.";
                    return Json(respuesta);

                }


                // Validate the uploaded image(optional)
                DateTime date1 = DateTime.Now;
                var nombrearchivo = "";
                if (MotivoId == "5")
                {
                    nombrearchivo = date1.ToString("yyyyMMddHHmmssffff" + i).ToString() + "-" + Parentesco + "-" + httpPostedFile.FileName;
                }
                else
                {
                    nombrearchivo = date1.ToString("yyyyMMddHHmmssffff" + i).ToString() + "-" + httpPostedFile.FileName;
                }
                // Get the complete file path
                var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/AnexosPermisos"), nombrearchivo);

                //// Save the uploaded file to "UploadedFiles" folder
                httpPostedFile.SaveAs(fileSavePath);
                Adjunto.PermisosId = Datospermisos.Id;
                Adjunto.Adjunto = nombrearchivo;
                Adjunto.MotivoPermiso = MotivoId;
                Adjuntos.Add(Adjunto);
                db.PermisosAdjuntos.Add(Adjunto);


            }


            Datospermisos.ListadoAdjuntos = Adjuntos;


            var deserializer = new JavaScriptSerializer();
           

            try
            {
               db.Permisos.Add(Datospermisos);
               db.SaveChanges();

                
                HistorialPermisos Historial = new HistorialPermisos();
                Historial.PermisoId = Datospermisos.Id;
                Historial.Fecha_Permiso = Datospermisos.Fecha;
                Historial.Estado = string.Format("{0}", Datospermisos.EstadoId);
                Historial.EmpleadoId = Datospermisos.EmpleadoId;
                Historial.Usuario_Modifica = Datospermisos.EmpleadoId;
                Historial.Observaciones_Permiso = Datospermisos.Observacion;
                db.HistorialPermisos.Add(Historial);
                db.SaveChanges();



                int empleadodatos = 0;
                Int32.TryParse(EmpleadoId, out empleadodatos);


                Empleado empleado = db.Empleados.FirstOrDefault(e => e.Id == empleadodatos);
                Empleado jefe = db.Empleados.FirstOrDefault(e => e.NroEmpleado == empleado.Jefe);
                Empleado lider = db.Empleados.FirstOrDefault(e => e.NroEmpleado == empleado.Lider);
                var correolider = "";

                if (jefe != null)
                {
                    if (lider == null)
                    {
                        correolider = "vacio";
                    }
                    else { correolider = lider.Correo;  }

                    notificar_Permiso(empleado.Documento, empleado.Nombres, jefe.Correo, correolider);


                    //if (notificar_Permiso(empleado.Documento, empleado.Nombres, jefe.Correo) == false)
                    //{
                    //    respuesta = "Los datos fueron guardados correctamente, pero no se pudo enviar correo de notificación al jefe.";
                    //    return Json(respuesta);
                    //}
                    
                    
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
        [Route("api/Permisos/Consulta/{Id}")]
        public string Consulta(string Id)
        {
            var Datos2 = new Permiso();
            using (var db = new AutogestionContext())
            {
                int id = 0;
                Int32.TryParse(Id, out id);

                List<Permiso> Datos = new List<Permiso>();

                Datos = db.Permisos.Where(e => e.EmpleadoId == id).OrderByDescending(e => e.Id).Take(20).ToList();

                foreach (var valor in Datos)
                {
                    var estadopermiso = db.EstadosPermiso.FirstOrDefault(x => x.Id == valor.EstadoId);
                    valor.EstadoPermiso.Nombre = estadopermiso.Nombre;
                    var motivopermiso = db.MotivosPermiso.FirstOrDefault(x => x.Id == valor.MotivoId);
                    valor.MotivoPermiso = motivopermiso;
                }
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(Datos);
                return json;
            }
            return "respuesta";
        }


        [HttpGet]
        [Route("api/Permisos/Permisosxusuario/{Id}")]
        public IHttpActionResult Permisosxusuario(string Id)
        {
            var Datos2 = new Permiso();
            using (var db = new AutogestionContext())
            {
                int id = 0;
                Int32.TryParse(Id, out id);

                List<Permiso> Datos = new List<Permiso>();

                Datos = db.Permisos.Where(e => e.EmpleadoId == id).OrderByDescending(e => e.Id).Take(20).ToList();

                foreach (var valor in Datos)
                {
                    var estadopermiso = db.EstadosPermiso.FirstOrDefault(x => x.Id == valor.EstadoId);
                    valor.EstadoPermiso.Nombre = estadopermiso.Nombre;
                    var motivopermiso = db.MotivosPermiso.FirstOrDefault(x => x.Id == valor.MotivoId);
                    valor.MotivoPermiso = motivopermiso;
                }
                return Json(Datos);
            }
            return null;
        }

        public bool notificar_Permiso(string cedula, string nombres, string email, string emailLider)
        {

            string textocorreo = "";
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);

            using (var db = new AutogestionContext())
            {

                var texto = db.Configuraciones.First(s => s.Parametro == "TXTREGISTROPERM").Valor.ToString();
                var link = db.Configuraciones.First(s => s.Parametro == "LINKLOGINPERM").Valor.ToString();


                texto = texto.Replace("$NOMBRE", nombres);
                texto = texto.Replace("$DOCUMENTO", cedula);
                texto = texto.Replace("$LINK", link);

                textocorreo = texto;
            }

            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);


                correo.To.Add(email);

                if (emailLider != "vacio") {
                    correo.CC.Add(emailLider);
                }

                
                correo.Subject = "Notificación Registro de Permiso";
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
        [Route("api/Permisos/TipoAdjunto/{id}/{parentesco}")]
        public string Consulta(int Id, string parentesco)
        {

            using (var db = new AutogestionContext())
            {
                var datos = db.AdjuntoPermisos.Where(e => e.IdMotivoPermiso == Id).ToList();

                if (Id == 5)
                {
                    datos = db.AdjuntoPermisos.Where(e => e.IdMotivoPermiso == Id && e.TipoLicenciaLuto == parentesco || e.TipoLicenciaLuto == "1").ToList();
                }
               
                
                var jsonDeserialize = new JavaScriptSerializer();
                var json = jsonDeserialize.Serialize(datos);
                return json;


            }

        }


        [HttpGet]
        [Route("api/Permisos/ValidarBono/{empleado}")]
        public string ConsultarBono(int empleado)
        {
            var message = "";
            using (var db = new AutogestionContext())
            {
                DateTime añoactual = DateTime.Now;
                var datos = db.Permisos.Where(e => e.EmpleadoId == empleado && e.FechaPermiso.Year == añoactual.Year && e.MotivoId == 10 && e.EstadoId == 4).ToList();
                var datos2 = db.Permisos.Where(e => e.EmpleadoId == empleado && e.FechaPermiso.Year == añoactual.Year && e.MotivoId == 10 && (e.EstadoId == 1 || e.EstadoId == 2 || e.EstadoId == 6)).ToList();

                var jsonDeserialize = new JavaScriptSerializer();
                if (datos.Count != 0)
                {
                    message = "True";
                }else if (datos2.Count != 0)
                {
                    message = "True2";
                }
                else { message = "False"; }

                var json = jsonDeserialize.Serialize(message);
                return json;


            }

        }

        

        [HttpGet]
        [Route("api/Permisos/ValidarSalarioEmocional/{empleado}")]
        public string ConsultarSalarioEmocional(int empleado)
        {
            var message = "";
            using (var db = new AutogestionContext())
            {
                DateTime añoactual = DateTime.Now;
                var datos = db.Permisos.Where(e => e.EmpleadoId == empleado && e.FechaPermiso.Year == añoactual.Year && e.MotivoId == 9 && e.EstadoId == 4).ToList();

                var datos2 = db.Permisos.Where(e => e.EmpleadoId == empleado && e.FechaPermiso.Year == añoactual.Year && e.MotivoId == 9 && (e.EstadoId == 1 || e.EstadoId == 2 ||e.EstadoId == 6 )).ToList();

                var jsonDeserialize = new JavaScriptSerializer();
                if (datos.Count != 0)
                {
                    message = "True1";
                }
                else if (datos2.Count != 0)
                {
                    message = "True2";
                }
                else { message = "False";  }
                var json = jsonDeserialize.Serialize(message);
                return json;


            }

        }



        [HttpGet]
        [Route("api/Permisos/anularpermiso/{Id}")]
        public string AnularPermiso(int id)
        { 
        var message = "";
            using (var db = new AutogestionContext())
            {
                var jsonDeserialize = new JavaScriptSerializer();
                DateTime Fecha2 = DateTime.Now;

            try
                {

                Permiso Permisos = new Permiso();
                Permisos = db.Permisos.FirstOrDefault(e => e.Id == id);
                db.Permisos.Attach(Permisos);
                Permisos.EstadoId = 10;
                db.SaveChanges();


                HistorialPermisos Historial = new HistorialPermisos();

                Historial.PermisoId = Convert.ToInt32(id);
                Historial.Estado = "10";                      
                Historial.Fecha_Permiso = Fecha2;
                Historial.EmpleadoId = Permisos.EmpleadoId;
                Historial.Usuario_Modifica = Permisos.EmpleadoId;
                Historial.Observaciones_Permiso = "Permiso anulado por el Trabajador.";
                db.HistorialPermisos.Add(Historial);
                db.SaveChanges();
                message = "True";
               

                }
            catch
            {
                message = "False";
            }

            var json = jsonDeserialize.Serialize(message);
            return json;

            }
        
        }







    }
}