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

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] User updatedUser)
        {
            if (id != updatedUser.Id)
                return BadRequest("ID do Usuario nao corresponde");

            var user = await _context.Users.FindAsync(id);

            if (user == null || user.IsDeleted == true)
                return NotFound();

            user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;
            user.Phone = updatedUser.Phone;
            user.Address = updatedUser.Address;
            user.Role = updatedUser.Role;
            user.PasswordHash = updatedUser.PasswordHash;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null || user.IsDeleted == true)
                return NotFound();

            user.IsDeleted = true;
            user.DeletedDate = DateTime.Now;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();

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
