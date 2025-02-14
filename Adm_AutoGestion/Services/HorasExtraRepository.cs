using Adm_AutoGestion.Migrations;
using Adm_AutoGestion.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.Expressions;

namespace Adm_AutoGestion.Services
{
    public class HorasExtraRepository
    {
        ServiciosRepository _serv = new ServiciosRepository();
        public List<HorasExtra> ObtenerTodos(string orden, string Empleadolog)
        {
            List<HorasExtra> Items2 = new List<HorasExtra>();
            using (var db = new AutogestionContext())
            {


                int id = -1;
                int id2 = -1;
                Int32.TryParse(orden, out id);
                Int32.TryParse(Empleadolog, out id2);

                //Items2 = db.Viaticos.Where(e => e.EstadosViaticos.id == id).ToList();

                if (orden == "JefeDirectoHorasExtra")
                {
                    var Jefecito = db.Empleados.FirstOrDefault(e => e.Id == id2);
                    var IdJefecito = Jefecito.NroEmpleado;


                    List<Empleado> EmpA = (from emp in db.Empleados
                                           where emp.Jefe == IdJefecito && emp.Activo!="NO"
                                           select emp).ToList();

                    //Items2 = db.Viaticos.Where(e => e.Estado == 1).ToList();
                    foreach (var emp in EmpA)
                    {
                        List<HorasExtra> cont = db.HorasExtra.Where(e => e.EmpleadoId ==emp.Id && e.Estado==1).OrderBy(x => x.FechaDeRegistro).ToList();

                        foreach (var HE in cont)
                        {
                            Items2.Add(HE);
                        }


                    }

                    foreach (HorasExtra Item in Items2)
                    {
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);

                        Item.EstadosHorasExtra = db.EstadosHorasExtra.FirstOrDefault(e => e.Id == Item.Estado);



                    }
                }




            }

            return Items2;
        }
        public List<HorasExtra> ObtenerHorasExtraAprobadasJefe(string orden, string Empleadolog)
        {
            List<HorasExtra> horasExtraAprobadas = new List<HorasExtra>();

            using (var db = new AutogestionContext())
            {
                int id = -1;
                int id2 = -1;
                Int32.TryParse(orden, out id);
                Int32.TryParse(Empleadolog, out id2);

                if (orden == "GestionHumana")
                {
                    var empresaDelJefe = db.Empleados.Where(e => e.Id == id2).Select(e => e.Empresa).FirstOrDefault();

                    //horasExtraAprobadas = db.HorasExtra
                    //.Where(he => he.Estado == 2 && he.Empleado.Empresa == empresaDelJefe)
                    //.OrderBy(he => he.FechaDeRegistro)
                    //.ToList();

                    horasExtraAprobadas = db.HorasExtra
                    .Where(he => he.Estado == 2)
                    .OrderBy(he => he.FechaDeRegistro)
                    .ToList();

                    foreach (HorasExtra he in horasExtraAprobadas)
                    {
                        he.Empleado = db.Empleados.FirstOrDefault(e => e.Id == he.EmpleadoId);
                        he.EstadosHorasExtra = db.EstadosHorasExtra.FirstOrDefault(e => e.Id == he.Estado);
                    }
                }
            }

            return horasExtraAprobadas;
        }

        public List<DetalleHorasExtra> ObtenerDetallesHorasExtra(int horasExtraId)
        {
            List<DetalleHorasExtra> detalles = new List<DetalleHorasExtra>();
            using (var db = new AutogestionContext())
            {
                detalles = db.DetalleHorasExtras
                    .Where(d => d.HorasExtraId == horasExtraId)
                    .OrderBy(x => x.Fecha)
                    .ToList();

                foreach (DetalleHorasExtra detalle in detalles)
                {
                    detalle.HorasExtra = db.HorasExtra.FirstOrDefault(e => e.Id == detalle.HorasExtraId);
                    detalle.HorasExtra.Empleado = db.Empleados.FirstOrDefault(e => e.Id == detalle.HorasExtra.EmpleadoId);
                    detalle.HorasExtra.EstadosHorasExtra = db.EstadosHorasExtra.FirstOrDefault(e => e.Id == detalle.HorasExtra.Estado);
                    detalle.MotivoTrabajoHE = db.MotivoTrabajoHE.FirstOrDefault(e => e.Id == detalle.MotivoTrabajoHEId);
                }
            }

            return detalles;
        }


        public List<DetalleHorasExtra> ObtenerDetallesHorasExtra2(string FechaI, string FechaF, string TrabajadorS)
        {
            DateTime Fecha1 = DateTime.Now;
            DateTime Fecha2 = DateTime.Now;

            if (!DateTime.TryParse(FechaI, out Fecha1))
            { Fecha1 = new DateTime(); }
            if (!DateTime.TryParse(FechaF, out Fecha2))
            { Fecha2 = DateTime.Now; }

            int trabajador = int.Parse(TrabajadorS);

            List<DetalleHorasExtra> detalles = new List<DetalleHorasExtra>();
            using (var db = new AutogestionContext())
            {
                detalles = db.DetalleHorasExtras
                           .Include(x => x.HorasExtra)
                    .Where(d => d.HorasExtra.FechaPago >= Fecha1 && d.HorasExtra.FechaPago <= Fecha2 && d.HorasExtra.EmpleadoId == trabajador)
                    .OrderBy(x => x.Fecha)
                    .ToList();

                foreach (DetalleHorasExtra detalle in detalles)
                {
                    detalle.HorasExtra = db.HorasExtra.FirstOrDefault(e => e.Id == detalle.HorasExtraId);
                    detalle.HorasExtra.Empleado = db.Empleados.FirstOrDefault(e => e.Id == detalle.HorasExtra.EmpleadoId);
                    detalle.HorasExtra.EstadosHorasExtra = db.EstadosHorasExtra.FirstOrDefault(e => e.Id == detalle.HorasExtra.Estado);
                    detalle.MotivoTrabajoHE = db.MotivoTrabajoHE.FirstOrDefault(e => e.Id == detalle.MotivoTrabajoHEId);
                }
            }

            return detalles;
        }


        internal bool Modificar(int Id, string Estado, string Observaciones, int IdUsuarioM, string FechaPago)
        {

            using (var db = new AutogestionContext())
            {
                try
                {

                    HorasExtra HorasExtra = new HorasExtra();
                    HistoricoHorasExtra Historial = new HistoricoHorasExtra();
                    //int Ident = 2 ;
                    int Ident2 = 0;


                    int.TryParse(Estado, out Ident2);

                    HorasExtra = db.HorasExtra.FirstOrDefault(e => e.Id == Id);
                    db.HorasExtra.Attach(HorasExtra);
                    HorasExtra.Estado = Ident2;
                    if (string.IsNullOrEmpty(FechaPago))
                    {
                        HorasExtra.FechaPago = null;  // O una fecha específica
                    }
                    else
                    {
                        HorasExtra.FechaPago = Convert.ToDateTime(FechaPago);
                    }

                    //Empleado emp = db.Empleados.Where(e => e.Id == Viaticos.EmpleadoId).FirstOrDefault();
                    //Empleado jefe = db.Empleados.Where(e => e.NroEmpleado == emp.NroEmpleado).FirstOrDefault();
                    //notificar_Solicitud(Viaticos.Id, jefe.Correo, Viaticos.EmpleadoId, Viaticos.FechaRegistro, Viaticos.MtvViaje, "AprobadoJefe");
                    db.SaveChanges();

                    Historial.HorasExtraId = Id;
                    Historial.EstadoNombre = Ident2.ToString();
                    Historial.FechaSolicitud = DateTime.Now;
                    Historial.UsuarioModifica = IdUsuarioM.ToString();
                    Historial.Observaciones = Observaciones;
                    db.HistoricoHorasExtra.Add(Historial);
                    db.SaveChanges();
                    return true;

                }
                catch
                {
                    return false;
                }
            }

        }

        public List<HorasExtra> ObtenerTodos2(string ID)
        {
            using (var db = new AutogestionContext())
            {
                List<HorasExtra> Items2 = db.HorasExtra.ToList();

                return Items2;
            }
        }
        public List<HistoricoHorasExtra> ObtenerTodos3(string EmpleadoId)
        {
            using (var db = new AutogestionContext())
            {
                List<HistoricoHorasExtra> Items2 = new List<HistoricoHorasExtra>();

                int id = -1;
                Int32.TryParse(EmpleadoId, out id);
                int Estado = 0;
                int Modifica = 0;

                Items2 = db.HistoricoHorasExtra.Where(e => e.HorasExtraId == id).ToList();

                foreach (HistoricoHorasExtra Item in Items2)
                {
                   // Int32.TryParse(Item.EstadoNombre, out Estado);
                    Int32.TryParse(Item.UsuarioModifica, out Modifica);
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Modifica);
                    // Item.EstadosHorasExtra = db.EstadosHorasExtra.FirstOrDefault(x => x.Id == Estado);
                    
                }

                return Items2;


            }
        }




        public Uri Url { get; }
        internal void Crear(HorasExtra model)
        {
            using (var db = new AutogestionContext())
            {

                db.HorasExtra.Add(model);
                db.SaveChanges();
                Empleado Emp = db.Empleados.Where(x => x.Id == model.EmpleadoId).FirstOrDefault();
                Empleado Jefe = db.Empleados.Where(x => x.NroEmpleado == Emp.Jefe).FirstOrDefault();
                Empleado Lider  = db.Empleados.Where(x => x.NroEmpleado == Emp.Lider).FirstOrDefault();

                if (Lider != null) {
                    
                    notificar_Solicitud(model.Id, Lider.Correo, Emp.Id, model.FechaDeRegistro, "solicitado");
                }

                   notificar_Solicitud(model.Id, Jefe.Correo, Emp.Id, model.FechaDeRegistro, "solicitado");
            }
        }
        public bool notificar_Solicitud(int IdHE, string EmailJefe, int usuarioid, DateTime fecha, string Solicitud)
        {
            using (var db = new AutogestionContext())
            {

                string textocorreo = "";
                bool confirmacion;
                string txtde = Properties.Settings.Default.Correo.ToString();
                string contraseñacorreo = "";
                var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
                var HorasExtra = db.HorasExtra.Where(e => e.Id == IdHE).FirstOrDefault();
                var HistoricoHorasExtra = db.HistoricoHorasExtra.Where(e => e.Id == IdHE).FirstOrDefault();
                var Fecha2 = String.Format("{0:dd-MM-yyyy}", fecha); ;
                var email = "";
                var AREASOLICITUD = "";
                Empleado EMP = db.Empleados.Where(x => x.Id == usuarioid).FirstOrDefault();
                //EstadosHorasExtra Estado = db.EstadosHorasExtra.Where(x => x.Id == HorasExtra.Estado).FirstOrDefault();

                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

                string texto = "";
                if (Solicitud == "solicitado")
                {
                    AREASOLICITUD = "Jefe Directo";
                    texto = db.Configuraciones.First(s => s.Parametro == "TXTHECREATE").Valor.ToString();
                    //email = db.Configuraciones.First(s => s.Parametro == "Prueba").Valor.ToString(); // TEMPORAL    
                    //Codigo Email
                    email = EmailJefe;                                                                                             //email= EmailJefe;


                    texto = texto.Replace("$ID", Convert.ToString(IdHE));
                    texto = texto.Replace("$FECHA", Fecha2);

                    texto = texto.Replace("$NOMBRE_EMP", EMP.Nombres);
                    texto = texto.Replace("$NMREMP", EMP.NroEmpleado);


                }
                if (Solicitud == "AprobadoJefe")
                {
                    AREASOLICITUD = "Gestión Humana";
                    texto = db.Configuraciones.First(s => s.Parametro == "TXTHECREATE").Valor.ToString();
                    email = db.Configuraciones.FirstOrDefault(s => s.Parametro == "TXTHECORREOGH").Valor.ToString();

                    texto = texto.Replace("$ID", Convert.ToString(IdHE));
                    texto = texto.Replace("$FECHA", Fecha2);

                    texto = texto.Replace("$NOMBRE_EMP", EMP.Nombres);
                    texto = texto.Replace("$NMREMP", EMP.NroEmpleado);

                }





                textocorreo = texto;



                try
                {


                    correo.To.Add(email);

                    correo.Subject = "Notificación Solicitud Horas Extra - " + AREASOLICITUD;
                    correo.Body = textocorreo;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;

                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    //envia
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
        public bool notificar_Rechazado(int IdHE, string EmailEmp, int usuarioid, DateTime fecha, string Observaciones, string Solicitud)
        {
            using (var db = new AutogestionContext())
            {

                string textocorreo = "";
                bool confirmacion;
                string txtde = Properties.Settings.Default.Correo.ToString();
                string contraseñacorreo = "";
                var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
                var HorasExtra = db.HorasExtra.Where(e => e.Id == IdHE).FirstOrDefault();
                var HistoricoHorasExtra = db.HistoricoHorasExtra.Where(e => e.HorasExtraId == IdHE).FirstOrDefault();
                var Fecha2 = String.Format("{0:dd-MM-yyyy}", fecha); ;
                var email1 = "";
                Empleado EMP = db.Empleados.Where(x => x.Id == usuarioid).FirstOrDefault();
                EstadosHorasExtra Estado = db.EstadosHorasExtra.Where(x => x.Id == HorasExtra.Estado).FirstOrDefault();

                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

                string texto = "";


                if (Solicitud == "Rechazado")
                {
                    email1 = EmailEmp;

                    texto = db.Configuraciones.First(s => s.Parametro == "TXTHERECHAZADO").Valor.ToString();
                    texto = texto.Replace("$ID", Convert.ToString(IdHE));
                    texto = texto.Replace("$FECHA", Fecha2);
                    texto = texto.Replace("$NOMBRE_EMP", EMP.Nombres);
                    texto = texto.Replace("$NMREMP", EMP.NroEmpleado);
                    texto = texto.Replace("$OBSERVACIONESJEFE", Observaciones);



                }

                textocorreo = texto;
                try
                {


                    correo.To.Add(email1);
                    correo.Subject = "Notificación Rechazo Solicitud Horas Extra.";
                    correo.Body = textocorreo;
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
        public bool notificar_RechazadoGH(int IdHE, string EmailEmp, string EmailJefe, int usuarioid, DateTime fecha, string Observaciones, string Solicitud)
        {
            using (var db = new AutogestionContext())
            {

                string textocorreo = "";
                bool confirmacion;
                string txtde = Properties.Settings.Default.Correo.ToString();
                string contraseñacorreo = "";
                var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
                var HorasExtra = db.HorasExtra.Where(e => e.Id == IdHE).FirstOrDefault();
                var HistoricoHorasExtra = db.HistoricoHorasExtra.Where(e => e.HorasExtraId == IdHE).FirstOrDefault();
                var Fecha2 = String.Format("{0:dd-MM-yyyy}", fecha); ;

                var email1 = "";
                var email2 = "";
                var email3 = "";
                Empleado EMP = db.Empleados.Where(x => x.Id == usuarioid).FirstOrDefault();
                EstadosHorasExtra Estado = db.EstadosHorasExtra.Where(x => x.Id == HorasExtra.Estado).FirstOrDefault();

                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

                string texto = "";


                if (Solicitud == "Rechazado")
                {
                    email1 = EmailEmp;
                    email2 = EmailJefe;
                    email3 = db.Configuraciones.FirstOrDefault(s => s.Parametro == "TXTHECORREOGH").Valor.ToString();
                    texto = db.Configuraciones.First(s => s.Parametro == "TXTHERECHAZADO").Valor.ToString();
                    texto = texto.Replace("$ID", Convert.ToString(IdHE));
                    texto = texto.Replace("$FECHA", Fecha2);


                    texto = texto.Replace("$NOMBRE_EMP", EMP.Nombres);
                    texto = texto.Replace("$NMREMP", EMP.NroEmpleado);
                    texto = texto.Replace("$OBSERVACIONESJEFE", Observaciones);

                }




                textocorreo = texto;



                try
                {


                    correo.To.Add(email1);
                    correo.To.Add(email2);
                    correo.To.Add(email3);

                    correo.Subject = "Notificación Rechazo Solicitud Horas Extra.";
                    correo.Body = textocorreo;
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
        public bool notificar_Cierre(int IdHE, string EmailEmp, string NombreJefe, int usuarioid, DateTime fecha, string Solicitud)
        {
            List<DetalleHorasExtra> detalles = new List<DetalleHorasExtra>();
            List<MotivoTrabajoHE> MotivoTrabajoHE = new List<MotivoTrabajoHE>();
            using (var db = new AutogestionContext())
            {

                string textocorreo = "";
                bool confirmacion;
                string txtde = Properties.Settings.Default.Correo.ToString();
                string contraseñacorreo = "";
                byte[] encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
                HorasExtra HorasExtra = db.HorasExtra.Where(e => e.Id == IdHE).FirstOrDefault();
                string Fecha2 = String.Format("{0:dd-MM-yyyy}", fecha); ;
                string NomJefe = NombreJefe;
                string email = "";
                string email2 = "";
                Empleado EMP = db.Empleados.Where(x => x.Id == usuarioid).FirstOrDefault();
                EstadosHorasExtra Estado = db.EstadosHorasExtra.Where(x => x.Id == HorasExtra.Estado).FirstOrDefault();
                MotivoTrabajoHE = db.MotivoTrabajoHE.ToList();
                detalles = db.DetalleHorasExtras.Where(d => d.HorasExtraId == IdHE).ToList();
                StringBuilder tablaDetalles = new StringBuilder();
                StringBuilder tablaDetallesEmp = new StringBuilder();

                tablaDetallesEmp.AppendLine($"<td style='text-align:center; border: 1px solid black;'>{EMP.Documento}</td>");
                tablaDetallesEmp.AppendLine($"<td style='text-align:center; border: 1px solid black;'>{EMP.Nombres}</td>");
                tablaDetallesEmp.AppendLine($"<td style='text-align:center; border: 1px solid black;'>{EMP.Cargo}</td>");
                tablaDetallesEmp.AppendLine($"<td style='text-align:center; border: 1px solid black;'>{"" + EMP.Empresa}</td>");
                tablaDetallesEmp.AppendLine($"<td style='text-align:center; border: 1px solid black;'>{EMP.AreaDescripcion}</td>");
                tablaDetallesEmp.AppendLine($"<td style='text-align:center; border: 1px solid black;'>{EMP.NroEmpleado}</td>");

                foreach (var detalle in detalles)
                {
                    var Motivo = MotivoTrabajoHE.Where(x => x.Id == detalle.MotivoTrabajoHEId).FirstOrDefault();
                    tablaDetalles.AppendLine("<tr>");

                    tablaDetalles.AppendLine($"<td style='text-align:center; border: 1px solid black;'>{Convert.ToDateTime(detalle.Fecha).ToString("dd/MM/yyyy")}</td>");
                    tablaDetalles.AppendLine($"<td style='text-align:center; border: 1px solid black;'>{Convert.ToDateTime(detalle.HoraDesde).ToString("HH:mm tt")}</td>");
                    tablaDetalles.AppendLine($"<td style='text-align:center; border: 1px solid black;'>{Convert.ToDateTime(detalle.HoraHasta).ToString("HH:mm tt")}</td>");
                    tablaDetalles.AppendLine($"<td style='text-align:center; border: 1px solid black;'>{detalle.LiquidacionDiurna}</td>");
                    tablaDetalles.AppendLine($"<td style='text-align:center; border: 1px solid black;'>{detalle.LiquidacionNocturna}</td>");
                    tablaDetalles.AppendLine($"<td style='text-align:center; border: 1px solid black;'>{detalle.LiquidacionDiurnaFestivo}</td>");
                    tablaDetalles.AppendLine($"<td style='text-align:center; border: 1px solid black;'>{detalle.LiquidacionNocturnaFestivo}</td>");
                    tablaDetalles.AppendLine($"<td style='text-align:center; border: 1px solid black;'>{detalle.TotalHoras}</td>");
                    tablaDetalles.AppendLine($"<td style='text-align:center; border: 1px solid black;'>{Motivo.Descripcion}</td>");
                    tablaDetalles.AppendLine($"<td style='text-align:center; border: 1px solid black;'>{detalle.ObservacionesMotivo}</td>");
                    tablaDetalles.AppendLine("</tr>");
                }

                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

                string texto = "";


                if (Solicitud == "Cierre")//EMPLEADO QUE CANCELA LA SOLICITUD
                {
                    email = EmailEmp;
                    email2 = db.Configuraciones.FirstOrDefault(s => s.Parametro == "TXTHECORREOGH").Valor.ToString();
                    texto = db.Configuraciones.First(s => s.Parametro == "TXTHECIERRE").Valor.ToString();
                    texto = texto.Replace("$ID", Convert.ToString(IdHE));



                    texto = texto.Replace("$NOMBRE_EMP", EMP.Nombres);
                    texto = texto.Replace("$DOCUMENTO", EMP.Documento);
                    texto = texto.Replace("$TABLADETALLE", tablaDetalles.ToString());
                    texto = texto.Replace("$TABLAEMP", tablaDetallesEmp.ToString());

                }




                textocorreo = texto;



                try
                {


                    correo.To.Add(email);
                    correo.To.Add(email2);

                    correo.Subject = "Notificación Cierre Solicitud Horas Extra.";
                    correo.Body = textocorreo;
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
        internal void CrearDetalleHorasExtra(IEnumerable<DetalleHorasExtra> modelelos)
        {
            using (var db = new AutogestionContext())
            {

                db.DetalleHorasExtras.AddRange(modelelos);
                db.SaveChanges();
            }
        }


        internal bool EnviarAprobados(int[] ids, int IdUsuarioM,string fechap,string opcion)
        {

            using (var db = new AutogestionContext())
            {
                try
                {

                    

                        HorasExtra model = new HorasExtra();
                        HistoricoHorasExtra Historial = new HistoricoHorasExtra();

                        foreach (int id2 in ids)
                        {
                            model = db.HorasExtra.FirstOrDefault(x => x.Id == id2);
                            db.HorasExtra.Attach(model);

                        

                        if (opcion == "JEFE") { model.Estado = 2; }
                        if (opcion == "LIDER") { model.Estado = 5; }
                        if (opcion == "GH") { model.Estado = 3; }

                        if (string.IsNullOrEmpty(fechap))
                        {
                            model.FechaPago = null;  // O una fecha específica
                        }
                        else
                        {
                            model.FechaPago = Convert.ToDateTime(fechap);
                        }
                        db.SaveChanges();

                            var estado = db.EstadosHorasExtra.FirstOrDefault(x => x.Id == model.Estado);

                            Historial.HorasExtraId = id2;
                            Historial.EstadoNombre = estado.Nombre;
                            Historial.FechaSolicitud = DateTime.Now;
                            Historial.UsuarioModifica = IdUsuarioM.ToString();
                            //Historial.Observaciones = Observaciones;
                            db.HistoricoHorasExtra.Add(Historial);
                            db.SaveChanges();
                           



                        }

                    return true;

                }
                catch
                {

                    return false;


                }

            }
        }


        internal bool EnviarRechazados(int[] ids, int IdUsuarioM, string Observaciones)
        {

            using (var db = new AutogestionContext())
            {
                try
                {



                    HorasExtra model = new HorasExtra();
                    HistoricoHorasExtra Historial = new HistoricoHorasExtra();

                    foreach (int id2 in ids)
                    {
                        model = db.HorasExtra.FirstOrDefault(x => x.Id == id2);
                        db.HorasExtra.Attach(model);
                        model.Estado = 4;
                        db.SaveChanges();

                        var estado = db.EstadosHorasExtra.FirstOrDefault(x => x.Id == model.Estado);

                        Historial.HorasExtraId = id2;
                        Historial.EstadoNombre = estado.Nombre;
                        Historial.FechaSolicitud = DateTime.Now;
                        Historial.UsuarioModifica = IdUsuarioM.ToString();
                        Historial.Observaciones = Observaciones;
                        db.HistoricoHorasExtra.Add(Historial);
                        db.SaveChanges();




                    }

                    return true;

                }
                catch
                {

                    return false;


                }

            }
        }



        internal bool EnviarRechazados2(int Id , int IdUsuarioM, string Observaciones)
        {

            using (var db = new AutogestionContext())
            {
                try
                {



                    HorasExtra model = new HorasExtra();
                    HistoricoHorasExtra Historial = new HistoricoHorasExtra();

                   
                        model = db.HorasExtra.FirstOrDefault(x => x.Id == Id);
                        db.HorasExtra.Attach(model);
                        model.Estado = 4;
                        model.FechaPago = null;
                        db.SaveChanges();

                        var estado = db.EstadosHorasExtra.FirstOrDefault(x => x.Id == model.Estado);

                        Historial.HorasExtraId = Id;
                        Historial.EstadoNombre = estado.Nombre;
                        Historial.FechaSolicitud = DateTime.Now;
                        Historial.UsuarioModifica = IdUsuarioM.ToString();
                        Historial.Observaciones = Observaciones;
                        db.HistoricoHorasExtra.Add(Historial);
                        db.SaveChanges();




                    

                    return true;

                }
                catch
                {

                    return false;


                }

            }
        }

    }
}
