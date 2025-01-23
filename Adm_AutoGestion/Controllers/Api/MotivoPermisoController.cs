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

namespace Adm_AutoGestion.Controllers.Api
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MotivoPermisoController : ApiController
    {
        private MotivoPermisoRepository _repo;

        public MotivoPermisoController()
        {
            _repo = new MotivoPermisoRepository();
        }
        
        // GET api/motivopermiso
        public string Get()
        {
            var model = _repo.ObtenerTodos();

            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(model);
            return json;
        }
    }
}
