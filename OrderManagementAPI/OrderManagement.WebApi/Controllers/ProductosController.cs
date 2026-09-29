using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Productos;

namespace OrderManagement.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/productos")]
public sealed class ProductosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductoDto>>> GetAll(CancellationToken cancellationToken)
    {
        var productos = await mediator.Send(new ListarProductosQuery(), cancellationToken);
        return Ok(productos);
    }
}
