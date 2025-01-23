using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Adm_AutoGestion.Models;

namespace Adm_AutoGestion.Controllers.Api
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DetalleEntregaEPPController : ApiController
    {
        // GET api/detalleentregaepp
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/detalleentregaepp/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/detalleentregaepp
        public void Post([FromBody]string value)
        {
        }

        // PUT api/detalleentregaepp/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/detalleentregaepp/5
        public void Delete(int id)
        {
        }

        [HttpGet]
        [Route("api/detallentrega/firmarentrega/{id}/{id_empleado}/{medio}")]
        public string firmarentrega(string id,string id_empleado,string medio)
        {

           int empleado_id = Convert.ToInt16(id_empleado);
             int entrega_id = Convert.ToInt16(id);
           List<DetalleEntregaEPP> DetalleEPP = new List<DetalleEntregaEPP>();
             using (var db = new AutogestionContext())
            {
               
                try {
                    DetalleEPP = db.DetalleEntregaEPP.Where(x => x.NumeroEntrega == entrega_id && x.EmpleadoId == empleado_id && ( x.FechaFirma == null || x.FechaFirma =="" )).ToList();
              
                if (DetalleEPP.Count < 1)
                {

                    return "nopendiente";
                }
                else { 
                
                    foreach(var item in DetalleEPP){
                        item.FechaFirma = DateTime.Now.ToString() + "|" + medio;
                        
                    }
                    db.SaveChanges();
                    return "ok";
                }
                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }

             }
           
        }
    }
}
