using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Services
{
    public class GrupoEmpleadosRepository
    {
        public List<GrupoEmpleados> ObtenerTodos()
        {
            using (var db = new AutogestionContext())
            {


                return db.GrupoEmpleados.ToList();
             

            }
        }


        internal void Crear(GrupoEmpleados model)
        {

            using (var db = new AutogestionContext())
            {
                try
                {
                    db.GrupoEmpleados.Add(model);
                    db.SaveChanges();
                }
                catch
                {
                }
            }

        }


        internal void Editar(GrupoEmpleados model)
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



    }
}