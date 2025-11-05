using LautusInformatica.Data;
using LautusInformatica.DTOs;
using LautusInformatica.DTOs.User;
using LautusInformatica.DTOs.Auth;
using LautusInformatica.Models;
using LautusInformatica.Services;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize]
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

        [Authorize]
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

        [Authorize]
        [HttpPost("{id}/change-password")]
        public async Task<ActionResult<ApiResponse<bool>>> ChangePassword(int id, [FromBody] ChangePasswordDTO changePasswordDTO)
        {
            var result = await _userService.ChangePassword(id, changePasswordDTO);
            var apiResponse = new ApiResponse<bool>
            {
                Message = result ? "Senha alterada com sucesso" : "Falha ao alterar a senha",
                Success = result,
                Data = result
            };
            return Ok(apiResponse);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/lock")]
        public async Task<ActionResult<ApiResponse<bool>>> UnlockUser(int id)
        {
            var result = await _userService.UnlockUser(id);
            var apiResponse = new ApiResponse<bool>
            {
                Message = result ? "User desbloqueado com sucesso" : "Falha ao desbloquear o user",
                Success = result,
                Data = result
            };
            return Ok(apiResponse);
        }
    }
}
