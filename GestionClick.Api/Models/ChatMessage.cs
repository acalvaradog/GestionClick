
using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace GestionClick.Api.Models
{
    public class ChatMessage
    {
        public long Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public int FromEmpleadoId { get; set; } // Propiedad para la clave foránea de FromUserId
        public int ToEmpleadoId { get; set; }   // Propiedad para la clave foránea de ToUserId

        public Empleadoes? FromEmpleado { get; set; } // Propiedad de navegación
        public Empleadoes? ToEmpleado { get; set; }   // Propiedad de navegación
    }
}