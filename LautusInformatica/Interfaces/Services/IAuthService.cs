using LautusInformatica.DTOs.Auth;
using LautusInformatica.DTOs.User;

namespace LautusInformatica.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> LoginUser(LoginRequestDTO loginUserDto);
        Task<AuthResponseDTO> RegisterUser(RegisterRequestDTO registerUserDto, int authId);
    }
}
