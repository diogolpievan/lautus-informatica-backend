using LautusInformatica.DTOs.User;
using LautusInformatica.Interfaces.Services;
using LautusInformatica.Interfaces.Repositories;
using LautusInformatica.Exceptions;
using LautusInformatica.Exceptions.NotFound;
using LautusInformatica.Models;
using MySqlConnector;
using LautusInformatica.DTOs.Auth;
using LautusInformatica.Exceptions.AlreadyExists;
using System.Linq;

namespace LautusInformatica.Services;
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponseDTO> GetUserById(int id)
    {
        var user = await _userRepository.GetUserById(id);
        if (user == null) throw new UserNotFoundException();
        return new UserResponseDTO
        {
            Id = user.Id,
            Username = user.Username,
            Phone = user.Phone,
            Email = user.Email,
            Role = user.Role,
            Address = user.Address
        };
    }

    public async Task<UserResponseDTO?> GetUserByEmail(string email)
    {
        var user = await _userRepository.GetUserByEmail(email);
        if (user == null) return null;
        return new UserResponseDTO
        {
            Id = user.Id,
            Username = user.Username,
            Phone = user.Phone,
            Email = user.Email,
            Role = user.Role,
            Address = user.Address
        };
    }

    public async Task<IEnumerable<UserResponseDTO>> GetAllUsers()
    {
        var allUsers = await _userRepository.GetAllUsers();
        return allUsers.Select(user => new UserResponseDTO
        {
            Id = user.Id,
            Username = user.Username,
            Phone = user.Phone,
            Email = user.Email,
            Role = user.Role,
            Address = user.Address
        });
    }

    public async Task<UserResponseDTO> CreateUser(UserRequestDTO userRequest, int authId)
    {
        var user = new User();
        user.Username = userRequest.Username;
        user.Password = userRequest.Password;
        user.Phone = userRequest.Phone;
        user.Email = userRequest.Email;
        user.Role = userRequest.Role;
        user.Address = userRequest.Address;

        try
        {
            int createdUserId = await _userRepository.CreateUser(user, authId);
            return await GetUserById(createdUserId);
        }
        catch (MySqlException exception)
        {
            switch (exception.SqlState)
            {
                case "45001":
                    throw new UserEmailAlreadyExistsException();
                default:
                    throw;
            }
        }

    }

    public async Task<UserResponseDTO> UpdateUser(int id, UserRequestDTO userRequest, int authId)
    {
        var user = await _userRepository.GetUserById(id);
        if (user == null) throw new UserNotFoundException();

        user.Phone = userRequest.Phone;
        user.Email = userRequest.Email;
        user.Role = userRequest.Role;
        user.Address = userRequest.Address;

        try
        {
            await _userRepository.UpdateUser(user, authId);
            return await GetUserById(id);
        }
        catch (MySqlException exception)
        {
            switch (exception.SqlState)
            {
                case "45000":
                    throw new UserNotFoundException();
                case "45001":
                    throw new UserEmailAlreadyExistsException();
                case "45002":
                    throw new UserLockedException();
                default:
                    throw;
            }
        }
    }
    public async Task<bool> DeleteUser(int id, int authId)
    {
        try
        {
            return await _userRepository.DeleteUser(id, authId);
        }
        catch (MySqlException exception)
        {
            switch (exception.SqlState)
            {
                case "45000":
                    throw new UserNotFoundException();
                default:
                    throw;
            }
        }
    }

    public async Task<bool> ChangePassword(int id, ChangePasswordDTO changePasswordDTO, int authId)
    {
        var user = await _userRepository.GetUserById(id);
        if (user == null) throw new UserNotFoundException();

        try
        {
            if (changePasswordDTO.NewPassword != changePasswordDTO.VerifyNewPassword)
            {
                throw new BadRequestException("As senhas não coincidem");
            }
            return await _userRepository.ChangePassword(id, changePasswordDTO.NewPassword, authId);
        }
        catch (MySqlException exception)
        {
            switch (exception.SqlState)
            {
                case "45000":
                    throw new UserNotFoundException();
                default:
                    throw;
            }
        }
    }
    public async Task<bool> UnlockUser(int id, int authId)
    {
        var user = await _userRepository.GetUserById(id);
        if (user == null) throw new UserNotFoundException();

        try
        {
            return await _userRepository.UnlockUser(id, authId);
        }
        catch (MySqlException exception)
        {
            switch (exception.SqlState)
            {
                case "45000":
                    throw new UserNotFoundException();
                default:
                    throw;
            }
        }
    }

    public async Task<bool> UserLoginIsValid(string email, string password)
    {
        var user = await _userRepository.GetUserByEmail(email);
        if (user == null) throw new UserNotFoundException();

        try
        {
            return await _userRepository.UserLoginIsValid(email, password);
        }
        catch (MySqlException exception)
        {
            switch (exception.SqlState)
            {
                case "45000":
                    throw new UserNotFoundException();
                case "45002":
                    throw new UserLockedException();
                default:
                    throw;
            }
        }
    }
}
