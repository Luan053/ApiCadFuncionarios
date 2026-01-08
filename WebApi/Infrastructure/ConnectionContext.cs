using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApi.Domain.Model.CompanyAggregate;
using WebApi.Domain.Model.EmployeeAggregate;
using WebApi.Domain.Model.UserAggregate;

namespace WebApi.Infrastructure
{
    public class ConnectionContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public ConnectionContext(DbContextOptions<ConnectionContext> options, IConfiguration configuration) 
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
