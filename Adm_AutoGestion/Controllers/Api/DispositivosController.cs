using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Adm_AutoGestion.Models;
using System.Web.Http.Cors;
using System.Web;
namespace Adm_AutoGestion.Controllers.Api
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DispositivosController : ApiController
    {

             // GET api/dispositivos
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/dispositivos/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/dispositivos
        [HttpPost]
        public IHttpActionResult Post(Dispositivos Dispositivos)
        {

            var token =    Dispositivos.token;
            var codempleado =     Dispositivos.NroEmpleado;
            var documento =     Dispositivos.Documento;

            using (var db = new AutogestionContext())
            {

                try
                {
                    var dispositivoreg = db.Dispositivos.FirstOrDefault(x => x.token == token);

                    if (dispositivoreg != null)
                    {
                        dispositivoreg.Documento = documento;
                        dispositivoreg.NroEmpleado = codempleado;
                        dispositivoreg.FechaRegistro = DateTime.Now.Date;
                        db.SaveChanges();
                        return Json("OK");
                    }
                    else {
                        var dispositivo = new Dispositivos();
                        dispositivo.token = token;
                        dispositivo.NroEmpleado = codempleado.ToString();
                        dispositivo.Documento = documento.ToString();
                        dispositivo.FechaRegistro = DateTime.Now.Date;
                        db.Dispositivos.Add(dispositivo);
                        db.SaveChanges();
                        return Json("OK");
                    
                    }

                 
                }
                catch (Exception ex)
                {

                    return Json(ex.Message.ToString());

                }



            }

        }

 
        // PUT api/dispositivos/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/dispositivos/5
        public void Delete(int id)
        {
        }
    }
}
