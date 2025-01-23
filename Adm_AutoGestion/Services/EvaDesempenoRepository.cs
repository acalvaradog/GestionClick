using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.IO;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using QRCoder;

namespace Adm_AutoGestion.Services
{
    public class EvaDesempenoRepository
    {
        private AutogestionContext _ContextAutogestion;
        private EvaDesempenoContext _ContextEvaluacion;
       
        public  EvaDesempenoRepository(AutogestionContext ContextAutogestion,EvaDesempenoContext ContextEvaluacion){
                _ContextAutogestion = ContextAutogestion;
                _ContextEvaluacion = ContextEvaluacion;
            }
        public string CrearPeriodo( DateTime Fecha1, DateTime Fecha2, int Año) 
        {
            var respuesta = "";
            try
            {
                
                Periodos_Evaluacion_Desempeño Periodo  = _ContextEvaluacion.Periodos_Evaluacion_Desempeño.Where(x => x.Año == Año).FirstOrDefault();
                if (Periodo == null) 
                {

                    Periodos_Evaluacion_Desempeño newPeriodo = new Periodos_Evaluacion_Desempeño();
                    newPeriodo.Año = Año;
                    newPeriodo.FechaInicial = Fecha1;
                    newPeriodo.FechaFinal = Fecha2;
                    _ContextEvaluacion.Periodos_Evaluacion_Desempeño.Add(newPeriodo);
                    _ContextEvaluacion.SaveChanges();
                    return respuesta = "Guardado exitoso";
                }
                else
                {
                    throw new Exception("Ya exite un Periodo para este Año");
                }
                
                
                
            }
            catch (Exception ex)
            {
                return respuesta + "" +ex;
            }

        
        }

        
       
    }
}