using Adm_AutoGestion.DTO;
using Adm_AutoGestion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Navigation;

namespace Adm_AutoGestion.Services
{
   

    public class NotificacionReposity
    {

        public AutogestionContext _context = new AutogestionContext();


        public async Task<List<Notificacion>> GetNotifications(int idEmpleado)
        {
            return await _context.Notificacion.Where(x=> x.EmpleadoId == idEmpleado).ToListAsync();
        }


        public async Task<bool> CreateNotification(Notificacion notification)
        {
            try
            {
       
            _context.Notificacion.Add(notification);
            await _context.SaveChangesAsync();
 

            return true;
            }
            catch (Exception)
            {

                return false;
            }
     
        }

        public async Task<bool> DeleteNotification(int id)
        {
            var notification = await _context.Notificacion.FindAsync(id);

            if (notification == null)
            {
                return false;
            }

            _context.Notificacion.Remove(notification);
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<String> NotificacionPush(string titulo, string texto, string icono, string accion, string dispositivo)
        {

            using (var db = new AutogestionContext())
            {


                // Get the server key from FCM console
                var serverKey = string.Format("key={0}", "AAAAW-LWqNk:APA91bHcqBON1kIe8iYeDh_deiM6DUAs5S74growtm7LkIIAK-frWfWpdjJyVXn_7LVIUjxaAN6fokJ1UOHNbVDnDWhkBZPA5o7ec4FSM2qq9MIsiCSJIluk4jwoxUmX0dT_2fcWwgLx");
                //  var serverKey = string.Format("key={0}", "AIzaSyBbjfI2Ho6KaIZcpzlDvPE-tUqCV3UYmnM");


                // Get the sender id from FCM console
                var senderId = string.Format("id={0}", "394647742681");




                try
                {
                    //var base64EncodedBytes = System.Convert.FromBase64String(dispositivo.ToString());
                    //var para = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                    var para = dispositivo;
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

        public async Task<bool> EnvioNotificacion(NotificacionDTO notificacion) {

            await CreateNotification(new Notificacion { EmpleadoId = notificacion.EmpleadoId, Fecha = DateTime.Now, Mensaje = notificacion.Mensaje });

            await NotificacionPush(notificacion.Titulo, notificacion.Mensaje,notificacion.Icono,notificacion.Accion, notificacion.Dispositivo);
            return true;
        }
    
    }
}