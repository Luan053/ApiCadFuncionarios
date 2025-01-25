using Microsoft.EntityFrameworkCore;
using WebApi.Domain.DTOs;
using WebApi.Domain.Model.EmployeeAggregate;

namespace WebApi.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ConnectionContext _context;
        private readonly IConfiguration _configuration;

        public EmployeeRepository(ConnectionContext context, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void Add(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
        }

        public (List<EmployeeDTO> Employees, int TotalCount) Get(int pageNumber, int pageSize)
        {
            var baseUrl = _configuration["BaseUrl"] ?? "http://localhost:5000";
            var query = _context.Employees.AsNoTracking();
            var totalCount = query.Count();

            var employees = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EmployeeDTO
                {
                    Id = e.Id,
                    Name = e.Name,
                    Age = e.Age,
                    Photo = e.Photo,
                    Role = e.Role,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    PhotoUrl = $"{baseUrl}/api/v1/employee/{e.Id}/photo"
                })
                .ToList();

            return (employees, totalCount);
        }

        public Employee? Get(int id)
        {
            return _context.Employees.Find(id);
        }

        public void Update(Employee employee)
        {
            _context.Entry(employee).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                _context.SaveChanges();
            }
        }

        public bool Exists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
