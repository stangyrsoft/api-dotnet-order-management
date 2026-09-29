using OrderManagement.Domain.Models;

namespace OrderManagement.Application.Pedidos.Dtos;

internal static class PedidoDtoMapper
{
    public static PedidoDto Map(Pedido pedido)
    {
        var clienteNombre = pedido.Cliente is null
            ? null
            : string.Join(' ', new[] { pedido.Cliente.Nombre, pedido.Cliente.Apellido }
                .Where(value => !string.IsNullOrWhiteSpace(value)));

        return new PedidoDto(
            pedido.Id,
            pedido.NumeroPedido ?? string.Empty,
            pedido.Estado ?? string.Empty,
            pedido.Observacion,
            pedido.ClienteId,
            clienteNombre,
            pedido.FechaPedido,
            pedido.TotalImporte,
            pedido.Detalles
                .Select(detalle => new PedidoDetalleDto(
                    detalle.ProductoId,
                    detalle.Producto?.Nombre,
                    detalle.Cantidad,
                    detalle.PrecioUnitario))
                .ToArray());
    }
}