using Adm_AutoGestion.Models;
using System;

public class ChatMessage
{ 

    public long Id { get; set; }
    public string Message { get; set; }
    public DateTime CreatedDate { get; set; }
    public int FromEmpleadoId {  get; set; }
    public int ToEmpleadoId { get; set; }

}