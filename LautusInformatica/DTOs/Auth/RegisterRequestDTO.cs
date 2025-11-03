using LautusInformatica.Models.Enums;

namespace LautusInformatica.DTOs.Auth
{
    public class RegisterRequestDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
}
