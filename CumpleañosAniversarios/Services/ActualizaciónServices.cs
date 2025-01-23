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

namespace CumpleañosAniversarios.Services
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
                await EnviarCorreoCumpleaños();
                await EnviarCorreoAniversario();

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> EnviarCorreoCumpleaños() {

            try
            {
                Log.Information("Inicia ejecuación proceso notificación cumpleaños");

                var ListEmpleados = await _httpClient.GetFromJsonAsync<List<EmpleadoDTO>>($"api/cumpleanos");

              //  var ListEmpleados = await _httpClient.GetFromJsonAsync<List<EmpleadoDTO>>($"api/Trabajador/GetxCodigo/50002730");
                Log.Information($"{ListEmpleados.Count()} registros encontrados");

                foreach (var item in ListEmpleados ) {
                    if (item.Correo != null || item.Correo != "")
                    {
                        var result = await _httpClient.PostAsJsonAsync($"api/enviarcorreocumpleanos", item);

                        if (result.IsSuccessStatusCode)
                        {

                            var respuesta = await result.Content.ReadFromJsonAsync<bool>();

                            if (respuesta) Log.Information($"Envia correo cumpleaños a trabajador {item.NroEmpleado}");
                            else Log.Error($"Error al enviar correo cumpleaños {item.NroEmpleado}  {item.Nombres}  ");


                        }
                        else
                        {
                            Log.Error($"Error al ejecutar metodo enviar correo cumpleaños {item.NroEmpleado} {item.Nombres}");
                        }

                    }
                    else {
                        Log.Error($"trabajador sin correo {item.NroEmpleado} {item.Nombres}");
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




        public async Task<bool> EnviarCorreoAniversario()
        {

            try
            {
                Log.Information("Inicia ejecuación proceso notificación aniversario");

                var ListEmpleados = await _httpClient.GetFromJsonAsync<List<EmpleadoDTO>>($"api/aniversario");

                //  var ListEmpleados = await _httpClient.GetFromJsonAsync<List<EmpleadoDTO>>($"api/Trabajador/GetxCodigo/50002730");
                Log.Information($"{ListEmpleados.Count()} registros encontrados");

                foreach (var item in ListEmpleados)
                {
                    if (item.Correo != null || item.Correo != "")
                    {
                        var result = await _httpClient.PostAsJsonAsync($"api/enviarcorreoaniversario", item);

                        if (result.IsSuccessStatusCode)
                        {

                            var respuesta = await result.Content.ReadFromJsonAsync<bool>();

                            if (respuesta) Log.Information($"Envia correo anviersario a trabajador {item.NroEmpleado}");
                            else Log.Error($"Error al enviar correo anviersario {item.NroEmpleado}  {item.Nombres}  ");


                        }
                        else
                        {
                            Log.Error($"Error al ejecutar metodo enviar correo anviersario {item.NroEmpleado} {item.Nombres}");
                        }

                    }
                    else
                    {
                        Log.Error($"trabajador sin correo {item.NroEmpleado} {item.Nombres}");
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



    }
}