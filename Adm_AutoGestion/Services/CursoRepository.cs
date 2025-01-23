using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Adm_AutoGestion.Models;
using Microsoft.Ajax.Utilities;

namespace Adm_AutoGestion.Services
{
    public class CursoRepository
    {
        AutogestionContext db = new AutogestionContext();

        public string CrearCurso(string FullName, string Categoria, bool EsObligatorio, string Modalidad, DateTime StartDate, DateTime EndDate)
        {
            try
            {
                Curso curso = new Curso();

                curso.FullName = FullName;
                curso.Categoria = Categoria;
                curso.EsObligatorio = EsObligatorio;
                curso.Modalidad = Modalidad;
                curso.StartDate = StartDate;
                curso.EndDate = EndDate;
                curso.FechaCreado = DateTime.Today;

                db.Curso.Add(curso);
                db.SaveChanges();
            }
            catch
            {
                return "false";
            }

            return "true";
        }

        public string RegistrarEmpleados(string Empleados, string CursoId)
        {
            try
            {
                var lista = Empleados.Split(',');
                if (!int.TryParse(CursoId, out int cursoid))
                {
                    return "Hubo un error al encontrar el curso a asignar.";
                }

                var emp = db.Empleados.Where(x => lista.Contains(x.Nombres)).ToList();

                var cursoxEmpleadoList = new List<CursoxEmpleado>();

                foreach (var empleado in emp)
                {
                    var comprobacion = db.CursoxEmpleado.FirstOrDefault(x=> x.EmpleadoId == empleado.Id && x.CursoId == cursoid);

                    if (comprobacion != null)
                    {
                        continue;
                    }
                    else
                    {
                        var curso = new CursoxEmpleado
                        {
                            CursoId = cursoid,
                            EmpleadoId = empleado.Id,
                        };

                        cursoxEmpleadoList.Add(curso);
                    }
                }

                db.CursoxEmpleado.AddRange(cursoxEmpleadoList);
                db.SaveChanges();

                return "true";
            }
            catch (Exception ex)
            {
                return "Error: " + ex;
            }
        }

        public List<Empleado> ObtenerEmpleados(string Area, string Cargo, string Empresa, string TipoArea)
        {
            var registros = db.Empleados.Where(x=> x.Activo == "SI");

            if (!string.IsNullOrEmpty(Area))
            {
                registros = registros.Where(e => db.Empleados.Any(x=> x.Area == Area && e.Id == x.Id));
            }
            if (!string.IsNullOrEmpty(Cargo))
            {
                registros = registros.Where(e => db.Empleados.Any(x => x.Cargo == Cargo && e.Id == x.Id));
            }
            if (!string.IsNullOrEmpty(Empresa))
            {
                registros = registros.Where(e => db.Empleados.Any(x => x.Empresa == Empresa && e.Id == x.Id));
            }
            if (!string.IsNullOrEmpty(TipoArea))
            {
                registros = registros.Where(e => db.Empleados.Any(x => x.TipoArea == TipoArea && e.Id == x.Id));
            }

            return registros.ToList();
        }
    }
}