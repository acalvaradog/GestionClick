using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Adm_AutoGestion.DTO;
namespace Adm_AutoGestion.Services
{
    public class DosimetriaRepository
    {
        private readonly AutogestionContext _context;

        public DosimetriaRepository(AutogestionContext context)
        {
            _context = context;
        }

        public IEnumerable<Dosimetria> GetAll()
        {
            return _context.Dosimetria.Include("Empleado").Include("Sede").ToList();
        }

        public Dosimetria GetById(int id)
        {
            return _context.Dosimetria.Include("Empleado").Include("Sede").FirstOrDefault(d => d.Id == id);
        }

        public bool Exists(int empleadoId, int anio, int mes)
        {
            return _context.Dosimetria.Any(d => d.EmpleadoId == empleadoId && d.Anio == anio && d.Mes == mes);
        }

        public void Add(Dosimetria dosimetria)
        {
            _context.Dosimetria.Add(dosimetria);
            _context.SaveChanges();
        }

        public void Update(Dosimetria dosimetria)
        {
            var dosimetriaInDb = _context.Dosimetria.SingleOrDefault(d => d.Id == dosimetria.Id);
            if (dosimetriaInDb != null)
            {
                dosimetriaInDb.ValorHp10 = dosimetria.ValorHp10;
                dosimetriaInDb.ValorHp3 = dosimetria.ValorHp3;
                dosimetriaInDb.Anio = dosimetria.Anio;
                dosimetriaInDb.Mes = dosimetria.Mes;
                dosimetriaInDb.SedeId = dosimetria.SedeId;
                _context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var dosimetria = _context.Dosimetria.SingleOrDefault(d => d.Id == id);
            if (dosimetria != null)
            {
                _context.Dosimetria.Remove(dosimetria);
                _context.SaveChanges();
            }
        }

        public decimal GetAcumuladoAnual(int empleadoId, int anio)
        {
            return _context.Dosimetria
                .Where(d => d.EmpleadoId == empleadoId && d.Anio == anio)
                .Sum(d => d.ValorHp10.Value);
        }

        public decimal GetAcumuladoTotal(int empleadoId)
        {
            return _context.Dosimetria
                .Where(d => d.EmpleadoId == empleadoId)
                .Sum(d => d.ValorHp10.Value);
        }
        public List<ReporteDosimetriaViewModelDTO> ObtenerReporteDosimetria(int anio, int? mes, int? empleadoId, int? sedeId)
        {
            // Obtén los datos básicos de la base de datos


            var registros = _context.Dosimetria
                .Where(r => r.Anio == anio);

            if (mes.HasValue)
            {
                registros = registros.Where(r => r.Mes == mes.Value);
            }

            if (empleadoId.HasValue)
            {
                registros = registros.Where(r => r.EmpleadoId == empleadoId.Value);
            }

            if (sedeId.HasValue)
            {
                registros = registros.Where(r => r.SedeId == sedeId.Value);
            }

            var datos = registros
                .GroupBy(r => new { r.EmpleadoId, r.Empleado.Nombres, r.SedeId, r.Sede.Nombre })
                .Select(g => new
                {
                    EmpleadoId = g.Key.EmpleadoId,
                    NombreEmpleado = g.Key.Nombres,
                    SedeId = g.Key.SedeId,
                    NombreSede = g.Key.Nombre,
                    Registros = g.ToList(),
                    TotalAnualHp10 = g.Sum(r => r.ValorHp10),
                    TotalAnualHp3 = g.Sum(r => r.ValorHp3),
                    TotalAcumuladoHp10 = _context.Dosimetria
                        .Where(r => r.EmpleadoId == g.Key.EmpleadoId)
                        .Sum(r => r.ValorHp10),
                    TotalAcumuladoHp3 = _context.Dosimetria
                        .Where(r => r.EmpleadoId == g.Key.EmpleadoId)
                        .Sum(r => r.ValorHp3)
                })
                .ToList(); // Convierte los datos en memoria para el procesamiento adicional

            // Procesa en memoria el cálculo para cada mes
            var resultado = datos.Select(d => new ReporteDosimetriaViewModelDTO
            {
                EmpleadoId = d.EmpleadoId,
                NombreEmpleado = d.NombreEmpleado,
                SedeId = d.SedeId,
                NombreSede = d.NombreSede,
                ValoresPorMes = Enumerable.Range(1, 12)
                    .Select(m => new ValoresPorMesDTO
                    {
                        Mes = m,
                        ValorHp10 = d.Registros.Where(r => r.Mes == m).Sum(r => r.ValorHp10.Value),
                        ValorHp3 = d.Registros.Where(r => r.Mes == m).Sum(r => r.ValorHp3.Value)
                    }).ToList(),
                TotalAnualHp10 = d.TotalAnualHp10.Value,
                TotalAnualHp3 = d.TotalAnualHp3.Value,
                TotalAcumuladoHp10 = d.TotalAcumuladoHp10.Value,
                TotalAcumuladoHp3 = d.TotalAcumuladoHp3.Value
            }).ToList();

            return resultado;
        }

        public List<ReporteDosimetriaViewModelDTO> ObtenerReporteDosimetriaxempleado(int empleadoId)
        {
            // Obtén los datos básicos de la base de datos


            var registros = _context.Dosimetria.
                Where(r => r.EmpleadoId == empleadoId);



       

            var datos = registros
                .GroupBy(r => new { r.EmpleadoId, r.Empleado.Nombres, r.SedeId, r.Sede.Nombre, r.Anio })
                .Select(g => new
                {
                    año = g.Key.Anio,
                    EmpleadoId = g.Key.EmpleadoId,
                    NombreEmpleado = g.Key.Nombres,
                    SedeId = g.Key.SedeId,
                    NombreSede = g.Key.Nombre,
                    Registros = g.ToList(),
                    TotalAnualHp10 = g.Sum(r => r.ValorHp10),
                    TotalAnualHp3 = g.Sum(r => r.ValorHp3),
                    TotalAcumuladoHp10 = _context.Dosimetria
                        .Where(r => r.EmpleadoId == g.Key.EmpleadoId)
                        .Sum(r => r.ValorHp10),
                    TotalAcumuladoHp3 = _context.Dosimetria
                        .Where(r => r.EmpleadoId == g.Key.EmpleadoId)
                        .Sum(r => r.ValorHp3)
                })
                .ToList(); // Convierte los datos en memoria para el procesamiento adicional

            // Procesa en memoria el cálculo para cada mes
            var resultado = datos.Select(d => new ReporteDosimetriaViewModelDTO
            {
                año = d.año,
                EmpleadoId = d.EmpleadoId,
                NombreEmpleado = d.NombreEmpleado,
                SedeId = d.SedeId,
                NombreSede = d.NombreSede,
                ValoresPorMes = Enumerable.Range(1, 12)
                    .Select(m => new ValoresPorMesDTO
                    {
                        Mes = m,
                        ValorHp10 = d.Registros.Where(r => r.Mes == m).Sum(r => r.ValorHp10),
                        ValorHp3 = d.Registros.Where(r => r.Mes == m).Sum(r => r.ValorHp3)
                    }).ToList(),
                TotalAnualHp10 = d.TotalAnualHp10,
                TotalAnualHp3 = d.TotalAnualHp3,
                TotalAcumuladoHp10 = d.TotalAcumuladoHp10,
                TotalAcumuladoHp3 = d.TotalAcumuladoHp3
            }).ToList();

            return resultado;
        }

    }
}