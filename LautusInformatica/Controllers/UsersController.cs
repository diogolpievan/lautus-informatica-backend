using LautusInformatica.Data;
using LautusInformatica.DTOs;
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
        public async Task<ActionResult<ApiResponse<IEnumerable<UserResponseDTO>>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            
            var apiResponse = new ApiResponse<IEnumerable<UserResponseDTO>>
            {
                Message = "User listado com sucesso",
                Success = true,
                Data = users
            };

            return Ok(apiResponse);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserResponseDTO>>> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);

            var apiResponse = new ApiResponse<UserResponseDTO>
            {
                Message = "User encontrado com sucesso",
                Success = true,
                Data = user
            };

            return Ok(apiResponse);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<ApiResponse<UserResponseDTO>>> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmail(email);

            var apiResponse = new ApiResponse<UserResponseDTO>
            {
                Message = "User encontrado com sucesso",
                Success = true,
                Data = user
            };

            return Ok(apiResponse);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserResponseDTO>>> CreateUser([FromBody] UserRequestDTO userRequestDTO)
        {
            var user = await _userService.CreateUser(userRequestDTO);

            var apiResponse = new ApiResponse<UserResponseDTO>
            {
                Message = "User criado com sucesso",
                Success = true,
                Data = user
            };

            return CreatedAtAction(nameof(GetUserById), apiResponse);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<UserResponseDTO>>> UpdateUser(int id, [FromBody] UserRequestDTO userRequestDTO)
        {
            var user = await _userService.UpdateUser(id, userRequestDTO);
            var apiResponse = new ApiResponse<UserResponseDTO>
            {
                Message = "User atualizado com sucesso",
                Success = true,
                Data = user
            };
            return Ok(apiResponse);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(int id)
        {
            var result = await _userService.DeleteUser(id);
            var apiResponse = new ApiResponse<bool>
            {
                Message = result ? "User deletado com successo" : "User deletion failed",
                Success = result,
                Data = result
            };
            return Ok(apiResponse);
        }

    }
}
