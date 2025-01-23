using Adm_AutoGestion.DTO;
using Autogestion.Shared.DTO.Cursos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Adm_AutoGestion.Services
{
    public class EduFoscalRepository
    {

        public EduFoscalRepository() {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public async Task<List<CursoModdleDTO>> ListarCursos() {
            try
            {
          var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://edufoscal.com/talentofoscal/webservice/rest/server.php?wsfunction=core_course_get_courses&moodlewsrestformat=json&wstoken=685a460d9a1e98f914d148a0de4fd487");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            if(response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<List<CursoModdleDTO>>();
               
            }
                return null;
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        public async Task<List<UserMoodleDTO>> GetIdxUserName(string Identificación)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"https://edufoscal.com/talentofoscal/webservice/rest/server.php?wsfunction=core_user_get_users_by_field&moodlewsrestformat=json&wstoken=685a460d9a1e98f914d148a0de4fd487&field=username&values[0]={Identificación}");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<List<UserMoodleDTO>>();

                }
                return null;
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        public async Task<List<CursoxUsuarioDTO>> GetCursoxUserName(int idusuario)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"https://edufoscal.com/talentofoscal/webservice/rest/server.php?wsfunction=core_enrol_get_users_courses&moodlewsrestformat=json&wstoken=685a460d9a1e98f914d148a0de4fd487&userid={idusuario}");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<List<CursoxUsuarioDTO>>();

                }
                return null;
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        public async Task<List<CursoModdleDTO>> GetCursoxCursoId(int courseId)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"https://edufoscal.com/talentofoscal/webservice/rest/server.php?wsfunction=core_course_get_courses&moodlewsrestformat=json&wstoken=685a460d9a1e98f914d148a0de4fd487&options[ids][0]={courseId}");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<List<CursoModdleDTO>>();

                }
                return null;
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        public class CursosResponse
        {
            public List<CursoModdleDTO> Courses { get; set; }
        }

        public async Task<List<CursoModdleDTO>> ListarCursosAF()
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://edufoscal.com/talentofoscal/webservice/rest/server.php?wsfunction=core_course_get_courses_by_field&moodlewsrestformat=json&wstoken=685a460d9a1e98f914d148a0de4fd487&field=category&value=35");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var cursosResponse = JsonConvert.DeserializeObject<CursosResponse>(responseData);
                    return cursosResponse.Courses;

                }
                return null;
            }
            catch (Exception ex)
            {

                return null;
            }

        }
        //private static readonly HttpClientHandler handler = new HttpClientHandler
        //{
        //    UseCookies = true,
        //    CookieContainer = new CookieContainer()
        //};
        //private static readonly HttpClient client = new HttpClient(handler);

        //public async Task<string> LoginEduFoscal(string targetUrl)
        //{
        //    var username = "certificadosfoscal";
        //    var password = "Certificados2024";
        //    var loginToken = "f5D0CyIrIFARorr68qFj97DebfzoLSxw";
        //    var loginUrl = "https://edufoscal.com/talentofoscal/login/index.php";

        //    var content = new MultipartFormDataContent
        //    {
        //        { new StringContent(targetUrl), "anchor" },
        //        { new StringContent(loginToken), "logintoken" },
        //        { new StringContent(username), "username" },
        //        { new StringContent(password), "password" },
        //        { new StringContent("1"), "rememberusername" } // si deseas recordar el nombre de usuario
        //    };

        //    var response = await client.PostAsync(loginUrl, content);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var targetResponse = await client.GetAsync(targetUrl);
        //        var responseContent = await targetResponse.Content.ReadAsStringAsync();
        //        return "";
        //    }

        //    throw new Exception("Error en el inicio de sesión");
        //}
        
    }
}