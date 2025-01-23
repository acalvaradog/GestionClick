using Adm_AutoGestion.Models;
using Adm_AutoGestion.Models.EvaDesempeno;
using Adm_AutoGestion.Services;
using SAP.Middleware.Connector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.UI.WebControls;
using static Adm_AutoGestion.Services.EmpleadoRepository;

namespace Adm_AutoGestion.Controllers.Api
{
    //habilitar acceso desde todos los origines, pero aca debemos poner las iP'S que pueden acceder. solo las internas
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EvaDesempenoController : ApiController
    {
        private AutogestionContext db = new AutogestionContext();
        private EvaDesempenoContext db2 = new EvaDesempenoContext();
        //private EvaDesempenoRepository _repo;
        private ServiciosRepository _servicios = new ServiciosRepository();

        //public EvaDesempenoController()
        //{
        //    _repo = new EvaDesempenoRepository();

        //}

        public class RestEmp
        {
            public Empleado Empleado { get; set; }
            public int EmpleadoId { get; set; }
            public string EvaPeriodica { get; set; }
            public string EvaEntrenamiento { get; set; }
            public string Autoevaluacion { get; set; }
            public int EstadoP { get; set; }
            public string EstadoPNombre { get; set; }
            public int EstadoE { get; set; }
            public string EstadoENombre { get; set; }
            public int EstadoA { get; set; }
            public string EstadoANombre { get; set; }
            public DateTime? FecharegistroPeriodica { get; set; }
            public DateTime? FecharegistroEntrenamiento { get; set; }
            public bool RetroalimentacionJefe { get; set; }
            public bool RetroalimentacionEmp { get; set; }
            public DateTime? FechaRTA_Jefe { get; set; }
            public DateTime? FechaRTA_EMP { get; set; }
            public int Año { get; set; }
            public int PeriodoCod { get; set; }

        }

        [HttpPost]
        [Route("api/EvaDesempeño/GenerarNotificaciones/{Emp}")]
        public IHttpActionResult GenerarNotificaciones(int Emp)
        {
            var respuesta = "";
            try
            {
                var Eva = db2.Encuestadoresxempleado.Where(x => x.codigoempleado == Emp && x.tipoevaluacion == 2).OrderByDescending(x => x.fecharegistro).FirstOrDefault();
                evaluacion_otros eva = db2.Evaluacion_otros.Where(x => x.codigoevaluacion == Eva.codigo).FirstOrDefault();
                if (eva != null)
                {
                    if (eva.Autoevaluacion == null) {
                        Notificacion NewNotificacion = new Notificacion();
                        string NmrEmp = Convert.ToString(Emp);
                        int Id = db.Empleados.Where(x => x.NroEmpleado == NmrEmp).Select(x => x.Id).FirstOrDefault();
                        DateTime FechaEva = db2.Evaluacion_encabezado.Where(x => x.codigo == Eva.codigo).Select(x => x.fecharegistro).FirstOrDefault();
                        NewNotificacion.EmpleadoId = Id;
                        NewNotificacion.Fecha = FechaEva;
                        NewNotificacion.Mensaje = "Tiene pendiente Calificar su periodo de Entrenamiento";
                        db.Notificacion.Add(NewNotificacion);
                        db.SaveChanges();
                        respuesta = "Se ha Guardado de forma Exitosa";
                    }
                }



            }
            catch (Exception ex)
            {

                Console.WriteLine("Error al crear:" + ex);
                respuesta = "Error al crear";
            }



            return Json(respuesta);
        }

        [HttpPost]
        [Route("api/EvaDesempeño/GuardarRegistro/{Otros}")]
        public IHttpActionResult GuardarRegistro(evaluacion_otros Otros)
        {
            var respuesta = "";
            try
            {
                int NmrEmp = Otros.CodEmp;

                string Autoeva = Convert.ToBase64String(System.Text.ASCIIEncoding.UTF8.GetBytes(Otros.Autoevaluacion));
                var Eva = db2.Encuestadoresxempleado.Where(x => x.codigoempleado == NmrEmp && x.tipoevaluacion == 2).OrderByDescending(x => x.fecharegistro).FirstOrDefault();
                evaluacion_otros eva = db2.Evaluacion_otros.Where(x => x.codigoevaluacion == Eva.codigo).FirstOrDefault();
                db2.Evaluacion_otros.Attach(eva);
                eva.Autoevaluacion = Autoeva;
                db2.SaveChanges();

                respuesta = "Se ha Guardado de forma Exitosa";
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error al crear:" + ex);
                respuesta = "Error al crear";
            }



            return Json(new { respuesta });
        }
        //API PARA GUARDAR LA RETROALIMENTACIÓN DEL EMPLEADO
        [HttpPost]
        [Route("api/EvaDesempeño/GuardarRetroalimentacionEmp")]
        public IHttpActionResult GuardarRetroalimentacionEmp(encuestadores_x_empleado EncXEmp)
        {
            string Respuesta = "";

            try
            {
                var Eva = db2.Encuestadoresxempleado.Where(x => x.codigoempleado == EncXEmp.codigoempleado && x.periodo == EncXEmp.periodo && x.tipoevaluador == 1).OrderByDescending(x => x.fecharegistro).FirstOrDefault();
                var AutoEva = db2.Encuestadoresxempleado.Where(x => x.codigoempleado == EncXEmp.codigoempleado && x.periodo == EncXEmp.periodo && x.tipoevaluador == 3).OrderByDescending(x => x.fecharegistro).FirstOrDefault();

                if (Eva != null)
                {
                    if (AutoEva != null)
                    {
                        if (AutoEva.Estado == 1)
                        {
                            throw new Exception("No es posible marcar como socializado, sin antes haber diligenciado la autoevaluación");
                        }
                        Eva.RetroalimentacionEmp = EncXEmp.RetroalimentacionEmp;
                        Eva.FechaRTA_Emp = DateTime.Now;
                        var seguimientos = db2.Seguimientos.Where(x => x.codigoempleado == Eva.codigoempleado && x.periodo == Eva.periodo && (x.cumplimiento == false || x.cumplimiento == null)).FirstOrDefault(); ;
                        if (seguimientos != null)
                        {
                            Eva.Estado = 2;
                            AutoEva.Estado = 2;
                        }
                        else
                        {
                            Eva.Estado = 4;
                            AutoEva.Estado = 4;
                        }
                        db2.SaveChanges();
                        Respuesta = "Guardado Exitoso";
                    }
                    else
                    {
                        throw new Exception("No se ha encontrado la autoevaluación");
                    }

                }


            }
            catch (Exception ex)
            {
                Respuesta = "Error al crear:" + ex;
                Console.WriteLine(Respuesta);

            }

            return Json(new { Respuesta });
        }
        [HttpGet]
        [Route("api/EvaDesempeño/ConsultarUltimaEva/{EmpCod}")]
        public IHttpActionResult ConsultarUltimaEva(int EmpCod)
        {
            bool respuesta = false;

            try
            {
                //var EMP = db.Empleados.Where(x => x.NroEmpleado == EmpCod).FirstOrDefault();
                //int Empcod2 =Convert.ToInt32(EmpCod);
                var Eva = db2.Encuestadoresxempleado.Where(x => x.codigoempleado == EmpCod && x.tipoevaluacion == 2).OrderByDescending(x => x.fecharegistro).FirstOrDefault();
                if (Eva != null) {
                    var otros = db2.Evaluacion_otros.Where(x => x.codigoevaluacion == Eva.codigo).FirstOrDefault();
                    if (otros != null) {
                        if (otros.Autoevaluacion == null)
                        {
                            respuesta = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine("Error al crear:" + ex);

            }

            return Json(respuesta);
        }

        [HttpGet]
        [Route("api/EvaDesempeño/ConsultarPeriodoP/{EmpCod}")]
        public IHttpActionResult ConsultarPeriodoP(int EmpCod)
        {
            List<periodos> periodo = new List<periodos>();

            try
            {
                var Eva = db2.Encuestadoresxempleado.Where(x => x.codigoempleado == EmpCod && x.tipoevaluador == 1 && (x.Estado == 2 || x.Estado == 3) && x.RetroalimentacionEmp == false).OrderByDescending(x => x.fecharegistro).ToList();
                if (Eva.Count > 0)
                {
                    //var otros = db2.Evaluacion_otros.Where(x => x.codigoevaluacion == Eva.codigo).FirstOrDefault();
                    //if (otros != null)
                    //{
                    //if (otros.Autoevaluacion == null)
                    //{
                    //    respuesta = true;
                    //}
                    //periodo = db2.Periodos.Where(x => x.TipoPeriodo == Eva.tipoevaluacion && x.codigo == Eva.periodo).FirstOrDefault();
                    //}
                    foreach (var item in Eva)
                    {
                        var p = db2.Periodos.Where(x => x.TipoPeriodo == item.tipoevaluacion && x.codigo == item.periodo).FirstOrDefault();
                        if (p != null) 
                        {
                            var dup = periodo.Where(x => x.codigo == p.codigo).FirstOrDefault();
                            if (dup == null) 
                            {
                                periodo.Add(p);
                            }
                            
                        }


                    }

                }

            }
            catch (Exception ex)
            {

                Console.WriteLine("Error al crear:" + ex);

            }

            return Json(periodo);
        }
        //API PARA EL INFORME DEL JEFE
        [HttpGet]
        [Route("api/EvaDesempeño/ConsultaEmpleado/{codigo},{Periodo} ")]
        public List<RestEmp> ConsultaEmpleado(string codigo , string Periodo)
        {
            var Datos = new empleados();
            List<Empleado> Datos2 = new List<Empleado>();
            List<RestEmp> Datos3 = new List<RestEmp>();
            List<Estructura_Jerarquica_EVADES> STREvadesh = new List<Estructura_Jerarquica_EVADES>();
            int cod = Convert.ToInt32(codigo);
            DateTime Hoy = DateTime.Today;
            DateTime Hoy2 = Hoy.AddYears(-2);
            int AñoSelect =0;
            if (Periodo == "" || Periodo == null) 
            {
                AñoSelect = Hoy.Year;
            }
            else 
            {

                AñoSelect = Convert.ToInt32(Periodo);
            }
            Periodos_Evaluacion_Desempeño Periodo1 = db2.Periodos_Evaluacion_Desempeño.Where(c => c.Año == AñoSelect).FirstOrDefault(); 
            if (Periodo1 != null)
            {


                periodos PeriodoSelect1000 = db2.Periodos.Where(x => x.Parametro_Id == Periodo1.Id && x.sociedad == 1000 && x.TipoPeriodo == 1).FirstOrDefault();
                periodos PeriodoSelect2000 = db2.Periodos.Where(x => x.Parametro_Id == Periodo1.Id && x.sociedad == 2000 && x.TipoPeriodo == 1).FirstOrDefault();

                STREvadesh = db2.Estructura_Jerarquica_EVADES.Where(x => x.Jefe == codigo).ToList();
            Datos2 = db.Empleados.Where(x => x.Jefe == codigo && x.Activo == "SI" && x.TipoArea != "SENA Pctivo/Pasante" && x.TipoArea != "SENA Lectivo").ToList();
            if (STREvadesh.Count() > 0)
            {
                foreach (var item in STREvadesh)
                {
                    List<Empleado> Emps = new List<Empleado>();
                    Emps = db.Empleados.Where(x => x.Activo == "SI" && x.AreaDescripcion == item.Area && x.NroEmpleado != codigo && x.TipoArea == "Asistenciales CO" && x.TipoArea != "SENA Pctivo/Pasante" && x.TipoArea != "SENA Lectivo").ToList();
                    if (Emps.Count > 0)
                    {
                        Datos2.AddRange(Emps);
                    }

                }

            }


            var listPeriodicas = db2.Encuestadoresxempleado.Where(x => x.tipoevaluacion == 1 && (x.periodo == PeriodoSelect1000.codigo || x.periodo == PeriodoSelect2000.codigo) && x.Estado != 6).ToList();

            var ListEntrenamiento = db2.Encuestadoresxempleado.Where(x => x.tipoevaluacion == 2 && (x.periodo == PeriodoSelect1000.codigo || x.periodo == PeriodoSelect2000.codigo) && x.Estado != 6).ToList();
            var ListEstados = db2.EstadosEvaluacionEncabezado.Where(x => x.Descripcion != null && x.Descripcion != "").ToList();

            int Año = DateTime.Today.Year;
            
            if (Periodo1 != null)
            {
                periodos Periodo1000 = db2.Periodos.Where(x => x.Parametro_Id == Periodo1.Id && x.TipoPeriodo == 1 && x.sociedad == 1000).FirstOrDefault();
                periodos Periodo2000 = db2.Periodos.Where(x => x.Parametro_Id == Periodo1.Id && x.TipoPeriodo == 1 && x.sociedad == 2000).FirstOrDefault();
                    //CONSULTA SEGUN LA ESTRUCTURA ACTUAL
                foreach (var empleado in Datos2)
                {
                    int Periodo2 = 0;
                    if (empleado.Empresa == "1000")
                    {
                        Periodo2 = Periodo1000.codigo;
                    }
                    else if (empleado.Empresa == "2000")
                    {
                        Periodo2 = Periodo2000.codigo;
                    }
                    int nmr = Convert.ToInt32(empleado.NroEmpleado);
                        if (nmr == 40000282)
                        {
                            var debug = 0;
                        }
                        encuestadores_x_empleado periodica = listPeriodicas.Where(x => x.codigoempleado == nmr && x.codigoevaluador == cod && x.tipoevaluacion == 1 && x.tipoevaluador == 1 && x.periodo == Periodo2).OrderByDescending(x => x.fecharegistro).FirstOrDefault();
                    encuestadores_x_empleado AutoevalucionPeriodica = listPeriodicas.Where(x => x.codigoempleado == nmr && x.tipoevaluacion == 1 && x.tipoevaluador == 3 && x.periodo == Periodo2).OrderByDescending(x => x.fecharegistro).FirstOrDefault();
                    //encuestadores_x_empleado entrenamiento = ListEntrenamiento.Where(x => x.codigoempleado == nmr && x.tipoevaluacion == 2).OrderByDescending(x => x.fecharegistro).FirstOrDefault(); ;

                    DateTime fecharegistro;
                    RestEmp restEmp = new RestEmp();
                    DateTime fechaingreso = Convert.ToDateTime(empleado.FechaIngreso);


                    TimeSpan ingreso = fechaingreso.Date - Hoy.Date;
                    double ingresoaño = (ingreso.Days / 365.2425) * -1;
                    double ingresomes = (ingreso.Days / 90) * -1;


                    if (periodica != null)
                    {
                        fecharegistro = Convert.ToDateTime(periodica.fecharegistro);
                        //TimeSpan Result = fecharegistro.Date - Hoy.Date;
                        //double totalTime = (Result.Days / 365.2425) * -1;
                        restEmp.EstadoP = periodica.Estado;
                        var Estado = ListEstados.Where(x => x.Codigo == periodica.Estado).FirstOrDefault();
                        restEmp.EstadoPNombre = Estado.Descripcion;
                        restEmp.FecharegistroPeriodica = fecharegistro;
                        restEmp.Año = Periodo1.Año;
                        restEmp.RetroalimentacionJefe = periodica.RetroalimentacionJefe;
                        restEmp.FechaRTA_Jefe = periodica.FechaRTA_Jefe;
                        restEmp.RetroalimentacionEmp = periodica.RetroalimentacionEmp;
                        restEmp.FechaRTA_EMP = periodica.FechaRTA_Emp;

                        //var fecharegistro2 = fecharegistro.AddDays(365);
                        //if (fecharegistro > empleado.FechaIngreso && fecharegistro < Hoy && fecharegistro2 >= Hoy)
                        //{
                        //    //var encabezado = db2.Evaluacion_encabezado.Where(x => x.codigo == peridioca.codigo).FirstOrDefault();
                        //    //if (encabezado!=null) 
                        //    //{
                        //    //    restEmp.EvaPeriodica = "Ya presentada";
                        //    //}
                        //    //else 
                        //    //{
                        //    //    restEmp.EvaPeriodica = "Generada";
                        //    //}

                        //}
                        //else
                        //{
                        //    //if (ingresoaño >= 1)
                        //    //{
                        //    //    restEmp.EvaPeriodica = "Sin presentar";

                        //    //}
                        //    //else if (ingresoaño < 1) { restEmp.EvaPeriodica = "No aplica"; }
                        //}


                    }
                    else
                    {

                        //if (ingresoaño >= 1) {
                        //    restEmp.EvaPeriodica = "Sin presentar";

                        //} else if (ingresoaño < 1) { restEmp.EvaPeriodica = "No aplica"; }
                        //restEmp.FecharegistroPeriodica = new DateTime();
                    }


                    //if (entrenamiento != null)
                    //{
                    //    fecharegistro = Convert.ToDateTime(entrenamiento.fecharegistro);
                    //    restEmp.FecharegistroEntrenamiento = fecharegistro;
                    //    restEmp.EstadoE = entrenamiento.Estado;
                    //    var Estado = ListEstados.Where(x => x.Codigo == periodica.Estado).FirstOrDefault();
                    //    restEmp.EstadoENombre = Estado.Descripcion;
                    //    restEmp.RetroalimentacionEmp = entrenamiento.RetroalimentacionEmp;
                    //    //if (entrenamiento.fecharegistro <= Hoy && entrenamiento.fecharegistro >= empleado.FechaIngreso)
                    //    //{

                    //    //    var encabezado = db2.Evaluacion_encabezado.Where(x => x.codigo == entrenamiento.codigo).FirstOrDefault();
                    //    //    if (encabezado != null)
                    //    //    {
                    //    //        restEmp.EvaEntrenamiento = "Ya presentada";
                    //    //    }
                    //    //    else
                    //    //    {
                    //    //        restEmp.EvaEntrenamiento = "Generada";
                    //    //    }
                    //    //}
                    //    //else
                    //    //{
                    //    //    if (ingresomes >= 1 && peridioca == null)
                    //    //    {
                    //    //        restEmp.EvaEntrenamiento = "Sin presentar";
                    //    //    }
                    //    //    else if (ingresomes < 1 && peridioca == null)
                    //    //    { restEmp.EvaEntrenamiento = "No aplica"; }
                    //    //}

                    //}
                    //else
                    //{

                    //    //if (ingresomes >= 1 && peridioca == null)
                    //    //{
                    //    //    restEmp.EvaEntrenamiento = "Sin presentar";
                    //    //}
                    //    //else if (ingresomes < 1 || peridioca != null)
                    //    //{ restEmp.EvaEntrenamiento = "No aplica"; }

                    //    restEmp.FecharegistroEntrenamiento = new DateTime();
                    //}
                    if (AutoevalucionPeriodica != null)
                    {
                        fecharegistro = Convert.ToDateTime(AutoevalucionPeriodica.fecharegistro);
                        restEmp.EstadoA = AutoevalucionPeriodica.Estado;
                        var Estado = ListEstados.Where(x => x.Codigo == AutoevalucionPeriodica.Estado).FirstOrDefault();
                        restEmp.EstadoANombre = Estado.Descripcion;
                        //TimeSpan Result = fecharegistro.Date - Hoy.Date;
                        //double totalTime = (Result.Days / 365.2425) * -1;
                        //var fecharegistro2 = fecharegistro.AddDays(365);

                        //if (fecharegistro > empleado.FechaIngreso && fecharegistro < Hoy && fecharegistro2 >= Hoy)
                        //{
                        //    var encabezado = db2.Evaluacion_encabezado.Where(x => x.codigo == AutoevalucionPeriodica.codigo).FirstOrDefault();
                        //    if (encabezado != null)
                        //    {
                        //        restEmp.Autoevaluacion = "Ya presentada";
                        //    }
                        //    else
                        //    {
                        //        restEmp.Autoevaluacion = "Generada";
                        //    }

                        //}
                        //else
                        //{
                        //    if (ingresoaño >= 1)
                        //    {
                        //        restEmp.Autoevaluacion = "Sin presentar";

                        //    }
                        //    else if (ingresoaño < 1) { restEmp.Autoevaluacion = "No aplica"; }
                        //}

                    }


                    restEmp.Empleado = empleado;



                    Datos3.Add(restEmp);


                }

                    //CONSULTA EVALUACIONES YA PRESENTADAS POR EL JEFE INDEPENDIENTE DE LA ESTRUCTURA
                    List<encuestadores_x_empleado> LisEvaNoAsig = listPeriodicas.Where(x => x.codigoevaluador == cod && x.tipoevaluacion == 1 && (x.periodo == PeriodoSelect1000.codigo || x.periodo == PeriodoSelect2000.codigo) && x.Estado != 6).ToList();
                    List<string> NrmEmpleados = LisEvaNoAsig.Select(x => x.codigoempleado.ToString()).ToList();
                    List<Empleado> ListEMPNoAsig = db.Empleados.Where(x => NrmEmpleados.Contains(x.NroEmpleado)).ToList();
               foreach (var empleado in ListEMPNoAsig) 
                    {
                        int Periodo2 = 0;
                        if (empleado.Empresa == "1000")
                        {
                            Periodo2 = Periodo1000.codigo;
                        }
                        else if (empleado.Empresa == "2000")
                        {
                            Periodo2 = Periodo2000.codigo;
                        }
                        int nmr = Convert.ToInt32(empleado.NroEmpleado);
                        if (nmr == 40000282) 
                        {
                            var debug = 0;
                        }
                        encuestadores_x_empleado periodica = listPeriodicas.Where(x => x.codigoempleado == nmr && x.codigoevaluador == cod && x.tipoevaluacion == 1 && x.tipoevaluador == 1 && x.periodo == Periodo2).OrderByDescending(x => x.fecharegistro).FirstOrDefault();
                        encuestadores_x_empleado AutoevalucionPeriodica = listPeriodicas.Where(x => x.codigoempleado == nmr && x.tipoevaluacion == 1 && x.tipoevaluador == 3 && x.periodo == Periodo2).OrderByDescending(x => x.fecharegistro).FirstOrDefault();
                        //encuestadores_x_empleado entrenamiento = ListEntrenamiento.Where(x => x.codigoempleado == nmr && x.tipoevaluacion == 2).OrderByDescending(x => x.fecharegistro).FirstOrDefault(); ;

                        DateTime fecharegistro;
                        RestEmp restEmp = new RestEmp();
                        DateTime fechaingreso = Convert.ToDateTime(empleado.FechaIngreso);


                        TimeSpan ingreso = fechaingreso.Date - Hoy.Date;
                        double ingresoaño = (ingreso.Days / 365.2425) * -1;
                        double ingresomes = (ingreso.Days / 90) * -1;


                        if (periodica != null)
                        {
                            fecharegistro = Convert.ToDateTime(periodica.fecharegistro);
                            //TimeSpan Result = fecharegistro.Date - Hoy.Date;
                            //double totalTime = (Result.Days / 365.2425) * -1;
                            restEmp.EstadoP = periodica.Estado;
                            var Estado = ListEstados.Where(x => x.Codigo == periodica.Estado).FirstOrDefault();
                            restEmp.EstadoPNombre = Estado.Descripcion;
                            restEmp.FecharegistroPeriodica = fecharegistro;
                            restEmp.Año = Periodo1.Año;
                            restEmp.RetroalimentacionJefe = periodica.RetroalimentacionJefe;
                            restEmp.FechaRTA_Jefe = periodica.FechaRTA_Jefe;
                            restEmp.RetroalimentacionEmp = periodica.RetroalimentacionEmp;
                            restEmp.FechaRTA_EMP = periodica.FechaRTA_Emp;

                            //var fecharegistro2 = fecharegistro.AddDays(365);
                            //if (fecharegistro > empleado.FechaIngreso && fecharegistro < Hoy && fecharegistro2 >= Hoy)
                            //{
                            //    //var encabezado = db2.Evaluacion_encabezado.Where(x => x.codigo == peridioca.codigo).FirstOrDefault();
                            //    //if (encabezado!=null) 
                            //    //{
                            //    //    restEmp.EvaPeriodica = "Ya presentada";
                            //    //}
                            //    //else 
                            //    //{
                            //    //    restEmp.EvaPeriodica = "Generada";
                            //    //}

                            //}
                            //else
                            //{
                            //    //if (ingresoaño >= 1)
                            //    //{
                            //    //    restEmp.EvaPeriodica = "Sin presentar";

                            //    //}
                            //    //else if (ingresoaño < 1) { restEmp.EvaPeriodica = "No aplica"; }
                            //}


                        }
                        else
                        {

                            //if (ingresoaño >= 1) {
                            //    restEmp.EvaPeriodica = "Sin presentar";

                            //} else if (ingresoaño < 1) { restEmp.EvaPeriodica = "No aplica"; }
                            //restEmp.FecharegistroPeriodica = new DateTime();
                        }


                        //if (entrenamiento != null)
                        //{
                        //    fecharegistro = Convert.ToDateTime(entrenamiento.fecharegistro);
                        //    restEmp.FecharegistroEntrenamiento = fecharegistro;
                        //    restEmp.EstadoE = entrenamiento.Estado;
                        //    var Estado = ListEstados.Where(x => x.Codigo == periodica.Estado).FirstOrDefault();
                        //    restEmp.EstadoENombre = Estado.Descripcion;
                        //    restEmp.RetroalimentacionEmp = entrenamiento.RetroalimentacionEmp;
                        //    //if (entrenamiento.fecharegistro <= Hoy && entrenamiento.fecharegistro >= empleado.FechaIngreso)
                        //    //{

                        //    //    var encabezado = db2.Evaluacion_encabezado.Where(x => x.codigo == entrenamiento.codigo).FirstOrDefault();
                        //    //    if (encabezado != null)
                        //    //    {
                        //    //        restEmp.EvaEntrenamiento = "Ya presentada";
                        //    //    }
                        //    //    else
                        //    //    {
                        //    //        restEmp.EvaEntrenamiento = "Generada";
                        //    //    }
                        //    //}
                        //    //else
                        //    //{
                        //    //    if (ingresomes >= 1 && peridioca == null)
                        //    //    {
                        //    //        restEmp.EvaEntrenamiento = "Sin presentar";
                        //    //    }
                        //    //    else if (ingresomes < 1 && peridioca == null)
                        //    //    { restEmp.EvaEntrenamiento = "No aplica"; }
                        //    //}

                        //}
                        //else
                        //{

                        //    //if (ingresomes >= 1 && peridioca == null)
                        //    //{
                        //    //    restEmp.EvaEntrenamiento = "Sin presentar";
                        //    //}
                        //    //else if (ingresomes < 1 || peridioca != null)
                        //    //{ restEmp.EvaEntrenamiento = "No aplica"; }

                        //    restEmp.FecharegistroEntrenamiento = new DateTime();
                        //}
                        if (AutoevalucionPeriodica != null)
                        {
                            fecharegistro = Convert.ToDateTime(AutoevalucionPeriodica.fecharegistro);
                            restEmp.EstadoA = AutoevalucionPeriodica.Estado;
                            var Estado = ListEstados.Where(x => x.Codigo == AutoevalucionPeriodica.Estado).FirstOrDefault();
                            restEmp.EstadoANombre = Estado.Descripcion;
                            //TimeSpan Result = fecharegistro.Date - Hoy.Date;
                            //double totalTime = (Result.Days / 365.2425) * -1;
                            //var fecharegistro2 = fecharegistro.AddDays(365);

                            //if (fecharegistro > empleado.FechaIngreso && fecharegistro < Hoy && fecharegistro2 >= Hoy)
                            //{
                            //    var encabezado = db2.Evaluacion_encabezado.Where(x => x.codigo == AutoevalucionPeriodica.codigo).FirstOrDefault();
                            //    if (encabezado != null)
                            //    {
                            //        restEmp.Autoevaluacion = "Ya presentada";
                            //    }
                            //    else
                            //    {
                            //        restEmp.Autoevaluacion = "Generada";
                            //    }

                            //}
                            //else
                            //{
                            //    if (ingresoaño >= 1)
                            //    {
                            //        restEmp.Autoevaluacion = "Sin presentar";

                            //    }
                            //    else if (ingresoaño < 1) { restEmp.Autoevaluacion = "No aplica"; }
                            //}

                        }


                        restEmp.Empleado = empleado;



                        Datos3.Add(restEmp);
                    }
            }
            }
            return Datos3;
        }
        //API PARA EL INFORME DE GESTION HUMANA
        [HttpGet]
        [Route("api/EvaDesempeño/ConsultaEmpleadoGH/{codigo}")]
        public List<RestEmp> ConsultaEmpleadoGH(int codigo)
        {
            var Datos = new empleados();
            List<Empleado> Datos2 = new List<Empleado>();
            List<RestEmp> Datos3 = new List<RestEmp>();

            Datos2 = db.Empleados.Where(x => x.Activo == "SI" && x.TipoArea != "SENA Pctivo/Pasante" && x.TipoArea != "SENA Lectivo").ToList();
            DateTime Hoy = DateTime.Today;
            DateTime Hoy2 = Hoy.AddYears(-2);
            var listPeriodicas = db2.Encuestadoresxempleado.Where(x => x.tipoevaluacion == 1 && x.periodo == codigo && x.Estado!=6).ToList();
            //var ListEntrenamiento = db2.Encuestadoresxempleado.Where(x => x.tipoevaluacion == 2 && x.fecharegistro >= Hoy2).ToList();
            var ListEstados = db2.EstadosEvaluacionEncabezado.Where(x => x.Descripcion != null && x.Descripcion != "").ToList();

            int Año = DateTime.Today.Year;
            //Periodos_Evaluacion_Desempeño Periodo1 = db2.Periodos_Evaluacion_Desempeño.Where(c => c.Año == Año).FirstOrDefault();
            //periodos Periodo2 = db2.Periodos.Where(x => x.fechaincio == Periodo1.FechaInicial && x.fechafinal == Periodo1.FechaFinal && x.TipoPeriodo == 1).FirstOrDefault();
            periodos Periodo2 = db2.Periodos.Where(x => x.codigo == codigo && x.TipoPeriodo == 1).FirstOrDefault();

            foreach (var empleado in Datos2)
            {
                int nmr = Convert.ToInt32(empleado.NroEmpleado);
                encuestadores_x_empleado periodica = listPeriodicas.Where(x => x.codigoempleado == nmr && x.tipoevaluacion == 1 && x.codigoempleado != x.codigoevaluador && x.periodo == Periodo2.codigo).OrderByDescending(x => x.fecharegistro).FirstOrDefault();
                encuestadores_x_empleado AutoevalucionPeriodica = listPeriodicas.Where(x => x.codigoempleado == nmr && x.tipoevaluacion == 1 && x.codigoempleado == x.codigoevaluador && x.periodo == Periodo2.codigo).OrderByDescending(x => x.fecharegistro).FirstOrDefault();
                //encuestadores_x_empleado entrenamiento = ListEntrenamiento.Where(x => x.codigoempleado == nmr && x.tipoevaluacion == 2).OrderByDescending(x => x.fecharegistro).FirstOrDefault(); ;
                //DateTime Hoy = DateTime.Today;
                DateTime fecharegistro;
                RestEmp restEmp = new RestEmp();
                DateTime fechaingreso = Convert.ToDateTime(empleado.FechaIngreso);


                TimeSpan ingreso = fechaingreso.Date - Hoy.Date;
                double ingresoaño = (ingreso.Days / 365.2425) * -1;
                double ingresomes = (ingreso.Days / 90) * -1;


                if (periodica != null)
                {
                    fecharegistro = Convert.ToDateTime(periodica.fecharegistro);
                    //TimeSpan Result = fecharegistro.Date - Hoy.Date;
                    //double totalTime = (Result.Days / 365.2425) * -1;
                    restEmp.EstadoP = periodica.Estado;
                    var Estado = ListEstados.Where(x => x.Codigo == periodica.Estado).FirstOrDefault();
                    restEmp.EstadoP = periodica.Estado;
                    restEmp.EstadoPNombre = Estado.Descripcion;
                    restEmp.FecharegistroPeriodica = fecharegistro;
                    restEmp.Año = Año;
                    restEmp.RetroalimentacionJefe = periodica.RetroalimentacionJefe;
                    restEmp.FechaRTA_Jefe = periodica.FechaRTA_Jefe;
                    restEmp.RetroalimentacionEmp = periodica.RetroalimentacionEmp;
                    restEmp.FechaRTA_EMP = periodica.FechaRTA_Emp;
                    var fecharegistro2 = fecharegistro.AddDays(365);
                    restEmp.FecharegistroPeriodica = fecharegistro;
                    restEmp.PeriodoCod = Periodo2.codigo;

                    if (fecharegistro > empleado.FechaIngreso && fecharegistro < Hoy && fecharegistro2 >= Hoy)
                    {
                        //var encabezado = db2.Evaluacion_encabezado.Where(x => x.codigo == peridioca.codigo).FirstOrDefault();
                        //if (encabezado != null)
                        //{
                        //    restEmp.EvaPeriodica = "Ya presentada";
                        //}
                        //else
                        //{
                        //    restEmp.EvaPeriodica = "Evaluación Generada";
                        //}
                    }
                    else
                    {
                        //if (ingresoaño >= 1)
                        //{
                        //    restEmp.EvaPeriodica = "Sin presentar";

                        //}
                        //else if (ingresoaño < 1) { restEmp.EvaPeriodica = "No aplica"; }
                    }

                    
                }
                else
                {

                    //if (ingresoaño >= 1)
                    //{
                    //    restEmp.EvaPeriodica = "Sin presentar";

                    //}
                    //else if (ingresoaño < 1) { restEmp.EvaPeriodica = "No aplica"; }
                    restEmp.FecharegistroPeriodica = new DateTime();
                }
                //if (entrenamiento != null)
                //{
                //    fecharegistro = Convert.ToDateTime(entrenamiento.fecharegistro);
                //    if (entrenamiento.fecharegistro <= Hoy && entrenamiento.fecharegistro >= empleado.FechaIngreso)
                //    {
                //        var encabezado = db2.Evaluacion_encabezado.Where(x => x.codigo == entrenamiento.codigo).FirstOrDefault();
                //        //if (encabezado != null)
                //        //{
                //        //    restEmp.EvaEntrenamiento = "Ya presentada";
                //        //}
                //        //else
                //        //{
                //        //    restEmp.EvaEntrenamiento = "Evaluación Generada";
                //        //}

                //    }
                //    else
                //    {
                //        //if (ingresomes >= 1)
                //        //{
                //        //    restEmp.EvaEntrenamiento = "Sin presentar";
                //        //}
                //        //else if (ingresomes < 1)
                //        //{ restEmp.EvaEntrenamiento = "No aplica"; }

                //    }
                //    restEmp.FecharegistroEntrenamiento = fecharegistro;
                //}
                //else
                //{

                //    //if (ingresomes >= 1)
                //    //{
                //    //    restEmp.EvaEntrenamiento = "Sin presentar";
                //    //}
                //    //else if (ingresomes < 1)
                //    //{ restEmp.EvaEntrenamiento = "No aplica"; }
                //    restEmp.FecharegistroEntrenamiento = new DateTime();

                //}
                if (AutoevalucionPeriodica != null)
                {
                    fecharegistro = Convert.ToDateTime(AutoevalucionPeriodica.fecharegistro);
                    //TimeSpan Result = fecharegistro.Date - Hoy.Date;
                    //double totalTime = (Result.Days / 365.2425) * -1;
                    var fecharegistro2 = fecharegistro.AddDays(365);
                    fecharegistro = Convert.ToDateTime(AutoevalucionPeriodica.fecharegistro);
                    restEmp.EstadoA = AutoevalucionPeriodica.Estado;
                    var Estado = ListEstados.Where(x => x.Codigo == AutoevalucionPeriodica.Estado).FirstOrDefault();
                    restEmp.EstadoANombre = Estado.Descripcion;
                    //if (fecharegistro > empleado.FechaIngreso && fecharegistro < Hoy && fecharegistro2 >= Hoy)
                    //{
                    //    var encabezado = db2.Evaluacion_encabezado.Where(x => x.codigo == AutoevalucionPeriodica.codigo).FirstOrDefault();
                    //    if (encabezado != null)
                    //    {
                    //        restEmp.Autoevaluacion = "Ya presentada";
                    //    }
                    //    else
                    //    {
                    //        restEmp.Autoevaluacion = "Evaluación Generada";
                    //    }
                    //}
                    //else
                    //{
                    //    if (ingresoaño >= 1)
                    //    {
                    //        restEmp.Autoevaluacion = "Sin presentar";

                    //    }
                    //    else if (ingresoaño < 1) { restEmp.Autoevaluacion = "No aplica"; }
                    //}
                }
                else 
                {
                    restEmp.EstadoANombre = "No Generada";
                }



                restEmp.Empleado = empleado;
                restEmp.EmpleadoId = empleado.Id;



                Datos3.Add(restEmp);


            }
            return Datos3;
        }


        //---------------PRIMERA API-----------------------------//
        [HttpGet]
        [Route("api/EvaDesempeño/ActualizarEmpsEvadesempeño")]
        public string ActualizarEmpsEvadesempeño()
        {
            try
            {
                List<Empleado> EmpeleadosAdmAutogestion = new List<Empleado>();
                List<empleados> EmpeleadosAEvadesempeño = new List<empleados>();

                //Variables para actualizar
                List<empleados> EmpeleadosaActualizar = new List<empleados>();
                List<usuarios_x_sociedad> SociedadAactualizar = new List<usuarios_x_sociedad>();
                List<encuestadores_x_empleado> EvaluacionesAnuladas = new List<encuestadores_x_empleado>();

                //Variable para Crear
                List<empleados> EmpleadosAcrear = new List<empleados>();
                List<usuarios_x_sociedad> SociedadACrear = new List<usuarios_x_sociedad>();
                List<Empleados_Cargo_Area_x_Periodo> ListTabRelActualizar = new List<Empleados_Cargo_Area_x_Periodo>();
                List<usuarios_x_sociedad> ListRemoveUxS = new List<usuarios_x_sociedad>();

                bool ActualizacionExitosa = ActualizarCargosAreasEvades();
                List<cargos> ListCargosEvades = db2.Cargos.ToList();
                List<areas> LisAreasEvades = db2.areas.ToList();
                
                EmpeleadosAdmAutogestion = db.Empleados.Where(x=>x.TipoArea != "SENA Pctivo/Pasante" && x.TipoArea != "SENA Lectivo").ToList();
                //EmpeleadosAdmAutogestion = db.Empleados.Where(x => x.NroEmpleado == "40000849").ToList();
                EmpeleadosAEvadesempeño = db2.empleados.ToList();
                //EmpeleadosAEvadesempeño = db2.empleados.Where(x => x.codigo == 40000282 || x.codigo == 40001088 || x.codigo == 40006793).ToList();
                List<usuarios_x_sociedad> UCLIST = db2.Usuarios_x_sociedad.ToList();
                List<Estructura_Jerarquica_EVADES> STREvadesh = db2.Estructura_Jerarquica_EVADES.Where(x => x.Jefe != "" && x.Jefe != null).ToList();
                List<usuario_perfil> PerfilesUSER = db2.Usuario_perfil.Where(x => x.codigoperfil == 1).ToList();
                int year = DateTime.Now.Year;
                Periodos_Evaluacion_Desempeño CongfinP = db2.Periodos_Evaluacion_Desempeño.Where(x => x.Año ==year).FirstOrDefault();
                periodos Periodo = db2.Periodos.Where(x => x.fechafinal == CongfinP.FechaFinal && x.fechaincio == CongfinP.FechaInicial).FirstOrDefault();
                List<encuestadores_x_empleado> Evalauciones = db2.Encuestadoresxempleado.Where(x => x.periodo == Periodo.codigo).ToList();
                            
                foreach (var Empleado in EmpeleadosAdmAutogestion)
                {
                    int NmrEmp = Convert.ToInt32(Empleado.NroEmpleado);

                    int NmrJefe = 0;
                    if (Empleado.Jefe != "")
                    {
                        NmrJefe = Convert.ToInt32(Empleado.Jefe);
                    }
                    if (NmrEmp == 40001088)
                    {
                        var debug = 0;
                    }
                    int Sociedad = Convert.ToInt32(Empleado.Empresa);
                    empleados Emp = EmpeleadosAEvadesempeño.Where(x => x.codigo == NmrEmp).FirstOrDefault();
                    
                    if (Emp == null)
                    {//Crear usuario Activo si no existe en Evaluacion de desempeño en la tabla: empelados
                        if (Empleado.Activo == "SI")
                        {
                            char[] delimiterChars = { ' ' };
                            string[] words = Empleado.Nombres.Split(delimiterChars);
                            string CodSAParea = Convert.ToString(Empleado.CodigoSAPArea);
                            string CodSAPcargo = Convert.ToString(Empleado.CodigoSAPCargo);
                            empleados EmpleadoN = new empleados();
                            EmpleadoN.codigo = Convert.ToInt32(Empleado.NroEmpleado);
                            EmpleadoN.estado = 1;
                            EmpleadoN.identificacion = Empleado.Documento;
                            EmpleadoN.tipoidentificacion = 1;
                            if (words.Length >= 4) {
                                EmpleadoN.nombres = words[2] + " " + words[3];
                                EmpleadoN.apellidos = words[0] + " " + words[1];
                            }
                            if (words.Length == 3)
                            {
                                EmpleadoN.nombres = words[2];
                                EmpleadoN.apellidos = words[0] + " " + words[1];
                            }
                            if (words.Length == 2)
                            {
                                EmpleadoN.nombres = words[1];
                                EmpleadoN.apellidos = words[0];
                            }


                            EmpleadoN.email = Empleado.Correo;
                            if (Empleado.Telefono != "" && Empleado.Telefono!= null)
                            {
                                var longitud = Empleado.Telefono.Length;
                                if (longitud < 29)
                                {
                                    EmpleadoN.movil = Empleado.Telefono;
                                }
                                else
                                {
                                    var debug = 0;
                                }

                            }
                          
                            
                            EmpleadoN.clave = "";
                            areas Area = db2.areas.Where(x => x.CodigoSAP == CodSAParea).FirstOrDefault();
                            if (Area != null) { EmpleadoN.codigoarea = Area.codigo; }
                            else { EmpleadoN.codigoarea = 0; }
                            cargos Cargo = db2.Cargos.Where(x => x.descripcion == Empleado.Cargo).FirstOrDefault();
                            if (Cargo != null) { EmpleadoN.codigocargo = Cargo.codigo; }
                            else { EmpleadoN.codigocargo = 0; }
                            var duplicadoEmp = EmpleadosAcrear.Where(x => x.codigo == NmrEmp).FirstOrDefault();
                            if (duplicadoEmp == null)
                            {
                                EmpleadosAcrear.Add(EmpleadoN);
                            }
                            //CREAR RELACION 

                            //Crear registro usuarios x sociedad
                            //usuarios_x_sociedad newrel1 = new usuarios_x_sociedad();
                            //newrel1.usuario = NmrEmp;
                            //newrel1.sociedad = Sociedad;
                            //var Duplicado = SociedadACrear.Where(x=>x.sociedad == newrel1.sociedad && x.usuario== newrel1.usuario ).FirstOrDefault();
                            //var Duplicado2 = UCLIST.Where(x => x.sociedad == newrel1.sociedad && x.usuario == newrel1.usuario).FirstOrDefault();
                            //if (Duplicado== null && Duplicado2 ==null) 
                            //{
                            //    SociedadACrear.Add(newrel1);
                            //}

                            int Empleados1000 = 0;
                            int Empleados2000 = 0;
                            Estructura_Jerarquica_EVADES StrEvadeshEmp = STREvadesh.Where(x => x.Area == Empleado.AreaDescripcion).FirstOrDefault();
                            if (StrEvadeshEmp != null)
                            {
                                if (StrEvadeshEmp.Jefe == Empleado.NroEmpleado)
                                {
                                    Empleados1000 = EmpeleadosAdmAutogestion.Where(x => x.Empresa == "1000" && x.AreaDescripcion == StrEvadeshEmp.Area && x.Activo == "SI").Count();
                                    Empleados2000 = EmpeleadosAdmAutogestion.Where(x => x.Empresa == "2000" && x.AreaDescripcion == StrEvadeshEmp.Area && x.Activo == "SI").Count();
                                }
                                else
                                {
                                    Empleados1000 = EmpeleadosAdmAutogestion.Where(x => x.Empresa == "1000" && x.Jefe == Empleado.NroEmpleado && x.Activo == "SI").Count();
                                    Empleados2000 = EmpeleadosAdmAutogestion.Where(x => x.Empresa == "2000" && x.Jefe == Empleado.NroEmpleado && x.Activo == "SI").Count();
                                }
                            }
                            else
                            {
                                Empleados1000 = EmpeleadosAdmAutogestion.Where(x => x.Empresa == "1000" && x.Jefe == Empleado.NroEmpleado && x.Activo == "SI").Count();
                                Empleados2000 = EmpeleadosAdmAutogestion.Where(x => x.Empresa == "2000" && x.Jefe == Empleado.NroEmpleado && x.Activo == "SI").Count();
                            }


                            if (Empleados1000 > 0 || Empleados2000 > 0)
                            {
                                usuarios_x_sociedad EmpSoc1000 = UCLIST.Where(x => x.usuario == NmrEmp && x.sociedad == 1000).FirstOrDefault();
                                usuarios_x_sociedad SociedadDuplicada = SociedadACrear.Where(x => x.usuario == NmrEmp && x.sociedad == 1000).FirstOrDefault();


                                if (EmpSoc1000 == null && SociedadDuplicada == null)
                                {
                                    if (Empleados1000 > 0)
                                    {
                                        usuarios_x_sociedad NewEmpSoc = new usuarios_x_sociedad();
                                        NewEmpSoc.sociedad = 1000;
                                        NewEmpSoc.usuario = NmrEmp;
                                        SociedadACrear.Add(NewEmpSoc);
                                    }
                                }

                                usuarios_x_sociedad EmpSoc2000 = UCLIST.Where(x => x.usuario == NmrEmp && x.sociedad == 2000).FirstOrDefault();
                                usuarios_x_sociedad SociedadDuplicada2 = SociedadACrear.Where(x => x.usuario == NmrEmp && x.sociedad == 2000).FirstOrDefault();

                                if (EmpSoc2000 == null && SociedadDuplicada2 == null)
                                {

                                    if (Empleados2000 > 0)
                                    {
                                        usuarios_x_sociedad NewEmpSoc = new usuarios_x_sociedad();
                                        NewEmpSoc.sociedad = 2000;
                                        NewEmpSoc.usuario = NmrEmp;
                                        SociedadACrear.Add(NewEmpSoc);
                                    }
                                }
                            }
                            else
                            {

                                if (Sociedad == 1000)
                                {
                                    // Si no tiene emeplados acargo elimina la sociedad a la cual no pertenece el empleado
                                    usuarios_x_sociedad EmpSoc2000 = UCLIST.Where(x => x.usuario == NmrEmp && x.sociedad == 2000).FirstOrDefault();
                                    if (EmpSoc2000 != null)
                                    {
                                        ListRemoveUxS.Add(EmpSoc2000);
                                    }
                                    // si no tiene creada la sociedad a la cual pertene
                                    usuarios_x_sociedad SociedadDuplicada = SociedadACrear.Where(x => x.usuario == NmrEmp && x.sociedad == 1000).FirstOrDefault();
                                    usuarios_x_sociedad EmpSoc1000 = UCLIST.Where(x => x.usuario == NmrEmp && x.sociedad == 1000).FirstOrDefault();
                                    if (EmpSoc1000 == null && SociedadDuplicada == null)
                                    {
                                        usuarios_x_sociedad NewEmpSoc = new usuarios_x_sociedad();
                                        NewEmpSoc.sociedad = 1000;
                                        NewEmpSoc.usuario = NmrEmp;
                                        SociedadACrear.Add(NewEmpSoc);
                                    }
                                }
                                if (Sociedad == 2000)
                                {
                                    // Si no tiene emeplados acargo elimina la sociedad a la cual no pertenece el empleado
                                    usuarios_x_sociedad EmpSoc1000 = UCLIST.Where(x => x.usuario == NmrEmp && x.sociedad == 1000).FirstOrDefault();
                                    if (EmpSoc1000 != null)
                                    {
                                        ListRemoveUxS.Add(EmpSoc1000);
                                    }
                                    // si no tiene creada la sociedad a la cual pertene
                                    usuarios_x_sociedad SociedadDuplicada = SociedadACrear.Where(x => x.usuario == NmrEmp && x.sociedad == 2000).FirstOrDefault();
                                    usuarios_x_sociedad EmpSoc2000 = UCLIST.Where(x => x.usuario == NmrEmp && x.sociedad == 2000).FirstOrDefault();
                                    if (EmpSoc2000 == null && SociedadDuplicada == null)
                                    {
                                        usuarios_x_sociedad NewEmpSoc = new usuarios_x_sociedad();
                                        NewEmpSoc.sociedad = 2000;
                                        NewEmpSoc.usuario = NmrEmp;
                                        SociedadACrear.Add(NewEmpSoc);
                                    }
                                }

                            }



                        }

                    }
                    else
                    {
                        //Actualizar en caso de ser necesario.

                        //Estados
                        //1= Activo
                        //2=Inactivo

                        //tipoevaluador
                        //1-jefe
                        //2-Par
                        //3-autoevaluador

                        //TIPO EVALUACION
                        //1 Periodica
                        //2 Entrenamiento
                        bool Estado = false;
                        bool Area = false;
                        bool Telefono = false;
                        bool Cargo = false;
                        bool Sociedad2 = false;
                        bool Email = false;
                        string CodigoAreaEmpleado = Convert.ToString(Empleado.UnidadOrganizativa);
                        string CodCargoEmpleado = Convert.ToString(Empleado.CodigoSAPCargo);
                        areas areaEmp = LisAreasEvades.Where(x => x.CodigoSAP == CodigoAreaEmpleado).FirstOrDefault();
                        cargos cargoEmp = ListCargosEvades.Where(x => x.descripcion == Empleado.Cargo).FirstOrDefault();
                        var EvaPEmp = Evalauciones.Where(x => x.codigoempleado == Emp.codigo && x.Estado == 1 && x.tipoevaluador == 1).FirstOrDefault();
                        var AutoEmp = Evalauciones.Where(x => x.codigoempleado == Emp.codigo && x.Estado == 1 && x.tipoevaluador == 3).FirstOrDefault();
                        //Empleado Activo
                        if ((Emp.estado == 1 && Empleado.Activo == "NO") || (Emp.estado == 2 && Empleado.Activo == "SI") || Emp.estado == 0)
                        {
                            //INACTIVAR
                            if (Empleado.Activo == "NO")
                            {
                                Emp.estado = 2;
                                
                                //ANULA LA EVALUACIÓN EN CASO DE NO ESTAR RESUELTA
                                if (EvaPEmp!= null && AutoEmp!=null) 
                                {
                                    EvaPEmp.Estado = 6;
                                    AutoEmp.Estado = 6;
                                    var Dup = EvaluacionesAnuladas.Where(x => x.codigo == EvaPEmp.codigo).FirstOrDefault();
                                    var Dup2 = EvaluacionesAnuladas.Where(x => x.codigo == AutoEmp.codigo).FirstOrDefault();
                                    if (Dup==null && Dup2 ==null) 
                                    {
                                        EvaluacionesAnuladas.Add(EvaPEmp);
                                        EvaluacionesAnuladas.Add(AutoEmp);
                                    }
                                    
                                }
                            }

                            //ACTIVAR
                            if (Empleado.Activo == "SI")
                            {
                                Emp.estado = 1;
                            }
                            Estado = true;

                        }
                        //Actualizar Area                
                        if (areaEmp != null) {
                            if (Emp.codigoarea != areaEmp.codigo)
                            {
                                Emp.codigoarea = areaEmp.codigo;
                                Area = true;
                            }
                        }
                        else
                        {
                            areaEmp = LisAreasEvades.Where(x => x.CodigoSAP == Empleado.UnidadOrganizativa).FirstOrDefault();
                            if (areaEmp !=null) 
                            {
                                Emp.codigoarea = areaEmp.codigo;
                                Area = true;
                            }
                            else 
                            {
                                var debug = 0;
                            }
                           

                        }
                        //Actualizar Cargo                      
                        if (cargoEmp != null) {
                            if (cargoEmp.codigo != Emp.codigocargo)
                            {
                                Emp.codigocargo = cargoEmp.codigo;
                                Cargo = true;
                                encuestadores_x_empleado evaluacion = Evalauciones.Where(x => x.periodo == Periodo.codigo && x.tipoevaluador ==1 && x.Estado ==1).FirstOrDefault();
                                encuestadores_x_empleado Autoevaluacion = Evalauciones.Where(x => x.periodo == Periodo.codigo && x.tipoevaluador ==3 && x.Estado ==1).FirstOrDefault();
                                if (evaluacion !=null && Autoevaluacion != null)
                                {
                                    Empleados_Cargo_Area_x_Periodo Relacion = db2.Empleados_Cargo_Area_x_Periodo.Where(x => x.Periodo ==Periodo.codigo && x.Empleado == Emp.codigo).FirstOrDefault();
                                    if (Relacion != null)
                                    {
                                       
                                        if (Relacion.Cargo != Emp.codigocargo && EvaPEmp != null && AutoEmp != null)
                                        {
                                            if (EvaPEmp.Estado == 1 && AutoEmp.Estado == 1) 
                                            {
                                                Relacion.Cargo = Emp.codigocargo;
                                                ListTabRelActualizar.Add(Relacion);
                                            }
                                           
                                        }
                                    }
                                }
                            }
                            else 
                            {
                                encuestadores_x_empleado evaluacion = Evalauciones.Where(x => x.periodo == Periodo.codigo && x.tipoevaluador ==1 && x.Estado ==1).FirstOrDefault();
                                encuestadores_x_empleado Autoevaluacion = Evalauciones.Where(x => x.periodo == Periodo.codigo && x.tipoevaluador ==3 && x.Estado ==1).FirstOrDefault();
                                if (evaluacion !=null && Autoevaluacion != null)
                                {
                                    Empleados_Cargo_Area_x_Periodo Relacion = db2.Empleados_Cargo_Area_x_Periodo.Where(x => x.Periodo ==Periodo.codigo && x.Empleado == Emp.codigo).FirstOrDefault();
                                    if (Relacion != null)
                                    {
                                        if (Relacion.Cargo != Emp.codigocargo && EvaPEmp != null && AutoEmp != null)
                                        {
                                            if (EvaPEmp.Estado == 1 && AutoEmp.Estado == 1)
                                            {
                                                Relacion.Cargo = Emp.codigocargo;
                                                ListTabRelActualizar.Add(Relacion);
                                            }

                                        }

                                    }
                                }
                            }
                        }
                        
                         

                        //usuarios_x_sociedad EmpSoc = UCLIST.Where(x => x.usuario == NmrEmp && x.sociedad == Sociedad).FirstOrDefault();
                        //if (EmpSoc != null)
                        //{
                        //    if (EmpSoc.sociedad != Sociedad)
                        //    {

                        //        EmpSoc.sociedad = Sociedad;
                        //        Sociedad2 = true;
                        //    }

                        //}
                        //else 
                        //{
                        //    usuarios_x_sociedad NewEmpSoc = new usuarios_x_sociedad();
                        //    NewEmpSoc.sociedad = Sociedad;
                        //    NewEmpSoc.usuario = NmrEmp;
                        //    SociedadACrear.Add(NewEmpSoc);
                        //}

                        int Empleados1000 = 0;
                        int Empleados2000 = 0;
                        Estructura_Jerarquica_EVADES StrEvadeshEmp = STREvadesh.Where(x => x.Area == Empleado.AreaDescripcion).FirstOrDefault();
                        if (StrEvadeshEmp != null)
                        {
                            if (StrEvadeshEmp.Jefe == Empleado.NroEmpleado)
                            {
                                Empleados1000 = EmpeleadosAdmAutogestion.Where(x => x.Empresa == "1000" && x.AreaDescripcion == StrEvadeshEmp.Area && x.Activo == "SI").Count();
                                Empleados2000 = EmpeleadosAdmAutogestion.Where(x => x.Empresa == "2000" && x.AreaDescripcion == StrEvadeshEmp.Area && x.Activo == "SI").Count();
                            }
                            else
                            {
                                Empleados1000 = EmpeleadosAdmAutogestion.Where(x => x.Empresa == "1000" && x.Jefe == Empleado.NroEmpleado && x.Activo == "SI").Count();
                                Empleados2000 = EmpeleadosAdmAutogestion.Where(x => x.Empresa == "2000" && x.Jefe == Empleado.NroEmpleado && x.Activo == "SI").Count();
                            }
                        }
                        else
                        {
                            Empleados1000 = EmpeleadosAdmAutogestion.Where(x => x.Empresa == "1000" && x.Jefe == Empleado.NroEmpleado && x.Activo == "SI").Count();
                            Empleados2000 = EmpeleadosAdmAutogestion.Where(x => x.Empresa == "2000" && x.Jefe == Empleado.NroEmpleado && x.Activo == "SI").Count();
                        }


                        if (Empleados1000 > 0 || Empleados2000 > 0) {
                            usuarios_x_sociedad EmpSoc1000 = UCLIST.Where(x => x.usuario == NmrEmp && x.sociedad == 1000).FirstOrDefault();
                            usuarios_x_sociedad SociedadDuplicada = SociedadACrear.Where(x => x.usuario == NmrEmp && x.sociedad == 1000).FirstOrDefault();


                            if (EmpSoc1000 == null)
                            {
                                if (Empleados1000 > 0)
                                {
                                    usuarios_x_sociedad NewEmpSoc = new usuarios_x_sociedad();
                                    NewEmpSoc.sociedad = 1000;
                                    NewEmpSoc.usuario = NmrEmp;
                                    if (SociedadDuplicada == null) 
                                    {
                                        SociedadACrear.Add(NewEmpSoc);
                                    }
                                    
                                }
                            }
                            else 
                            {
                                
                                usuario_perfil PerfilADM = PerfilesUSER.Where(x => x.codigousuario == NmrEmp).FirstOrDefault();
                                if (PerfilADM == null) //Si tiene perfil de administrador No elimina la sociedad
                                {
                                    if (Empleados1000 == 0 && Sociedad !=1000) //Si no tiene Empleados Acargo elimina la sociedad
                                    {
                                        ListRemoveUxS.Add(EmpSoc1000);
                                    }
                                       
                                    
                                }
                            }

                            usuarios_x_sociedad EmpSoc2000 = UCLIST.Where(x => x.usuario == NmrEmp && x.sociedad == 2000).FirstOrDefault();
                            usuarios_x_sociedad SociedadDuplicada2 = SociedadACrear.Where(x => x.usuario == NmrEmp && x.sociedad == 2000).FirstOrDefault();

                            if (EmpSoc2000 == null) {

                                if (Empleados2000 > 0)
                                {
                                    usuarios_x_sociedad NewEmpSoc = new usuarios_x_sociedad();
                                    NewEmpSoc.sociedad = 2000;
                                    NewEmpSoc.usuario = NmrEmp;
                                    if (SociedadDuplicada2 == null) 
                                    {
                                        SociedadACrear.Add(NewEmpSoc);
                                    }
                                    
                                }
                            }
                            else
                            {
                                usuario_perfil PerfilADM = PerfilesUSER.Where(x => x.codigousuario == NmrEmp).FirstOrDefault();
                                if (PerfilADM == null) //Si tiene perfil de administrador No elimina la sociedad
                                {
                                    if (Empleados2000 == 0 && Sociedad != 2000) //Si no tiene Empleados Acargo elimina la sociedad
                                    {
                                        ListRemoveUxS.Add(EmpSoc2000);
                                    }


                                }
                            }
                        }
                        else
                        {

                            if (Sociedad == 1000)
                            {
                                // Si no tiene empelados acargo elimina la sociedad a la cual no pertenece el empleado
                                usuarios_x_sociedad EmpSoc2000 = UCLIST.Where(x => x.usuario == NmrEmp && x.sociedad == 2000).FirstOrDefault();
                                usuario_perfil PerfilADM = PerfilesUSER.Where(x => x.codigousuario == NmrEmp).FirstOrDefault();
                                if (PerfilADM==null) //Si tiene perfil de administrador No elimina la sociedad
                                {
                                    if (EmpSoc2000 != null)
                                    {
                                        ListRemoveUxS.Add(EmpSoc2000);
                                    }
                                }
                                
                                // si no tiene creada la sociedad a la cual pertene
                                usuarios_x_sociedad SociedadDuplicada = SociedadACrear.Where(x => x.usuario == NmrEmp && x.sociedad == 1000).FirstOrDefault();
                                usuarios_x_sociedad EmpSoc1000 = UCLIST.Where(x => x.usuario == NmrEmp && x.sociedad == 1000).FirstOrDefault();
                                if (EmpSoc1000 == null && SociedadDuplicada == null)
                                {
                                    usuarios_x_sociedad NewEmpSoc = new usuarios_x_sociedad();
                                    NewEmpSoc.sociedad = 1000;
                                    NewEmpSoc.usuario = NmrEmp;
                                    SociedadACrear.Add(NewEmpSoc);
                                }
                            }
                            if (Sociedad == 2000)
                            {
                                // Si no tiene empleados acargo elimina la sociedad a la cual no pertenece el empleado
                                usuarios_x_sociedad EmpSoc1000 = UCLIST.Where(x => x.usuario == NmrEmp && x.sociedad == 1000).FirstOrDefault();
                                usuario_perfil PerfilADM = PerfilesUSER.Where(x => x.codigousuario == NmrEmp).FirstOrDefault();
                                if (PerfilADM==null) //Si tiene perfil de administrador No elimina la sociedad
                                {
                                    if (EmpSoc1000 != null)
                                    {
                                        ListRemoveUxS.Add(EmpSoc1000);
                                    }
                                }
                                
                                // si no tiene creada la sociedad a la cual pertene
                                usuarios_x_sociedad SociedadDuplicada = SociedadACrear.Where(x => x.usuario == NmrEmp && x.sociedad == 2000).FirstOrDefault();
                                usuarios_x_sociedad EmpSoc2000 = UCLIST.Where(x => x.usuario == NmrEmp && x.sociedad == 2000).FirstOrDefault();
                                if (EmpSoc2000 == null && SociedadDuplicada == null)
                                {
                                    usuarios_x_sociedad NewEmpSoc = new usuarios_x_sociedad();
                                    NewEmpSoc.sociedad = 2000;
                                    NewEmpSoc.usuario = NmrEmp;
                                    SociedadACrear.Add(NewEmpSoc);
                                }
                            }

                        }

                        if (Emp.email != Empleado.Correo)
                        {
                            Emp.email = Empleado.Correo;
                            Email = true;
                        }

                        if (Emp.telefono != Empleado.Telefono && Empleado.Telefono!=null)
                        {

                            var longitud = Empleado.Telefono.Length;
                            if (longitud < 29) 
                            {
                                Emp.telefono = Empleado.Telefono;
                                Telefono = true;
                            }
                            
                        }

                        if (Estado == true || Area == true || Cargo == true || Email == true || Telefono == true)
                        {
                            EmpeleadosaActualizar.Add(Emp);
                        }


                    }

                }

                if (EmpeleadosaActualizar.Count() > 0 || SociedadAactualizar.Count() > 0 || ListTabRelActualizar.Count() > 0 || EvaluacionesAnuladas.Count() > 0)
                {
                    db2.SaveChanges();
                }
                if (EmpleadosAcrear.Count() > 0 || SociedadACrear.Count > 0)
                {
                    db2.empleados.AddRange(EmpleadosAcrear);
                    db2.Usuarios_x_sociedad.AddRange(SociedadACrear);
                    db2.SaveChanges();
                }
                if (ListRemoveUxS.Count() > 0)
                {
                    db2.Usuarios_x_sociedad.RemoveRange(ListRemoveUxS);
                    db2.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                return "" + ex;
            }
            return "Proceso Exitoso";
        }

        //---------------SEGUNDA API-----------------------------//
        [HttpGet]
        [Route("api/EvaDesempeño/HabilitadoParaEvaP")]
        public string HabilitadoParaEvaP()
        {
            string respuesta = "";
            try
            {
                DateTime Hoy = DateTime.Now;
                DateTime Hoy2 = Hoy.AddYears(-2);
                int Año = Hoy.Year;
                Periodos_Evaluacion_Desempeño ConfigPeriodoP2 = db2.Periodos_Evaluacion_Desempeño.Where(x => x.Año == Año).FirstOrDefault();
                var listPeriodicas = db2.Encuestadoresxempleado.Where(x => x.tipoevaluacion == 1 && x.fecharegistro >= Hoy2).ToList();

                var config = db.Configuraciones.Where(x => x.Parametro == "TiempoEvaDP").FirstOrDefault();
                int dias = Convert.ToInt32(config.Valor);
                DateTime ingreso = Hoy.AddDays(-dias);
                //List<Empleado> empleadosList = db.Empleados.Where(x => x.FechaIngreso <= ingreso && x.Activo == "SI" && (x.NroEmpleado == "40006793" || x.NroEmpleado == "40003700" || x.NroEmpleado== "40000282")).ToList();
                List<Empleado> empleadosList = db.Empleados.Where(x => x.FechaIngreso <= ingreso && x.Activo == "SI" && (x.Jefe != "" && x.Jefe != null) && x.TipoArea != "SENA Pctivo/Pasante" && x.TipoArea != "SENA Lectivo").ToList();
                List<encuestadores_x_empleado> HabilitarEva = new List<encuestadores_x_empleado>();
                List<Empleados_Cargo_Area_x_Periodo> ListEvaP = new List<Empleados_Cargo_Area_x_Periodo>();


                if (ConfigPeriodoP2 != null)
                {
                    DateTime FechaI = ConfigPeriodoP2.FechaInicial;
                    DateTime FechaF = ConfigPeriodoP2.FechaFinal;

                    List<cargos> listCargosEvades = db2.Cargos.ToList();
                    List<areas> ListAreasEvades = db2.areas.ToList();
                    periodos Periodo1000 = db2.Periodos.Where(x => x.Parametro_Id == ConfigPeriodoP2.Id && x.sociedad == 1000 && x.TipoPeriodo == 1).FirstOrDefault();
                    periodos Periodo2000 = db2.Periodos.Where(x => x.Parametro_Id == ConfigPeriodoP2.Id && x.sociedad == 2000 && x.TipoPeriodo == 1).FirstOrDefault();
                    List<Estructura_Jerarquica_EVADES> EstructurasEvades = db2.Estructura_Jerarquica_EVADES.ToList();
                    if (empleadosList.Count > 0)
                    {

                        List<periodos> PeriodosN = new List<periodos>();

                        if (Periodo1000 == null)
                        {
                            periodos NewPeriodo = new periodos();
                            NewPeriodo.fechaincio = FechaI;
                            NewPeriodo.fechafinal = FechaF;
                            NewPeriodo.sociedad = 1000;
                            NewPeriodo.TipoPeriodo = 1;
                            PeriodosN.Add(NewPeriodo);

                        }
                        if (Periodo2000 == null)
                        {
                            periodos NewPeriodo2 = new periodos();
                            NewPeriodo2.fechaincio = FechaI;
                            NewPeriodo2.fechafinal = FechaF;
                            NewPeriodo2.sociedad = 2000;
                            NewPeriodo2.TipoPeriodo = 1;
                            PeriodosN.Add(NewPeriodo2);


                        }
                        if (PeriodosN.Count() > 0)
                        {
                            db2.Periodos.AddRange(PeriodosN);
                            db2.SaveChanges();
                            Periodo1000 = db2.Periodos.Where(x => x.Parametro_Id == ConfigPeriodoP2.Id && x.sociedad == 1000).FirstOrDefault();
                            Periodo2000 = db2.Periodos.Where(x => x.Parametro_Id == ConfigPeriodoP2.Id && x.sociedad == 2000).FirstOrDefault();

                        }

                    }
                    List<Empleados_Cargo_Area_x_Periodo> ListDuplicados1000 = db2.Empleados_Cargo_Area_x_Periodo.Where(x => x.Periodo == Periodo1000.codigo).ToList();
                    List<Empleados_Cargo_Area_x_Periodo> ListDuplicados2000 = db2.Empleados_Cargo_Area_x_Periodo.Where(x => x.Periodo == Periodo2000.codigo).ToList();

                    foreach (var empleado in empleadosList)
                    {
                        encuestadores_x_empleado Eval = new encuestadores_x_empleado();
                        encuestadores_x_empleado AutEval = new encuestadores_x_empleado();
                        Empleados_Cargo_Area_x_Periodo NewRelation = new Empleados_Cargo_Area_x_Periodo();
                        Empleados_Cargo_Area_x_Periodo NewRelationJEFE = new Empleados_Cargo_Area_x_Periodo();
                        //Crear registro en evaluacion Periodica: encuestadores x empleado
                        var NMREMP = Convert.ToInt32(empleado.NroEmpleado);
                        var NMREMPJEFE = Convert.ToInt32(empleado.Jefe);
                        DateTime fecharegistro;
                        int PeriodoCod = 0;
                        int PeriodoCod2 = 0;
                        if (empleado.Empresa == "1000")
                        {
                            PeriodoCod = Periodo1000.codigo;
                            PeriodoCod2 = Periodo2000.codigo;
                        }
                        if (empleado.Empresa == "2000")
                        {
                            PeriodoCod = Periodo2000.codigo;
                            PeriodoCod2 = Periodo1000.codigo;
                        }
                        var estructuraasist = EstructurasEvades.Where(x => x.Area == empleado.AreaDescripcion).FirstOrDefault();
                        encuestadores_x_empleado peridioca = listPeriodicas.Where(x => x.codigoempleado == NMREMP && x.tipoevaluacion == 1 && x.codigoempleado != x.codigoevaluador && (x.periodo == PeriodoCod || x.periodo == PeriodoCod2)).OrderByDescending(x => x.fecharegistro).FirstOrDefault();
                        encuestadores_x_empleado AutoevalucionPeriodica = listPeriodicas.Where(x => x.codigoempleado == NMREMP && x.tipoevaluacion == 1 && x.codigoempleado == x.codigoevaluador && (x.periodo == PeriodoCod || x.periodo == PeriodoCod2)).OrderByDescending(x => x.fecharegistro).FirstOrDefault();

                        if (NMREMP == 40004133)
                        {
                            int debug = 0;
                        }
                        if (peridioca != null)
                        {
                            int AñoIngreso = Convert.ToInt32(empleado.FechaIngreso.Value.Year);
                            int AñoActual = Convert.ToInt32(Hoy.Year);
                            int DiferenciaAños = AñoActual - AñoIngreso;
                            fecharegistro = Convert.ToDateTime(peridioca.fecharegistro);
                            DateTime FechaProxEva = empleado.FechaIngreso.Value.AddYears(DiferenciaAños);

                            var fecharegistro2 = fecharegistro.AddDays(dias);
                            if (FechaProxEva <= Hoy)
                            {

                                var EvaPres = listPeriodicas.Where(x => x.codigoempleado == NMREMP && x.periodo == PeriodoCod).FirstOrDefault();
                                if (EvaPres == null)
                                {
                                    //Evaluacion Periodica
                                    Eval.codigoempleado = NMREMP;
                                    var Emp = db2.empleados.Where(x => x.codigo == Eval.codigoempleado).FirstOrDefault();
                                    if (Emp!=null) 
                                    {
                                    
                                   
                                    //***DEFINIR EVALUADOR**
                                    if (estructuraasist != null && empleado.TipoArea == "Asistenciales CO")
                                    {
                                        int nmrJefe = Convert.ToInt32(estructuraasist.Jefe);
                                            if (empleado.NroEmpleado == estructuraasist.Jefe) 
                                            {
                                                nmrJefe = Convert.ToInt32(estructuraasist.Superior);
                                            }
                                        Eval.codigoevaluador = nmrJefe;

                                    }
                                    else
                                    {
                                        if (empleado.Jefe != "")
                                        {
                                            Eval.codigoevaluador = NMREMPJEFE;
                                        }
                                        else
                                        {
                                            Eval.codigoevaluador = 0;//JEFE NO ESTA DEFINIDO EN AUTOGESTION 
                                        }
                                    }
                                    //******************
                                    Eval.tipoevaluacion = 1;
                                    Eval.tipoevaluador = 1;
                                    Eval.fecharegistro = Hoy;
                                    Eval.horaregistro = Hoy.TimeOfDay;
                                    Eval.usuarioregistro = 99999999;//No aplica
                                    Eval.Estado = 1;

                                    //Autoevaluacion
                                    AutEval.codigoempleado = Convert.ToInt32(empleado.NroEmpleado);
                                    AutEval.codigoevaluador = Convert.ToInt32(empleado.NroEmpleado);
                                    AutEval.tipoevaluacion = 1;
                                    AutEval.tipoevaluador = 3;
                                    AutEval.fecharegistro = Hoy;
                                    AutEval.horaregistro = Hoy.TimeOfDay;
                                    AutEval.usuarioregistro = 99999999;//No aplica
                                    AutEval.Estado = 1;

                                    if (empleado.Empresa == "1000")
                                    {
                                        Eval.periodo = Periodo1000.codigo;
                                        NewRelation.Periodo = Periodo1000.codigo;
                                        AutEval.periodo = Periodo1000.codigo;
                                    }
                                    if (empleado.Empresa == "2000")
                                    {
                                        Eval.periodo = Periodo2000.codigo;
                                        NewRelation.Periodo = Periodo2000.codigo;
                                        AutEval.periodo = Periodo2000.codigo;
                                    }
                                    var Jefe = db2.empleados.Where(x => x.codigo == NMREMPJEFE).FirstOrDefault();
                                    if (Jefe != null)
                                    {
                                        var socjefe = db2.Usuarios_x_sociedad.Where(x => x.usuario == NMREMPJEFE).FirstOrDefault();
                                        int PeridoJefe = 0;
                                        if (socjefe != null)
                                        {
                                            if (socjefe.sociedad == 1000)
                                            {
                                                PeridoJefe = Periodo1000.codigo;
                                            }
                                            if (socjefe.sociedad == 2000)
                                            {
                                                PeridoJefe = Periodo2000.codigo;
                                            }
                                        }

                                        NewRelationJEFE.Empleado = NMREMPJEFE;

                                        NewRelationJEFE.Area = Jefe.codigoarea;
                                        NewRelationJEFE.Cargo = Jefe.codigocargo;
                                        string areaJefe = Convert.ToString(Emp.codigoarea);
                                        //string copdSAPcargoJefe = Convert.ToString(empleado.CodigoSAPCargo);
                                        areas empleadoareaJefe = ListAreasEvades.Where(x => x.codigo == Jefe.codigoarea).FirstOrDefault();
                                        cargos empleadocargoJefe = listCargosEvades.Where(x => x.codigo == Jefe.codigocargo).FirstOrDefault();
                                        if (empleadoareaJefe != null)
                                        {
                                            NewRelationJEFE.Area = empleadoareaJefe.codigo;
                                        }
                                        else
                                        {
                                            NewRelationJEFE.Area = 0;
                                        }
                                        if (empleadocargoJefe != null)
                                        {
                                            NewRelationJEFE.Cargo = empleadocargoJefe.codigo;
                                        }
                                        else
                                        {
                                            NewRelationJEFE.Cargo = 0;
                                        }
                                        NewRelationJEFE.Periodo = PeridoJefe;
                                        bool HayDuplicadosJefe = false;
                                        if (socjefe.sociedad == 1000)
                                        {
                                            var duplicadosJefe = ListDuplicados1000.Where(x => x.Empleado == NMREMPJEFE && x.Periodo == Periodo1000.codigo).FirstOrDefault();
                                            var duplicadosJefe2 = ListEvaP.Where(x => x.Empleado == NMREMPJEFE && x.Periodo == Periodo1000.codigo).FirstOrDefault();//listado a crear
                                            if (duplicadosJefe != null || duplicadosJefe2 != null)
                                            {
                                                HayDuplicadosJefe = true;
                                            }

                                        }
                                        else if (socjefe.sociedad == 2000)
                                        {
                                            var duplicadosJefe = ListDuplicados2000.Where(x => x.Empleado == NMREMPJEFE && x.Periodo == Periodo2000.codigo).FirstOrDefault();//base de datos
                                            var duplicadosJefe2 = ListEvaP.Where(x => x.Empleado == NMREMPJEFE && x.Periodo == Periodo2000.codigo).FirstOrDefault();//listado a crear
                                            if (duplicadosJefe != null || duplicadosJefe2 != null)
                                            {
                                                HayDuplicadosJefe = true;
                                            }
                                        }
                                        if (HayDuplicadosJefe == false)
                                        {
                                                if (NewRelationJEFE.Empleado == 50001768)
                                                {
                                                    var debug = "";
                                                }
                                                var Duplicado2 = ListDuplicados1000.Where(x => x.Empleado == NewRelationJEFE.Empleado && x.Periodo == NewRelationJEFE.Periodo).FirstOrDefault();
                                                var Duplicado2_0 = ListDuplicados2000.Where(x => x.Empleado == NewRelationJEFE.Empleado && x.Periodo == NewRelationJEFE.Periodo).FirstOrDefault();
                                                if (Duplicado2 == null && Duplicado2_0 == null)
                                                {
                                                    ListEvaP.Add(NewRelationJEFE);
                                                }
                                               
                                        }



                                        NewRelation.Empleado = NMREMP;
                                        var socEmp = db2.Usuarios_x_sociedad.Where(x => x.usuario == NMREMP).FirstOrDefault();
                                        //string copdSAPareaEmp = Convert.ToString(empleado.CodigoSAPArea);
                                        //string copdSAPcargoEmp = Convert.ToString(empleado.CodigoSAPCargo);
                                        areas empleadoarea = ListAreasEvades.Where(x => x.codigo == Emp.codigoarea).FirstOrDefault();
                                        cargos empleadocargo = listCargosEvades.Where(x => x.codigo == Emp.codigocargo).FirstOrDefault();
                                        if (empleadoarea != null)
                                        {
                                            NewRelation.Area = empleadoarea.codigo; //DATO TEMPORAL?
                                        }
                                        else
                                        {
                                            NewRelation.Area = 0;
                                        }
                                        if (empleadocargo != null)
                                        {
                                            NewRelation.Cargo = empleadocargo.codigo;//DATO TEMPORAL?
                                        }
                                        else
                                        {
                                            NewRelation.Cargo = 0;
                                        }
                                        //NewRelation.Area = Emp.codigoarea;// DATO TEMPORAL
                                        //NewRelation.Cargo = Emp.codigocargo;// DATO TEMPORAL
                                        HabilitarEva.Add(Eval);
                                        HabilitarEva.Add(AutEval);

                                        //Empleados_Cargo_Area_x_Periodo RelacionePECAEMP = db2.Empleados_Cargo_Area_x_Periodo.Where(x => x.Periodo == Eval.periodo && x.Empleado == NMREMP).FirstOrDefault();
                                        bool HayDuplicados = false;
                                        if (socEmp.sociedad == 1000)
                                        {
                                            if (NewRelation.Empleado == 40001088)
                                            {
                                                var debug = "";
                                            }
                                            var duplicadosEmp = ListDuplicados1000.Where(x => x.Empleado == NMREMP && x.Periodo == Periodo1000.codigo).FirstOrDefault();
                                            var duplicadosEmp2 = ListEvaP.Where(x => x.Empleado == NMREMP && x.Periodo == Periodo1000.codigo).FirstOrDefault();//listado a crear
                                            if (duplicadosEmp != null || duplicadosEmp2 != null)
                                            {
                                                HayDuplicados = true;
                                            }

                                        }
                                        else if (socEmp.sociedad == 2000)
                                        {
                                            if (NewRelation.Empleado == 40001088)
                                            {
                                                var debug = "";
                                            }
                                            var duplicadosEmp = ListDuplicados2000.Where(x => x.Empleado == NMREMP && x.Periodo == Periodo2000.codigo).FirstOrDefault();//base de datos
                                            var duplicadosEmp2 = ListEvaP.Where(x => x.Empleado == NMREMP && x.Periodo == Periodo2000.codigo).FirstOrDefault();//listado a crear
                                            if (duplicadosEmp != null || duplicadosEmp2 != null)
                                            {
                                                HayDuplicados = true;
                                            }

                                        }
                                        if (HayDuplicados == false)
                                        {
                                                if (NewRelation.Empleado == 50001768)
                                                {
                                                    var debug = 0;
                                                }
                                                var Duplicado2 = ListDuplicados1000.Where(x => x.Empleado == NewRelation.Empleado && x.Periodo == NewRelation.Periodo).FirstOrDefault();
                                                var Duplicado2_0 = ListDuplicados2000.Where(x => x.Empleado == NewRelation.Empleado && x.Periodo == NewRelation.Periodo).FirstOrDefault();
                                                if (Duplicado2 == null && Duplicado2_0 == null)
                                                {
                                                    ListEvaP.Add(NewRelation);
                                                }
                                             
                                        }

                                    }
                                }
                               }
                            }

                        }
                        else
                        {
                            int AñoIngreso = Convert.ToInt32(empleado.FechaIngreso.Value.Year);
                            int AñoActual = Convert.ToInt32(Hoy.Year);
                            int DiferenciaAños = AñoActual - AñoIngreso;
                            //fecharegistro = Convert.ToDateTime(peridioca.fecharegistro);
                            DateTime FechaProxEva = empleado.FechaIngreso.Value.AddYears(DiferenciaAños);

                            if (FechaProxEva <= Hoy)
                            {
                                //var fecharegistro2 = fecharegistro.AddDays(dias);

                                var EvaPres = listPeriodicas.Where(x => x.codigoempleado == NMREMP && x.periodo == PeriodoCod).FirstOrDefault();
                                if (EvaPres == null)
                                {
                                    //Evaluacion Periodica
                                    Eval.codigoempleado = NMREMP;
                                    var Emp = db2.empleados.Where(x => x.codigo == NMREMP).FirstOrDefault();
                                    if (Emp!=null) 
                                    {
                                        //***DEFINIR EVALUADOR**
                                        if (estructuraasist != null && empleado.TipoArea == "Asistenciales CO")
                                        {
                                            int nmrJefe = Convert.ToInt32(estructuraasist.Jefe);
                                            Eval.codigoevaluador = nmrJefe;

                                        }
                                        else
                                        {
                                            if (empleado.Jefe != "")
                                            {
                                                Eval.codigoevaluador = NMREMPJEFE;
                                            }
                                            else
                                            {
                                                Eval.codigoevaluador = 0;//JEFE NO ESTA DEFINIDO EN AUTOGESTION 
                                            }
                                        }
                                        //******************
                                        Eval.tipoevaluacion = 1;
                                        Eval.tipoevaluador = 1;
                                        Eval.fecharegistro = Hoy;
                                        Eval.horaregistro = Hoy.TimeOfDay;
                                        Eval.usuarioregistro = 99999999;//No aplica
                                        Eval.Estado = 1;
                                        //Autoevaluacion
                                        AutEval.codigoempleado = Convert.ToInt32(empleado.NroEmpleado);
                                        AutEval.codigoevaluador = Convert.ToInt32(empleado.NroEmpleado);
                                        AutEval.tipoevaluacion = 1;
                                        AutEval.tipoevaluador = 3;
                                        AutEval.fecharegistro = Hoy;
                                        AutEval.horaregistro = Hoy.TimeOfDay;
                                        AutEval.usuarioregistro = 99999999;//No aplica
                                        AutEval.Estado = 1;

                                        if (empleado.Empresa == "1000")
                                        {
                                            Eval.periodo = Periodo1000.codigo;
                                            NewRelation.Periodo = Periodo1000.codigo;
                                            AutEval.periodo = Periodo1000.codigo;
                                        }
                                        if (empleado.Empresa == "2000")
                                        {
                                            Eval.periodo = Periodo2000.codigo;
                                            NewRelation.Periodo = Periodo2000.codigo;
                                            AutEval.periodo = Periodo2000.codigo;
                                        }
                                        var Jefe = db2.empleados.Where(x => x.codigo == NMREMPJEFE).FirstOrDefault();
                                        var socjefe = db2.Usuarios_x_sociedad.Where(x => x.usuario == NMREMPJEFE).FirstOrDefault();
                                        if (Jefe != null)
                                        {
                                            int PeridoJefe = 0;
                                            if (socjefe != null)
                                            {
                                                if (socjefe.sociedad == 1000)
                                                {
                                                    PeridoJefe = Periodo1000.codigo;
                                                }
                                                if (socjefe.sociedad == 2000)
                                                {
                                                    PeridoJefe = Periodo2000.codigo;
                                                }
                                            }

                                            NewRelationJEFE.Empleado = NMREMPJEFE;
                                            //NewRelationJEFE.Area = Jefe.codigoarea;
                                            //NewRelationJEFE.Cargo = Jefe.codigocargo;
                                            string copdSAPareaJefe = Convert.ToString(Jefe.codigoarea);
                                            //string copdSAPcargoJefe = Convert.ToString(empleado.CodigoSAPCargo);
                                            areas empleadoareaJefe = ListAreasEvades.Where(x => x.codigo == Jefe.codigoarea).FirstOrDefault();
                                            cargos empleadocargoJefe = listCargosEvades.Where(x => x.codigo == Jefe.codigocargo).FirstOrDefault();
                                            if (empleadoareaJefe != null)
                                            {
                                                NewRelationJEFE.Area = empleadoareaJefe.codigo;
                                            }
                                            else
                                            { NewRelationJEFE.Area = 0; }
                                            if (empleadocargoJefe != null)
                                            {
                                                NewRelationJEFE.Cargo = empleadocargoJefe.codigo;
                                            }
                                            else
                                            { NewRelationJEFE.Cargo = 0; }
                                            NewRelationJEFE.Periodo = PeridoJefe;
                                            bool HayDuplicadosJefe = false;
                                            if (socjefe.sociedad == 1000)
                                            {
                                                
                                                var duplicadosJefe = ListDuplicados1000.Where(x => x.Empleado == NMREMPJEFE && x.Periodo == Periodo1000.codigo).FirstOrDefault();
                                                var duplicadosJefe2 = ListEvaP.Where(x => x.Empleado == NMREMPJEFE && x.Periodo == Periodo1000.codigo).FirstOrDefault();//listado a crear
                                                if (duplicadosJefe != null || duplicadosJefe2 != null)
                                                {
                                                    HayDuplicadosJefe = true;
                                                }

                                            }
                                            else if (socjefe.sociedad == 2000)
                                            {
                                                
                                                var duplicadosJefe = ListDuplicados2000.Where(x => x.Empleado == NMREMPJEFE && x.Periodo == Periodo2000.codigo).FirstOrDefault();//base de datos
                                                var duplicadosJefe2 = ListEvaP.Where(x => x.Empleado == NMREMPJEFE && x.Periodo == Periodo2000.codigo).FirstOrDefault();//listado a crear
                                                if (duplicadosJefe != null || duplicadosJefe2 != null)
                                                {
                                                    HayDuplicadosJefe = true;
                                                }

                                            }
                                            if (HayDuplicadosJefe == false)
                                            {
                                                if (NewRelationJEFE.Empleado == 50001768)
                                                {
                                                    var debug = 0;
                                                }
                                                var Duplicado2 = ListDuplicados1000.Where(x => x.Empleado == NewRelationJEFE.Empleado && x.Periodo == NewRelationJEFE.Periodo).FirstOrDefault();
                                                var Duplicado2_0 = ListDuplicados2000.Where(x => x.Empleado == NewRelationJEFE.Empleado && x.Periodo == NewRelationJEFE.Periodo).FirstOrDefault();
                                                if (Duplicado2 == null && Duplicado2_0 == null)
                                                {
                                                    ListEvaP.Add(NewRelationJEFE);
                                                }
                                                
                                            }



                                            NewRelation.Empleado = NMREMP;
                                            var socEmp = db2.Usuarios_x_sociedad.Where(x => x.usuario == NMREMP).FirstOrDefault();
                                            string areaEmp = Convert.ToString(Emp.codigoarea);
                                            string cargoEmp = Convert.ToString(empleado.CodigoSAPCargo);
                                            var empleadoarea = ListAreasEvades.Where(x => x.codigo == Emp.codigoarea).FirstOrDefault();
                                            var empleadocargo = listCargosEvades.Where(x => x.codigo == Emp.codigocargo).FirstOrDefault();
                                            if (empleadoarea != null)
                                            {
                                                NewRelation.Area = empleadoarea.codigo; //DATO TEMPORAL?
                                            }
                                            else
                                            { NewRelation.Area = 0; }
                                            if (empleadocargo != null)
                                            {
                                                NewRelation.Cargo = empleadocargo.codigo;//DATO TEMPORAL?
                                            }
                                            else
                                            { NewRelation.Cargo = 0; }
                                            //NewRelation.Area = Emp.codigoarea;// DATO TEMPORAL
                                            //NewRelation.Cargo = Emp.codigocargo;// DATO TEMPORAL
                                            HabilitarEva.Add(Eval);
                                            HabilitarEva.Add(AutEval);

                                            //Empleados_Cargo_Area_x_Periodo RelacionePECAEMP = db2.Empleados_Cargo_Area_x_Periodo.Where(x => x.Periodo == Eval.periodo && x.Empleado == NMREMP).FirstOrDefault();
                                            bool HayDuplicados = false;
                                            if (socEmp.sociedad == 1000)
                                            {
                                               
                                                var duplicadosEmp = ListDuplicados1000.Where(x => x.Empleado == NMREMP && x.Periodo == Periodo1000.codigo).FirstOrDefault();
                                                var duplicadosEmp2 = ListEvaP.Where(x => x.Empleado == NMREMP && x.Periodo == Periodo1000.codigo).FirstOrDefault();//listado a crear
                                                if (duplicadosEmp != null || duplicadosEmp2 != null)
                                                {
                                                    HayDuplicados = true;
                                                }

                                            }
                                            else if (socEmp.sociedad == 2000)
                                            {
                                                var duplicadosEmp = ListDuplicados2000.Where(x => x.Empleado == NMREMP && x.Periodo == Periodo2000.codigo).FirstOrDefault();//base de datos
                                                var duplicadosEmp2 = ListEvaP.Where(x => x.Empleado == NMREMP && x.Periodo == Periodo2000.codigo).FirstOrDefault();//listado a crear
                                                if (duplicadosEmp != null || duplicadosEmp2 != null)
                                                {
                                                    HayDuplicados = true;
                                                }

                                            }
                                            if (HayDuplicados == false)
                                            {
                                                //if (NewRelation.Empleado == 50001768) 
                                                //{
                                                //    var debug = 0;
                                                //}
                                                var Duplicado2= ListDuplicados1000.Where(x => x.Empleado == NewRelation.Empleado && x.Periodo == NewRelation.Periodo).FirstOrDefault();
                                                var Duplicado2_0 = ListDuplicados2000.Where(x => x.Empleado == NewRelation.Empleado && x.Periodo == NewRelation.Periodo).FirstOrDefault();
                                                if (Duplicado2 == null && Duplicado2_0 ==null) 
                                                {
                                                    ListEvaP.Add(NewRelation);
                                                }
                                                
                                            }
                                        }
                                    }
                                    

                                }

                            }
                        }


                    }

                    if (HabilitarEva.Count() > 0)
                    {
                        db2.Encuestadoresxempleado.AddRange(HabilitarEva);
                        db2.Empleados_Cargo_Area_x_Periodo.AddRange(ListEvaP);

                        db2.SaveChanges();
                        return respuesta = "Activacion Exitosa";
                    }
                    if (respuesta == "")
                    {

                        respuesta = "No hay Evaluaciones pendientes";
                    }
                }
                else
                {
                    respuesta = "Es necesario definir los parametros del Perido en la DB";
                }
                return respuesta;

            }
            catch (Exception ex)
            {
                return respuesta + "Error:" + ex;
            }
        }


        //---------------TERCERA API-----------------------------//
        [HttpGet]
        [Route("api/EvaDesempeño/HabilitadoParaEvaE")]
        public string HabilitadoParaEvaE()
        {
            string respuesta = "";
            try
            {
                List<encuestadores_x_empleado> EvaEntremaniento = new List<encuestadores_x_empleado>();
                List<Empleados_Cargo_Area_x_Periodo> EvaEntremaniento2 = new List<Empleados_Cargo_Area_x_Periodo>();
                var Hoy = DateTime.Now;
                var mes = Hoy.Month;
                var dia = Hoy.Day;
                var año = Hoy.Year;
                var Fecha = "" + dia + "/" + mes + "/" + año;
                DateTime FechaHoy = Convert.ToDateTime(Fecha);
                List<Estructura_Jerarquica_EVADES> EstructurasEvades = db2.Estructura_Jerarquica_EVADES.ToList();
                var ConfigEntrenamiento = db.Configuraciones.Where(x => x.Parametro == "PeriodoEvaDE").FirstOrDefault();//cantidad en días para habilitar la evaluacion
                var ConfigEntrenamiento2 = db.Configuraciones.Where(x => x.Parametro == "TiempoEvaDE").FirstOrDefault();//cantidad en días para presentar la evaluación
                int CantDiasPeridodo = Convert.ToInt32(ConfigEntrenamiento.Valor);
                int CantDias = Convert.ToInt32(ConfigEntrenamiento2.Valor);
                DateTime CantDiasNeg = FechaHoy.AddDays(-CantDias);
                DateTime CantDiasPos = FechaHoy.AddDays(CantDias);

                List<cargos> listCargosEvades = db2.Cargos.ToList();
                List<areas> ListAreasEvades = db2.areas.ToList();
                List<Empleado> empleados = db.Empleados.Where(x => x.FechaIngreso == CantDiasNeg && x.Activo == "SI").ToList();
                //List<Empleado> empleados = db.Empleados.Where(x => x.FechaIngreso == CantDiasNeg && x.NroEmpleado == "40001088" || x.NroEmpleado == "40000282").ToList();
                var fechaHoy = DateTime.Now;

                //------------------OBTENER ULTIMOS PERIODOS Y CREAR EN CASO TAL PERIODICO--------------//

                periodos PeriodoEntrenamiento1000 = db2.Periodos.Where(x => x.fechaincio == FechaHoy && x.fechafinal == CantDiasPos && x.sociedad == 1000).FirstOrDefault();
                periodos PeriodoEntrenamiento2000 = db2.Periodos.Where(x => x.fechaincio == FechaHoy && x.fechafinal == CantDiasPos && x.sociedad == 2000).FirstOrDefault();

                if (empleados.Count > 0)
                {

                    List<periodos> PeriodosN = new List<periodos>();
                    var Count1000 = empleados.Where(x => x.Empresa == "1000").Count();
                    var Count2000 = empleados.Where(x => x.Empresa == "2000").Count();

                    if (PeriodoEntrenamiento1000 == null && Count1000 > 0)
                    {
                        periodos NewPeriodo = new periodos();
                        periodos NewPeriodo2 = new periodos();
                        NewPeriodo.fechaincio = FechaHoy;
                        NewPeriodo.fechafinal = FechaHoy.AddDays(CantDias);
                        NewPeriodo.sociedad = 1000;
                        NewPeriodo.TipoPeriodo = 2;
                        PeriodosN.Add(NewPeriodo);

                    }
                    if (PeriodoEntrenamiento2000 == null && Count2000 > 0)
                    {
                        periodos NewPeriodo2 = new periodos();

                        NewPeriodo2.fechaincio = FechaHoy;
                        NewPeriodo2.fechafinal = FechaHoy.AddDays(CantDias);
                        NewPeriodo2.sociedad = 2000;
                        NewPeriodo2.TipoPeriodo = 2;
                        PeriodosN.Add(NewPeriodo2);


                    }
                    if (PeriodosN.Count() > 0)
                    {
                        db2.Periodos.AddRange(PeriodosN);
                        db2.SaveChanges();
                        PeriodoEntrenamiento1000 = db2.Periodos.Where(x => x.fechaincio == FechaHoy && x.fechafinal == CantDiasPos && x.sociedad == 1000).FirstOrDefault();
                        PeriodoEntrenamiento2000 = db2.Periodos.Where(x => x.fechafinal == FechaHoy && x.fechafinal == CantDiasPos && x.sociedad == 2000).FirstOrDefault();

                    }

                }

                //----------------------FIN--------------------------//
                foreach (var empleado in empleados)
                {
                    var NmrEmp = Convert.ToInt32(empleado.NroEmpleado);
                    var NmrJefe = 0;
                    var Emp = db2.empleados.Where(x => x.codigo == NmrEmp).FirstOrDefault();
                    if (empleado.Jefe != "" && empleado.Jefe != null) { NmrJefe = Convert.ToInt32(empleado.Jefe); }
                    //Entrenamiento
                    encuestadores_x_empleado newAutoEv = new encuestadores_x_empleado();
                    Empleados_Cargo_Area_x_Periodo NewRelation = new Empleados_Cargo_Area_x_Periodo();
                    Empleados_Cargo_Area_x_Periodo NewRelJEFE = new Empleados_Cargo_Area_x_Periodo();
                    newAutoEv.tipoevaluacion = 2;
                    newAutoEv.codigoempleado = NmrEmp;
                    //***DEFINIR EVALUADOR**
                    var estructuraasist = EstructurasEvades.Where(x => x.Area == empleado.AreaDescripcion).FirstOrDefault();
                    if (estructuraasist != null && empleado.TipoArea == "Asistenciales CO")
                    {
                        int nmrJefestr = Convert.ToInt32(estructuraasist.Jefe);
                        newAutoEv.codigoevaluador = nmrJefestr;

                    }
                    else
                    {
                        newAutoEv.codigoevaluador = NmrJefe;
                    }

                    newAutoEv.tipoevaluador = 1;
                    newAutoEv.fecharegistro = DateTime.Now;
                    newAutoEv.horaregistro = Hoy.TimeOfDay;
                    newAutoEv.usuarioregistro = 99999999; //No aplica
                    newAutoEv.Estado = 1;
                    if (empleado.Empresa == "1000")
                    {
                        newAutoEv.periodo = PeriodoEntrenamiento1000.codigo;
                        NewRelation.Periodo = PeriodoEntrenamiento1000.codigo;
                        NewRelJEFE.Periodo = PeriodoEntrenamiento1000.codigo;
                    }
                    if (empleado.Empresa == "2000")
                    {
                        NewRelJEFE.Periodo = PeriodoEntrenamiento2000.codigo;
                        newAutoEv.periodo = PeriodoEntrenamiento2000.codigo;
                        NewRelation.Periodo = PeriodoEntrenamiento2000.codigo;
                    }
                    NewRelation.Empleado = NmrEmp;
                    string copdSAPareaEmp = Convert.ToString(empleado.CodigoSAPArea);
                    string copdSAPcargoEmp = Convert.ToString(empleado.CodigoSAPCargo);
                    var empleadoarea = ListAreasEvades.Where(x => x.CodigoSAP == copdSAPareaEmp).FirstOrDefault();
                    var empleadocargo = listCargosEvades.Where(x => x.CodigoSAP == copdSAPcargoEmp).FirstOrDefault();
                    if (empleadoarea != null) {
                        NewRelation.Area = empleadoarea.codigo; //DATO TEMPORAL?
                    } else { NewRelation.Area = 0; }
                    if (empleadocargo != null) {
                        NewRelation.Cargo = empleadocargo.codigo;//DATO TEMPORAL?
                    }
                    else { NewRelation.Cargo = 0; }

                    var DatosJefe = db2.empleados.Where(x => x.codigo == NmrJefe).FirstOrDefault();
                    string copdSAPareaJefe = Convert.ToString(DatosJefe.codigoarea);
                    string copdSAPcargoJefe = Convert.ToString(DatosJefe.codigocargo);
                    var empleadoareaJefe = ListAreasEvades.Where(x => x.codigo == DatosJefe.codigoarea).FirstOrDefault();
                    var empleadocargoJefe = listCargosEvades.Where(x => x.codigo == DatosJefe.codigocargo).FirstOrDefault();

                    NewRelJEFE.Empleado = DatosJefe.codigo;
                    if (empleadoareaJefe != null) {
                        NewRelJEFE.Area = empleadoareaJefe.codigo;
                    }
                    else { NewRelJEFE.Area = 0; }
                    if (empleadocargoJefe != null) {
                        NewRelJEFE.Cargo = empleadocargoJefe.codigo;
                    } else
                    {
                        NewRelJEFE.Cargo = 0;
                    }
                    encuestadores_x_empleado VerificarduplicadosEmp = EvaEntremaniento.Find(x => x.periodo == NewRelation.Periodo && x.codigoempleado == NewRelation.Empleado);
                    Empleados_Cargo_Area_x_Periodo VerificarduplicadosEmp2 = EvaEntremaniento2.Find(x => x.Periodo == NewRelation.Periodo && x.Empleado == NewRelation.Empleado);

                    if (VerificarduplicadosEmp == null)
                    {
                        EvaEntremaniento.Add(newAutoEv);
                    }
                    if (VerificarduplicadosEmp == null)
                    {
                        EvaEntremaniento2.Add(NewRelation);
                    }
                    Empleados_Cargo_Area_x_Periodo Verificarduplicadosjefe = EvaEntremaniento2.Find(x => x.Periodo == NewRelJEFE.Periodo && x.Empleado == NewRelJEFE.Empleado);
                    if (Verificarduplicadosjefe == null)
                    {
                        EvaEntremaniento2.Add(NewRelJEFE);
                    }






                }
                if (EvaEntremaniento.Count() > 0)
                {
                    db2.Encuestadoresxempleado.AddRange(EvaEntremaniento);
                    db2.Empleados_Cargo_Area_x_Periodo.AddRange(EvaEntremaniento2);
                    db2.SaveChanges();
                    respuesta = "Las pruebas han sido activadas de forma satisfactoria";

                }
                else
                {
                    respuesta = "No hay Evaluaciones de entrenamiento pendientes el día de hoy ";
                }
            }
            catch (Exception ex)
            {
                return respuesta + "Error:" + ex;
            }


            return respuesta;
        }


        //--------------- API4 (ACTUALIZACION AUTOGESTION)-----------------------------//
        [HttpGet]
        [Route("api/EvaDesempeño/ActualizarDatosEmpleado")]
        public string ActualizarDatosEmpleado()
        {

            var ListResult = "";
            using (var db = new AutogestionContext())
            {

                DataTable Datos = new DataTable();
                var Result = "";
                var datos2 = new Retiros();

                InMemoryDestinationConfiguration DestinacionConfiguracion = new InMemoryDestinationConfiguration();

                String contraseña = Properties.Settings.Default.Contraseña.ToString();
                var encodedTextBytes = Convert.FromBase64String(contraseña);

                contraseña = Encoding.UTF8.GetString(encodedTextBytes);

                RfcConfigParameters config = new RfcConfigParameters();

                try
                {

                    config.Add(RfcConfigParameters.Name, "SAP");
                    config.Add(RfcConfigParameters.AppServerHost, Properties.Settings.Default.Servidor.ToString());
                    config.Add(RfcConfigParameters.SystemNumber, Properties.Settings.Default.Id.ToString());
                    config.Add(RfcConfigParameters.User, Properties.Settings.Default.Usuario.ToString());
                    config.Add(RfcConfigParameters.Password, contraseña);
                    config.Add(RfcConfigParameters.Client, Properties.Settings.Default.Mandante.ToString());
                    config.Add(RfcConfigParameters.Language, "ES");
                    config.Add(RfcConfigParameters.SAPRouter, Properties.Settings.Default.Saprouter.ToString());
                    config.Add(RfcConfigParameters.LogonGroup, "Foscal");

                    RfcDestinationManager.RegisterDestinationConfiguration(DestinacionConfiguracion);
                    DestinacionConfiguracion.AddOrEditDestination(config);
                    RfcDestination destination = RfcDestinationManager.GetDestination("SAP");

                    RfcRepository repository = destination.Repository;
                    IRfcFunction function = repository.CreateFunction("ZMF_EMPLEADOS_3000");
                    //PARAMETROS IMPORT
                    DateTime date = DateTime.Now;
                    DateTime date2 = date.AddDays(1);
                    var dia = date2.Day;
                    var mes = date2.Month;
                    var año = date2.Year;
                    string FECHA = dia + "." + mes + "." + año;
                    function.SetValue("I_PARAMETRO", 2);
                    function.SetValue("I_PERSG", 1);
                    function.SetValue("I_FECHAI", "");
                    function.SetValue("I_FECHAF", Convert.ToDateTime(FECHA));

                    function.Invoke(destination);
                    //OBTENER RESPUESTA 
                    IRfcTable Tabla = function.GetTable("T_DATOS");
                    ArrayList EMPSAPLIST = new ArrayList();
                    Datos = GetDataTableFromRFCTable(Tabla);
                    int count = Datos.Rows.Count;
                    int s = count - 1;
                    for (var f = 0; f < count; f++)
                    {


                        string[] Result2 = { Datos.Rows[f]["PERNR"].ToString(), Datos.Rows[f]["PERSG"].ToString(), Datos.Rows[f]["ENDDA"].ToString(),
                            Datos.Rows[f]["BUKRS"].ToString(), Datos.Rows[f]["ENAME"].ToString(), Datos.Rows[f]["CARGO"].ToString(),
                            Datos.Rows[f]["PLANS"].ToString(),Datos.Rows[f]["ORGEH"].ToString(),Datos.Rows[f]["AREA"].ToString(),
                            Datos.Rows[f]["EPS"].ToString(),Datos.Rows[f]["RH"].ToString(),Datos.Rows[f]["GESCH"].ToString(),
                            Datos.Rows[f]["GBDAT"].ToString(),Datos.Rows[f]["FTEXT"].ToString(),
                            Datos.Rows[f]["FAMST"].ToString(),Datos.Rows[f]["PERID"].ToString()
                        };
                        EMPSAPLIST.Add(Result2);

                    }
                    List<Empleado> listEmpActualizar = new List<Empleado>();
                    List<Empleado> listEmpCrear = new List<Empleado>();
                    List<PersonalActivo> listPersonalActivoCrear = new List<PersonalActivo>();
                    // CREAR   / ACTUALIZAR -    EMPELADO ADM_AUTOGESTION
                    List<int> ListNmr= new List<int>();
                    List<string> ListNmr2 = new List<string>();
                    List<Empleado> EmpleadosAdm = db.Empleados.ToList();

                    foreach (string[] Item in EMPSAPLIST)
                    {
                        string NmrEmp = Item[0];
                        int NmrEmp2 =Convert.ToInt32( Item[0]);
                        ListNmr.Add(NmrEmp2);
                        ListNmr2.Add(NmrEmp);

                        if (NmrEmp== "50002658" || NmrEmp2 == 50002658)
                        {
                            var debug = 0;
                        }
                        if (Item[12] == "0000-00-00")
                        {
                            Item[12] = "" + new DateTime();
                        }
                        DateTime FechaNa = Convert.ToDateTime(Item[12]);
                        DateTime FechaFin = Convert.ToDateTime(Item[2]);
                        int CodigoSAPCargo = Convert.ToInt32(Item[6]);                      
                        Empleado empleado = EmpleadosAdm.Where(x => x.NroEmpleado.Trim() == NmrEmp.Trim()).FirstOrDefault();

                            if (empleado != null)
                            {
                                string aredescrip = empleado.AreaDescripcion;
                                DateTime? fechaing = empleado.FechaIngreso;
                                string tipoArea = empleado.TipoArea;

                                bool PERNR = false;//0
                                bool PERSG = false;//1
                                bool ENDDA = false;//2
                                bool BUKRS = false;//3
                                bool ENAME = false;//4
                                bool CARGO = false;//5
                                bool PLANS = false;//6
                                bool ORGEH = false;//7
                                bool AREA = false;//8                          
                                bool EPS = false;//9
                                bool RH = false;//10
                                bool GESCH = false;//11
                                bool GBDAT = false;//12
                                bool FTEXT = false;//13
                                bool FAMST = false;//14
                                bool PERID = false;//15

                                if (empleado.NroEmpleado != NmrEmp)
                                {
                                    empleado.NroEmpleado = NmrEmp;
                                    PERNR = true;
                                }

                                //Estado
                                if ("1" != Item[1])
                                {
                                    empleado.Activo = "NO";
                                    PERSG = true;
                                }
                                else if (empleado.Activo == "NO")
                                {
                                    empleado.Activo = "SI";
                                }
                                // Fecha Fin
                                if (empleado.FechaFin != FechaFin)
                                {

                                    empleado.FechaFin = FechaFin;
                                    ENDDA = true;
                                }
                                //Sociedad
                                if (empleado.Empresa != Item[3])
                                {

                                    empleado.Empresa = Item[3];
                                    BUKRS = true;
                                }
                                // Nombres Empleado
                                if (empleado.Nombres != Item[4])
                                {

                                    empleado.Nombres = Item[4];
                                    ENAME = true;
                                }
                                // Nombre Cargo Empleado
                                if (empleado.Cargo != Item[5])
                                {

                                    empleado.Cargo = Item[5];
                                    CARGO = true;
                                }
                                // Codigo Cargo Empleado
                                if (empleado.CodigoSAPCargo != CodigoSAPCargo)
                                {

                                    empleado.CodigoSAPCargo = CodigoSAPCargo;
                                    PLANS = true;
                                }
                                int cod = Convert.ToInt32(Item[7]);
                                if (empleado.UnidadOrganizativa != Item[7] || empleado.CodigoSAPArea != cod)
                                {
                                    //Unidad Organizativa
                                    empleado.UnidadOrganizativa = Item[7];

                                    empleado.CodigoSAPArea = cod;
                                    ORGEH = true;
                                }
                                //Nombre del Area del Empleado
                                if (empleado.Area != Item[8])
                                {

                                    empleado.Area = Item[8];
                                    AREA = true;
                                }
                                //if (empleado.Eps != Item[9])
                                //{

                                //    EPS = true;
                                //}
                                if (empleado.RH != Item[10])
                                {
                                    empleado.RH = Item[10];
                                    RH = true;
                                }
                                if (empleado.Genero != Item[11])
                                {
                                    //Genero
                                    empleado.Genero = Item[11];
                                    GESCH = true;
                                }
                                //                             //Item[12]
                                if (empleado.FechaNacimiento != FechaNa)
                                {
                                    empleado.FechaNacimiento = FechaNa;
                                    GBDAT = true;
                                }
                                //if (empleado.NroEmpleado != Item[13])
                                //{

                                //    FTEXT = true;
                                //}
                                //if (empleado.NroEmpleado != Item[14])
                                //{
                                //    // Estado civil
                                //    FAMST = true;
                                //}
                                if (empleado.Documento != Item[15])
                                {
                                // Documento de Identidad
                                    //empleado.Documento = Item[15].Trim();
                                    //PERID = true;
                                }

                                empleado = ActualizarDatosEmpleado2(empleado, destination);

                                if (FAMST == true || FTEXT == true || GBDAT == true || GESCH == true || RH == true || EPS == true || CARGO == true || AREA == true || ORGEH == true || PLANS == true || ENAME == true || BUKRS == true || ENDDA == true || PERSG == true || PERNR == true || empleado.FechaIngreso != fechaing || empleado.TipoArea != tipoArea || empleado.AreaDescripcion != aredescrip || PERID ==true)
                                {
                                    empleado.A_UsuarioModifica = "EVDAPI04";
                                    empleado.A_Modificacion = DateTime.Now;
                                    listEmpActualizar.Add(empleado);
                                }


                            }
                            else
                            {
                                Empleado NewEmp = new Empleado();
                                
                                NewEmp.Documento = Item[15];
                                NewEmp.NroEmpleado = NmrEmp;
                                NewEmp.Nombres = Item[4];
                                NewEmp.Empresa = Item[3];
                                NewEmp.Contraseña = "";
                                NewEmp.Cargo = Item[5];
                                NewEmp.FechaNacimiento = FechaNa;
                                //NewEmp.FechaIngreso = date;//DATO TEMPORAL
                                //NewEmp.TipoArea = "";
                                //NewEmp.AreaDescripcion="";
                                DateTime fechaf = Convert.ToDateTime(Item[2]);
                                NewEmp.FechaFin = fechaf;
                                NewEmp.RH = Item[10];
                                NewEmp.Activo = "SI";
                                NewEmp.UnidadOrganizativa = Item[7];
                                int codarea = Convert.ToInt32(Item[7]);
                                int codcargo = Convert.ToInt32(Item[6]);
                                NewEmp.CodigoSAPArea = codarea;
                                NewEmp.CodigoSAPCargo = codcargo;
                                NewEmp.Genero = Item[11];
                                NewEmp.Area = Item[8];
                                NewEmp.Cargo = Item[5];
                                NewEmp.A_UsuarioCreador = "EVDAPI04";
                                NewEmp.A_Creacion = DateTime.Now;
                                NewEmp = empleado = ActualizarDatosEmpleado2(NewEmp, destination);
                               //VERIFICAR DUPLICADOS EN EL LISTADO
                                var Dup = listEmpCrear.Where(x=>x.NroEmpleado == NewEmp.NroEmpleado).FirstOrDefault();
                                var DupDB = EmpleadosAdm.Where(x => x.NroEmpleado == NewEmp.NroEmpleado).FirstOrDefault();
                                if (Dup == null && DupDB== null && (NewEmp.Nombres != null && NewEmp.Nombres != "") && (NewEmp.Documento != null && NewEmp.Documento != "")) 
                                {
                                listEmpCrear.Add(NewEmp);
                                
                                //CREAR EMPLEADO EN PERSONAL ACTIVO
                                PersonalActivo NewEmp2 = db.PersonalActivo.Where(x => x.CodigoEmpleado == NmrEmp).FirstOrDefault();
                                if (NewEmp2 == null)
                                {
                                    NewEmp2 = new PersonalActivo();
                                    NewEmp2.CodigoEmpleado = NmrEmp;
                                    NewEmp2.Nombres = Item[4];
                                    NewEmp2.Empresa = Item[3];
                                    NewEmp2.Documento = Item[15];
                                    NewEmp2.UnidadOrganizativa = Item[7];
                                    NewEmp2.Cargo = Item[8];
                                    NewEmp2.Area = NewEmp.AreaDescripcion;
                                    listPersonalActivoCrear.Add(NewEmp2);
                                }
                            }
                            else if(Dup == null && DupDB== null)
                            {
                                if (NewEmp.Nombres == null && NewEmp.Nombres == "") 
                                {
                                    ListResult = ListResult + " " + empleado.NroEmpleado + " - ";
                                }
                                
                            }
                        }
                        
                    }
                   
                    if (listEmpActualizar.Count > 0)
                    {
                        db.SaveChanges();
                    }
                    if (listEmpCrear.Count > 0)
                    {
                        db.Empleados.AddRange(listEmpCrear);
                        db.PersonalActivo.AddRange(listPersonalActivoCrear);
                        db.SaveChanges();
                    }


                    return "Proceso Exitoso";


                }
                catch (SystemException ex )
                {
                    return "Error: " + ex + " --- Omitidos por Falta de Datos: "+ ListResult;
                }
                finally
                {
                    RfcDestinationManager.UnregisterDestinationConfiguration(DestinacionConfiguracion);
                }



            }
        }


        //--------------QUINTA API----------------//
        [HttpGet]
        [Route("api/EvaDesempeño/NotificacionEmailPeriodica")]
        public string NotificacionEmailPeriodica()
        {
            try
            {


                DateTime Hoy = DateTime.Today;
                var config = db.Configuraciones.Where(x => x.Parametro == "TiempoEvaDP").FirstOrDefault();
                int CantD = Convert.ToInt32(config.Valor);
                var PeriodoActual = db2.Periodos_Evaluacion_Desempeño.Where(x => x.Año == Hoy.Year).FirstOrDefault();
                if (PeriodoActual != null)
                {
                    periodos Periodo1000 = db2.Periodos.Where(x => x.fechaincio == PeriodoActual.FechaInicial && x.fechafinal == PeriodoActual.FechaFinal && x.sociedad == 1000 && x.TipoPeriodo == 1).FirstOrDefault();
                    periodos Periodo2000 = db2.Periodos.Where(x => x.fechaincio == PeriodoActual.FechaInicial && x.fechafinal == PeriodoActual.FechaFinal && x.sociedad == 2000 && x.TipoPeriodo == 1).FirstOrDefault();
                    DateTime ingreso = Hoy.AddDays(-CantD);
                    List<Empleado> listEmpCOMPLETO = db.Empleados.Where(x => (x.Jefe != null && x.Jefe != "") && x.Activo == "SI").OrderBy(s => s.Jefe).ToList();
                    List<Empleado> listEmp = listEmpCOMPLETO.Where(x => (x.Jefe != null && x.Jefe != "") && x.FechaIngreso <= ingreso && x.Activo == "SI").OrderBy(s => s.Jefe).ToList();
                    var Areas = db.Empleados.Where(s => s.Activo == "SI" && (s.AreaDescripcion != null || s.AreaDescripcion == "")).GroupBy(b => b.UnidadOrganizativa).ToList();

                    List<RestEmp> listNotificar = new List<RestEmp>();

                    var EstructurasADM = db.EstructuraJerarquica.ToList();
                    var EstructurasEVADES = db2.Estructura_Jerarquica_EVADES.ToList();
                    //List<string> jefes = new List<string>();
                    foreach (var item in Areas)
                    {
                        var Area = item.FirstOrDefault();
                        EstructuraJerarquica STRAREAADM = EstructurasADM.Where(o => o.UnidadOrg == Area.UnidadOrganizativa).FirstOrDefault();
                        Estructura_Jerarquica_EVADES STRAREAEVADES = EstructurasEVADES.Where(o => o.UnidadOrg == Area.UnidadOrganizativa).FirstOrDefault();
                        if (Area.AreaDescripcion == "URGENCIAS - PERSONAL DE ENFERMERIA") 
                        {
                            var debug = 0;
                        }
                        Empleado jefe = new Empleado();
                        if (STRAREAADM != null || STRAREAEVADES != null)
                            {
                            
                            if (STRAREAEVADES != null && Area.TipoArea == "Asistenciales CO")
                            {
                                jefe = listEmpCOMPLETO.Where(o => o.NroEmpleado == STRAREAEVADES.Jefe).FirstOrDefault();
                            }
                            else
                            {
                                jefe = listEmpCOMPLETO.Where(o => o.NroEmpleado == STRAREAADM.Jefe).FirstOrDefault();

                            }                           
                            //jefes.Add(x.Key.ToString());
                            var listemp2 = listEmp.Where(s => s.Jefe == jefe.NroEmpleado).ToList();
                            foreach (Empleado emp in listemp2)
                            {
                                periodos Periodo = new periodos();
                                if (emp.Empresa == "1000")
                                {
                                    Periodo = Periodo1000;
                                }
                                if (emp.Empresa == "2000")
                                {
                                    Periodo = Periodo2000;
                                }
                                int NmrEmp = Convert.ToInt32(emp.NroEmpleado);
                                encuestadores_x_empleado Eva = db2.Encuestadoresxempleado.Where(p => p.codigoempleado == NmrEmp && p.tipoevaluacion == 1 && p.codigoempleado != p.codigoevaluador && p.periodo == Periodo.codigo).OrderByDescending(c => c.codigo).FirstOrDefault();
                                int AñoIngreso = Convert.ToInt32(emp.FechaIngreso.Value.Year);
                                int AñoActual = Convert.ToInt32(Hoy.Year);
                                int DiferenciaAños = AñoActual - AñoIngreso;



                                if (Eva != null)
                                {
                                  
                                    if ((Eva.fecharegistro.Year == Hoy.Year && Eva.fecharegistro.Month == Hoy.Month) && Eva.Estado==1)
                                    {
                                        RestEmp restEmp = new RestEmp();
                                        string FechaProx = Convert.ToDateTime(Eva.fecharegistro).ToString("dd/MM/yyyy");
                                        restEmp.Empleado = emp;
                                        restEmp.EvaPeriodica = FechaProx;
                                        listNotificar.Add(restEmp);
                                        
                                    }
                                }
                                else
                                {
                                    
                                    DateTime FechaProxEva = emp.FechaIngreso.Value.AddYears(DiferenciaAños);
                                    if ((FechaProxEva.Year == Hoy.Year && FechaProxEva.Month == Hoy.Month))
                                    {
                                        RestEmp restEmp = new RestEmp();
                                        string fecha = "" + FechaProxEva.Day.ToString() + "/" + FechaProxEva.Month + "/" + Hoy.Year;
                                        string FechaProx = Convert.ToDateTime(fecha).ToString("dd/MM/yyyy");
                                        restEmp.Empleado = emp;
                                        restEmp.EvaPeriodica = FechaProx;
                                        listNotificar.Add(restEmp);
                                       
                                    }
                                }
                                if (listNotificar.Count >= 48)
                                {
                                    EnviaNotificacionesJefes(listNotificar, jefe.Correo);
                                    listNotificar.Clear();

                                }
                            }

                            // Notifica lista de empleado por jefe
                            if (listNotificar.Count >= 1)
                            {
                                EnviaNotificacionesJefes(listNotificar, jefe.Correo);
                                listNotificar.Clear();

                            }
                        }

                    }

                }

                return "Proceso finalizado de Forma Exitosa";
            }
            catch (Exception ex)
            {
                return "Error: " + ex;

            }

        }

        //--------------SEXTA API----------------//
        [HttpGet]
        [Route("api/EvaDesempeño/NotificacionEmailEntrenamiento")]
        public string NotificacionEmailEntrenamiento()
        {
            try {
                DateTime Hoy = DateTime.Today;
                var cconfig = db.Configuraciones.Where(x => x.Parametro == "TiempoEvaDE").FirstOrDefault();
                int catndias = Convert.ToInt32(cconfig.Valor);
                DateTime ingreso = Hoy.AddDays(-catndias);
                List<Empleado> listEmp = db.Empleados.Where(x => (x.Jefe != null && x.Jefe != "") && x.FechaIngreso == ingreso && x.Activo == "SI").OrderBy(s => s.Jefe).ToList();
                List<RestEmp> listNotificar = new List<RestEmp>();
                List<RestEmp> listNotificar2 = new List<RestEmp>();
                periodos PeriodoHoy1000 = db2.Periodos.Where(x => x.fechaincio == Hoy && x.sociedad == 1000 && x.TipoPeriodo == 2).FirstOrDefault();
                periodos PeriodoHoy2000 = db2.Periodos.Where(x => x.fechaincio == Hoy && x.sociedad == 2000 && x.TipoPeriodo == 2).FirstOrDefault();
                if (PeriodoHoy1000 == null && listEmp.Count >= 1)
                {
                    var soc100 = listEmp.Where(x => x.Empresa == "1000").Count();
                    var soc2000 = listEmp.Where(x => x.Empresa == "2000").Count();
                    DateTime Fechafin = Hoy.AddDays(catndias);
                    if (soc100 >= 1)
                    {
                        PeriodoHoy1000 = CrearPeriodo(Hoy, Fechafin, 1000, 2);
                    }
                    if (soc2000 >= 1)
                    {
                        PeriodoHoy2000 = CrearPeriodo(Hoy, Fechafin, 2000, 2);
                    }
                }
                if (PeriodoHoy2000 == null && listEmp.Count >= 1)
                {
                    var soc2000 = listEmp.Where(x => x.Empresa == "2000").Count();
                    DateTime Fechafin = Hoy.AddDays(catndias);

                    if (soc2000 >= 1)
                    {
                        PeriodoHoy2000 = CrearPeriodo(Hoy, Fechafin, 2000, 2);
                    }
                }
                //var lista = db.Empleados.Where(s => s.Empresa == id && s.Activo == "SI" && (s.AreaDescripcion != null || s.AreaDescripcion == "") && s.TipoArea == "Asistenciales CO").Select(x => new { x.AreaDescripcion }).GroupBy(b => b.AreaDescripcion).ToList();
                var countjefes = listEmp.Select(x => new { x.Jefe }).GroupBy(b => b.Jefe).ToArray();
                List<string> jefes = new List<string>();
                foreach (var x in countjefes)
                {
                    string jefe = x.Key.ToString();
                    jefes.Add(x.Key.ToString());
                    var listemp2 = listEmp.Where(s => s.Jefe == jefe).ToList();
                    foreach (Empleado emp in listemp2)
                    {
                        bool EveExist = false;
                        int NmrEmp = Convert.ToInt32(emp.NroEmpleado);
                        if (emp.Empresa == "1000") {
                            encuestadores_x_empleado Eva = db2.Encuestadoresxempleado.Where(p => p.codigoempleado == NmrEmp && p.tipoevaluacion == 2 && p.codigoempleado != p.codigoevaluador && p.periodo == PeriodoHoy1000.codigo).OrderByDescending(c => c.codigo).FirstOrDefault();
                            if (Eva != null)
                            {
                                EveExist = true;
                            }
                        } else if (emp.Empresa == "2000")
                        {
                            encuestadores_x_empleado Eva = db2.Encuestadoresxempleado.Where(p => p.codigoempleado == NmrEmp && p.tipoevaluacion == 2 && p.codigoempleado != p.codigoevaluador && p.periodo == PeriodoHoy2000.codigo).OrderByDescending(c => c.codigo).FirstOrDefault();
                            if (Eva != null)
                            {
                                EveExist = true;
                            }
                        }
                        if (EveExist == true)
                        {
                            RestEmp restEmp = new RestEmp();
                            restEmp.Empleado = emp;
                            listNotificar.Add(restEmp);


                            listNotificar2.Add(restEmp);
                            if (listNotificar2.Count >= 20)
                            {
                                EnviaNotificacionesEmpleadoEntrenamiento(listNotificar2);
                                listNotificar2.Clear();
                            }

                        }
                    }

                    // Notifica lista de empleado por jefe
                    if (listNotificar.Count >= 1)
                    {
                        EnviaNotificacionesJefes2(listNotificar);

                        listNotificar.Clear();

                    }
                }

                return "true";
            }
            catch
            {
                return "False";
            }

        }

        //--------------SEPTIMA API----------------//
        [HttpGet]
        [Route("api/EvaDesempeño/NotificacionEmailPeriodicaEmpleados")]
        public string NotificacionEmailPeriodicaEmpleados()
        {
            DateTime Hoy = DateTime.Today;
            var config = db.Configuraciones.Where(x => x.Parametro == "TiempoEvaDP").FirstOrDefault();
            int CantD = Convert.ToInt32(config.Valor);
            DateTime ingreso = Hoy.AddDays(-CantD);
            List<Empleado> listEmp = db.Empleados.Where(x => (x.Jefe != null && x.Jefe != "") && x.FechaIngreso <= ingreso && x.Activo == "SI").OrderBy(s => s.Jefe).ToList();
            List<RestEmp> listNotificar = new List<RestEmp>();
            var countjefes = listEmp.Select(x => new { x.Jefe }).GroupBy(b => b.Jefe).ToArray();
            List<string> jefes = new List<string>();
            Periodos_Evaluacion_Desempeño Parametro = db2.Periodos_Evaluacion_Desempeño.Where(x => x.Año == Hoy.Year).FirstOrDefault();
            periodos PeriodoHoy1000 = db2.Periodos.Where(x => x.fechaincio == Parametro.FechaInicial && x.fechafinal == Parametro.FechaFinal && x.sociedad == 1000 && x.TipoPeriodo == 1).FirstOrDefault();
            periodos PeriodoHoy2000 = db2.Periodos.Where(x => x.fechaincio == Parametro.FechaInicial && x.fechafinal == Parametro.FechaFinal && x.sociedad == 2000 && x.TipoPeriodo == 1).FirstOrDefault();
            if (PeriodoHoy1000 == null && listEmp.Count >= 1)
            {
                var soc100 = listEmp.Where(x => x.Empresa == "1000").Count();
                var soc2000 = listEmp.Where(x => x.Empresa == "2000").Count();
                DateTime Fechafin = Hoy.AddDays(CantD);
                if (soc100 >= 1)
                {
                    PeriodoHoy1000 = CrearPeriodo(Parametro.FechaInicial, Parametro.FechaFinal, 1000, 1);
                }
                if (soc2000 >= 1)
                {
                    PeriodoHoy2000 = CrearPeriodo(Parametro.FechaInicial, Parametro.FechaFinal, 2000, 1);
                }
            }
            if (PeriodoHoy2000 == null && listEmp.Count >= 1)
            {
                var soc2000 = listEmp.Where(x => x.Empresa == "2000").Count();
                DateTime Fechafin = Hoy.AddDays(CantD);

                if (soc2000 >= 1)
                {
                    PeriodoHoy2000 = CrearPeriodo(Hoy, Fechafin, 2000, 2);
                }
            }

            foreach (var x in countjefes)
            {

                RestEmp restEmp = new RestEmp();
                string jefe = x.Key.ToString();
                jefes.Add(x.Key.ToString());
                var listemp2 = listEmp.Where(s => s.Jefe == jefe).ToList();
                foreach (Empleado emp in listemp2)
                {
                    bool EveExist = false;
                    int NmrEmp = Convert.ToInt32(emp.NroEmpleado);
                    if (NmrEmp==50000384) 
                    {
                        var debug = 0;
                    }

                    encuestadores_x_empleado Eva2 = new encuestadores_x_empleado();
                    if (emp.Empresa == "1000")
                    {

                        encuestadores_x_empleado Eva = db2.Encuestadoresxempleado.Where(p => p.codigoempleado == NmrEmp && p.tipoevaluacion == 1 && p.codigoempleado != p.codigoevaluador && p.periodo == PeriodoHoy1000.codigo).OrderByDescending(c => c.codigo).FirstOrDefault();
                        if (Eva != null)
                        {
                            Eva2 = Eva;
                            EveExist = true;
                        }
                    }
                    else if (emp.Empresa == "2000")
                    {
                        encuestadores_x_empleado Eva = db2.Encuestadoresxempleado.Where(p => p.codigoempleado == NmrEmp && p.tipoevaluacion == 1 && p.codigoempleado != p.codigoevaluador && p.periodo == PeriodoHoy2000.codigo).OrderByDescending(c => c.codigo).FirstOrDefault();
                        if (Eva != null)
                        {
                            Eva2 = Eva;
                            EveExist = true;
                        }
                    }
                    if (EveExist == true)
                    {

                        DateTime FechaProxEva = Eva2.fecharegistro.AddDays(CantD);
                        if ((Eva2.fecharegistro == Hoy))
                        {
                            restEmp.Empleado = emp;
                            restEmp.EvaPeriodica = Convert.ToDateTime(Eva2.fecharegistro).ToString("dd/MM/yyyy");
                            listNotificar.Add(restEmp);
                            if (listNotificar.Count >= 20)
                            {
                                EnviaNotificacionesEmpleadosPeriodica(listNotificar);
                                listNotificar.Clear();
                            }
                        }
                    }

                }

                // Notifica lista de empleado por jefe en caso de no haber mas de 20 empleados
                if (listNotificar.Count >= 1)
                {
                    EnviaNotificacionesEmpleadosPeriodica(listNotificar);
                    listNotificar.Clear();

                }
            }
            return "true";


        }
        //--------------OCTAVA API----------------//
        [HttpGet]
        [Route("api/EvaDesempeño/NotificacionEmailSeguimientosDiarios")]
        public string NotificacionEmailSeguimientosDiarios()
        {
            DateTime Hoy = DateTime.Today;
            DateTime Hoy2 = Hoy.AddYears(-1);

            List <Empleado> listEmp = db.Empleados.Where(x => (x.Jefe != null && x.Jefe != "") && x.Activo == "SI").OrderBy(s => s.Jefe).ToList();
            List<RestEmp> listNotificar = new List<RestEmp>();
            List<RestEmp> listNotificar2 = new List<RestEmp>();
            var countjefes = listEmp.Select(x => new { x.Jefe }).GroupBy(b => b.Jefe).ToArray();
            //Toma encuenta Periodo actual y periodo anterior para los seguimientos
            Periodos_Evaluacion_Desempeño ParamActual = db2.Periodos_Evaluacion_Desempeño.Where(x => x.Año == Hoy.Year).FirstOrDefault();
            Periodos_Evaluacion_Desempeño ParamAnterior = db2.Periodos_Evaluacion_Desempeño.Where(x => x.Año == Hoy2.Year).FirstOrDefault();
            periodos PeriodoAct1000 = db2.Periodos.Where(x => x.Parametro_Id == ParamActual.Id && x.sociedad == 1000).FirstOrDefault();
            periodos PeriodoAct2000 = db2.Periodos.Where(x => x.Parametro_Id == ParamActual.Id && x.sociedad == 2000).FirstOrDefault();
            periodos PeriodoAnt1000 = db2.Periodos.Where(x => x.Parametro_Id == ParamAnterior.Id && x.sociedad == 1000).FirstOrDefault();
            periodos PeriodoAnt2000 = db2.Periodos.Where(x => x.Parametro_Id == ParamAnterior.Id && x.sociedad == 2000).FirstOrDefault();


            if (PeriodoAct1000 != null  && PeriodoAct2000 != null) 
            {
                if (PeriodoAnt1000 == null)
                {
                    PeriodoAnt1000 = db2.Periodos.Where(x => x.Parametro_Id == ParamActual.Id).FirstOrDefault();
                }
                if (PeriodoAnt2000 == null)
                {
                    PeriodoAnt2000 = db2.Periodos.Where(x => x.Parametro_Id == ParamActual.Id).FirstOrDefault();
                }
                List<seguimientos> Seguimientos = db2.Seguimientos.Where(x =>
                    (x.periodo == PeriodoAct1000.codigo || x.periodo == PeriodoAct2000.codigo)
                || ( x.periodo == PeriodoAnt1000.codigo || x.periodo == PeriodoAnt2000.codigo)).ToList();
                List<seguimientos> SeguimientosEmps = Seguimientos.Where(x =>  x.cumplimiento == null && x.proximoseguimiento == Hoy).ToList(); // fata validadcion Hoy
                List<Seguimientos_x_Registro> Relacionados = db2.Seguimientos_x_Registro.Where(x => x.Cumplimiento == null && x.Proximo_Seguimiento == Hoy).ToList(); // fata validadcion Hoy
                if (SeguimientosEmps.Count > 0 || Relacionados.Count > 0)
                { 
                    foreach (var Jefe in countjefes)
                    {
                        string jefe = Jefe.Key.ToString();
                        Empleado JefeArea = listEmp.Where(x => x.NroEmpleado == jefe).FirstOrDefault();
                        var listemp2 = listEmp.Where(s => s.Jefe == jefe).ToList();
                        foreach (Empleado emp in listemp2)
                        {
                            int NmrEmp = Convert.ToInt32(emp.NroEmpleado);
                            if (NmrEmp == 50000639) 
                            {
                                var debug = 0;
                            }
                            //Seguimientos Principales
                            List<seguimientos> result = SeguimientosEmps.Where(s => s.codigoempleado == NmrEmp).ToList();
                            //Seguimientos Relacionados
                            List<seguimientos> result2 = Seguimientos.Where(x => x.codigoempleado == NmrEmp).ToList();
                            List<Seguimientos_x_Registro> Result3 = new List<Seguimientos_x_Registro>();
                            foreach (var item in result2) 
                            {
                                var RelacionadosHoy = Relacionados.Where(x => x.Cod_Seguimiento == item.codigo).ToList();
                                if (RelacionadosHoy.Count() > 0) 
                                {
                                    Result3.AddRange(RelacionadosHoy);
                                }
                            }
                            if (result.Count > 0 || Result3.Count > 0)
                            {
                                RestEmp restEmp = new RestEmp();
                                restEmp.Empleado = emp;
                                listNotificar.Add(restEmp);
                            }

                        }

                        // Notifica lista de empleado por jefe
                        if (listNotificar.Count >= 1 && JefeArea != null)
                        {
                           EnviaNotificacionesJefesSeguimientos(listNotificar , JefeArea);
                            listNotificar.Clear();

                        }
                    }
                }
            }
            

            return "true";


        }
        //--------------NOVENA API----------------//
        [HttpGet]
        [Route("api/EvaDesempeño/ActualizacionEvaluacion")]
        public string ActualizacionEvaluacion()
        {
            try 
            {
                DateTime HoyHace2Años = DateTime.Today.AddDays(-730);
                DateTime HOY = DateTime.Now;
                List<Empleado> listEmp = db.Empleados.Where(x => (x.Jefe != null && x.Jefe != "") && x.Activo == "SI").OrderBy(s => s.Jefe).ToList();
                List <encuestadores_x_empleado> Evaluaciones = db2.Encuestadoresxempleado.Where(x => x.Estado != 6 && x.tipoevaluador == 1 && x.fecharegistro >= HoyHace2Años).ToList();
                List<encuestadores_x_empleado> AutoEvaluaciones = db2.Encuestadoresxempleado.Where(x => x.Estado != 6 && x.tipoevaluador == 3 && x.fecharegistro >= HoyHace2Años).ToList();
                List<encuestadores_x_empleado> EvaluacionesActualizar = new List<encuestadores_x_empleado>();
                //Consulta de Periodo Actual
                Periodos_Evaluacion_Desempeño ConfigPP = db2.Periodos_Evaluacion_Desempeño.Where(x => x.Año == HOY.Year).FirstOrDefault();
                List<Periodos_Evaluacion_Desempeño> ConfigPPList = db2.Periodos_Evaluacion_Desempeño.ToList();
                List<periodos> PeriodosList = db2.Periodos.ToList();
                if (ConfigPP!=null) 
                {            
                periodos Periodo1000 = db2.Periodos.Where(x => x.fechaincio == ConfigPP.FechaInicial && x.fechafinal == ConfigPP.FechaFinal && x.sociedad==1000).FirstOrDefault();
                periodos Periodo2000 = db2.Periodos.Where(x => x.fechaincio == ConfigPP.FechaInicial && x.fechafinal == ConfigPP.FechaFinal && x.sociedad == 2000).FirstOrDefault();

                List<Empleados_Cargo_Area_x_Periodo> Relacion_ECAP = db2.Empleados_Cargo_Area_x_Periodo.Where(x=>x.Periodo == Periodo1000.codigo || x.Periodo == Periodo2000.codigo).ToList();
                List<Empleados_Cargo_Area_x_Periodo> Relacion_ECAP_Actualizar = new List<Empleados_Cargo_Area_x_Periodo>();
                List<Empleados_Cargo_Area_x_Periodo> Relacion_ECAP_Crear = new List<Empleados_Cargo_Area_x_Periodo>();
                List<Empleados_Cargo_Area_x_Periodo> Relacion_ECAP_Eliminar = new List<Empleados_Cargo_Area_x_Periodo>();
                List<areas> AreasList = db2.areas.ToList();
                List<cargos> CargosList = db2.Cargos.ToList();
                List <Estructura_Jerarquica_EVADES> STREvadesh = db2.Estructura_Jerarquica_EVADES.Where(x => x.Jefe != "" || x.Jefe != null).ToList();
                    //ACTUALIZACION DEL EVALUADOR
                    foreach (Empleado Item in listEmp)
                    {
                        int NmrEmp = Convert.ToInt32(Item.NroEmpleado);
                        if (NmrEmp== 40000282)
                        {
                            var debug = 0;
                        }
                        int Jefe = Convert.ToInt32(Item.Jefe);
                        int JefeStr = 0;
                        areas areaemp = AreasList.Where(x => x.CodigoSAP == Item.UnidadOrganizativa).FirstOrDefault();
                        encuestadores_x_empleado EvaEmp = Evaluaciones.Where(x => x.codigoempleado == NmrEmp && x.tipoevaluacion == 1).OrderByDescending(x => x.fecharegistro).FirstOrDefault();
                        encuestadores_x_empleado autoevaluacion = AutoEvaluaciones.Where(x => x.codigoempleado == NmrEmp).OrderByDescending(x => x.fecharegistro).FirstOrDefault();
                        encuestadores_x_empleado EvaEmp2 = Evaluaciones.Where(x => x.codigoempleado == NmrEmp && x.tipoevaluacion == 2).OrderByDescending(x => x.fecharegistro).FirstOrDefault();
                        if (Item.UnidadOrganizativa != null)
                        {
                            Estructura_Jerarquica_EVADES StrEmp = STREvadesh.Where(x => x.UnidadOrg.Trim() == Item.UnidadOrganizativa.Trim()).FirstOrDefault();
                            if (StrEmp != null)
                            {
                                Jefe = Convert.ToInt32(StrEmp.Jefe);
                                if (StrEmp.Jefe == Item.NroEmpleado)
                                {
                                    Jefe = Convert.ToInt32(StrEmp.Superior);
                                }
                            }
                        }
                        
                        // EVALUACION PERIODICA
                    if (EvaEmp != null )
                    {
                        // Periodo Evaluacion  - Anular evaluacion

                        periodos Per_Empleado = new periodos();

                            
                       

                            int AñoEvaluado = EvaEmp.fecharegistro.Year;
                            var ConfigActual = ConfigPPList.Where(x => x.Año == AñoEvaluado).FirstOrDefault();
                            if (ConfigActual!=null) { 
                            var Periodo1000EvaActual = PeriodosList.Where(x => x.sociedad == 1000 && x.Parametro_Id == ConfigActual.Id).FirstOrDefault();
                            var Periodo2000EvaActual = PeriodosList.Where(x => x.sociedad == 2000 && x.Parametro_Id == ConfigActual.Id).FirstOrDefault();
                            //ACTUALIZACION DEL EVALUADOR Y ESTADO DE LA EVALUACION SI ESTA SIN PRESENTAR Y EL PERIODO SEGUN EL AÑO EN QUE SE GENERA LA EVALUACION
                            if (EvaEmp.codigoevaluador != Jefe && EvaEmp.Estado == 1)
                            {
                            
                            db2.Encuestadoresxempleado.Attach(EvaEmp);
                            EvaEmp.codigoevaluador = Jefe;
                                if (Item.Empresa =="1000" && EvaEmp.periodo != Periodo1000EvaActual.codigo) 
                                {
                                    EvaEmp.periodo = Periodo1000EvaActual.codigo;
                                }
                                if (Item.Empresa == "2000" && EvaEmp.periodo != Periodo2000EvaActual.codigo)
                                {
                                    EvaEmp.periodo = Periodo2000EvaActual.codigo;
                                }
                                if (autoevaluacion != null)
                                { 
                                    if (Item.Activo =="NO" && EvaEmp.Estado==1 && autoevaluacion.Estado == 1)
                                {
                                    EvaEmp.Estado = 6;
                                    autoevaluacion.Estado=6;
                                    EvaluacionesActualizar.Add(autoevaluacion);
                                }
                                }
                                EvaluacionesActualizar.Add(EvaEmp);

                            }
                            else 
                            {
                                if (Item.Empresa == "1000" && EvaEmp.periodo != Periodo1000EvaActual.codigo)
                                {
                                    EvaEmp.periodo = Periodo1000EvaActual.codigo;
                                }
                                if (Item.Empresa == "2000" && EvaEmp.periodo != Periodo2000EvaActual.codigo)
                                {
                                    EvaEmp.periodo = Periodo2000EvaActual.codigo;
                                }
                                if (autoevaluacion != null)
                                {
                                        // ANULAR EVALUACION DE PERSONAL INACTIVO QUE NO PRESENTO EVALUACION
                                    if (Item.Activo == "NO" && EvaEmp.Estado == 1 && autoevaluacion.Estado == 1)
                                    {
                                        EvaEmp.Estado = 6;
                                        autoevaluacion.Estado = 6;
                                        EvaluacionesActualizar.Add(autoevaluacion);
                                        EvaluacionesActualizar.Add(EvaEmp);
                                    }
                                    else if(EvaEmp.Estado == 1 && autoevaluacion.Estado == 1)
                                    {
                                            var cambio = false;
                                        if (Item.Empresa == "1000" && autoevaluacion.periodo != Periodo1000EvaActual.codigo)
                                        {
                                          autoevaluacion.periodo = Periodo1000EvaActual.codigo;
                                          cambio = true;
                                        }
                                        if (Item.Empresa == "2000" && autoevaluacion.periodo != Periodo2000EvaActual.codigo)
                                        {
                                            autoevaluacion.periodo = Periodo2000EvaActual.codigo;
                                            cambio = true;
                                        }
                                            if (cambio == true) 
                                            {
                                                EvaluacionesActualizar.Add(autoevaluacion);
                                                EvaluacionesActualizar.Add(EvaEmp);
                                            }
                                        
                                    }
                                }
                                
                            }
                            }
                            //else
                            //{
                            //    if (StrEmp != null)
                            //    {
                            //        JefeStr = Convert.ToInt32(StrEmp.Jefe);
                            //        if (EvaEmp.codigoevaluador != JefeStr)
                            //        {
                            //            db2.Encuestadoresxempleado.Attach(EvaEmp);
                            //            EvaEmp.codigoevaluador = JefeStr;
                            //            EvaluacionesActualizar.Add(EvaEmp);
                            //        }

                            //    }
                            //}

                        }
                    
                    // EVALUACION DE ENTRENAMIENTO    
                    //if (EvaEmp2 != null) 
                    //{
                    //    if (EvaEmp2.codigoevaluador != Jefe)
                    //    {
                    //        if (StrEmp != null)
                    //        {
                    //            JefeStr = Convert.ToInt32(StrEmp.Jefe);
                    //            if (EvaEmp2.codigoevaluador != JefeStr)
                    //            {
                    //                db2.Encuestadoresxempleado.Attach(EvaEmp2);
                    //                EvaEmp2.codigoevaluador = JefeStr;
                    //                EvaluacionesActualizar.Add(EvaEmp2);
                    //            }
                    //        }
                    //        else
                    //        {
                    //            db2.Encuestadoresxempleado.Attach(EvaEmp2);
                    //            EvaEmp2.codigoevaluador = Jefe;
                    //            EvaluacionesActualizar.Add(EvaEmp2);

                    //        }

                    //    }
                    //    else
                    //    {
                    //        if (StrEmp != null)
                    //        {
                    //            JefeStr = Convert.ToInt32(StrEmp.Jefe);
                    //            if (EvaEmp2.codigoevaluador != JefeStr)
                    //            {
                    //                db2.Encuestadoresxempleado.Attach(EvaEmp2);
                    //                EvaEmp2.codigoevaluador = JefeStr;
                    //                EvaluacionesActualizar.Add(EvaEmp2);
                    //            }

                    //        }
                    //    }
                    //}

                    //ACTUALIZAR RELACIONES PARA TODOS LOS EMPLEADOS
                    if (Relacion_ECAP.Count > 0) 
                    {
                           
                            int Empleados1000 = 0;
                            int Empleados2000 = 0;
                          
                            if (areaemp != null)

                            { //DEFINIR RELACION PARA LAS CADA SOCIEDAD EN CASO DE SER NECESARIO
                                string NroEmpleado = Item.NroEmpleado;
                                Estructura_Jerarquica_EVADES StrEvadeshEmp = STREvadesh.Where(x => x.Area == areaemp.descripcion).FirstOrDefault();
                                if (StrEvadeshEmp != null)
                                {
                                    if (StrEvadeshEmp.Jefe == NroEmpleado)
                                    {
                                        Empleados1000 = listEmp.Where(x => x.Empresa == "1000" && x.AreaDescripcion == StrEvadeshEmp.Area && x.Activo == "SI").Count();
                                        Empleados2000 = listEmp.Where(x => x.Empresa == "2000" && x.AreaDescripcion == StrEvadeshEmp.Area && x.Activo == "SI").Count();
                                    }
                                    else
                                    {
                                        Empleados1000 = listEmp.Where(x => x.Empresa == "1000" && x.Jefe == NroEmpleado && x.Activo == "SI").Count();
                                        Empleados2000 = listEmp.Where(x => x.Empresa == "2000" && x.Jefe == NroEmpleado && x.Activo == "SI").Count();
                                    }
                                }
                                else
                                {
                                    Empleados1000 = listEmp.Where(x => x.Empresa == "1000" && x.Jefe == NroEmpleado && x.Activo == "SI").Count();
                                    Empleados2000 = listEmp.Where(x => x.Empresa == "2000" && x.Jefe == NroEmpleado && x.Activo == "SI").Count();
                                }

                                // DEFINE LA RELACION SI TIENE EMPLEADOS A CARGO
                                if (Empleados1000 > 0)
                                {

                                    var verificarDuplicados = Relacion_ECAP.Where(x => x.Periodo == Periodo1000.codigo && x.Empleado == NmrEmp).FirstOrDefault();
                                    if (verificarDuplicados ==null) 
                                    {
                                        cargos Cargoemp = CargosList.Where(x => x.descripcion == Item.Cargo).FirstOrDefault();

                                        if (Cargoemp != null)
                                        {
                                            Empleados_Cargo_Area_x_Periodo NewRel = new Empleados_Cargo_Area_x_Periodo();
                                            NewRel.Periodo = Periodo1000.codigo;
                                            NewRel.Empleado = NmrEmp;                                          
                                            NewRel.Area = areaemp.codigo;
                                            var VerificarduplicadosCrear = Relacion_ECAP_Crear.Where(x => x.Periodo == Periodo1000.codigo && x.Empleado == NmrEmp).FirstOrDefault();
                                            if (VerificarduplicadosCrear== null) 
                                            {
                                                Relacion_ECAP_Crear.Add(NewRel);
                                            }
                                          

                                        }
                                    }


                                }
                                if (Empleados2000 > 0)
                                {
                                    var verificarDuplicados = Relacion_ECAP.Where(x => x.Periodo == Periodo2000.codigo && x.Empleado == NmrEmp).FirstOrDefault();
                                    if (verificarDuplicados == null)
                                    {
                                        cargos Cargoemp = CargosList.Where(x => x.descripcion == Item.Cargo).FirstOrDefault();

                                        if (areaemp != null && Cargoemp != null)
                                        {
                                            Empleados_Cargo_Area_x_Periodo NewRel = new Empleados_Cargo_Area_x_Periodo();
                                            NewRel.Periodo = Periodo2000.codigo;
                                            NewRel.Empleado = NmrEmp;
                                            NewRel.Cargo = Cargoemp.codigo;
                                            NewRel.Area = areaemp.codigo;
                                            var VerificarduplicadosCrear = Relacion_ECAP_Crear.Where(x => x.Periodo == Periodo2000.codigo && x.Empleado == NmrEmp).FirstOrDefault();
                                            if (VerificarduplicadosCrear == null)
                                            {
                                                Relacion_ECAP_Crear.Add(NewRel);
                                            }

                                        }
                                    }

                                }
                                if (Item.Empresa == "1000")
                                {
                                    var VerificarRelacion = Relacion_ECAP.Where(x => x.Empleado == NmrEmp && x.Periodo == Periodo1000.codigo).FirstOrDefault();
                                    if (VerificarRelacion == null)
                                    {

                                        cargos Cargoemp = CargosList.Where(x => x.descripcion == Item.Cargo).FirstOrDefault();

                                        if (areaemp != null && Cargoemp != null)
                                        {
                                            Empleados_Cargo_Area_x_Periodo NewRel = new Empleados_Cargo_Area_x_Periodo();
                                            NewRel.Periodo = Periodo1000.codigo;
                                            NewRel.Empleado = NmrEmp;
                                            NewRel.Cargo = Cargoemp.codigo;
                                            NewRel.Area = areaemp.codigo;
                                            var VerificarduplicadosCrear = Relacion_ECAP_Crear.Where(x => x.Periodo == Periodo1000.codigo && x.Empleado == NmrEmp).FirstOrDefault();
                                            if (VerificarduplicadosCrear == null)
                                            {
                                                Relacion_ECAP_Crear.Add(NewRel);
                                            }
                                            

                                        }

                                    }
                                    //var VerificarRelacionContraria2000 = Relacion_ECAP.Where(x => x.Empleado == NmrEmp && x.Periodo == Periodo1000.codigo).First();
                                }
                                if (Item.Empresa == "2000")
                                {
                                    var VerificarRelacion = Relacion_ECAP.Where(x => x.Empleado == NmrEmp && x.Periodo == Periodo2000.codigo).FirstOrDefault();
                                    if (VerificarRelacion == null)
                                    {

                                        cargos Cargoemp = CargosList.Where(x => x.descripcion == Item.Cargo).FirstOrDefault();

                                        if (areaemp != null && Cargoemp != null)
                                        {
                                            Empleados_Cargo_Area_x_Periodo NewRel = new Empleados_Cargo_Area_x_Periodo();
                                            NewRel.Periodo = Periodo2000.codigo;
                                            NewRel.Empleado = NmrEmp;
                                            NewRel.Cargo = Cargoemp.codigo;
                                            NewRel.Area = areaemp.codigo;
                                            var VerificarduplicadosCrear = Relacion_ECAP_Crear.Where(x => x.Periodo == Periodo2000.codigo && x.Empleado == NmrEmp).FirstOrDefault();
                                            if (VerificarduplicadosCrear == null)
                                            {
                                                Relacion_ECAP_Crear.Add(NewRel);
                                            }

                                        }

                                    }
                                }

                                Empleados_Cargo_Area_x_Periodo Relacion_ECAPxEMP = Relacion_ECAP.Where(x => x.Empleado == NmrEmp && (x.Periodo == Periodo1000.codigo || x.Periodo == Periodo2000.codigo)).FirstOrDefault();
                                if (EvaEmp !=null && autoevaluacion!=null) 
                                { 
                                if (Relacion_ECAPxEMP != null && EvaEmp.Estado == 1 && autoevaluacion.Estado == 1 && Periodo1000 != null && Periodo2000 != null)
                                {
                                    areas AreaEmp = AreasList.Where(x => x.CodigoSAP == Item.UnidadOrganizativa).FirstOrDefault();
                                    cargos CargoEmp = CargosList.Where(x => x.descripcion == Item.Cargo).FirstOrDefault();
                                    bool cambio = false;
                                    //ACTUALIZAR PERIODO
                                    if (Item.Empresa == "1000" && Relacion_ECAPxEMP.Periodo != Periodo1000.codigo && Empleados1000 == 0)
                                    {
                                        Relacion_ECAP_Eliminar.Add(Relacion_ECAPxEMP);

                                        if (CargoEmp != null && AreaEmp != null)
                                        {
                                            Empleados_Cargo_Area_x_Periodo NewRel = new Empleados_Cargo_Area_x_Periodo();
                                            NewRel.Periodo = Periodo1000.codigo;
                                            NewRel.Empleado = NmrEmp;

                                            if (autoevaluacion != null)
                                            {
                                                if (autoevaluacion.Estado == 1 && EvaEmp.Estado == 1)
                                                {
                                                    NewRel.Cargo = CargoEmp.codigo;
                                                }

                                            }
                                            else
                                            {
                                                NewRel.Cargo = Relacion_ECAPxEMP.Cargo;
                                            }

                                            NewRel.Area = AreaEmp.codigo;
                                            var Duplicado = Relacion_ECAP_Crear.Where(x => x.Periodo == Periodo1000.codigo && x.Empleado == NmrEmp).FirstOrDefault();
                                            if (Duplicado == null)
                                            {
                                                var Duplicado2 = Relacion_ECAP.Where(x => x.Periodo == Periodo1000.codigo && x.Empleado == NmrEmp).FirstOrDefault();
                                                if (Duplicado2 == null)
                                                {
                                                    Relacion_ECAP_Crear.Add(NewRel);
                                                        cambio = true;
                                                    }

                                            }

                                            
                                        }
                                    }
                                    if (Item.Empresa == "2000" && Relacion_ECAPxEMP.Periodo != Periodo2000.codigo && Empleados2000 == 0)
                                    {
                                        Relacion_ECAP_Eliminar.Add(Relacion_ECAPxEMP);

                                        if (CargoEmp != null && AreaEmp != null)
                                        {
                                            Empleados_Cargo_Area_x_Periodo NewRel = new Empleados_Cargo_Area_x_Periodo();
                                            NewRel.Periodo = Periodo2000.codigo;
                                            NewRel.Empleado = NmrEmp;
                                            if (autoevaluacion != null)
                                            {
                                                if (autoevaluacion.Estado == 1 && EvaEmp.Estado == 1)
                                                {
                                                    NewRel.Cargo = CargoEmp.codigo;
                                                }

                                            }
                                            else
                                            {
                                                NewRel.Cargo = Relacion_ECAPxEMP.Cargo;
                                            }
                                            NewRel.Area = AreaEmp.codigo;
                                            
                                            var Duplicado = Relacion_ECAP_Crear.Where(x => x.Periodo == Periodo2000.codigo && x.Empleado == NmrEmp).FirstOrDefault();
                                            if (Duplicado == null)
                                            {
                                                var Duplicado2 = Relacion_ECAP.Where(x => x.Periodo == Periodo2000.codigo && x.Empleado == NmrEmp).FirstOrDefault();
                                                if (Duplicado2 == null)
                                                {
                                                    Relacion_ECAP_Crear.Add(NewRel);
                                                        cambio = true;
                                                    }
                                            }

                                        }
                                    }

                                    //ACTUALIZAR AREA

                                    if (AreaEmp != null  && cambio == false)
                                    {
                                        if (Relacion_ECAPxEMP.Area != AreaEmp.codigo)
                                        {
                                            Relacion_ECAPxEMP.Area = AreaEmp.codigo;
                                            cambio = true;
                                        }
                                    }

                                    // ACTUALIZAR CARGO

                                    if (CargoEmp != null && cambio == false)
                                    {

                                        if (Relacion_ECAPxEMP.Cargo != CargoEmp.codigo && (autoevaluacion.Estado == 1 && EvaEmp.Estado == 1))
                                        {
                                            Relacion_ECAPxEMP.Cargo = CargoEmp.codigo;
                                            cambio = true;
                                        }
                                    }



                                    if (cambio == true)
                                    {
                                        Relacion_ECAP_Actualizar.Add(Relacion_ECAPxEMP);
                                    }
                                }
                                else
                                {
                                    areas AreaEmp = AreasList.Where(x => x.CodigoSAP == Item.UnidadOrganizativa).FirstOrDefault();
                                    cargos CargoEmp = CargosList.Where(x => x.descripcion == Item.Cargo).FirstOrDefault();
                                    bool cambio = false;
                                    if (Relacion_ECAPxEMP == null)
                                    {
                                        //CREAR EN CASO DE NO EXISTIR
                                        if (Item.Empresa == "1000")
                                        {


                                            if (CargoEmp != null && AreaEmp != null)
                                            {
                                                Empleados_Cargo_Area_x_Periodo NewRel = new Empleados_Cargo_Area_x_Periodo();
                                                NewRel.Periodo = Periodo1000.codigo;
                                                NewRel.Empleado = NmrEmp;
                                                if (autoevaluacion != null)
                                                {
                                                    if (autoevaluacion.Estado == 1 && EvaEmp.Estado == 1)
                                                    {
                                                        NewRel.Cargo = CargoEmp.codigo;
                                                    }

                                                }
                                                else
                                                {
                                                    NewRel.Cargo = CargoEmp.codigo;
                                                }
                                                NewRel.Area = AreaEmp.codigo;
                                                var Duplicado = Relacion_ECAP_Crear.Where(x => x.Periodo == Periodo1000.codigo && x.Empleado == NmrEmp).FirstOrDefault();
                                                if (Duplicado == null)
                                                {
                                                    Relacion_ECAP_Crear.Add(NewRel);
                                                }

                                                cambio = true;
                                            }
                                        }
                                        if (Item.Empresa == "2000")
                                        {


                                            if (CargoEmp != null && AreaEmp != null)
                                            {
                                                Empleados_Cargo_Area_x_Periodo NewRel = new Empleados_Cargo_Area_x_Periodo();
                                                NewRel.Periodo = Periodo2000.codigo;
                                                NewRel.Empleado = NmrEmp;
                                                if (autoevaluacion != null)
                                                {
                                                    if (autoevaluacion.Estado == 1 && EvaEmp.Estado == 1)
                                                    {
                                                        NewRel.Cargo = CargoEmp.codigo;
                                                    }

                                                }
                                                else
                                                {
                                                    NewRel.Cargo = CargoEmp.codigo; ;
                                                }
                                                NewRel.Area = AreaEmp.codigo;
                                                cambio = true;
                                                var Duplicado = Relacion_ECAP_Crear.Where(x => x.Periodo == Periodo2000.codigo && x.Empleado == NmrEmp).FirstOrDefault();
                                                if (Duplicado == null)
                                                {
                                                    Relacion_ECAP_Crear.Add(NewRel);
                                                }

                                            }
                                        }

                                    }
                                    else 
                                    {
                                            // NO EDITA SI YA ESTA PRESENTADA  PARA EL PERIODO ACTUAL
                                            if (EvaEmp.periodo == Periodo1000.codigo || EvaEmp.periodo == Periodo2000.codigo ) 
                                            {
                                            
                                            }
                                    }
                                }
                                }
                                else if(Relacion_ECAPxEMP!=null)
                                {
                                 //PENDIENTE REVISAR LOGICA
                                }
                            }
                            else 
                            {
                                var debug = 0;
                            }
                        }


                           

                }

                    if (Relacion_ECAP_Eliminar.Count() >= 1)
                    {
                        db2.Empleados_Cargo_Area_x_Periodo.RemoveRange(Relacion_ECAP_Eliminar);
                        db2.SaveChanges();
                    }
                    if (EvaluacionesActualizar.Count() > 0 || Relacion_ECAP_Actualizar.Count() > 0)
                    {
                        db2.SaveChanges();
                    }
                    if (Relacion_ECAP_Crear.Count() >= 1)
                    {
                        db2.Empleados_Cargo_Area_x_Periodo.AddRange(Relacion_ECAP_Crear);
                        db2.SaveChanges();
                    }

                }
                return "true";
            }
            catch (Exception ex)
            {
                return "Error: " + ex;
            }
           


        }

        //--------------DECIMA API----------------//
        [HttpGet]
        [Route("api/EvaDesempeño/ActualizacionEmpleadoxSociedad")]
        public string ActualizacionEmpleadoxSociedad()
        {
            try
            {

                DateTime hoy = new DateTime();
                List<usuario_perfil> PerfilesUSER = db2.Usuario_perfil.Where(x => x.codigoperfil == 1).ToList();
                List<Empleado> listEmp = db.Empleados.Where(x => (x.Jefe != null && x.Jefe != "") && x.Activo == "SI" && x.TipoArea != "SENA Pctivo/Pasante" && x.TipoArea != "SENA Lectivo").ToList();
                List<Estructura_Jerarquica_EVADES> STREvadesh = db2.Estructura_Jerarquica_EVADES.Where(x => x.Jefe != "" && x.Jefe != null).ToList();
                List<usuarios_x_sociedad> Lusuarios_X_Sociedads = db2.Usuarios_x_sociedad.ToList();
                List<usuarios_x_sociedad> UsuariosCrear = new List<usuarios_x_sociedad>();
                List<usuarios_x_sociedad> UsuariosEliminar = new List<usuarios_x_sociedad>();
                //ACTUALIZACION DE RELACION DE SOCIEDAD
                foreach (Empleado Item in listEmp)
                {
                    int NmrEmp = Convert.ToInt32(Item.NroEmpleado);
                    if (NmrEmp== 40000342) 
                    {
                        var debug = 0;
                    }
                    var RolADM = PerfilesUSER.Where(x => x.codigousuario == NmrEmp).FirstOrDefault();
                    int Jefe = Convert.ToInt32(Item.Jefe);
                    int SociedadEmp = Convert.ToInt32(Item.Empresa);
                    List<Empleado> EmpleadosAcargo = new List<Empleado>();
                    Estructura_Jerarquica_EVADES StrEvadeshEmp = STREvadesh.Where(x => x.Area == Item.AreaDescripcion).FirstOrDefault();

                    EmpleadosAcargo = listEmp.Where(x => x.Jefe == Item.NroEmpleado).ToList();
                    if (StrEvadeshEmp != null)
                    {
                        var result = listEmp.Where(x => x.Jefe == StrEvadeshEmp.Jefe).ToList();
                        EmpleadosAcargo.AddRange(result);
                    }
                  
                        
                    
                    var relaciones = Lusuarios_X_Sociedads.Where(x => x.usuario == NmrEmp).ToList();
                    if (EmpleadosAcargo.Count() > 0)
                    {
                        int Emp1000 = EmpleadosAcargo.Where(x => x.Empresa == "1000").Count();
                        int Emp2000 = EmpleadosAcargo.Where(x => x.Empresa == "2000").Count();
                        if (Emp1000 > 0)
                        {
                            usuarios_x_sociedad Rel1000 = relaciones.Where(x => x.sociedad == 1000).FirstOrDefault();
                            if (Rel1000 == null)
                            {
                                Rel1000 = new usuarios_x_sociedad();
                                Rel1000.usuario = NmrEmp;
                                Rel1000.sociedad = 1000;
                                var duplicado = UsuariosCrear.Where(x => x.sociedad== Rel1000.sociedad && x.usuario == Rel1000.usuario).FirstOrDefault();
                                if (duplicado==null) 
                                {
                                    UsuariosCrear.Add(Rel1000);
                                }
                                
                            }

                        }
                        else if(SociedadEmp != 1000)
                        {
                            //ELIMINA LA SOCIEDAD A LA QUE NO PERTENECE EL JEFE

                            usuarios_x_sociedad Rel1000 = relaciones.Where(x => x.sociedad == 1000).FirstOrDefault();
                            
                            if (Rel1000 != null && RolADM == null)
                            {
                                UsuariosEliminar.Add(Rel1000);
                            }
                        }
                        if (Emp2000 > 0)
                        {
                            usuarios_x_sociedad Rel2000 = relaciones.Where(x => x.sociedad == 2000).FirstOrDefault();
                            if (Rel2000 == null)
                            {
                                Rel2000 = new usuarios_x_sociedad();
                                Rel2000.usuario = NmrEmp;
                                Rel2000.sociedad = 2000;
                                var duplicado = UsuariosCrear.Where(x => x.sociedad == Rel2000.sociedad && x.usuario == Rel2000.usuario).FirstOrDefault();
                                if (duplicado == null)
                                {
                                    UsuariosCrear.Add(Rel2000);
                                }
                            }
                        }
                        else if(SociedadEmp != 2000)
                        { 
                            //ELIMINA LA SOCIEDAD A LA QUE NO PERTENECE EL JEFE
                            usuarios_x_sociedad Rel2000 = relaciones.Where(x => x.sociedad == 2000).FirstOrDefault();
                            if (Rel2000 != null && RolADM == null)
                            {
                                UsuariosEliminar.Add(Rel2000);
                            }
                        }
                    }
                    else
                    { // NO TIENE EMPLEADOS ACARGO
                        // DEFINE LA SOCIEDAD A LA CUAL PERTENECE
                        usuarios_x_sociedad RelEMP = relaciones.Where(x => x.sociedad == SociedadEmp).FirstOrDefault();
                        if (RelEMP == null)
                        {
                            RelEMP = new usuarios_x_sociedad();
                            RelEMP.usuario = NmrEmp;
                            RelEMP.sociedad = SociedadEmp;
                            var Dupliacados = UsuariosCrear.Where(x => x.sociedad == RelEMP.sociedad && x.usuario == RelEMP.usuario).FirstOrDefault();
                            if (Dupliacados== null) 
                            {
                                UsuariosCrear.Add(RelEMP);
                            }
                           
                        }
                        //CONSULTA DEL LISTADO DE SOCIEDADES AQUELLAS A LAS QUE NO PERTENECE EL EMPLEADO Y LAS ELIMINA
                        usuarios_x_sociedad Rel1000R = relaciones.Where(x => x.sociedad != SociedadEmp).FirstOrDefault();
                        if (Rel1000R != null && RolADM == null)
                        {
                            UsuariosEliminar.Add(Rel1000R);
                        }
                    }


                }

                if (UsuariosCrear.Count() > 0)
                {
                    db2.Usuarios_x_sociedad.AddRange(UsuariosCrear);
                    db2.SaveChanges();
                }
                if (UsuariosEliminar.Count() > 0)
                {
                    db2.Usuarios_x_sociedad.RemoveRange(UsuariosEliminar);
                    db2.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return "" + ex;
            }



            return "Proceso Exitoso";


        }
        //--------------UNDECIMA API----------------//
        [HttpGet]
        [Route("api/EvaDesempeño/CreacionDeRoles")]
        public string CreacionDeRoles()
        {
            try
            {
                int Empleados1000 = 0;
                int Empleados2000 = 0;
                List<areas> LisAreasEvades = db2.areas.ToList();
                List<Empleado> EmpeleadosAdmAutogestion = db.Empleados.ToList(); ;
                List<Estructura_Jerarquica_EVADES> STREvadesh = db2.Estructura_Jerarquica_EVADES.Where(x => x.Jefe != "" && x.Jefe != null).ToList();
                List<perfiles> Perfiles = db2.Perfiles.ToList();
                List<usuario_perfil> PerfilesxEmpleados = db2.Usuario_perfil.ToList();
                List<empleados> EmpleadosEvaadesh = db2.empleados.Where(x => x.estado == 1).ToList();
                foreach (var Empleado in EmpleadosEvaadesh)
                {

                    string NroEmpleado = Convert.ToString(Empleado.codigo);
                    if (NroEmpleado == "40005117")
                    {
                        var debug = 0;
                    }
                    areas Area = LisAreasEvades.Where(x => x.codigo == Empleado.codigoarea).FirstOrDefault();

                    if (Area != null)
                    {
                        Estructura_Jerarquica_EVADES StrEvadeshEmp = STREvadesh.Where(x => x.Area == Area.descripcion).FirstOrDefault();
                        if (StrEvadeshEmp != null)
                        {
                            if (StrEvadeshEmp.Jefe == NroEmpleado)
                            {
                                Empleados1000 = EmpeleadosAdmAutogestion.Where(x => x.Empresa == "1000" && x.AreaDescripcion == StrEvadeshEmp.Area && x.Activo == "SI").Count();
                                Empleados2000 = EmpeleadosAdmAutogestion.Where(x => x.Empresa == "2000" && x.AreaDescripcion == StrEvadeshEmp.Area && x.Activo == "SI").Count();
                            }
                            else
                            {
                                Empleados1000 = EmpeleadosAdmAutogestion.Where(x => x.Empresa == "1000" && x.Jefe == NroEmpleado && x.Activo == "SI").Count();
                                Empleados2000 = EmpeleadosAdmAutogestion.Where(x => x.Empresa == "2000" && x.Jefe == NroEmpleado && x.Activo == "SI").Count();
                            }
                        }
                        else
                        {
                            Empleados1000 = EmpeleadosAdmAutogestion.Where(x => x.Empresa == "1000" && x.Jefe == NroEmpleado && x.Activo == "SI").Count();
                            Empleados2000 = EmpeleadosAdmAutogestion.Where(x => x.Empresa == "2000" && x.Jefe == NroEmpleado && x.Activo == "SI").Count();
                        }


                        if (Empleados1000 > 0 || Empleados2000 > 0)
                        {
                            usuario_perfil Perfilusuario = PerfilesxEmpleados.Where(x => x.codigousuario == Empleado.codigo).FirstOrDefault();
                            if (Perfilusuario == null)
                            {
                                usuario_perfil Nuevo = new usuario_perfil();
                                Nuevo.codigousuario = Empleado.codigo;
                                Nuevo.codigoperfil = 3;
                                db2.Usuario_perfil.Add(Nuevo);
                                db2.SaveChanges();
                            }


                        }

                    }

                }

                return "Proceso Exitoso";

            }
            catch (Exception ex)
            {
                return "Error en el Proceso: " + ex;
            }

        }
        //--------------DOCEAVA API----------------//
        [HttpGet]
        [Route("api/EvaDesempeño/ActualizarCargosAreasEvades")]
        public bool ActualizarCargosAreasEvades()
        {
            try
            {
                List<areas> AreasEdit = new List<areas>();
                List<areas> AreasCrear = new List<areas>();
                List<areas> lstEvadesareas = db2.areas.ToList();
                var lista = db.Empleados.Where(s => s.Activo == "SI" && (s.AreaDescripcion != null && s.AreaDescripcion != "") && s.UnidadOrganizativa != "" && s.Empresa!=null && s.Empresa !="").Select(x => new { x.AreaDescripcion, x.UnidadOrganizativa,x.Empresa }).GroupBy(b => b.UnidadOrganizativa).ToList();
                foreach (var item in lista)
                {
                    var val = item.FirstOrDefault();
                    string Cod = Convert.ToString(val.UnidadOrganizativa);
                    string Descripcion = val.AreaDescripcion;
                    string Soc = val.Empresa;
                    if (Descripcion == "SISTEMAS DE INFORMACIÓN")
                    {
                        var debug = true;
                    }
                    areas AreaEvades = lstEvadesareas.Where(x => x.CodigoSAP == Cod).FirstOrDefault();
                    if (AreaEvades == null)
                    {
                        areas NewArea = new areas();
                        NewArea.descripcion = Descripcion;
                        NewArea.CodigoSAP = Cod;
                        NewArea.Sociedad = Soc;
                        AreasCrear.Add(NewArea);
                    }
                    else
                    {
                        bool cambios = false;
                        if (AreaEvades.descripcion != Descripcion)
                        {
                            AreaEvades.descripcion = Descripcion;
                            cambios = true;


                        }
                        if (AreaEvades.Sociedad != Soc) 
                        {
                            AreaEvades.Sociedad = Soc;
                            cambios = true;
                        }

                        if (cambios==true)
                        {
                            AreasEdit.Add(AreaEvades);
                        }
                    }
                }
                if (AreasCrear.Count() > 0)
                {
                    db2.areas.AddRange(AreasCrear);

                }

                List<cargos> CargosCrear = new List<cargos>();
                List<cargos> lstEvadescargos = db2.Cargos.ToList();
                var listaCargos = db.Empleados.Where(s => s.Activo == "SI" && (s.Cargo != null && s.Cargo != "") && s.CodigoSAPCargo != 0).Select(x => new { x.Cargo, x.CodigoSAPCargo }).GroupBy(b => b.Cargo).ToList();
                foreach (var cargo in listaCargos)
                {
                    var val = cargo.FirstOrDefault();
                    string Cod = Convert.ToString(val.CodigoSAPCargo);
                    string Descripcion = val.Cargo;
                    cargos CargoEmp = lstEvadescargos.Where(x => x.descripcion == Descripcion).FirstOrDefault();
                    if (CargoEmp == null)
                    {
                        cargos NewCargos = new cargos();
                        NewCargos.descripcion = Descripcion;
                        //NewCargos.CodigoSAP = Cod;
                        NewCargos.completo = false;
                        CargosCrear.Add(NewCargos);
                    }

                }
                if (CargosCrear.Count() > 0)
                {
                    db2.Cargos.AddRange(CargosCrear);

                }
                if (AreasCrear.Count() > 0 || CargosCrear.Count() > 0 || AreasEdit.Count() > 0)
                {
                    db2.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //--------------Treceava API----------------//

        [HttpGet]
        [Route("api/EvaDesempeño/ConsultarseguimientosPSoc/{NmrEmP}")]
        public List<Seguimientos_x_Registro> ConsultarseguimientosPSoc(int NmrEmP) 
        {
            List<Seguimientos_x_Registro> ListSeg = new List<Seguimientos_x_Registro>();
            try 
            {
                int year = DateTime.Now.Year;
                var Config = db2.Periodos_Evaluacion_Desempeño.Where(x=>x.Año == year).FirstOrDefault();
                if (Config != null) 
                {
                    periodos periodoactual = db2.Periodos.Where(x => x.fechaincio == Config.FechaInicial && x.fechafinal == Config.FechaFinal).FirstOrDefault();
                    if (periodoactual != null) 
                    {
                        List<seguimientos> Segs = db2.Seguimientos.Where(x=>x.periodo == periodoactual.codigo && x.codigoempleado == NmrEmP).ToList();
                        foreach (var item in Segs)
                        {
                            List<Seguimientos_x_Registro> List = db2.Seguimientos_x_Registro.Where(x => x.Cod_Seguimiento == item.codigo && ( x.Socializado_Emp == null || x.Socializado_Emp == false)).ToList();
                            ListSeg.AddRange(List);
                        }


                    }
                    
                }
                

            }
            catch(Exception ex) 
            { 
              Seguimientos_x_Registro error = new Seguimientos_x_Registro();
                error.Descripcion ="" + ex.Message;
                error.Fecha = DateTime.Now;
                ListSeg.Add(error);
           
            }
            return ListSeg;


        }
        //-------------CATORCEAVA API (SOCIALIZADO SEGUIMIENTOS X REGISTRO EMPLEADO) --------------------//
        
        [HttpPost]
        [Route("api/EvaDesempeño/GuardarRegistroSegSoc")]
        public IHttpActionResult GuardarRegistroSeg_Soc(Seguimientos_x_Registro ItemSelect) 
        {
            var respuesta = "";
            try 
            {
                Seguimientos_x_Registro Item = db2.Seguimientos_x_Registro.Where(x => x.Codigo == ItemSelect.Codigo).FirstOrDefault();
                db2.Seguimientos_x_Registro.Attach(Item);
                Item.Fecha_SocEmp = ItemSelect.Fecha_SocEmp;
                Item.Socializado_Emp = ItemSelect.Socializado_Emp;
                Item.Observacion_Emp = ItemSelect.Observacion_Emp;
                db2.SaveChanges();

                respuesta = "Guardado Exitoso";

            }
            catch(Exception ex) 
            {
                respuesta = "" + ex.Message;
            }
            return Json(new { respuesta }) ;
        }
       
        //-------------QUINCEAVA API (ELIMINAR COMPETENCIAS DEL CARGO Y ELIMINAR EVALUACIONES/ RESET) --------------------//
        [HttpGet]
        [Route("api/EvaDesempeño/GuardarRegistroSegSoc/{IdCargo}")]
        public string EliminarEvaluaciones(int IdCargo)
        {
            var respuesta = "";
            try
            {
                DateTime Hoy = DateTime.Now;
                int year = Hoy.Year;
                Periodos_Evaluacion_Desempeño Config = db2.Periodos_Evaluacion_Desempeño.Where(x => x.Año == year).FirstOrDefault();
                if (Config!= null) 
                {
                    periodos Periodo1000 = db2.Periodos.Where(x => x.fechaincio == Config.FechaInicial && x.fechafinal == Config.FechaFinal && x.TipoPeriodo == 1 && x.sociedad ==1000).FirstOrDefault();
                    periodos Periodo2000 = db2.Periodos.Where(x => x.fechaincio == Config.FechaInicial && x.fechafinal == Config.FechaFinal && x.TipoPeriodo == 1 && x.sociedad == 2000).FirstOrDefault();


                    if (Periodo1000 != null && Periodo2000 != null) 
                    {
                        var Cargo = db2.Cargos.Where(x => x.codigo == IdCargo).FirstOrDefault();
                        if (Cargo != null)
                        { 

                            var cargosxCompetencia = db2.Cargos_x_competencias.Where(x => x.codigocargo == Cargo.codigo).ToList();
                            if (cargosxCompetencia.Count > 0)
                            {
                                List<Empleados_Cargo_Area_x_Periodo> ListRelacion = db2.Empleados_Cargo_Area_x_Periodo.Where(x => (x.Periodo == Periodo1000.codigo || x.Periodo == Periodo2000.codigo) && x.Cargo == Cargo.codigo).ToList();
                                List<encuestadores_x_empleado> ListEvaluaciones = new List<encuestadores_x_empleado>();
                                List<evaluacion_encabezado> ListEncabezadosDelete = new List<evaluacion_encabezado>();
                                List<evaluacion_aspecto_general> ListGeneralDelte = new List<evaluacion_aspecto_general>();
                                List<evaluacion_conductuales> ListcondutualesDelete = new List<evaluacion_conductuales>();
                                List<evaluacion_institucionales> ListIntitucionalesDelete = new List<evaluacion_institucionales>();
                                List<evaluacion_otros> ListOtrosDelete = new List<evaluacion_otros>();

                                foreach (var item in ListRelacion) 
                                {
                                    var Evaluacionresult = db2.Encuestadoresxempleado.Where(x => x.periodo == item.Periodo && x.codigoempleado == item.Empleado && x.tipoevaluador == 1).FirstOrDefault();
                                    if (Evaluacionresult!= null) 
                                    {
                                        Evaluacionresult.Estado = 1;
                                        Evaluacionresult.RetroalimentacionEmp = false;
                                        Evaluacionresult.RetroalimentacionJefe = false;
                                        Evaluacionresult.FechaRTA_Emp = null;
                                        Evaluacionresult.FechaRTA_Jefe = null;
                                        ListEvaluaciones.Add(Evaluacionresult);
                                    }

                                    var AutoEvaluacionresult = db2.Encuestadoresxempleado.Where(x => x.periodo == item.Periodo && x.codigoempleado == item.Empleado && x.tipoevaluador == 3).FirstOrDefault();
                                    if (Evaluacionresult != null)
                                    {
                                        AutoEvaluacionresult.Estado = 1;
                                        AutoEvaluacionresult.RetroalimentacionEmp = false;
                                        AutoEvaluacionresult.RetroalimentacionJefe = false;
                                        AutoEvaluacionresult.FechaRTA_Emp = null;
                                        AutoEvaluacionresult.FechaRTA_Jefe = null;
                                        ListEvaluaciones.Add(AutoEvaluacionresult);
                                    }
                                }
                                
                                foreach (var item in ListEvaluaciones) 
                                {
                                    var encabezado = db2.Evaluacion_encabezado.Where(x => x.codigo == item.codigo && x.codigoempleado == item.codigoempleado).FirstOrDefault();
                                    if (encabezado!=null) 
                                    {
                                        ListEncabezadosDelete.Add(encabezado);
                                    }
                                    var Generales = db2.Evaluacion_aspecto_general.Where(x => x.codigoevaluacion == item.codigo).ToList();
                                    if (Generales.Count > 0) 
                                    {
                                        ListGeneralDelte.AddRange(Generales);
                                    }
                                    var institucionales = db2.Evaluacion_institucionales.Where(x => x.codigoevaluacion == item.codigo).ToList();
                                    if (institucionales.Count > 0) 
                                    {
                                        ListIntitucionalesDelete.AddRange(institucionales);
                                    }
                                    var conductuales =db2.Evaluacion_conductuales.Where(x => x.codigoevaluacion == item.codigo).ToList();
                                    if (conductuales.Count > 0) 
                                    {
                                     ListcondutualesDelete.AddRange(conductuales);
                                    }
                                    var Otros = db2.Evaluacion_otros.Where(x => x.codigoevaluacion == item.codigo).ToList();
                                    if (Otros.Count > 0) 
                                    {
                                    ListOtrosDelete.AddRange(Otros);
                                    }
                                }
                                respuesta += "Cargo: " + Cargo.descripcion +
                                    " Cargosx competencia eliminados: " + cargosxCompetencia.Count() +
                                    " Evaluaciones actualizadas: " + ListEvaluaciones.Count() +
                                    " Encabezados Eliminados: " + ListEncabezadosDelete.Count() +
                                    " Evaluación Generales Eliminados: " + ListGeneralDelte.Count() +
                                    " Institucionales Eliminados: " + ListIntitucionalesDelete.Count() +
                                    " Conductuales Eliminados: " + ListcondutualesDelete.Count() +
                                    " Evaluacion Otros Eliminados: " + ListOtrosDelete.Count() 
                                    ;
                                db2.Evaluacion_encabezado.RemoveRange(ListEncabezadosDelete);
                                db2.Evaluacion_aspecto_general.RemoveRange(ListGeneralDelte);
                                db2.Evaluacion_institucionales.RemoveRange(ListIntitucionalesDelete);
                                db2.Evaluacion_conductuales.RemoveRange(ListcondutualesDelete);
                                db2.Evaluacion_otros.RemoveRange(ListOtrosDelete);
                                db2.SaveChanges();
                               

                            }
                            else 
                            {
                                respuesta = "No existen cargos x competencias para el cargo: " + Cargo.descripcion;
                            }
                 
                        }
                        else
                        {

                            respuesta = "No existe el cargo";
                        }
                    }
                    else 
                    {
                        respuesta = "No se ha encontrado un periodo";
                    }
                    
                    }
                else 
                {
                    respuesta = "No hay configuracion para el año actual";
                }


            }
            catch (Exception ex)
            {
                respuesta = "" + ex.Message;
            }
            return respuesta;
        }

        //-------------DIECISEISAVA API (RESET CONTRASEÑA EVADESH) --------------------//
        [HttpGet]
        [Route("api/EvaDesempeño/ResetPassword/{CODEMP}")]
        public string ResetPassword(int CODEMP) 
        {
            string respuesta="";
            try
            {
                if (CODEMP!=0) 
                {
                    empleados emp = db2.empleados.Where(x => x.codigo == CODEMP && x.estado==1).FirstOrDefault();
                    if (emp!=null) 
                    {
                        if (emp.estado == 1) 
                        {
                            db2.empleados.Attach(emp);
                            emp.clave = "";
                            db2.SaveChanges();
                            respuesta = "Se ha Reseteado con exito la contraseña de: " + emp.nombres + " " + emp.apellidos + ", NroEmpleado: " + emp.codigo;

                        }else 
                        {
                            string StingNrm = "" + CODEMP;
                            var EmpAdm = db.Empleados.Where(x => x.NroEmpleado == StingNrm).FirstOrDefault();
                            respuesta = "El estado del Empleado: " + emp.nombres + "Estado: "+ emp.estado +" Gestión Personal Activo: "+ EmpAdm.Activo; 
                        }
                    }
                    else
                    {
                        respuesta = "Empleado no existe en Evaluación de Desempeño o se encuetra inactivo";
                    }
                }
                else 
                {
                    respuesta = "Se debe insertar el Número del Empleado unicamente";
                }
               

            }
            catch (Exception ex) 
            {
                respuesta = "" + ex.Message;
            }


        return respuesta;
        }

        //[HttpGet]
        //[Route("api/EvaDesempeño/CorrecciondeErroresEstados")]
        //public string CorrecciondeErroresEstados() 
        //{
        //    try 
        //    {
        //        var ListEvaMal = db2.Encuestadoresxempleado.Where(x=>x.RetroalimentacionJefe == true && x.Estado==5).ToList();
        //        var ListSeguimientos = db2.Seguimientos.ToList();
        //        List<encuestadores_x_empleado> Actualizar = new List<encuestadores_x_empleado>();
        //        foreach (var item in ListEvaMal) 
        //        {
        //            int Estado = 5 ;
        //            var consulta1 = ListSeguimientos.Where(x => x.periodo == item.periodo && x.codigoempleado == item.codigoempleado && x.cumplimiento == null).ToList();
        //            if (consulta1.Count >0) 
        //            {
        //                Estado = 2;
        //            }
        //            else 
        //            {
        //                Estado =3;
        //            }
        //            item.Estado = Estado;
        //            Actualizar.Add(item);

        //        }
        //        if (Actualizar.Count()>0) 
        //        {
        //            db2.SaveChanges();
        //        }
        //        return "Proceso Existoso: ";
        //    }
        //    catch (Exception ex) 
        //    {
        //        return "Ha ocurrido un errror: " +ex;
        //    }

        //}

        //------FUNCIONES EXTRA PARA UN CORRECTO FUNCIONAMIENTO--------------------------//
        public static DataTable GetDataTableFromRFCTable(IRfcTable myrfcTable)
        {
            DataTable loTable = new DataTable();
            int liElement = 0;
            for (liElement = 0; liElement <= myrfcTable.ElementCount - 1; liElement++)
            {
                RfcElementMetadata metadata = myrfcTable.GetElementMetadata(liElement);
                loTable.Columns.Add(metadata.Name);
            }
            foreach (IRfcStructure Row in myrfcTable)
            {
                DataRow ldr = loTable.NewRow();
                for (liElement = 0; liElement <= myrfcTable.ElementCount - 1; liElement++)
                {
                    RfcElementMetadata metadata = myrfcTable.GetElementMetadata(liElement);
                    ldr[metadata.Name] = Row.GetString(metadata.Name);
                }
                loTable.Rows.Add(ldr);
            }
            return loTable;
        }
        public Empleado ActualizarDatosEmpleado2(Empleado CodigoEmpleado, RfcDestination destination)
        {
            using (var db = new AutogestionContext())
            {

                DataTable Datos = new DataTable();
                var Result = "";
                var datos2 = new Retiros();

                InMemoryDestinationConfiguration DestinacionConfiguracion = new InMemoryDestinationConfiguration();

                String contraseña = Properties.Settings.Default.Contraseña.ToString();
                var encodedTextBytes = Convert.FromBase64String(contraseña);

                contraseña = Encoding.UTF8.GetString(encodedTextBytes);

                RfcConfigParameters config = new RfcConfigParameters();
                Empleado empleado1 = CodigoEmpleado;
                try
                {

                    RfcRepository repository = destination.Repository;
                    IRfcFunction function = repository.CreateFunction("ZMF_ASIGNACION_ORGANIZ_3000");
                    //PARAMETROS IMPORT
                    function.SetValue("I_PERNR", CodigoEmpleado.NroEmpleado);

                    function.Invoke(destination);
                    //OBTENER RESPUESTA 


                    string PrimerApellido = function.GetStructure("T_RESULT").GetString("NACHN");
                    string SegundoApellido = function.GetStructure("T_RESULT").GetString("NACH2");
                    string PrimerNombre = function.GetStructure("T_RESULT").GetString("VORNA");
                    string SegundoNombre = function.GetStructure("T_RESULT").GetString("NAME2");
                    string TipoContrato = function.GetStructure("T_RESULT").GetString("CTTXT");
                    string InicioContrato = function.GetStructure("T_RESULT").GetString("BEGDA");
                    string AreaPersonal = function.GetStructure("T_RESULT").GetString("DPTO");
                    string NroEmpleado = function.GetStructure("T_RESULT").GetString("PERNR");
                    string tipoarea = function.GetStructure("T_RESULT").GetString("PTEXT");
                    string cargo = function.GetStructure("T_RESULT").GetString("CARGO");
                    string doc = function.GetStructure("T_RESULT").GetString("PERID");
                    if (PrimerNombre != "")
                    {

                        DateTime enteredDate = DateTime.Parse(InicioContrato);
                        Empleado empleado = new Empleado();
                        //empleado = db.Empleados.FirstOrDefault(x => x.NroEmpleado == CodigoEmpleado.NroEmpleado);
                        if (empleado1.FechaIngreso != enteredDate)
                        {
                            empleado1.FechaIngreso = enteredDate;
                        }
                        if (empleado1.TipoArea != tipoarea)
                        {
                            empleado1.TipoArea = tipoarea;
                        }
                        if (empleado1.AreaDescripcion != AreaPersonal)
                        {
                            empleado1.AreaDescripcion = AreaPersonal;
                        }
                        //if (empleado1.Cargo != cargo)
                        //{
                        //    empleado1.Cargo = cargo;
                        //}

                    }
                    else
                    {

                    }

                    return empleado1;
                }
                catch (SystemException ex)
                {
                    // ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
                    return empleado1;
                }
                finally
                {
                    //RfcDestinationManager.UnregisterDestinationConfiguration(DestinacionConfiguracion);
                    // DestinacionConfiguracion.RemoveDestination("SAP");
                }



            }
        }

        public string EnviaNotificacionesJefes(List<RestEmp> Empleados, string CorreoJefe)
        {

            using (var db = new AutogestionContext())
            {
                string hostname = HttpContext.Current.Request.Url.Host;
                string textocorreo = "";
                string txtde = Properties.Settings.Default.Correo.ToString();
                string contraseñacorreo = "";
                var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
                var email = "";
                var Nombresyfechas = "";
                foreach (var item in Empleados)
                {

                    //email += "" + item.Empleado.Correo + " ";
                    Nombresyfechas += "<li>" + item.Empleado.Nombres + " Fecha a presentar: " + item.EvaPeriodica + "</li>";


                }
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

                string texto = "";

                texto = db.Configuraciones.First(s => s.Parametro == "TXTEVADESNOTIFICACIONP").Valor.ToString();
                //Codigo Email
                //email= EmailJefe;
                texto = texto.Replace("$EMPLEADOS", Nombresyfechas);
                textocorreo = texto;

                try
                {
                    if (hostname == "localhost")
                    {
                        correo.To.Add("desarrollador.junior1@foscal.com.co");
                    }
                    else
                    {
                        correo.To.Add(CorreoJefe);
                    }

                    correo.Subject = "Notificación Evaluaciones Periódicas";
                    correo.Body = textocorreo;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;

                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    smtp.Send(correo);
                    return "";
                }
                catch (Exception ex)
                {

                    return "";
                }


            }
        }

        public string EnviaNotificacionesJefes2(List<RestEmp> Empleados)
        {

            using (var db = new AutogestionContext())
            {
                string hostname = HttpContext.Current.Request.Url.Host;
                string textocorreo = "";
                string txtde = Properties.Settings.Default.Correo.ToString();
                string contraseñacorreo = "";
                var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
                var email = "";
                var Nombres = "";
                foreach (var item in Empleados)
                {

                    email += "" + item.Empleado.Correo + " ";
                    Nombres = Nombres + "<li>" + item.Empleado.Nombres + "</li>";


                }
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

                string texto = "";

                texto = db.Configuraciones.First(s => s.Parametro == "TXTEVADESNOTIFICACIONE").Valor.ToString();
                //Codigo Email
                //email= EmailJefe;
                texto = texto.Replace("$EMPLEADOS", Nombres);
                textocorreo = texto;

                try
                {
                    if (hostname == "localhost")
                    {
                        correo.To.Add("desarrollador.junior1@foscal.com.co");
                    }
                    else
                    {
                        correo.To.Add(email);
                    }

                    correo.Subject = "Notificación Evaluaciones de Entrenamiento Hoy";
                    correo.Body = textocorreo;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;

                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    smtp.Send(correo);
                    return "";
                }
                catch (Exception ex)
                {

                    return "";
                }


            }
        }

        public string EnviaNotificacionesEmpleadosPeriodica(List<RestEmp> Empleados)
        {

            using (var db = new AutogestionContext())
            {

                string textocorreo = "";
                string txtde = Properties.Settings.Default.Correo.ToString();
                string contraseñacorreo = "";
                var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
                var email = "";
                var Nombresyfechas = "";
                foreach (var item in Empleados)
                {

                    email += "" + item.Empleado.Correo + " ";
                    //Nombresyfechas = Nombresyfechas + "<li>" + item.Empleado.Nombres + " Fecha a presentar: " + item.EvaPeriodica + "</li>";
                }
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

                string texto = "";

                texto = db.Configuraciones.First(s => s.Parametro == "TXT_EVADESH_EMP").Valor.ToString();
                string link = db.Configuraciones.First(s => s.Parametro == "LINKEVADESH").Valor.ToString();
                //Codigo Email
                //email= EmailJefe;
                /*texto = texto.Replace("$EMPLEADOS", Nombresyfechas);*/
                texto = texto.Replace("$TIPOEVA", "Periódica");
                texto = texto.Replace("$LINK", link);
                textocorreo = texto;
                string hostname = HttpContext.Current.Request.Url.Host;
                try
                {
                    if (hostname == "localhost")
                    {
                        correo.To.Add("desarrollador.junior1@foscal.com.co");
                    }
                    else
                    {
                        correo.To.Add(email);
                    }



                    correo.Subject = "Notificación Evaluaciones Periódicas";
                    correo.Body = textocorreo;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;

                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    smtp.Send(correo);
                    return "";
                }
                catch (Exception ex)
                {

                    return "";
                }


            }
        }

        public string EnviaNotificacionesEmpleadoEntrenamiento(List<RestEmp> Empleados)
        {

            using (var db = new AutogestionContext())
            {

                string textocorreo = "";
                string txtde = Properties.Settings.Default.Correo.ToString();
                string contraseñacorreo = "";
                var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
                var email = "";
                var Nombres = "";
                foreach (var item in Empleados)
                {

                    email += "" + item.Empleado.Correo + " ";
                    //Nombres = Nombres + "<li>" + item.Empleado.Nombres + "</li>";


                }
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

                string texto = "";

                texto = db.Configuraciones.First(s => s.Parametro == "TXT_EVADESH_EMP").Valor.ToString();
                string link = db.Configuraciones.First(s => s.Parametro == "LINKEVADESH").Valor.ToString();
                //Codigo Email
                //email= EmailJefe;
                texto = texto.Replace("$LINK", link);
                texto = texto.Replace("$TIPOEVA", "de Entrenamiento");
                textocorreo = texto;

                try
                {
                    //correo.To.Add("email");
                    correo.To.Add("desarrollador.junior1@foscal.com.co");

                    correo.Subject = "Notificación Evaluaciones de Entrenamiento " + DateTime.Today;
                    correo.Body = textocorreo;
                    correo.Priority = System.Net.Mail.MailPriority.Normal;
                    correo.IsBodyHtml = true;

                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                    smtp.EnableSsl = true;
                    smtp.Send(correo);
                    return "";
                }
                catch (Exception ex)
                {

                    return "";
                }


            }
        }

        public periodos CrearPeriodo(DateTime FechaInicial, DateTime FechaFinal, int Sociedad, int TipoP)
        {
            periodos NuevoP = new periodos();

            try
            {
                NuevoP.fechafinal = FechaFinal;
                NuevoP.fechaincio = FechaInicial;
                NuevoP.sociedad = Sociedad;
                NuevoP.TipoPeriodo = TipoP;
                db2.Periodos.Add(NuevoP);
                db2.SaveChanges();

            }
            catch (Exception ex)
            {

            }

            return NuevoP;
        }
       
        public string EnviaNotificacionesJefesSeguimientos(List<RestEmp> Empleados, Empleado Jefe)
        {

            using (var db = new AutogestionContext())
            {
                string hostname = HttpContext.Current.Request.Url.Host;
                string textocorreo = "";
                string txtde = Properties.Settings.Default.Correo.ToString();
                string contraseñacorreo = "";
                var encodedTextBytes = Convert.FromBase64String(Properties.Settings.Default.PassCorreo.ToString());
                contraseñacorreo = Encoding.UTF8.GetString(encodedTextBytes);
                //var email = "";
                var Nombres = "";
                foreach (var item in Empleados)
                {

               
                    Nombres = Nombres + "<li>" + item.Empleado.Nombres + "</li>";


                }
                System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
                correo.From = new System.Net.Mail.MailAddress(txtde);

                string texto = "";

                var config = db.Configuraciones.Where(s => s.Parametro == "TXTEVADESNOTIFICACIONSEG").FirstOrDefault();
                texto = config.Valor;
                //Codigo Email
             
               
               
                texto = texto.Replace("$EMPLEADOS", Nombres);
                textocorreo = texto;

                try
                {
                    //if (hostname == "localhost")
                    //{
                    //    correo.To.Add("desarrollador.junior1@foscal.com.co");
                    //}
                    //else
                    //{
                        correo.To.Add("desarrollador.senior2@foscal.com.co");
                        correo.To.Add("desarrollador.junior1@foscal.com.co");
                    // }
                    if (Jefe.Correo != null && Jefe.Correo != "")
                    {
                        correo.To.Add(Jefe.Correo);
                        correo.Subject = "Notificación Seguimientos Evaluación de Desempeño";
                        correo.Body = textocorreo;
                        correo.Priority = System.Net.Mail.MailPriority.Normal;
                        correo.IsBodyHtml = true;

                        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.Correo.ToString(), contraseñacorreo);
                        smtp.EnableSsl = true;
                        smtp.Send(correo);
                    }
                    return "";
                }
                catch (Exception ex)
                {

                    return "";
                }
            

            }
        }


        //-----------Pruebas---------------//
        [HttpGet]
        [Route("api/EvaDesempeño/Prueba3")]
        public void ActualizarDatosEmpleadoPrueba3()
        {

            DataTable Datos = new DataTable();
            var Result = "";
            var datos2 = new Retiros();

            InMemoryDestinationConfiguration DestinacionConfiguracion = new InMemoryDestinationConfiguration();

            String contraseña = Properties.Settings.Default.Contraseña.ToString();
            var encodedTextBytes = Convert.FromBase64String(contraseña);

            contraseña = Encoding.UTF8.GetString(encodedTextBytes);

            RfcConfigParameters config = new RfcConfigParameters();

            try
            {

                config.Add(RfcConfigParameters.Name, "SAP");
                config.Add(RfcConfigParameters.AppServerHost, Properties.Settings.Default.Servidor.ToString());
                config.Add(RfcConfigParameters.SystemNumber, Properties.Settings.Default.Id.ToString());
                config.Add(RfcConfigParameters.User, Properties.Settings.Default.Usuario.ToString());
                config.Add(RfcConfigParameters.Password, contraseña);
                config.Add(RfcConfigParameters.Client, Properties.Settings.Default.Mandante.ToString());
                config.Add(RfcConfigParameters.Language, "ES");
                config.Add(RfcConfigParameters.SAPRouter, Properties.Settings.Default.Saprouter.ToString());
                config.Add(RfcConfigParameters.LogonGroup, "Foscal");

                RfcDestinationManager.RegisterDestinationConfiguration(DestinacionConfiguracion);
                DestinacionConfiguracion.AddOrEditDestination(config);
                RfcDestination destination = RfcDestinationManager.GetDestination("SAP");

                RfcRepository repository = destination.Repository;
                IRfcFunction function = repository.CreateFunction("ZMF_EMPLEADOS");
                //PARAMETROS IMPORT
                DateTime date = DateTime.Now;
                DateTime date2 = date.AddDays(1);
                var dia = date2.Day;
                var mes = date2.Month;
                var año = date2.Year;
                string FECHA = dia + "." + mes + "." + año;
                function.SetValue("I_PARAMETRO", 2);
                function.SetValue("I_PERSG", 1);
                function.SetValue("I_FECHAI", "");
                function.SetValue("I_FECHAF", Convert.ToDateTime(FECHA));

                function.Invoke(destination);
                //OBTENER RESPUESTA 
                IRfcTable Tabla = function.GetTable("T_DATOS");
                ArrayList EMPSAPLIST = new ArrayList();
                List<Empleado> EMPSAPLIST2 = new List<Empleado>();
                Datos = GetDataTableFromRFCTable(Tabla);
                int count = Datos.Rows.Count;
                int s = count - 1;
                for (var f = 0; f < count; f++)
                {
                    var EMPLEADO = new Empleado();
                    EMPLEADO.NroEmpleado = Datos.Rows[f]["PERNR"].ToString();
                    var estado = Datos.Rows[f]["PERSG"].ToString();
                    if (estado == "1")
                    {
                        EMPLEADO.Activo = "SI";
                    }
                    else
                    {
                        EMPLEADO.Activo = "NO";
                    }
                    var fechafin = Datos.Rows[f]["ENDDA"].ToString();
                    var fecchafin2 = Convert.ToDateTime(fechafin);
                    EMPLEADO.FechaFin = fecchafin2;
                    EMPLEADO.Empresa = Datos.Rows[f]["BUKRS"].ToString();
                    EMPLEADO.Nombres = Datos.Rows[f]["ENAME"].ToString();
                   
                    var codcargo = Datos.Rows[f]["PLANS"].ToString();
                    var codcargo2 = Convert.ToInt32(codcargo);
                    EMPLEADO.CodigoSAPCargo = codcargo2;
                    EMPLEADO.UnidadOrganizativa = Datos.Rows[f]["ORGEH"].ToString();
                    EMPLEADO.Area = Datos.Rows[f]["AREA"].ToString();
                    EMPLEADO.RH = Datos.Rows[f]["RH"].ToString();
                    EMPLEADO.Genero = Datos.Rows[f]["GESCH"].ToString();
                    var fechaNaci = Datos.Rows[f]["GBDAT"].ToString();
                    var fechaNaci2 = Convert.ToDateTime(fechaNaci);
                    EMPLEADO.FechaNacimiento = fechaNaci2;
                    EMPLEADO.Documento = Datos.Rows[f]["PERID"].ToString();
                    EMPLEADO.Cargo = Datos.Rows[f]["CARGO"].ToString();


                    EMPSAPLIST2.Add(EMPLEADO);
                    string[] Result2 = { Datos.Rows[f]["PERNR"].ToString(), Datos.Rows[f]["PERSG"].ToString(), Datos.Rows[f]["ENDDA"].ToString(),
                            Datos.Rows[f]["BUKRS"].ToString(), Datos.Rows[f]["ENAME"].ToString(), Datos.Rows[f]["CARGO"].ToString(),
                            Datos.Rows[f]["PLANS"].ToString(),Datos.Rows[f]["ORGEH"].ToString(),Datos.Rows[f]["AREA"].ToString(),
                            Datos.Rows[f]["EPS"].ToString(),Datos.Rows[f]["RH"].ToString(),Datos.Rows[f]["GESCH"].ToString(),
                            Datos.Rows[f]["GBDAT"].ToString(),Datos.Rows[f]["FTEXT"].ToString(),
                            Datos.Rows[f]["FAMST"].ToString(),Datos.Rows[f]["PERID"].ToString()
                        };
                    EMPSAPLIST.Add(Result2);

                }

                var debug = 0;
            } catch
            {
            
            }
                //using (var db = new AutogestionContext())
                //{

                //    DataTable Datos = new DataTable();
                //    var Result = "";
                //    var datos2 = new Retiros();

                //    InMemoryDestinationConfiguration DestinacionConfiguracion = new InMemoryDestinationConfiguration();

                //    String contraseña = Properties.Settings.Default.Contraseña.ToString();
                //    var encodedTextBytes = Convert.FromBase64String(contraseña);

                //    contraseña = Encoding.UTF8.GetString(encodedTextBytes);

                //    RfcConfigParameters config = new RfcConfigParameters();

                //    try
                //    {

                //        config.Add(RfcConfigParameters.Name, "SAP");
                //        config.Add(RfcConfigParameters.AppServerHost, Properties.Settings.Default.Servidor.ToString());
                //        config.Add(RfcConfigParameters.SystemNumber, Properties.Settings.Default.Id.ToString());
                //        config.Add(RfcConfigParameters.User, Properties.Settings.Default.Usuario.ToString());
                //        config.Add(RfcConfigParameters.Password, contraseña);
                //        config.Add(RfcConfigParameters.Client, Properties.Settings.Default.Mandante.ToString());
                //        config.Add(RfcConfigParameters.Language, "ES");
                //        config.Add(RfcConfigParameters.SAPRouter, Properties.Settings.Default.Saprouter.ToString());
                //        config.Add(RfcConfigParameters.LogonGroup, "Foscal");

                //        RfcDestinationManager.RegisterDestinationConfiguration(DestinacionConfiguracion);
                //        DestinacionConfiguracion.AddOrEditDestination(config);
                //        RfcDestination destination = RfcDestinationManager.GetDestination("SAP");

                //        RfcRepository repository = destination.Repository;
                //        IRfcFunction function = repository.CreateFunction("ZMF_CREAREPISODIO");
                //        //PARAMETROS IMPORT
                //        function.SetValue("I_PASSNR", "1099366461");
                //        function.SetValue("I_PASSTY", "CC");
                //        function.SetValue("I_EINRI", "1000");
                //        function.SetValue("I_KH_KOSTR", "20009540");
                //        function.SetValue("I_ORGFA", "1MURGEN");
                //        function.SetValue("I_ORGPF", "1TTCP5U1");


                //        function.Invoke(destination);
                //        //OBTENER RESPUESTA 


                //        string NFALNR = function.GetString("E_FALNR");
                //        //string SegundoApellido = function.GetString("E_GBNAM4");
                //        string PrimerNombre = function.GetString("E_VNAME1");
                //        string SegundoNombre = function.GetString("E_NAME2");
                //        string PrimerApellido = function.GetString("E_NNAME3");
                //        string RESULT   = function.GetString("E_RESULT");


                //    }
                //    catch (SystemException ex)
                //    {
                //        // ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
                //    }
                //    catch (RfcLogonException ex)
                //    {

                //    }
                //    catch (RfcAbapException ex)
                //    {
                //        //ErrorLog(ex.Message + ";Conectar a Sap;Clase Procedimientos;Rutina Consultar Citas");
                //    }
                //    finally
                //    {
                //        RfcDestinationManager.UnregisterDestinationConfiguration(DestinacionConfiguracion);
                //        // DestinacionConfiguracion.RemoveDestination("SAP");
                //    }



                //}
            }

        [HttpGet]
        [Route("api/EvaDesempeño/InformeEstructurasJerarquicas")]
        public void InformeEstructurasJerarquicas()
        {
            //var Empleado = db.Empleados.Where(x => x.NroEmpleado == "40005803" && x.NroEmpleado == "40005803").ToList() ;
            //foreach (var item in Empleado) 
            //{

            //}
            List<AreasAdmAutogestion> Areas = new List<AreasAdmAutogestion>();
            var lista1000 = db.Empleados.Where(s =>s.Empresa=="1000" && s.Activo == "SI" && (s.AreaDescripcion != null && s.AreaDescripcion != "")).GroupBy(b => b.UnidadOrganizativa).ToList();
            var list_estructuras = db.EstructuraJerarquica.ToList();
            foreach (var x in lista1000)
            {
                Empleado Item = x.FirstOrDefault();
                var Areas1  = new AreasAdmAutogestion();
                Areas1.NombreArea = Item.AreaDescripcion;
                Areas1.UnidadOrganizativa = Item.UnidadOrganizativa;
                Areas1.Sociedad = "1000";
                var estructura = list_estructuras.Where(O => O.UnidadOrg == Item.UnidadOrganizativa && O.Sociedad =="1000").FirstOrDefault();
                if (estructura == null)
                {
                    Areas1.Estructura = "NO";
                }
                else
                {
                    Areas1.Estructura = "SI";
                }
                Areas.Add(Areas1);

             }

            var lista2000 = db.Empleados.Where(s => s.Empresa == "2000" && s.Activo == "SI" && (s.AreaDescripcion != null & s.AreaDescripcion != "")).GroupBy(b => b.UnidadOrganizativa).ToList();
            foreach (var x in lista2000)
            {
                Empleado Item = x.FirstOrDefault();
                var Areas1 = new AreasAdmAutogestion();
                Areas1.NombreArea = Item.AreaDescripcion;
                Areas1.UnidadOrganizativa = Item.UnidadOrganizativa;
                Areas1.Sociedad = "2000";
                var estructura = list_estructuras.Where(O => O.UnidadOrg == Item.UnidadOrganizativa && O.Sociedad == "2000").FirstOrDefault();
                if (estructura==null) 
                {
                    Areas1.Estructura = "NO";
                }
                else 
                {
                    Areas1.Estructura = "SI";
                }
                Areas.Add(Areas1);

            }
            var debug = 0;

        }

        [HttpGet]
        [Route("api/EvaDesempeño/ConsultaErrores")]
        public string ConsultaErrores()
        {
            try
            {

                var EmpleadosAdm = db.Empleados.ToList();
                var PersonalActivo = db.PersonalActivo.ToList();
                List<Empleado> Duplicados = new List<Empleado>();
                List<PersonalActivo> DuplicadosPactivo = new List<PersonalActivo>();
                foreach (var Emp in EmpleadosAdm) 
                {
                    var Dup = EmpleadosAdm.Where(x=>x.NroEmpleado == Emp.NroEmpleado && x.Id != Emp.Id).FirstOrDefault();
                    if (Dup !=null) 
                    {
                        Duplicados.Add(Dup);
                    }
                
                }

                foreach(var item in PersonalActivo) 
                {
                    var Dup = PersonalActivo.Where(x => x.CodigoEmpleado == item.CodigoEmpleado && x.Id != item.Id).FirstOrDefault();
                    if (Dup != null)
                    {
                        DuplicadosPactivo.Add(Dup);
                    }

                }

                var debug = 0;

                //var EmpAutogestión = db.Empleados.Where(x => x.Activo == "NO").ToList();
                //var EXE = db2.Encuestadoresxempleado.ToList();
                //List<Empleado> ListaEmps = new List<Empleado>();
                //foreach (var Emp in EmpAutogestión) 
                //{
                //    var Nmrp = Convert.ToInt32(Emp.NroEmpleado);
                //    var Eva = EXE.Where(x => x.codigoempleado == Nmrp).ToList();
                //    if ( Eva.Count()>0) 
                //    {
                //        ListaEmps.Add(Emp);  
                //    }


                //}
                //var debug = 0;

                //var empleados = db2.empleados.Where(x => x.estado == 1).ToList();
                //var ExE = db2.Encuestadoresxempleado.ToList();

                //var Encabezados = db2.Evaluacion_encabezado.ToList();

                //var Otros = db2.Evaluacion_otros.ToList();
                //var Conceptuales = db2.Evaluacion_conductuales.ToList();
                //var Institucionales = db2.Evaluacion_institucionales.ToList();
                //var Generales = db2.Evaluacion_aspecto_general.ToList();    
                //foreach (var EMP in empleados) 
                //{
                //    var Result = ExE.Where(x => x.codigoempleado == EMP.codigo).ToList();
                //    if (Result != null || Result.Count>0) 
                //    {
                //        var Periodica = Result.Where(x => x.tipoevaluacion == 1).FirstOrDefault();
                //        var Autoevaluacion = Result.Where(x => x.tipoevaluacion == 2).FirstOrDefault();
                //        if (Periodica!=null && Autoevaluacion !=null) 
                //        {


                        
                //        }


                //    }
                //}
                return "";
            }
            catch (Exception ex)
            {

                return "" + ex;
            }

        }
        [HttpGet]
        [Route("api/EvaDesempeño/ActualizarEMP/{NmrEmp}")]
        public string ActualizarEMP(string NmrEmp)
        {
            try
            {


                Empleado empleado = db.Empleados.Where(x => x.NroEmpleado == NmrEmp).FirstOrDefault();
                Empleado Empleado = new Empleado();
                PersonalActivo ps = new PersonalActivo();
                DateTime Fecha = Convert.ToDateTime(DateTime.Today);
                Empleado ItemEmpleado = new Empleado();
                using (var db = new AutogestionContext())
                {
                    var RepoEmp = new EmpleadoRepository();
                    RepoEmp.ProcesoActualizacionyRetiradosIndividual(empleado);
                    return "true";
                    //try
                    //{


                    //    //Retirar empleados que ya no esten activos en sap
                    //    Empleado = db.Empleados.First(x => x.Id == empleado.Id);
                    //    DataTable Dt = new DataTable();
                    //    EmpleadoRepository EmpleadoServices = new EmpleadoRepository();

                    //    if (Empleado != null)
                    //    {
                    //        if (Empleado.Activo != "NO")
                    //        {
                    //            Dt = EmpleadoServices.ValidarEmpleadoActivo(Empleado.Documento);

                    //            if (Dt.Rows.Count < 1)
                    //            {
                    //                Empleado.Activo = "NO";
                    //                Empleado.FechaFin = DateTime.Now.Date;
                    //                Empleado.A_UsuarioModifica = "99999999";
                    //                Empleado.A_Modificacion = DateTime.Now;
                    //                db.SaveChanges();
                    //                ps = db.PersonalActivo.FirstOrDefault(e => e.Documento == Empleado.Documento);
                    //                if (ps != null)
                    //                {
                    //                    db.PersonalActivo.Remove(ps);
                    //                    db.SaveChanges();
                    //                }


                    //            }

                    //        }
                    //    }



                    //    //Retirar empleados que ya no esten activos en sap
                    //    Empleado = db.Empleados.First(x => x.Id == empleado.Id);
                    //    DataTable Dt2 = new DataTable();
                    //    EmpleadoRepository EmpleadoServices2 = new EmpleadoRepository();

                    //    if (Empleado != null)
                    //    {
                    //        if (Empleado.Activo == "NO")
                    //        {
                    //            Dt2 = EmpleadoServices2.ValidarEmpleadoActivo(Empleado.Documento);

                    //            if (Dt2.Rows.Count >= 1)
                    //            {
                    //                Empleado.Activo = "SI";
                    //                Empleado.FechaFin = null;
                    //                Empleado.A_UsuarioModifica = "99999999";
                    //                Empleado.A_Modificacion = DateTime.Now;
                    //                db.SaveChanges();

                    //            }
                    //        }
                    //    }





                    //    //Eliminar de la tabla personal activo los trabadores retirados
                    //    Empleado = db.Empleados.First(x => x.Id == empleado.Id);

                    //    if (Empleado != null)
                    //    {
                    //        if (Empleado.Activo == "NO" && Empleado.FechaFin.Value.Date < Fecha)
                    //        {


                    //            ps = db.PersonalActivo.FirstOrDefault(e => e.Documento == Empleado.Documento);
                    //            if (ps != null)
                    //            {
                    //                    db.PersonalActivo.Remove(ps);
                    //                db.SaveChanges();
                    //            }
                    //        }
                    //    }

                    //    //Actualizar datos empleados activos
                    //    Empleado = db.Empleados.First(x => x.Id == empleado.Id);
                    //    EmpleadoRepository services = new EmpleadoRepository();

                    //    if (Empleado != null)
                    //    {
                    //        if (Empleado.Activo != "NO")
                    //        {
                    //            services.ActualizarDatosEmpleado(Empleado.NroEmpleado);

                    //            //var query = from row in Dt.AsEnumerable()
                    //            //            where row.Field<string>("PERNR") == Empleado.NroEmpleado
                    //            //            select new
                    //            //            {
                    //            //                Sociedad = row.Field<string>("BUKRS")
                    //            //            };


                    //            var EMPArray = Dt.AsEnumerable().FirstOrDefault().ItemArray;                 
                    //            Empleado = db.Empleados.First(x => x.Id == empleado.Id);
                    //            //Empleado.Empresa = query.FirstOrDefault().Sociedad;
                    //            Empleado.Empresa = EMPArray[3].ToString();
                    //            Empleado.UnidadOrganizativa = EMPArray[7].ToString();
                    //            Empleado.Area = EMPArray[8].ToString();
                    //            Empleado.A_UsuarioModifica = "99999999";
                    //            Empleado.A_Modificacion = DateTime.Now;
                    //            db.SaveChanges();

                    //            if (db.PersonalActivo.Where(x => x.CodigoEmpleado == Empleado.NroEmpleado).ToList().Count() < 1)
                    //            {

                    //                var personalActivo = new PersonalActivo();
                    //                personalActivo.Area = Empleado.Area;
                    //                personalActivo.Cargo = Dt.Rows[0]["CARGO"].ToString();
                    //                personalActivo.CodigoEmpleado = Empleado.NroEmpleado;
                    //                personalActivo.Documento = Empleado.Documento;
                    //                personalActivo.Empresa = Empleado.Empresa;
                    //                personalActivo.Nombres = Empleado.Nombres;
                    //                personalActivo.UnidadOrganizativa = Empleado.UnidadOrganizativa;
                    //                db.PersonalActivo.Add(personalActivo);
                    //                db.SaveChanges();
                    //            }
                    //        }
                    //    }


                    //    var fileSavePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/LogJob"), "logJobUPDEmpleado.txt");


                    //    // Write the string array to a new file named "WriteLines.txt".
                    //    using (StreamWriter w = File.AppendText(fileSavePath))
                    //    {
                    //        w.Write(Environment.NewLine);
                    //        w.Write("[{0} {1}]", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                    //        w.Write("\t");
                    //        w.WriteLine(" {0}", "Ejecución correcta Empl: " + DateTime.Now);
                    //        w.WriteLine("-----------------------");
                    //    }


                    //}
                    //catch (Exception ex)
                    //{
                    //    var fileSavePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/LogJob"), "logJobUPDEmpleado.txt");

                    //    // Write the string array to a new file named "WriteLines.txt".
                    //    using (StreamWriter w = File.AppendText(fileSavePath))
                    //    {
                    //        w.Write(Environment.NewLine);
                    //        w.Write("[{0} {1}]", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                    //        w.Write("\t");
                    //        w.WriteLine(" {0}", "Error Empl: " + ex.Message.ToString() + DateTime.Now);
                    //        w.WriteLine("-----------------------");
                    //    }


                    //}
                }
            }
            catch (Exception ex)
            {
                return "false: " + ex;
            }

            }

        //[HttpGet]
        //[Route("api/ActualizarConfig")]
        //public string ActualizarConfig() 
        //{
        //    var config = db.Configuraciones.Where(x => x.Parametro == "MENSG_CERTIFICACIONES1000").FirstOrDefault();
        //        config.Valor = "Apreciado Trabajador de Fundación Foscal,\r\n\r\nCordial saludo.\r\n\r\nLa FUNDACIÓN OFTALMOLÓGICA DE SANTANDER -FOSCAL- desea informarle que la generación de la nómina correspondiente al mes de enero de 2024 presentó errores que afectaron incluso la contabilidad de la empresa. Es importante señalar que los documentos descargados desde el aplicativo AUTOGESTIÓN antes del día 04 de febrero de 2024 y que corresponde a la nomina de Enero 2024 reflejarán no solo un salario que no corresponde al acordado entre las partes, sino que también mostrarán un descuento incorrecto para el mes de enero.\r\n\r\nYa se han realizado los ajustes necesarios, sin embargo, si en su caso particular los inconvenientes persisten y la información consignada no corresponde a la realidad agradecemos hacérnoslo saber para dar la solución en el menor tiempo posible. \r\n\r\nLamentamos sinceramente los inconvenientes causados y agradecemos su comprensión. ";
        //          db.SaveChanges();
        //    return "";        
        //}

    }
    }



