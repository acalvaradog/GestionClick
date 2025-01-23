using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Services
{
    public class EncabezadoEncuestaRepository
    {

        public List<EncabezadoEncuesta> ObtenerTodos()
        {
            using (var db = new AutogestionContext())
            {

                List<EncabezadoEncuesta> Items = db.EncabezadoEncuesta.ToList();
                foreach (EncabezadoEncuesta Item in Items)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);


                }

                return Items;

            }
        }


        internal void Crear(SeguimientoSintomas model, string Id, string EmpleadoId)
        {

            using (var db = new AutogestionContext())
            {
                try
                {
                    SeguimientoSintomas sintomas = new SeguimientoSintomas();
                    int id = 0;
                    int empleado = 0;
                    Int32.TryParse(Id, out id);
                    Int32.TryParse(EmpleadoId, out empleado);

                    sintomas.Plan = model.Plan;
                    sintomas.Observacion = model.Observacion;
                    sintomas.Fecha = model.Fecha;
                    sintomas.EmpleadoId = empleado;
                    sintomas.EncabezadoEncuestaId = id;


                    db.SeguimientoSintomas.Add(sintomas);
                    db.SaveChanges();
                }
                catch
                {
                }
            }

        }

        internal void Editar(SeguimientoSintomas model, string EncabezadoId, string EmpleadoId, string Idsintomas)
        {

            using (var db = new AutogestionContext())
            {
                try
                {
                    SeguimientoSintomas sintomas = new SeguimientoSintomas();
                    int id = 0;
                    int Encabezadoid = 0;
                    int empleado = 0;
                    Int32.TryParse(Idsintomas, out id);
                    Int32.TryParse(EncabezadoId, out Encabezadoid);
                    Int32.TryParse(EmpleadoId, out empleado);

                    sintomas.Id = id; 
                    sintomas.Plan = model.Plan;
                    sintomas.Observacion = model.Observacion;
                    sintomas.Fecha = model.Fecha;
                    sintomas.EmpleadoId = empleado;
                    sintomas.EncabezadoEncuestaId = Encabezadoid;

                    db.Entry(sintomas).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch
                {
                }
            }

        }

       






    }
}