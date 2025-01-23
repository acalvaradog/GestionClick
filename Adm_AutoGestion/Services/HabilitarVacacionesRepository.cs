using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Services
{
    public class HabilitarVacacionesRepository
    {

        public List<HabilitarVacaciones> ObtenerTodos(string EmpleadoId)
        {
            using (var db = new AutogestionContext())
            {
                int id = -1;
                Int32.TryParse(EmpleadoId, out id);
                Empleado datos = new Empleado();

                datos = db.Empleados.Find(id);

                //List<HabilitarVacaciones> Items = db.HabilitarVacaciones.Where(s => s.Empleado.Empresa == datos.Empresa).ToList();
                List<HabilitarVacaciones> Items = db.HabilitarVacaciones.ToList();
                foreach (HabilitarVacaciones Item in Items)
                {
                    int emp = 0;
                    Int32.TryParse(Item.UsuarioRegistra, out emp);
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.Empleado2 = db.Empleados.FirstOrDefault(e => e.Id == emp);


                }
                return Items;
            }
        }


        internal void Crear(HabilitarVacaciones model)
        {

            using (var db = new AutogestionContext())
            {
                try
                {
                    db.HabilitarVacaciones.Add(model);
                    db.SaveChanges();
                }
                catch
                {
                }
            }

        }

        internal void Editar(HabilitarVacaciones model)
        {
            using (var db = new AutogestionContext())
            {
                try
                {

                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();


                }
                catch
                { }
            }
        }


        internal void Eliminar(int id)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    HabilitarVacaciones model = db.HabilitarVacaciones.Find(id);
                    db.HabilitarVacaciones.Remove(model);
                    db.SaveChanges();


                }
                catch
                { }
            }
        }





    }
}