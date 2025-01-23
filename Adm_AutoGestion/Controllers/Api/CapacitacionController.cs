using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using System.Web.Http.Cors;
using System.Web.Http;
using Autogestion.Shared.DTO.TalentoHumano;
using System.Web;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace Adm_AutoGestion.Controllers.Api
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CapacitacionController : ApiController
    {

        private CapacitacionRepository _repo;
        private AutogestionContext db = new AutogestionContext();

        public CapacitacionController()
        {
            _repo = new CapacitacionRepository();

        }

        [HttpPost]
        [Route("api/TalentoHumano/SubirArchivo")]
        public async Task<HttpResponseMessage> SubirArchivo()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            var EmpId = HttpContext.Current.Request.Params["EmpleadoId"];
            var Titulo = HttpContext.Current.Request.Params["Titulo"];
            var FechaCaducidad = HttpContext.Current.Request.Params["FechaCaducidad"];
            var fecha = DateTime.Parse(FechaCaducidad);
            var EmpleadoId = int.Parse(EmpId);

            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                var nombreArchivo = DateTime.Now.ToString("yyyyMMHHmmssffff").ToString() + "-" + filename;
                var contentType = file.Headers.ContentType.MediaType;
                var buffer = await file.ReadAsByteArrayAsync();

                if (contentType != "application/pdf")
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Solo se permiten archivos PDF.");
                }

                if (buffer.Length > 10 * 1024 * 1024)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "El archivo no debe superar los 10 MB.");
                }

                var filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/AnexosCertificados"), nombreArchivo);

                var respuestaRepo = _repo.SubirArchivo(EmpleadoId, Titulo, nombreArchivo, fecha);

                if (respuestaRepo == "true")
                {
                    File.WriteAllBytes(filePath, buffer);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

            }

            return Request.CreateResponse(HttpStatusCode.BadRequest, "Error inesperado");

        }

        [HttpGet]
        [Route("api/TalentoHumano/MisCertificados/{EmpleadoId}")]
        public IHttpActionResult MisCertificados(int EmpleadoId)
        {
            List<Certificados> cert = db.Certificados.Where(e => e.EmpleadoId == EmpleadoId).ToList();
            List<CertificadosDTO> certDTOs = new List<CertificadosDTO>();

            foreach (var i in cert)
            {
                CertificadosDTO certDTO = new CertificadosDTO();
                certDTO.Id = i.Id;
                certDTO.EmpleadoId = i.EmpleadoId;
                certDTO.Titulo = i.Titulo;
                certDTO.Archivo = i.Archivo;
                certDTO.Estado = i.Estado;
                certDTO.Observacion = i.Observacion;
                certDTO.FechaCaducidad = i.FechaCaducidad;

                certDTOs.Add(certDTO);
            }

            return Json(certDTOs);
        }

        [HttpGet]
        [Route("api/TalentoHumano/ListarCursosNormativa")]
        public IHttpActionResult ListarCursosNormativa()
        {
            List<CursosNormativa> cursos = db.CursosNormativa.Where(x => x.Estado == "Activo").ToList();
            List<CursosNomativaDTO> cursosDTO = new List<CursosNomativaDTO>();

            foreach (var i in cursos)
            {
                CursosNomativaDTO cursoDTO = new CursosNomativaDTO();
                cursoDTO.Id = i.Id;
                cursoDTO.Titulo = i.Titulo;
                cursoDTO.Estado = i.Estado;

                cursosDTO.Add(cursoDTO);
            }

            return Json(cursosDTO);
        }

        [HttpGet]
        [Route("api/TalentoHumano/Listar/{EmpleadoId}/{Metodologia}")]
        public IHttpActionResult Listar(int EmpleadoId, int Metodologia)
        {
            var anioactual = DateTime.Now.Year;
            var startDate = new DateTime(anioactual, 1, 1);
            var endDate = new DateTime(anioactual, 12, 31);

            var capacitaciones = db.Capacitacion
                .Where(e => e.Metodologia == Metodologia && e.Estado == "Activo" && e.FechaCapacitacion >= startDate && e.FechaCapacitacion <= endDate)
                .OrderBy(e => e.FechaCapacitacion)
                .ToList();

            var capacitacionIds = capacitaciones.Select(c => c.Id).ToList();
            var detallesCapacitacion = db.DetalleCapacitacion
                .Where(dc => capacitacionIds.Contains(dc.CapacitacionId) && dc.EmpleadoId == EmpleadoId)
                .ToList();

            var tipoPECs = db.TipoPEC.Where(t => capacitacionIds.Contains(t.Id)).ToDictionary(t => t.Id, t => t.Valor);
            var requerimientos = db.RequerimientoInstitucional.Where(r => capacitacionIds.Contains(r.Id)).ToDictionary(r => r.Id, r => r.Valor);

            var capDTOs = capacitaciones
                .Select(i =>
                {
                    var detalle = detallesCapacitacion.FirstOrDefault(dc => dc.CapacitacionId == i.Id);
                    if (detalle == null) return null;

                    var capDTO = new CapacitacionDTO
                    {
                        Id = i.Id,
                        Nombre = i.Nombre,
                        FechaCapacitacion = i.FechaCapacitacion,
                        HoraInicio = i.HoraInicio,
                        HoraFin = i.HoraFin,
                        temas = i.temas,
                        Objetivo = i.Objetivo,
                        Modalidad = i.Modalidad,
                        Ciudad = i.Ciudad,
                        Lugar = i.Lugar,
                        Asistencia = detalle.FechaFirma != null ? "SI" : "NO",
                        ResponsablePrograma = tipoPECs.TryGetValue(i.IdTipoPEC, out var tipoPEC) && tipoPEC == "Cultura Organizacional" ? i.ResponsablePrograma : i.Responsable,
                        AreaObjetivo = i.AreaObjetivo,
                        Proveedor = i.Proveedor,
                        RequerimientoInstitucional = requerimientos.TryGetValue(i.IdRequerimientoInstitucional, out var reqIns) ? reqIns : null,
                        Estado = i.Estado,
                        EvaluacionConocimiento = i.EvaluacionConocimiento,
                        EncuestaSatisfaccion = i.EncuestaSatisfaccion
                    };

                    return capDTO;
                })
                .Where(dto => dto != null)
                .ToList();

            return Json(capDTOs);
        }

        [HttpGet]
        [Route("api/TalentoHumano/Detalles/{IdCap}")]
        public IHttpActionResult Detalles(int IdCap)
        {

            var anioactual = DateTime.Now.Year;
            Capacitacion cap = db.Capacitacion.FirstOrDefault(e => e.Id == IdCap);
            CapacitacionDTO capDTO = new CapacitacionDTO();

            capDTO.Id = cap.Id;
            capDTO.Nombre = cap.Nombre;
            capDTO.FechaCapacitacion = cap.FechaCapacitacion;
            capDTO.HoraInicio = cap.HoraInicio;
            capDTO.HoraFin = cap.HoraFin;
            capDTO.temas = cap.temas;
            capDTO.Objetivo = cap.Objetivo;
            capDTO.Modalidad = cap.Modalidad;
            capDTO.Ciudad = cap.Ciudad;
            capDTO.Lugar = cap.Lugar;
            capDTO.Docente = cap.Docente;

            //var PICO = db.TipoPEC.FirstOrDefault(x => x.Id == cap.IdTipoPEC)?.Valor;

            if (cap.IdTipoPEC == 2)
            {
                capDTO.ResponsablePrograma = cap.ResponsablePrograma;
            }
            else
            {
                capDTO.Responsable = cap.Responsable;
            }

            capDTO.AreaObjetivo = cap.AreaObjetivo;
            capDTO.Proveedor = cap.Proveedor;

            var ReqIns = db.RequerimientoInstitucional.FirstOrDefault(x => x.Id == cap.IdRequerimientoInstitucional)?.Valor;

            if (!string.IsNullOrEmpty(ReqIns))
            {
                capDTO.RequerimientoInstitucional = ReqIns;
            }

            capDTO.Estado = cap.Estado;
            capDTO.EvaluacionConocimiento = cap.EvaluacionConocimiento;
            capDTO.EncuestaSatisfaccion = cap.EncuestaSatisfaccion;

            return Json(capDTO);
        }

        [HttpGet]
        [Route("api/TalentoHumano/Historico/{EmpleadoId}")]
        public IHttpActionResult Historico(int EmpleadoId)
        {
            var capacitaciones = db.Capacitacion
                .Where(e => e.Estado == "Cerrado")
                .OrderBy(e => e.FechaCapacitacion)
                .ToList();

            var capacitacionIds = capacitaciones.Select(c => c.Id).ToList();
            var detallesCapacitacion = db.DetalleCapacitacion
                .Where(dc => capacitacionIds.Contains(dc.CapacitacionId) && dc.EmpleadoId == EmpleadoId)
                .ToList();

            var tipoPECs = db.TipoPEC.Where(t => capacitacionIds.Contains(t.Id)).ToDictionary(t => t.Id, t => t.Valor);
            var requerimientos = db.RequerimientoInstitucional.Where(r => capacitacionIds.Contains(r.Id)).ToDictionary(r => r.Id, r => r.Valor);

            var capDTOs = capacitaciones
                .Select(i =>
                {
                    var detalle = detallesCapacitacion.FirstOrDefault(dc => dc.CapacitacionId == i.Id);
                    if (detalle == null) return null;

                    var capDTO = new CapacitacionDTO
                    {
                        Id = i.Id,
                        Nombre = i.Nombre,
                        FechaCapacitacion = i.FechaCapacitacion,
                        HoraInicio = i.HoraInicio,
                        HoraFin = i.HoraFin,
                        temas = i.temas,
                        Objetivo = i.Objetivo,
                        Modalidad = i.Modalidad,
                        Ciudad = i.Ciudad,
                        Lugar = i.Lugar,
                        Asistencia = detalle.FechaFirma != null ? "SI" : "NO",
                        ResponsablePrograma = tipoPECs.TryGetValue(i.IdTipoPEC, out var tipoPEC) && tipoPEC == "Cultura Organizacional" ? i.ResponsablePrograma : i.Responsable,
                        AreaObjetivo = i.AreaObjetivo,
                        Proveedor = i.Proveedor,
                        RequerimientoInstitucional = requerimientos.TryGetValue(i.IdRequerimientoInstitucional, out var reqIns) ? reqIns : null,
                        Estado = i.Estado,
                        EvaluacionConocimiento = i.EvaluacionConocimiento,
                        EncuestaSatisfaccion = i.EncuestaSatisfaccion
                    };

                    return capDTO;
                })
                .Where(dto => dto != null)
                .ToList();

            return Json(capDTOs);
        }

        [HttpGet]
        [Route("api/TalentoHumano/FormacionesAbiertas/{EmpleadoId}")]
        public IHttpActionResult FormacionesAbiertas(int EmpleadoId)
        {
            var anioactual = DateTime.Now.Year;
            var startDate = new DateTime(anioactual, 1, 1);
            var endDate = new DateTime(anioactual, 12, 31);

            var capacitaciones = db.Capacitacion
                .Where(e => e.Estado == "Activo" && e.FechaCapacitacion >= startDate && e.FechaCapacitacion <= endDate)
                .OrderBy(e => e.FechaCapacitacion)
                .ToList();

            var capacitacionIds = capacitaciones.Select(c => c.Id).ToList();
            var detallesCapacitacion = db.DetalleCapacitacion
                .Where(dc => capacitacionIds.Contains(dc.CapacitacionId) && dc.EmpleadoId == EmpleadoId)
                .ToList();

            var tipoPECs = db.TipoPEC.Where(t => capacitacionIds.Contains(t.Id)).ToDictionary(t => t.Id, t => t.Valor);
            var requerimientos = db.RequerimientoInstitucional.Where(r => capacitacionIds.Contains(r.Id)).ToDictionary(r => r.Id, r => r.Valor);

            var capDTOs = capacitaciones
                .Select(i =>
                {
                    var detalle = detallesCapacitacion.FirstOrDefault(dc => dc.CapacitacionId == i.Id);
                    if (detalle == null) return null;

                    var capDTO = new CapacitacionDTO
                    {
                        Id = i.Id,
                        Nombre = i.Nombre,
                        FechaCapacitacion = i.FechaCapacitacion,
                        HoraInicio = i.HoraInicio,
                        HoraFin = i.HoraFin,
                        temas = i.temas,
                        Objetivo = i.Objetivo,
                        Modalidad = i.Modalidad,
                        Ciudad = i.Ciudad,
                        Lugar = i.Lugar,
                        Asistencia = detalle.FechaFirma != null ? "SI" : "NO",
                        ResponsablePrograma = tipoPECs.TryGetValue(i.IdTipoPEC, out var tipoPEC) && tipoPEC == "Cultura Organizacional" ? i.ResponsablePrograma : i.Responsable,
                        AreaObjetivo = i.AreaObjetivo,
                        Proveedor = i.Proveedor,
                        RequerimientoInstitucional = requerimientos.TryGetValue(i.IdRequerimientoInstitucional, out var reqIns) ? reqIns : null,
                        Estado = i.Estado,
                        EvaluacionConocimiento = i.EvaluacionConocimiento,
                        EncuestaSatisfaccion = i.EncuestaSatisfaccion
                    };

                    return capDTO;
                })
                .Where(dto => dto != null)
                .ToList();

            return Json(capDTOs);
        }

        // GET api/default1
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/default1/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/default1
        public void Post([FromBody]string value)
        {
        }

        // PUT api/default1/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/default1/5
        public void Delete(int id)
        {
        }

        [HttpGet]
        [Route("api/capacitacion/firmacapacita/{id}/{id_empleado}/{medio}")]
        public string firmacapacita(string id, string id_empleado, string medio)
        {

            int empleado_id = Convert.ToInt16(id_empleado);
            int capacita_id = Convert.ToInt16(id);
            List<DetalleCapacitacion> DetalleCapacita = new List<DetalleCapacitacion>();
            string message = "";
            

            using (var db = new AutogestionContext())
            {

                try
                {
                    DetalleCapacita = db.DetalleCapacitacion.Where(x => x.CapacitacionId == capacita_id && x.EmpleadoId == empleado_id && (x.FechaFirma == null || x.FechaFirma == "")).ToList();

                    if (DetalleCapacita.Count > 0)
                    {
                        foreach (var item in DetalleCapacita)
                        {
                            item.FechaFirma = DateTime.Now.ToString() + "|" + medio;

                        }
                        db.SaveChanges();
                        return "ok";
                    }
                    else
                    {
                        message = _repo.Modificar(capacita_id, empleado_id, medio);
                        return message;
                    }
                }
                catch (Exception ex)
                {
                    message = "Error, " + ex.Message.ToString();
                    return message;
                }

            }
            

        }



        [HttpGet]
        [Route("api/encuestacapacitacion/{id}/{cedula}")]
        public string encuestacapacitacion(string id, string cedula)
        {

            //DetalleCapacitacion det = new DetalleCapacitacion();

            using (var db = new AutogestionContext())
            {
                try
                {

                    var codigo = Convert.ToInt32(id);

                    Empleado empl = db.Empleados.FirstOrDefault(e => e.Documento == cedula);

                    DetalleCapacitacion det = db.DetalleCapacitacion.FirstOrDefault(e => e.CapacitacionId == codigo && e.EmpleadoId == empl.Id);

                    
                    det.RespuestaEncuesta = "SI";
                    _repo.ActualizarEnvioEncuesta(det);

                    
                    return "OK";
                }
                catch
                {
                    return "ERROR";

                }
            }
        }



    }
}
