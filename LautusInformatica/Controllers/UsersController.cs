using LautusInformatica.Data;
using LautusInformatica.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LautusInformatica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly AppDbContext _context;

        public UsersController(ILogger<UsersController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser([FromBody] User user)
        {
            if (user == null)
                return BadRequest("Usuário inválido.");

            bool emailExists = await _context.Users.AnyAsync(u => u.Email == user.Email && u.IsDeleted == false);
            if (emailExists)
                return Conflict("Já existe um usuário com este e-mail.");

            user.CreatedAt = DateTime.UtcNow;
            user.IsDeleted = false;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null || user.IsDeleted == true)
                return NotFound();

            return Ok(user);
        }

    }
}
