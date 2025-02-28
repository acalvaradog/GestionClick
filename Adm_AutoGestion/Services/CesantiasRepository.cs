using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Web.Hosting;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text;
namespace Adm_AutoGestion.Services
{
    public class CesantiasRepository
    {
        private readonly AutogestionContext _context;

        public CesantiasRepository(AutogestionContext context)
        {
            _context = context;
        }

        public int RegistrarSolicitud(SolicitudCesantia solicitud, List<SoporteCesantia> soportes)
        {
            _context.SolicitudCesantia.Add(solicitud);
            _context.SaveChanges();

            if (soportes != null && soportes.Any())
            {
                foreach (var soporte in soportes)
                {
                    soporte.SolicitudCesantiaId = solicitud.Id;
                    _context.SoporteCesantia.Add(soporte);
                }
                _context.SaveChanges();
            }

            return solicitud.Id;
        }

        public SolicitudCesantia ObtenerSolicitudConSoportes(int solicitudId)
        {
            return _context.SolicitudCesantia
                .Include("Soportes")
                .FirstOrDefault(s => s.Id == solicitudId);
        }

        public List<SolicitudCesantia> ObtenerSolicitudesPorEmpleado(int empleadoId)
        {
            return _context.SolicitudCesantia
                .Include("Soportes")
                .Include("Destino")
                .Include("Estado")
                  .Include(x => x.FondoCesantias)
                .Where(s => s.EmpleadoId == empleadoId)
                .ToList();
        }

        public List<DestinoCesantia> ListarDestinos()
        {
            return _context.DestinoCesantia.ToList();
        }

        public List<FondoCesantias> ListarFondos()
        {
            return _context.FondoCesantias.ToList();
        }

        public List<SoporteDestino> ListarSoportesPorDestino(int destinoId)
        {
            return _context.SoporteDestino
                .Where(rs => rs.DestinoId == destinoId)
                .ToList();
        }

        public async Task<List<SolicitudCesantia>> ObtenerSolicitudesAsync(int? empleadoId, DateTime? fechaInicio, DateTime? fechaFin, int? estado,string empresa)
        {
          
            var query = _context.SolicitudCesantia
                .Include(s => s.Destino)
                .Include(s => s.Estado)
                .Include(s => s.Empleado)
                  .Include(x => x.FondoCesantias)
                .Where(x=> x.Empleado.Empresa == empresa)
                .AsQueryable();

            if (estado.HasValue)
                query = query.Where(s => s.EstadoId == estado);

            if (empleadoId.HasValue)
                query = query.Where(s => s.EmpleadoId == empleadoId.Value);

            if (fechaInicio.HasValue)
                query = query.Where(s => s.FechaRegistro.Date >= fechaInicio.Value.Date);

            if (fechaFin.HasValue)
                query = query.Where(s => s.FechaRegistro.Date <= fechaFin.Value.Date);

            return await query.ToListAsync();
        }

        public async Task<SolicitudCesantia> ObtenerSolicitudConDetallesAsync(int id)
        {
            return await _context.SolicitudCesantia
                .Include(s => s.Soportes)
                .Include(s => s.Estado)
                .Include(s => s.Destino)
                .Include(x=> x.Log)
                 .Include(x => x.Empleado)
                 .Include(x=> x.FondoCesantias)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<LogSolicitudCesantia>> ObtenerLogsPorSolicitudAsync(int solicitudId)
        {
            return await _context.LogSolicitudCesantia
                .Where(log => log.SolicitudCesantiaId == solicitudId)
                .OrderByDescending(log => log.FechaHora)
                .ToListAsync();
        }

        public async Task ActualizarEstadoSolicitudAsync(int solicitudId, int nuevoEstadoId, string usuario,string Observacion)
        {
            var solicitud = await _context.SolicitudCesantia.FindAsync(solicitudId);
            if (solicitud != null)
            {
                solicitud.EstadoId = nuevoEstadoId;
                if (Observacion != null) solicitud.Observacion = Observacion;

                // Crear un log de la acción
                var log = new LogSolicitudCesantia
                {
                    SolicitudCesantiaId = solicitudId,
                    Usuario = usuario,
                    Accion = nuevoEstadoId == 2 ? "Aprobado" : nuevoEstadoId == 3 ? "Rechazado" : "Pagado",
                    FechaHora = DateTime.Now
                };

                _context.LogSolicitudCesantia.Add(log);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<string> GenerarCartaPdfBase64(int solicitudId)
        {
            // Obtener datos de la base de datos
            var solicitud = await ObtenerSolicitudConDetallesAsync(solicitudId);

            ;

            //// Generar HTML
            //var html = GenerarHtml(solicitud);

            //// Convertir HTML a PDF
           //var pdfBytes = await ConvertirHtmlAPdf(html);

            // Convertir PDF a base64
            return Convert.ToBase64String(GenerarCartaPdf(solicitud));
        }

        public byte[] GenerarPdfConItextSharp(string html)
        {
            using (var memoryStream = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 20f, 20f, 20f, 20f);
                var writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Rutas de las imágenes
                string logoPath = HttpContext.Current.Server.MapPath("~/Contents/image/Logo.png");
                string watermarkPath = HttpContext.Current.Server.MapPath("~/Contents/image/logo.png");

                //// ➤ Agregar logo en la parte superior izquierda (antes del contenido HTML)
                //if (File.Exists(logoPath))
                //{
                //    var logo = Image.GetInstance(new Uri(logoPath));
                //    logo.ScaleToFit(120f, 120f); // Ajustar tamaño

                //    // Posicionar en la esquina superior izquierda
                //    float logoX = 20f; // Margen izquierdo
                //    float logoY = document.PageSize.Height - 120f; // Altura total - tamaño del logo
                //    logo.SetAbsolutePosition(logoX, logoY);
                //    writer.DirectContent.AddImage(logo);
                //}

                // ➤ Agregar marca de agua (centrada en la página)
                if (File.Exists(watermarkPath))
                {
                    var watermark = Image.GetInstance(new Uri(watermarkPath));
                    watermark.ScaleToFit(400f, 400f); // Tamaño de la marca de agua

                    // Centrar en la página
                    float watermarkX = (document.PageSize.Width - watermark.ScaledWidth) / 2;
                    float watermarkY = (document.PageSize.Height - watermark.ScaledHeight) / 2;
                    watermark.SetAbsolutePosition(watermarkX, watermarkY);
                    watermark.Alignment = Image.UNDERLYING;
                    writer.DirectContentUnder.AddImage(watermark);
                }

                // ➤ Renderizar el HTML (sin imagen del logo)
                using (var stringReader = new StringReader(html))
                {
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, stringReader);
                }

                document.Close();
                return memoryStream.ToArray();
            }
        }



        public byte[] GenerarCartaPdf(SolicitudCesantia solicitud)
        {
            string html = GenerarHtml(solicitud);
            return GenerarPdfConItextSharp(html);
        }

        public string GenerarHtml(SolicitudCesantia solicitud)
        {
            string rutaRelativa = "~/PlantillasPDF/Cesantias.html";
            string rutaPlantilla = HttpContext.Current.Server.MapPath(rutaRelativa);


            string htmlTemplate = File.ReadAllText(rutaPlantilla);

            var datos = new Dictionary<string, string>

           
            {

           


        { "{{Fecha}}", DateTime.Now.ToString("dd/MM/yyyy") },
        { "{{FondoCesantias}}", solicitud.FondoCesantias.Name },
        { "{{NombreTrabajador}}", solicitud.Empleado.Nombres},
        { "{{Identificacion}}", solicitud.Empleado.Documento},
        { "{{ValorRetiro}}", solicitud.ValorRetiro.ToString("C",new System.Globalization.CultureInfo("es-CO")) },
        { "{{ConceptoRetiro}}",  solicitud.Destino.Nombre },
        { "{{RepresentanteLegal}}", "JUAN CARLOS MANTILLA SUAREZ" },
        { "{{CCRepresentante}}", "13.827.980" },
        { "{{LugarExpedicion}}", "Bucaramanga" },
        { "{{Telefono}}", "6059355" },
        { "{{Extension}}", "1538" }
    };

            foreach (var dato in datos)
            {
                htmlTemplate = htmlTemplate.Replace(dato.Key, dato.Value);
            }

            return htmlTemplate;
        }

   
        public async Task ActualizarSolicitudAsync(SolicitudCesantia solicitud)
        {
            // Obtener la solicitud existente de la base de datos
            var solicitudExistente = await _context.SolicitudCesantia
                .FirstOrDefaultAsync(s => s.Id == solicitud.Id);

            if (solicitudExistente == null)
            {
                throw new Exception("Solicitud no encontrada.");
            }

            // Actualizar los campos necesarios
            solicitudExistente.CartaFondo = solicitud.CartaFondo; // Actualizar el campo CartaFondo
            solicitudExistente.EmpleadoId = solicitud.EmpleadoId;
            solicitudExistente.FechaRegistro = solicitud.FechaRegistro;
            solicitudExistente.ValorRetiro = solicitud.ValorRetiro;
            solicitudExistente.DestinoId = solicitud.DestinoId;
            solicitudExistente.EstadoId = solicitud.EstadoId;
            solicitudExistente.FondoCesantiasId = solicitud.FondoCesantiasId;

            // Marcar la entidad como modificada
            _context.Entry(solicitudExistente).State = EntityState.Modified;

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();
        }
    }


}