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
    internal class EvaPeriodicaA
    {
        private readonly HttpClient _httpClient;
        //private readonly IConfiguration _config;
        public EvaPeriodicaA(HttpClient httpclient)
        {

            _httpClient = httpclient;

        }
        public async Task<bool> CorreoMensualEvaDPeriodica()
        {

            try
            {
                Log.Information("Correo Mensual Jefe de la Evaluaciones Pendientes");
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
