using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;


namespace Adm_AutoGestion.Controllers.Api
{
    //habilitar acceso desde todos los origines, pero aca debemos poner las iP'S que pueden acceder. solo las internas
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HorasExtraController : ApiController
    {
        private AutogestionContext db = new AutogestionContext();

        private HorasExtraRepository _repo;
        private ServiciosRepository _servicios = new ServiciosRepository();

        public HorasExtraController()
        {
            _repo = new HorasExtraRepository();
        }

        public class ContructorHorasExtras
        {
            public bool HayRegistroValido { get; set; }
            public float HorasdiaunoDB { get; set; }
            public float HorasdiaPosterior { get; set; }
            public string Error { get; set; }

        }

        [HttpPost]
        [Route("api/HorasExtra/GuardarHorasExtra/{HorasExtra}")]
        public IHttpActionResult GuardarHorasExtra(HorasExtra HorasExtra)
        {
            var respuesta = "";
            try
            {

                _repo.Crear(HorasExtra);
                respuesta = "El Proceso fue Exitoso";

            }
            catch (Exception ex)
            {

                Console.WriteLine("Error al crear:" + ex);
                respuesta = "Error al crear";
            }



            return Json(new { IdGenerado = HorasExtra.Id });


        }






        [HttpPost]
        [Route("api/HorasExtra/GuardarDetalleHorasExtra")]
        public IHttpActionResult GuardarDetalleHorasExtra(List<DetalleHorasExtra> detallesHorasExtra)
        {
            var respuesta = "";
            try
            {
                foreach(DetalleHorasExtra item in detallesHorasExtra) 
                {
                    item.Id = 0;
                    
                }
                _repo.CrearDetalleHorasExtra(detallesHorasExtra);
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
        [Route("api/HorasExtra/ValidarRegistro/{empleadoId}")]
        public IHttpActionResult ValidarRegistro(int empleadoId, string fecha1, string fecha2)
        {
            try
            {
                DateTime fechaInicio = DateTime.ParseExact(fecha1, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                DateTime fechaFin = DateTime.ParseExact(fecha2, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                float horasdiauno = 0; // HORAS REGISTRO ACTUAL
                float horasdiados = 0; // HORAS REGISTRO UN DIA DESPUES DEL ACTUAL
                using (var db = new AutogestionContext())
                {
                    var idsHorasExtra = db.HorasExtra
                .Where(he => he.EmpleadoId == empleadoId && he.Estado != 4)
                .Select(he => he.Id  )
                .ToList();

                    if ((fechaFin.TimeOfDay <= fechaInicio.TimeOfDay && (fechaFin.Hour == 0 && fechaFin.Minute != 0)) || (fechaFin.TimeOfDay <= fechaInicio.TimeOfDay && (fechaFin.Hour != 0)))
                    {
                        //VALIDACIONES CUANDO HAY APROXIMACION DEL DIA SIGUIENTE

                        DateTime fechaFin2 = fechaInicio.Date.AddDays(1) + fechaFin.TimeOfDay;
                        DateTime hora1 = fechaInicio.Date.AddHours(23).AddMinutes(59).AddSeconds(0);
                        DateTime hora2 = fechaFin2.Date.AddHours(0).AddMinutes(0).AddSeconds(0);

                        var fechaAnterior = fechaInicio.Date.AddDays(-1);
                        var registrosAnteriores = db.DetalleHorasExtras
                            .Where(d => idsHorasExtra.Contains(d.HorasExtraId) && d.Fecha == fechaAnterior)
                            .ToList();
                        var fechaPosterior = fechaInicio.Date.AddDays(+1);
                        var registrosPosteriores = db.DetalleHorasExtras
                            .Where(d => idsHorasExtra.Contains(d.HorasExtraId) && d.Fecha == fechaPosterior)
                            .ToList();
                        var registroSeleccionado = db.DetalleHorasExtras
                            .Where(d => idsHorasExtra.Contains(d.HorasExtraId) && d.Fecha == fechaInicio.Date)
                            .ToList();
                        bool hayRegistroValido = false;
                        float horasdia = 0;

                        // float sumaTotalAnterior = 0; // Cantidad de horas Dia Anterior (El valor no se tiene en cuenta)
                        float sumaTotal = 0; // Cantidad de horas del Dia Seleccionado DB 
                        float sumaTotalDespues = 0; // Cantidad de horas del Dia Posterior DB
                        // VALIDAR REGISTROS ANTERIORES A LA FECHA DEL REGISTRO ACTUAL
                        if (!hayRegistroValido && registrosAnteriores.Any())
                        {
                            foreach (var registro in registrosAnteriores)
                            {
                                TimeSpan horaDesde = TimeSpan.ParseExact(registro.HoraDesde, "hh\\:mm", CultureInfo.InvariantCulture);
                                TimeSpan horaHasta = TimeSpan.ParseExact(registro.HoraHasta, "hh\\:mm", CultureInfo.InvariantCulture);
                                DateTime fechaHoraDesde = registro.Fecha.Date + horaDesde;

                                if (horaHasta < horaDesde)
                                {
                                    fechaHoraDesde = fechaHoraDesde.AddDays(1);
                                    DateTime fechaHoraHasta = registro.Fecha.Date + horaHasta;
                                    TimeSpan medianoche = fechaHoraDesde.Date.AddHours(24) - fechaHoraDesde.Date;
                                    TimeSpan diferenciaDesdeMedianoche = medianoche - horaDesde;
                                    float horasAntesMedianoche = Math.Abs((float)diferenciaDesdeMedianoche.TotalHours);
                                    float horasDespuesMedianoche = (float)(fechaHoraHasta - fechaHoraHasta.Date).TotalHours;
                                    sumaTotal = sumaTotal + horasDespuesMedianoche;
                                    if (horasDespuesMedianoche > 2 || sumaTotal > 2)
                                    {
                                        hayRegistroValido = true;
                                    }
                                }
                            }
                        }
                        // VALIDAR REGISTROS DE LA FECHA DEL REGISTRO ACTUAL
                        if (!hayRegistroValido && registroSeleccionado.Any())
                        {
                            foreach (var registro in registroSeleccionado)
                            {
                                TimeSpan horaDesde = TimeSpan.ParseExact(registro.HoraDesde, "hh\\:mm", CultureInfo.InvariantCulture);
                                TimeSpan horaHasta = TimeSpan.ParseExact(registro.HoraHasta, "hh\\:mm", CultureInfo.InvariantCulture);
                                DateTime fechaHoraDesde = registro.Fecha.Date + horaDesde;

                                if (horaHasta < horaDesde)
                                {
                                    fechaHoraDesde = fechaHoraDesde.AddDays(1);
                                    DateTime fechaHoraHasta = registro.Fecha.Date + horaHasta;
                                    TimeSpan medianoche = fechaHoraDesde.Date.AddHours(24) - fechaHoraDesde.Date;
                                    TimeSpan diferenciaDesdeMedianoche = medianoche - horaDesde;
                                    float horasAntesMedianoche = Math.Abs((float)diferenciaDesdeMedianoche.TotalHours);
                                    float horasDespuesMedianoche = (float)(fechaHoraHasta - fechaHoraHasta.Date).TotalHours;
                                    sumaTotal = sumaTotal + horasAntesMedianoche;
                                    if (horasAntesMedianoche > 2 || sumaTotal > 2)
                                    {
                                        hayRegistroValido = true;
                                    }
                                }
                                else
                                {
                                    DateTime HoraDesde = registro.Fecha.Date + horaDesde;
                                    DateTime HoraHasta = registro.Fecha.Date + horaHasta;
                                    while (HoraDesde < HoraHasta)
                                    {
                                        horasdia = horasdia + 0.5f;
                                        HoraDesde = HoraDesde.AddMinutes(30);
                                    }
                                    if (sumaTotal > 2)
                                    {
                                        hayRegistroValido = true;
                                    }
                                }
                            }
                        }
                        // VALIDAR REGISTROS POSTERIORES A LA FECHA DEL REGISTRO ACTUAL
                        if (!hayRegistroValido && registrosPosteriores.Any())
                        {

                            foreach (var registro in registrosPosteriores)
                            {
                                TimeSpan horaDesde = TimeSpan.ParseExact(registro.HoraDesde, "hh\\:mm", CultureInfo.InvariantCulture);
                                TimeSpan horaHasta = TimeSpan.ParseExact(registro.HoraHasta, "hh\\:mm", CultureInfo.InvariantCulture);
                                DateTime fechaHoraDesde = registro.Fecha.Date + horaDesde;

                                if (horaHasta < horaDesde)
                                {
                                    fechaHoraDesde = fechaHoraDesde.AddDays(1);
                                    DateTime fechaHoraHasta = registro.Fecha.Date + horaHasta;
                                    TimeSpan medianoche = fechaHoraDesde.Date.AddHours(24) - fechaHoraDesde.Date;
                                    TimeSpan diferenciaDesdeMedianoche = medianoche - horaDesde;
                                    float horasAntesMedianoche = Math.Abs((float)diferenciaDesdeMedianoche.TotalHours);
                                    float horasDespuesMedianoche = (float)(fechaHoraHasta - fechaHoraHasta.Date).TotalHours;
                                    sumaTotalDespues = sumaTotalDespues + horasAntesMedianoche;
                                    if (horasAntesMedianoche > 2 || sumaTotalDespues > 2)
                                    {
                                        hayRegistroValido = true;
                                    }
                                }
                                else
                                {
                                    DateTime HoraDesde = registro.Fecha.Date + horaDesde;
                                    DateTime HoraHasta = registro.Fecha.Date + horaHasta;
                                    while (HoraDesde < HoraHasta)
                                    {
                                        sumaTotalDespues = sumaTotalDespues + 0.5f;
                                        HoraDesde = HoraDesde.AddMinutes(30);
                                    }
                                    if (sumaTotalDespues > 2)
                                    {
                                        hayRegistroValido = true;
                                    }
                                }
                            }
                        }
                        // ASIGNACIÓN HORAS DEL REGISTRO ACTUAL
                        while (fechaInicio < hora1)
                        {
                            hora1 = hora1.AddMinutes(-30);
                            horasdiauno = horasdiauno + 0.5f;
                        }
                        while (hora2 < fechaFin2)
                        {
                            hora2 = hora2.AddMinutes(30);
                            horasdiados = horasdiados + 0.5f;
                        }

                        //-------- SUMA TOTAL HORAS DE LOS REGRISTROS ANTERIORES Y EL REGISTRO ACTUAL----------//                
                        sumaTotal = sumaTotal + horasdiauno;
                        if (sumaTotal > 2)
                        {
                            hayRegistroValido = true;
                        }

                        sumaTotalDespues = sumaTotalDespues + horasdiados;
                        if (sumaTotalDespues > 2)
                        {
                            hayRegistroValido = true;
                        }
                        ContructorHorasExtras ResultadoFinal = new ContructorHorasExtras();
                        ResultadoFinal.HorasdiaunoDB = sumaTotal; //TOTAL HORAS DEL DIA SELECCIONADO + DB 
                        ResultadoFinal.HayRegistroValido = hayRegistroValido;
                        ResultadoFinal.HorasdiaPosterior = sumaTotalDespues; // TOTAL HORAS DEL DIA POSTERIOR 

                        return Ok(ResultadoFinal);

                    }
                    else
                    {
                        //VALIDACIONES CUANDO NO HAY APROXIMACION DEL DIA SIGUIENTE

                        var fechaAnterior = fechaInicio.Date.AddDays(-1);
                        var registrosAnteriores = db.DetalleHorasExtras
                            .Where(d => idsHorasExtra.Contains(d.HorasExtraId) && d.Fecha == fechaAnterior)
                            .ToList();
                        var registroSeleccionado = db.DetalleHorasExtras
                            .Where(d => idsHorasExtra.Contains(d.HorasExtraId) && d.Fecha == fechaInicio.Date)
                            .ToList();
                        bool hayRegistroValido = false;
                        float sumaTotal = 0;
                        // VALIDAR REGISTROS ANTERIORES A LA FECHA DEL REGISTRO ACTUAL
                        if (!hayRegistroValido && registrosAnteriores.Any())
                        {

                            foreach (var registro in registrosAnteriores)
                            {
                                TimeSpan horaDesde = TimeSpan.ParseExact(registro.HoraDesde, "hh\\:mm", CultureInfo.InvariantCulture);
                                TimeSpan horaHasta = TimeSpan.ParseExact(registro.HoraHasta, "hh\\:mm", CultureInfo.InvariantCulture);
                                DateTime fechaHoraDesde = registro.Fecha.Date + horaDesde;
                                if (horaHasta < horaDesde)
                                {
                                    fechaHoraDesde = fechaHoraDesde.AddDays(1);
                                    DateTime fechaHoraHasta = registro.Fecha.Date + horaHasta;
                                    TimeSpan medianoche = fechaHoraDesde.Date.AddHours(24) - fechaHoraDesde.Date;
                                    TimeSpan diferenciaDesdeMedianoche = medianoche - horaDesde;
                                    float horasAntesMedianoche = Math.Abs((float)diferenciaDesdeMedianoche.TotalHours);
                                    float horasDespuesMedianoche = (float)(fechaHoraHasta - fechaHoraHasta.Date).TotalHours;
                                    sumaTotal = sumaTotal + horasDespuesMedianoche;
                                    if (horasDespuesMedianoche > 2 || sumaTotal > 2)
                                    {
                                        hayRegistroValido = true;
                                    }
                                }
                            }
                        }
                        // VALIDAR REGISTROS DE LA FECHA DEL REGISTRO ACTUAL
                        if (!hayRegistroValido && registroSeleccionado.Any())
                        {

                            foreach (var registro in registroSeleccionado)
                            {
                                TimeSpan horaDesde = TimeSpan.ParseExact(registro.HoraDesde, "hh\\:mm", CultureInfo.InvariantCulture);
                                TimeSpan horaHasta = TimeSpan.ParseExact(registro.HoraHasta, "hh\\:mm", CultureInfo.InvariantCulture);


                                DateTime fechaHoraDesde = registro.Fecha.Date + horaDesde;


                                if (horaHasta < horaDesde)
                                {

                                    fechaHoraDesde = fechaHoraDesde.AddDays(1);
                                    DateTime fechaHoraHasta = registro.Fecha.Date + horaHasta;
                                    TimeSpan medianoche = fechaHoraDesde.Date.AddHours(24) - fechaHoraDesde.Date;
                                    TimeSpan diferenciaDesdeMedianoche = medianoche - horaDesde;
                                    float horasAntesMedianoche = Math.Abs((float)diferenciaDesdeMedianoche.TotalHours);
                                    float horasDespuesMedianoche = (float)(fechaHoraHasta - fechaHoraHasta.Date).TotalHours;
                                    sumaTotal = sumaTotal + horasAntesMedianoche;
                                    if (horasAntesMedianoche > 2 || sumaTotal > 2)
                                    {
                                        hayRegistroValido = true;
                                    }

                                }
                                else
                                {
                                    DateTime HoraDesde = fechaInicio.Date + horaDesde;
                                    DateTime HoraHasta = fechaInicio.Date + horaHasta;
                                    while (HoraDesde < HoraHasta)
                                    {
                                        sumaTotal = sumaTotal + 0.5f;
                                        HoraDesde = HoraDesde.AddMinutes(30);
                                    }
                                    if (sumaTotal >= 2)
                                    {
                                        hayRegistroValido = true;
                                    }
                                }
                            }

                        }


                        while (fechaInicio < fechaFin)
                        {
                            horasdiauno = horasdiauno + 0.5f;
                            fechaInicio = fechaInicio.AddMinutes(30);
                        }

                        // SUMA TOTAL DE LAS HORAS DEL REGISTRO ACTUAL Y LA BASE DE DATOS
                        sumaTotal = sumaTotal + horasdiauno;
                        if (sumaTotal > 2)
                        {
                            hayRegistroValido = true;
                        }
                        ContructorHorasExtras ResultadoFinal = new ContructorHorasExtras();
                        ResultadoFinal.HorasdiaunoDB = sumaTotal; //TOTAL HORAS DEL DIA SELECCIONADO + DB 
                        ResultadoFinal.HayRegistroValido = hayRegistroValido;
                        ResultadoFinal.HorasdiaPosterior = 0; // NO APLCIA

                        return Ok(ResultadoFinal);
                    }
                }
            }
            catch (Exception ex)
            {
                ContructorHorasExtras ResultadoFinal = new ContructorHorasExtras();
                ResultadoFinal.HorasdiaunoDB = 0; //TOTAL HORAS DEL DIA SELECCIONADO + DB 
                ResultadoFinal.HayRegistroValido = false;
                ResultadoFinal.HorasdiaPosterior = 0; // NO APLCIA
                ResultadoFinal.Error = ex.Message;
                return InternalServerError(ex);
            }
        }



        public class InformeTotalHE
        {
            public int Id { get; set; }
            public int EmpleadoId { get; set; }
            public Empleado Empleado { get; set; }
            public HorasExtra HoraExtra { get; set; }
            public DateTime FechaHora { get; set; }
            public string HoraDesde { get; set; }
            public string HoraHasta { get; set; }
            public string ObservacionesMotivo { get; set; }
            public float? LiquidacionDiurna { get; set; }
            public float? LiquidacionNocturna { get; set; }
            public float? LiquidacionDiurnaFestivo { get; set; }
            public float? LiquidacionNocturnaFestivo { get; set; }
            public MotivoTrabajoHE MotivoTrabajoHE { get; set; }
            public float? TotalHoras { get; set; }
            public string MotivoNombre { get; set; }
            public DateTime FechadeRegistro { get; set; }
            public int Estado { get; set; }
            public DateTime? FechaPago { get; set; }
            public EstadosHorasExtra EstadosHorasExtra { get; set; }
            public string EstadoNombre { get; set; }
            public string Observacion { get; set; }


        }



        [HttpGet]
        [Route("api/HorasExtra/ConsultaHorasExtra/{Id}")]
        public IHttpActionResult Get(string Id)
        {

            int Emp = Convert.ToInt32(Id);
            List<InformeTotalHE> ListadoFinal = new List<InformeTotalHE>();

            using (var db = new AutogestionContext())
            {

                ListadoFinal = db.HorasExtra
                             .Join(db.DetalleHorasExtras,
                                Encabezado => Encabezado.Id,
                                Detallado => Detallado.HorasExtraId,
                                (Encabezado, Detallado) => new { Encabezado, Detallado })
                              .Join(db.EstadosHorasExtra,
                              EncabezadoDetallado => EncabezadoDetallado.Encabezado.Estado,
                                Estado => Estado.Id,
                                (EncabezadoDetallado, Estado) => new { EncabezadoDetallado, Estado }) // Combinar los resultados// Agregar el JOIN con la tabla Estados
                              .Where(x => x.EncabezadoDetallado.Encabezado.EmpleadoId == Emp)
                             .Select(x =>
                                    new InformeTotalHE
                                    {
                                        Id = x.EncabezadoDetallado.Encabezado.Id,
                                        FechadeRegistro = x.EncabezadoDetallado.Encabezado.FechaDeRegistro,
                                        FechaHora = x.EncabezadoDetallado.Detallado.Fecha,
                                        EmpleadoId = x.EncabezadoDetallado.Encabezado.EmpleadoId,
                                        HoraDesde = x.EncabezadoDetallado.Detallado.HoraDesde,
                                        HoraHasta = x.EncabezadoDetallado.Detallado.HoraHasta,
                                        LiquidacionDiurna = x.EncabezadoDetallado.Detallado.LiquidacionDiurna,
                                        LiquidacionDiurnaFestivo = x.EncabezadoDetallado.Detallado.LiquidacionDiurnaFestivo,
                                        LiquidacionNocturna = x.EncabezadoDetallado.Detallado.LiquidacionNocturna,
                                        LiquidacionNocturnaFestivo = x.EncabezadoDetallado.Detallado.LiquidacionNocturnaFestivo,
                                        TotalHoras = x.EncabezadoDetallado.Detallado.TotalHoras,
                                        Estado = x.EncabezadoDetallado.Encabezado.Estado,
                                        FechaPago = x.EncabezadoDetallado.Encabezado.FechaPago,
                                        EstadoNombre = x.Estado.Nombre

                                    }).ToList();

                foreach (InformeTotalHE item in ListadoFinal) { 
                    var observaciones = db.HistoricoHorasExtra
                    .Where(x => x.HorasExtraId == item.Id && x.EstadoNombre == "Rechazado").FirstOrDefault();

                    if (observaciones != null)
                    {
                        item.Observacion = observaciones.Observaciones;
                    }
                    else
                    {
                        // Manejar el caso donde no se encuentra una observación
                        item.Observacion = ""; // O algún otro valor por defecto
                    }

                }



        }

            //List<HorasExtra> Datos = new List<HorasExtra>();

            //int Emp = Convert.ToInt32(Id);
            //using (var db = new AutogestionContext())
            //{
            //    Datos = db.HorasExtra.Where(e => e.EmpleadoId == Emp).ToList();
            //    foreach (HorasExtra HorasExtra in Datos)
            //    {
            //        var Estado = db.EstadosHorasExtra.Where(e => e.Id == HorasExtra.Estado).FirstOrDefault();
            //        HorasExtra.EstadoNombre = Estado.Nombre;

            //    }

            //}

            return Json(ListadoFinal.OrderByDescending(a => a.FechaHora));


        }

        [HttpGet]
        [Route("api/HorasExtra/ObtenerUnidadOrganizativa/{Id}")]
        public IHttpActionResult ObtenerUnidadOrganizativa(int Id)
        {
            using (var db = new AutogestionContext())
            {
                
                var empleado = db.Empleados.FirstOrDefault(e => e.Id == Id);
                var UnidadOrganizativa = empleado.UnidadOrganizativa;


                return Ok(UnidadOrganizativa);
            }
        }

        [HttpGet]
        [Route("api/HorasExtra/DetalleHorasExtra/{Id}")]
        public IHttpActionResult GetDetalleHorasExtra(int Id)
        {
            List<DetalleHorasExtra> detalleHorasExtra = new List<DetalleHorasExtra>();
            try
            {
                using (var db = new AutogestionContext())
                {

                    detalleHorasExtra = db.DetalleHorasExtras
    .Where(d => d.HorasExtraId == Id)
    .ToList();
                    foreach (DetalleHorasExtra DetalleHorasExtra in detalleHorasExtra)
                    {
                        var MotivoTrabajoHEId = db.MotivoTrabajoHE.Where(e => e.Id == DetalleHorasExtra.MotivoTrabajoHEId).FirstOrDefault();
                        DetalleHorasExtra.MotivoNombre = MotivoTrabajoHEId.Descripcion;
                        //HorasExtra.EstadosHorasExtra = db.EstadosHorasExtra.FirstOrDefault(e => e.Id == HorasExtra.Estado);
                    }
                    if (detalleHorasExtra != null)
                    {

                        return Json(detalleHorasExtra);

                    }
                    else
                    {
                        
                        return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/HorasExtra/ValidarSena/{Id}")]
        public IHttpActionResult ValidarSena(int Id)
        {
            
            try
            {
                using (var db = new AutogestionContext())
                {


                    bool esEmpleadoSENA = db.Empleados.Any(d => d.Id == Id && (d.TipoArea == "SENA Pctivo/Pasante" || d.TipoArea == "SENA Lectivo") && d.Activo != "NO");


                    return Ok(esEmpleadoSENA);

                }
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/HorasExtra/LiquidacionDiurna/{parametro}")]
        public IHttpActionResult LiquidacionDiurna(string parametro)
        {
            try
            {
                var config = db.Configuraciones.FirstOrDefault(x => x.Parametro == parametro);

                if (config != null)
                {
                    
                    string[] partes = config.Valor.Split('@');

                    
                    TimeSpan horaInicio = TimeSpan.Parse(partes[0]);
                    TimeSpan horaFin = TimeSpan.Parse(partes[1]);

                    
                    return Ok(new { HoraInicio = horaInicio, HoraFin = horaFin });
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        [Route("api/HorasExtra/AreasHE/{parametro}")]
        public IHttpActionResult AreasHE(string parametro)
        {
            try
            {
                var config = db.Configuraciones.FirstOrDefault(x => x.Parametro == parametro);

                if (config != null)
                {
                    string[] codigos = config.Valor.Split(',');

                    return Ok(codigos);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/HorasExtra/DiasFestivos")]
        public IHttpActionResult DiasFestivos()
        {
            try
            {
                
                int Actual = DateTime.Now.Year;
                int Anterior = Actual -1;

                var diasFestivos = db.DiasFestivos.Where(x => x.festivo.Year == Actual ||  x.festivo.Year == Anterior).ToList();

                return Ok(diasFestivos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }




        [HttpGet]
        [Route("api/HorasExtra/MotivoTrabajoHE")]
        public IHttpActionResult MotivoTrabajoHE()
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    List<MotivoTrabajoHE> Datos = db.MotivoTrabajoHE.ToList();
                    return Json(Datos);
                }
                catch (Exception ex)
                {
                    return Json("Error: " + ex);
                }
            }
        }
    }
}

