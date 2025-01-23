using Autogestion.Shared.DTO.Empleado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Chat
{
    public class ApplicationUser : EmpleadoDTO
    {
        public virtual ICollection<MessageDTO> ChatMessagesFromUsers { get; set; }
        public virtual ICollection<MessageDTO> ChatMessagesToUsers { get; set; }
        public ApplicationUser()
        {
            ChatMessagesFromUsers = new HashSet<MessageDTO>();
            ChatMessagesToUsers = new HashSet<MessageDTO>();
        }
    }
}
