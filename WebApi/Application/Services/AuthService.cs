using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApi.Application.ViewModel;
using WebApi.Domain.Model.UserAggregate;
using BC = BCrypt.Net.BCrypt;

namespace WebApi.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IConfiguration configuration, IUserRepository userRepository, ILogger<AuthService> logger)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<(bool success, string token, string refreshToken)> AuthenticateAsync(LoginViewModel model)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(model.Username);
                if (user == null || !BC.Verify(model.Password, user.PasswordHash))
                {
                    return (false, string.Empty, string.Empty);
                }

                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();
                
                user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
                await _userRepository.UpdateAsync(user);

                return (true, token, refreshToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao autenticar usuário {Username}", model.Username);
                return (false, string.Empty, string.Empty);
            }
        }

        public async Task<(bool success, string token, string refreshToken)> RefreshTokenAsync(string token, string refreshToken)
        {
            try
            {
                var principal = GetPrincipalFromExpiredToken(token);
                var username = principal.Identity?.Name;
                
                var user = await _userRepository.GetByUsernameAsync(username);
                if (user == null || 
                    user.RefreshToken != refreshToken || 
                    user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    return (false, string.Empty, string.Empty);
                }

                var newToken = GenerateJwtToken(user);
                var newRefreshToken = GenerateRefreshToken();
                
                user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
                await _userRepository.UpdateAsync(user);

                return (true, newToken, newRefreshToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao renovar token");
                return (false, string.Empty, string.Empty);
            }
        }

        public async Task<bool> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                if (await _userRepository.ExistsByUsernameOrEmailAsync(model.Username, model.Email))
                {
                    return false;
                }

                var passwordHash = BC.HashPassword(model.Password);
                var user = new User(model.Username, model.Email, passwordHash);
                
                await _userRepository.AddAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar usuário {Username}", model.Username);
                return false;
            }
        }

        public async Task<bool> RevokeTokenAsync(string username)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(username);
                if (user == null) return false;

                user.ClearRefreshToken();
                await _userRepository.UpdateAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao revogar token do usuário {Username}", username);
                return false;
            }
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Key"])),
                ValidateLifetime = false,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || 
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Token inválido");
            }

            return principal;
        }
    }
}
