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
}