using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

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
                .Where(s => s.EmpleadoId == empleadoId)
                .ToList();
        }

        public List<DestinoCesantia> ListarDestinos()
        {
            return _context.DestinoCesantia.ToList();
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
                .Where(x=> x.Empleado.Empresa == empresa)
                .AsQueryable();

            if (estado.HasValue)
                query = query.Where(s => s.EstadoId == estado);

            if (empleadoId.HasValue)
                query = query.Where(s => s.EmpleadoId == empleadoId.Value);

            if (fechaInicio.HasValue)
                query = query.Where(s => s.FechaRegistro >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(s => s.FechaRegistro <= fechaFin.Value);

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
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<LogSolicitudCesantia>> ObtenerLogsPorSolicitudAsync(int solicitudId)
        {
            return await _context.LogSolicitudCesantia
                .Where(log => log.SolicitudCesantiaId == solicitudId)
                .OrderByDescending(log => log.FechaHora)
                .ToListAsync();
        }

        public async Task ActualizarEstadoSolicitudAsync(int solicitudId, int nuevoEstadoId, string usuario)
        {
            var solicitud = await _context.SolicitudCesantia.FindAsync(solicitudId);
            if (solicitud != null)
            {
                solicitud.EstadoId = nuevoEstadoId;

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
    }


}