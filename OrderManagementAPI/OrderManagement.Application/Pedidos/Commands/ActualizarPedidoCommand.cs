using MediatR;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Domain.Models;
using OrderManagement.Domain.Repositories;

namespace OrderManagement.Application.Pedidos.Commands;

public sealed record ActualizarPedidoCommand(long Id, PedidoInput Pedido, string UsuarioAuditoria) : IRequest<Unit>;

public sealed class ActualizarPedidoCommandHandler(
    IPedidoRepository pedidos,
    IClienteRepository clientes,
    IProductoRepository productos,
    IUnitOfWork unitOfWork) : IRequestHandler<ActualizarPedidoCommand, Unit>
{
    public async Task<Unit> Handle(ActualizarPedidoCommand request, CancellationToken cancellationToken)
    {
        var pedido = await pedidos.GetActivoConDetallesParaActualizarAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"No existe un pedido activo con ID {request.Id}.");

        var input = request.Pedido;
        var cliente = await clientes.GetByIdAsync(input.ClienteId, cancellationToken);
        if (cliente is null || !cliente.Activo)
        {
            throw new NotFoundException($"No existe un cliente activo con ID {input.ClienteId}.");
        }

        var existingOrder = await pedidos.GetByNumeroPedidoAsync(input.NumeroPedido.Trim(), cancellationToken);
        if (existingOrder is not null && existingOrder.Id != request.Id)
        {
            throw new ConflictException($"Ya existe un pedido con número '{input.NumeroPedido}'.");
        }

        var details = input.Detalles!;
        var productIds = details.Select(detail => detail.ProductoId).Distinct().ToArray();
        var activeProducts = await productos.GetActivosByIdsAsync(productIds, cancellationToken);
        if (activeProducts.Count != productIds.Length)
        {
            throw new NotFoundException("Uno o más productos no existen o están inactivos.");
        }

        var productsById = activeProducts.ToDictionary(product => product.Id);
        var total = CrearPedidoCommandHandler.CalculateTotal(details, productsById);
        CrearPedidoCommandHandler.EnsureValidTotal(total);

        var now = DateTime.UtcNow;
        var actor = CrearPedidoCommandHandler.NormalizeAuditUser(request.UsuarioAuditoria);
        pedido.NumeroPedido = input.NumeroPedido.Trim();
        pedido.Estado = input.Estado.Trim();
        pedido.Observacion = input.Observacion;
        pedido.ClienteId = input.ClienteId;
        pedido.FechaPedido = input.FechaPedido;
        pedido.TotalImporte = total;
        pedido.UsuarioModificacion = actor;
        pedido.FechaModificacion = now;
        pedido.Detalles.Clear();

        foreach (var detail in details)
        {
            pedido.Detalles.Add(new PedidoDetalle
            {
                ProductoId = detail.ProductoId,
                Cantidad = detail.Cantidad,
                PrecioUnitario = productsById[detail.ProductoId].Precio,
                Activo = true,
                UsuarioCreacion = actor,
                FechaCreacion = now
            });
        }

        pedidos.Update(pedido);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}