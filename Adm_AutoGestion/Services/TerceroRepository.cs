using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Adm_AutoGestion.Models;
using System.Data.Entity;
using SAP.Middleware.Connector;
using SAPExtractorDotNET;
using System.Data;
using System.Text;

namespace Adm_AutoGestion.Services
{
    public class TerceroRepository
    {
        public List<Tercero> ObtenerTodos()
        {
            using (var db = new AutogestionContext())
            {
                var Terceros = db.Tercero.ToList();


                return Terceros;
            }
        }
        internal void Crear(Tercero model, int Id) 
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    model.Activo = "SI";
                    model.FechaRegistro = DateTime.Now;
                    var emplog = db.Empleados.FirstOrDefault();
                    model.UsuarioRegistraId = emplog.Id;

                    db.Tercero.Add(model);
                    db.SaveChanges();
                   
                }
                catch(SystemException Ex)
                {
                  var  message="ERROR: "+ Ex;
                }
            }
        
        }
        internal void Editar(Tercero model, int Id)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    model.UsuarioModificaId = Id;
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();


                }
                catch
                { }
            }
        }


        //__________________________________//
    }
}