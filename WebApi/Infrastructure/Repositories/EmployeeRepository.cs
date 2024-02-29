using WebApi.Domain.DTOs;
using WebApi.Domain.Model;

namespace WebApi.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ConnectionContext _context = new ConnectionContext();
        public void Add(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
        }

        public List<EmployeeDTO> Get(int pageNumber, int pageQuantity)
        {
            return _context.Employees.Skip(pageNumber * pageQuantity)
                .Take(pageQuantity)
                .Select(b => 
                    new EmployeeDTO()
                    {
                        Id = b.id,
                        NameEmployee = b.name,
                        Photo = b.photo,
                        Role = b.role
                    })
                .ToList();
        }

        public Employee? Get(int id)
        {
            return _context.Employees.Find(id);
        }

        

        public void Delete(int id)
        {
            var employeeToDelete = _context.Employees.FirstOrDefault(e => e.id == id);
            if (employeeToDelete != null)
            {
                _context.Employees.Remove(employeeToDelete);
                _context.SaveChanges();
            }
        }


    }
}
