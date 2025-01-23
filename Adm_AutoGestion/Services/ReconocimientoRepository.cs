using Adm_AutoGestion.Models;
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
    public class ReconocimientoRepository
    {
        ServiciosRepository _serv = new ServiciosRepository();
        internal void Crear(Reconocimiento model)
        {
            using (var db = new AutogestionContext())
            {

                db.Reconocimientos.Add(model);
                db.SaveChanges();
                Empleado Emp = db.Empleados.Where(x => x.Id == model.EmpleadoReconocidoId).FirstOrDefault();
                notificar_Solicitud(Emp.Correo, model.EmpleadoId, model.Fecha, "solicitado");
            }
        }
        public List<Reconocimiento> ObtenerTodos2(string ID)
        {
            using (var db = new AutogestionContext())
            {
                List<Reconocimiento> Items2 = db.Reconocimientos.ToList();

                return Items2;
            }
        }
        public List<Reconocimiento> ObtenerDetallesReconocimiento(int reconocimientoId)
        {
            List<Reconocimiento> reconocimientos = new List<Reconocimiento>();
            using (var db = new AutogestionContext())
            {
                reconocimientos = db.Reconocimientos
                    .Where(d => d.Id == reconocimientoId)
                    .OrderBy(x => x.Fecha)
                    .ToList();

                foreach (Reconocimiento reconocimiento in reconocimientos)
                {
                    
                    reconocimiento.Empleado = db.Empleados.FirstOrDefault(e => e.Id == reconocimiento.EmpleadoId);
                    reconocimiento.EmpleadoReconocido = db.Empleados.FirstOrDefault(e => e.Id == reconocimiento.EmpleadoReconocidoId);
                    reconocimiento.TipoReconocimiento = db.TipoReconocimiento.FirstOrDefault(e => e.Id == reconocimiento.TipoReconocimientoId);
                    
                }
            }

            return reconocimientos;
        }
        internal bool ModificarGH(int Id, int IdUsuarioM)
        {

            using (var db = new AutogestionContext())
            {
                try
                {

                    Reconocimiento Reconocimiento = new Reconocimiento();

                    //int Ident = 2 ;
                    bool Ident2 = false;




                    Reconocimiento = db.Reconocimientos.FirstOrDefault(e => e.Id == Id);
                    db.Reconocimientos.Attach(Reconocimiento);
                    Reconocimiento.Activo = Ident2;
                    Reconocimiento.EmpleadoModificaId = IdUsuarioM;


                    db.SaveChanges();


                    return true;

                }
                catch
                {
                    return false;
                }
            }

        }
        public async Task<bool> ModificarVisto(int Id)
        {

            using (var db = new AutogestionContext())
            {
                try
                {

                    Reconocimiento Reconocimiento = new Reconocimiento();
                    
                    //int Ident = 2 ;
                    bool Ident2 = false;


                    

                    Reconocimiento = db.Reconocimientos.FirstOrDefault(e => e.Id == Id);
                    db.Reconocimientos.Attach(Reconocimiento);
                    Reconocimiento.Visto = Ident2;

                    
                    db.SaveChanges();

                    
                    return true;

                }
                catch
                {
                    return false;
                }
            }

        }
        public bool notificar_Solicitud(string EmailEmp, int? usuarioid, DateTime fecha, string Solicitud)
        {
            using (var db = new AutogestionContext())
            {

                string textocorreo = "";
                bool confirmacion;
                string txtde = Properties.Settings.Default.Correo.ToString();
                string contraseñacorreo = "";
                var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
                
                var Fecha2 = String.Format("{0:dd-MM-yyyy}", fecha); ;
                var email = "";
                var email2 = "";

                Empleado EMP = db.Empleados.Where(x => x.Id == usuarioid).FirstOrDefault();
                //EstadosHorasExtra Estado = db.EstadosHorasExtra.Where(x => x.Id == HorasExtra.Estado).FirstOrDefault();

                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

                string texto = "";
                if (Solicitud == "solicitado")
                {
                    
                    texto = db.Configuraciones.First(s => s.Parametro == "TXTRECONCREATE").Valor.ToString();
                    //email = db.Configuraciones.First(s => s.Parametro == "Prueba").Valor.ToString(); // TEMPORAL    
                    //Codigo Email
                    email = EmailEmp;                                                                                             //email= EmailJefe;
                    email2 = "gomezmadiedokevindanilo@gmail.com";


                    texto = texto.Replace("$FECHA", Fecha2);

                    texto = texto.Replace("$NOMBRE_EMP", EMP.Nombres);
                    


                }
                





                textocorreo = texto;



                try
                {


                    correo.To.Add(email);
                    correo.To.Add(email2);

                    correo.Subject = "Notificación Reconocimiento.";
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
    }
}
