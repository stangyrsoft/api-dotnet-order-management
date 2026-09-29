namespace OrderManagement.Application.Pedidos.Commands;

public sealed record PedidoDetalleInput(long ProductoId, int Cantidad);

public sealed record ClienteInput(
    string Dni,
    string Nombre,
    string? Apellido,
    string? Direccion);

public sealed record PedidoInput(
    string NumeroPedido,
    string Estado,
    string? Observacion,
    ClienteInput Cliente,
    DateTime FechaPedido,
    IReadOnlyList<PedidoDetalleInput>? Detalles);