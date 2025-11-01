using LautusInformatica.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LautusInformatica.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        [Range(0, 1, ErrorMessage = "O campo Role deve ser 0 (Admin) ou 1 (Client).")]
        public UserRole Role { get; set; } = UserRole.Client;
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedDate { get; set; }
        public bool IsLocked { get; set; } = false;
        public int AccessFailedCount { get; set; } = 0;


        public ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();

    }
}
