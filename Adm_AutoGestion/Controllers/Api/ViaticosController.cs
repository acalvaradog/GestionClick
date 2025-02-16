using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.Runtime.InteropServices;

namespace Adm_AutoGestion.Controllers.Api
{
    //habilitar acceso desde todos los origines, pero aca debemos poner las iP'S que pueden acceder. solo las internas
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ViaticosController : ApiController
    {
        private AutogestionContext db = new AutogestionContext();

        private ViaticoRepository _repo;
        private ServiciosRepository _servicios = new ServiciosRepository();

        public ViaticosController()
        {
            _repo = new ViaticoRepository();

        }

        // GET: api/Viaticos
        public IEnumerable<string> Get()
        {

            return new string[] { "value1", "value2" };
        }

        // DELETE: api/Viaticos/5
        public void Delete(int id)
        {
        }


        [HttpGet]
        [Route("api/Viaticos/ConsultaViaticos/{Id}")]
        public IHttpActionResult Get(string Id)
        {


            List<Viaticos> Datos = new List<Viaticos>();

            int Emp = Convert.ToInt32(Id);
            using (var db = new AutogestionContext())
            {
                Datos = db.Viaticos.Where(e => e.EmpleadoId == Emp).ToList();
                foreach (Viaticos viatico in Datos)
                {
                    viatico.DestinoViatico = db.DestinoViaticos.Where(e => e.Id == viatico.DestinoViaticoID).FirstOrDefault();
                    viatico.Destino = viatico.DestinoViatico.Nombre;
                    var Estado = db.EstadosViaticos.Where(e => e.id == viatico.Estado).FirstOrDefault();
                    viatico.EstadoNombre = Estado.Nombre;
                }

            }

            return Json(Datos.OrderByDescending(a => a.Id));


        }
        [HttpGet]
        [Route("api/Viaticos/Viticoslist/{Id}")]
        public IHttpActionResult Viticlist(string Id)
        {


            List<Viaticos> Datos = new List<Viaticos>();

            int Emp = Convert.ToInt32(Id);
            using (var db = new AutogestionContext())
            {
                Datos = db.Viaticos.Where(e => e.EmpleadoId == Emp && e.Estado == 3).ToList();
                foreach (Viaticos viatico in Datos)
                {
                    viatico.DestinoViatico = db.DestinoViaticos.Where(e => e.Id == viatico.DestinoViaticoID).FirstOrDefault();
                    viatico.Destino = viatico.DestinoViatico.Nombre;
                    var Estado = db.EstadosViaticos.Where(e => e.id == viatico.Estado).FirstOrDefault();
                    viatico.EstadoNombre = Estado.Nombre;
                }

            }

            return Json(Datos.OrderByDescending(a => a.Id));


        }

        [HttpGet]
        [Route("api/Viaticos/ConsultaDestinos/{Estado}")]
        public IHttpActionResult Destinos(string Estado)
        {


            List<DestinoViatico> Datos = new List<DestinoViatico>();

            using (var db = new AutogestionContext())
            {
                Datos = db.DestinoViaticos.Where(e => e.Estado == Estado).ToList();

            }

            return Json(Datos);


        }
        [HttpGet]
        [Route("api/Viaticos/ConsultaVehiculos")]
        public IHttpActionResult Vehiculos()
        {
            List<Vehiculos> Datos = new List<Vehiculos>();
            using (var db = new AutogestionContext())
            {
                Datos = db.Vehiculos.Where(e => e.Estado == "Activo").ToList();

            }

            return Json(Datos);
        }
        [HttpGet]
        [Route("api/Viaticos/ConsultaVehiculo/{Id}")]
        public IHttpActionResult Vehiculo(string Id)
        {
            Vehiculos Datos = new Vehiculos();
            using (var db = new AutogestionContext())
            {
                var Id2 = Convert.ToInt32(Id);
                Datos = db.Vehiculos.Where(e => e.ID == Id2).FirstOrDefault();
            }
            return Json(Datos);
        }

        [HttpGet]
        [Route("api/Viaticos/ValoresViatico")]
        public IHttpActionResult ValoresViatico()
        {
            string[] Datos;
            using (var db = new AutogestionContext())
            {
                var text1 = db.Configuraciones.First(s => s.Parametro == "VALORINICIAL").Valor.ToString();
                var text2 = db.Configuraciones.First(s => s.Parametro == "VALORDIAVIATICO").Valor.ToString();
                //var Dias = db.Configuraciones.First(s => s.Parametro == "CANTDIASEXTRA").Valor.ToString();
                //Datos = new string[] { text1, text2, Dias };
                Datos = new string[] { text1, text2 };

            }


            return Json(Datos);
        }

        [HttpPost]
        [Route("api/Viaticos/GuardarViatico")]
        public IHttpActionResult GuardarViatico(Viaticos Viatico)
        {
            var respuesta = "";
            try
            {

                _repo.Crear(Viatico);
                respuesta = "El Usuario fue creado Exitosamente";
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error al crear:" + ex);
                respuesta = "Error al crear";
            }



            return Json(new { respuesta });


        }

        [HttpPost]
        [Route("api/Viaticos/CancelarSolicitud")]
        public IHttpActionResult CancelarSolicitud(Viaticos model)
        {
            return Json(_repo.Cancelar(model.Id, model.Observacion));

        }

        [HttpGet]
        [Route("api/Viaticos/VerificarViajeManual/{Id}/{EmpId}")]
        public IHttpActionResult ConfirmarViaje(int Id, int EmpId)
        {
            return Json(_repo.VerificarViajeManual(Id, "" ,true,""));
        }

        [HttpGet]
        [Route("api/Viaticos/ConsultaFechaFin")]
        public IHttpActionResult ConsultaFechaFin()
        {
            List<Viaticos> Datos = new List<Viaticos>();
            try
            {
                using (var db = new AutogestionContext())
                {
                    var fechaFin = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy"));
                    //var Year = Hoy.Year;
                    //var month = Hoy.Month;
                    //var day = Hoy.Day;
                    //var fechaFin = Convert.ToDateTime(day + "/" + month + "/" + Year);
                    string host = HttpContext.Current.Request.Url.Host;
                    int portnumber = HttpContext.Current.Request.Url.Port;
                    //CODIGO PARA TRABAJAR LOCAL
                    string url = host + ":" + portnumber + "/Viaticos/ConfirmacionViaje";
                    ////CODIGO PARA PRUEBAS
                    //string url = host + "/Viatico/ConfirmacionViaje";
                    Datos = db.Viaticos.Where(e => e.FechaFin == fechaFin).ToList();
                    foreach (var item in Datos)
                    {
                        Empleado Emp = db.Empleados.Where(x => x.Id == item.EmpleadoId).FirstOrDefault();
                        var cadena = item.Id + "%" + Emp.Id;
                        var encriptado = _servicios.encriptar(cadena);
                        var url2 = url + "?cadena=" + encriptado;
                        var QR = _servicios.GenerarQR(url2);

                        var respuesta = Notificar_Encuesta(item.Id, Emp.Correo, Emp.Id, fechaFin, QR, url2);
                    }
                    return Json(true);
                }
            }
            catch
            {
                return Json(false);
            }
        }

        public bool Notificar_Encuesta(int IdViatico, string Emailemp, int usuarioid, DateTime fecha, Byte[] QR, string LINK)
        {
            using (var db = new AutogestionContext())
            {

                string textocorreo = "";
                bool confirmacion;
                string txtde = Properties.Settings.Default.Correo.ToString();
                string contraseñacorreo = "";
                var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
                var Fecha2 = String.Format("{0:dd-MM-yyyy}", fecha);
                var email = ""+ Emailemp;
                //var email = "cplazas66@misena.edu.co";

                //var i = Image.FromStream(new MemoryStream(QR));

                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

                string texto = "";
                texto = db.Configuraciones.First(s => s.Parametro == "TXTVIATICOENCUESTACONFIRMACION").Valor.ToString();
                texto = texto.Replace("$ID", Convert.ToString(IdViatico));
                texto = texto.Replace("$LINK", LINK);


                textocorreo = texto;



                try
                {


                    correo.To.Add(email);

                    correo.Subject = "Notificación Solicitud Viático";
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
        }

        [HttpGet]
        [Route("api/Viaticos/MinDiasViaticos")]
        public IHttpActionResult MinDiasViaticos() 
        {
            Configuraciones Config = new Configuraciones();
            int valor = 0;
            try 
            {
                Config = db.Configuraciones.Where(x=>x.Parametro== "VR_CDIAMIN").FirstOrDefault();
                if (Config.Valor != "" && Config.Valor !=null) 
                {
                    valor = Convert.ToInt32(Config.Valor);
                }
            }
            catch ( Exception ex) 
            {
            
            }
            return Json(valor);
        }
    }
}
