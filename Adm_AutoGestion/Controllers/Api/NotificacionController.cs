using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Adm_AutoGestion.Controllers.Api
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class NotificacionController : ApiController
    {

        public NotificacionReposity _repository = new NotificacionReposity();

   


        [HttpGet]

        [Route("api/notificacion/GetNotifications/{empleadoid}")]

        public async Task<List<Notificacion>> GetNotifications(int empleadoid)
        {
            return await _repository.GetNotifications(empleadoid);
        }

        [HttpDelete]
        [Route("api/notificacion/DeleteNotification/{id}")]
        public async Task<bool> DeleteNotification(int id)
        {
            var notification = await _repository.DeleteNotification(id);

            return notification;
        }


    }
}
