namespace WebApi.Domain.Model.UserAggregate
{
    public interface IUserRepository
    {
        User? GetByUsername(string username);
        void Add(User user);
        bool UsernameExists(string username);
    }
}
