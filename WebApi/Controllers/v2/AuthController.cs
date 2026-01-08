using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.Services;
using WebApi.Domain.Model.UserAggregate;

namespace WebApi.Controllers.v2
{
    [ApiVersion(2.0)]
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly TokenService _tokenService;

        public AuthController(IUserRepository userRepository, TokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Username e password são obrigatórios");

            var user = _userRepository.GetByUsername(request.Username);
            if (user == null)
                return Unauthorized("Credenciais inválidas");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Credenciais inválidas");

            var token = _tokenService.GenerateToken(user);
            return Ok(token);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Username e password são obrigatórios");

            if (request.Password.Length < 6)
                return BadRequest("Password deve ter no mínimo 6 caracteres");

            if (_userRepository.UsernameExists(request.Username))
                return Conflict("Username já existe");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User(request.Username, passwordHash, request.Role);
            
            _userRepository.Add(user);

            return Created("", new { message = "Usuário criado com sucesso" });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Role { get; set; }
    }
}