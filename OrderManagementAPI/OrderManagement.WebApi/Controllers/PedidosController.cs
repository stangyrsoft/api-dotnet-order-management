using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Pedidos.Commands;
using OrderManagement.Application.Pedidos.Dtos;
using OrderManagement.Application.Pedidos.Queries;

namespace OrderManagement.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/pedidos")]
public sealed class PedidosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PedidoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PedidoDto>>> GetAll(CancellationToken cancellationToken)
    {
        var pedidos = await mediator.Send(new ListarPedidosQuery(), cancellationToken);
        return Ok(pedidos);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PedidoDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var pedido = await mediator.Send(new ObtenerPedidoPorIdQuery(id), cancellationToken);
        return Ok(pedido);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PedidoDto>> Create(
        [FromBody] PedidoInput request,
        CancellationToken cancellationToken)
    {
        var id = await mediator.Send(
            new CrearPedidoCommand(request, AuditUser),
            cancellationToken);
        var pedido = await mediator.Send(new ObtenerPedidoPorIdQuery(id), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, pedido);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] PedidoInput request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(
            new ActualizarPedidoCommand(id, request, AuditUser),
            cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await mediator.Send(new EliminarPedidoCommand(id, AuditUser), cancellationToken);
        return NoContent();
    }

    private string AuditUser => User.Identity?.Name ?? "system";
}