using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Services
{
    public class DescuentosRepository
    {

        public List<Descuentos> ObtenerTodos()
        {
           
            using (var db = new AutogestionContext())
            {

                List<Descuentos> Items = db.Descuentos.ToList();
                foreach (Descuentos Item in Items)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.Servicios = db.Servicios.FirstOrDefault(e => e.Id == Item.ServicioId);


                }
                return Items;
            }
        }


        public string Crear(Descuentos model)
        {
            string message = "";
            using (var db = new AutogestionContext())
            {
                var empleado = db.Empleados.Find(model.EmpleadoId);
                try
                {
                    model.Fecha = DateTime.Now;
                    model.Activo = "SI";
                    model.Empresa = empleado.Empresa;
                    db.Descuentos.Add(model);
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


        internal void Editar(Descuentos model)
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