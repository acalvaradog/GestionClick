using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;

namespace Adm_AutoGestion.Controllers
{
    public class NotificacionesController : Controller
    {
        //
        // GET: /Notificaciones/
        private NotificacionReposity _repository = new NotificacionReposity();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Notificar()
        {
            return View();
        }


        public ActionResult ListaDispositivos()
        {

            using (var db = new AutogestionContext())
            {
                var model = db.Dispositivos.ToList();
                return View(model);
            }

        }






        public async Task<String> EnviarNotificacion(string titulo, string texto, string icono, string accion)
        {

            using (var db = new AutogestionContext())
            {


                // Get the server key from FCM console
                var serverKey = string.Format("key={0}", "AAAAW-LWqNk:APA91bHcqBON1kIe8iYeDh_deiM6DUAs5S74growtm7LkIIAK-frWfWpdjJyVXn_7LVIUjxaAN6fokJ1UOHNbVDnDWhkBZPA5o7ec4FSM2qq9MIsiCSJIluk4jwoxUmX0dT_2fcWwgLx");
                //  var serverKey = string.Format("key={0}", "AIzaSyBbjfI2Ho6KaIZcpzlDvPE-tUqCV3UYmnM");


                // Get the sender id from FCM console
                var senderId = string.Format("id={0}", "394647742681");
                List<Dispositivos> dispositivos = new List<Dispositivos>();
                dispositivos = db.Dispositivos.ToList();

                foreach (var dispositivo in dispositivos)
                {


                    try
                    {
                        //var base64EncodedBytes = System.Convert.FromBase64String(dispositivo.token.ToString());
                        //var para = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                        var para = dispositivo.token.ToString();
                        var data = new
                        {
                            to = para, // Recipient device token
                            notification = new { title = titulo, body = texto, icon = icono, click_action = accion }
                        };

                        // Using Newtonsoft.Json
                        var jsonBody = JsonConvert.SerializeObject(data);

                        using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/fcm/send"))
                        {
                            httpRequest.Headers.TryAddWithoutValidation("Authorization", serverKey);
                            //  httpRequest.Headers.TryAddWithoutValidation("Sender", senderId);
                            httpRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                            using (var httpClient = new HttpClient())
                            {
                                var result = await httpClient.SendAsync(httpRequest);

                                if (result.IsSuccessStatusCode)
                                {
                                    // return "OK"; ;
                                }
                                else
                                {
                                    // Use result.StatusCode to handle failure
                                    // Your custom error handler here
                                    //return "error";
                                }
                            }
                        }

                    }


                    catch (Exception ex)
                    {
                        return ex.Message.ToString();
                    }




                }
                return "OK";
            }

            return "OK";

        }


        public async Task<String> EnviarNotificacionDispositivo(string titulo, string texto, string icono, string accion, string dispositivo)
        {

            return await _repository.NotificacionPush(titulo, texto, icono, accion, dispositivo);



        }
    }
}




   







       
