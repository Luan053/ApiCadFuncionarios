using WebApi.Domain.DTOs;

namespace WebApi.Domain.Model.EmployeeAggregate
{
    public interface IEmployeeRepository
    {
        void Add(Employee employee);
        (List<EmployeeDTO> Employees, int TotalCount) Get(int pageNumber, int pageSize);
        Employee? Get(int id);
        void Update(Employee employee);
        void Delete(int id);
        bool Exists(int id);
        Task<bool> SaveChangesAsync();
    }
}
