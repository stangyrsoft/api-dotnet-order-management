using MediatR;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Domain.Models;
using OrderManagement.Domain.Repositories;

namespace OrderManagement.Application.Pedidos.Commands;

public sealed record CrearPedidoCommand(PedidoInput Pedido, string UsuarioAuditoria) : IRequest<long>;

public sealed class CrearPedidoCommandHandler(
    IPedidoRepository pedidos,
    IClienteRepository clientes,
    IProductoRepository productos,
    IUnitOfWork unitOfWork) : IRequestHandler<CrearPedidoCommand, long>
{
    public async Task<long> Handle(CrearPedidoCommand request, CancellationToken cancellationToken)
    {
        var input = request.Pedido;
        var clienteInput = input.Cliente;
        var cliente = await clientes.GetByDniAsync(clienteInput.Dni.Trim(), cancellationToken);
        if (cliente is not null && !cliente.Activo)
        {
            throw new ConflictException("El DNI pertenece a un cliente inactivo.");
        }

        var existingOrder = await pedidos.GetByNumeroPedidoAsync(input.NumeroPedido.Trim(), cancellationToken);
        if (existingOrder is not null)
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
        var total = CalculateTotal(details, productsById);
        EnsureValidTotal(total);

        var now = DateTime.UtcNow;
        var actor = NormalizeAuditUser(request.UsuarioAuditoria);
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
        }

        var pedido = new Pedido
        {
            NumeroPedido = input.NumeroPedido.Trim(),
            Estado = input.Estado.Trim(),
            Observacion = input.Observacion,
            FechaPedido = DateTime.SpecifyKind(input.FechaPedido, DateTimeKind.Unspecified),
            TotalImporte = total,
            Activo = true,
            UsuarioCreacion = actor,
            FechaCreacion = now
        };

        if (crearCliente)
        {
            pedido.Cliente = cliente;
        }
        else
        {
            pedido.ClienteId = cliente.Id;
        }

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

        await pedidos.AddAsync(pedido, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return pedido.Id;
    }

    internal static decimal CalculateTotal(
        IReadOnlyCollection<PedidoDetalleInput> details,
        IReadOnlyDictionary<long, Producto> productsById)
    {
        return details.Sum(detail => productsById[detail.ProductoId].Precio * detail.Cantidad);
    }

    internal static void EnsureValidTotal(decimal total)
    {
        if (total <= 0 || total > 99999999.99m)
        {
            throw new BusinessRuleException("El total calculado debe ser mayor a 0 y no exceder 99,999,999.99.");
        }
    }

    internal static string NormalizeAuditUser(string user)
    {
        var normalized = string.IsNullOrWhiteSpace(user) ? "system" : user.Trim();
        return normalized.Length <= 30 ? normalized : normalized[..30];
    }
}