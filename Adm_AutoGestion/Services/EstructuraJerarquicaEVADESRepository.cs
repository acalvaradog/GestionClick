using Adm_AutoGestion.Models;
using Adm_AutoGestion.Models.EvaDesempeno;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace Adm_AutoGestion.Services
{
    public class EstructuraJerarquicaEVADESRepository
    {
        private EvaDesempenoContext db2 = new EvaDesempenoContext();

        public List<Estructura_Jerarquica_EVADES> ObtenerTodos()
        {
            using (var db = new AutogestionContext())
            {

                List<Estructura_Jerarquica_EVADES> Items = db2.Estructura_Jerarquica_EVADES.ToList();
                foreach (Estructura_Jerarquica_EVADES Item in Items)
                {
                    Item.DJefe = db.Empleados.FirstOrDefault(e => e.NroEmpleado == Item.Jefe);
                    Item.DSuperior = db.Empleados.FirstOrDefault(e => e.NroEmpleado == Item.Superior);
                    Item.DDirector = db.Empleados.FirstOrDefault(e => e.NroEmpleado == Item.Director);

                }
                return Items;
            }
        }

        internal void Crear(Estructura_Jerarquica_EVADES model)
        {

            using (var db = new AutogestionContext())
            {

                Empleado emple = new Empleado();
                PersonalActivo PA = new PersonalActivo();

                try
                {
                    var listaAreas = db.Empleados.Where(s => s.Empresa == model.Sociedad && s.Activo == "SI" && (s.AreaDescripcion != null || s.AreaDescripcion == "") && s.UnidadOrganizativa == model.UnidadOrg).GroupBy(b => b.UnidadOrganizativa).FirstOrDefault();
                    var Area = listaAreas.FirstOrDefault();
                    model.Area = Area.AreaDescripcion;
                    db2.Estructura_Jerarquica_EVADES.Add(model);
                    db2.SaveChanges();


                }
                catch(Exception Ex)
                {
                    Console.WriteLine("Error: "+ Ex);
                }
            }

        }


        internal void Editar(int id, Estructura_Jerarquica_EVADES model)
        {
            using (var db = new AutogestionContext())
                {

                Estructura_Jerarquica_EVADES EJ = db2.Estructura_Jerarquica_EVADES.Find(id);
                Empleado emple = new Empleado();
                PersonalActivo PA = new PersonalActivo();

                try
                {

                    db2.Estructura_Jerarquica_EVADES.Attach(EJ);
                    EJ.Area = model.Area;
                    EJ.Jefe = model.Jefe;
                    EJ.Superior = model.Superior;
                    EJ.Director = model.Director;
                    db2.SaveChanges();


                }
                catch
                {
                }
            }
        }

        internal string Delete(int id)
        {
            string Respuesta = "";
            using (var db = new AutogestionContext())
            {
                try
                {
                    Estructura_Jerarquica_EVADES estr = db2.Estructura_Jerarquica_EVADES.Where(x => x.Id == id).FirstOrDefault();
                    db2.Estructura_Jerarquica_EVADES.Remove(estr);
                    db2.SaveChanges();
                    Respuesta = "Proceso Exitoso";
                }
                catch (Exception ex)
                {
                    Respuesta = "Error: " + ex;
                }

            }
            return Respuesta;
        }



    }
}