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
        [MaxLength(100)]
        public string Username { get; private set; }
        
        [Required]
        public string PasswordHash { get; private set; }
        
        [MaxLength(50)]
        public string? Role { get; private set; }
        
        public DateTime CreatedAt { get; private set; }

        // Construtor para EF Core
        private User() 
        { 
            Username = null!;
            PasswordHash = null!;
        }

        public User(string username, string passwordHash, string? role = null)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            Role = role;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdatePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash ?? throw new ArgumentNullException(nameof(newPasswordHash));
        }
    }
}
