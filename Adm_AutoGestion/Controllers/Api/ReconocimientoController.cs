using Adm_AutoGestion.Models;
using Adm_AutoGestion.Models.EvaDesempeno;
using Adm_AutoGestion.Services;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;


namespace Adm_AutoGestion.Controllers.Api
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReconocimientoController : ApiController
    {
        private AutogestionContext db = new AutogestionContext();

        private ReconocimientoRepository _repo;
        private ServiciosRepository _servicios = new ServiciosRepository();

        public ReconocimientoController()
        {
            _repo = new ReconocimientoRepository();
        }

        [HttpPost]
        [Route("api/Reconocimiento/GuardarReconocimiento/{Reconocimiento}")]
        public IHttpActionResult GuardarReconocimiento(Reconocimiento Reconocimiento)
        {
            var respuesta = "";
            try
            {

                _repo.Crear(Reconocimiento);
                respuesta = "El Proceso fue Exitoso";

            }
            catch (Exception ex)
            {

                Console.WriteLine("Error al crear:" + ex);
                respuesta = "Error al crear";
            }



            return Json(new { respuesta });


        }

        [HttpGet]
        [Route("api/Reconocimiento/TipoReconocimiento")]
        public IHttpActionResult TipoReconocimiento()
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    List<TipoReconocimiento> Datos = db.TipoReconocimiento.ToList();
                    return Json(Datos);
                }
                catch (Exception ex)
                {
                    return Json("Error: " + ex);
                }
            }
        }
        [HttpGet]
        [Route("api/Reconocimiento/ListaEmpleados")]
        public IHttpActionResult ListaEmpleados()
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    List<Empleado> Datos = db.Empleados.Where(f => f.Activo != "NO").ToList();
                    return Json(Datos);
                }
                catch (Exception ex)
                {
                    return Json("Error: " + ex);
                }
            }
        }
        [HttpGet]

        [Route("api/Reconocimiento/GetNotifications/{empleadoid}")]

        public async Task<List<Reconocimiento>> GetNotifications(int empleadoid)
        {
            List<Reconocimiento> Datos = new List<Reconocimiento>();
            using (var db = new AutogestionContext())
            {
                Datos = await db.Reconocimientos.Where(x => x.EmpleadoReconocidoId == empleadoid && x.Visto == true).ToListAsync();
                foreach (Reconocimiento Reconocimiento in Datos)
                {
                    TipoReconocimiento Tipo = db.TipoReconocimiento.Where(e => e.Id == Reconocimiento.TipoReconocimientoId).FirstOrDefault();
                    Empleado Emp = db.Empleados.Where(e => e.Id == Reconocimiento.EmpleadoId).FirstOrDefault();
                    Reconocimiento.TipoNombre = Tipo.Nombre;
                    Reconocimiento.TipoImagen = Tipo.Imagen;
                    Reconocimiento.TipoTexto = Tipo.Texto;
                    Reconocimiento.EmpleadoNombre = Emp.Nombres;
                    //HorasExtra.EstadosHorasExtra = db.EstadosHorasExtra.FirstOrDefault(e => e.Id == HorasExtra.Estado);
                }

            }
            return Datos;
        }
        [HttpPost]
        [Route("api/Reconocimiento/obtenerimagen/{id}")]
        public String obtenerimagen(string id)
        {
            try
            {
                // Intenta leer la imagen del servidor de archivos
                Byte[] b = null;
                try
                {
                    b = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/AnexosReconocimiento/" + id + ".png"));
                }
                catch
                {
                    try
                    {
                        b = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/AnexosReconocimiento/" + id + ".jpg"));
                    }
                    catch
                    {
                        try
                        {
                            b = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/AnexosReconocimiento/" + id + ".jpeg"));
                        }
                        catch
                        {
                            // Manejo de errores si la imagen no puede ser encontrada
                        }

                    }

                }

                // Convierte la imagen a base64
                return Convert.ToBase64String(b);
            }
            catch (Exception e)
            {
                return "error";
            }
        }
        [HttpPost]
        [Route("api/Reconocimiento/obtenerfondo/{id}")]
        public String obtenerfondo(string id)
        {
            try
            {
                // Intenta leer la imagen del servidor de archivos
                Byte[] b = null;
                try
                {
                    b = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/AnexosReconocimiento/" + id + ".png"));
                }
                catch
                {
                    try
                    {
                        b = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/AnexosReconocimiento/" + id + ".jpg"));
                    }
                    catch
                    {
                        try
                        {
                            b = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/AnexosReconocimiento/" + id + ".jpeg"));
                        }
                        catch
                        {
                            // Manejo de errores si la imagen no puede ser encontrada
                        }

                    }

                }

                // Convierte la imagen a base64
                return Convert.ToBase64String(b);
            }
            catch (Exception e)
            {
                return "error";
            }
        }

        [HttpGet]
        [Route("api/Reconocimiento/ConsultaReconocimiento/{Id}")]
        public IHttpActionResult Get(string Id)
        {


            List<Reconocimiento> Datos = new List<Reconocimiento>();

            int Emp = Convert.ToInt32(Id);
            using (var db = new AutogestionContext())
            {
                Datos = db.Reconocimientos.Where(e => e.EmpleadoReconocidoId == Emp && e.Visto == true).ToList();
                

            }

            return Json(Datos.Any());


        }
        [HttpGet]
        [Route("api/Reconocimiento/HistoricoReconocimiento/{Id}")]
        public IHttpActionResult HistoricoReconocimiento(string Id)
        {


            List<Reconocimiento> Datos = new List<Reconocimiento>();

            int Emp = Convert.ToInt32(Id);
            using (var db = new AutogestionContext())
            {
                Datos = db.Reconocimientos.Where(e => e.EmpleadoReconocidoId == Emp && e.Activo == true).ToList();
                foreach (Reconocimiento Reconocimiento in Datos)
                {
                    TipoReconocimiento Tipo = db.TipoReconocimiento.Where(e => e.Id == Reconocimiento.TipoReconocimientoId).FirstOrDefault();
                    Empleado Empleado = db.Empleados.Where(e => e.Id == Reconocimiento.EmpleadoId).FirstOrDefault();
                    Reconocimiento.TipoNombre = Tipo.Nombre;
                    Reconocimiento.TipoImagen = Tipo.Imagen;
                    Reconocimiento.TipoTexto = Tipo.Texto + " " + Reconocimiento.Observaciones;
                    Reconocimiento.EmpleadoNombre = Empleado.Nombres;
                    //HorasExtra.EstadosHorasExtra = db.EstadosHorasExtra.FirstOrDefault(e => e.Id == HorasExtra.Estado);
                }

            }

            return Json(Datos.OrderByDescending(a => a.Id));


        }
        [HttpGet]
        [Route("api/Reconocimiento/ReconocimientosUltimoMes")]
        public IHttpActionResult ReconocimientosUltimoMes()
        {


            List<Reconocimiento> Datos = new List<Reconocimiento>();

            
            using (var db = new AutogestionContext())
            {
                DateTime fechaInicio = DateTime.Now.AddMonths(-1);
                Datos = db.Reconocimientos.Where(r => r.Fecha >= fechaInicio && r.Activo == true).ToList();
                foreach (Reconocimiento Reconocimiento in Datos)
                {
                    TipoReconocimiento Tipo = db.TipoReconocimiento.Where(e => e.Id == Reconocimiento.TipoReconocimientoId).FirstOrDefault();
                    Empleado Empleado = db.Empleados.Where(e => e.Id == Reconocimiento.EmpleadoId).FirstOrDefault();
                    Empleado EmpReco = db.Empleados.Where(e => e.Id == Reconocimiento.EmpleadoReconocidoId).FirstOrDefault();
                    Reconocimiento.TipoNombre = Tipo.Nombre;
                    Reconocimiento.TipoImagen = Tipo.Imagen;
                    Reconocimiento.TipoTexto = Tipo.Texto + " " + Reconocimiento.Observaciones;
                    Reconocimiento.EmpleadoNombre = Empleado.Nombres;
                    Reconocimiento.EmpReco = EmpReco.Nombres;
                    //HorasExtra.EstadosHorasExtra = db.EstadosHorasExtra.FirstOrDefault(e => e.Id == HorasExtra.Estado);
                }

            }

            return Json(Datos.OrderByDescending(a => a.Id));


        }
        [HttpDelete]
        [Route("api/Reconocimiento/DeleteReconocimiento/{id}")]
        public async Task<bool> DeleteReconocimiento(int id)
        {
            var reconocimiento = await _repo.ModificarVisto(id);

            return reconocimiento;
        }
    }
}