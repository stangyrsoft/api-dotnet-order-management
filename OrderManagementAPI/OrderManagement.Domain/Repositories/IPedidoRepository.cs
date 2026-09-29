using OrderManagement.Domain.Models;

namespace OrderManagement.Domain.Repositories;

public interface IPedidoRepository : IRepository<Pedido>
{
    Task<Pedido?> GetByNumeroPedidoAsync(string numeroPedido, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Pedido>> GetActivosAsync(CancellationToken cancellationToken = default);
    Task<Pedido?> GetActivoConDetallesAsync(long id, CancellationToken cancellationToken = default);
    Task<Pedido?> GetActivoConDetallesParaActualizarAsync(long id, CancellationToken cancellationToken = default);
}