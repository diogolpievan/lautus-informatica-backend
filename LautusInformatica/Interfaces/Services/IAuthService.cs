using LautusInformatica.DTOs.Auth;
using LautusInformatica.DTOs.User;

namespace LautusInformatica.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> UserLoginIsValid(LoginRequestDTO loginUserDto);
        Task<AuthResponseDTO> RegisterUser(RegisterRequestDTO registerUserDto);
        Task<string> GenerateJWTToken(UserResponseDTO userResponseDto);
    }
}
