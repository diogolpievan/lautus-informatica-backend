using Dapper;
using LautusInformatica.Data;
using LautusInformatica.Interfaces.Repositories;
using LautusInformatica.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace LautusInformatica.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<User> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await _context.Users.FindAsync(email);
            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public async Task<int> CreateUser(User user)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_Username", user.Username);
            parameters.Add("@p_PasswordHash", user.PasswordHash);
            parameters.Add("@p_Phone", user.Phone);
            parameters.Add("@p_Email", user.Email);
            parameters.Add("@p_Role", user.Role);
            parameters.Add("@p_Address", user.Address);
            parameters.Add("@p_UserId", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            using (var connection = new MySqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.ExecuteAsync("sp_CreateUser", parameters, commandType: System.Data.CommandType.StoredProcedure);
                int userId = parameters.Get<int>("@p_UserId");
                return userId;
            }
        }

        public async Task<bool> UpdateUser(User user)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_Id", user.Id);
            parameters.Add("@p_Username", user.Username);
            parameters.Add("@p_Phone", user.Phone);
            parameters.Add("@p_Email", user.Email);
            parameters.Add("@p_Role", user.Role);
            parameters.Add("@p_Address", user.Address);
            parameters.Add("@p_Success", dbType: System.Data.DbType.Boolean, direction: System.Data.ParameterDirection.Output);

            using (var connection = new MySqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.ExecuteAsync("sp_UpdateUser", parameters, commandType: System.Data.CommandType.StoredProcedure);
                bool success = parameters.Get<bool>("@p_Success");
                return success;
            }
        }

        public async Task<bool> DeleteUser(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_Id", id);
            parameters.Add("@p_Success", dbType: System.Data.DbType.Boolean, direction: System.Data.ParameterDirection.Output);

            using (var connection = new MySqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.ExecuteAsync("sp_DeleteUser", parameters, commandType: System.Data.CommandType.StoredProcedure);
                bool success = parameters.Get<bool>("@p_Success");
                return success;
            }
        }

        public async Task<bool> ChangePassword(int id, string newPasswordHash)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_Id", id);
            parameters.Add("@p_NewPasswordHash", newPasswordHash);
            parameters.Add("@p_Success", dbType: System.Data.DbType.Boolean, direction: System.Data.ParameterDirection.Output);

            using (var connection = new MySqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.ExecuteAsync("sp_ChangeUserPassword", parameters, commandType: System.Data.CommandType.StoredProcedure);
                bool success = parameters.Get<bool>("@p_Success");
                return success;
            }
        }

        public async Task<bool> UnlockUser(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_Id", id);
            parameters.Add("@p_Success", dbType: System.Data.DbType.Boolean, direction: System.Data.ParameterDirection.Output);

            using (var connection = new MySqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.ExecuteAsync("sp_UnlockUser", parameters, commandType: System.Data.CommandType.StoredProcedure);
                bool success = parameters.Get<bool>("@p_Success");
                return success;
            }
        }

        public async Task<bool> UserLoginIsValid(string email, string passwordHash)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_Email", email);
            parameters.Add("@p_PasswordHash", passwordHash);
            parameters.Add("@p_Success", dbType: System.Data.DbType.Boolean, direction: System.Data.ParameterDirection.Output);
            using (var connection = new MySqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.ExecuteAsync("sp_ValidateUserLogin", parameters, commandType: System.Data.CommandType.StoredProcedure);
                bool isValid = parameters.Get<bool>("@p_Success");
                return isValid;
            }
        } 
    }
}
