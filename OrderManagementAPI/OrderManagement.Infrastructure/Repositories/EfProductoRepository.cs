using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Models;
using OrderManagement.Domain.Repositories;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.Infrastructure.Repositories;

public sealed class EfProductoRepository(OrderDbContext context)
    : EfRepository<Producto>(context), IProductoRepository
{
    public async Task<IReadOnlyList<Producto>> GetActivosAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Productos.AsNoTracking()
            .Where(producto => producto.Activo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Producto>> GetActivosByIdsAsync(
        IReadOnlyCollection<long> ids,
        CancellationToken cancellationToken = default)
    {
        return await Context.Productos.AsNoTracking()
            .Where(producto => producto.Activo && ids.Contains(producto.Id))
            .ToListAsync(cancellationToken);
    }
}