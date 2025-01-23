using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace EncuestaViaje.Services
{
    public class EnvioEncuestaViaje
    {
        private readonly HttpClient _httpClient;
        //private readonly IConfiguration _config;
        public EnvioEncuestaViaje(HttpClient httpclient)
        {

            _httpClient = httpclient;

        }
        public async Task<bool> EnviarEncuestaViaje()
        {
            try
            {
                Log.Information("Ejecutando comprobación de envio encuestas");
                var Respuesta = await _httpClient.GetFromJsonAsync<bool>($"api/Viaticos/ConsultaFechaFin");
                if (Respuesta == true)
                {
                    Log.Information("Comprobación terminada correctamente");
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Error al ejecutar la comprobación: " + ex);
                throw;
            }
        }
    }
}
