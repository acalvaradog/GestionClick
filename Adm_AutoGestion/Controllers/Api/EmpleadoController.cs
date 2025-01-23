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
using System.Text;
using Newtonsoft.Json;
using System.Xml.Linq;
using Adm_AutoGestion.DTO;

namespace Adm_AutoGestion.Controllers.Api
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EmpleadoController : ApiController
    {



        private ServiciosRepository _servicios;
        private EmpleadoRepository _empleadoRepository;
        public EmpleadoController()
        {

            _servicios = new ServiciosRepository();
            _empleadoRepository = new EmpleadoRepository();
        }



        [HttpGet]
        [Route("api/validaringreso/{documento}/{pass}/{nroempleado}")]
        public IEnumerable<string> certificacion(string documento, string pass, string nroempleado)
        {

            using (var db = new AutogestionContext())
            {


                var empleado = db.Empleados.FirstOrDefault(x => x.Documento == documento && x.NroEmpleado == nroempleado);

                if (Object.ReferenceEquals(null, empleado))
                {

                    return new string[] { "3", "No se encontro" };
                }
                else
                {




                    if (empleado.Contraseña == "")
                    {
                        //asignar contaseña
                        empleado.Contraseña = pass;
                        db.SaveChanges(); if (empleado.Telefono == null)
                        {
                            empleado.Telefono = "";
                        }

                        if (empleado.Correo == null)
                        {
                            empleado.Correo = "";
                        }

                        if (empleado.CorreoPersonal == null)
                        {
                            empleado.CorreoPersonal = "";
                        }
                        if (empleado.DesplazamientosLaborales == null)
                        {
                            empleado.DesplazamientosLaborales = "";
                        }
                        if (empleado.Barrio == null)
                        {
                            empleado.Barrio = "";
                        }
                        if (empleado.Direccion == null)
                        {
                            empleado.Direccion = "";
                        }
                        if (empleado.MunicipioId == null || empleado.MunicipioId == 0)
                        {
                            empleado.MunicipioId = 0;
                        }
                        if (empleado.Estrato == null)
                        {
                            empleado.Estrato = "";
                        }
                        if (empleado.TipoViviendaId == null || empleado.TipoViviendaId == 0)
                        {
                            empleado.TipoViviendaId = 0;
                        }
                        return new string[] { "2", empleado.Id.ToString(), empleado.Telefono.ToString(), empleado.Correo.ToString(), empleado.CorreoPersonal.ToString(), empleado.DesplazamientosLaborales.ToString(), empleado.Barrio.ToString(), empleado.Direccion.ToString(), empleado.MunicipioId.ToString(), empleado.Estrato.ToString(), empleado.TipoViviendaId.ToString() };

                    }
                    else {
                        if (empleado.FechaFin >= DateTime.Now.Date || empleado.Activo != "NO")
                        {



                            //ya tiene contraseña, validar ingreso.
                            var empleadovalido = db.Empleados.FirstOrDefault(x => x.Documento == documento && x.Contraseña == pass && x.NroEmpleado == nroempleado);

                            if (empleadovalido == null)
                            {

                                return new string[] { "1", empleado.Id.ToString()};
                                //contraseña no valida
                            }
                            else
                            {

                                if (empleado.Telefono == null)
                                {
                                    empleado.Telefono = "";
                                }

                                if (empleado.Correo == null)
                                {
                                    empleado.Correo = "";
                                }

                                if (empleado.CorreoPersonal == null)
                                {
                                    empleado.CorreoPersonal = "";
                                }
                                if (empleado.DesplazamientosLaborales == null) 
                                {
                                    empleado.DesplazamientosLaborales = "";
                                }
                                if (empleado.Barrio == null)
                                {
                                    empleado.Barrio = "";
                                }
                                if (empleado.Direccion == null)
                                {
                                    empleado.Direccion = "";
                                }
                                if (empleado.MunicipioId == null || empleado.MunicipioId == 0)
                                {
                                    empleado.MunicipioId = 0;
                                }
                                if (empleado.Estrato == null)
                                {
                                    empleado.Estrato = "";
                                }
                                if (empleado.TipoViviendaId == null || empleado.TipoViviendaId == 0)
                                {
                                    empleado.TipoViviendaId = 0;
                                }
                                return new string[] { "0", empleado.Id.ToString(), empleado.Telefono.ToString(), empleado.Correo.ToString(), empleado.CorreoPersonal.ToString(), empleado.DesplazamientosLaborales.ToString(), empleado.Barrio.ToString(), empleado.Direccion.ToString(), empleado.MunicipioId.ToString(), empleado.Estrato.ToString(), empleado.TipoViviendaId.ToString() };
                                //datos validados correctamente

                            }
                        }
                        else {
                            return new string[] { "3", "No se encontro" };
                        }

                    }
                }

                //  return db.PreguntasFrecuentes.ToList();
                //return preguntas();

            }
            return new string[] { "value", "value2" };
        }





        [HttpGet]
        [Route("api/recordatoriopass/{documento}")]
        public IEnumerable<string> recordatoriopass(string documento)
        {

            using (var db = new AutogestionContext())
            {


                var empleado = db.Empleados.FirstOrDefault(x => x.Documento == documento);

                if (Object.ReferenceEquals(null, empleado))
                {

                    return new string[] { "3", "No se encontro" };
                }
                else if (empleado.Correo == null || empleado.Correo == "")
                {
                    return new string[] { "2", "Sin Correo" };

                } else {

                    if (enviar_correo(empleado.Nombres, empleado.Correo, empleado.Contraseña))
                    {
                        return new string[] { "1", empleado.Correo.ToString() };
                    }
                    else {
                        return new string[] { "0", "Error envio" };
                    }

                }


                //  return db.PreguntasFrecuentes.ToList();
                //return preguntas();

            }
            return new string[] { "value", "value2" };
        }


        [HttpGet]
        [Route("api/cambiopass/{documento}/{actual}/{nueva}")]
        public IEnumerable<string> cambiopass(string documento, string actual, string nueva)
        {

            using (var db = new AutogestionContext())
            {


                var empleado = db.Empleados.FirstOrDefault(x => x.Documento == documento);

                if (Object.ReferenceEquals(null, empleado))
                {

                    return new string[] { "3", "No se encontro" };
                }
                else if (empleado.Contraseña != actual)
                {
                    return new string[] { "2", "No coinciden" };

                }
                else
                {
                    try
                    {
                        empleado.Contraseña = nueva;
                        db.SaveChanges();
                        return new string[] { "1", "Cambios realizados" };
                    }
                    catch {
                        return new string[] { "0", "Error al actualizar" };

                    }



                }


                //  return db.PreguntasFrecuentes.ToList();
                //return preguntas();

            }
            return new string[] { "value", "value2" };
        }


        [HttpGet]
        [Route("api/cumpleanos")]
        public IHttpActionResult cumpleanos()
        {

            using (var db = new AutogestionContext())
            {

                return Json(db.Empleados.Where(x => x.FechaNacimiento.Value.Day == DateTime.Now.Day && x.FechaNacimiento.Value.Month == DateTime.Now.Month && x.Activo != "NO").ToList());

            }

        }


        [HttpPost]
        [Route("api/enviarcorreocumpleanos")]
        public IHttpActionResult enviarcorreocumpleanos(Empleado empleado)
        {

            using (var db = new AutogestionContext())
            {

                Configuraciones configuracion = db.Configuraciones.FirstOrDefault(x => x.Parametro == "CORREOCUMPLEANO");

                configuracion.Valor =  configuracion.Valor.Replace("{empleado}",empleado.Nombres.ToString());
               var enviocorreo =  _servicios.EnviarEmailGenerico(empleado.Correo, configuracion.Valor, "Feliz Cumpleaños");

                return Json(enviocorreo);

            }

        }

        [HttpPost]
        [Route("api/enviarcorreoaniversario")]
        public IHttpActionResult enviarcorreoaniversario(Empleado empleado)
        {

            using (var db = new AutogestionContext())
            {

                Configuraciones configuracion = db.Configuraciones.FirstOrDefault(x => x.Parametro == "CORREOANIVERSARIO");

                configuracion.Valor = configuracion.Valor.Replace("{empleado}", empleado.Nombres.ToString());
                var enviocorreo = _servicios.EnviarEmailGenerico(empleado.Correo, configuracion.Valor, "Feliz Cumpleaños");

                return Json(enviocorreo);

            }

        }

        [HttpGet]
        [Route("api/aniversario")]
        public IHttpActionResult aniversario()
        {

            using (var db = new AutogestionContext())
            {

                return Json(db.Empleados.Where(x => x.FechaIngreso.Value.Day == DateTime.Now.Day && x.FechaIngreso.Value.Month == DateTime.Now.Month && x.Activo != "NO").ToList());

            }

        }

        // GET api/empleado
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/empleado/5
        public string Get(int Id)
        {
            return "value";
        }

        // POST api/empleado
        [HttpPost]
        public IHttpActionResult Post([FromBody] Empleado Empleado)
        {
            var idempleado = Empleado.Id;
            var telefono = Empleado.Telefono;
            var correo = Empleado.Correo;
            var correoPersonal = Empleado.CorreoPersonal;
            var tipovivienda = Empleado.TipoViviendaId;
            var barrio = Empleado.Barrio; 
            var municipio = Empleado.MunicipioId;
            var direccion = Empleado.Direccion;
            var estrato = Empleado.Estrato;
            var encuesta = Empleado.DesplazamientosLaborales;
            string respuesta = "";

            using (var db = new AutogestionContext())
            {


                var empleado = db.Empleados.FirstOrDefault(x => x.Id == idempleado);

                if (Object.ReferenceEquals(null, empleado))
                {
                    respuesta = "3";
                    //return new string[] { "3", "Ocurrio un errro al actualizar" };
                }
                else
                {
                    empleado.Telefono = telefono;
                    empleado.Correo = correo;
                    empleado.CorreoPersonal = correoPersonal;
                    empleado.TipoViviendaId = tipovivienda;
                    empleado.Barrio = barrio;
                    empleado.MunicipioId = municipio;
                    empleado.Direccion = direccion;
                    empleado.Estrato = estrato;
                    empleado.DesplazamientosLaborales = encuesta;

                    db.SaveChanges();
                    //return new string[] { "1", "Se han actualizado los datos" };
                    respuesta = "1";

                }

                //  return db.PreguntasFrecuentes.ToList();
                //return preguntas();

            }

            return Json(respuesta);

        }




        // PUT api/empleado/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/empleado/5
        public void Delete(int id)
        {
        }



        [HttpPost]
        [Route("api/qrempleado/{id}")]
        public String qrempleado(string id)
        {
            try
            {
                var idempleado = Convert.ToInt32(id);
                using (var db = new AutogestionContext())
                {

                    var model_empleado = db.Empleados.FirstOrDefault(x => x.Id == idempleado);

                    if (model_empleado != null)
                    {

                        var personal = db.Empleados.FirstOrDefault(x => x.Documento == model_empleado.Documento);
                        string cargo = "";
                        if (personal != null)
                        {
                            cargo = personal.Cargo;
                        }

                        string textoqr = model_empleado.NroEmpleado + "|" + model_empleado.Nombres + "|" + model_empleado.Documento + "|" + model_empleado.Empresa + "|" + cargo;
                        byte[] ImagenQR = _servicios.GenerarQREmpleado(textoqr);
                        return Convert.ToBase64String(ImagenQR);
                    }
                    else
                    {
                        return "noexiste";

                    }

                }


            }
            catch (Exception e)
            {

                return "error";
            }


        }

        [HttpPost]
        [Route("api/obtenerfotoempleado/{id}")]
        public String obtenerfotoempleado(string id)
        {
            try
            {
                Byte[] b = null;
                try
                {
                    b = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/AnexosFotos/" + id + ".png"));
                }
                catch {
                    try
                    {
                        b = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/AnexosFotos/" + id + ".jpg"));
                    }
                    catch
                    {
                        try
                        {
                            b = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/AnexosFotos/" + id + ".jpeg"));
                        }
                        catch
                        {


                        }

                    }

                }


                return Convert.ToBase64String(b);

            }
            catch (Exception e)
            {

                return "error";
            }


        }

        [HttpPost]
        [Route("api/cargarfotoempleado")]
        public IHttpActionResult Post()
        {

            var EmpleadoId = System.Web.HttpContext.Current.Request.Params["CodigoEmpleado"];
            var httpPostedFile = System.Web.HttpContext.Current.Request.Files["Adjunto"];
            try
            {

                //Validate the uploaded image(optional)

                var extension = httpPostedFile.FileName.Split('.');
                // Get the complete file path
                var fileSavePath = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/AnexosFotos"), EmpleadoId + "." + extension[1]);

                //// Save the uploaded file to "UploadedFiles" folder
                httpPostedFile.SaveAs(fileSavePath);

                return Json("ok");
            }
            catch (Exception e)
            {

                return Json("error");
            }


        }


        [HttpPost]
        [Route("api/Trabajador/ActualizacionEmpleados")]
        public IHttpActionResult ActualizacionEmpleados() {

            try
            {
                _empleadoRepository.ProcesoActualizacionyRetirados();
                return Json(true);
            }
            catch (Exception)
            {

                return Json(false);
            }
         
        }

        [HttpPost]
        [Route("api/Trabajador/ActualizacionEstructura")]
        public IHttpActionResult ActualizacionEstructura()
        {

            try
            {
                _empleadoRepository.ActualizarEstructuraJerarquicaEmpleado();
                return Json(true);
            }
            catch (Exception)
            {

                return Json(false);
            }

        }

        [HttpGet]
        [Route("api/Trabajador/ActualizarEstructura_UnidadOrg")]
        public string ActualizarEstructura_UnidadOrg()
        {

            try
            {
               string respuesta = _empleadoRepository.ActualizarEstructuraJerarquica_unidadOr();
                return "Proceso Exitoso" + respuesta;
            }
            catch (Exception ex)
            {

                return "Error: "+ ex;
            }

        }

        [HttpPost]
        [Route("api/Trabajador/ActualizacionEmpleadosIndividual")]
        public IHttpActionResult ActualizacionEmpleadosIndividual(Empleado empleado)
        {

            try
            {
                _empleadoRepository.ProcesoActualizacionyRetiradosIndividual(empleado);
                return Json(true);
            }
            catch (Exception)
            {

                return Json(false);
            }

        }

        [HttpGet]
        [Route("api/Trabajador/GetAll")]
        public IHttpActionResult GetAll() {


            return Json(_empleadoRepository.ObtenerTodos());
        }


        [HttpGet]
        [Route("api/Trabajador/GetxCodigo/{codigo}")]
        public IHttpActionResult GetxCodigo(string codigo)
        {


            return Json(_empleadoRepository.ObtenerxCodigo(codigo));
        }


        [HttpPost]
        [Route("api/Trabajador/GenerarCertificado")]
        public IHttpActionResult GenerarCertificado(SolicitudCertificadoIngresosDTO solicitud)
        {


            return Json(_empleadoRepository.GenerarCertificadoIngresos(solicitud.url, solicitud.documento, solicitud.correo));
        }


        public bool enviar_correo(string nombres, string correoenviar,string pass)
        {
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);

   


            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

                    correo.To.Add(correoenviar);
            

                correo.Subject = "Recordatorio Contraseña";
                correo.Body = "Hola " + nombres + ",<br>" + "Su contraseña de ingreso a Autogestión es:" + pass + "<br><br> Cordialmente, <br> Divisiòn TIC Foscal";
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

        [HttpGet]
        [Route("api/Trabajador/ObtenerTipoVivienda")]
        public IHttpActionResult ObtenerTipoVivienda()
        {
            List<TipoVivienda> Datos = new List<TipoVivienda>();
            try 
            {
                using (var db = new AutogestionContext())
                {
                    Datos = db.TipoVivienda.Where(x => x.Estado == "Activo").OrderBy(x => x.Nombre).ToList();
                }
            }
            catch(Exception ex) 
            {
            
            
            }                    

                return Json(Datos);
        }

        [HttpGet]
        [Route("api/Trabajador/ObtenerMunicipios/{Id}")]
        public IHttpActionResult ObtenerMunicipios(int? Id)
        {
            List<Municipio> Datos = new List<Municipio>();
            try
            {
                if (Id != 0)
                {
                    using (var db = new AutogestionContext())
                    {
                        Datos = db.Municipio.Where(x => x.Estado == "Activo" && x.DepartamentoId == Id).OrderBy(x => x.Nombre).ToList();
                    }

                }
                else
                {
                    using (var db = new AutogestionContext())
                    {
                        Datos = db.Municipio.Where(x => x.Estado == "Activo").OrderBy(x => x.Nombre).ToList();
                    }

                }
               
            }
            catch (Exception ex)
            {


            }

            return Json(Datos);
        }

        [HttpGet]
        [Route("api/Trabajador/Departamentos")]
        public IHttpActionResult Departamentos() 
        {
            List<Departamento> Datos = new List<Departamento>();
            try
            {
                using (var db = new AutogestionContext())
                {
                    Datos = db.Departamento.Where(x => x.Estado == "Activo").OrderBy(x => x.Nombre).ToList();
                }
            }
            catch (Exception ex)
            {


            }

            return Json(Datos);

        }
    }
}
