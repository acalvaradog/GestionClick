using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Adm_AutoGestion.Models;
namespace Adm_AutoGestion.Services
{
    public class PreguntasRepository
    {

        public List<PreguntaFrecuente> ObtenerTodos()
        {
            using (var db = new AutogestionContext())
            {

                var preguntas = db.PreguntasFrecuentes.Join(db.TemasPreguntas, pre => pre.TemaId, tema => tema.Id, (pre, tema) => new { pre, tema }).ToList();

                return db.PreguntasFrecuentes.ToList();
                //return preguntas();

            }
        }

        public List<TemaPregunta> ObtenerTemas()
        {
            using (var db = new AutogestionContext())
            {
                var list = db.TemasPreguntas.ToList();
                return list;
            }
             
        }
        
        public List<PreguntaFrecuente> ObtenerPreguntasxTema( int TemaId)
        {
            using (var db = new AutogestionContext())
            {
            var Preguntas = db.PreguntasFrecuentes.Where(x=>x.TemaId == TemaId && x.Activo == true).ToList();
                return Preguntas;
            }
        }
            internal void Crear(PreguntaFrecuente model)
        {

            using (var db = new AutogestionContext())
            {
                   try


            {
                db.PreguntasFrecuentes.Add(model);
                db.SaveChanges();
                   }
                       catch
                   {
                   }
            }

        }


    }
}