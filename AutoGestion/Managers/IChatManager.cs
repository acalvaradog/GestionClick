using Autogestion.Shared.DTO.Chat;
using Autogestion.Shared.DTO.Empleado;
using System.Collections.Generic;
using System.Threading.Tasks;

 namespace AutoGestion.Managers

{
    public interface IChatManager
    {
        Task<List<EmpleadoDTO>> GetUsersAsync();
        Task SaveMessageAsync(MessageDTO message);
        Task<List<MessageDTO>> GetConversationAsync(int contactId,int userid);
        Task<EmpleadoDTO> GetUserDetailsAsync(int userId);
        Task<List<EmpleadoDTO>> GetUsersChatAsync(int userId);
    }
}
