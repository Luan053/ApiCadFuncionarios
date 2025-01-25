using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Domain.Model.UserAggregate
{
    [Table("users")]
    public class User
    {
        [Key]
        public int Id { get; private set; }

        [Required]
        [StringLength(100)]
        public string Username { get; private set; }

        [Required]
        [StringLength(100)]
        public string Email { get; private set; }

        [Required]
        public string PasswordHash { get; private set; }

        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }

        protected User() { } // For EF Core

        public User(string username, string email, string passwordHash)
        {
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
        }

        public void SetRefreshToken(string refreshToken, DateTime expiryTime)
        {
            RefreshToken = refreshToken;
            RefreshTokenExpiryTime = expiryTime;
        }

        public void ClearRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiryTime = null;
        }
    }
}
