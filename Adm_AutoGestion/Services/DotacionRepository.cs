using Adm_AutoGestion.Models;
using Adm_AutoGestion.Models.EvaDesempeno;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.Win32;
using System.Web.Services.Description;
using System.Text;
using Adm_AutoGestion.DTO;
using WebGrease.Css.Extensions;

namespace Adm_AutoGestion.Services
{
    public class DotacionRepository
    {
        AutogestionContext db = new AutogestionContext();
        ServiciosRepository _services = new ServiciosRepository();

        public List<Dotacion> ObtenerListado(string Year, string Cat, string Area, string Recibido)
        {
            int.TryParse(Year, out int year);
            bool.TryParse(Recibido, out bool recibido);
            if (Area == "Asi")
            {
                Area = "Asistenciales CO";
            }
            if (Area == "Adm")
            {
                Area = "Administrativos CO";
            }

            var registros = db.Dotacion.Where(x => x.Fecha.Value.Year == year && x.Fecha < DateTime.Now);

            registros = registros.Where(x => db.HistorialDotacion.Any(e => e.Nro == x.Nro && e.Recibido == recibido));

            if (!string.IsNullOrEmpty(Cat))
            {
                registros = registros.Where(x => db.Empleados.Any(e => e.CategoriaDotacion == Cat && e.Id == x.EmpleadoId));
            }

            if (!string.IsNullOrEmpty(Area))
            {
                registros = registros.Where(x => db.Empleados.Any(e => e.TipoArea == Area && e.Id == x.EmpleadoId));
            }

            return registros.ToList();
        }

        public bool LimpiezaEmpleadoNoActivo()
        {
            try
            {
                var empleados = db.Empleados.Where(x => x.Activo == "NO").Select(x => x.Id).ToList();
                var dotacion = db.Dotacion.Where(x => empleados.Contains(x.EmpleadoId)).ToList();
                db.Dotacion.RemoveRange(dotacion);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ActualizarDotaciones()
        {
            try
            {
                var listaAreas = db.Configuraciones.Where(x => x.Parametro == "DotacionConfig").ToList();

                var tipoAreaIntervalo = new Dictionary<string, int>();

                foreach (var i in listaAreas)
                {
                    tipoAreaIntervalo.Add(i.Descripcion, Convert.ToInt32(i.Valor));
                }

                var tipoAreaValidos = tipoAreaIntervalo.Keys.ToList();

                var empleados = db.Empleados
                    .Where(x => x.Activo == "SI" && tipoAreaValidos.Contains(x.TipoArea))
                    .ToList();

                List<Dotacion> datos = new List<Dotacion>();
                List<Dotacion> datosActualizables = new List<Dotacion>();
                var Nro = "";
                var AñoActual = DateTime.Now.Year;
                foreach (var i in empleados)
                {
                    var comprobacion = db.Dotacion.Where(x => x.EmpleadoId == i.Id).FirstOrDefault();
                    if (comprobacion == null)
                    {
                        int intervalo = tipoAreaIntervalo[i.TipoArea];
                        var FechaIntervalo = i.FechaIngreso.Value.AddMonths(intervalo);

                        if (FechaIntervalo < DateTime.Now)
                        {
                            var AñoIngresoEmpleado = FechaIntervalo.Year;
                            
                            if (AñoIngresoEmpleado == AñoActual)
                            {
                                Nro = FechaIntervalo.ToString("ddMMyyyy") + i.Id;
                                datos.Add(new Dotacion { Nro = Nro, EmpleadoId = i.Id, Fecha = FechaIntervalo });
                                //EnvioCorreoDerechoDotacion(i.Id);
                            }
                            else
                            {
                                var FechaIngreso = (DateTime)i.FechaIngreso;
                                var DiferenciaAños = AñoActual - AñoIngresoEmpleado;
                                var FechaSiguiente = FechaIngreso.AddYears(DiferenciaAños);
                                Nro = FechaSiguiente.ToString("ddMMyyyy") + i.Id;
                                datos.Add(new Dotacion { Nro = Nro, EmpleadoId = i.Id, Fecha = FechaSiguiente });
                                //EnvioCorreoDerechoDotacion(i.Id);
                            }
                        }
                    }
                    else if (comprobacion.Fecha < DateTime.Now)
                    {
                        var FechaIngreso = (DateTime)i.FechaIngreso;
                        DateTime FechaSiguiente = new DateTime();
                        if (FechaIngreso.Year == AñoActual)
                        {
                            FechaSiguiente = FechaIngreso.AddYears(1);
                        }
                        else
                        {
                            var DiferenciaAños = AñoActual - FechaIngreso.Year;
                            FechaSiguiente = FechaIngreso.AddYears(DiferenciaAños + 1);
                        }

                        if (FechaSiguiente < DateTime.Now)
                        {
                            Nro = FechaSiguiente.ToString("ddMMyyyy") + i.Id;
                            datosActualizables.Add(new Dotacion { Nro = Nro, EmpleadoId = i.Id, Fecha = FechaSiguiente });
                            //EnvioCorreoDerechoDotacion(i.Id);
                        }
                    }
                }
                foreach (var item in datos)
                {
                    db.Dotacion.Add(item);
                }
                foreach (var item in datosActualizables)
                {
                    var dotacion = db.Dotacion.FirstOrDefault(x => x.EmpleadoId == item.EmpleadoId);
                    
                    if (dotacion != null)
                    {
                        dotacion.Fecha = item.Fecha;
                        dotacion.Nro = item.Nro;
                    }
                }
                db.SaveChanges();
                RegistrarHistorial(datos);
                RegistrarHistorial(datosActualizables);

                return true;
            }
            catch
            {
                return false;
            }
        }

        internal void RegistrarHistorial(List<Dotacion> dotacion)
        {
            foreach (var item in dotacion)
            {
                HistorialDotacion model = new HistorialDotacion
                {
                    Nro = item.Nro,
                    EmpleadoId = item.EmpleadoId,
                    Fecha = item.Fecha,
                };

                db.HistorialDotacion.Add(model);
            }
            db.SaveChanges();
        }

        internal string AgregarRegistro(int EmpleadoId, DateTime Fecha)
        {
            if (EmpleadoId == 0 && Fecha == null)
            {
                return "null";
            }

            if (Fecha > DateTime.Now)
            {
                return "fecha";
            }

            try
            {
                var verificacion = db.Dotacion.FirstOrDefault(x => x.EmpleadoId == EmpleadoId);
                if (verificacion == null)
                {
                    var Nro = Fecha.ToString("ddMMyyyy") + EmpleadoId;
                    Dotacion dotacion = new Dotacion
                    {
                        EmpleadoId = EmpleadoId,
                        Fecha = Fecha,
                        Nro = Nro
                    };
                    db.Dotacion.Add(dotacion);
                    List<Dotacion> dotacions = new List<Dotacion>
                    {
                        dotacion
                    };
                    RegistrarHistorial(dotacions);
                    db.SaveChanges();
                    //EnvioCorreoDerechoDotacion(EmpleadoId);
                    return "true";
                }
                else
                {
                    return "existe";
                }
            }
            catch
            {
                return "error";
            }
        }

        public string EnvioCorreoDerechoDotacion()
        {
            using (var db = new AutogestionContext())
            {
                string txtde = Properties.Settings.Default.Correo.ToString();
                string contraseñacorreo = "";
                var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);

                var dotacionEmpleados = db.Dotacion.Select(x=> x.EmpleadoId).ToList();
                var emp = db.Empleados.Where(e => dotacionEmpleados.Contains(e.Id)).ToList();

                var correos = emp.Where(e=> e.Correo != null).Select(e => e.Correo).ToList();
                var correosPorComa = string.Join(",", correos);

                if (!string.IsNullOrEmpty(correosPorComa))
                {
                    var textoCorreo = db.Configuraciones.FirstOrDefault(x => x.Parametro == "TXTDOTACION");
                    var Asunto = "Notificación de Derecho a Dotación";
                    var Texto = textoCorreo.Valor;
                    var ListaCorreos = correosPorComa;

                    System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                    correo.From = new System.Net.Mail.MailAddress(txtde);

                    try
                    {
                        correo.To.Add(ListaCorreos);
                        correo.Subject = Asunto;
                        correo.Body = Texto;
                        correo.IsBodyHtml = true;

                        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                        smtp.Host = "smtp-relay.gmail.com";
                        smtp.Port = 587;
                        smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                        smtp.EnableSsl = true;
                        smtp.Send(correo);

                        return "true";
                    }
                    catch
                    {
                        return "false";
                    }
                }

                return "correo";
            }
        }

        public void EnvioCorreoDotacionFirmada(int id)
        {
            using (var db = new AutogestionContext())
            {
                string txtde = Properties.Settings.Default.Correo.ToString();
                string contraseñacorreo = "";
                var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);

                var emp = db.Empleados.FirstOrDefault(e => e.Id == id);
                var correoEmpleado = emp.Correo;
                if (!string.IsNullOrEmpty(correoEmpleado))
                {
                    var textoCorreo = db.Configuraciones.FirstOrDefault(x=> x.Parametro == "TXTDOTACIONFIRMADA");
                    var Asunto = "Confirmación de Entrega de Dotación";
                    var Texto = textoCorreo.Valor;
                    var correos = correoEmpleado;

                    System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                    correo.From = new System.Net.Mail.MailAddress(txtde);

                    try
                    {
                        correo.To.Add(correos);
                        correo.Subject = Asunto;
                        correo.Body = Texto;
                        correo.IsBodyHtml = true;

                        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                        smtp.Host = "smtp-relay.gmail.com";
                        smtp.Port = 587;
                        smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                        smtp.EnableSsl = true;
                        smtp.Send(correo);
                    }
                    catch
                    {

                    }
                }
            }
        }

        internal string FirmarQR(string id, string nro, string Cantidad)
        {
            using (var db = new AutogestionContext())
            {
                string message = "";

                try
                {
                    string datosqr = _services.desencriptar(id);
                    string[] datostrabajador = datosqr.Split('|');
                    var NroTrabajador = datostrabajador[0];

                    HistorialDotacion query = new HistorialDotacion();
                    //Query para obtener el registro del empleado que se quiere firmar
                    Empleado queryempleado = db.Empleados.FirstOrDefault(e => e.NroEmpleado == NroTrabajador);
                    var EmpleadoId = queryempleado.Id;
                    //Comprobar que el QR sea del empleado correcto
                    if (queryempleado != null)
                    {
                        query = db.HistorialDotacion.FirstOrDefault(e => e.Nro == nro && e.EmpleadoId == EmpleadoId);

                        if (query != null)
                        {
                            int.TryParse(Cantidad, out int cant);
                            db.HistorialDotacion.Attach(query);
                            query.Recibido = true;
                            query.FechaRecibido = DateTime.Now;
                            query.CantidadEntregada = cant;
                            query.CantidadPendiente -= cant;
                            //db.SaveChanges();
                            EnvioCorreoDotacionFirmada(EmpleadoId);
                            message = "true";
                        }
                        else
                        {
                            return "El QR escaneado no coincide con el empleado seleccionado.";
                        }
                    }
                    else
                    {
                        return "El empleado no existe";
                    }
                }
                catch (Exception ex)
                {
                    message = "Error" + ex;
                    return message;
                }
                return message;
            }
        }

        public string ComprobarDotacion(int Id)
        {
            try
            {
                var emp = db.Dotacion.FirstOrDefault(x=> x.EmpleadoId == Id && x.Fecha < DateTime.Now);
                var area = "false";
                if (emp != null)
                {

                    var tipoarea = db.Empleados.Find(Id);
                    
                    if (tipoarea.TipoArea == "Asistenciales CO")
                    {
                        area = "asi";
                    }
                    else if (tipoarea.TipoArea == "Administrativos CO")
                    {
                        area = "adm";
                    }
                }

                return area;
            }
            catch
            {
                return "error";
            }
        }

        public DotacionDTO ObtenerTallas(int Id)
        {
            DotacionDTO tallas = new DotacionDTO();

            var emp = db.Dotacion.FirstOrDefault(x => x.EmpleadoId == Id);

            if (emp != null)
            {
                var talla = emp.Tallas;

                if (!string.IsNullOrEmpty(talla))
                {
                    string[] numeros = talla.Split(',');

                    if (numeros.Length >= 2 && numeros.Length <= 3)
                    {
                        tallas.Id = emp.Id;
                        tallas.Camisa = numeros[0];
                        tallas.Pantalon = numeros[1];

                        if (numeros.Length == 3)
                        {
                            tallas.Zapatos = numeros[2];
                        }
                    }
                }
                else
                {
                    tallas.Id = emp.Id;
                    tallas.Camisa = "";
                    tallas.Pantalon = "";
                    tallas.Zapatos = "";
                }
            }
        
            return tallas;
        }

        public bool GuardarTallas(DotacionDTO model)
        {
            try
            {
                var dotacion = db.Dotacion.Find(model.Id);

                if (dotacion != null)
                {
                    if (string.IsNullOrEmpty(model.Zapatos))
                    {
                        dotacion.Tallas = model.Camisa + "," + model.Pantalon;
                    }
                    else
                    {
                        dotacion.Tallas = model.Camisa + "," + model.Pantalon + "," + model.Zapatos;
                    }
                    db.SaveChanges();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
