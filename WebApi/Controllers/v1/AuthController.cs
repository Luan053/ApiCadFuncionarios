using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.Services;
using WebApi.Application.ViewModel;

namespace WebApi.Controllers.v1
{
    [ApiVersion(1.0)]
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Registra um novo usuário
        /// </summary>
        /// <param name="model">Dados do usuário</param>
        /// <returns>Status do registro</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.RegisterAsync(model);
                if (!result)
                    return BadRequest("Nome de usuário ou email já existe");

                return Ok("Usuário registrado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar usuário");
                return StatusCode(500, "Erro interno ao processar a requisição");
            }
        }

        /// <summary>
        /// Realiza login do usuário
        /// </summary>
        /// <param name="model">Credenciais do usuário</param>
        /// <returns>Token de acesso e refresh token</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var (success, token, refreshToken) = await _authService.AuthenticateAsync(model);
                if (!success)
                    return Unauthorized("Usuário ou senha inválidos");

                return Ok(new AuthResponseViewModel
                {
                    Token = token,
                    RefreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar login");
                return StatusCode(500, "Erro interno ao processar a requisição");
            }
        }

        /// <summary>
        /// Renova o token de acesso usando refresh token
        /// </summary>
        /// <param name="model">Token e refresh token atuais</param>
        /// <returns>Novo token de acesso e refresh token</returns>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(AuthResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var (success, token, refreshToken) = await _authService.RefreshTokenAsync(model.Token, model.RefreshToken);
                if (!success)
                    return Unauthorized("Token inválido ou expirado");

                return Ok(new AuthResponseViewModel
                {
                    Token = token,
                    RefreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao renovar token");
                return StatusCode(500, "Erro interno ao processar a requisição");
            }
        }

        /// <summary>
        /// Revoga o refresh token do usuário
        /// </summary>
        /// <returns>Status da operação</returns>
        [Authorize]
        [HttpPost("revoke")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Revoke()
        {
            try
            {
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                    return Unauthorized();

                var result = await _authService.RevokeTokenAsync(username);
                if (!result)
                    return BadRequest("Erro ao revogar token");

                return Ok("Token revogado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao revogar token");
                return StatusCode(500, "Erro interno ao processar a requisição");
            }
        }
    }
}