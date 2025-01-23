using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using Autogestion.Shared.DTO.Eventos;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Services.Description;
using System.Windows.Controls;


namespace Adm_AutoGestion.Controllers.Api
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]


    public class EventosController : ApiController
    {
        private AutogestionContext db = new AutogestionContext();
        private EventosRepository _repo = new EventosRepository();


        [HttpGet]
        [Route("api/Eventos/ListarEventosCerrados")]
        public IHttpActionResult EventosCerrados()
        {
            List<Eventos> eventos = new List<Eventos>();

            var anioactual = DateTime.Now.Year;
            var fechaActual = DateTime.Now;
            List<Eventos> eventosporcierre = db.Eventos.Where(e => e.FechaCierre <= fechaActual && e.FechaPublicacion >= new DateTime(anioactual, 1, 1) && e.FechaPublicacion <= new DateTime(anioactual, 12, 31)).ToList();
            List<Eventos> eventosgenerales = db.Eventos.Where(e => e.RegistroRequerido == false && e.FechaFin <= fechaActual && e.FechaPublicacion >= new DateTime(anioactual, 1, 1) && e.FechaPublicacion <= new DateTime(anioactual, 12, 31)).ToList();

            foreach (var i in eventosgenerales)
            {
                var fechaFin = Convert.ToDateTime(i.FechaFin);
                var horaFin = Convert.ToDateTime(i.HoraFin);
                DateTime fechaCombinada = fechaFin.Date.Add(horaFin.TimeOfDay);
                var FinEventoGeneral = fechaCombinada <= fechaActual;

                if (FinEventoGeneral == true)
                {
                    eventos.Add(i);
                }
            }

            foreach (var i in eventosporcierre)
            {
                var fechaCierre = Convert.ToDateTime(i.FechaCierre);
                var horaCierre = Convert.ToDateTime(i.HoraCierre);
                DateTime fechaCombinada = fechaCierre.Date.Add(horaCierre.TimeOfDay);
                var EsFechaLimite = fechaCombinada <= fechaActual;

                if (EsFechaLimite == true)
                {
                    eventos.Add(i);
                }
            }

            //eventos = db.Eventos.Where(e => e.FechaCierre <= fechaActual && e.FechaPublicacion >= new DateTime(anioactual, 1, 1) && e.FechaPublicacion <= new DateTime(anioactual, 12, 31)).ToList();
            return Json(eventos);
        }
        [HttpGet]
        [Route("api/Eventos/ListarEventos/{eventoId}")]
        public IHttpActionResult ListarEventos(int eventoId)
        {
            Eventos evento = new Eventos();
            evento = db.Eventos.Find(eventoId);
            return Json(evento);
        }
        [HttpGet]
        [Route("api/Eventos/EventosRelacionados")]
        public IHttpActionResult EventosRelacionados() {
            List<EventosRelacionados> eventosR = new List<EventosRelacionados>();
            eventosR = db.EventosRelacionados.Where(e => e.EventosId2 != 0).ToList();
            return Json(eventosR);
        }


        [HttpGet]
        [Route("api/Eventos/EventosAgendados/{EmpleadoId}")]
        public IHttpActionResult Get(int EmpleadoId)
        {
            List<EventosDTO> eventosDTOs = new List<EventosDTO>();
            List<Eventos> eventos = new List<Eventos>();

            eventos = db.Eventos.Where(e => e.Estado == "Activo").OrderBy(e=> e.FechaInicio).ToList();

            foreach (var i in eventos)
            {
                var eventosDTO = new EventosDTO();
                var Asistentes = db.DetalleEventos.Where(e => e.EventosId == i.Id).Count();
                i.Cupo -= Asistentes;

                var ConsultarAsistentes = db.DetalleEventos.Where(e => e.EventosId == i.Id && e.FamiliarId != null).Select(e => e.EmpleadoId).ToList();
                var ConsultarEmpInscrito = db.DetalleEventos.Where(e => e.EventosId == i.Id && e.FamiliarId == null).Select(e => e.EmpleadoId).ToList();

                if (i.TipoEvento == "3" && i.EsEventoPrincipal == false)
                {
                    var dato = db.EventosRelacionados.FirstOrDefault(e => e.EventosId2 == i.Id);

                    if (dato.EventosId2 == i.Id)
                    {
                        eventosDTO.EventoRelacionado = dato.EventosId;
                    }
                    else
                    {
                        eventosDTO.EventoRelacionado = 0;
                    }
                }
                else
                {
                    eventosDTO.EventoRelacionado = 0;
                }

                if (ConsultarAsistentes.Contains(EmpleadoId))
                {
                    eventosDTO.Inscrito = true;
                }
                else
                {
                    eventosDTO.Inscrito = false;
                }

                if (ConsultarEmpInscrito.Contains(EmpleadoId))
                {
                    eventosDTO.EmpInscrito = true;
                }
                else
                {
                    eventosDTO.EmpInscrito = false;
                }

                if (i.TipoEvento == "3" && eventosDTO.EmpInscrito == false && i.EsEventoPrincipal == true)
                {
                    List<EventosRelacionados> ER = db.EventosRelacionados.Where(e => e.EventosId == i.Id).ToList();

                    foreach (var item in ER)
                    {
                        var buscarInscritoOtraFecha = db.DetalleEventos.FirstOrDefault(e => e.EventosId == item.EventosId2 && e.EmpleadoId == EmpleadoId && e.FamiliarId == null);

                        if (buscarInscritoOtraFecha != null && buscarInscritoOtraFecha.EventosId != i.Id)
                        {
                            eventosDTO.EstaInscritoOtraFecha = true;
                            break;
                        }
                        else
                        {
                            eventosDTO.EstaInscritoOtraFecha = false;
                        }
                    }
                }
                else if (i.TipoEvento == "3" && eventosDTO.EmpInscrito == false)
                {
                    List<EventosRelacionados> ER = db.EventosRelacionados.Where(e => e.EventosId == eventosDTO.EventoRelacionado).ToList();

                    foreach (var item in ER)
                    {
                        var buscarInscritoOtraFecha = db.DetalleEventos.FirstOrDefault(e => e.EventosId == item.EventosId2 && e.EmpleadoId == EmpleadoId && e.FamiliarId == null);
                        var buscarInscritoEnPrincipal = db.DetalleEventos.FirstOrDefault(e => e.EventosId == eventosDTO.EventoRelacionado && e.EmpleadoId == EmpleadoId && e.FamiliarId == null);
                        if (buscarInscritoOtraFecha != null && buscarInscritoOtraFecha.EventosId != i.Id)
                        {
                            eventosDTO.EstaInscritoOtraFecha = true;
                            break;
                        }
                        else
                        {
                            eventosDTO.EstaInscritoOtraFecha = false;
                        }

                        if (buscarInscritoEnPrincipal != null && buscarInscritoEnPrincipal.EventosId == eventosDTO.EventoRelacionado)
                        {
                            eventosDTO.EstaInscritoOtraFecha = true;
                            break;
                        }
                        else
                        {
                            eventosDTO.EstaInscritoOtraFecha = false;
                        }
                    }
                }

                eventosDTO.Id = i.Id;
                eventosDTO.NombreEvento = i.NombreEvento;
                eventosDTO.TipoEvento = i.TipoEvento;
                eventosDTO.DirigidoA = i.DirigidoA;
                eventosDTO.Cupo = i.Cupo;
                eventosDTO.FechaPublicacion = i.FechaPublicacion;
                eventosDTO.FechaInicio = i.FechaInicio;
                eventosDTO.FechaFin = i.FechaFin;
                eventosDTO.HoraInicio = i.HoraInicio;
                eventosDTO.HoraFin = i.HoraFin;
                eventosDTO.FechaCierre = i.FechaCierre;
                eventosDTO.HoraCierre = i.HoraCierre;
                eventosDTO.RegistroRequerido = i.RegistroRequerido;
                eventosDTO.LinkEncuestaAsistidos = i.LinkEncuestaAsistidos;
                eventosDTO.LinkEncuestaNoAsistidos = i.LinkEncuestaNoAsistidos;
                eventosDTO.Descripcion = i.Descripcion;
                eventosDTO.Imagen = i.Imagen;
                eventosDTO.Estado = i.Estado;
                eventosDTO.ParentescoPermitido = i.ParentescoPermitido;
                eventosDTO.EdadLimite = i.EdadLimite;
                eventosDTO.EsEventoPrincipal = i.EsEventoPrincipal;

                eventosDTOs.Add(eventosDTO);

            }
            return Json(eventosDTOs);
        }

        [HttpGet]
        [Route("api/Eventos/ListarFamiliares/{EmpleadoId}")]
        public IHttpActionResult ListarFamiliares(int EmpleadoId)
        {
            List<Familiar> familiar = new List<Familiar>();
            familiar = db.Familiar.Where(e => e.EmpleadoId == EmpleadoId).ToList();

            return Json(familiar);
        }

        [HttpGet]
        [Route("api/Eventos/ListarEventoFamiliares/{EmpleadoId}/{eventoId}")]
        public IHttpActionResult ListarEventoFamiliares(int EmpleadoId, int eventoId)
        {
            List<EventosDTO> eventosDTOs = new List<EventosDTO>();
            List<FamiliarDTO> familiarDTOs = new List<FamiliarDTO>();
            List<Familiar> familiar = new List<Familiar>();
            Eventos evento = new Eventos();
            evento = db.Eventos.Find(eventoId);

            if (evento.ParentescoPermitido == "NucleoFamiliar" && evento.EdadLimite >= 1)
            {
                familiar = db.Familiar.Where(e => e.EmpleadoId == EmpleadoId && e.Parentesco != "Hermano/a" && e.Parentesco != "Padre/Madre" && e.Edad <= evento.EdadLimite).ToList();
            } else if (evento.ParentescoPermitido == "NucleoFamiliar" && evento.EdadLimite == 0)
            {
                familiar = db.Familiar.Where(e => e.EmpleadoId == EmpleadoId ).ToList();
            } else if (evento.ParentescoPermitido == "Hijos" && evento.EdadLimite >= 1)
            {
                familiar = db.Familiar.Where(e => e.EmpleadoId == EmpleadoId && e.Parentesco == "Hijo/a" && e.Edad <= evento.EdadLimite).ToList();
            } else if (evento.ParentescoPermitido == "Hijos" && evento.EdadLimite == 0)
            {
                familiar = db.Familiar.Where(e => e.EmpleadoId == EmpleadoId && e.Parentesco == "Hijo/a").ToList();
            } else if (evento.ParentescoPermitido == "Todos" && evento.EdadLimite >= 1)
            {
                familiar = db.Familiar.Where(e => e.EmpleadoId == EmpleadoId && e.Edad <= evento.EdadLimite).ToList();
            }
            else
            {
                familiar = db.Familiar.Where(e => e.EmpleadoId == EmpleadoId).ToList();
            }


            foreach (var i in familiar)
            {
                var familiarDTO = new FamiliarDTO();
                var eventosDTO = new EventosDTO();

                var ConsultarAsistentes = db.DetalleEventos.Where(e => e.EventosId == eventoId && e.EmpleadoId == EmpleadoId && e.FamiliarId != null).Select(e => e.FamiliarId).ToList();

                if (evento.TipoEvento == "3" && evento.EsEventoPrincipal == false)
                {
                    var dato = db.EventosRelacionados.FirstOrDefault(e => e.EventosId2 == evento.Id);

                    if (dato.EventosId2 == evento.Id)
                    {
                        familiarDTO.EventoRelacionado = dato.EventosId;
                    }
                    else
                    {
                        familiarDTO.EventoRelacionado = 0;
                    }
                }
                else
                {
                    familiarDTO.EventoRelacionado = 0;
                }

                if (ConsultarAsistentes.Contains(Convert.ToString(i.Id)))
                {
                    familiarDTO.Inscrito = true;
                }
                else
                {
                    familiarDTO.Inscrito = false;
                }

                if (evento.TipoEvento == "3" && familiarDTO.Inscrito == false && evento.EsEventoPrincipal == true)
                {
                    List<EventosRelacionados> ER = db.EventosRelacionados.Where(e => e.EventosId == evento.Id).ToList();

                    foreach (var item in ER)
                    {
                        string familiarId = Convert.ToString(i.Id);
                        var buscarInscritoOtraFecha = db.DetalleEventos.FirstOrDefault(e => e.EventosId == item.EventosId2 && e.EmpleadoId == EmpleadoId && e.FamiliarId == familiarId);

                        if (buscarInscritoOtraFecha != null && buscarInscritoOtraFecha.EventosId != evento.Id)
                        {
                            familiarDTO.EstaInscritoOtraFecha = true;
                            break;
                        }
                        else
                        {
                            familiarDTO.EstaInscritoOtraFecha = false;
                        }
                    }
                }
                else if (evento.TipoEvento == "3" && familiarDTO.Inscrito == false)
                {
                    List<EventosRelacionados> ER = db.EventosRelacionados.Where(e => e.EventosId == familiarDTO.EventoRelacionado).ToList();

                    foreach (var item in ER)
                    {
                        string familiarId = Convert.ToString(i.Id);
                        var buscarInscritoOtraFecha = db.DetalleEventos.FirstOrDefault(e => e.EventosId == item.EventosId2 && e.EmpleadoId == EmpleadoId && e.FamiliarId == familiarId);
                        var buscarInscritoEnPrincipal = db.DetalleEventos.FirstOrDefault(e => e.EventosId == familiarDTO.EventoRelacionado && e.EmpleadoId == EmpleadoId && e.FamiliarId == familiarId);
                        if (buscarInscritoOtraFecha != null && buscarInscritoOtraFecha.EventosId != evento.Id)
                        {
                            familiarDTO.EstaInscritoOtraFecha = true;
                            break;
                        }
                        else
                        {
                            familiarDTO.EstaInscritoOtraFecha = false;
                        }

                        if (buscarInscritoEnPrincipal != null && buscarInscritoEnPrincipal.EventosId == familiarDTO.EventoRelacionado)
                        {
                            familiarDTO.EstaInscritoOtraFecha = true;
                            break;
                        }
                        else
                        {
                            familiarDTO.EstaInscritoOtraFecha = false;
                        }
                    }
                }

                familiarDTO.Id = i.Id;
                familiarDTO.PrimerNombre = i.PrimerNombre;
                familiarDTO.SegundoNombre = i.SegundoNombre;
                familiarDTO.PrimerApellido = i.PrimerApellido;
                familiarDTO.SegundoApellido = i.SegundoApellido;
                familiarDTO.TipoDocumento = i.TipoDocumento;
                familiarDTO.Documento = i.Documento;
                familiarDTO.Parentesco = i.Parentesco;
                familiarDTO.FechaNacimiento = i.FechaNacimiento;
                familiarDTO.Edad = i.Edad;
                familiarDTO.NroEmpleado = i.NroEmpleado;
                familiarDTO.EmpleadoId = i.EmpleadoId;

                familiarDTOs.Add(familiarDTO);
            }

            return Json(familiarDTOs);
        }

        [HttpGet]
        [Route("api/Eventos/BorrarFamiliar/{FamiliarId}")]
        public IHttpActionResult BorrarFamiliar(int FamiliarId)
        {
            var respuesta = _repo.BorrarFamiliar(FamiliarId);
            return Json(respuesta);
        }

        [HttpGet]
        [Route("api/Eventos/BorrarInscripcionEmpleado/{EmpleadoId}/{EventoId}")]
        public IHttpActionResult BorrarInscripcionEmpleado(int EmpleadoId, int EventoId)
        {
            var respuesta = _repo.BorrarInscripcionEmpleado(EmpleadoId, EventoId);
            return Json(respuesta);
        }

        [HttpGet]
        [Route("api/Eventos/BorrarInscripcionFamiliar/{FamiliarId}/{EventoId}")]
        public IHttpActionResult BorrarInscripcionFamiliar(int FamiliarId, string EventoId)
        {
            var respuesta = _repo.BorrarInscripcionFamiliar(FamiliarId, EventoId);
            return Json(respuesta);
        }

        [HttpPost]
        [Route("api/Eventos/GuardarFamiliar")]
        public IHttpActionResult GuardarFamiliar(Familiar model)
        {
            return Json(_repo.GuardarFamiliar(model));
        }

        //API para la inscripcion a Eventos

        [HttpPost]
        [Route("api/Eventos/GuardarEventoEmpleado")]
        public IHttpActionResult GuardarEventoEmpleado(DetalleEventos model)
        {
            List<EventosRelacionados> ERP = db.EventosRelacionados.Where(e => e.EventosId == model.EventosId).ToList();
            var buscarInscrito = db.DetalleEventos.Where(e => e.EventosId == model.EventosId && e.EmpleadoId == model.EmpleadoId && e.FamiliarId == null).FirstOrDefault();


            var Evento = db.Eventos.Where(e => e.Id == model.EventosId).FirstOrDefault();
            var Asistentes = db.DetalleEventos.Where(e => e.EventosId == model.EventosId).Count();
            var CuposFaltantes = Evento.Cupo -= Asistentes;

            var fechaCierre = Convert.ToDateTime(Evento.FechaCierre);
            var horaCierre = Convert.ToDateTime(Evento.HoraCierre);
            var fechaActual = DateTime.Now;
            DateTime fechaCombinada = fechaCierre.Date.Add(horaCierre.TimeOfDay);
            var EsFechaLimite = fechaCombinada <= fechaActual;

            if (EsFechaLimite == true)
            {
                return Json("FechaLimite");
            }

            if (ERP.Count() >= 1 & buscarInscrito == null)
            {
                foreach (var i in ERP)
                {
                    var buscarInscritoEnOtraFecha = db.DetalleEventos.FirstOrDefault(e => e.EventosId == i.EventosId2 && e.EmpleadoId == model.EmpleadoId && e.FamiliarId == null);

                    if (buscarInscritoEnOtraFecha != null)
                    {
                        buscarInscritoEnOtraFecha.EventosId = model.EventosId;
                        return Json("EstaInscritoEventoConcurrente");
                    }

                }
            } 
            else if (buscarInscrito != null)
            {
                return Json("Inscrito");
            } 
            else if (CuposFaltantes == 0)
            {
                return Json("SinCupos");
            }
            return Json(_repo.GuardarEventoEmpleado(model));
        }

        [HttpPost]
        [Route("api/Eventos/GuardarEventoFamiliar")]
        public IHttpActionResult GuardarEventoFamiliar(DetalleEventos model)
        {
            List<EventosRelacionados> ERP = db.EventosRelacionados.Where(e => e.EventosId == model.EventosId).ToList();
            var buscarInscrito = db.DetalleEventos.Where(e => e.EventosId == model.EventosId && e.EmpleadoId == model.EmpleadoId && e.FamiliarId == model.FamiliarId).FirstOrDefault();

            var Evento = db.Eventos.Where(e => e.Id == model.EventosId).FirstOrDefault();
            var Asistentes = db.DetalleEventos.Where(e => e.EventosId == model.EventosId).Count();
            var CuposFaltantes = Evento.Cupo -= Asistentes;

            var fechaCierre = Convert.ToDateTime(Evento.FechaCierre);
            var horaCierre = Convert.ToDateTime(Evento.HoraCierre);
            var fechaActual = DateTime.Now;
            DateTime fechaCombinada = fechaCierre.Date.Add(horaCierre.TimeOfDay);
            var EsFechaLimite = fechaCombinada <= fechaActual;

            if (EsFechaLimite == true)
            {
                return Json("FechaLimite");
            }

            if (ERP.Count() >= 1 && buscarInscrito == null)
            {
                foreach (var i in ERP)
                {
                    var buscarInscritoEnOtraFecha = db.DetalleEventos.FirstOrDefault(e => e.EventosId == i.EventosId2 && e.EmpleadoId == model.EmpleadoId && e.FamiliarId == model.FamiliarId);

                    if (buscarInscritoEnOtraFecha != null)
                    {
                        buscarInscritoEnOtraFecha.EventosId = model.EventosId;
                        return Json("EstaInscritoEventoConcurrente");
                    }
                }
            } else if (buscarInscrito != null)
            {
                return Json("Inscrito");
            } else if (CuposFaltantes == 0)
            {
                return Json("SinCupos");
            }

            return Json(_repo.GuardarEventoFamiliar(model));
        }

        [HttpGet]
        [Route("api/CalcularEdad/{EmpleadoId}")]
        public IHttpActionResult CalcularEdad(int EmpleadoId)
        {
            return Json(_repo.ActualizarEdad(EmpleadoId));
        }

        [HttpGet]
        [Route("api/Eventos/EnvioCorreoEncuestas/{eventoId}")]
        public IHttpActionResult EnvioCorreoEncuestas(int eventoId)
        {
            string respuesta1;
            string respuesta2;
            string message = "";
            try
            {
                respuesta1 = _repo.EnvioCorreosEncuestasAsistentes(eventoId);
                respuesta2 = _repo.EnvioCorreosEncuestasNoAsistentes(eventoId);

                if (respuesta1 == "True" && respuesta2 == "True")
                {
                    var evento = db.Eventos.FirstOrDefault(x=> x.Id == eventoId);
                    evento.EncuestaEnviada = true;
                    db.SaveChanges();
                    return Json("Se enviaron los correos correctamente");
                }
                if (respuesta1 == "True")
                {
                    message += "La encuesta a los Asistentes se envió correctamente.";
                }
                if (respuesta2 == "True")
                {
                    message += "La encuesta a los No Asistentes se envió correctamente.";
                }
                if (respuesta1 == "NoLink")
                {
                    message += "No se envió la Encuesta a los Asistentes porque el evento no tiene un enlace establecido.";
                }
                if (respuesta2 == "NoLink")
                {
                    message += "No se envió la Encuesta a los No Asistentes porque el evento no tiene un enlace establecido.";
                }
                return Json(message);
            }
            catch (Exception ex)
            {
                return Json("Ha ocurrido un error" + ex);
            }
        }

        [HttpGet]
        [Route("api/Eventos/EventosEncuestaPendiente")]
        public IHttpActionResult EventosEncuestaPendiente()
        {
            var eventos = db.Eventos.Where(e=> e.Estado == "Cerrado" && e.EncuestaEnviada == false).ToList();
            List<int> eventosId = new List<int>();

            foreach (var i in eventos)
            {
                var FechaEnvio = i.FechaFin.Value.AddDays(3);
                if (FechaEnvio <= DateTime.Today)
                {
                    eventosId.Add(i.Id);
                }
            }

            return Json(eventosId);
        }

        [HttpGet]
        [Route("api/Eventos/TipoDocumento")]
        public IHttpActionResult TipoDocumento()
        {
            using (var db = new AutogestionContext())
            {
                try
                {
                    List<TipoDocumento> Datos = db.TipoDocumento.ToList();
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