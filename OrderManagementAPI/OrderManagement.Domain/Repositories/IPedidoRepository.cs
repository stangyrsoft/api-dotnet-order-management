using OrderManagement.Domain.Models;

namespace OrderManagement.Domain.Repositories;

public interface IPedidoRepository : IRepository<Pedido>
{
    Task<Pedido?> GetByNumeroPedidoAsync(string numeroPedido, CancellationToken cancellationToken = default);
}