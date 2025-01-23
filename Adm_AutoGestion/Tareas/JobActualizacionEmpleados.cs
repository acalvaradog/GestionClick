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
using System.Data.Entity;
using System.IO;
using System.Text;
using System.Data;
using Adm_AutoGestion.Services;
namespace Adm_AutoGestion.Tareas
{
    public class JobActualizacionEmpleados : IJob
    {

         private EntregaEPPRepository _repo;
        private ServiciosRepository _servicios;
        public JobActualizacionEmpleados()
        {
           
            _servicios = new ServiciosRepository();

        }

        public  void Execute(IJobExecutionContext context)
        {
            EmpleadoRepository er = new EmpleadoRepository();
            er.ProcesoActualizacionyRetirados();
            er.ActualizarEstructuraJerarquicaEmpleado();
        }

        

    }
}