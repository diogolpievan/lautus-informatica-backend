using LautusInformatica.DTOs.Auth;
using LautusInformatica.DTOs.User;

namespace LautusInformatica.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserResponseDTO> GetUserById(int id);
        Task<UserResponseDTO?> GetUserByEmail(string email);
        Task<IEnumerable<UserResponseDTO>> GetAllUsers();
        Task<UserResponseDTO> CreateUser(UserRequestDTO userDto, int authId);
        Task<UserResponseDTO> UpdateUser(int id, UserRequestDTO userDto, int authId);
        Task<bool> DeleteUser(int id, int authId);
        Task<bool> ChangePassword(int id, ChangePasswordDTO changePasswordDTO, int authId);
        Task<bool> UnlockUser(int id, int authId);
        Task<bool> UserLoginIsValid(string email, string password);
    }
}
