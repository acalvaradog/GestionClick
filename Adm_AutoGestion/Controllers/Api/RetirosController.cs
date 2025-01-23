using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
namespace Adm_AutoGestion.Controllers.Api
{
    public class RetirosController : ApiController
    {
            private RetirosRepository _repo;

         public RetirosController()
        {
            _repo = new RetirosRepository();

        }


        [HttpGet]
        [Route("api/encuestaretiro/{id}")]
        public string encuestaretiro(string id)
        {

            Retiros retiro = new Retiros();
                 using (var db = new AutogestionContext())
            {
                      try{

                          var codigo = Convert.ToInt32(id);
                         retiro = db.Retiros.Find(codigo);
                       retiro.RespuestaEncuesta = "SI";

                       _repo.ActualizarEnvioEncuesta(retiro);

                       return "OK";
                      }catch{
                          return "ERROR";
                      
                      }

                  


                 }
        }

    }
}
