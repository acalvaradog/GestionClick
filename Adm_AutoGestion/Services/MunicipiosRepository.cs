using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Services
{
    public class MunicipiosRepository
    {
        private AutogestionContext db = new AutogestionContext();
        public List<Municipio> Obtenertodo() 
        {
            List<Municipio> List = new List<Municipio>();
            try 
            {
             List = db.Municipio.Where(x => x.Estado == "Activo").OrderBy(x => x.Nombre).ToList();
                foreach (var item in List) 
                {
                 
                    item.Departamento = db.Departamento.Where(x => x.Id == item.DepartamentoId).FirstOrDefault();
                }
            }
            catch(Exception ex) 
            {
            
            }
            return (List);
        }

        public string Crear(Municipio model)
        {
            string respuesta = "";
            try
            {
                db.Municipio.Add(model);
                db.SaveChanges();
                respuesta = "Guardado";
            }
            catch (Exception ex)
            {
                respuesta = ex.Message;
            }

            return respuesta;
        }
        public string Edit(Municipio Model)
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
                Respuesta = ex.Message;
            }
            return Respuesta;
        }
    }
}