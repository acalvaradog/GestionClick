using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Services
{
    public class ConfiguracionesRepository
    {

        public List<Configuraciones> ObtenerTodos()
        {
            using (var db = new AutogestionContext())
            {


                return db.Configuraciones.ToList();
                //return preguntas();

            }
        }

        internal void Crear(Configuraciones model)
        {

            using (var db = new AutogestionContext())
            {
                try
                {
                    db.Configuraciones.Add(model);
                    db.SaveChanges();
                }
                catch
                {
                }
            }

        }


        internal void Editar(Configuraciones model)
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


        internal void Modificar(string Valor)
        {

            using (var db = new AutogestionContext())
            {
                try
                {

                    Configuraciones Configura = new Configuraciones();

                    Configura = db.Configuraciones.FirstOrDefault(e => e.Id == 1);
                    db.Configuraciones.Attach(Configura);
                    Configura.Valor = Valor;
                    db.SaveChanges();


                   
                }
                catch
                {
                }
            }

        }



    }
}