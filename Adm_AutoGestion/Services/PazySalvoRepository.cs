using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Services
{
    public class PazySalvoRepository
    {

        public List<PazySalvo> ObtenerTodos(string area )
        {
            List<PazySalvo> items = new List<PazySalvo>();
            using (var db = new AutogestionContext())
            {

                //AreasPazySalvo area = db.AreasPazySalvo.FirstOrDefault(e => e.Responsable == usuario);
                var detalles = (from p in db.PazySalvo
                                join d in db.DetallePazySalvo on p.Id equals d.IdPazySalvo
                                join e in db.Empleados on p.EmpleadoId equals e.Id
                                where d.Area == area && d.Firma == null && p.Estado != "Anulado"
                                select new {p.Id, p.Fecha, p.EmpleadoId, p.Estado }).ToList();

                foreach(var item in detalles)
                {
                    items.Add(new PazySalvo() { Id = item.Id, Fecha = item.Fecha, EmpleadoId = item.EmpleadoId, Estado = item.Estado });
                }

                foreach (PazySalvo Item in items)
                {
                   Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                }

                return items;
            }
        }

        public List<AreasPazySalvo> ObtenerAreas()
        {
            using (var db = new AutogestionContext())
            {


                List<AreasPazySalvo> Items = db.AreasPazySalvo.ToList();
                foreach (AreasPazySalvo Item in Items)
                {
                    int id = 0;
                    Int32.TryParse(Item.Responsable, out id);
                    Item.empleado = db.Empleados.FirstOrDefault(e => e.Id == id);


                }
                return Items;
            }
        }

        public string CrearAreasResp(AreasPazySalvo model)
        {
            string respuesta = "";
            using (var db = new AutogestionContext())
            {
                try
                {
                    db.AreasPazySalvo.Add(model);
                    db.SaveChanges();
                    respuesta = "OK";
                }
                catch
                {
                    respuesta = "ERROR";
                }
            }

            return respuesta;
        }

        public string FirmarPazySalvo(string usuario, string password, int Id, string area, string Obs, string fuente)
        {
            string message = "";
            int cont = 0;

            
            using (var db = new AutogestionContext())
            {
                var empleado = db.Empleados.FirstOrDefault(e => e.Documento == usuario);


                if (empleado.Documento == usuario && empleado.Contraseña == password)
                {

                    DetallePazySalvo detalle = db.DetallePazySalvo.FirstOrDefault(e => e.IdPazySalvo == Id && e.Area == area);
                    db.DetallePazySalvo.Attach(detalle);
                    detalle.FechaFirma = DateTime.Now;
                    detalle.Responsable = empleado.Id;
                    detalle.Firma = DateTime.Now + "|" + empleado.Id + "|" + empleado.Nombres + "|" + fuente;
                    detalle.Observacion = Obs;
                    db.SaveChanges();
                    message = "Se firmo correctamente el Paz y Salvo.";

                    List<DetallePazySalvo> detalles = db.DetallePazySalvo.Where(e => e.IdPazySalvo == Id).ToList();
                    foreach (DetallePazySalvo Item in detalles)
                    {
                        if (Item.Firma == null)
                        {
                           cont = cont + 1;
                        }
                    }

                    if (cont == 0)
                    {
                        PazySalvo encabezado = db.PazySalvo.Find(Id);
                        db.PazySalvo.Attach(encabezado);
                        encabezado.Estado = "Cerrado";
                        db.SaveChanges();
                    }
                    

                }
                else {

                    message = "Los datos ingresados son incorrectos.";
                }

            }


            return message;
        }

        internal void modificar(int id, string opcion)
        {
            using (var db = new AutogestionContext())
            {
                try
                {

                    PazySalvo pys = new PazySalvo();
                    pys = db.PazySalvo.FirstOrDefault(s => s.Id == id);
                    db.PazySalvo.Attach(pys);
                    pys.Estado = "Anulado";
                    db.SaveChanges();

                }
                catch
                {
                }
            }
        }


        internal void Eliminar(int id)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    AreasPazySalvo model = db.AreasPazySalvo.Find(id);
                    db.AreasPazySalvo.Remove(model);
                    db.SaveChanges();


                }
                catch
                { }
            }
        }



    }
}