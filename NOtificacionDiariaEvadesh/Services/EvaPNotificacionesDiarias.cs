using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace NOtificacionDiariaEvadesh.Services
{
    internal class EvaPNotificacionesDiarias
    {
        private readonly HttpClient _httpClient;
        //private readonly IConfiguration _config;
        public EvaPNotificacionesDiarias(HttpClient httpclient)
        {

            _httpClient = httpclient;

        }
        public async Task<bool> Principal()
        {

            try
            {
                _httpClient.Timeout = TimeSpan.FromMinutes(30);
                await CorreoDiarioSeguimientos();
                //await CorreoDiarioEmpleadoEvaPeriodica();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> CorreoDiarioSeguimientos()
        {

            try
            {
                Log.Information("Inicia Proceso de Notificaciónes Seguimientos");
                var Respuesta = await _httpClient.GetFromJsonAsync<string>($"api/EvaDesempeño/NotificacionEmailSeguimientosDiarios");

                if (Respuesta == "true")
                {
                    Log.Information($"Termina Proceso de notificación de Seguimientos");

                }
                else
                {
                    Log.Error($"Error en la Notificación de seguimientos: " + Respuesta+ " // " + DateTime.Now);
                }


                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> CorreoDiarioEmpleadoEvaPeriodica()
        {

            try
            {
                Log.Information("Correo Diario Evaluación Generada del Empleado");
                //_httpClient.Timeout = TimeSpan.FromMinutes(30);
                var EventosPendientes = await _httpClient.GetFromJsonAsync<string>($"api/EvaDesempeño/NotificacionEmailPeriodica");
                var eventosId = new List<int>();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
