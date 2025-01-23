using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Services
{
    public class PerfilRepository
    {



        public List<Perfil> ObtenerTodos()
        {
            using (var db = new AutogestionContext())
            {
               
                    List<Perfil> Items = db.Perfil.ToList();
                    foreach (Perfil Item in Items)
                    {
                        
                        Item.GrupoEmpleados = db.GrupoEmpleados.FirstOrDefault(x => x.Id == Item.GrupoId);


                    }
                    return Items;
                
                
            }
        }


        internal void Crear(Perfil model)
        {

            using (var db = new AutogestionContext())
            {
                try
                {
                    db.Perfil.Add(model);
                    db.SaveChanges();
                }
                catch
                {
                }
            }

        }


        internal void Editar(Perfil model)
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


        internal void Eliminar(int id)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    Perfil model = db.Perfil.Find(id);
                    db.Perfil.Remove(model);
                    db.SaveChanges();


                }
                catch
                { }
            }
        }




    }
}