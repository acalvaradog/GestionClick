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
using System.IO;
using System.Threading.Tasks;

namespace Adm_AutoGestion.Controllers.Api
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CesantiasController : ApiController
    {

        private readonly CesantiasRepository _repository;
        private readonly AutogestionContext _context;

        public CesantiasController()
        {
            _context = new AutogestionContext();
            _repository = new CesantiasRepository(_context);
        }

        [HttpPost]
        [Route("api/cesantias/registrar-solicitud-con-soportes")]
        public IHttpActionResult RegistrarSolicitudConSoportes()
        {
            string respuesta = "true";
            List<SoporteCesantia> Adjuntos = new List<SoporteCesantia>();
            try
            {
                var solicitud = new SolicitudCesantia
                {
                    EmpleadoId = Convert.ToInt16(HttpContext.Current.Request.Params["solicitud.EmpleadoId"]),
                    FechaRegistro = DateTime.Now,
                    ValorRetiro = Convert.ToDecimal(HttpContext.Current.Request.Params["solicitud.ValorRetiro"]),
                    DestinoId = Convert.ToInt16(HttpContext.Current.Request.Params["solicitud.DestinoId"]),
                    FondoCesantiasId = Convert.ToInt16(HttpContext.Current.Request.Params["solicitud.FondoCesantiasId"]),
                    EstadoId = 1
                };

                var CantidadAdjuntos = HttpContext.Current.Request.Params["solicitud.CantidadAdjuntos"];

                for (int i = 0; i < Convert.ToInt16(CantidadAdjuntos); i++)
                {

                    SoporteCesantia Adjunto = new SoporteCesantia();
                    string nombreadjunto = "Adjunto" + i;
                    var httpPostedFile = HttpContext.Current.Request.Files[nombreadjunto];
                    var TipoAdjunto = HttpContext.Current.Request.Params["TipoAdjunto" + i];

                    var lastDotIndex = httpPostedFile.FileName.LastIndexOf('.');

                    if (lastDotIndex != -1 && lastDotIndex < httpPostedFile.FileName.Length - 1)
                    {
                        var extension = httpPostedFile.FileName.Substring(lastDotIndex + 1).ToLower();

                        if (extension != "jpg" && extension != "jpeg" && extension != "png" && extension != "doc" && extension != "docx" && extension != "pdf")
                        {
                            respuesta = "El tipo de archivo " + extension + " no es permitido.";
                            return Json(respuesta);
                        }
                    }
                    else
                    {
                        // Manejo del caso en que el archivo no tiene extensión
                        respuesta = "El archivo no tiene una extensión válida.";
                        return Json(respuesta);
                    }

                    var size = httpPostedFile.ContentLength / (1024 * 1024); //MB
                    var pesoMaximoStr = _context.Configuraciones.First(s => s.Parametro == "LIMITEPESOARCHIVO").Valor.ToString();
                    int pesoMaximo = 0;
                    Int32.TryParse(pesoMaximoStr, out pesoMaximo);

                    if (size > pesoMaximo)
                    {
                        respuesta = "El archivo supera el tamaño permitido de carga.";
                        return Json(respuesta);

                    }


                    // Validate the uploaded image(optional)
                    DateTime date1 = DateTime.Now;
                    var nombrearchivo = date1.ToString("yyyyMMddHHmmssffff" + i).ToString() + "-" + httpPostedFile.FileName;
                    // Get the complete file path
                    var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/AnexosCesantias"), nombrearchivo);

                    //// Save the uploaded file to "UploadedFiles" folder
                    httpPostedFile.SaveAs(fileSavePath);
                    Adjunto.SolicitudCesantiaId = solicitud.Id;
                    Adjunto.NombreSoporte = nombrearchivo;
                    Adjuntos.Add(Adjunto);
                    _context.SoporteCesantia.Add(Adjunto);


                }

               //  solicitud.Soportes = Adjuntos;
                _context.SolicitudCesantia.Add(solicitud);
                _context.SaveChanges();

                return Ok(new { Message = "Solicitud registrada con éxito", SolicitudId = solicitud.Id });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al registrar la solicitud: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("api/cesantias/obtener-solicitud/{solicitudId}")]
        public IHttpActionResult ObtenerSolicitud(int solicitudId)
        {
            try
            {
                var solicitud = _repository.ObtenerSolicitudConSoportes(solicitudId);
                if (solicitud == null)
                {
                    return NotFound();
                }

                return Ok(solicitud);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al obtener la solicitud: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("api/cesantias/obtener-solicitudes-por-empleado/{empleadoId}")]
        public IHttpActionResult ObtenerSolicitudesPorEmpleado(int empleadoId)
        {
            try
            {
                var solicitudes = _repository.ObtenerSolicitudesPorEmpleado(empleadoId);
                return Ok(solicitudes);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al obtener las solicitudes: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("api/cesantias/listar-destinos")]
        public IHttpActionResult ListarDestinos()
        {
            try
            {
                var destinos = _repository.ListarDestinos();
                return Ok(destinos);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al listar los destinos: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("api/cesantias/listar-soportes-por-destino/{destinoId}")]
        public IHttpActionResult ListarSoportesPorDestino(int destinoId)
        {
            try
            {
                var soportes = _repository.ListarSoportesPorDestino(destinoId);
                return Ok(soportes);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al listar los soportes: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("api/cesantias/GenerarCarta/{solicitudid}")]
        public async Task<IHttpActionResult> GenerarCarta(int solicitudid)
        {
            try
            {
                var carta = await _repository.GenerarCartaPdfBase64(solicitudid);
                return Ok(carta);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al listar los soportes: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("api/cesantias/listar-fondos")]
        public IHttpActionResult listarfondos()
        {
            try
            {
                var destinos = _repository.ListarFondos();
                return Ok(destinos);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al listar los destinos: {ex.Message}");
            }
        }
    }
}
