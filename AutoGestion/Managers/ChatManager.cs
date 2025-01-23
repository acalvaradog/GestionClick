using Autogestion.Shared.DTO.Chat;
using Autogestion.Shared.DTO.Empleado;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AutoGestion.Managers
{
    public class ChatManager : IChatManager
    {
        private readonly HttpClient _httpClient;

        public ChatManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
           
        }
        public async Task<List<MessageDTO>> GetConversationAsync(int contactId,int userid)
        {
     
            return await _httpClient.GetFromJsonAsync<List<MessageDTO>>($"api/chat/{contactId}/{userid}");
        }
        public async Task<EmpleadoDTO> GetUserDetailsAsync(int userId)
        {
  
            return await _httpClient.GetFromJsonAsync<EmpleadoDTO>($"api/chat/users/{userId}");
        }
        public async Task<List<EmpleadoDTO>> GetUsersAsync()
        {
        
            var data = await _httpClient.GetFromJsonAsync<List<EmpleadoDTO>>("api/chat/users");
            return data;
        }

        public async Task<List<EmpleadoDTO>> GetUsersChatAsync(int userId)
        {

            var data = await _httpClient.GetFromJsonAsync<List<EmpleadoDTO>>($"api/chat/userschats/{userId}");
            return data;
        }


        public async Task SaveMessageAsync(MessageDTO message)
        {
          
            await _httpClient.PostAsJsonAsync("api/chat", message);
        }
    }
}
