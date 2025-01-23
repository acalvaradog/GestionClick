using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Services
{
    public class VacacionesRepository
    {

        public List<Vacaciones> ObtenerTodos()
        {
            using (var db = new AutogestionContext())
            {


                   List<Vacaciones> Items  = db.Vacaciones.ToList();
                   foreach (Vacaciones Item in Items)
                   {
                       Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                       Item.EstadoVacaciones = db.EstadoVacaciones.FirstOrDefault(x => x.Id == Item.EstadoId);

                       
                   }
                   return Items;
            }
        }

        public List<Vacaciones> ObtenerTodos2(string opcion, string EmpleadoId, string Area, string Sociedad)
        {
            using (var db = new AutogestionContext())
            {


                List<Vacaciones> Items = new List<Vacaciones>();
                Empleado datos = new Empleado();

                int id = -1;
                Int32.TryParse(EmpleadoId, out id);

                datos = db.Empleados.Find(id);
                //var lider = db.Empleados.Where(e => e.Lider == datos.NroEmpleado).ToList();
                //var jefe = db.Empleados.Where(e => e.Jefe == datos.NroEmpleado).ToList();

                if (opcion == "Aprobar")
                {


                    //if (lider.Count != 0)
                    //{
                    //    Items = db.Vacaciones.Where(e => e.EstadoId == 1 && e.Empleado.Lider == datos.NroEmpleado).ToList();
                    //}

                    //if (jefe.Count != 0)
                    //{
                    //    Items = db.Vacaciones.Where(e => e.EstadoId == 1 && e.Empleado.Jefe == datos.NroEmpleado).ToList();
                    //}


                    Items = db.Vacaciones.Where(e => e.EstadoId == 1 &&  e.Empleado.Jefe == datos.NroEmpleado && e.Empleado.AreaDescripcion == Area && e.Empleado.Empresa == Sociedad).ToList();
                foreach (Vacaciones Item in Items)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.EstadoVacaciones = db.EstadoVacaciones.FirstOrDefault(x => x.Id == Item.EstadoId);
                }

                }


                if (opcion == "Confirmar")
                {
                    Items = db.Vacaciones.Where(e => (e.EstadoId == 2 || e.EstadoId == 3 || e.EstadoId == 11 || e.EstadoId == 12) && e.Empresa == datos.Empresa ).ToList();
                    foreach (Vacaciones Item in Items)
                    {
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                        Item.EstadoVacaciones = db.EstadoVacaciones.FirstOrDefault(x => x.Id == Item.EstadoId);
                    }
                }


                if (opcion == "Gestion") {
                    Items = db.Vacaciones.Where(e => e.EstadoId == 11).ToList();
                    foreach (Vacaciones Item in Items)
                    {
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                        Item.EstadoVacaciones = db.EstadoVacaciones.FirstOrDefault(x => x.Id == Item.EstadoId);
                    }
                
                }

                return Items;

               

            }
        }

        public List<HistorialVacaciones> ObtenerTodos3(string EmpleadoId)
        {
            using (var db = new AutogestionContext())
            {
                List<HistorialVacaciones> Items2 = new List<HistorialVacaciones>();

                int id = -1;
                Int32.TryParse(EmpleadoId, out id);
                int Estado = 0;


                    Items2 = db.HistorialVacaciones.Where(e => e.VacacionesId == id).ToList();

                    foreach (HistorialVacaciones Item in Items2)
                    {
                        Int32.TryParse(Item.Accion, out Estado);
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.UsuarioModifica);
                        Item.EstadoVacaciones = db.EstadoVacaciones.FirstOrDefault(x => x.Id == Estado);
                    }

                    return Items2;

                
            }
        }

        public List<Vacaciones> ObtenerTodos4(string EmpleadoId, string Area, string Sociedad)
        {
            using (var db = new AutogestionContext())
            {


                List<Vacaciones> Items = new List<Vacaciones>();
                Empleado datos = new Empleado();

                int id = -1;
                Int32.TryParse(EmpleadoId, out id);

                datos = db.Empleados.Find(id);

                    Items = db.Vacaciones.Where(e => e.EstadoId == 1 &&  e.Empleado.Jefe == datos.NroEmpleado)
                                                                .Where(e => e.Empleado.AreaDescripcion.Contains(Area))
                                                                .Where(e => e.Empleado.Empresa.Contains(Sociedad)).ToList();

                
                foreach (Vacaciones Item in Items)
                    {
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                        Item.EstadoVacaciones = db.EstadoVacaciones.FirstOrDefault(x => x.Id == Item.EstadoId);
                    }

                return Items;



            }
        }


        public List<Vacaciones> ObtenerTodos5(string EmpleadoId, string FechaIni, string FechaFin, string Empresa) 
        {
            using (var db = new AutogestionContext())
            {

                List<Vacaciones> Items = new List<Vacaciones>();
                Empleado datos = new Empleado();

                DateTime inicio = DateTime.Parse(FechaIni);
                DateTime fin = DateTime.Parse(FechaFin);

                int id = -1;
                Int32.TryParse(EmpleadoId, out id);

                datos = db.Empleados.Find(id);

                Items = db.Vacaciones.Where(e => (e.EstadoId == 2 || e.EstadoId == 3 || e.EstadoId == 11 || e.EstadoId == 12) && e.Empresa == Empresa ) 
                                                            .Where(e => (e.FechaInicial >= inicio) && (e.FechaInicial <= fin)).ToList();

                //Items = db.Vacaciones.Where(e => (e.EstadoId == 2 || e.EstadoId == 3 || e.EstadoId == 11 || e.EstadoId == 12) )
                //                                            .Where(e => (e.FechaInicial >= inicio) && (e.FechaInicial <= fin)).ToList();

                foreach (Vacaciones Item in Items)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.EstadoVacaciones = db.EstadoVacaciones.FirstOrDefault(x => x.Id == Item.EstadoId);
                }


                return Items;
            }
        }


        public string Crear(Vacaciones model)
        {
            string mensaje = "";
            using (var db = new AutogestionContext())
            {
                try
                {
                    HistorialVacaciones Historial = new HistorialVacaciones();
                    var empleado = db.Empleados.Find(model.EmpleadoId);
                    


                    model.EstadoId = 1;
                    model.Fecha = DateTime.Now;
                    model.Empresa = empleado.Empresa;
                    model.Observacion = "NoAplica";
                   


                    db.Vacaciones.Add(model);
                    db.SaveChanges();
                    Historial.VacacionesId = model.Id;
                    Historial.Fecha = model.Fecha;
                    Historial.Accion = "1";
                    Historial.EmpleadoId = model.EmpleadoId;
                    Historial.UsuarioModifica = model.IdModifica;
                    db.HistorialVacaciones.Add(Historial);
                    db.SaveChanges();
                    mensaje = "true";
                }
                catch(Exception ex)
                {
                    mensaje = "Error, " + ex;
                }
            }
            return mensaje;
        }


        public List<Vacaciones> Buscar(int Id)
        {

            using (var db = new AutogestionContext())
            {
              
                List<Vacaciones> Items = db.Vacaciones.Where(e => e.Id == Id).ToList();
                foreach (Vacaciones Item in Items)
                {
                    Item.ListadoEmpleado = db.Empleados.Where(e => e.Id == Item.EmpleadoId).ToList();
                }
                return Items;
            }

        }


        internal void Modificar(string Observacion, string Id, string Empleado, string Estado, int IdUsuarioM, string FechaInicial, string FechaFin, string Cds, string Cdp, string ObservacionTra)
        {

            using (var db = new AutogestionContext())
            {
                try
                {

                    HistorialVacaciones Historial = new HistorialVacaciones();
                    Vacaciones Vacaciones = new Vacaciones();
                    int estado = 0;
                    int empleado = 0;
                    int id = 0;
                    DateTime Fecha2 = DateTime.Now;
                    DateTime Fechainicio = DateTime.Now;
                    DateTime Fechafinal = DateTime.Now;


                 
                    int.TryParse(Estado, out estado);
                    int.TryParse(Empleado, out empleado);
                    int.TryParse(Id, out id);
                    

                  
                    Vacaciones = db.Vacaciones.FirstOrDefault(e => e.Id == id);
                    db.Vacaciones.Attach(Vacaciones);

                    if (!DateTime.TryParse(FechaInicial, out Fechainicio))
                    {
                        Fechainicio = Vacaciones.FechaInicial;
                    }

                    if (!DateTime.TryParse(FechaFin, out Fechafinal))
                    {
                        Fechafinal = Vacaciones.FechaFin;
                    }
                    if (Cdp == null && Cds == null) {
                        Cdp = Vacaciones.CantDiasPendientes;
                        Cds = Vacaciones.CantDiasSolicitados;
                    }

                   
                    Vacaciones.FechaInicial = Fechainicio;
                    Vacaciones.FechaFin = Fechafinal;
                    Vacaciones.CantDiasPendientes = Cdp;
                    Vacaciones.CantDiasSolicitados = Cds;
                    Vacaciones.EstadoId = estado;
                    Vacaciones.Observacion = ObservacionTra;
                    db.SaveChanges();


                    Historial.VacacionesId = id;
                    Historial.Fecha = DateTime.Now;
                    Historial.Accion = Estado;
                    Historial.EmpleadoId = empleado;
                    Historial.Observaciones = Observacion;
                    Historial.UsuarioModifica = IdUsuarioM;
                    db.HistorialVacaciones.Add(Historial);
                    db.SaveChanges();

                }
                catch
                {
                }
            }

        }


        internal void CambioModoTrabajo(int id, string ModoTrabajo, string ObservacionModoTrabajo, string RequiereDesplazamiento)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    List<Empleado> empleado = new List<Empleado>();
                    empleado = db.Empleados.Where(x => x.Id == id).ToList();

                    foreach (var Item in empleado)
                    {   
                        db.Empleados.Attach(Item);
                        Item.ModoTrabajo = ModoTrabajo;
                        Item.ObservacionModoTrabajo = ObservacionModoTrabajo;
                        Item.RequiereDesplazamiento = RequiereDesplazamiento;
                        db.SaveChanges();
                    }
                }
                catch
                {

                }
            }
        }
    }
}