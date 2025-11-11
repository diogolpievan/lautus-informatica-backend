using LautusInformatica.Models.Enums;

namespace LautusInformatica.DTOs.User
{
    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public string Address { get; set; }
        public bool IsLocked { get; set; }
    }
}
