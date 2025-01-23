using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Adm_AutoGestion.Models;

namespace Adm_AutoGestion.Services
{
    public class EntregaEPPRepository
    {

        public List<EntregaEPP> ObtenerTodos(string usuario)
        {
           int cont = 0;
            using (var db = new AutogestionContext())
            {

                int Idemp = 0;
                int Soc = 0;
                Int32.TryParse(usuario, out Idemp);
                List<EntregaEPP> Items = db.EntregaEPP.Where(e => e.Estado == "Activo" && e.EmpleadoId == Idemp).ToList();
                foreach (EntregaEPP Item in Items)
                {
                    Int32.TryParse(Item.Sociedad, out Soc);
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.Sociedades = db.Sociedad.FirstOrDefault(e => e.Id == Soc);
                    cont = 0;
                    List<DetalleEntregaEPP> detalle = db.DetalleEntregaEPP.Where(e => e.NumeroEntrega == Item.Id).ToList();
                    foreach (DetalleEntregaEPP det in detalle)
                    {
                        if (det.FechaFirma == null)
                        {
                            if (det.Estado != "Anulado")
                            {
                                cont = cont + 1;
                            }
                        }
                        
                    }
                    Item.cantifirmados = string.Format("{0}", cont);
                }


                return Items;

            }
        }


        internal void Crear(EntregaEPP model, string usuario)
        {

            using (var db = new AutogestionContext())
            {
                try
                {

                    EntregaEPP entrega = new EntregaEPP();

                    int empleado = 0;
                    Int32.TryParse(usuario, out empleado);

                    entrega.Fecha = model.Fecha;
                    entrega.Area = model.Area;
                    entrega.EmpleadoId = empleado;
                    entrega.Sociedad = model.Sociedad;
                    entrega.Estado = "Activo";


                    db.EntregaEPP.Add(entrega);
                    db.SaveChanges();
                }
                catch
                {
                }
            }

        }

        internal void modificar(int id, string opcion)
        {
            using (var db = new AutogestionContext())
            {
              try
                {
                    EntregaEPP entrega = new EntregaEPP();

                    entrega = db.EntregaEPP.FirstOrDefault(e => e.Id == id);
                    db.EntregaEPP.Attach(entrega);

                    if (opcion == "cerrar")
                    {
                        entrega.Estado = "Cerrado";

                        List<DetalleEntregaEPP> Items = db.DetalleEntregaEPP.Where(x => x.NumeroEntrega == id).ToList();
                        foreach (DetalleEntregaEPP Item in Items)
                        {
                            db.DetalleEntregaEPP.Attach(Item);
                            if (Item.Estado == "Anulado")
                            {
                                Item.Estado = "Anulado";
                            }
                            else{
                                Item.Estado = "Cerrado";
                            }
                            db.SaveChanges();
                        }

                        
                    }

                    if (opcion == "anular") {
                        entrega.Estado = "Anulado";
                        List<DetalleEntregaEPP> Items = db.DetalleEntregaEPP.Where(x => x.NumeroEntrega == id).ToList();
                        foreach (DetalleEntregaEPP Item in Items)
                        {
                            db.DetalleEntregaEPP.Attach(Item);
                            Item.Estado = "Anulado";
                            db.SaveChanges();
                        }
                    }
                    
                   
                    db.SaveChanges();

                }
                catch
                {
                }
            }
        }

        public List<EntregaEPP> ObtenerCerradas()
        {
            using (var db = new AutogestionContext())
            {

                List<EntregaEPP> Items = db.EntregaEPP.Where(e => e.Estado == "Cerrado").ToList();
                foreach (EntregaEPP Item in Items)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                }

                return Items;
            }

            
        }




        

    }
}