using FluentValidation;

namespace OrderManagement.Application.Pedidos.Commands;

public sealed class PedidoInputValidator : AbstractValidator<PedidoInput>
{
    public PedidoInputValidator()
    {
        RuleFor(pedido => pedido.NumeroPedido)
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(pedido => pedido.Estado)
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(pedido => pedido.Cliente)
            .NotNull()
            .SetValidator(new ClienteInputValidator());
        RuleFor(pedido => pedido.FechaPedido).NotEmpty();
        RuleFor(pedido => pedido.Detalles)
            .NotNull()
            .NotEmpty()
            .Must(detalles => detalles is not null &&
                detalles.Select(detalle => detalle.ProductoId).Distinct().Count() == detalles.Count)
            .WithMessage("El pedido requiere detalles y no puede repetir productos.");
        RuleForEach(pedido => pedido.Detalles!)
            .ChildRules(detalle =>
            {
                detalle.RuleFor(item => item.ProductoId).GreaterThan(0);
                detalle.RuleFor(item => item.Cantidad).GreaterThan(0);
            });
    }
}

public sealed class ClienteInputValidator : AbstractValidator<ClienteInput>
{
    public ClienteInputValidator()
    {
        RuleFor(cliente => cliente.Dni)
            .NotEmpty()
            .Matches("^[0-9]{8}$")
            .WithMessage("El DNI debe contener exactamente 8 dígitos.");
        RuleFor(cliente => cliente.Nombre)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(cliente => cliente.Apellido).MaximumLength(100);
    }
}

public sealed class CrearPedidoCommandValidator : AbstractValidator<CrearPedidoCommand>
{
    public CrearPedidoCommandValidator()
    {
        RuleFor(command => command.Pedido).SetValidator(new PedidoInputValidator());
    }
}

public sealed class ActualizarPedidoCommandValidator : AbstractValidator<ActualizarPedidoCommand>
{
    public ActualizarPedidoCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.Pedido).SetValidator(new PedidoInputValidator());
    }
}

public sealed class EliminarPedidoCommandValidator : AbstractValidator<EliminarPedidoCommand>
{
    public EliminarPedidoCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
    }
}