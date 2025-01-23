using Adm_AutoGestion.Models;
using Adm_AutoGestion.Models.EvaDesempeno;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Windows.Documents;

namespace Adm_AutoGestion.Services
{
    public class EstructuraJerarquicaRepository
    {


        public List<EstructuraJerarquica> ObtenerTodos()
        {
            using (var db = new AutogestionContext())
            {

                List<EstructuraJerarquica> Items = db.EstructuraJerarquica.ToList();
                foreach (EstructuraJerarquica Item in Items)
                {
                    Item.NJefe = db.Empleados.FirstOrDefault(e => e.NroEmpleado == Item.Jefe);
                    Item.NSuperior = db.Empleados.FirstOrDefault(e => e.NroEmpleado == Item.Superior);
                    Item.NDirector = db.Empleados.FirstOrDefault(e => e.NroEmpleado == Item.Director);
                    Item.NLider = db.Empleados.FirstOrDefault(e => e.NroEmpleado == Item.Lider);
                }
                return Items;
            }
        }

        internal void Crear(EstructuraJerarquica model)
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
                    db.EstructuraJerarquica.Add(model);
                    db.SaveChanges();

                    List<Empleado> Items = db.Empleados.Where(e => e.AreaDescripcion == model.Area && e.Empresa == model.Sociedad).ToList();
                    if (Items != null)
                    {
                        foreach (Empleado Item in Items)
                        {
                            db.Empleados.Attach(Item);
                            if (Item.NroEmpleado == model.Jefe)
                            {
                                Item.Jefe = model.Superior;
                                Item.Superior = model.Superior;
                                Item.Director = model.Director;
                                Item.A_Modificacion = model.A_Modificacion;
                                Item.A_UsuarioModifica = model.A_UsuarioModifica;
                                if (Item.NroEmpleado != model.Lider)
                                {
                                    Item.Lider = model.Lider;
                                }
                                else
                                {
                                    Item.Lider = "";
                                }
                            }
                            else
                            {
                                Item.Jefe = model.Jefe;
                                Item.Superior = model.Superior;
                                Item.Director = model.Director;
                                Item.A_Modificacion = model.A_Modificacion;
                                Item.A_UsuarioModifica = model.A_UsuarioModifica;
                                if (Item.NroEmpleado != model.Lider)
                                {
                                    Item.Lider = model.Lider;
                                }
                                else
                                {
                                    Item.Lider = "";
                                }
                            }
                            db.SaveChanges();
                        }
                    }

                    List<PersonalActivo> Items2 = db.PersonalActivo.Where(e => e.Area == model.Area && e.Empresa == model.Sociedad).ToList();
                    if (Items2 != null)
                    {
                        foreach (PersonalActivo Item2 in Items2)
                        {
                            db.PersonalActivo.Attach(Item2);
                            if (Item2.CodigoEmpleado == model.Jefe)
                            {
                                Item2.Jefe = model.Superior;
                                Item2.Superior = model.Superior;
                                Item2.Director = model.Director;
                                Item2.UnidadOrganizativa = "1234";
                                if (Item2.CodigoEmpleado != model.Lider)
                                {
                                    Item2.Lider = model.Lider;
                                }
                                else 
                                {
                                    Item2.Lider = "";
                                }
                            }
                            else
                            {
                                Item2.Jefe = model.Jefe;
                                Item2.Superior = model.Superior;
                                Item2.Director = model.Director;
                                Item2.UnidadOrganizativa = "1234";
                                if (Item2.CodigoEmpleado != model.Lider)
                                {
                                    Item2.Lider = model.Lider;
                                }
                                else
                                {
                                    Item2.Lider = "";
                                }
                            }
                            db.SaveChanges();
                        }
                    }



                }
                catch
                {
                }
            }

        }


        internal void Editar(int id, EstructuraJerarquica model)
        {
            using (var db = new AutogestionContext())
            {

                EstructuraJerarquica EJ = db.EstructuraJerarquica.Find(id);
                Empleado emple = new Empleado();
                PersonalActivo PA = new PersonalActivo();

                try
                {

                    db.EstructuraJerarquica.Attach(EJ);
                    EJ.Area = model.Area;
                    EJ.Jefe = model.Jefe;
                    EJ.Superior = model.Superior;
                    EJ.Director = model.Director;
                    EJ.Lider = model.Lider;
                    EJ.A_Modificacion = model.A_Modificacion;
                    EJ.A_UsuarioModifica = model.A_UsuarioModifica;
                    db.SaveChanges();


                    List<Empleado> Items = db.Empleados.Where(e => e.UnidadOrganizativa.Trim() == EJ.UnidadOrg.Trim() && e.Empresa == EJ.Sociedad && e.Activo=="SI").ToList();
                    if (Items != null)
                    {
                        foreach (Empleado Item in Items)
                        {
                            db.Empleados.Attach(Item);
                            if (Item.NroEmpleado == model.Jefe)
                            {
                                Item.Jefe = model.Superior;
                                Item.Superior = model.Director;
                                Item.Director = model.Director;
                                Item.A_Modificacion = model.A_Modificacion;
                                Item.A_UsuarioModifica = model.A_UsuarioModifica;
                                if (Item.NroEmpleado != model.Lider)
                                {
                                    Item.Lider = model.Lider;
                                }
                                else
                                {
                                    Item.Lider = "";
                                }
                            }
                            else
                            {
                                Item.Jefe = model.Jefe;
                                Item.Superior = model.Superior;
                                Item.Director = model.Director;
                                Item.A_Modificacion = model.A_Modificacion;
                                Item.A_UsuarioModifica = model.A_UsuarioModifica;
                                if (Item.NroEmpleado != model.Lider)
                                {
                                    Item.Lider = model.Lider;
                                }
                                else
                                {
                                    Item.Lider = "";
                                }
                            }
                            db.SaveChanges();
                        }
                    }


                    List<PersonalActivo> Items2 = db.PersonalActivo.Where(e => e.Area == model.Area && e.Empresa == model.Sociedad).ToList();
                    if (Items2 != null)
                    {
                        foreach (PersonalActivo Item2 in Items2)
                        {
                            db.PersonalActivo.Attach(Item2);
                            if (Item2.CodigoEmpleado == model.Jefe)
                            {
                                Item2.Jefe = model.Superior;
                                Item2.Superior = model.Director;
                                Item2.Director = model.Director;
                                Item2.UnidadOrganizativa = "1234";
                                if (Item2.CodigoEmpleado != model.Lider)
                                {
                                    Item2.Lider = model.Lider;
                                }
                                else
                                {
                                    Item2.Lider = "";
                                }
                            }
                            else
                            {
                                Item2.Jefe = model.Jefe;
                                Item2.Superior = model.Superior;
                                Item2.Director = model.Director;
                                Item2.UnidadOrganizativa = "1234";
                                if (Item2.CodigoEmpleado != model.Lider)
                                {
                                    Item2.Lider = model.Lider;
                                }
                                else
                                {
                                    Item2.Lider = "";
                                }
                            }
                            db.SaveChanges();
                        }
                    }



                }
                catch(Exception Ex)
                {
                }
            }
        }


        internal string Delete(int id) 
        {
            string Respuesta="";
            using (var db = new AutogestionContext()) 
            {
                try 
                {
                    EstructuraJerarquica estr = db.EstructuraJerarquica.Where(x => x.Id == id).FirstOrDefault();
                    db.EstructuraJerarquica.Remove(estr);
                    db.SaveChanges ();
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