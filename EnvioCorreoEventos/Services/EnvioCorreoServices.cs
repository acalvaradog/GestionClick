using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace EnvioCorreoEventos.Services
{
    public class EnvioCorreoServices
    {

        private readonly HttpClient _httpClient;
        //private readonly IConfiguration _config;
        public EnvioCorreoServices(HttpClient httpclient)
        {

            _httpClient = httpclient;
            
        }

        public async Task<bool> EnviarCorreos()
        {
            try
            {
                Log.Information("Buscando Eventos Cerrados");
                var EventosPendientes = await _httpClient.GetFromJsonAsync<List<int>>($"api/Eventos/EventosEncuestaPendiente");
                var eventosId = new List<int>();
                foreach (var i in EventosPendientes)
                {
                    eventosId.Add(i);
                }
                Log.Information("Se encontraron {Cant} registros", eventosId.Count);

                foreach (var i in eventosId)
                {
                    var a = i.ToString();
                    var id = Convert.ToInt32(a);
                    var envio = await _httpClient.GetFromJsonAsync<string>($"api/Eventos/EnvioCorreoEncuestas/{id}");

                    Log.Information("Se envió correos del evento id: {id}", a);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Hubo un error: "+ ex);
                throw;
            }
        }
    }
}