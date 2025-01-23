using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Services
{
    public class DepartamentoRepository
    {
        private AutogestionContext db = new AutogestionContext();

        public string Crear(Departamento model) 
        {
            string respuesta = "";
            try 
            {
                //db.Departamento.Add(model);
                db.SaveChanges();
                respuesta = "Guardado";
            }
            catch (Exception ex) 
            {
                respuesta = ex.Message;
            }
        
        return respuesta;
        }
        public string Edit(Departamento Model) 
        {
            string Respuesta = "";
            try 
            {
                db.Entry(Model).State = EntityState.Modified;
                db.SaveChanges();
                Respuesta = "Guardado";
            }
            catch (Exception ex) 
            {
                Respuesta= ex.Message;
            }
        return Respuesta;
        }
    }
}