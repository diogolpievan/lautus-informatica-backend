using LautusInformatica.DTOs.User;

namespace LautusInformatica.DTOs.Auth
{
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public UserResponseDTO User { get; set; }
    }
}
