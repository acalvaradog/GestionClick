using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Services
{
    public class TurnoDisponibilidadRepository
    {

        public List<TurnoDisponibilidad> ObtenerTodos()
        {
            using (var db = new AutogestionContext())
            {


                List<TurnoDisponibilidad> Items = db.TurnoDisponibilidad.ToList();
                foreach (TurnoDisponibilidad Item in Items)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.TrabajadorId);


                }
                return Items;
                //return preguntas();

            }
        }

        public List<TurnoDisponibilidad> ObtenerTodos2()
        {
            using (var db = new AutogestionContext())
            {


                List<TurnoDisponibilidad> Items = db.TurnoDisponibilidad.Where(e => e.Estado == "Registrado").ToList();
                foreach (TurnoDisponibilidad Item in Items)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.TrabajadorId);


                }
                return Items;
                //return preguntas();

            }
        }


        public string Crear(TurnoDisponibilidad model)
        {
            string message = "";
            using (var db = new AutogestionContext())
            {
                try
                {
                    //if (model.CantExtras == null) { model.CantExtras = "0"; }
                    model.FechaRegistro = DateTime.Now;
                    model.Estado = "Registrado";
                    model.Liquidado = "";
                    db.TurnoDisponibilidad.Add(model);
                    db.SaveChanges();
                    message = "Ok";
                }
                catch
                {
                    message = "Error";
                }
            }
            return message;
        }



        internal void Modificar(string Id, string Liquidado, string Estado)
        {
            using (var db = new AutogestionContext())
            {
                try
                {

                    TurnoDisponibilidad Turno = new TurnoDisponibilidad();
                    
                    int id = 0;
                    int.TryParse(Id, out id);



                    Turno = db.TurnoDisponibilidad.FirstOrDefault(e => e.Id == id);
                    db.TurnoDisponibilidad.Attach(Turno);
                    Turno.Liquidado = Liquidado;
                    Turno.Estado = Estado;
                    db.SaveChanges();


                }
                catch
                {
                }
            } 


        }



    }
}