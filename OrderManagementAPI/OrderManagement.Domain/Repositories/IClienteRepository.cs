using OrderManagement.Domain.Models;

namespace OrderManagement.Domain.Repositories;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<Cliente?> GetByDniAsync(string dni, CancellationToken cancellationToken = default);
}