using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Adm_AutoGestion.Models;
namespace Adm_AutoGestion.Services
{
    public class TemaPreguntaRepository
    {

        public List<TemaPregunta> ObtenerTodos()
        {
            using (var db = new AutogestionContext())
            {
                return db.TemasPreguntas.ToList();

            }
        }


        internal void Crear(TemaPregunta model)
        {

            using (var db = new AutogestionContext())
            {
                db.TemasPreguntas.Add(model);
                db.SaveChanges();

            }

        }

    }
}