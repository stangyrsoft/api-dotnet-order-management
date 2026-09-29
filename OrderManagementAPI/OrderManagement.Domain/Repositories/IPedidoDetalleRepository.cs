using OrderManagement.Domain.Models;

namespace OrderManagement.Domain.Repositories;

public interface IPedidoDetalleRepository : IRepository<PedidoDetalle>
{
    Task<IReadOnlyList<PedidoDetalle>> GetByPedidoIdAsync(long pedidoId, CancellationToken cancellationToken = default);
}