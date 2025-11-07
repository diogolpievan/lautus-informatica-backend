using LautusInformatica.Models;

namespace LautusInformatica.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserById(int id);
        Task<User> GetUserByEmail(string email);
        Task<IEnumerable<User>> GetAllUsers();
        Task<int> CreateUser(User user, int authId);
        Task<bool> UpdateUser(User user, int authId);
        Task<bool> DeleteUser(int id, int authId);
        Task<bool> ChangePassword(int id, string newPasswordHash, int authId);
        Task<bool> UnlockUser(int id, int authId);
        Task<bool> UserLoginIsValid(string email, string passwordHash);

    }
}
