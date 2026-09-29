using OrderManagement.Domain.Models;

namespace OrderManagement.Domain.Repositories;

public interface IProductoRepository : IRepository<Producto>
{
    Task<IReadOnlyList<Producto>> GetActivosAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Producto>> GetActivosByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);
}