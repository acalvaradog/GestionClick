
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using Adm_AutoGestion.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Adm_AutoGestion.Services
{
    public class ServiciosRepository
    {

        public Byte[] GenerarQR(string txtQRCode)
        {
          
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(txtQRCode, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            //System.Web.UI.WebControls.Image imgBarCode = new System.Web.UI.WebControls.Image();
            //imgBarCode.Height = 150;
            //imgBarCode.Width = 150;
            using (Bitmap bitMap = qrCode.GetGraphic(20))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    Byte[] imageBytes = ms.ToArray();
                    return imageBytes;
                    //imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                }
            }
           
        }


        public Byte[] GenerarQREmpleado(string txtQRCode)
        {
            string cadena = "";
            cadena = encriptar(txtQRCode);
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(cadena, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            using (Bitmap bitMap = qrCode.GetGraphic(20))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    Byte[] imageBytes = ms.ToArray();
                    return imageBytes;
                    //imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                }
            }

        }

        public string encriptar(string cadena) {
            byte[] llave; //Arreglo donde guardaremos la llave para el cifrado 3DES.
            byte[] arreglo = UTF8Encoding.UTF8.GetBytes(cadena); //Arreglo donde guardaremos la cadena descifrada.
            // Ciframos utilizando el Algoritmo MD5.
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            llave = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes("$S3gur0$"));
            md5.Clear();
            //Ciframos utilizando el Algoritmo 3DES.
            TripleDESCryptoServiceProvider tripledes = new TripleDESCryptoServiceProvider();
            tripledes.Key = llave;
            tripledes.Mode = CipherMode.ECB;
            tripledes.Padding = PaddingMode.PKCS7;
            ICryptoTransform convertir = tripledes.CreateEncryptor(); // Iniciamos la conversión de la cadena
            byte[] resultado = convertir.TransformFinalBlock(arreglo, 0, arreglo.Length); //Arreglo de bytes donde guardaremos la cadena cifrada.
            tripledes.Clear();
            return Convert.ToBase64String(resultado, 0, resultado.Length); // Convertimos la cadena y la regresamos.
        
        }
        public string desencriptar(string cadena)
        {
            byte[] llave;
            byte[] arreglo = Convert.FromBase64String(cadena); // Arreglo donde guardaremos la cadena descovertida.
            // Ciframos utilizando el Algoritmo MD5.
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            llave = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes("$S3gur0$"));
            md5.Clear();
            //Ciframos utilizando el Algoritmo 3DES.
            TripleDESCryptoServiceProvider tripledes = new TripleDESCryptoServiceProvider();
            tripledes.Key = llave;
            tripledes.Mode = CipherMode.ECB;
            tripledes.Padding = PaddingMode.PKCS7;
            ICryptoTransform convertir = tripledes.CreateDecryptor();
            byte[] resultado = convertir.TransformFinalBlock(arreglo, 0, arreglo.Length);
            tripledes.Clear();
            string cadena_descifrada = UTF8Encoding.UTF8.GetString(resultado); // Obtenemos la cadena
            return cadena_descifrada; // Devolvemos la cadena
        }

        public bool  EnviarEmailEPP(Empleado Empleado,DetalleEntregaEPP Entrega){

            using (var db = new AutogestionContext())
            {
                try {

                    var id = Entrega.Id + "|" + Empleado.Id + "|email" ;
                    string EntregaId = String.Format("{0}", Entrega.Id);
                    var plainTextBytes= System.Text.Encoding.UTF8.GetBytes(id);
                    id = System.Convert.ToBase64String(plainTextBytes);
                    var configuracion = db.Configuraciones.First(x => x.Parametro == "TXTEMAIL");
                    var link = db.Configuraciones.First(x => x.Parametro == "LINKFIRMA").Valor.ToString();
                    link = link + "/Submit?str=" + id;
                    //Nombre empleado
                    var texto = configuracion.Valor.ToString();
                    texto = texto.Replace("$NOMBRES", Empleado.Nombres);
                    //Elementos entregados
                    ElementosProtecionPersonal EPP = new ElementosProtecionPersonal();
                    EPP = db.ElementosProtecionPersonal.FirstOrDefault(x => x.Id == Entrega.EPP);
                    texto = texto.Replace("$FECHA", Entrega.Fecha.ToShortDateString());
                    texto = texto.Replace("$ELEMENTOS", Entrega.Cantidad + " " + EPP.Nombre.ToString() + " Entregado el " + Entrega.Fecha.ToShortDateString());
                    texto = texto.Replace("$LINK", link);
                    return EnviarEmail(Empleado.Correo.ToString(), texto, Entrega.Fecha.ToShortDateString(), EntregaId);
              

                }catch(Exception ex){
                    return false;
                
                }
            }

           
        }

        public bool EnviarEmail(string para, string texto, string fecha, string EntregaId)
        {
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);

            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde, "Notificaciones Foscal");
                correo.To.Add(para);
                correo.Subject = "Entrega EPP " + fecha + " - " + EntregaId ;
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


        public bool EnviarEmailCAP(Empleado Empleado, DetalleCapacitacion Detalle)
        {

            using (var db = new AutogestionContext())
            {
                try
                {
                    var cap = db.Capacitacion.FirstOrDefault(x => x.Id == Detalle.CapacitacionId);
                    var id = Detalle.Id + "|" + Empleado.Id + "|email";
                    string EntregaId = String.Format("{0}", Detalle.Id);
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(id);
                    id = System.Convert.ToBase64String(plainTextBytes);
                    var configuracion = db.Configuraciones.First(x => x.Parametro == "TXTFIRMACAPACT");

                    var link = db.Configuraciones.First(x => x.Parametro == "LINKFIRMACAPACT").Valor.ToString();
                    link = link + "/Submit?str=" + id;

                    var texto = configuracion.Valor.ToString();
                    texto = texto.Replace("$USUARIO", Empleado.Nombres);
                    texto = texto.Replace("$NOMBRECAP", cap.Nombre);
                    texto = texto.Replace("$HORAI", cap.HoraInicio);
                    texto = texto.Replace("$HORAF", cap.HoraFin);
                    texto = texto.Replace("$FECHACAP", cap.FechaCapacitacion.ToShortDateString());
                    texto = texto.Replace("$LINK", link);



                    return EnviarEmail2(Empleado.Correo.ToString(), texto, EntregaId);



                }
                catch (Exception ex)
                {
                    return false;

                }
            }



        }

        public bool EnviarEmailCAP2(Tercero Empleado, DetalleCapacitacion Detalle)
        {

            using (var db = new AutogestionContext())
            {
                try
                {
                    var cap = db.Capacitacion.FirstOrDefault(x => x.Id == Detalle.CapacitacionId);

                    var id = Detalle.Id + "|" + Empleado.Id + "|email";

                    string EntregaId = String.Format("{0}", Detalle.Id);

                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(id);

                    id = System.Convert.ToBase64String(plainTextBytes);

                    var configuracion = db.Configuraciones.First(x => x.Parametro == "TXTFIRMACAPACT");

                    var link = db.Configuraciones.First(x => x.Parametro == "LINKFIRMACAPACT").Valor.ToString();
                    link = link + "/Submit?str=" + id;


                    var texto = configuracion.Valor.ToString();
                    texto = texto.Replace("$USUARIO", Empleado.Nombres);
                    texto = texto.Replace("$NOMBRECAP", cap.Nombre);
                    texto = texto.Replace("$HORAI", cap.HoraInicio);
                    texto = texto.Replace("$HORAF", cap.HoraFin);
                    texto = texto.Replace("$FECHACAP", cap.FechaCapacitacion.ToShortDateString());
                    texto = texto.Replace("$LINK", link);



                    return EnviarEmail2(Empleado.CorreoPersonal.ToString(), texto, EntregaId);



                }
                catch (Exception ex)
                {
                    return false;

                }
            }



        }

        public bool EnviarEmailRdP(RequerimientosDelPersonal Detalle)
        {

            using (var db = new AutogestionContext())
            {
                try
                {
                    //var cap = db.Capacitacion.FirstOrDefault(x => x.Id == Detalle.CapacitacionId);
                    var emp = db.Empleados.FirstOrDefault(x => x.Id == Detalle.EmpleadoRegistraId);

                    var emp2 = db.Empleados.FirstOrDefault(x => x.NroEmpleado == emp.Director);

                    var id = Detalle.Id + "|" + emp2.Id + "|email";

                    string EntregaId = String.Format("{0}", Detalle.Id);

                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(id);

                    id = System.Convert.ToBase64String(plainTextBytes);

                    var configuracion = db.Configuraciones.First(x => x.Parametro == "TEXTORPFIRMA");

                    var link = "http://localhost:55389/RequerimientosPersonal/FirmaSolicitud2";
                    //db.Configuraciones.First(x => x.Parametro == "LINKFIRMARP").Valor.ToString();

                    link = link + "/Submit?str=" + id;

                    var idRdp = Convert.ToString(Detalle.Id);
                    var fechaRdp = Convert.ToString(Detalle.Fecha);
                    var texto = configuracion.Valor.ToString();

                    texto = texto.Replace("$ID", idRdp);
                    texto = texto.Replace("$FECHAREGISTRO", fechaRdp);
                    texto = texto.Replace("$EMPLEADOREGISTRA", emp.Nombres);
                    texto = texto.Replace("$LINK", link);
                    var para = "";

                    if (emp2.CorreoPersonal != null && emp2.CorreoPersonal != "")
                    {
                        para = emp2.CorreoPersonal.ToString();
                    }
                    if (emp2.Correo != null && emp2.Correo != "")
                    {
                        para = para + " " + emp2.Correo.ToString();
                    }


                    return EnviarEmailRdP(para, texto, EntregaId);



                }
                catch (Exception ex)
                {
                    return false;

                }
            }



        }


        public bool EnviarEmailRdP(string para, string texto, string EntregaId)
        {
            var para2 = "lflorez661@gmail.com";
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
            //var fecha = DateTime.Now ;
            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde, "Notificaciones Foscal");
                correo.To.Add(para2);
                correo.Subject = "Requerimiento del Personal " + " - " + EntregaId;
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


        public bool EnviarEmail2(string para, string texto, string EntregaId)
        {
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
            //var fecha = DateTime.Now ;
            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde, "Notificaciones Foscal");
                correo.To.Add(para);
                correo.Subject = "Detalle Capacitacion " + " - " + EntregaId;
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


        // Envio de correo trajes de mayo 

        public bool EnviarEmailPrestamo(string correo, int IdEncabezado, string Nombres)
        {

            using (var db = new AutogestionContext())
            {
                try
                {

                    List<DetalleEncabezadoPrestamo> detalle = new List<DetalleEncabezadoPrestamo>();
                    EncabezadoPrestamo prestamo = new EncabezadoPrestamo(); 

                    detalle = db.DetalleEncabezadoPrestamo.Where(x => x.IdEncabezadoPrestamo == IdEncabezado).ToList();

                    prestamo = db.EncabezadoPrestamo.FirstOrDefault(x => x.Id == IdEncabezado); 
                    var id = IdEncabezado +  "|email";
                    string detalleId = String.Format("{0}", IdEncabezado);
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(id);
                    id = System.Convert.ToBase64String(plainTextBytes);
                    var configuracion = db.Configuraciones.First(x => x.Parametro == "TXTEMAILPRESTAMO");
                    //var link = db.Configuraciones.First(x => x.Parametro == "LINKFIRMA").Valor.ToString();
                    //link = link + "/Submit?str=" + id;
                    //Nombre empleado
                    var texto = configuracion.Valor.ToString();
                    texto = texto.Replace("$NOMBRES", Nombres);
                    texto = texto.Replace("$FECHA", prestamo.FechaRegistro.ToShortDateString());
                    //Elementos entregados
                    TipoElementos TipoE = new TipoElementos();

                    string elementos = ""; 

                    foreach (var item in detalle) {

                    TipoE = db.TipoElementos.FirstOrDefault(x => x.Id == item.IdTipoElementos);

                    elementos = elementos +"<BR> " + item.Cantidad + " " + TipoE.Nombre.ToString();
                    
                    
                    }

                    texto = texto.Replace("$ELEMENTOS", elementos);
                    //TipoE = db.TipoElementos.FirstOrDefault(x => x.Id == detalle.IdTipoElementos);
                    //texto = texto.Replace("$FECHA", detalle.FechaEntrega.ToShortDateString());
                    //texto = texto.Replace("$ELEMENTOS", detalle.Cantidad + " " + TipoE.Nombre.ToString());
                    //texto = texto.Replace("$LINK", link);
                    return EnviarEmailPres(correo.ToString(), texto, prestamo.FechaRegistro.ToShortDateString(), detalleId);


                }
                catch (Exception ex)
                {
                    return false;

                }
            }


        }

        public bool EnviarEmailPres(string para, string texto, string fecha, string detalleId)
        {
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);






            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde, "Notificaciones Foscal");
                correo.To.Add(para);
                correo.Subject = " Notificación Prestamo Traje de Mayo " + fecha + " - " + detalleId;
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


        // Envio de correo devolucion tranjes de mayo 

        public bool EnviarEmailDevolucion(string correo, int id, string Nombres)
        {

            using (var db = new AutogestionContext())
            {
                try
                {

                    List<DetalleEncabezadoPrestamo> detalle = new List<DetalleEncabezadoPrestamo>();
                    EncabezadoPrestamo prestamo = new EncabezadoPrestamo();

                    detalle = db.DetalleEncabezadoPrestamo.Where(x => x.IdEncabezadoPrestamo == id).ToList();

                    prestamo = db.EncabezadoPrestamo.FirstOrDefault(x => x.Id == id);
                    var Id = id + "|email";
                    string detalleId = String.Format("{0}", id);
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Id);
                    Id = System.Convert.ToBase64String(plainTextBytes);
                    var configuracion = db.Configuraciones.First(x => x.Parametro == "TXTEMAILDEVOLUCION");
                    //var link = db.Configuraciones.First(x => x.Parametro == "LINKFIRMA").Valor.ToString();
                    //link = link + "/Submit?str=" + id;
                    //Nombre empleado
                    var texto = configuracion.Valor.ToString();
                    texto = texto.Replace("$NOMBRES", Nombres);
                    texto = texto.Replace("$FECHA", prestamo.FechaRegistro.ToShortDateString());
                    //Elementos entregados
                    TipoElementos TipoE = new TipoElementos();

                    string elementos = "";

                    foreach (var item in detalle)
                    {

                        TipoE = db.TipoElementos.FirstOrDefault(x => x.Id == item.IdTipoElementos);

                        elementos = elementos + "<BR> " + item.Cantidad + " " + TipoE.Nombre.ToString();


                    }

                    texto = texto.Replace("$ELEMENTOS", elementos);
                    //TipoE = db.TipoElementos.FirstOrDefault(x => x.Id == detalle.IdTipoElementos);
                    //texto = texto.Replace("$FECHA", detalle.FechaEntrega.ToShortDateString());
                    //texto = texto.Replace("$ELEMENTOS", detalle.Cantidad + " " + TipoE.Nombre.ToString());
                    //texto = texto.Replace("$LINK", link);
                    return EnviarEmailDevo(correo.ToString(), texto, prestamo.FechaRegistro.ToShortDateString(), detalleId);


                }
                catch (Exception ex)
                {
                    return false;

                }
            }


        }

        public bool EnviarEmailDevo(string para, string texto, string fecha, string detalleId)
        {
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);






            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde, "Notificaciones Foscal");
                correo.To.Add(para);
                correo.Subject = " Notificación Devolución Traje de Mayo " + fecha + " - " + detalleId;
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

        // // Envio de correo Anulacion tranjes de mayo 
        public bool EnviarEmailAnulacion(string correo, int id, string Nombres)
        {

            using (var db = new AutogestionContext())
            {
                try
                {

                    List<DetalleEncabezadoPrestamo> detalle = new List<DetalleEncabezadoPrestamo>();
                    EncabezadoPrestamo prestamo = new EncabezadoPrestamo();

                    detalle = db.DetalleEncabezadoPrestamo.Where(x => x.IdEncabezadoPrestamo == id).ToList();

                    prestamo = db.EncabezadoPrestamo.FirstOrDefault(x => x.Id == id);
                    var Id = id + "|email";
                    string detalleId = String.Format("{0}", id);
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Id);
                    Id = System.Convert.ToBase64String(plainTextBytes);
                    var configuracion = db.Configuraciones.First(x => x.Parametro == "TXTEMAILANULACION");
                    //var link = db.Configuraciones.First(x => x.Parametro == "LINKFIRMA").Valor.ToString();
                    //link = link + "/Submit?str=" + id;
                    //Nombre empleado
                    var texto = configuracion.Valor.ToString();
                    texto = texto.Replace("$NOMBRES", Nombres);
                    texto = texto.Replace("$FECHA", prestamo.FechaRegistro.ToShortDateString());
                    //Elementos entregados
                    TipoElementos TipoE = new TipoElementos();

                    string elementos = "";

                    foreach (var item in detalle)
                    {

                        TipoE = db.TipoElementos.FirstOrDefault(x => x.Id == item.IdTipoElementos);

                        elementos = elementos + "<BR> " + item.Cantidad + " " + TipoE.Nombre.ToString();


                    }

                    texto = texto.Replace("$ELEMENTOS", elementos);
                    //TipoE = db.TipoElementos.FirstOrDefault(x => x.Id == detalle.IdTipoElementos);
                    //texto = texto.Replace("$FECHA", detalle.FechaEntrega.ToShortDateString());
                    //texto = texto.Replace("$ELEMENTOS", detalle.Cantidad + " " + TipoE.Nombre.ToString());
                    //texto = texto.Replace("$LINK", link);
                    return EnviarEmailAnu(correo.ToString(), texto, prestamo.FechaRegistro.ToShortDateString(), detalleId);


                }
                catch (Exception ex)
                {
                    return false;

                }
            }


        }

        public bool EnviarEmailAnu(string para, string texto, string fecha, string detalleId)
        {
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);






            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde, "Notificaciones Foscal");
                correo.To.Add(para);
                correo.Subject = " Notificación Anulación Traje de Mayo " + fecha + " - " + detalleId;
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

        public bool EnviarEmailGenerico(string para, string texto, string asunto)
        {
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);






            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde, "Notificaciones Foscal");
                correo.To.Add(para);
                correo.Subject = asunto;
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

        public bool EmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

    }
}