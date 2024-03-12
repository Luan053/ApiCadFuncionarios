using WebApi.Domain.DTOs;
using WebApi.Domain.Model.EmployeeAggregate;

namespace WebApi.Domain.Model.CompanyAggregate
{
    public interface ICompanyRepository
    {
        void Add(Employee employee);
        List<EmployeeDTO> Get(int pageNumber, int pageQuantity);
        Employee? Get(int id);
        void Delete(int id);
    }
}
