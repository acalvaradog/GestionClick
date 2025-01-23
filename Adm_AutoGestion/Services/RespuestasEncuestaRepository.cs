using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Services
{
    public class RespuestasEncuestaRepository
    {

        public List<Encuesta> ObtenerTodos(int Id)
        {
            using (var db = new AutogestionContext())
            {

                List<Encuesta> Items = db.Encuesta.Where(e => e.EncabezadoEncuesta.Id == Id).ToList();
                foreach (Encuesta Item in Items) {                    
                    Item.ListadoPreguntas = db.Preguntas.Where(e => e.Id == Item.NumeroPregunta).ToList();
                }
                
                return Items;

            }
        }


    }
}