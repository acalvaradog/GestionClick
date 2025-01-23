using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using Adm_AutoGestion.Controllers;
using System.Threading.Tasks;
using Quartz;
using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
namespace Adm_AutoGestion.Tareas
{
    public class JobNotificacionFirmaEPP : IJob
    {

         private EntregaEPPRepository _repo;
        private ServiciosRepository _servicios;
        public JobNotificacionFirmaEPP()
        {
            _repo = new EntregaEPPRepository();
            _servicios = new ServiciosRepository();

        }

        public  void Execute(IJobExecutionContext context)
        {
            List<DetalleEntregaEPP> Entrega = new List<DetalleEntregaEPP>();

            using (var db = new AutogestionContext())
            {

                try
                {

                    Entrega = db.DetalleEntregaEPP.Where(x => x.Estado == "Activo" && x.FechaFirma == null && x.EntregaEPP.Estado =="Activo").ToList();

                    foreach (var item in Entrega)
                    {
                        Empleado empleado = new Empleado();
                        empleado = db.Empleados.FirstOrDefault(x => x.Id == item.EmpleadoId);

                        if (item.FechaFirma == null)
                        {
                            _servicios.EnviarEmailEPP(empleado, item);
                        }
                    }

                   
                }
                catch (Exception ex)
                {

                 
                }


            }
        }

    }
}