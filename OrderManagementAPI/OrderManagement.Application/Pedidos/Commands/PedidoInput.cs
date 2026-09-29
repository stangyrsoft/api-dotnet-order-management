namespace OrderManagement.Application.Pedidos.Commands;

public sealed record PedidoDetalleInput(long ProductoId, int Cantidad);

public sealed record PedidoInput(
    string NumeroPedido,
    string Estado,
    string? Observacion,
    long ClienteId,
    DateTime FechaPedido,
    IReadOnlyList<PedidoDetalleInput>? Detalles);