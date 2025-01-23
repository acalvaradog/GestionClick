using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Adm_AutoGestion.Services;
using Adm_AutoGestion.Models;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;

namespace Adm_AutoGestion.Controllers.Api
{
     [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PreguntasController : ApiController
    {

        private PreguntasRepository _repo;

        
        public PreguntasController()
        {
            _repo = new PreguntasRepository();
        }


        // GET api/preguntas
        public string Get()
        {
            //return new string[] { "value1", "value2" };
            var model = _repo.ObtenerTodos();

            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(model);
            return json;
        }
        [HttpGet]
        [Route("api/Preguntas/ObtenerPreguntasdelTema/{TemaId}")]
        public List<PreguntaFrecuente> ObtenerPreguntasdelTema( int TemaId) 
        {
            List<PreguntaFrecuente> Preguntas = new List<PreguntaFrecuente>();
            try 
            {
                 Preguntas = _repo.ObtenerPreguntasxTema(TemaId);
            }
            catch (Exception ex) 
            {
            
            }
            return Preguntas;
        }
 



    }
}
