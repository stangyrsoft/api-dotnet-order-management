using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Models;
using OrderManagement.Domain.Repositories;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.Infrastructure.Repositories;

public sealed class EfPedidoRepository(OrderDbContext context)
    : EfRepository<Pedido>(context), IPedidoRepository
{
    public Task<Pedido?> GetByNumeroPedidoAsync(string numeroPedido, CancellationToken cancellationToken = default)
    {
        return Context.Pedidos.AsNoTracking()
            .FirstOrDefaultAsync(pedido => pedido.NumeroPedido == numeroPedido, cancellationToken);
    }

    public async Task<IReadOnlyList<Pedido>> GetActivosAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Pedidos.AsNoTracking()
            .Where(pedido => pedido.Activo)
            .Include(pedido => pedido.Cliente)
            .Include(pedido => pedido.Detalles)
                .ThenInclude(detalle => detalle.Producto)
            .OrderByDescending(pedido => pedido.FechaPedido)
            .ThenByDescending(pedido => pedido.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<Pedido?> GetActivoConDetallesAsync(long id, CancellationToken cancellationToken = default)
    {
        return Context.Pedidos.AsNoTracking()
            .Where(pedido => pedido.Activo && pedido.Id == id)
            .Include(pedido => pedido.Cliente)
            .Include(pedido => pedido.Detalles)
                .ThenInclude(detalle => detalle.Producto)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Pedido?> GetActivoConDetallesParaActualizarAsync(long id, CancellationToken cancellationToken = default)
    {
        return Context.Pedidos
            .Where(pedido => pedido.Activo && pedido.Id == id)
            .Include(pedido => pedido.Detalles)
            .FirstOrDefaultAsync(cancellationToken);
    }
}