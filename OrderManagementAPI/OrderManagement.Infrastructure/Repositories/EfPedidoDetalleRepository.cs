using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Models;
using OrderManagement.Domain.Repositories;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.Infrastructure.Repositories;

public sealed class EfPedidoDetalleRepository(OrderDbContext context)
    : EfRepository<PedidoDetalle>(context), IPedidoDetalleRepository
{
    public async Task<IReadOnlyList<PedidoDetalle>> GetByPedidoIdAsync(long pedidoId, CancellationToken cancellationToken = default)
    {
        return await Context.DetallesPedido.AsNoTracking()
            .Where(detalle => detalle.PedidoId == pedidoId)
            .ToListAsync(cancellationToken);
    }
}