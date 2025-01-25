using WebApi.Domain.Model.EmployeeAggregate;

namespace WebApi.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IEmployeeRepository Employees { get; }
        Task<bool> CommitAsync();
        Task RollbackAsync();
    }
}
