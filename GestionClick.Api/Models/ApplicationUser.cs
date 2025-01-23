using Autogestion.Shared.DTO.Chat;
using Autogestion.Shared.DTO.Empleado;
namespace GestionClick.Api.Models
{
    public class ApplicationUser :EmpleadoDTO
    {
        public virtual ICollection<ChatMessage> ChatMessagesFromUsers { get; set; }
        public virtual ICollection<ChatMessage> ChatMessagesToUsers { get; set; }
        public ApplicationUser()
        {
            ChatMessagesFromUsers = new HashSet<ChatMessage>();
            ChatMessagesToUsers = new HashSet<ChatMessage>();
        }
    }
}
