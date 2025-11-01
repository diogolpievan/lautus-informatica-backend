using LautusInformatica.Models.Enums;

namespace LautusInformatica.DTOs.User
{
    public class UserRequestDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public string Address { get; set; }
    }
}
