using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using Adm_AutoGestion.Services;
using Adm_AutoGestion.Models;
using System.Web.Http.Cors;

namespace Adm_AutoGestion.Controllers.Api
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TemaPreguntasController : ApiController
    {

            private TemaPreguntaRepository _repo;

            public TemaPreguntasController()
        {
            _repo = new TemaPreguntaRepository();
        }

        // GET api/temapreguntas
        public string Get()
        {
               //return new string[] { "value1", "value2" };
            var model = _repo.ObtenerTodos();

            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(model);
            return json;
        }

        [HttpGet]
        [Route("api/TemaPreguntas/ObtenerTemas")]
        public List<TemaPregunta> ObtenerTemas()
        {
            List<TemaPregunta> List = new List<TemaPregunta>();
            try
            {

                List = _repo.ObtenerTodos();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return List;
        }

        // GET api/temapreguntas/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/temapreguntas
        public void Post([FromBody]string value)
        {
        }

        // PUT api/temapreguntas/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/temapreguntas/5
        public void Delete(int id)
        {
        }
    }
}
