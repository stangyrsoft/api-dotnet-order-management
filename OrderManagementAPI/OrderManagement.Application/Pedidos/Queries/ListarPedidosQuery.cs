using MediatR;
using OrderManagement.Application.Pedidos.Dtos;
using OrderManagement.Domain.Repositories;

namespace OrderManagement.Application.Pedidos.Queries;

public sealed record ListarPedidosQuery : IRequest<IReadOnlyList<PedidoDto>>;

public sealed class ListarPedidosQueryHandler(IPedidoRepository pedidos)
    : IRequestHandler<ListarPedidosQuery, IReadOnlyList<PedidoDto>>
{
    public async Task<IReadOnlyList<PedidoDto>> Handle(
        ListarPedidosQuery request,
        CancellationToken cancellationToken)
    {
        var orders = await pedidos.GetActivosAsync(cancellationToken);
        return orders.Select(PedidoDtoMapper.Map).ToArray();
    }
}