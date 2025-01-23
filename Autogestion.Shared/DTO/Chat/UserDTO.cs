using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autogestion.Shared.DTO.Chat
{
    public class UserDTO
    {
        public string Name { get; set; } = null;
        public string ConnectionId { get; set; } = null;
        public string? Groups { get; set; }
    }
}
