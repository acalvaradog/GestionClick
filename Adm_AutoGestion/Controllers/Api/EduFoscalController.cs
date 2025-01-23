using Adm_AutoGestion.DTO;
using Adm_AutoGestion.Models;
using Adm_AutoGestion.Models.CertificadosEduFoscal;
using Adm_AutoGestion.Services;
using Autogestion.Shared.DTO.Cursos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Adm_AutoGestion.Controllers.Api
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EduFoscalController : ApiController
    {
        public EduFoscalRepository _EduFoscalRepository = new EduFoscalRepository();
        private CertificadosContext db = new CertificadosContext(); 
       // public EduFoscalController(EduFoscalRepository eduFoscalRepository) {
       //_EduFoscalRepository= eduFoscalRepository;
       // }

        [HttpGet]
        [Route("api/EduFoscal/ListarCursos")]
        public async Task<List<CursoModdleDTO>>  ListarCursos()
        {
            return await _EduFoscalRepository.ListarCursos();
        }

        [HttpGet]
        [Route("api/EduFoscal/GetIdUserName/{identificacion}")]
        public async Task<UserMoodleDTO> GetIdUserName(string identificacion)
        {

            var respuesta = await _EduFoscalRepository.GetIdxUserName(identificacion);
            return respuesta.First();
        }

        [HttpGet]
        [Route("api/EduFoscal/GetCursosxUsuario/{id}")]
        public async Task<List<CursoxUsuarioDTO>> GetCursosxUsuario(int id)
        {
            return await _EduFoscalRepository.GetCursoxUserName(id);
        }

        [HttpGet]
        [Route("api/EduFoscal/GetCursosxCursoId/{courseId}")]
        public async Task<List<CursoModdleDTO>> GetCursosxCursoId(int courseId)
        {
            return await _EduFoscalRepository.GetCursoxCursoId(courseId);
        }
        
        [HttpGet]
        [Route("api/EduFoscal/GetCertificadosxCursoxUsuario/{userid}/{courseid}")]
        public IHttpActionResult GetCertificadosxCursoxUsuario(int userid, int courseid)
        {
            List<vista_certificados_reducida> certificados_Reducidas = db.CertificadoEduFoscal
                .Where(x => x.user_id == userid && x.course_id == courseid)
                .ToList();

            List<CertificadoDTO> cert = new List<CertificadoDTO>();

            foreach (var i in certificados_Reducidas)
            {
                CertificadoDTO certDTO = new CertificadoDTO();

                certDTO.user_id = i.user_id;
                certDTO.course_id = i.course_id;
                certDTO.certificate_url = i.certificate_url;
                certDTO.username = i.username;

                cert.Add(certDTO);
            }

            return Json(cert);
        }
    }
}
