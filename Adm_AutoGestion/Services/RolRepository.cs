using Adm_AutoGestion.Models;
using Adm_AutoGestion.Models.EvaDesempeno;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Services
{
    public class RolRepository
    {
        public List<Rol> ObtenerTodos()
        {
            using (var db = new AutogestionContext())
            {

                List<Rol> Items = db.Rol.ToList();
                foreach (Rol Item in Items)
                {

                    Item.Empleado  = db.Empleados.FirstOrDefault(x => x.Id == Item.EmpleadoId);
                    Item.GrupoEmpleados = db.GrupoEmpleados.FirstOrDefault(x => x.Id == Item.GrupoId);


                }
                return Items;

                
            }
        }


        internal void Crear(Rol model)
        {

            using (var db = new AutogestionContext())
            {
                try
                {
                    db.Rol.Add(model);
                    db.SaveChanges();
                }
                catch
                {
                }
            }

        }


        internal void Editar(Rol model)
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
                    Rol model = db.Rol.Find(id);
                    db.Rol.Remove(model);
                    db.SaveChanges();


                }
                catch
                { }
            }
        }


    }
}