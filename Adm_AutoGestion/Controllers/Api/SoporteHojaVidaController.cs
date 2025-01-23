using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Adm_AutoGestion.Controllers.Api
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SoporteHojaVidaController : ApiController
    {
        private SoporteHojaDeVidaRepository _repository;

        public  SoporteHojaVidaController(){
            _repository = new SoporteHojaDeVidaRepository();
    }

        [HttpPost]
        [Route("api/SoporteHojaVida/GuardarSoporte")]
        public IHttpActionResult GuardarSoporte()
        {

            SoportesHojaDeVida soporte = new SoportesHojaDeVida();
            soporte.Archivo = HttpContext.Current.Request.Files["Archivo"];
            soporte.EmpleadoId = Convert.ToInt32(HttpContext.Current.Request.Params["EmpleadoId"].ToString());
            soporte.Titulo = HttpContext.Current.Request.Params["Titulo"];
            soporte.TipoSoporteId = Convert.ToInt32(HttpContext.Current.Request.Params["TipoSoporte"].ToString());
            soporte.NombreArchivo = HttpContext.Current.Request.Params["NombreArchivo"].ToString();
            if (_repository.GuardarSoporte(soporte))
                return Ok();
            else
                return StatusCode(HttpStatusCode.BadRequest);

        }
        [HttpGet]
        [Route("api/SoporteHojaVida/GetSoportes/{id}")]
        public IHttpActionResult GetSoportes(int id)
        {

            return Json(_repository.ObtenerSoportes(id));
        }
        [HttpGet]
        [Route("api/SoporteHojaVida/TipoSoportes")]
        public IHttpActionResult TipoSoportes()
        {

            return Json(_repository.ListarTipoSoporte());
        }

    }
 
}
