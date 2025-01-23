using GestionClick.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;

namespace GestionClick.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize]
    public class ChatController : ControllerBase
    {
     
        private readonly ApplicationDbContext _context;

        public ChatController( ApplicationDbContext context)
        {
           
            _context = context;
        }

        [HttpGet("{contactId}/{user}")]
        public async Task<IActionResult> GetConversationAsync(int contactId,int user)
        {
            try
            {
                var messages = await _context.ChatMessages
        .Where(h => (h.FromEmpleadoId == contactId && h.ToEmpleadoId == user) || (h.FromEmpleadoId == user && h.ToEmpleadoId == contactId))
        .OrderBy(a => a.CreatedDate)
        .Include(a => a.FromEmpleado)
        .Include(a => a.ToEmpleado)
        .Select(x => new ChatMessage
        {
            FromEmpleadoId = x.FromEmpleadoId,
            Message = x.Message,
            CreatedDate = x.CreatedDate,
            Id = x.Id,
            ToEmpleadoId = x.ToEmpleadoId,
            ToEmpleado = x.ToEmpleado,
            FromEmpleado = x.FromEmpleado
        }).ToListAsync();

                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
             
            }

         
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsersAsync()
        {
         
            var allUsers = await _context.Empleadoes.Where(x=> x.Activo == "SI").ToListAsync();
            return Ok(allUsers);
        }

        [HttpGet("userschats/{user}")]
        public async Task<IActionResult> userschats(int user)
        {


            try
            {
                var messages = await _context.ChatMessages
                    .Where(h => (h.FromEmpleadoId == user))
                    .OrderBy(a => a.CreatedDate)
                    .Include(a => a.FromEmpleado)
                    .Include(a => a.ToEmpleado)
                    .Select(x => new Empleadoes
                    {
                        Id = x.ToEmpleadoId,
                        Nombres = x.ToEmpleado.Nombres
                    })
                    .Union(_context.ChatMessages
                        .Where(h => (h.ToEmpleadoId == user))
                        .OrderBy(a => a.CreatedDate)
                        .Include(a => a.FromEmpleado)
                        .Include(a => a.ToEmpleado)
                        .Select(x => new Empleadoes
                        {
                            Id = x.FromEmpleadoId,
                            Nombres = x.FromEmpleado.Nombres
                        }))
                    .ToListAsync();

                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }

          
        }
        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserDetailsAsync(int userId)
        {
            var user = await _context.Empleadoes.Where(user => user.Id == userId).FirstOrDefaultAsync();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> SaveMessageAsync(ChatMessage message)
        {
            try
            {
                message.CreatedDate = DateTime.Now;
                message.ToEmpleado = await _context.Empleadoes.Where(user => user.Id == message.ToEmpleadoId).FirstOrDefaultAsync();
                await _context.ChatMessages.AddAsync(message);
                await _context.SaveChangesAsync();
            return Ok();
            }
            catch (Exception)
            {

                return BadRequest();
            }
        
        }
    }
}
