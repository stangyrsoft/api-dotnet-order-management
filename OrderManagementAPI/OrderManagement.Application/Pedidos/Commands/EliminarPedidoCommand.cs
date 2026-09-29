using MediatR;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Domain.Repositories;

namespace OrderManagement.Application.Pedidos.Commands;

public sealed record EliminarPedidoCommand(long Id, string UsuarioAuditoria) : IRequest<Unit>;

public sealed class EliminarPedidoCommandHandler(
    IPedidoRepository pedidos,
    IUnitOfWork unitOfWork) : IRequestHandler<EliminarPedidoCommand, Unit>
{
    public async Task<Unit> Handle(EliminarPedidoCommand request, CancellationToken cancellationToken)
    {
        var pedido = await pedidos.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"No existe un pedido con ID {request.Id}.");

        if (!pedido.Activo)
        {
            return Unit.Value;
        }

        pedido.Activo = false;
        pedido.UsuarioModificacion = CrearPedidoCommandHandler.NormalizeAuditUser(request.UsuarioAuditoria);
        pedido.FechaModificacion = DateTime.UtcNow;
        pedidos.Update(pedido);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}