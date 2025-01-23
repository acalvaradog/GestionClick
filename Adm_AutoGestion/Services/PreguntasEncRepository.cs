using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Services
{
    public class PreguntasEncRepository
    {

        public List<Preguntas> ObtenerTodos()
        {
            using (var db = new AutogestionContext())
            {


                return db.Preguntas.ToList();
                //return preguntas();

            }
        }

        public List<Preguntas> ObtenerTodos2(string grupo)
        {
            using (var db = new AutogestionContext())
            {


                return db.Preguntas.Where(e => e.Grupo == grupo).ToList();
                //return preguntas();

            }
        }


        public string ObtenerDatosVacunas(string Cod)
        {
            using (var db = new AutogestionContext())
            {

                Empleado emp = db.Empleados.FirstOrDefault(e => e.NroEmpleado == Cod);

                string res = emp.VacunaDosis1 + "," + emp.DosisRefuerzo;

                return res;
                //return preguntas();

            }
        }

        internal void Crear(Preguntas model)
        {

            using (var db = new AutogestionContext())
            {
                try
                {
                    db.Preguntas.Add(model);
                    db.SaveChanges();
                }
                catch
                {
                }
            }

        }

        internal void Editar(Preguntas model)
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