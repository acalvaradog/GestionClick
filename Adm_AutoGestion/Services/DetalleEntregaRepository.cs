using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


using Adm_AutoGestion.Services;
using System.Web.Mvc;
using System.Text;
using System.Data.Entity;


namespace Adm_AutoGestion.Services
{
    public class DetalleEntregaRepository
    {

        internal void Crear(List<DetalleEntregaEPP> model)
        {

            using (var db = new AutogestionContext())
            {
                try
                {
                    DetalleEntregaEPP detalle = new DetalleEntregaEPP();

                    foreach (DetalleEntregaEPP Item in model ) {
                         
                    
                        var empleado = db.Empleados.FirstOrDefault(x => x.Id == Item.EmpleadoId);
                   

                        detalle.NumeroEntrega = Item.NumeroEntrega;
                        detalle.EPP = Item.EPP;
                        detalle.EmpleadoId = Item.EmpleadoId;
                        detalle.Cantidad = Item.Cantidad;
                        detalle.MotivoEntrega = Item.MotivoEntrega;
                        detalle.Fecha = Item.Fecha;
                        detalle.Estado = "Activo";
                        detalle.FechaFin = Item.FechaFin;
                        detalle.FechaFirma = Item.FechaFirma;
                        detalle.Cargo = empleado.Cargo;
                        detalle.Area = empleado.Area;

                        db.DetalleEntregaEPP.Add(detalle);
                        db.SaveChanges();
                    }

                    
                }
                catch
                {
                }
            }

        }



        public List<DetalleEntregaEPP> ObtenerTodos(int id)
        {
            using (var db = new AutogestionContext())
            {
                int soc = 0;
                List<DetalleEntregaEPP> Items = db.DetalleEntregaEPP.Where(e => e.NumeroEntrega == id && e.Estado != "Anulado").ToList();
                foreach (DetalleEntregaEPP Item in Items)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.ElementosProtecionPersonal = db.ElementosProtecionPersonal.FirstOrDefault(e => e.Id == Item.EPP);
                    Item.EntregaEPP = db.EntregaEPP.FirstOrDefault(x => x.Id == Item.NumeroEntrega);
                    Item.Empleado2 = db.Empleados.FirstOrDefault(e => e.Id == Item.EntregaEPP.EmpleadoId);
                    Int32.TryParse(Item.EntregaEPP.Sociedad, out soc);
                    Item.Sociedades = db.Sociedad.FirstOrDefault(e => e.Id == soc);
                }


                Items = Items.OrderBy(e => e.Empleado.Nombres).ToList();


                return Items;
            }


        }


        internal void modificar(int id, string opcion)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    DetalleEntregaEPP entrega = new DetalleEntregaEPP();

                    entrega = db.DetalleEntregaEPP.FirstOrDefault(e => e.Id == id);
                    db.DetalleEntregaEPP.Attach(entrega);

                    //if (opcion == "cerrar")
                    //{
                    //    entrega.Estado = "Cerrado";
                    //}

                    if (opcion == "anular")
                    {
                        entrega.Estado = "Anulado";
                    }


                    db.SaveChanges();

                }
                catch
                {
                }
            }
        }


        public List<DetalleEntregaEPP> CrearCopia(List<DetalleEntregaEPP> Entrega, int id)
        {
            
            EntregaEPP entregas = new EntregaEPP();
            DetalleEntregaEPP detalle = new DetalleEntregaEPP();
            List<DetalleEntregaEPP> model = new List<DetalleEntregaEPP>();

            using (var db = new AutogestionContext())
            {

                try
                {
                    EntregaEPP acta = db.EntregaEPP.FirstOrDefault(x => x.Id == id);

                    entregas.EmpleadoId = acta.EmpleadoId;
                    entregas.Fecha = DateTime.Now;
                    entregas.Area = acta.Area;
                    entregas.Estado = "Activo";
                    entregas.Sociedad = acta.Sociedad;
                    db.EntregaEPP.Add(entregas);
                    db.SaveChanges();

                    foreach (var Item in Entrega)
                    {

                        detalle.NumeroEntrega = entregas.Id;
                        detalle.EPP = Item.EPP;
                        detalle.EmpleadoId = Item.EmpleadoId;
                        detalle.Cantidad = Item.Cantidad;
                        detalle.MotivoEntrega = Item.MotivoEntrega;
                        detalle.Fecha = Item.Fecha;
                        detalle.Estado = "Activo";
                        detalle.FechaFin = Item.FechaFin;
                        detalle.Cargo = Item.Cargo;
                        detalle.Area = Item.Area;
                        db.DetalleEntregaEPP.Add(detalle);
                        db.SaveChanges();


                    }


                }
                catch
                {
                }

                model = db.DetalleEntregaEPP.Where(s => s.NumeroEntrega == entregas.Id).ToList();
                foreach (DetalleEntregaEPP Item in model)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.ElementosProtecionPersonal = db.ElementosProtecionPersonal.FirstOrDefault(e => e.Id == Item.EPP);
                }

                
                
            }
            return model;
            
        }



        public int EditarCopia(int id, DetalleEntregaEPP model)
        {
            int res = 0;

            DetalleEntregaEPP detalle = new DetalleEntregaEPP();

            using (var db = new AutogestionContext())
            {
                detalle = db.DetalleEntregaEPP.Find(id);
                db.DetalleEntregaEPP.Attach(detalle);
                detalle.EPP = model.EPP;
                detalle.EmpleadoId = model.EmpleadoId;
                detalle.Cantidad = model.Cantidad;
                detalle.MotivoEntrega = model.MotivoEntrega;
                detalle.Fecha = model.Fecha;
                detalle.FechaFin = model.FechaFin;
                detalle.Observacion = model.Observacion;
                db.SaveChanges();
                res = detalle.NumeroEntrega;

            }
            return res;
        }

        public int Eliminar(int id)
        {
            int res = 0;
            using (var db = new AutogestionContext())
            {
                try
                {
                    DetalleEntregaEPP model = db.DetalleEntregaEPP.Find(id);
                    db.DetalleEntregaEPP.Remove(model);
                    db.SaveChanges();
                    res = model.NumeroEntrega;


                }
                catch
                { }
            }
            return res;
        }

        internal void Cambiofechas(int id, string Fecha, string FechaFin)
        {
            var fecha = DateTime.Parse(Fecha);
            var fechafin = DateTime.Parse(FechaFin);

            using (var db = new AutogestionContext())
            {

                List<DetalleEntregaEPP> detalle = new List<DetalleEntregaEPP>();

                detalle = db.DetalleEntregaEPP.Where(x => x.NumeroEntrega == id).ToList();

                foreach (var Item in detalle) 
                {

                    db.DetalleEntregaEPP.Attach(Item);
                    Item.Fecha = fecha;
                    Item.FechaFin = fechafin;
                    db.SaveChanges();

                }
            
            
            }

        
        }







    }
}