using LautusInformatica.Exceptions;
using LautusInformatica.Interfaces.Services;
using LautusInformatica.Interfaces.Repositories;
using MySqlConnector;
using LautusInformatica.DTOs.User;
using LautusInformatica.Models;
using LautusInformatica.DTOs.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LautusInformatica.Exceptions.NotFound;
using LautusInformatica.Exceptions.AlreadyExists;

namespace LautusInformatica.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;

        public AuthService(IConfiguration config, IUserService userService)
        {
            _userService = userService;
            _config = config;
        } 
        public async Task<AuthResponseDTO> LoginUser(LoginRequestDTO loginUserDto)
        {   
            string email = loginUserDto.Email;
            string password = loginUserDto.Password;

            var user = await _userService.GetUserByEmail(email);
            if (user == null) throw new UserNotFoundException();

            try
            {
                if (await _userService.UserLoginIsValid(email, password))

                {
                    return new AuthResponseDTO
                    {
                        User = user,
                        Token = await GenerateJWTToken(user)
                    };
                }
                else
                {
                    throw new InvalidLoginException();
                }
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

        public async Task<AuthResponseDTO> RegisterUser(RegisterRequestDTO registerDto, int authId)
        {
            var existingUser = await _userService.GetUserByEmail(registerDto.Email);
            if (existingUser != null) throw new UserEmailAlreadyExistsException();

            try
            {
                var userDto = new UserRequestDTO
                {
                    Username = registerDto.Username,
                    Password = registerDto.Password,
                    Phone = registerDto.Phone,
                    Email = registerDto.Email,
                    Role = Models.Enums.UserRole.Client,
                    Address = registerDto.Address
                };

                var createdUser = await _userService.CreateUser(userDto, authId);

                return new AuthResponseDTO
                {
                    User = createdUser,
                    Token = await GenerateJWTToken(createdUser)
                };
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

        private Task<string> GenerateJWTToken(UserResponseDTO userResponseDto)
        {
            var jwtSettings = _config.GetSection("Jwt");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userResponseDto.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, userResponseDto.Email),
                new Claim(ClaimTypes.Role, userResponseDto.Role == 0 ? "Admin" : "User"),
                new Claim("username", userResponseDto.Username),
                new Claim("role", ((int)userResponseDto.Role).ToString())  
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(int.Parse(jwtSettings["ExpiresInHours"])),
                signingCredentials: creds
            );

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
