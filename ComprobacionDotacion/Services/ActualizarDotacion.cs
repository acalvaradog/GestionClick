using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ComprobacionDotacion.Services
{
    internal class ActualizarDotacion
    {
        private readonly HttpClient _httpClient;
        //private readonly IConfiguration _config;
        public ActualizarDotacion(HttpClient httpclient)
        {

            _httpClient = httpclient;

        }
        public async Task<bool> ActualizacionDotacion()
        {
            try
            {
                Log.Information("Ejecutando comprobación de empleados inactivos");
                var EmpleadosInactivos = await _httpClient.GetFromJsonAsync<bool>($"api/Dotacion/LimpiezaEmpleadoNoActivo");
                if (EmpleadosInactivos == true)
                {
                    Log.Information("Comprobación terminada correctamente");
                }
                Log.Information("Ejecutando comprobación de dotación");
                var Respuesta = await _httpClient.GetFromJsonAsync<bool>($"api/Dotacion/ActualizarDotacion");
                if (Respuesta == true)
                {
                    Log.Information("Comprobación terminada correctamente");
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Error al ejecutar la comprobación de dotación: "+ ex);
                throw;
            }
        }
    }
}
