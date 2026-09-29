using OrderManagement.Domain.Models;

namespace OrderManagement.Domain.Repositories;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}