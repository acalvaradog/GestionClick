using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Services
{
    public class TiposIncapacidadRepository
    {

        public List<TiposIncapacidad> ObtenerTodos()
        {
            using (var db = new AutogestionContext())
            {


                List<TiposIncapacidad> Items = db.TiposIncapacidad.ToList();
                foreach (TiposIncapacidad Item in Items)
                {
                    Item.EstadosIncapacidades= db.EstadosIncapacidades.FirstOrDefault(x => x.Id == Item.EstadoId);
                   

                }
                return Items;
            }
        }

        internal void Crear(TiposIncapacidad model)
        {

            using (var db = new AutogestionContext())
            {
                try
                {
                    db.TiposIncapacidad.Add(model);
                    db.SaveChanges();
                }
                catch
                {
                }
            }

        }


        internal void Editar(TiposIncapacidad model)
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