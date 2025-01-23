using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Services
{
    public class EncuestaRepository
    {

        public List<Encuesta> ObtenerTodos()
        {
            using (var db = new AutogestionContext())
            {
                return db.Encuesta.ToList();
            }
        }

        internal void Crear(Encuesta model)
        {

            using (var db = new AutogestionContext())
            {
                try
                {
                    db.Encuesta.Add(model);
                    db.SaveChanges();
                }
                catch
                {
                }
            }

        }

        internal void Editar(Encuesta model)
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