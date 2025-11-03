using LautusInformatica.Data;
using LautusInformatica.DTOs.User;
using LautusInformatica.Models;
using LautusInformatica.Services;
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
        private readonly UserService _userService;

        public UsersController(ILogger<UsersController> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDTO>> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);

            return Ok(user);
        }

        [HttpGet("by-email/{email}")]
        public async Task<ActionResult<UserResponseDTO>> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmail(email);
            return Ok(user);
        }

        [HttpPost("createUser")]
        public async Task<ActionResult<UserResponseDTO>> CreateUser([FromBody] UserRequestDTO userRequestDTO)
        {
            var user = await _userService.CreateUser(userRequestDTO);

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

    }
}
