using Adm_AutoGestion.Controllers;
using Adm_AutoGestion.Models;
using Adm_AutoGestion.Models.EvaDesempeno;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Services.Description;

namespace Adm_AutoGestion.Services
{
    public class DetalleCapacitacionRepository
    {

        AutogestionContext db = new AutogestionContext();
        private CapacitacionRepository _repo;

        public DetalleCapacitacionRepository()
        {
            _repo = new CapacitacionRepository();
        }


        internal string Crear(List<DetalleCapacitacion> model, int IdCap, int cantParticipantes, bool esFirmaAbierta)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    DetalleCapacitacion detalle = new DetalleCapacitacion();

                    List<int> empleadosIds = new List<int>();

                    var documento = "";

                    foreach (DetalleCapacitacion Item in model)
                    {
                        db.DetalleCapacitacion.Add(Item);
                        //db.SaveChanges();
                        empleadosIds.Add(Item.EmpleadoId);
                        if (esFirmaAbierta)
                        {
                            documento = db.Empleados.FirstOrDefault(x => x.Id == Item.EmpleadoId).Documento;
                        }
                    }

                    //var relacion = db.Capacitacion.FirstOrDefault(x => x.Id == IdCap)?.IdentificadorRelacion;
                    //var capP = db.Capacitacion.FirstOrDefault(x => x.IdentificadorRelacion == relacion);
                    //capP.TotalPersonas += cantParticipantes;

                    var cap = db.Capacitacion.Find(IdCap);
                    cap.CtnProgramados += cantParticipantes;
                    db.SaveChanges();

                    enviar_correo_inscripcion(IdCap,empleadosIds);

                    if (esFirmaAbierta)
                    {
                        return _repo.FirmaEmpleado(documento, IdCap, "Empleado");
                    }

                    return "true";
                }
                catch (Exception ex)
                {
                    return "Error :" + ex;
                }
            }

        }

        public class Datos
        {
            public string Id { get; set; }
            public string Nombres { get; set; }
        }

        public List<Datos> ObtenerEmpleados(List<string> Area, List<string> Cargo, string Empresa, string TipoArea, List<int> empleadosOmitidos)
        {

            var codigosJefes = db.Empleados.Where(x => !string.IsNullOrEmpty(x.Jefe) && x.Activo == "SI").Select(x => x.Jefe).Distinct().ToList();
            var codigosSuperiores = db.Empleados.Where(x => !string.IsNullOrEmpty(x.Superior) && x.Activo == "SI").Select(x => x.Superior).Distinct().ToList();
            var Codigos = codigosJefes.Union(codigosSuperiores).ToList();

            var registros = db.Empleados.AsQueryable();

            registros = registros.Where(x => x.Activo == "SI");

            if (Area != null)
            {
                registros = registros.Where(e => Area.Contains(e.AreaDescripcion));
            }
            if (Cargo != null)
            {
                registros = registros.Where(e => Cargo.Contains(e.Cargo));
            }
            if (!string.IsNullOrEmpty(Empresa))
            {
                registros = registros.Where(e => e.Empresa == Empresa);
            }
            if (!string.IsNullOrEmpty(TipoArea))
            {
                if (TipoArea == "Jefes")
                {
                    registros = registros.Where(x => Codigos.Contains(x.NroEmpleado));
                }
            }

            registros = registros.Where(e => !empleadosOmitidos.Contains(e.Id));

            var listaRegistros = registros.ToList();

            var empresas = db.Sociedad.ToDictionary(x => x.Codigo, x => x.Descripcion);

            var datosEmpleados = listaRegistros.Select(item => new Datos
            {
                Id = $"{item.Id}%{item.Documento}%{item.AreaDescripcion}%{item.Cargo}%{(empresas.ContainsKey(item.Empresa) ? empresas[item.Empresa] : "Desconocida")}",
                Nombres = item.Nombres
            }).ToList();

            //foreach (var item in listaRegistros)
            //{
            //    var codigoEmpresa = item.Empresa;
            //    var NombreEmpresa = db.Sociedad.FirstOrDefault(x => x.Codigo == codigoEmpresa)?.Descripcion;
            //    datosEmpleados.Add(new Datos
            //    { 
            //        Id = $"{item.Id}%{item.Documento}%{item.AreaDescripcion}%{item.Cargo}%{NombreEmpresa}",
            //        Nombres = item.Nombres
            //    });
            //}

            return datosEmpleados;
        }

        //public List<Datos> ObtenerTerceros(string AreaT, string CargoT, string SociedadT)
        //{

        //    List<Datos> datosTercero = new List<Datos>();

        //    var registros = db.Tercero.Where(x => x.Activo == "SI");

        //    if (!string.IsNullOrEmpty(AreaT))
        //    {
        //        registros = registros.Where(e => e.Area == AreaT);
        //    }
        //    if (!string.IsNullOrEmpty(CargoT))
        //    {
        //        registros = registros.Where(e => e.Cargo == CargoT);
        //    }
        //    if (!string.IsNullOrEmpty(SociedadT))
        //    {
        //        registros = registros.Where(e => e.SociedadCOD == SociedadT);
        //    }

        //    var listaRegistros = registros.ToList();

        //    foreach (var item in listaRegistros)
        //    {
        //        var codigoEmpresa = int.Parse(item.SociedadCOD);
        //        var NombreEmpresa = db.Empresas.FirstOrDefault(x => x.Id == codigoEmpresa)?.Nombre;
        //        datosTercero.Add(new Datos
        //        { 
        //            Id = $"{item.Id}%{item.Documento}%{item.Area}%{item.Cargo}%{NombreEmpresa}",
        //            Nombres = item.Nombres
        //        });
        //    }

        //    return datosTercero;
        //}

        public List<DetalleCapacitacion> ObtenerTodos(int id)
        {
            using (var db = new AutogestionContext())
            {

                List<DetalleCapacitacion> Items = db.DetalleCapacitacion.Where(e => e.CapacitacionId == id && e.Estado != "Anulado").ToList();
                foreach (DetalleCapacitacion Item in Items)
                {
                    if (Item.EsTercero == "NO")
                    {
                        var Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                        var EmpEmpresa = Empleado.Empresa;
                        Item.Sociedad2 = db.Sociedad.FirstOrDefault(x => x.Codigo == EmpEmpresa)?.Descripcion;
                        Item.Empleado2 = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    }
                    else
                    {
                        var TerceroId = Convert.ToInt32(Item.TerceroId);
                        var Tercero = db.Tercero.FirstOrDefault(x => x.Id == TerceroId);
                        Item.Tercero2 = db.Tercero.FirstOrDefault(e => e.Id == TerceroId);

                        if (int.TryParse(Tercero.SociedadCOD, out int TerEmpresa))
                        {
                            Item.NombreEmpresaTercera = db.Empresas.FirstOrDefault(x => x.Id == TerEmpresa)?.Nombre;
                        }
                        else
                        {
                            Item.NombreEmpresaTercera = Tercero.SociedadCOD;
                        }

                        Item.Universidad = db.Tercero.FirstOrDefault(x => x.Id == TerceroId)?.Universidad;
                    }

                }

                return Items;
            }


        }

        internal void modificar(int id, string opcion)
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    DetalleCapacitacion entrega = new DetalleCapacitacion();

                    entrega = db.DetalleCapacitacion.FirstOrDefault(e => e.Id == id);
                    db.DetalleCapacitacion.Attach(entrega);
                    var capid = entrega.CapacitacionId;

                    //if (opcion == "cerrar")
                    //{
                    //    entrega.Estado = "Cerrado";
                    //}

                    if (opcion == "anular")
                    {
                        entrega.Estado = "Anulado";

                        //var relacion = db.Capacitacion.FirstOrDefault(x => x.Id == capid)?.IdentificadorRelacion;
                        //var capP = db.Capacitacion.FirstOrDefault(x => x.IdentificadorRelacion == relacion);
                        //capP.TotalPersonas--;

                        var cap = db.Capacitacion.Find(capid);
                        cap.CtnProgramados--;
                    }


                    db.SaveChanges();

                }
                catch
                {
                }
            }
        }

        public bool enviar_correo_inscripcion(int idCap, List<int> empleadosIds)
        {
            string textocorreo = "";
            bool confirmacion;
            string txtde = Properties.Settings.Default.Correo.ToString();
            string contraseñacorreo = "";
            var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
            contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
            using (var db = new AutogestionContext())
            {
                var configuracion = db.Configuraciones.First(x => x.Parametro == "TXTEMAILINSCRIPCIONCAPACITACION");
                var nombreCap = db.Capacitacion.FirstOrDefault(x => x.Id == idCap)?.Nombre;

                textocorreo = configuracion.Valor;
                textocorreo = textocorreo.Replace("$CAPACITACION", nombreCap);
            }
            try
            {
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage
                {
                    From = new System.Net.Mail.MailAddress(txtde)
                };

                var correos = db.Empleados
                    .Where(x => empleadosIds.Contains(x.Id))
                    .Select(x => new { x.CorreoPersonal, x.Correo })
                    .ToList();

                foreach (var email in correos)
                {
                    if(!string.IsNullOrEmpty(email.CorreoPersonal))
                    {
                        correo.To.Add(email.CorreoPersonal);
                    }

                    if (!string.IsNullOrEmpty(email.Correo))
                    {
                        correo.To.Add(email.Correo);
                    }
                }

                correo.Subject = "Inscripción acción de formación - CORREO DE PRUEBA IGNORAR";
                correo.Body = "" + textocorreo;
                correo.Priority = System.Net.Mail.MailPriority.Normal;
                correo.IsBodyHtml = true;

                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient
                {
                    Host = "smtp-relay.gmail.com",
                    Port = 587,
                    Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo),
                    EnableSsl = true
                };
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