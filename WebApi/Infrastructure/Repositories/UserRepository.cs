using WebApi.Domain.Model.UserAggregate;

namespace WebApi.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ConnectionContext _context;

        public UserRepository(ConnectionContext context)
        {
            _context = context;
        }

        public User? GetByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public bool UsernameExists(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }
    }
}
