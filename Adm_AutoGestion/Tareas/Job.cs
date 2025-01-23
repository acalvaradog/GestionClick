using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using System.Net.Mail;
using System.Net;
using Adm_AutoGestion.Controllers;
using System.Threading.Tasks;
namespace Adm_AutoGestion.Tareas
{
    public class JobNotificacionAutoevaluacion :IJob
    {
        public async void Execute(IJobExecutionContext context)
        {
            NotificacionesController notificacion = new NotificacionesController();
            await notificacion.EnviarNotificacion("AUTOEVALUACIÓN", "Si HOY no ha diligenciado la autoevaluación de síntomas COVID, recuerde que es su compromiso realizarlo diariamente ", "https://foscal.co/admautogestion/Contents/image/icono.png", "");

        }


    }
}