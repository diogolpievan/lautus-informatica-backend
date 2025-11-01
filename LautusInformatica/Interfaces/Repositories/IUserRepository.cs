using LautusInformatica.Models;

namespace LautusInformatica.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserById(int id);
        Task<User> GetUserByEmail(string email);
        Task<IEnumerable<User>> GetAllUsers();
        Task<int> CreateUser(User user);
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(int id);
        Task<bool> ChangePassword(int id, string newPasswordHash);
        Task<bool> UnlockUser(int id);
        Task<bool> UserLoginIsValid(string email, string passwordHash);

    }
}
