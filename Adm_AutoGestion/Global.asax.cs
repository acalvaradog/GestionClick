using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using Adm_AutoGestion.App_Start;
using System.Web.Http;
using Adm_AutoGestion.Tareas;
//using Microsoft.AspNet.SignalR;
namespace Adm_AutoGestion
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            BungleConfig.RegisterBundles(BundleTable.Bundles);
            //Planificacion.StartNotificacion();
            //Planificacion.StartActualizacionEmpleados();
            //Planificacion.StartActualizacionEstructuraJerarquicadeEmpleados();
          
        }
    }
}
