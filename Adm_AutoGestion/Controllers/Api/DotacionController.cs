using Adm_AutoGestion.DTO;
using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Adm_AutoGestion.Controllers.Api
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DotacionController : ApiController
    {
        AutogestionContext db = new AutogestionContext();
        DotacionRepository _repo = new DotacionRepository();

        [HttpGet]
        [Route("api/Dotacion/ActualizarDotacion")]
        public IHttpActionResult ActualizarDotaciones()
        {
            return Json(_repo.ActualizarDotaciones());
        }

        [HttpGet]
        [Route("api/Dotacion/LimpiezaEmpleadoNoActivo")]
        public IHttpActionResult LimpiezaEmpleadoNoActivo()
        {
            return Json(_repo.LimpiezaEmpleadoNoActivo());
        }

        [HttpGet]
        [Route("api/Dotacion/ComprobarDotacion/{Id}")]
        public IHttpActionResult ComprobarDotacion(int Id)
        {
            return Json(_repo.ComprobarDotacion(Id));
        }

        [HttpGet]
        [Route("api/Dotacion/ObtenerTallas/{Id}")]
        public IHttpActionResult ObtenerTallas(int Id)
        {
            return Json(_repo.ObtenerTallas(Id));
        }

        [HttpPost]
        [Route("api/Dotacion/GuardarTallas")]
        public IHttpActionResult GuardarTallas(DotacionDTO model)
        {
            return Json(_repo.GuardarTallas(model));
        }
    }
}