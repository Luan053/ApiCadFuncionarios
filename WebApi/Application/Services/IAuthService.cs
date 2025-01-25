using WebApi.Application.ViewModel;
using WebApi.Domain.Model.UserAggregate;

namespace WebApi.Application.Services
{
    public interface IAuthService
    {
        Task<(bool success, string token, string refreshToken)> AuthenticateAsync(LoginViewModel model);
        Task<(bool success, string token, string refreshToken)> RefreshTokenAsync(string token, string refreshToken);
        Task<bool> RegisterAsync(RegisterViewModel model);
        Task<bool> RevokeTokenAsync(string username);
        Task<User?> GetUserByUsernameAsync(string username);
    }
}
