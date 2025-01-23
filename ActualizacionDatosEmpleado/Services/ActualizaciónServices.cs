using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Autogestion.Shared.DTO.Empleado;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace ActualizacionDatosEmpleado.Services
{
    public class ActualizaciónServices
    {

        private readonly HttpClient _httpClient;
        //private readonly IConfiguration _config;
        public ActualizaciónServices(HttpClient httpclient)
        {

            _httpClient = httpclient;
            
        }

        public async Task<bool> EjecutarProceso()
        {

            try
            {
                _httpClient.Timeout = TimeSpan.FromMinutes(60);

                await ActualizarEmpleadoMasivo();
                await ActualizarEmpleado();
                await ActualizarEstructura();
                
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> ActualizarEmpleado() {

            try
            {
                Log.Information("Inicia ejecución proceso Actualización datos empleado");
               ;
                var ListEmpleados = await _httpClient.GetFromJsonAsync<List<EmpleadoDTO>>($"api/Trabajador/GetAll");

              //  var ListEmpleados = await _httpClient.GetFromJsonAsync<List<EmpleadoDTO>>($"api/Trabajador/GetxCodigo/50002730");
                Log.Information($"{ListEmpleados.Count()} registros encontrados");

                foreach (var item in ListEmpleados ) { 
               var result = await _httpClient.PostAsJsonAsync($"api/Trabajador/ActualizacionEmpleadosIndividual",item);

                if (result.IsSuccessStatusCode)
                {

                    var respuesta = await result.Content.ReadFromJsonAsync<bool>();

                    if (respuesta) Log.Information($"Termina Proceso actualización datos empleado {item.NroEmpleado}");
                    else Log.Error($"Error al ejecutar metodo actualizarempleado {item.NroEmpleado}");


                }
                else {
                    Log.Error($"Error al ejecutar metodo actualizarempleado {item.NroEmpleado}" );
                }
                }

      
   
             
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Error al ejecutar metodo actualizarempleado " + ex.Message);
                throw;
            }
      
        }

        public async Task<bool> ActualizarEstructura()
        {

            try
            {
                Log.Information("Inicia ejecuación proceso Actualización Estructura");
                //_httpClient.Timeout = TimeSpan.FromMinutes(60);
                var result = await _httpClient.PostAsync($"api/Trabajador/ActualizacionEstructura", null);

                if (result.IsSuccessStatusCode)
                {

                    var respuesta = await result.Content.ReadFromJsonAsync<bool>();

                    if (respuesta) Log.Information("Termina Proceso actualización Estructura");
                    else Log.Error("Error al ejecutar metodo actualizareEstructura ");


                }
                else
                {
                    Log.Error("Error al ejecutar metodo ActualizarEstructura: "+ result.RequestMessage);
                }


                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Error al ejecutar metodo ActualizarEstructura " + ex.Message);
                throw;
            }

        }


        public async Task<bool> ActualizarEmpleadoMasivo()
        {

            try
            {
                Log.Information("Inicia ejecución proceso Actualización De Empleados Masiva API4");
                //_httpClient.Timeout = TimeSpan.FromMinutes(60);
                var result = await _httpClient.GetFromJsonAsync<string>($"api/EvaDesempeño/ActualizarDatosEmpleado");

                if (result == "Proceso Exitoso")
                {

                    //var respuesta = await result.Content.ReadFromJsonAsync<bool>();

                    Log.Information("Termina Proceso actualización de Empleados Masiva API4");
                    //else Log.Error("Error al ejecutar metodo ActualizarEmpleadoMasivo ");


                }
                else
                {
                    Log.Error("Error al ejecutar metodo ActualizarEmpleadoMasivo API4: " + result);
                }


                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Error al ejecutar metodo ActualizarEmpleadoMasivo  API4" + ex.Message);
                throw;
            }

        }
    }
}