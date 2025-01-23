using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;


namespace EvaDesempeñoActualizacion.Services
{
    internal class ActualizacionDiaria
    {
        private readonly HttpClient _httpClient;
        //private readonly IConfiguration _config;
        public ActualizacionDiaria(HttpClient httpclient)
        {

            _httpClient = httpclient;

        }
        public async Task<bool> ActualizacionDiariaM()
        {

            try
            {
                await ActualizacionEmpleados();
                await ActualizarEvaluaciones();
                //await GenerarEvaluacionesPeriodicas();
                await ActualizaciónEmpleadoxSociedad();
                await GenerarRolesEvadesh();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> ActualizacionEmpleados()
        {

            try
            {
                Log.Information("Actualización Empleados Proyecto Evaluación De Desempeño");
                _httpClient.Timeout = TimeSpan.FromMinutes(90);
                var Respuesta = await _httpClient.GetFromJsonAsync<string>($"api/EvaDesempeño/ActualizarEmpsEvadesempeño");
                if (Respuesta == "Proceso Exitoso")
                {
                    Log.Information($"Proceso de Actualización empleados de evaluación de desempeño terminado con exito");
                    return true;
                }
                else
                {
                    Log.Error($"Error en el proceso de Actualización empleados de evaluación de desempeño. Fecha: " + DateTime.Now + "Erro: "+ Respuesta);
                    return false;
                }
                
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el proceso de Actualización empleados de evaluación de desempeño. Fecha: " + DateTime.Now + "Erro: " + ex.Message);
                return false;
            }
        }
        public async Task<bool> GenerarEvaluacionesPeriodicas()
        {

            try
            {
                Log.Information("Creación Evaluaciones Periódicas Empleados");
                //_httpClient.Timeout = TimeSpan.FromMinutes(40);
                var Respuesta = await _httpClient.GetFromJsonAsync<string>($"api/EvaDesempeño/HabilitadoParaEvaP");
                if (Respuesta == "No hay Evaluaciones pendientes" || Respuesta == "Activacion Exitosa") 
                {
                    Log.Information($"Proceso de Evaluaciones periódicas terminado con exito");
                    return true;
                }
                else 
                {
                    Log.Error($"Error en el proceso al generar las Evaluaciones periodicas: " + Respuesta);
                    return false;
                }
                
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el proceso al generar las Evaluaciones periodicas: " + ex.Message);
                return false;
            }
        }
        public async Task<bool> ActualizarEvaluaciones()
        {

            try
            {
                Log.Information("Actualización de Evaluadores de la Evaluación");
                //_httpClient.Timeout = TimeSpan.FromMinutes(30);
                var Respuesta = await _httpClient.GetFromJsonAsync<string>($"api/EvaDesempeño/ActualizacionEvaluacion");
                if (Respuesta == "true")
                {
                    Log.Information($"Proceso de Actualizacion de Evaluacion / Evaluadores Terminado exitosamente");
                    return true;
                }
                else
                {
                    Log.Error($"Error en la Actualizacion de Evaluacion / Evaluadores. Error: " + Respuesta);
                    return false;
                }
                
            }
            catch (Exception ex)
            {
                Log.Error($"Error en la Actualizacion de Evaluacion / Evaluadores. Error: " + ex.Message);
                return false;
            }
        }       
        public async Task<bool> GenerarRolesEvadesh()
        {

            try
            {
                Log.Information("Actualización Empleados Proyecto Evaluación De Desempeño");
                //_httpClient.Timeout = TimeSpan.FromMinutes(20);
                var Respuesta = await _httpClient.GetFromJsonAsync<string>($"api/EvaDesempeño/CreacionDeRoles");
                if (Respuesta == "Proceso Exitoso")
                {
                    Log.Information($"Proceso de Generar roles de Jefe Termino de forma exitosa");
                    return true;
                }
                else
                {
                    Log.Error($"Error al Generar roles de Jefe: " + Respuesta);
                    return false;
                }
               
            }
            catch (Exception ex)
            {
                Log.Error($"Error al Generar roles de Jefe: " + ex.Message);
                return false;
            }
        }
        public async Task<bool> ActualizaciónEmpleadoxSociedad()
        {

            try
            {
                Log.Information("Actualización Empleados x Sociedad Evadesh");
                //_httpClient.Timeout = TimeSpan.FromMinutes(40);
                var Respuesta = await _httpClient.GetFromJsonAsync<string>($"api/EvaDesempeño/ActualizacionEmpleadoxSociedad");
                if (Respuesta == "Proceso Exitoso")
                {
                    Log.Information($"Proceso de Actualización de Sociedades por Empleado Exitoso");
                    return true;
                }
                else
                {
                    Log.Error($"Error al Actualizar las Sociedades: " + Respuesta);
                    return false;
                }
               
            }
            catch (Exception ex)
            {
                Log.Error($"Error al Actualizar las Sociedades: " + ex.Message);
                return false;
            }
        }
    }
}
