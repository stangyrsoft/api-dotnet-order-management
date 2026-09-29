namespace OrderManagement.Application.Pedidos.Dtos;

public sealed record PedidoDetalleDto(
    long ProductoId,
    string? ProductoNombre,
    int Cantidad,
    decimal PrecioUnitario);

public sealed record PedidoClienteDto(
    long Id,
    string Dni,
    string Nombre,
    string? Apellido,
    string? Direccion);

public sealed record PedidoDto(
    long Id,
    string NumeroPedido,
    string Estado,
    string? Observacion,
    long ClienteId,
    string? ClienteDni,
    string? ClienteNombre,
    DateTime FechaPedido,
    decimal TotalImporte,
    IReadOnlyList<PedidoDetalleDto> Detalles,
    PedidoClienteDto? Cliente);