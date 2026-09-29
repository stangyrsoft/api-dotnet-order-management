namespace OrderManagement.Application.Pedidos.Dtos;

public sealed record PedidoDetalleDto(
    long ProductoId,
    string? ProductoNombre,
    int Cantidad,
    decimal PrecioUnitario);

public sealed record PedidoDto(
    long Id,
    string NumeroPedido,
    string Estado,
    string? Observacion,
    long ClienteId,
    string? ClienteNombre,
    DateTime FechaPedido,
    decimal TotalImporte,
    IReadOnlyList<PedidoDetalleDto> Detalles);