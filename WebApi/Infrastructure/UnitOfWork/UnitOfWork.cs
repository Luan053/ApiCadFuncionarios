using Microsoft.EntityFrameworkCore;
using WebApi.Domain.Model.EmployeeAggregate;
using WebApi.Infrastructure.Repositories;

namespace WebApi.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ConnectionContext _context;
        private readonly IConfiguration _configuration;
        private IEmployeeRepository _employeeRepository;
        private bool _disposed;

        public UnitOfWork(ConnectionContext context, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IEmployeeRepository Employees => _employeeRepository ??= new EmployeeRepository(_context, _configuration);

        public async Task<bool> CommitAsync()
        {
            try
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var result = await _context.SaveChangesAsync() > 0;
                    await transaction.CommitAsync();
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    await entry.ReloadAsync();
                }
                throw;
            }
        }

        public async Task RollbackAsync()
        {
            await _context.DisposeAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}
