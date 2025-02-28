using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Adm_AutoGestion.Services
{
    public class PermisosRepository
    {
        public List<Permiso> ObtenerTodos()
        {
            using (var db = new AutogestionContext())
            {
                List<Permiso> Items = db.Permisos.ToList();
                foreach (Permiso Item in Items)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.EstadoPermiso = db.EstadosPermiso.FirstOrDefault(x => x.Id == Item.EstadoId);
                }
                return Items;
            }
        }

        public List<Permiso> ObtenerTodos2(string opcion, string EmpleadoId, int? EmpreUsu)
        {
            using (var db = new AutogestionContext())
            {
                List<Permiso> Items = new List<Permiso>();
                Empleado datos = new Empleado();

                int id = -1;
                Int32.TryParse(EmpleadoId, out id);
                datos = db.Empleados.Find(id);

                var lider = db.Empleados.Where(e => e.Lider.Trim() == datos.NroEmpleado.Trim() && e.Activo.Trim() == "SI").ToList();
                var jefe = db.Empleados.Where(e => e.Jefe.Trim() == datos.NroEmpleado.Trim() && e.Activo.Trim()  == "SI").ToList();

                if (opcion == "Aprobar")
                {

                    if (lider.Count != 0) {
                        Items.AddRange(db.Permisos.Where(e => e.EstadoId == 1 && e.Empleado.Lider == datos.NroEmpleado  && e.Empresa == datos.Empresa).ToList());
                    }

                    if (jefe.Count != 0)
                    {
                        Items.AddRange(db.Permisos.Where(e => e.EstadoId == 1 && e.Empleado.Jefe == datos.NroEmpleado && e.Empresa == datos.Empresa).ToList());
                    }


                    //Items = db.Permisos.Where(e => (e.EstadoId == 1 || e.EstadoId == 5) && (e.Empleado.Superior == datos.NroEmpleado || e.Empleado.Lider == datos.NroEmpleado || e.Empleado.Jefe == datos.NroEmpleado)).ToList();
                    foreach (Permiso Item in Items)
                    {
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                        Item.EstadoPermiso = db.EstadosPermiso.FirstOrDefault(x => x.Id == Item.EstadoId);
                        Item.MotivoPermiso = db.MotivosPermiso.FirstOrDefault(s => s.Id == Item.MotivoId);

                    }
                }

                if (opcion == "Confirmar")
                {
                    Items = db.Permisos.Where(e => (e.EstadoId == 2 || e.EstadoId == 6) && e.Empresa == EmpreUsu.ToString()).ToList();
                    foreach (Permiso Item in Items)
                    {
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                        Item.EstadoPermiso = db.EstadosPermiso.FirstOrDefault(x => x.Id == Item.EstadoId);
                        Item.MotivoPermiso = db.MotivosPermiso.FirstOrDefault(s => s.Id == Item.MotivoId);
                    }
                }
                //if (opcion == "Confirmar_Nom")
                //{
                //    Items = db.Permisos.Where(a => a.RevisadoNomina == "NO" && a.EstadoId == 4).ToList();
                //    foreach (Permiso Item in Items)
                //    {
                //        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                //        Item.EstadoPermiso = db.EstadosPermiso.FirstOrDefault(x => x.Id == Item.EstadoId);
                //        Item.MotivoPermiso = db.MotivosPermiso.FirstOrDefault(s => s.Id == Item.MotivoId);
                //    }

                //}

                return Items;
            }
        }


        public List<Permiso> ObtenerTodos3(string Id, string Fecha, string FechaPermiso, string FechaFinPermiso, string TrabajadorS)
        {
            using (var db = new AutogestionContext())
            {

                List<Permiso> Items = new List<Permiso>();
                Empleado datos = new Empleado();

                DateTime fecha = DateTime.Now;
                DateTime inicio = DateTime.Now;
                DateTime fin = DateTime.Now;


                if (!DateTime.TryParse(Fecha, out fecha))
                {
                    fecha = new DateTime();
                }

                if (!DateTime.TryParse(FechaPermiso, out inicio))
                {
                    //Fecha1 = DateTime.Now;
                    inicio = new DateTime();
                }

                if (!DateTime.TryParse(FechaFinPermiso, out fin))
                {
                    //Fecha1 = DateTime.Now;
                    fin = DateTime.Now;
                }


                if (Fecha == "" || Fecha == null)
                {

                    
                    int id = -1;
                    Int32.TryParse(Id, out id);

                    datos = db.Empleados.Find(id);

                    Items = db.Permisos.Where(a => a.RevisadoNomina == "NO" && a.EstadoId == 4
                                 && DbFunctions.TruncateTime(a.FechaPermiso) >= inicio
                                 && DbFunctions.TruncateTime(a.FechaFinPermiso) <= fin
                                 && (string.IsNullOrEmpty(TrabajadorS) || SqlFunctions.StringConvert((decimal)a.EmpleadoId).Contains(TrabajadorS)))
                                 .ToList();


                }
                else {

                   
                    int id = -1;
                    Int32.TryParse(Id, out id);

                    datos = db.Empleados.Find(id);

                    Items = db.Permisos.Where(a => a.RevisadoNomina == "NO" && a.EstadoId == 4
                                                && DbFunctions.TruncateTime(a.Fecha) == fecha
                                                && DbFunctions.TruncateTime(a.FechaPermiso) >= inicio
                                                && DbFunctions.TruncateTime(a.FechaFinPermiso) <= fin
                                                 && (string.IsNullOrEmpty(TrabajadorS) || SqlFunctions.StringConvert((decimal)a.EmpleadoId).Contains(TrabajadorS))).ToList();

                    
                }

                foreach (Permiso Item in Items)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.EstadoPermiso = db.EstadosPermiso.FirstOrDefault(x => x.Id == Item.EstadoId);
                    Item.MotivoPermiso = db.MotivosPermiso.FirstOrDefault(s => s.Id == Item.MotivoId);
                }

                return Items;
            }
        }

        internal void Modifica(string Observacion, string Id, string Empleado, string Estado, int IdUsuarioM, string Remunerado, string opcion, string Jornada, int tipo, int motivo, int usumod)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    if (usumod > 0)
                    {
                        Permiso Permisos = new Permiso();
                        int estado = 0;
                        int empleado = 0;
                        int id = 0;
                        DateTime Fecha2 = DateTime.Now;

                        int.TryParse(Estado, out estado);
                        int.TryParse(Empleado, out empleado);
                        int.TryParse(Id, out id);


                        //Confirmacion si viene de nomina, de gestion Humana o aprobacion

                        if ((estado == 4 && Jornada == "SI" && tipo == 2) || (estado == 4 && motivo == 2 && tipo == 2)) //si es Jornada completa y viene de confirmacion gestion Humana o Aprobado Jefe
                        {
                            Permisos = db.Permisos.FirstOrDefault(e => e.Id == id);
                            db.Permisos.Attach(Permisos);
                            Permisos.RevisadoNomina = "NO"; //Se le cambia el parametro Revisado Nomina a NO
                            db.SaveChanges();
                        }
                        else  //si es Jornada completa en confirmacion Nomina
                        {
                            Permisos = db.Permisos.FirstOrDefault(e => e.Id == id);
                            db.Permisos.Attach(Permisos);
                            Permisos.RevisadoNomina = "SI";// Se cambia a SI
                            db.SaveChanges();
                        }


                        Permisos = db.Permisos.FirstOrDefault(e => e.Id == id);
                        db.Permisos.Attach(Permisos);
                        Permisos.Jornada = Jornada;
                        Permisos.Id = id;
                        Permisos.EstadoId = estado;
                        if (Remunerado!="" && Remunerado !=null) 
                        {
                            Permisos.Remunerado = Remunerado;
                        }
                        
                        Permisos.EmpleadoId = empleado;
                        db.SaveChanges();

                        //************ Logica Tabla Historial Permisos *****************

                        HistorialPermisos Historial = new HistorialPermisos();



                        //if (estado == 4 && Jornada == "SI" && tipo == 2) //si es Jornada completa y viene de confirmacion GH
                        //{
                        //    Historial.PermisoId = Convert.ToInt32(Id);
                        //    //Historial.Estado = Convert.ToString(7); //Se le cambia el estado a Enviado a Nomina
                        //    Historial.Estado = Convert.ToString(estado);
                        //    Historial.Fecha_Permiso = Fecha2;
                        //    Historial.EmpleadoId = Convert.ToInt32(Empleado);
                        //    Historial.Usuario_Modifica = IdUsuarioM;
                        //    Historial.Observaciones_Permiso = Observacion;
                        //    db.HistorialPermisos.Add(Historial);
                        //}
                        //else  //si es Jornada completa en confirmacion Nomina
                        //{
                        Historial.PermisoId = Convert.ToInt32(Id);
                        Historial.Estado = Convert.ToString(estado);// Se deja normal como viene                       
                        Historial.Fecha_Permiso = Fecha2;
                        Historial.EmpleadoId = Convert.ToInt32(Empleado);
                        Historial.Usuario_Modifica = IdUsuarioM;
                        Historial.Observaciones_Permiso = Observacion;
                        db.HistorialPermisos.Add(Historial);
                        //}




                        //******************----------------****************************
                        if (opcion == "1")
                        {
                            Permisos.ObservacionJefe = Observacion;
                        }

                        if (opcion == "2")
                        {
                            Permisos.ObservacionGH = Observacion;
                        }
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