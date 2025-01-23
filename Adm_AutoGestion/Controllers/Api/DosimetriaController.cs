using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;


namespace Adm_AutoGestion.Controllers.Api
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DosimetriaController : ApiController
    {
        private readonly DosimetriaRepository _repository;
        private readonly AutogestionContext _context;

        public DosimetriaController()
        {
            _context = new AutogestionContext();
            _repository = new DosimetriaRepository(_context);
        }

        [HttpGet]
        [Route("api/Dosimetria/GetDosisAnualYAcumulada")]
        public IHttpActionResult GetDosisAnualYAcumulada(int empleadoId)
        {
            try
            {
                var resultado = _repository.ObtenerReporteDosimetriaxempleado(empleadoId);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
