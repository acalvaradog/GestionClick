using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Adm_AutoGestion.Controllers.Api
{
    public class DiasFestivosController : ApiController
    {
        // GET api/diasfestivos
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/diasfestivos/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/diasfestivos
        public void Post([FromBody]string value)
        {
        }

        // PUT api/diasfestivos/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/diasfestivos/5
        public void Delete(int id)
        {
        }
    }
}
