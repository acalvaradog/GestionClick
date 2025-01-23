using Adm_AutoGestion.Models;
using Adm_AutoGestion.Models.EvaDesempeno;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using static Adm_AutoGestion.Controllers.RequerimientosPersonalController;

namespace Adm_AutoGestion.Services
{
    public class RequerimientosPersonalRepository
    {
        //---------EstadosRdPInialmenteCreados-------------//
        // id(1)= "Solicitado";
        // id(3)= "AprobadoDirectorArea";
        // id(4)= "AprobadoGerencia";
        // id(5)= "Selección";
        // id(6)= "Anulado";
        // id(7)= "Cerrado";
        //----------------------------//

        //-------------MtvSolicitud-----------------------//
        //id(1)="VACANTE";
        //id(2)="NUEVO CARGO";
        //id(3)="AUMENTO DE PLANTA DE PERSONAL";
        //id(4)="VACACIONES";
        //id(5)="LICENCIA NO REMUNERADA";
        //id(6)="INCAPACIDAD";
        //id(7)="LICENCIA MATERNIDAD/PATERNIDAD";
        //id(8)="PROGRAMA ESPECIAL";
        //-------------------------------------------------//
        internal string Crear(RequerimientosDelPersonal model, int userlog, string[] filas, string npersonas)
        {
            using (var db = new AutogestionContext())
            {
                string message = null;
                DetalleRequerimiento detalle = new DetalleRequerimiento();
                try
                {
                    model.EmpleadoRegistraId = userlog;
                    model.Fecha = DateTime.Now;
                    model.EstadoID = 1;
                    db.RequerimientosDelPersonal.Add(model);
                    db.SaveChanges();
                    if (filas != null)
                    {
                        foreach (var item in filas)
                        {
                            var linea = item.Split(';');
                            detalle.RequerimientoId = model.Id;
                            detalle.EmpSaliente = linea[0];
                            detalle.MotivoEgresoId = Convert.ToInt32(linea[1]);
                            db.DetalleRequerimiento.Add(detalle);
                            db.SaveChanges();
                        }
                    }                    
                    message = String.Format("Ok");                    
                }
                catch (SystemException ex)
                {
                    message = String.Format("Se genero un error. {0}", ex.Message);
                }
                return message;
            }
        }

        public List<RequerimientosDelPersonal> ObtenerTodos()
        {
            using (var db = new AutogestionContext())
            {
                List<RequerimientosDelPersonal> Items = db.RequerimientosDelPersonal.Where(x => x.EstadoID == 1).OrderByDescending(x => x.Fecha).ToList();
                foreach (RequerimientosDelPersonal Item in Items)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
                    Item.Empresa = db.Sociedad.FirstOrDefault(e => e.Id == Item.EmpresaId);
                    Item.EstadoRdP = db.EstadoRdP.FirstOrDefault(e => e.Id == Item.EstadoID);
                    Item.Jornada = db.Jornada.FirstOrDefault(e => e.Id == Item.JornadaRequeridaId);
                    Item.Horario = db.Horario.FirstOrDefault(e => e.Id == Item.HorarioTrabajoId);
                    //Item.MotivoEgreso = db.MotivoEgreso.FirstOrDefault(e => e.Id == Item.MotivoEgresoId);
                    Item.MtvSolicitud = db.MtvSolicitud.FirstOrDefault(e => e.Id == Item.MtvSolicitudID);
                }
                return Items;
            }
        }

        public RequerimientosDelPersonal ObtenerTodos2(int id)
        {

            List<DetalleRequerimiento> lista = new List<DetalleRequerimiento>();
            List<DetalleRequerimiento> Lista_Cont = new List<DetalleRequerimiento>();

            using (var db = new AutogestionContext())
            {
                RequerimientosDelPersonal Items = db.RequerimientosDelPersonal.FirstOrDefault(e => e.Id == id);

                Items.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Items.EmpleadoRegistraId);
                Items.Empresa = db.Sociedad.FirstOrDefault(e => e.Id == Items.EmpresaId);
                Items.EstadoRdP = db.EstadoRdP.FirstOrDefault(e => e.Id == Items.EstadoID);
                Items.Jornada = db.Jornada.FirstOrDefault(e => e.Id == Items.JornadaRequeridaId);
                Items.Horario = db.Horario.FirstOrDefault(e => e.Id == Items.HorarioTrabajoId);
                Items.MtvSolicitud = db.MtvSolicitud.FirstOrDefault(e => e.Id == Items.MtvSolicitudID);
                var det = db.DetalleRequerimiento.Where(e => e.RequerimientoId == Items.Id && e.EmpSaliente != "NOAPLICA").ToList();
                var Tot_Cont = db.DetalleRequerimiento.Where(e => e.RequerimientoId == Items.Id && e.Contratado != null).ToList();
                
                foreach (DetalleRequerimiento detalle in det)
                {
                    int emp = 0;
                    Int32.TryParse(detalle.EmpSaliente, out emp);
                    detalle.Empleado = db.Empleados.FirstOrDefault(e => e.Id == emp);
                    detalle.MotivoEgreso = db.MotivoEgreso.FirstOrDefault(e => e.Id == detalle.MotivoEgresoId);                    
                    lista.Add(detalle);                    
                }
                foreach (DetalleRequerimiento detalle2 in Tot_Cont)
                {
                    
                    Lista_Cont.Add(detalle2);
                }
                
                Items.Detalle_Requerimientos_Cont =Lista_Cont;
                Items.Requerimientos_Cont = Tot_Cont.Count();
                Items.DetalleRequerimiento = lista;
                return Items;
            }
        }

        public List<RequerimientosDelPersonal> ObtenerTodos3(int opcion)
        {
            using (var db = new AutogestionContext())
            {
                List<RequerimientosDelPersonal> Items2 = new List<RequerimientosDelPersonal>();
                if (opcion == 1)
                {
                    Items2 = db.RequerimientosDelPersonal.Where(e => e.EstadoID == 3
                         && (e.MtvSolicitudID == 2 || e.MtvSolicitudID == 3 || e.MtvSolicitudID == 7 || e.MtvSolicitudID == 8)
                        ).OrderByDescending(x => x.Fecha).ToList();
                    foreach (RequerimientosDelPersonal Item in Items2)
                    {
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
                        Item.Empresa = db.Sociedad.FirstOrDefault(e => e.Id == Item.EmpresaId);
                        Item.EstadoRdP = db.EstadoRdP.FirstOrDefault(e => e.Id == Item.EstadoID);
                        Item.Jornada = db.Jornada.FirstOrDefault(e => e.Id == Item.JornadaRequeridaId);
                        Item.Horario = db.Horario.FirstOrDefault(e => e.Id == Item.HorarioTrabajoId);
                        Item.MtvSolicitud = db.MtvSolicitud.FirstOrDefault(e => e.Id == Item.MtvSolicitudID);
                    }
                }
                else if (opcion == 2)
                {
                    Items2 = db.RequerimientosDelPersonal.Where(e =>
                       (e.EstadoID == 3 && e.MtvSolicitudID != 2 && e.MtvSolicitudID != 3 && e.MtvSolicitudID != 7 && e.MtvSolicitudID != 8)
                                                      ||
                        (e.EstadoID == 4)
                        ).OrderByDescending(x => x.Fecha).ToList();

                    foreach (RequerimientosDelPersonal Item in Items2)
                    {
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
                        Item.Empresa = db.Sociedad.FirstOrDefault(e => e.Id == Item.EmpresaId);
                        Item.EstadoRdP = db.EstadoRdP.FirstOrDefault(e => e.Id == Item.EstadoID);
                        Item.Jornada = db.Jornada.FirstOrDefault(e => e.Id == Item.JornadaRequeridaId);
                        Item.Horario = db.Horario.FirstOrDefault(e => e.Id == Item.HorarioTrabajoId);
                        Item.MtvSolicitud = db.MtvSolicitud.FirstOrDefault(e => e.Id == Item.MtvSolicitudID);
                    }
                }
                return Items2;
            }
        }

        public List<RequerimientosDelPersonal> ObtenerTodos4(int Emplog)
        {
            List<DetalleRequerimiento> Lista_Cont = new List<DetalleRequerimiento>();
            List<RequerimientosDelPersonal> Items = new List<RequerimientosDelPersonal>();
            using (var db = new AutogestionContext())
            {
                Items = db.RequerimientosDelPersonal.Where(x => x.EncargadoContratacion == Emplog && x.EstadoID == 5).OrderByDescending(x => x.Fecha).ToList();
                
                foreach (RequerimientosDelPersonal Item in Items)
                {
                    var Tot_Cont = db.DetalleRequerimiento.Where(e => e.RequerimientoId == Item.Id && e.Contratado != null).ToList();
                    Item.Detalle_Requerimientos_Cont = db.DetalleRequerimiento.Where(a=> a.RequerimientoId == Item.Id).ToList();
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
                    Item.Empresa = db.Sociedad.FirstOrDefault(e => e.Id == Item.EmpresaId);
                    Item.EstadoRdP = db.EstadoRdP.FirstOrDefault(e => e.Id == Item.EstadoID);
                    Item.EstadoSeleccionRdP = db.EstadoSeleccionRdP.FirstOrDefault(e => e.Id == Item.EstadoSeleccion);
                    Item.Jornada = db.Jornada.FirstOrDefault(e => e.Id == Item.JornadaRequeridaId);
                    Item.Horario = db.Horario.FirstOrDefault(e => e.Id == Item.HorarioTrabajoId);
                    Item.MtvSolicitud = db.MtvSolicitud.FirstOrDefault(e => e.Id == Item.MtvSolicitudID);
                    Item.Requerimientos_Cont = Tot_Cont.Count();                    
                }
            }
            return Items;
        }

        public string Aprobar(int IdSolicitud, string Accion, string TipoConcurso, Aprobacioneslog Model, int EmpContratacion)
        {
            try
            {
                using (var db = new AutogestionContext())
                {
                    var Solicitud = db.RequerimientosDelPersonal.FirstOrDefault(x => x.Id == IdSolicitud);
                    var empresa = db.Sociedad.Find(Solicitud.EmpresaId);
                    if (Accion == "DirectorArea")
                    {
                        Model.EstadoNuevo = 3;
                        Solicitud.EstadoID = 3;
                        if (Solicitud.MtvSolicitudID == 3 || Solicitud.MtvSolicitudID == 2 || Solicitud.MtvSolicitudID == 7 || Solicitud.MtvSolicitudID == 8)
                        {
                            enviar_correo_Aprobado(Solicitud.EmpleadoRegistraId, Solicitud.Cargo, Solicitud.Area, empresa.Descripcion, Solicitud.NumeroPresonas, Accion, true);

                        }
                        else
                        {
                            enviar_correo_Aprobado(Solicitud.EmpleadoRegistraId, Solicitud.Cargo, Solicitud.Area, empresa.Descripcion, Solicitud.NumeroPresonas, Accion, false);
                        }
                    }
                    if (Accion == "Gerencia")
                    {
                        Model.EstadoNuevo = 4;
                        Solicitud.EstadoID = 4;
                        enviar_correo_Aprobado(Solicitud.EmpleadoRegistraId, Solicitud.Cargo, Solicitud.Area, empresa.Descripcion, Solicitud.NumeroPresonas, Accion, false);
                    }
                    if (Accion == "GestiónHumana")
                    {
                        Model.EstadoNuevo = 5;
                        Solicitud.EstadoID = 5;                        
                        if (string.IsNullOrEmpty(TipoConcurso))
                        {
                            Solicitud.TipoConcurso = "INTERNO";
                        }
                        else
                        {
                            Solicitud.TipoConcurso = TipoConcurso;
                        }
                        if (EmpContratacion != 0)
                        {
                            Solicitud.EncargadoContratacion = EmpContratacion;
                            enviar_correo_Encargado(Solicitud.Cargo, Solicitud.Area, empresa.Descripcion, Solicitud.NumeroPresonas, EmpContratacion, IdSolicitud);
                        }
                    }

                    db.Entry(Solicitud).State = EntityState.Modified;
                    db.Aprobacioneslog.Add(Model);
                    db.SaveChanges();
                }
                return "Válido";
            }
            catch (SystemException ex)
            {
                return "Error: " + ex;
            }
        }


        public string Anular(int IdSolicitud, Aprobacioneslog Model)
        {
            try
            {
                using (var db = new AutogestionContext())
                {
                    var Solicitud = db.RequerimientosDelPersonal.FirstOrDefault(x => x.Id == IdSolicitud);
                    var empresa = db.Sociedad.Find(Solicitud.EmpresaId);
                    var emple = db.Empleados.Find(Solicitud.EmpleadoRegistraId);
                    var Estado = Solicitud.EstadoID;
                    //if (Estado == 1)
                    //{
                    //    Model.EstadoNuevo = 7;
                    //    Solicitud.EstadoID = 7;
                    //}
                    //if (Estado == 3)
                    //{
                    //    Model.EstadoNuevo = 7;
                    //    Solicitud.EstadoID = 7;
                    //}
                    //if (Estado == 4)
                    //{
                    Model.EstadoNuevo = 7;
                    Solicitud.EstadoID = 7;
                    //}
                    db.Entry(Solicitud).State = EntityState.Modified;
                    db.Aprobacioneslog.Add(Model);
                    db.SaveChanges();
                    enviar_correo_Anulado(Solicitud.Cargo, Solicitud.Area, empresa.Descripcion, emple.Documento, emple.Nombres, Solicitud.EstadoID, Solicitud.NumeroPresonas, Model.Observación);
                }
                return "Válido";
            }
            catch (SystemException ex)
            {
                return "Error: " + ex;
            }
        }
        public string Reactivar(int IdSolicitud, Aprobacioneslog Model)
        {
            try
            {
                using (var db = new AutogestionContext())
                {
                    var Solicitud = db.RequerimientosDelPersonal.FirstOrDefault(x => x.Id == IdSolicitud);
                    var empresa = db.Sociedad.Find(Solicitud.EmpresaId);
                    var emple = db.Empleados.Find(Solicitud.EmpleadoRegistraId);
                    var Estado = Solicitud.EstadoID;
                    var Accion = "Reactivado";
                    Model.EstadoNuevo = 1;
                    Solicitud.EstadoID = 1;
                    db.Entry(Solicitud).State = EntityState.Modified;
                    db.Aprobacioneslog.Add(Model);
                    db.SaveChanges();
                    enviar_correo_Aprobado(Solicitud.EmpleadoRegistraId, Solicitud.Cargo, Solicitud.Area, empresa.Descripcion, Solicitud.NumeroPresonas, Accion, false);
                }
                return "Válido";
            }
            catch (SystemException ex)
            {
                return "Error: " + ex;
            }
        }
        public string CambiarEstadoSeleccion(int IdSolicitud,int estadosele, Aprobacioneslog Model)
        {
            try
            {
                using (var db = new AutogestionContext())
                {
                    var Solicitud = db.RequerimientosDelPersonal.FirstOrDefault(x => x.Id == IdSolicitud);
                    var empresa = db.Sociedad.Find(Solicitud.EmpresaId);
                    
                    var emple = db.Empleados.Find(Solicitud.EmpleadoRegistraId);
                    var correo = emple.Correo;
                    var Estado = Solicitud.EstadoID;
                    var Accion = "CambioEstSel";
                    Model.EstadoNuevo = estadosele;
                    Solicitud.EstadoSeleccion = estadosele;
                    db.Entry(Solicitud).State = EntityState.Modified;
                    db.Aprobacioneslog.Add(Model);
                    db.SaveChanges();
                    enviar_correo_CambioEstSel(correo, Solicitud.EstadoSeleccion,Solicitud.EmpleadoRegistraId, Solicitud.Cargo, Solicitud.Area, empresa.Descripcion, Solicitud.NumeroPresonas, Accion, false);
                }
                return "Válido";
            }
            catch (SystemException ex)
            {
                return "Error: " + ex;
            }
        }
        public string Cerrar(int IdSolicitud, Aprobacioneslog Model, string[] filas)
        {
            try
            {
                using (var db = new AutogestionContext())
                {
                    int f = 0;
                    int l = 0;
                    DetalleRequerimiento detalle = new DetalleRequerimiento();
                    DateTime Fecha = DateTime.Now;
                    RequerimientosDelPersonal Solicitud = db.RequerimientosDelPersonal.FirstOrDefault(x => x.Id == IdSolicitud);                    
                    List<DetalleRequerimiento> detalle2 = db.DetalleRequerimiento.Where(x => x.RequerimientoId == Solicitud.Id).ToList();                    
                    if (detalle2.Count() < Solicitud.NumeroPresonas)
                    {
                        for (int i = 0; i < (Solicitud.NumeroPresonas - detalle2.Count()); i++)
                        {
                            var motiegre = db.MotivoEgreso.Where(a => a.Nombre == "NO APLICA").FirstOrDefault().Id;
                            DetalleRequerimiento detalle3 = new DetalleRequerimiento();
                            detalle3.RequerimientoId = Solicitud.Id;
                            detalle3.EmpSaliente = "NOAPLICA";
                            detalle3.MotivoEgresoId = motiegre;
                            db.DetalleRequerimiento.Add(detalle3);
                            db.SaveChanges();
                        }
                    }    
                    
                    ////---------------------------------------------------
                    var empresa = db.Sociedad.Find(Solicitud.EmpresaId);
                    var emple = db.Empleados.Find(Solicitud.EmpleadoRegistraId);
                    var Estado = Solicitud.EstadoID;
                    Model.EstadoNuevo = 6;
                    Model.Observación = "Registro Final Personal contratado";
                    db.Aprobacioneslog.Add(Model);
                    db.RequerimientosDelPersonal.Attach(Solicitud);
                    Solicitud.EstadoID = 6;
                    foreach (var item in filas)
                    {    
                            List<DetalleRequerimiento> detalle1 = db.DetalleRequerimiento.Where(x => x.RequerimientoId == Solicitud.Id && x.Contratado == null).ToList();
                            if (detalle1.Count() != 0)
                            {
                                    var linea = item.Split(';');
                                    detalle = detalle1[0];
                                    db.DetalleRequerimiento.Attach(detalle);
                                    DateTime.TryParse(linea[1], out Fecha);
                                    detalle.Contratado = linea[0];
                                    detalle.FechaIngreso = Fecha;
                                    detalle.TipoEmpleado = linea[2];
                                    l++;
                                    db.SaveChanges();
                            }                                                      
                    }
                    db.Entry(Solicitud).State = EntityState.Modified;
                    db.Aprobacioneslog.Add(Model);
                    enviar_correo_Cerrado(Solicitud.Cargo, Solicitud.Area, empresa.Descripcion, emple.Documento, emple.Nombres, Solicitud.EstadoID, Solicitud.NumeroPresonas, filas,Solicitud.Id);
                    db.SaveChanges();
                }
                return "Válido";
            }
            catch (SystemException ex)
            {
                return "Error: " + ex;
            }
        }
        public string Cerrar_Parcial(int IdSolicitud, Aprobacioneslog Model, string[] filas)
        {
            try
            {
                using (var db = new AutogestionContext())
                {

                    int f = 0;
                    int l = 0;
                    DetalleRequerimiento detalle = new DetalleRequerimiento();
                    DateTime Fecha = DateTime.Now;

                    //Se consulta el requerimiento y el detalle del mismo
                    RequerimientosDelPersonal Solicitud = db.RequerimientosDelPersonal.FirstOrDefault(x => x.Id == IdSolicitud);
                    List<DetalleRequerimiento> detalle1 = db.DetalleRequerimiento.Where(x => x.RequerimientoId == Solicitud.Id && x.Contratado == null).ToList();
                    //---------------------------------------------------



                    var empresa = db.Sociedad.Find(Solicitud.EmpresaId);
                    var emple = db.Empleados.Find(Solicitud.EmpleadoRegistraId);
                    var Estado = Solicitud.EstadoID;
                    Solicitud.EstadoID = 5;
                    Model.EstadoNuevo = 5;
                    Model.Observación = "Registro Parcial Personal contratado";
                    db.RequerimientosDelPersonal.Attach(Solicitud);
                    foreach (var item in filas)
                    {
                        //List<DetalleRequerimiento> detalle2 = db.DetalleRequerimiento.Where(x => x.RequerimientoId == Solicitud.Id).ToList();
                       
                            var linea = item.Split(';');
                            if (detalle1.Count() > 0)
                            {
                                detalle = detalle1[l];
                                db.DetalleRequerimiento.Attach(detalle);
                            }

                            DateTime.TryParse(linea[1], out Fecha);
                            detalle.Contratado = linea[0];
                            detalle.FechaIngreso = Fecha;
                            detalle.TipoEmpleado = linea[2];
                            l++;
                            if (detalle1.Count() == 0)
                            {
                                detalle.EmpSaliente = "NOAPLICA";
                                detalle.RequerimientoId = Solicitud.Id;
                                detalle.MotivoEgresoId = Solicitud.MtvSolicitudID;
                                db.DetalleRequerimiento.Add(detalle);
                            }
                            db.SaveChanges();
                        
                    }
                    db.Entry(Solicitud).State = EntityState.Modified;
                    db.Aprobacioneslog.Add(Model);
                    db.SaveChanges();
                    //if (Solicitud.MtvSolicitudID == 2 || Solicitud.MtvSolicitudID == 3 || Solicitud.MtvSolicitudID == 7 || Solicitud.MtvSolicitudID == 8)
                    //{
                    //    db.Entry(Solicitud).State = EntityState.Modified;
                    //    enviar_correo_Cerrado(Solicitud.Cargo, Solicitud.Area, empresa.Descripcion, emple.Documento, emple.Nombres, Solicitud.EstadoID, Solicitud.NumeroPresonas, filas);
                    //}
                    //else
                    //{                     
                        db.Entry(Solicitud).State = EntityState.Modified;
                        enviar_correo_Cerrado_Parcial(Solicitud.Cargo, Solicitud.Area, empresa.Descripcion, emple.Documento, emple.Nombres, Solicitud.EstadoID, Solicitud.NumeroPresonas, filas, Solicitud.Id);
                    //}
                }
                return "Válido";
            }
            catch (SystemException ex)
            {
                return "Error: " + ex;
            }
        }


        public List<Aprobacioneslog> ObtenerRegistro(int Id)
        {
            List<Aprobacioneslog> Items = new List<Aprobacioneslog>();
            using (var db = new AutogestionContext())
            {
                Items = db.Aprobacioneslog.Where(x => x.IdRequerimiento == Id).ToList();
                foreach (var Item in Items)
                {
                    Item.DatosUsuario = db.Empleados.FirstOrDefault(x => x.Id == Item.Usuario);
                }
            }
            return Items;
        }
public bool enviar_correo_CambioEstSel(string correoEmpl, int EstadoSeleccion, int EmpleadoRegistraId, string Cargo, string Area, string empresa, int NumeroPresonas, string Accion, bool EsGerencia)
        {
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
            using (var db = new AutogestionContext())
            {
                var texto = db.Configuraciones.First(s => s.Parametro == "TEXTOCAMBIOESTSELECC").Valor.ToString();
                var link = db.Configuraciones.First(s => s.Parametro == "LINKREQPER").Valor.ToString();
                var jefe = db.Empleados.First(c => c.Id == EmpleadoRegistraId);
                var est = db.EstadoSeleccionRdP.Where(a=>a.Id == EstadoSeleccion && a.Estado == "Activo").FirstOrDefault().Nombre;
                var NombreJefe = jefe.Nombres;
                texto = texto.Replace("$JEFESOLICITUD", NombreJefe);
                texto = texto.Replace("$CARGOREQUERIMIENTO", Cargo);
                texto = texto.Replace("$AREAREQUERIMIENTO", Area);
                texto = texto.Replace("$EMPRESAREQUERIMIETNO", empresa);
                texto = texto.Replace("$NUMEROPERSONAS", NumeroPresonas.ToString());
                texto = texto.Replace("$ACCION", Accion);
                texto = texto.Replace("$NUEVOEST", est);
                try
                {
                    System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                    correo.From = new System.Net.Mail.MailAddress(txtde);
                    correo.To.Add(correoEmpl);   
                    correo.Subject = "Notificación Cambio del Estado de Selección del Requerimiento de Personal";
                    correo.Body = texto;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;
                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    smtp.Send(correo);
                    confirmacion = true;
                    return confirmacion;
                }
                catch (Exception ex)
                {
                    confirmacion = false;
                    return confirmacion;
                }
            }
        }

        public bool enviar_correo_Aprobado(int EmpleadoRegistraId, string Cargo, string Area, string empresa, int NumeroPresonas, string Accion, bool EsGerencia)
        {
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);


            using (var db = new AutogestionContext())
            {
                var correo1 = "";
                if (EsGerencia == true)
                {
                    correo1 = db.Configuraciones.First(s => s.Parametro == "CORREOGERENCIA").Valor.ToString();
                }
                else
                {
                    correo1 = db.Configuraciones.First(s => s.Parametro == "CORREOGESTIONHUMANA").Valor.ToString();
                }

                var texto = db.Configuraciones.First(s => s.Parametro == "TEXTOAPROBADOREQUEPER").Valor.ToString();
                var link = db.Configuraciones.First(s => s.Parametro == "LINKREQPER").Valor.ToString();
                var jefe = db.Empleados.First(c => c.Id == EmpleadoRegistraId);
                var NombreJefe = jefe.Nombres;

                texto = texto.Replace("$JEFESOLICITUD", NombreJefe);
                texto = texto.Replace("$CARGOREQUERIMIENTO", Cargo);
                texto = texto.Replace("$AREAREQUERIMIENTO", Area);
                texto = texto.Replace("$EMPRESAREQUERIMIETNO", empresa);
                texto = texto.Replace("$NUMEROPERSONAS", NumeroPresonas.ToString());
                texto = texto.Replace("$ACCION", Accion);
                texto = texto.Replace("$LINK", link);

                try
                {
                    System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                    correo.From = new System.Net.Mail.MailAddress(txtde);
                    correo.To.Add(correo1);
                    
                    if (Accion == "Gerencia")
                    {
                        var CorreoGH = "";
                        var EmpleadosGH = db.Empleados.Where(a=> a.Jefe == "40001527" ).ToList();                        
                        foreach (var item in EmpleadosGH)
                        {
                            //List<string> funciones = Acceso.Validar(item.Id);
                            //if (funciones.Contains("RdPGestionHumana"))
                            //{
                                CorreoGH += "," +  item.Correo;
                            //}
                        }
                        correo.CC.Add(CorreoGH);
                    }
                    correo.Subject = "Notificación de Aprobación del Requerimiento de Personal";
                    correo.Body = texto;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;


                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    smtp.Send(correo);
                    confirmacion = true;
                    return confirmacion;

                }
                catch (Exception ex)
                {

                    confirmacion = false;
                    return confirmacion;
                }
            }
        }

        public bool enviar_correo_Encargado(string Cargo, string Area, string empresa, int NumeroPresonas, int EmpContratacion, int Id)
        {
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);


            using (var db = new AutogestionContext())
            {
                //var correo1 = db.Configuraciones.First(s => s.Parametro == "CORREOENCARGADO").Valor.ToString();
                var texto = db.Configuraciones.First(s => s.Parametro == "CORREOENCARGADO").Valor.ToString();
                // var link = db.Configuraciones.First(s => s.Parametro == "LINKREQPER").Valor.ToString();


                texto = texto.Replace("$CARGOREQUERIMIENTO", Cargo);
                texto = texto.Replace("$AREAREQUERIMIENTO", Area);
                texto = texto.Replace("$EMPRESAREQUERIMIETNO", empresa);
                texto = texto.Replace("$NUMEROPERSONAS", NumeroPresonas.ToString());

                // texto = texto.Replace("$LINK", link);


                try
                {
                    enviar_correo_Notificacion_Jefe_Encargado_Seleccion(Id, EmpContratacion);
                    System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                    correo.From = new System.Net.Mail.MailAddress(txtde);
                    //correo.To.Add(correo1);
                    var CorreoPer = db.Empleados.First(c => c.Id == EmpContratacion);
                    correo.To.Add(CorreoPer.Correo); //Correo Laboral del Empleado
                    correo.Subject = "Correo Notificación de asignación como encargado de contratación  del Requerimiento de Personal aprobado por  Gestion Humana";
                    correo.Body = texto;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;


                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    smtp.Send(correo);
                    confirmacion = true;
                    return confirmacion;

                }
                catch (Exception ex)
                {

                    confirmacion = false;
                    return confirmacion;
                }
            }
        }


        public bool enviar_correo_Anulado(string Cargo, string Area, string empresa, string cedula, string nombre, int estado, int NumeroPresonas, string Observación)
        {
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);


            using (var db = new AutogestionContext())
            {

                var texto = db.Configuraciones.First(s => s.Parametro == "TEXTOCORREOANULADO").Valor.ToString();
                //var link = db.Configuraciones.First(s => s.Parametro == "LINKLOGINPERM").Valor.ToString();

                texto = texto.Replace("$USUARIO", nombre);
                texto = texto.Replace("$ESTADO", "NO APROBADO");
                texto = texto.Replace("$CARGOREQUERIMIENTO", Cargo);
                texto = texto.Replace("$AREAREQUERIMIENTO", Area);
                texto = texto.Replace("$EMPRESAREQUERIMIETNO", empresa);
                texto = texto.Replace("$NUMEROPERSONAS", NumeroPresonas.ToString());
                texto = texto.Replace("$OBSERVACION", Observación.ToString());

                //texto = texto.Replace("$LINK", link);


                try
                {
                    System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                    correo.From = new System.Net.Mail.MailAddress(txtde);
                    var CorreoPer = db.Empleados.First(c => c.Documento == cedula);

                    correo.To.Add(CorreoPer.Correo); //Correo Laboral del Empleado
                    //correo.To.Add(correo1);
                    correo.Subject = "Notificación de Anulacion del Requerimiento de Personal Solicitado";
                    correo.Body = texto;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;


                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    smtp.Send(correo);
                    confirmacion = true;
                    return confirmacion;

                }
                catch (Exception ex)
                {

                    confirmacion = false;
                    return confirmacion;
                }
            }
        }


        public bool enviar_correo_Cerrado(string Cargo, string Area, string empresa, string cedula, string nombre, int estado, int NumeroPresonas, string[] datos, int Idre)
        {
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var data = "<label>Personas contratadas:</label>";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);


            using (var db = new AutogestionContext())
            {
                var ant = db.DetalleRequerimiento.Where(a => a.RequerimientoId == Idre && a.Contratado != null).ToList();
                var texto = db.Configuraciones.First(s => s.Parametro == "TEXTOCORRECERRADO").Valor.ToString();
                texto = texto.Replace("$USUARIO", nombre);
                texto = texto.Replace("$ESTADO", "CERRADO");
                texto = texto.Replace("$CARGOREQUERIMIENTO", Cargo);
                texto = texto.Replace("$AREAREQUERIMIENTO", Area);
                texto = texto.Replace("$EMPRESAREQUERIMIETNO", empresa);
                texto = texto.Replace("$NUMEROPERSONAS", NumeroPresonas.ToString());
                //foreach (var i in datos)
                //{
                //    var linea = i.Split(';');
                //    data += "<ul><li>Nombre: " + linea[0] + " </li> <li>Fecha de Ingreso:" + linea[1] + "</li></ul>";                    
                //}
                foreach (var e in ant)
                {
                    data += "<ul><li>Nombre: " + e.Contratado + " </li> <li>Fecha de Ingreso:" + e.FechaIngreso + "</li><li class='capitalize'>Tipo de Empleado:" + e.TipoEmpleado.ToLower() + "</li></ul>";
                }
                texto = texto.Replace("$DATOS", data);
                try
                {
                    System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                    correo.From = new System.Net.Mail.MailAddress(txtde);
                    var CorreoPer = db.Empleados.First(c => c.Documento == cedula);
                    correo.To.Add(CorreoPer.Correo); 
                    correo.Subject = "Notificación de Cierre del Requerimiento de Personal Solicitado";
                    correo.Body = texto;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;


                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    smtp.Send(correo);
                    confirmacion = true;
                    return confirmacion;

                }
                catch (Exception ex)
                {

                    confirmacion = false;
                    return confirmacion;
                }
            }
        }
        
            //_________________________________________________________________________________________________________//

            public bool enviar_correo_Cerrado_Parcial(string Cargo, string Area, string empresa, string cedula, string nombre, int estado, int NumeroPresonas, string[] datos, int Idre)
        {
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var data = "<label>Personas contratadas:</label>";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
            

            using (var db = new AutogestionContext())
            {
                var ant = db.DetalleRequerimiento.Where(a => a.RequerimientoId == Idre && a.Contratado != null).ToList();
                var texto = db.Configuraciones.First(s => s.Parametro == "TEXTOCORRECERRADOPARCIAL").Valor.ToString();
                texto = texto.Replace("$USUARIO", nombre);
                texto = texto.Replace("$ESTADO", "SELECCIÓN");
                texto = texto.Replace("$CARGOREQUERIMIENTO", Cargo);
                texto = texto.Replace("$AREAREQUERIMIENTO", Area);
                texto = texto.Replace("$EMPRESAREQUERIMIETNO", empresa);
                texto = texto.Replace("$NUMEROPERSONAS", NumeroPresonas.ToString());                
                //foreach (var i in datos.OrderByDescending(a=> a.Count()))
                //{
                //    var linea = i.Split(';');
                //    data += "<ul><li>Nombre: " + linea[0] + " </li> <li>Fecha de Ingreso:" + linea[1] + "</li></ul>";                    
                //}                
                foreach (var e in ant)
                {                    
                    data += "<ul><li>Nombre: " + e.Contratado + " </li> <li>Fecha de Ingreso:" + e.FechaIngreso + "</li><li>Tipo de Empleado:" + e.TipoEmpleado + "</li></ul>";
                }
                texto = texto.Replace("$DATOS", data);
                try
                {
                    System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                    correo.From = new System.Net.Mail.MailAddress(txtde);
                    var CorreoPer = db.Empleados.First(c => c.Documento == cedula);
                    correo.To.Add(CorreoPer.Correo);
                    correo.Subject = "Notificación de Cierre Parcial del Requerimiento de Personal Solicitado #" +Idre;
                    correo.Body = texto;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;
                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    smtp.Send(correo);
                    confirmacion = true;
                    return confirmacion;

                }
                catch (Exception ex)
                {

                    confirmacion = false;
                    return confirmacion;
                }
            }
        }

        //_________________________________________________________________________________________________________//

        public bool enviar_correo_Notificacion_Jefe_Encargado_Seleccion(int Idre, int EmpContratacion)
        {
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            //var data = "<label>Personas contratadas:</label>";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);


            using (var db = new AutogestionContext())
            {
                var req = db.RequerimientosDelPersonal.Where(a => a.Id == Idre).FirstOrDefault();
                var texto = db.Configuraciones.First(s => s.Parametro == "TEXTOCORREOENCARSELEC").Valor.ToString();
                var empre = db.Sociedad.Where(a => a.Id == req.EmpresaId).FirstOrDefault();
                var encarcontra = db.Empleados.Where(a => a.Id == EmpContratacion).FirstOrDefault();
                texto = texto.Replace("$USUARIOENCARSELEC", encarcontra.Nombres.ToString());
                texto = texto.Replace("$IDREQ", req.Id.ToString());
                //texto = texto.Replace("$USUARIO", req.EmpleadoRegistraId.ToString());
                texto = texto.Replace("$ESTADO", "Selección");                
                texto = texto.Replace("$CARGOREQUERIMIENTO", req.Cargo);                
                texto = texto.Replace("$AREAREQUERIMIENTO", req.Area);            
                texto = texto.Replace("$EMPRESAREQUERIMIETNO", empre.Descripcion.ToString());                
                texto = texto.Replace("$NUMEROPERSONAS", req.NumeroPresonas.ToString());
                try
                {
                    System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                    correo.From = new System.Net.Mail.MailAddress(txtde);
                    var CorreoPer = db.Empleados.First(c => c.Id == req.EmpleadoRegistraId);
                    correo.To.Add(CorreoPer.Correo);
                    correo.Subject = "Notificación de Asignación Encargado Selección Requerimiento de Personal Número: " + req.Id.ToString();
                    correo.Body = texto;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;


                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    smtp.Send(correo);
                    confirmacion = true;
                    return confirmacion;

                }
                catch (Exception ex)
                {

                    confirmacion = false;
                    return confirmacion;
                }
            }
        }
    }
}