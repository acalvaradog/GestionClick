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
using Newtonsoft.Json;

namespace Adm_AutoGestion.Controllers.Api
{

     [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PazySalvoController : ApiController
    {

        private AutogestionContext db = new AutogestionContext();
        // GET api/vacaciones
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/vacaciones/5
        public string Get(int id)
        {
            return "value";
        }



        [HttpGet]
        [Route("api/PazySalvo/{documento}")]
        public string Historial(string documento)
        {

            var Result = "";
            string resultado = "";
           
            try
            {
                int id = 0;
                Int32.TryParse(documento, out id);

                    using (var db = new AutogestionContext())
                    {
                        var empleado = db.PazySalvo.FirstOrDefault(x => x.EmpleadoId == id && x.Estado == "Activo");

                        if (empleado != null)
                        {

                            List<DetallePazySalvo> lista = db.DetallePazySalvo.Where(e => e.IdPazySalvo == empleado.Id).ToList();

                            foreach (var fila in lista)
                            {
                                var nombre = db.Empleados.FirstOrDefault(s => s.Id == fila.Responsable);
                                Result += String.Format("<tr><td>" + fila.Area + "</td><td>" + fila.FechaFirma + "</td><td>" + nombre.Nombres + "</td></tr>");
                                resultado = (Result);
                            }
                        }

                        
                    }
                

            }
            catch (SystemException ex)
            {
            }
           
            return resultado;

        }


       

       
       
    }
}
