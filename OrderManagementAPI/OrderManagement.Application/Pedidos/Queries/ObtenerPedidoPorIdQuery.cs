using MediatR;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Pedidos.Dtos;
using OrderManagement.Domain.Repositories;

namespace OrderManagement.Application.Pedidos.Queries;

public sealed record ObtenerPedidoPorIdQuery(long Id) : IRequest<PedidoDto>;

public sealed class ObtenerPedidoPorIdQueryHandler(IPedidoRepository pedidos)
    : IRequestHandler<ObtenerPedidoPorIdQuery, PedidoDto>
{
    public async Task<PedidoDto> Handle(
        ObtenerPedidoPorIdQuery request,
        CancellationToken cancellationToken)
    {
        var pedido = await pedidos.GetActivoConDetallesAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"No existe un pedido activo con ID {request.Id}.");

        return PedidoDtoMapper.Map(pedido);
    }
}