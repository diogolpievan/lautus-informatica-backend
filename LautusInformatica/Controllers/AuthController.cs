using LautusInformatica.DTOs;
using LautusInformatica.DTOs.Auth;
using LautusInformatica.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LautusInformatica.Controllers
{
        
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;
        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> LoginUser([FromBody] LoginRequestDTO loginUserDto)
        {
            var authResponse = await _authService.LoginUser(loginUserDto);
            var apiResponse = new ApiResponse<AuthResponseDTO>
            {
                Message = "Login realizado com sucesso",
                Success = true,
                Data = authResponse
            };
            return Ok(apiResponse);
        }

        public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> RegisterUser([FromBody] RegisterRequestDTO registerUserDto)
        {
            var authResponse = await _authService.RegisterUser(registerUserDto);
            var apiResponse = new ApiResponse<AuthResponseDTO>
            {
                Message = "Registro realizado com sucesso",
                Success = true,
                Data = authResponse
            };
            return Ok(apiResponse);
        }
    }
}
