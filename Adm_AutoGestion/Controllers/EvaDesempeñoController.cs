using Adm_AutoGestion.Controllers.Api;
using Adm_AutoGestion.Models;
using Adm_AutoGestion.Models.EvaDesempeno;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Adm_AutoGestion.Controllers.EncabezadoPrestamoController;
using System.Data.Entity.SqlServer;
using Adm_AutoGestion.Services;
using System.Runtime.ConstrainedExecution;

namespace Adm_AutoGestion.Controllers
{
    public class EvaDesempeñoController : Controller
    {
        private EvaDesempenoContext db2 = new EvaDesempenoContext();
        
        public ActionResult InformeEvaPendientes(string FechaInicio, string FechaFin, string TipoPresentacion2, string TipoPresentacionAUT, string TipoPrueba, string Empleados, string RetroEmp ,string PeriodosParametros)
        {

            List<EvaDesempenoController.RestEmp> ListEmp = new List<EvaDesempenoController.RestEmp>();
            List<EvaDesempenoController.RestEmp> ListEmp2 = new List<EvaDesempenoController.RestEmp>();
            DateTime HoYConstante = DateTime.Now;
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("InformeEvaPendientes"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            EvaDesempenoController EvaDesempeño = new EvaDesempenoController();
            using (var db = new AutogestionContext())
            {

                //if (TipoPrueba != "" && (TipoPresentacion == 0 || TipoPresentacion == null))
                //{
                //    //throw new ArgumentOutOfRangeException(null, "Definir Rango de Fechas");
                //    Session["message"] = "Se debe seleccionar el estado de la prueba";
                //}
                DateTime FechaI = DateTime.Now;
                DateTime.TryParse(FechaInicio, out FechaI);
                DateTime FechaF = DateTime.Now;
                if (FechaFin!="")
                {
                    DateTime.TryParse(FechaFin, out FechaF);
                }
               
                List<reporteprestamos> consulta = new List<reporteprestamos>();
                int Usuario = 0;
                var usuariolog = String.Format("{0}", Session["Empleado"]);
                Int32.TryParse(usuariolog, out Usuario);
                var Jefe = db.Empleados.FirstOrDefault(e => e.Id == Usuario);
                bool RetroEMP2 = false;
                List<string> ListEmpsPeriodoSelecionado = new List<string>();
                List<Empleado> EmpsPActual = new List<Empleado>();
                if (RetroEmp =="Si") 
                {
                    RetroEMP2 = true;
                }
                else 
                {
                    RetroEMP2 = false;
                }
                //Tipo prueba 1 = Periodica
                //Tipo prueba 2 = Entrenamiento
                if (TipoPresentacion2=="") 
                {
                    TipoPresentacion2 = "0";
                }
                if (TipoPresentacionAUT== "") 
                {
                    TipoPresentacionAUT = "0";
                }
                int TipoPresentacion = Convert.ToInt32(TipoPresentacion2);
                int TipoPresentacionA = Convert.ToInt32(TipoPresentacionAUT);
                ListEmp = EvaDesempeño.ConsultaEmpleado(Jefe.NroEmpleado, PeriodosParametros);               
                if (Empleados!="" && Empleados !=null) 
                {
                    var empleado = Convert.ToInt32(Empleados);
                    if (TipoPrueba=="2") 
                    {
                        if (TipoPresentacion != 0)
                        {
                            ListEmp2 = ListEmp.Where(x => x.FecharegistroEntrenamiento >= FechaI && x.FecharegistroEntrenamiento <= FechaF
                        && x.Empleado.Id == empleado && x.EstadoE == TipoPresentacion).ToList();
                        }
                        else
                        {
                            ListEmp2 = ListEmp.Where(x => x.FecharegistroEntrenamiento >= FechaI && x.FecharegistroEntrenamiento <= FechaF
                         && x.Empleado.Id == empleado).ToList();
                        }

                    }
                    else if (TipoPrueba == "1") 
                    {
                        if (TipoPresentacion != 0)
                        {
                            ListEmp2 = ListEmp.Where(x => x.FecharegistroPeriodica >= FechaI && x.FecharegistroPeriodica <= FechaF
                             && x.Empleado.Id == empleado && x.EstadoP == TipoPresentacion).ToList();
                        }
                        else
                        {
                            ListEmp2 = ListEmp.Where(x => x.FecharegistroPeriodica >= FechaI && x.FecharegistroPeriodica <= FechaF
                             && x.Empleado.Id == empleado).ToList();
                        }

                    }
                    else if (TipoPrueba == "") 
                    {
                        if (TipoPresentacion!=0)
                        {
                            ListEmp2 = ListEmp.Where(x => ((x.FecharegistroEntrenamiento >= FechaI && x.FecharegistroEntrenamiento <= FechaF)
                      || (x.FecharegistroPeriodica >= FechaI && x.FecharegistroPeriodica <= FechaF))
                      && (x.Empleado.Id == empleado) && (x.EstadoP == TipoPresentacion || x.EstadoE == TipoPresentacion)).ToList();

                        }
                        else
                        {
                            ListEmp2 = ListEmp.Where(x => ((x.FecharegistroEntrenamiento >= FechaI && x.FecharegistroEntrenamiento <= FechaF)
                      || (x.FecharegistroPeriodica >= FechaI && x.FecharegistroPeriodica <= FechaF))
                      && (x.Empleado.Id == empleado)).ToList();

                        }

                    }
                    
                }
                else if (Empleados=="") 
                {
                    if (TipoPrueba == "2")
                    {
                        if (TipoPresentacion != 0)
                        {
                            ListEmp2 = ListEmp.Where(x => x.FecharegistroEntrenamiento >= FechaI && x.FecharegistroEntrenamiento <= FechaF
                             && (x.EstadoE == TipoPresentacion)).ToList();
                        }
                        else
                        {
                            ListEmp2 = ListEmp.Where(x => x.FecharegistroEntrenamiento >= FechaI
                            && x.FecharegistroEntrenamiento <= FechaF).ToList();
                        }

                    }
                    else if (TipoPrueba == "1")
                    {
                        if (TipoPresentacion!=0)
                        {
                            ListEmp2 = ListEmp.Where(x => x.FecharegistroPeriodica >= FechaI && x.FecharegistroPeriodica <= FechaF
                      && x.EstadoP == TipoPresentacion).ToList();
                        }
                        else
                        {
                            ListEmp2 = ListEmp.Where(x => x.FecharegistroPeriodica >= FechaI && x.FecharegistroPeriodica <= FechaF).ToList();
                        }
                    }
                    else if (TipoPrueba == "")
                    {
                        if ( TipoPresentacion !=0)
                        {

                            ListEmp2 = ListEmp.Where(x => ((x.FecharegistroPeriodica >= FechaI && x.FecharegistroPeriodica <= FechaF)
                            || (x.FecharegistroEntrenamiento >= FechaI && x.FecharegistroEntrenamiento <= FechaF)) && (x.EstadoP == TipoPresentacion || x.EstadoE == TipoPresentacion)).ToList();
                        }
                        else
                        {

                            ListEmp2 = ListEmp.Where(x => ((x.FecharegistroPeriodica >= FechaI && x.FecharegistroPeriodica <= FechaF)
                            || (x.FecharegistroEntrenamiento >= FechaI && x.FecharegistroEntrenamiento <= FechaF))).ToList();
                        }
                    }

                }
                if (TipoPresentacionA != 0 && RetroEmp != null) { 
                    foreach (EvaDesempenoController.RestEmp item in ListEmp2.Reverse<EvaDesempenoController.RestEmp>())
                    {
                    //RETOALIMENTADO EMP
                        if ((RetroEmp != "" && RetroEmp != null))
                        {
                            if (item.RetroalimentacionEmp != RetroEMP2)
                            {
                                ListEmp2.Remove(item);
                            }
                        }
                      //ESTADO AUTOEVALUACIÓN
                        if (item.EstadoA != TipoPresentacionA && TipoPresentacionA!= 0) 
                            {
                                ListEmp2.Remove(item);
                            }
                    }
                }


                var lista = db.Empleados.Select(x => new { x.AreaDescripcion }).GroupBy(b => b.AreaDescripcion).ToList();
                List<Empleado> Emps = new List<Empleado>();
                List<tipo_evaluacion> TipoEv = new List<tipo_evaluacion>();
                List<Estructura_Jerarquica_EVADES> STREvadesh = new List<Estructura_Jerarquica_EVADES>();
                 STREvadesh = db2.Estructura_Jerarquica_EVADES.Where(x => x.Jefe == Jefe.NroEmpleado).ToList();
                //Emps = db.Empleados.Where(f =>  f.Jefe == Jefe.NroEmpleado && f.TipoArea != "SENA Pctivo/Pasante" && f.TipoArea != "SENA Lectivo").ToList();
                //if (STREvadesh.Count>0) 
                //{
                    
                //    foreach (var item in STREvadesh) 
                //    {
                //        List<Empleado> Emps2 = new List<Empleado>();
                //         Emps2 = db.Empleados.Where(x => x.AreaDescripcion == item.Area && x.Activo == "SI").ToList();
                //        if (Emps2.Count() > 0 ) 
                //        {
                //        Emps.AddRange( Emps2);
                //        }
                //    }
                    
                //}

                foreach (var item in ListEmp) 
                {
                    string NroEmpleadoH = item.Empleado.NroEmpleado;
                    ListEmpsPeriodoSelecionado.Add(NroEmpleadoH);
                }
                EmpsPActual.AddRange(db.Empleados.Where(x=> ListEmpsPeriodoSelecionado.Contains(x.NroEmpleado)).ToList() );
                ViewBag.Empleados = EmpsPActual;
                ViewBag.TipoPrueba = db2.Tipo_evaluacion.Where(x=>x.codigo ==1).ToList();
                ViewBag.Sociedad = db.Sociedad.ToList();
                ViewBag.LinkEvadesh =db.Configuraciones.Where(x=>x.Parametro == "LINKEVADESH").FirstOrDefault().Valor;
                ViewBag.Estados = db2.EstadosEvaluacionEncabezado.Where(x => x.Codigo != 0 && x.Codigo != 6 && x.Descripcion != "").ToList();
                ViewBag.PeriodosParametros = db2.Periodos_Evaluacion_Desempeño.Where(x => x.Año <= HoYConstante.Year).ToList();


                return View(ListEmp2.OrderBy(x=>x.Empleado.Nombres));

            }


        }
        public ActionResult InformeEvaPendientesGH(int? PeriodosParametros, string TipoPresentacion2, string TipoPresentacionAUT , string TipoPrueba, string Empleados, string Areas, string RetroEmp)
        {
            DateTime HoYConstante = DateTime.Now;
            List<EvaDesempenoController.RestEmp> ListEmp = new List<EvaDesempenoController.RestEmp>();
            List<EvaDesempenoController.RestEmp> ListEmp2 = new List<EvaDesempenoController.RestEmp>();

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("InformeEvaPendientesGH"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            EvaDesempenoController EvaDesempeño = new EvaDesempenoController();
            using (var db = new AutogestionContext())
            {
                periodos Periodo = new periodos();
                //if (TipoPrueba != "" && (TipoPresentacion == 0 || TipoPresentacion == null))
                //{
                //    //throw new ArgumentOutOfRangeException(null, "Definir Rango de Fechas");
                //    Session["message"] = "Se debe seleccionar el estado de la Prueba";
                //}
                if (PeriodosParametros != null) 
                {
                
                
                var ParametroPeriodo = db2.Periodos_Evaluacion_Desempeño.Where(x => x.Año == PeriodosParametros).FirstOrDefault();
                 Periodo = db2.Periodos.Where(x => x.fechaincio == ParametroPeriodo.FechaInicial && x.fechafinal == ParametroPeriodo.FechaFinal && x.TipoPeriodo == 1).FirstOrDefault();
                
                //DateTime.TryParse(FechaInicio, out FechaI);
                //DateTime FechaF = DateTime.Now;
                //if (FechaFin != "")
                //{
                //    DateTime.TryParse(FechaFin, out FechaF);
                //}

                List<reporteprestamos> consulta = new List<reporteprestamos>();
                int Usuario = 0;
                var usuariolog = String.Format("{0}", Session["Empleado"]);
                Int32.TryParse(usuariolog, out Usuario);
                var Jefe = db.Empleados.FirstOrDefault(e => e.Id == Usuario);
                bool RetroEMP2 = false;
                if (RetroEmp == "Si")
                {
                    RetroEMP2 = true;
                }
                else
                {
                    RetroEMP2 = false;
                }
                //Tipo prueba 1 = Periodica
                //Tipo prueba 2 = Entrenamiento
                if (PeriodosParametros != 0 && TipoPresentacion2!=null && TipoPrueba!=null) 
                {
                    ListEmp = EvaDesempeño.ConsultaEmpleadoGH(Periodo.codigo);
                }
                
                if (TipoPresentacion2 == "")
                {
                    TipoPresentacion2 = "0";
                }
                    if (TipoPresentacionAUT == "")
                    {
                        TipoPresentacionAUT = "0";
                    }
                    int TipoPresentacion = Convert.ToInt32(TipoPresentacion2);
                    int TipoPresentacionA = Convert.ToInt32(TipoPresentacionAUT);
                  
                    if (Empleados != "" && Empleados != null)
                {
                    var empleado = Convert.ToInt32(Empleados);
                    if (TipoPrueba == "2")
                    {
                        //if (TipoPresentacion != 0)
                        //{
                        //    ListEmp2 = ListEmp.Where(x => x.FecharegistroEntrenamiento >= FechaI && x.FecharegistroEntrenamiento <= FechaF
                        //&& x.Empleado.Id == empleado && x.EstadoE == TipoPresentacion).ToList();
                        //}
                        //else
                        //{
                        //    ListEmp2 = ListEmp.Where(x => x.FecharegistroEntrenamiento >= FechaI && x.FecharegistroEntrenamiento <= FechaF
                        // && x.Empleado.Id == empleado).ToList();
                        //}

                    }
                    else if (TipoPrueba == "1")
                    {
                        if (TipoPresentacion != 0)
                        {
                            ListEmp2 = ListEmp.Where(x => x.PeriodoCod >= Periodo.codigo  
                             && x.Empleado.Id == empleado && x.EstadoP == TipoPresentacion).ToList();
                        }
                        else
                        {
                            ListEmp2 = ListEmp.Where(x => x.PeriodoCod >= Periodo.codigo 
                             && x.Empleado.Id == empleado).ToList();
                        }

                    }
                    else if (TipoPrueba == "")
                    {
                        if (TipoPresentacion != 0)
                        {
                            ListEmp2 = ListEmp.Where(x => (x.PeriodoCod >= Periodo.codigo) &&
                       (x.Empleado.Id == empleado) && (x.EstadoP == TipoPresentacion || x.EstadoE == TipoPresentacion)).ToList();

                        }
                        else
                        {
                            ListEmp2 = ListEmp.Where(x =>
                             (x.PeriodoCod >= Periodo.codigo)
                      && (x.Empleado.Id == empleado)).ToList();

                        }

                    }

                }
                else if (Empleados == "")
                {
                    if (TipoPrueba == "2")
                    {
                        if (TipoPresentacion != 0)
                        {
                            ListEmp2 = ListEmp.Where(x => x.PeriodoCod >= Periodo.codigo
                             && (x.EstadoE == TipoPresentacion)).ToList();
                        }
                        else
                        {
                            ListEmp2 = ListEmp.Where(x => x.PeriodoCod >= Periodo.codigo).ToList();
                        }

                    }
                    else if (TipoPrueba == "1")
                    {
                        if (TipoPresentacion != null && TipoPresentacion != 0)
                        {
                            ListEmp2 = ListEmp.Where(x => x.PeriodoCod >= Periodo.codigo
                      && x.EstadoP == TipoPresentacion).ToList();
                        }
                        else
                        {
                            ListEmp2 = ListEmp.Where(x => x.PeriodoCod >= Periodo.codigo).ToList();
                        }
                    }
                    else if (TipoPrueba == "")
                    {
                        if (TipoPresentacion != null && TipoPresentacion != 0)
                        {

                            ListEmp2 = ListEmp.Where(x => x.PeriodoCod >= Periodo.codigo
                                && (x.EstadoP == TipoPresentacion || x.EstadoE == TipoPresentacion)).ToList();
                        }
                        else
                        {

                            ListEmp2 = ListEmp.Where(x => x.PeriodoCod >= Periodo.codigo).ToList();
                        }
                    }

                }

                if (Areas!=null && Areas!="") { 
                foreach (var item in ListEmp2.Reverse<EvaDesempenoController.RestEmp>())
                {
                    if (item.Empleado.AreaDescripcion != Areas) 
                    {
                        ListEmp2.Remove(item);
                    }
                }
                }
                    //foreach (EvaDesempenoController.RestEmp item in ListEmp2.Reverse<EvaDesempenoController.RestEmp>()) 
                    //{                                    
                    //}
                    //var lista = db.Empleados.Select(x => new { x.AreaDescripcion }).GroupBy(b => b.AreaDescripcion).ToList();

                    //List<tipo_evaluacion> TipoEv = new List<tipo_evaluacion>();

                    if (TipoPresentacionA != 0 && RetroEmp != null)
                    {
                        foreach (EvaDesempenoController.RestEmp item in ListEmp2.Reverse<EvaDesempenoController.RestEmp>())
                        {

                            //RETOALIMENTADO EMP
                            if ((RetroEmp != "" && RetroEmp != null))
                            {
                                if (item.RetroalimentacionEmp != RetroEMP2)
                                {
                                    ListEmp2.Remove(item);
                                }
                            }
                            //ESTADO AUTOEVALUACIÓN
                            if (item.EstadoA != TipoPresentacionA && TipoPresentacionA != 0)
                            {
                                ListEmp2.Remove(item);
                            }
                        }
                    }
                
                }
                var areas = db.Empleados.Where(s => s.Activo == "SI" && (s.AreaDescripcion != null & s.AreaDescripcion != "")).Select(x => new { x.AreaDescripcion }).GroupBy(b => b.AreaDescripcion).ToList();
                List<string> areas2 = new List<string>();
                foreach (var item in areas)
                {
                    var key = item.Key;
                    areas2.Add(key);


                }
                ViewBag.Areas = areas2;
                ViewBag.Empleados = db.Empleados.Where(f => f.Activo != "NO").ToList();
                ViewBag.TipoPrueba = db2.Tipo_evaluacion.Where(x => x.codigo == 1).ToList();
                ViewBag.Sociedad = db.Sociedad.ToList();
                ViewBag.LinkEvadesh = db.Configuraciones.Where(x => x.Parametro == "LINKEVADESH").FirstOrDefault().Valor;
                ViewBag.Estados = db2.EstadosEvaluacionEncabezado.Where(x => x.Codigo != 0 & x.Codigo !=6 && x.Descripcion != "").ToList();
                ViewBag.PeriodosParametros = db2.Periodos_Evaluacion_Desempeño.Where(x=>x.Año <= HoYConstante.Year).ToList();
                return View(ListEmp2);

            }


        }

        public ActionResult GenerarPeriodo(string FechaI, string FechaF ,string Año) 
        {
            try
            {
                List<string> funciones = Acceso.Validar(Session["Empleado"]);
                if (Acceso.EsAnonimo)
                {
                    return RedirectToAction("Login", "Login");
                }
                else if (!Acceso.EsAnonimo && !funciones.Contains("GenerarPeriodoEvaDes"))
                {
                    Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                    return RedirectToAction("Index", "Login");
                }

                if (FechaI=="" || FechaF=="" || Año == "")
            {
                throw new Exception("Hacen falta datos");
            }
                int Año2 = Convert.ToInt32(Año);
            if (FechaI != null || FechaF != null || Año2!=0) 
            {
                DateTime F1 = Convert.ToDateTime(FechaI);
                DateTime F2 = Convert.ToDateTime(FechaF);
                    Periodos_Evaluacion_Desempeño Periodo = db2.Periodos_Evaluacion_Desempeño.Where(x => x.Año == Año2).FirstOrDefault();
                    if (Periodo == null)
                    {

                        Periodos_Evaluacion_Desempeño newPeriodo = new Periodos_Evaluacion_Desempeño();
                        newPeriodo.Año = Año2;
                        newPeriodo.FechaInicial = F1;
                        newPeriodo.FechaFinal = F2;
                        db2.Periodos_Evaluacion_Desempeño.Add(newPeriodo);
                        db2.SaveChanges();
                        Session["message2"] = "Guardado Exitoso";
                    }
                    else
                    {
                        throw new Exception("Ya exite un Periodo para este Año");

                    }
                }
            }catch (Exception ex) 
            {
              
                 
                    Session["message"] = "Error al Guardar: " + ex;
                
            }


            return View("");
        }
        
        public ActionResult VerSeguimientosJF(string Empleado, int? TipoPrueba,string cumplimiento) 
        {
            try 
            {
                List<string> funciones = Acceso.Validar(Session["Empleado"]);
                if (Acceso.EsAnonimo)
                {
                    return RedirectToAction("Login", "Login");
                }
                else if (!Acceso.EsAnonimo && !funciones.Contains("VerSeguimientosJF"))
                {
                    Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                    return RedirectToAction("Index", "Login");
                }
                bool Cump = false;
                if(cumplimiento == "Si cumplió") 
                {
                    Cump = true;
                }
                if (cumplimiento == "No cumplió")
                {
                    Cump = false;

                }
                List<seguimientos> ListSeguimientos = new List<seguimientos>();
                List<seguimientos> ListSeguimientos2 = new List<seguimientos>();
                using (var db = new AutogestionContext())
                {
                    List<seguimientos> Seguimientos = db2.Seguimientos.ToList();
                    int Usuario = 0;
                    var usuariolog = String.Format("{0}", Session["Empleado"]);
                    Int32.TryParse(usuariolog, out Usuario);
                    var Jefe = db.Empleados.FirstOrDefault(e => e.Id == Usuario);
                    List<Empleado> ListEmps = db.Empleados.Where(f => f.Activo != "NO" && f.Jefe == Jefe.NroEmpleado).ToList();
                    if (Empleado ==null || Empleado == "") 
                    {
                        if (TipoPrueba==null) 
                        {                            
                            foreach (var item in ListEmps)
                            {
                                int NmrEmp = Convert.ToInt32(item.NroEmpleado);
                                //var emp = db.Empleados.Where(x => x.NroEmpleado == item.NroEmpleado).Select(s => s.Nombres);
                                var ListsegEmp = Seguimientos.Where(x => x.codigoempleado == NmrEmp).ToList();
                                foreach (var seg in ListsegEmp) 
                                {
                                    seg.NombreEmp = item.Nombres;
                                }
                                ListSeguimientos2.AddRange(ListsegEmp);

                            }
                        }
                        else 
                        {
                            ListSeguimientos = Seguimientos.Where(x =>x.tipoevaluacion== TipoPrueba).ToList();
                            foreach (var item in ListEmps) 
                            {
                                int NmrEmp = Convert.ToInt32(item.NroEmpleado);
                                //var emp = db.Empleados.Where(x=>x.NroEmpleado == item.NroEmpleado).Select(s=>s.Nombres);
                                var ListsegEmp = ListSeguimientos.Where(x => x.codigoempleado == NmrEmp && x.tipoevaluacion==TipoPrueba).ToList();
                                foreach (var seg in ListsegEmp)
                                {
                                    seg.NombreEmp = item.Nombres;
                                }
                                ListSeguimientos2.AddRange(ListsegEmp);
                                   
                            }
                        }
                    }
                    else 
                    {
                        if (TipoPrueba == null)
                        {
                            var Emp = db.Empleados.Where(x => x.NroEmpleado == Empleado).FirstOrDefault();
                                int NmrEmp = Convert.ToInt32(Emp.NroEmpleado);
                                var ListsegEmp = Seguimientos.Where(x => x.codigoempleado == NmrEmp).ToList();
                                foreach (var seg in ListsegEmp)
                                {
                                    seg.NombreEmp = Emp.Nombres;
                                }
                            ListSeguimientos2.AddRange(ListsegEmp);

                            
                        }
                        else
                        {
                            var Emp = db.Empleados.Where(x => x.NroEmpleado == Empleado).FirstOrDefault();
                            ListSeguimientos = Seguimientos.Where(x => x.tipoevaluacion == TipoPrueba).ToList();

                                int NmrEmp = Convert.ToInt32(Emp.NroEmpleado);
                                var ListsegEmp = ListSeguimientos.Where(x => x.codigoempleado == NmrEmp && x.tipoevaluacion == TipoPrueba).ToList();
                                foreach (var seg in ListsegEmp)
                                {
                                    seg.NombreEmp = Emp.Nombres;
                                }
                                ListSeguimientos2.AddRange(ListsegEmp);

                            
                        }
                    }

                    if (cumplimiento != "" && cumplimiento != null)
                    {
                        foreach (seguimientos item in ListSeguimientos2.Reverse<seguimientos>())
                        {

                            if (item.cumplimiento != Cump)
                            {
                                ListSeguimientos2.Remove(item);
                            }
                        }
                    }
                    ViewBag.TipoPrueba = db2.Tipo_evaluacion.ToList();
                    ViewBag.Empleados = ListEmps;
                }
                return View(ListSeguimientos2);
            }

            catch (Exception ex)
            {
                return View();
            }

            
        }
        public ActionResult VerSeguimientosGH(string Empleado, int? TipoPrueba)
        {
            try
            {
                List<string> funciones = Acceso.Validar(Session["Empleado"]);
                if (Acceso.EsAnonimo)
                {
                    return RedirectToAction("Login", "Login");
                }
                else if (!Acceso.EsAnonimo && !funciones.Contains("VerSeguimientosGH"))
                {
                    Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                    return RedirectToAction("Index", "Login");
                }
                List<seguimientos> ListSeguimientos = new List<seguimientos>();
                List<seguimientos> ListSeguimientos2 = new List<seguimientos>();
                using (var db = new AutogestionContext())
                {
                    List<seguimientos> Seguimientos = db2.Seguimientos.Where(x => x.cumplimiento == null).ToList();
                    List<Empleado> ListEmps = db.Empleados.Where(f => f.Activo != "NO").ToList();
                    if (Empleado == null || Empleado == "")
                    {
                        if (TipoPrueba == null)
                        {
                            foreach (var item in ListEmps)
                            {
                                int NmrEmp = Convert.ToInt32(item.NroEmpleado);
                                //var emp = db.Empleados.Where(x => x.NroEmpleado == item.NroEmpleado).Select(s => s.Nombres);
                                var ListsegEmp = Seguimientos.Where(x => x.codigoempleado == NmrEmp).ToList();
                                foreach (var seg in ListsegEmp)
                                {
                                    seg.NombreEmp = item.Nombres;
                                }
                                ListSeguimientos2.AddRange(ListsegEmp);

                            }
                        }
                        else
                        {
                            ListSeguimientos = Seguimientos.Where(x => x.tipoevaluacion == TipoPrueba).ToList();
                            foreach (var item in ListEmps)
                            {
                                int NmrEmp = Convert.ToInt32(item.NroEmpleado);
                                //var emp = db.Empleados.Where(x=>x.NroEmpleado == item.NroEmpleado).Select(s=>s.Nombres);
                                var ListsegEmp = ListSeguimientos.Where(x => x.codigoempleado == NmrEmp && x.tipoevaluacion == TipoPrueba).ToList();
                                foreach (var seg in ListsegEmp)
                                {
                                    seg.NombreEmp = item.Nombres;
                                }
                                ListSeguimientos2.AddRange(ListsegEmp);

                            }
                        }
                    }
                    else
                    {
                        if (TipoPrueba == null)
                        {
                            var Emp = db.Empleados.Where(x => x.NroEmpleado == Empleado).FirstOrDefault();
                            int NmrEmp = Convert.ToInt32(Emp.NroEmpleado);
                            var ListsegEmp = Seguimientos.Where(x => x.codigoempleado == NmrEmp).ToList();
                            foreach (var seg in ListsegEmp)
                            {
                                seg.NombreEmp = Emp.Nombres;
                            }
                            ListSeguimientos2.AddRange(ListsegEmp);


                        }
                        else
                        {
                            var Emp = db.Empleados.Where(x => x.NroEmpleado == Empleado).FirstOrDefault();
                            ListSeguimientos = Seguimientos.Where(x => x.tipoevaluacion == TipoPrueba).ToList();

                            int NmrEmp = Convert.ToInt32(Emp.NroEmpleado);
                            var ListsegEmp = ListSeguimientos.Where(x => x.codigoempleado == NmrEmp && x.tipoevaluacion == TipoPrueba).ToList();
                            foreach (var seg in ListsegEmp)
                            {
                                seg.NombreEmp = Emp.Nombres;
                            }
                            ListSeguimientos2.AddRange(ListsegEmp);


                        }
                    }


                    ViewBag.TipoPrueba = db2.Tipo_evaluacion.ToList();
                    ViewBag.Empleados = ListEmps;
                }
                return View(ListSeguimientos2);
            }

            catch (Exception ex)
            {
                return View();
            }


        }

        public ActionResult AñadirComentario(int? codigo)
        {
        
            using (var db = new AutogestionContext())
            {
                var seguimiento = db2.Seguimientos.Where(x => x.codigo == codigo).FirstOrDefault();
                return PartialView(seguimiento);
            }

        }
        [HttpPost]
        public ActionResult AñadirComentario(seguimientos seguimiento, string Fecha1)
        {
            try
            {

                DateTime Hoy = DateTime.Today;
               
                if (Fecha1 == "")
                {
               
                    Session["Message"] += "Debe digitar la fecha para el proximo seguimiento  / ";
                }
                else 
                {
                    seguimiento.proximoseguimiento = Convert.ToDateTime(Fecha1);
                }
               
                var seg = db2.Seguimientos.Where(x => x.codigo == seguimiento.codigo).FirstOrDefault();
                if (seguimiento.comentario!= seg.comentario|| seguimiento.proximoseguimiento!=seg.proximoseguimiento || seguimiento.cumplimiento!=seg.cumplimiento)
                {
                    if(seguimiento.proximoseguimiento <= Hoy)
                    {

                        Session["Message"]+= "Debe digitar una fecha mayor a la actual  / ";
                    }

                    if (seguimiento.comentario == null)
                    {
                        Session["Message"] += "Debe digitar un comentario  / ";
                    }
                    if (seguimiento.cumplimiento == null)
                    {
                        Session["Message"] += "Debe selecionar si cumplio o no el seguimiento / ";
                    }
                    string respuesta = string.Format("{0}", Session["message"]); ;
                    if (!string.IsNullOrWhiteSpace(respuesta))
                    {
                        return RedirectToAction("VerSeguimientosJF");
                    }
                    else 
                    {
                    seg.proximoseguimiento = seguimiento.proximoseguimiento;
                    seg.comentario = seguimiento.comentario;
                    seg.cumplimiento = seguimiento.cumplimiento;


                    db2.SaveChanges();
                    
                    }
                    
                }
          
                    return RedirectToAction("VerSeguimientosJF");
                
            }
            catch
            {
                return View();
            }
        }
    }

}
