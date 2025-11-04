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

namespace LautusInformatica.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;
        private readonly UserService _userService;

        public AuthService(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _userService = new UserService(userRepository);
            _config = config;
        } 
        public async Task<AuthResponseDTO> LoginUser(LoginRequestDTO loginUserDto)
        {   
            string email = loginUserDto.Email;
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(loginUserDto.Password);

            var user = await _userService.GetUserByEmail(email);
            if (user == null) throw new UserNotFoundException();

            try
            {
                if (_userRepository.UserLoginIsValid(email, hashedPassword).Result)
                {
                    //token
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

        public async Task<AuthResponseDTO> RegisterUser(RegisterRequestDTO registerDto)
        {
            var existingUser = await _userService.GetUserByEmail(registerDto.Email);
            if (existingUser != null) throw new UserEmailAlreadyExistsException();

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

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

                var createdUser = _userService.CreateUser(userDto).Result;

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
                new Claim(ClaimTypes.Role, userResponseDto.Role.ToString()),
                new Claim("username", userResponseDto.Username)
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
