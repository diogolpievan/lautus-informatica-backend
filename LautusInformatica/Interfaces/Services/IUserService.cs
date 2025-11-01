using LautusInformatica.DTOs.User;

namespace LautusInformatica.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserResponseDTO> GetUserById(int id);
        Task<UserResponseDTO> GetUserByEmail(string email);
        Task<IEnumerable<UserResponseDTO>> GetAllUsers();
        Task<UserResponseDTO> CreateUser(UserRequestDTO userDto);
        Task<bool> UpdateUser(int id, UserRequestDTO userDto);
        Task<bool> DeleteUser(int id);
        Task<bool> ChangePassword(int id, string newPassword);
        Task<bool> UnlockUser(int id);
        Task<bool> UserLoginIsValid(string email, string password);
    }
}
