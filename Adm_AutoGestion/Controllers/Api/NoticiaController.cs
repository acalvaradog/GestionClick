using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

using System.Windows.Media.Animation;
using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
namespace Adm_AutoGestion.Controllers.Api
{ 
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class NoticiaController : ApiController
    {
       
        NoticiaRepository _repository = new NoticiaRepository();


        [HttpGet]
        [Route("api/Noticia/ListarNoticias")]
        public async Task<List<Noticia>> ListarNoticias()
        {
            return await _repository.ObtenerTodosActuales();
        }

        [HttpGet]
        [Route("api/Noticia/GetNoticiasConImagenes")]
        public async Task<IHttpActionResult> GetNoticiasConImagenes()
        {
            try
            {
                var noticias = await _repository.ObtenerNoticiasConImagenesAsync();
                return Ok(noticias);  // Devolvemos los DTOs de noticias con sus imágenes
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
