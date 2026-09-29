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
        var clienteInput = input.Cliente;
        var cliente = await clientes.GetByDniAsync(clienteInput.Dni.Trim(), cancellationToken);
        if (cliente is not null && !cliente.Activo)
        {
            throw new ConflictException("El DNI pertenece a un cliente inactivo.");
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
        var crearCliente = cliente is null;
        cliente ??= new Cliente
        {
            DNI = clienteInput.Dni.Trim(),
            Nombre = clienteInput.Nombre.Trim(),
            Apellido = clienteInput.Apellido?.Trim(),
            Direccion = clienteInput.Direccion?.Trim(),
            Activo = true,
            UsuarioCreacion = actor,
            FechaCreacion = now
        };

        if (crearCliente)
        {
            await clientes.AddAsync(cliente, cancellationToken);
            pedido.Cliente = cliente;
        }
        else
        {
            pedido.ClienteId = cliente.Id;
        }

        pedido.NumeroPedido = input.NumeroPedido.Trim();
        pedido.Estado = input.Estado.Trim();
        pedido.Observacion = input.Observacion;
        pedido.FechaPedido = DateTime.SpecifyKind(input.FechaPedido, DateTimeKind.Unspecified);
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